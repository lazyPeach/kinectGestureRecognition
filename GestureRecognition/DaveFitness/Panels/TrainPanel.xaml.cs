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
      AddTimeRectangles();
      LoadDatabase();
    }

    private void LoadDatabase() {
      gestureIndex = new GestureIndex();
      gestureIndex.LoadDB();
      UpdateGestureList(gestureIndex.GetAllGestures());
    }

    public BodyManager BodyManager { set { bodyManager = value; } }
    
    public KinectManager KinectManager {
      set {
        kinectManager = value;
        videoStream.KinectManager = value;
      }
    }

    private void UpdateGestureList(List<string> gestures) {
      gestureList.Items.Clear();
      foreach (string gesture in gestures) {
        gestureList.Items.Add(gesture);
      }
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

    public void ExecuteVoiceCommand(VoiceCommand command) {
      switch (command) {
        case VoiceCommand.Start:
          if (bodyManager != null) {
            initialPositionComputer = new InitialPositionValidator(bodyManager);
            gestureRecorder = new GestureRecorder(bodyManager, initialPositionComputer, gestureIndex.GestureDB[gestureIndex.NewGesture].fileName);
            gestureRecorder.GestureRecordEventHandler += GestureRecordEventHandler;
            gestureRecorder.RecordInitialPosition(true);
            StartRecordingTimer();
          }
          break;
        case VoiceCommand.Stop:
          break;
      }
    }

    private void GestureRecordEventHandler(object sender, GestureRecordEventArgs e) {
      this.Dispatcher.Invoke((Action)(() => { // update from any thread
        repetitionsLbl.Content = e.StampleNr.ToString();
      }));

      if (e.StampleNr == 5) {
        gestureIndex.SaveNewGesture();
        gestureRecorder.GestureRecordEventHandler -= GestureRecordEventHandler;
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
        timer.Elapsed -= UpdateTimerBar;
        gestureRecorder.RecordInitialPosition(false);
        return;
      }
    }

    private void addGestureBtn_Click(object sender, RoutedEventArgs e) {
      this.Dispatcher.Invoke((Action)(() => { // update from any thread
        repetitionsLbl.Content = "0";
        for (int i = 0; i < 5; i++) {
          timeRect[i].Visibility = System.Windows.Visibility.Visible;
        }
      }));
      
      if (newGestureTxt.Text == "")
        MessageBox.Show("You have to type a gesture name");

      try {
        gestureIndex.AddGesture(newGestureTxt.Text);
        UpdateGestureList(gestureIndex.GetAllGestures());
        newGestureTxt.Text = "";
        //newGestureTxt.IsEnabled = false;
      } catch (GestureAlreadyExistsException ex) {
        MessageBox.Show("Gesture '" + ex.Gesture + "' already exists");
      }
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

    private InitialPositionValidator initialPositionComputer;
    private GestureRecorder gestureRecorder;
    private BodyManager bodyManager;
    private Rectangle[] timeRect;
    private KinectManager kinectManager;
    private GestureIndex gestureIndex;
    private int countdownSec;
    private Timer timer;
  }
}
