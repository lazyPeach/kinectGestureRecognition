using GestureRecognition;
using GestureRecognition.Events;
using GestureRecognition.Exceptions;
using Microsoft.Kinect;
using SkeletonModel.Events;
using SkeletonModel.Managers;
using SpeechRecognition;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DaveFitness.Panels {
  public partial class TrainPanel : UserControl {
    public TrainPanel() {
      InitializeComponent();

      gestureIndex = new GestureIndex();
      gestureIndex.LoadDB();
      UpdateGestureList(gestureIndex.GetAllGestures());
      gestureList.SelectedIndex = 0;

      AddRectangles();
    }

    public BodyManager BodyManager { set { bodyManager = value; } }

    public SpeechRecognitionManager SpeechManager {
      set {
        speechManager = value;
        speechManager.RecognizedCommandEventHandler += RecognizedCommand;
      }
    }

    public KinectManager KinectManager {
      set {
        kinectManager = value;
        kinectManager.KinectColorFrameEventHandler += ColorFrameHandler;
        kinectManager.KinectSkeletonEventHandler += SkeletonEventHandler;
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
      SolidColorBrush fillBrush = new SolidColorBrush(Colors.Green);

      for (int i = 0; i < 5; i++) {
        timeRect[i] = new Rectangle();
        timeRect[i].Height = 50;
        timeRect[i].Width = 30;
        timeRect[i].Fill = fillBrush;
        timerGrid.Children.Add(timeRect[i]);
        Grid.SetColumn(timeRect[i], i);
      }
    }

    private void RecognizedCommand(object sender, RecognizedCommandEventArgs e) {
      switch (e.RecognizedCommand) {
        case "start":
          if (bodyManager != null) {
            initialPositionComputer = new InitialPositionComputer(bodyManager);
            initialPositionComputer.InitialPositionEventHandler += InitialPositionEventHandler;

            initialPositionComputer.Record = true;
            StartRecordingTimer();
          }
          break;
        case "back": // TODO
          Console.WriteLine("back");
          break;
        case "up":
          if (gestureList.SelectedIndex > 0)
            gestureList.SelectedIndex--;
          break;
        case "down":
          if (gestureList.SelectedIndex < gestureList.Items.Count)
            gestureList.SelectedIndex++;
          break;
        case "select": //TODO
          Console.WriteLine("select");
          break;
      }
    }

    private void InitialPositionEventHandler(object sender, InitialPositionEventArgs e) {
      Console.WriteLine("initial position");
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
        initialPositionComputer.Record = false;
        initialPositionComputer.ComputeInitialPosition();
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
      cameraSource.WritePixels( new Int32Rect(0, 0, e.ImageFrame.Width, e.ImageFrame.Height),
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

      foreach (Joint joint in skeleton.Joints) {
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

    private void addGestureBtn_Click(object sender, RoutedEventArgs e) {
      if (newGestureTxt.Text == "")
        MessageBox.Show("You have to type a gesture name");

      try {
        gestureIndex.AddGesture(newGestureTxt.Text);
      } catch (GestureAlreadyExistsException ex) {
        MessageBox.Show("Gesture '" + ex.Gesture + "' already exists");
      }

      gestureIndex.SaveDB();
      UpdateGestureList(gestureIndex.GetAllGestures());
      newGestureTxt.Text = "";
    }

    private void removeGestureBtn_Click(object sender, RoutedEventArgs e) {
      gestureIndex.RemoveGesture((string)gestureList.SelectedItem);
      gestureIndex.SaveDB();
      UpdateGestureList(gestureIndex.GetAllGestures());
    }

    private void NewGestureTxtGotFocus(object sender, RoutedEventArgs e) {
      if (newGestureTxt.Text == "gesture name") {
        newGestureTxt.Text = "";
        newGestureTxt.Foreground = new SolidColorBrush(Colors.Black);
      }
    }

    private void gestureList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      Console.WriteLine((string)gestureList.SelectedItem);
    }


    private SpeechRecognitionManager speechManager;
    private InitialPositionComputer initialPositionComputer;
    private BodyManager bodyManager;
    private Rectangle[] timeRect;
    private byte[] pixels;
    private WriteableBitmap cameraSource;
    private KinectManager kinectManager;
    private GestureIndex gestureIndex;
    private int countdownSec;
    private Timer timer;
  }
}
