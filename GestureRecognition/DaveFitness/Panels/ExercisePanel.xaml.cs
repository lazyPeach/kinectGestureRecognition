using GestureRecognition;
using GestureRecognition.Events;
using Microsoft.Kinect;
using SkeletonModel.Events;
using SkeletonModel.Managers;
using SkeletonModel.Model;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DaveFitness.Panels {
  public partial class ExercisePanel : UserControl {
    public ExercisePanel() {
      InitializeComponent();

      gestureIndex = new GestureIndex();
      gestureIndex.LoadDB();
      gestureDetector = new GestureDetector(gestureIndex);

      UpdateGestureList(gestureIndex.GetAllGestures());
      gestureList.SelectedIndex = 0;

      AddRectangles();

      centerX = (int)sampleGestureCanvas.Width / 2;
      centerY = (int)sampleGestureCanvas.Height / 2;
      PlaySelectedGesture();
    }

    public BodyManager BodyManager { set { bodyManager = value; } }

    public KinectManager KinectManager {
      set {
        kinectManager = value;
        kinectManager.KinectColorFrameEventHandler += ColorFrameHandler;
        kinectManager.KinectSkeletonEventHandler += SkeletonEventHandler;
      }
    }


    public void ExecuteVoiceCommand(VoiceCommand command) {
      switch (command) {
        case VoiceCommand.Start:
          if (bodyManager != null) {
            initialPositionComputer = new InitialPositionComputer(bodyManager);
            RecordInitialPosition(true);
            StartRecordingTimer();
          }
          break;
        case VoiceCommand.Up:
          if (gestureList.SelectedIndex > 0)
            gestureList.SelectedIndex--;
          break;
        case VoiceCommand.Down:
          if (gestureList.SelectedIndex < gestureList.Items.Count)
            gestureList.SelectedIndex++;
          break;
        case VoiceCommand.Select: //TODO
          //Console.WriteLine("select");
          //PlotFeedback();
          break;
      }
    }

    // move this in gesture detector?
    private void InitialPositionEventHandler(object sender, InitialPositionEventArgs e) {
      if (e.PositionState == InitialPositionState.Enter) {
        // check if correct gesture -> send to GestureDetector the gesture name, the gestureIndex
        // and the data accumulated in the body manager.
        Console.WriteLine("enter init pos");
        if (bodyManager.RecordedData.Count > 50) { // consider each gesture with less than 50 samples incorrect
          // change this such that each gesture performed by user to be saved in a separate file
          // maybe append the time at which the gesture ended
          string gestureName = (string)gestureList.SelectedItem;
          if (gestureDetector.IsCorrectGesture(gestureName, bodyManager.RecorderDataAsArray)) {
            Console.WriteLine("correct gesture");
          } else {
            Console.WriteLine("incorrect gesture");
            PlotFeedback(gestureDetector.ClosestSample.RecorderDataAsArray, bodyManager.RecorderDataAsArray);
            // now you should give feedback... you have the recorded gesture in body
          }
        }
      }

      if (e.PositionState == InitialPositionState.Exit) {
        Console.WriteLine("exit init pos");

        // record the new gesture -> clear body data and don't do anything else. bodymanager pushes
        // any new body sample into a queue so if one clears the body data when the user is no more
        // in initial position, when it will enter back into the initial position, body manager will
        // contain only the last performed gesture
        bodyManager.ClearBodyData();

      }
    }

    private void PlotFeedback(Body[] reference, Body[] record) {
      gestureTimer.Elapsed -= UpdateGesturePlayer;
      gestureTimer.Stop();
      initialPositionComputer.InitialPositionEventHandler -= InitialPositionEventHandler;
      referenceGesture = reference;
      recordedGesture = record;
      
      this.Dispatcher.Invoke((Action)(() => { // update from any thread
        sampleGestureCanvas.Height = 580;
        sampleGestureCanvas.Width = 640;
        PlayReferenceGesture();
      }));
      //gestureTimer.Start();
      //initialPositionComputer.InitialPositionEventHandler += InitialPositionEventHandler;

      Console.WriteLine("enough");

    }

    private void PlayReferenceGesture() {
      feedbackTimer = new System.Timers.Timer { Interval = 40 };
      feedbackTimer.Elapsed += UpdateFeedback;
      feedbackTimer.Start();
    }

    private int recordIndex = 0;
    private int referenceIndex = 0;
    private Timer feedbackTimer;

    private void UpdateFeedback(object sender, ElapsedEventArgs e) {
      if (recordedGesture != null && referenceGesture != null) {
        this.Dispatcher.Invoke((Action)(() => { // update from any thread
          DrawFeedbackSkeleton(recordedGesture[recordIndex++], referenceGesture[referenceIndex++]);
        }));
      }

      // play last sample until both are at the end
      if (recordIndex == recordedGesture.Length && referenceIndex == referenceGesture.Length) {
        feedbackTimer.Elapsed -= UpdateFeedback;
        feedbackTimer.Stop();
        this.Dispatcher.Invoke((Action)(() => { // update from any thread
          sampleGestureCanvas.Height = 360;
          sampleGestureCanvas.Width = 480;
          
        }));
        
        return;
      }
      
      if (recordIndex == recordedGesture.Length) {
        recordIndex--;
      }

      if (referenceIndex == referenceGesture.Length) {
        referenceIndex--;
      }
    }

    private void DrawFeedbackSkeleton(Body reference, Body record) {
      sampleGestureCanvas.Children.Clear();

      SkeletonModel.Model.Joint centerReferenceJoint = reference.JointSkeleton.GetJoint(JointName.HipCenter);
      SkeletonModel.Model.Joint centerRecordJoint = record.JointSkeleton.GetJoint(JointName.HipCenter);

      DrawPoint(centerX, centerY);
      DrawRedPoint(centerX, centerY);

      foreach (JointName jointType in Enum.GetValues(typeof(JointName))) {
        SkeletonModel.Model.Joint joint = reference.JointSkeleton.GetJoint(jointType);

        if (joint == null) continue;

        double x = joint.XCoord - centerReferenceJoint.XCoord;
        double y = joint.YCoord - centerReferenceJoint.YCoord;

        DrawPoint(centerX + x * 200, centerY - y * 200); // have a mirror display
      }

      foreach (JointName jointType in Enum.GetValues(typeof(JointName))) {
        SkeletonModel.Model.Joint joint = record.JointSkeleton.GetJoint(jointType);

        if (joint == null) continue;

        double x = joint.XCoord - centerRecordJoint.XCoord;
        double y = joint.YCoord - centerRecordJoint.YCoord;

        DrawRedPoint(centerX + x * 200, centerY - y * 200); // have a mirror display
      }
    }

    
    


    private void UpdateGestureList(List<string> gestures) {
      gestureList.Items.Clear();
      foreach (string gesture in gestures) {
        gestureList.Items.Add(gesture);
      }

      gestureList.SelectedIndex = 0;
    }

    private void AddRectangles() {
      timeRect = new Rectangle[5];
      SolidColorBrush fillBrush = new SolidColorBrush(Colors.Red);

      for (int i = 0; i < 5; i++) {
        timeRect[i] = new Rectangle();
        timeRect[i].Height = 50;
        timeRect[i].Width = 30;
        timeRect[i].Fill = fillBrush;
        timerGrid.Children.Add(timeRect[i]);
        Grid.SetColumn(timeRect[i], i);
      }
    }

    private void RecordInitialPosition(bool rec = false) {
      if (rec) {
        initialPositionComputer.Record = true;  // start recording
      } else {
        initialPositionComputer.Record = false; // stop recording
        bodyManager.ClearBodyData();
        initialPositionComputer.ComputeInitialPosition();
        initialPositionComputer.InitialPositionEventHandler += InitialPositionEventHandler;
      }
    }

    private void StartRecordingTimer() {
      countdownSec = 0;
      timer = new System.Timers.Timer { Interval = 1000 };
      timer.Elapsed += UpdateTimerBar;
      timer.Start();
    }

    private void UpdateTimerBar(object sender, ElapsedEventArgs e) {
      this.Dispatcher.Invoke((Action)(() => { // update from any thread
        timeRect[countdownSec++].Visibility = System.Windows.Visibility.Hidden;
      }));

      if (countdownSec == 5) {
        timer.Stop();
        RecordInitialPosition(false);
        return;
      }
    }

    private void ColorFrameHandler(object sender, KinectColorFrameEventArgs e) {
      if (pixels == null) {
        pixels = new byte[e.ImageFrame.PixelDataLength];
      }
      e.ImageFrame.CopyPixelDataTo(pixels);

      //// A WriteableBitmap is a WPF construct that enables resetting the Bits of the image.
      //// This is more efficient than creating a new Bitmap every frame.
      if (cameraSource == null) {
        cameraSource = new WriteableBitmap(e.ImageFrame.Width, e.ImageFrame.Height, 96, 96,
          PixelFormats.Bgr32, null);
      }

      int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
      cameraSource.WritePixels(new Int32Rect(0, 0, e.ImageFrame.Width, e.ImageFrame.Height),
            pixels, e.ImageFrame.Width * Bgr32BytesPerPixel, 0);

      cameraFrame.Source = cameraSource;
    }

    private void SkeletonEventHandler(object sender, KinectSkeletonEventArgs e) {
      KinectSensor sensor = GetKinectSensor(sender);
      DrawSkeleton(new CoordinateMapper(sensor), e.Skeleton);
    }

    private KinectSensor GetKinectSensor(object eventSender) {
      KinectManager manager = eventSender as KinectManager;
      return manager.Sensor;
    }

    private void DrawSkeleton(CoordinateMapper coordinateMapper, Skeleton skeleton) {
      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();

      foreach (Microsoft.Kinect.Joint joint in skeleton.Joints) {
        ColorImagePoint colorImagePoint = coordinateMapper.MapSkeletonPointToColorPoint(
          joint.Position, ColorImageFormat.RgbResolution640x480Fps30);

        Point point = new Point(colorImagePoint.X, colorImagePoint.Y);
        drawingContext.DrawEllipse(new SolidColorBrush(Color.FromArgb(255, 255, 255, 0)),
          null, point, 5, 5);
      }

      drawingContext.Close();
      RenderTargetBitmap renderBmp = new RenderTargetBitmap(640, 480, 96d, 96d, PixelFormats.Pbgra32);
      renderBmp.Render(drawingVisual);
      skeletonFrame.Source = renderBmp;
    }

    // display a sample of the gesture that should be performed by the user
    private void gestureList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      try {
        // select first of the samples in the database and play it
        string gestureName = (string)gestureList.SelectedItem;
        string gestureFileName = @"..\..\..\..\database\" + gestureIndex.GestureDB[gestureName].fileName + "0.xml";
        // load the gesture
        BodyManager b = new BodyManager();
        b.LoadBodyData(gestureFileName);
        gestureSampleIndex = 0;
        selectedGestureSamples = b.RecorderDataAsArray;
      } catch (Exception ex) {
        Console.WriteLine(ex.Message);
      }
    }

    private void PlaySelectedGesture() {
      gestureTimer = new System.Timers.Timer { Interval = 40 };
      gestureTimer.Elapsed += UpdateGesturePlayer;
      gestureTimer.Start();
    }

    private void UpdateGesturePlayer(object sender, ElapsedEventArgs e) {
      if (selectedGestureSamples != null) {
        this.Dispatcher.Invoke((Action)(() => { // update from any thread
          DrawSampleGestureSkeleton(selectedGestureSamples[gestureSampleIndex++]);
        }));
      }

      if (gestureSampleIndex == selectedGestureSamples.Length) {
        gestureSampleIndex = 0;
      }
    }

    private void DrawSampleGestureSkeleton(Body body) {
      DrawSampleGestureJoints(body.JointSkeleton);
    }

    private void DrawRedPoint(double x, double y) {
      Ellipse point = new Ellipse {
        Width = 5,
        Height = 5,
        Fill = new SolidColorBrush(Colors.Red)
      };

      Canvas.SetLeft(point, x - point.Width / 2);
      Canvas.SetTop(point, y - point.Height / 2);
      sampleGestureCanvas.Children.Add(point);
    }

    private void DrawPoint(double x, double y) {
      Ellipse point = new Ellipse {
        Width = 10,
        Height = 10,
        Fill = new SolidColorBrush(Colors.Yellow)
      };

      Canvas.SetLeft(point, x - point.Width / 2);
      Canvas.SetTop(point, y - point.Height / 2);
      sampleGestureCanvas.Children.Add(point);
    }

    private void DrawSampleGestureJoints(JointSkeleton jointSkeleton) {
      sampleGestureCanvas.Children.Clear();
      SkeletonModel.Model.Joint centerJoint = jointSkeleton.GetJoint(JointName.HipCenter);
      DrawPoint(centerX, centerY);

      foreach (JointName jointType in Enum.GetValues(typeof(JointName))) {
        SkeletonModel.Model.Joint joint = jointSkeleton.GetJoint(jointType);

        if (joint == null) continue;

        double x = joint.XCoord - centerJoint.XCoord;
        double y = joint.YCoord - centerJoint.YCoord;

        DrawPoint(centerX + x * 150, centerY - y * 150); // have a mirror display
      }
    }

    private int gestureSampleIndex;
    private Body[] referenceGesture;
    private Body[] recordedGesture;
    private Body[] selectedGestureSamples;
    private InitialPositionComputer initialPositionComputer;
    private GestureIndex gestureIndex;
    private BodyManager bodyManager;
    private byte[] pixels;
    private WriteableBitmap cameraSource;
    private KinectManager kinectManager;
    private int countdownSec;
    private Timer timer;
    private Timer gestureTimer;
    private Rectangle[] timeRect;
    private int centerX = 0;
    private int centerY = 0;
    private GestureDetector gestureDetector;
  }
}
