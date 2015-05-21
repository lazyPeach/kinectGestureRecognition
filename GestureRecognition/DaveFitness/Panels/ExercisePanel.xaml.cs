using GestureRecognition;
using GestureRecognition.Events;
using Microsoft.Kinect;
using SkeletonModel.Events;
using SkeletonModel.Managers;
using SkeletonModel.Model;
using SkeletonModel.Util;
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

      AddTimeRectangles();

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
            RecordInitialPosition();
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
      }
    }


    private void UpdateGestureList(List<string> gestures) {
      gestureList.Items.Clear();
      foreach (string gesture in gestures) {
        gestureList.Items.Add(gesture);
      }

      gestureList.SelectedIndex = 0;
    }

    private void AddTimeRectangles() {
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

    private void PlaySelectedGesture() {
      gestureTimer = new System.Timers.Timer { Interval = 40 };
      gestureTimer.Elapsed += UpdateGesturePlayer;
      gestureTimer.Start();
    }

    private void UpdateGesturePlayer(object sender, ElapsedEventArgs e) {
      if (selectedGestureSamples != null) {
        this.Dispatcher.Invoke((Action)(() => { // update from any thread
          centerX = (int)sampleGestureCanvas.Width / 2;
          centerY = (int)sampleGestureCanvas.Height / 2; 
          DrawSampleGestureSkeleton(selectedGestureSamples[gestureSampleIndex++]);
        }));
      }

      if (gestureSampleIndex == selectedGestureSamples.Length) { // start from the beginning
        gestureSampleIndex = 0;
      }
    }

    private void DrawSampleGestureSkeleton(Body body) {
      DrawSampleGestureJoints(body.JointSkeleton);
      DrawSampleGestureBones(body.JointSkeleton);
    }

    private void DrawSampleGestureJoints(JointSkeleton jointSkeleton) {
      sampleGestureCanvas.Children.Clear();
      DrawPoint(centerX, centerY, Colors.Yellow);

      SkeletonModel.Model.Joint centerJoint = jointSkeleton.GetJoint(JointName.HipCenter);

      foreach (JointName jointType in Enum.GetValues(typeof(JointName))) {
        SkeletonModel.Model.Joint joint = jointSkeleton.GetJoint(jointType);

        if (joint == null) continue;

        double x = joint.XCoord - centerJoint.XCoord;
        double y = joint.YCoord - centerJoint.YCoord;

        DrawPoint(centerX + x * 150, centerY - y * 150, Colors.Yellow); // have a mirror display
      }
    }

    private void DrawSampleGestureBones(JointSkeleton jointSkeleton) {
      SkeletonModel.Model.Joint centerJoint = jointSkeleton.GetJoint(JointName.HipCenter);

      foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
        Tuple<JointName, JointName> boneExtremities = Mapper.BoneJointMap[boneName];
        SkeletonModel.Model.Joint startJoint = jointSkeleton.GetJoint(boneExtremities.Item1);
        SkeletonModel.Model.Joint endJoint = jointSkeleton.GetJoint(boneExtremities.Item2);

        if (startJoint == null || endJoint == null) continue;

        double x1 = startJoint.XCoord - centerJoint.XCoord;
        double y1 = startJoint.YCoord - centerJoint.YCoord;
        double x2 = endJoint.XCoord - centerJoint.XCoord;
        double y2 = endJoint.YCoord - centerJoint.YCoord;

        DrawLine(centerX + x1 * 150, centerY - y1 * 150, centerX + x2 * 150, centerY - y2 * 150, Colors.Yellow);
      }
    }

    private void DrawPoint(double x, double y, Color color) {
      Ellipse point = new Ellipse {
        Width = 7,
        Height = 7,
        Fill = new SolidColorBrush(color)
      };

      Canvas.SetLeft(point, x - point.Width / 2);
      Canvas.SetTop(point, y - point.Height / 2);
      sampleGestureCanvas.Children.Add(point);
    }

    private void DrawLine(double x1, double y1, double x2, double y2, Color color) {
      Line myLine = new Line();
      myLine.Stroke = new SolidColorBrush(color);
      myLine.X1 = x1;
      myLine.X2 = x2;
      myLine.Y1 = y1;
      myLine.Y2 = y2;
      myLine.HorizontalAlignment = HorizontalAlignment.Left;
      myLine.VerticalAlignment = VerticalAlignment.Top;
      myLine.StrokeThickness = 5;
      sampleGestureCanvas.Children.Add(myLine);
    }

    private void RecordInitialPosition() {
      initialPositionComputer.Record = true;  // start recording
    }

    private void StopRecordingInitialPosition() {
      initialPositionComputer.Record = false; // stop recording
      bodyManager.ClearBodyData();            // clear from body manager what is not necessary anymore
      initialPositionComputer.ComputeInitialPosition();
      initialPositionComputer.InitialPositionEventHandler += InitialPositionEventHandler;
    }

    private void StartRecordingTimer() {
      countdownSec = 0;
      initialPositionTimer = new System.Timers.Timer { Interval = 1000 };
      initialPositionTimer.Elapsed += UpdateTimerBar;
      initialPositionTimer.Start();
    }

    private void UpdateTimerBar(object sender, ElapsedEventArgs e) {
      this.Dispatcher.Invoke((Action)(() => { // update from any thread
        timeRect[countdownSec++].Visibility = System.Windows.Visibility.Hidden;
      }));

      if (countdownSec == 5) {
        initialPositionTimer.Elapsed -= UpdateTimerBar;
        initialPositionTimer.Stop();
        StopRecordingInitialPosition();
        return;
      }
    }

    // move this in gesture detector?
    private void InitialPositionEventHandler(object sender, InitialPositionEventArgs e) {
      if (e.PositionState == InitialPositionState.Enter) {
        if (bodyManager.RecordedData.Count > 50) { // consider each gesture with less than 50 samples incorrect
          // change this such that each gesture performed by user to be saved in a separate file; maybe append the time at which the gesture ended
          string gestureName = (string)gestureList.SelectedItem;
          if (gestureDetector.IsCorrectGesture(gestureName, bodyManager.RecordedDataAsArray)) {
            Console.WriteLine("correct gesture");
            // update the score
          } else {
            Console.WriteLine("incorrect gesture");
            PlotFeedback(gestureDetector.ClosestSample.RecordedDataAsArray, bodyManager.RecordedDataAsArray);
          }
        }
      }

      if (e.PositionState == InitialPositionState.Exit) {
        // record the new gesture -> clear body data and don't do anything else. bodymanager pushes
        // any new body sample into a queue so if one clears the body data when the user is no more
        // in initial position, when it will enter back into the initial position, body manager will
        // contain only the last performed gesture
        bodyManager.ClearBodyData();
      }
    }


    // use the same canvas on which the sample gesture is drawn to draw the feedback
    private void PlotFeedback(Body[] reference, Body[] record) {
      referenceGesture = reference;
      recordedGesture = record;
      recordIndex = 0;
      referenceIndex = 0;

      PauseGestureDetection();

      this.Dispatcher.Invoke((Action)(() => { // update from any thread
        sampleGestureCanvas.Height = 580;
        sampleGestureCanvas.Width = 640;
        PlayFeedback();
      }));
    }

    private void PauseGestureDetection() {
      gestureTimer.Elapsed -= UpdateGesturePlayer;
      gestureTimer.Stop();
      initialPositionComputer.InitialPositionEventHandler -= InitialPositionEventHandler;
    }

    private void ResumeGestureDetection() {
      gestureTimer.Elapsed += UpdateGesturePlayer;
      gestureTimer.Start();
      initialPositionComputer.InitialPositionEventHandler += InitialPositionEventHandler;
    }

    private void PlayFeedback() {
      feedbackTimer = new System.Timers.Timer { Interval = 40 };
      feedbackTimer.Elapsed += UpdateFeedback;
      feedbackTimer.Start();
    }

    private void UpdateFeedback(object sender, ElapsedEventArgs e) {
      if (recordedGesture != null && referenceGesture != null) {
        this.Dispatcher.Invoke((Action)(() => { // update from any thread
          DrawFeedbackSkeleton(referenceGesture[referenceIndex++], recordedGesture[recordIndex++]);
        }));
      }

      // play last sample until both are at the end
      if (recordIndex == recordedGesture.Length && referenceIndex == referenceGesture.Length) {
        feedbackTimer.Elapsed -= UpdateFeedback;
        feedbackTimer.Stop();
        this.Dispatcher.Invoke((Action)(() => { // update from any thread
          sampleGestureCanvas.Height = 360;
          sampleGestureCanvas.Width = 480;
          ResumeGestureDetection();
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

      DrawPoint(centerX, centerY, Colors.Yellow);
      DrawPoint(centerX, centerY, Colors.Red);

      foreach (JointName jointType in Enum.GetValues(typeof(JointName))) {
        SkeletonModel.Model.Joint joint = reference.JointSkeleton.GetJoint(jointType);

        if (joint == null) continue;

        double x = joint.XCoord - centerReferenceJoint.XCoord;
        double y = joint.YCoord - centerReferenceJoint.YCoord;

        DrawPoint(centerX + x * 200, centerY - y * 200, Colors.Yellow);
      }

      foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
        Tuple<JointName, JointName> boneExtremities = Mapper.BoneJointMap[boneName];
        SkeletonModel.Model.Joint startJoint = reference.JointSkeleton.GetJoint(boneExtremities.Item1);
        SkeletonModel.Model.Joint endJoint = reference.JointSkeleton.GetJoint(boneExtremities.Item2);

        if (startJoint == null || endJoint == null) continue;

        double x1 = startJoint.XCoord - centerReferenceJoint.XCoord;
        double y1 = startJoint.YCoord - centerReferenceJoint.YCoord;
        double x2 = endJoint.XCoord - centerReferenceJoint.XCoord;
        double y2 = endJoint.YCoord - centerReferenceJoint.YCoord;
        
        DrawLine(centerX + x1 * 200, centerY - y1 * 200, centerX + x2 * 200, centerY - y2 * 200, Colors.Yellow);
      }

      foreach (JointName jointType in Enum.GetValues(typeof(JointName))) {
        SkeletonModel.Model.Joint joint = record.JointSkeleton.GetJoint(jointType);

        if (joint == null) continue;

        double x = joint.XCoord - centerRecordJoint.XCoord;
        double y = joint.YCoord - centerRecordJoint.YCoord;

        DrawPoint(centerX + x * 200, centerY - y * 200, Colors.Red);
      }

      foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
        Tuple<JointName, JointName> boneExtremities = Mapper.BoneJointMap[boneName];
        SkeletonModel.Model.Joint startJoint = record.JointSkeleton.GetJoint(boneExtremities.Item1);
        SkeletonModel.Model.Joint endJoint = record.JointSkeleton.GetJoint(boneExtremities.Item2);

        if (startJoint == null || endJoint == null) continue;

        double x1 = startJoint.XCoord - centerRecordJoint.XCoord;
        double y1 = startJoint.YCoord - centerRecordJoint.YCoord;
        double x2 = endJoint.XCoord - centerRecordJoint.XCoord;
        double y2 = endJoint.YCoord - centerRecordJoint.YCoord;

        DrawLine(centerX + x1 * 200, centerY - y1 * 200, centerX + x2 * 200, centerY - y2 * 200, Colors.Red);
      }

    }

    private void ColorFrameHandler(object sender, KinectColorFrameEventArgs e) {
      if (pixels == null) {
        pixels = new byte[e.ImageFrame.PixelDataLength];
      }

      e.ImageFrame.CopyPixelDataTo(pixels);

      // A WriteableBitmap is a WPF construct that enables resetting the Bits of the image.
      // This is more efficient than creating a new Bitmap every frame.
      if (cameraSource == null) {
        cameraSource = new WriteableBitmap(e.ImageFrame.Width, e.ImageFrame.Height, 96, 96, PixelFormats.Bgr32, null);
      }

      int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
      cameraSource.WritePixels(new Int32Rect(0, 0, e.ImageFrame.Width, e.ImageFrame.Height),
            pixels, e.ImageFrame.Width * Bgr32BytesPerPixel, 0);

      cameraFrame.Source = cameraSource;
    }

    private void SkeletonEventHandler(object sender, KinectSkeletonEventArgs e) {
      KinectManager manager = sender as KinectManager;
      KinectSensor sensor = manager.Sensor;
      DrawSkeletonOnVideo(new CoordinateMapper(sensor), e.Skeleton);
    }

    private void DrawSkeletonOnVideo(CoordinateMapper coordinateMapper, Skeleton skeleton) {
      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();

      foreach (Microsoft.Kinect.Joint joint in skeleton.Joints) {
        ColorImagePoint colorImagePoint = coordinateMapper.MapSkeletonPointToColorPoint(
          joint.Position, ColorImageFormat.RgbResolution640x480Fps30);

        Point point = new Point(colorImagePoint.X, colorImagePoint.Y);
        drawingContext.DrawEllipse(new SolidColorBrush(Colors.Red), new Pen(new SolidColorBrush(Colors.White), 3), point, 7, 7);
      }

      drawingContext.Close();
      RenderTargetBitmap renderBmp = new RenderTargetBitmap(640, 480, 96d, 96d, PixelFormats.Pbgra32);
      renderBmp.Render(drawingVisual);
      skeletonFrame.Source = renderBmp;
    }

    // display a sample of the gesture that should be performed by the user
    private void gestureList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      gestureSampleIndex = 0;
      selectedGestureSamples = GetSelectedGestureSample();
    }

    private Body[] GetSelectedGestureSample() {
      try {
        string gestureName = (string)gestureList.SelectedItem;
        string gestureFileName = @"..\..\..\..\database\" + gestureIndex.GestureDB[gestureName].fileName + "0.xml";

        // load the gesture
        BodyManager b = new BodyManager();
        b.LoadBodyData(gestureFileName);

        return b.RecordedDataAsArray;
      } catch (Exception ex) {
        Console.WriteLine(ex.Message);
        return null;
      }
    }


    private BodyManager bodyManager;
    private KinectManager kinectManager;
    private GestureIndex gestureIndex;
    private GestureDetector gestureDetector;
    private InitialPositionComputer initialPositionComputer;
    private Body[] referenceGesture;
    private Body[] recordedGesture;
    private Body[] selectedGestureSamples;

    private Rectangle[] timeRect;
    private Timer gestureTimer;
    private Timer initialPositionTimer;
    private Timer feedbackTimer;
    private WriteableBitmap cameraSource;

    private int centerX;
    private int centerY;
    private int gestureSampleIndex;
    private int countdownSec;
    private int recordIndex;
    private int referenceIndex;
    private byte[] pixels;
  }
}
