using DaveFitness.Events;
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
      LoadDatabase();
      UpdateGestureList(gestureIndex.GetAllGestures());
      countdownTimer.CountdownEventHandler += CountdownEventHandler;
    }

    private void CountdownEventHandler(object sender, CountdownEventArgs e) {
      Console.WriteLine("done");
      //gestureRecorder.RecordInitialPosition(false);
    }

    public BodyManager BodyManager { set { bodyManager = value; } }

    public KinectManager KinectManager {
      set {
        kinectManager = value;
        videoStream.KinectManager = value;
      }
    }

    public void ExecuteVoiceCommand(VoiceCommand command) {
      switch (command) {
        case VoiceCommand.Start:
          if (bodyManager != null) {
            countdownTimer.ResetTimer();
            countdownTimer.StartCountdown();
            //initialPositionComputer = new InitialPositionValidator(bodyManager);
            //gestureRecorder = new GestureRecorder(bodyManager, initialPositionComputer, gestureIndex.GestureDB[gestureIndex.NewGesture].fileName);
            //gestureRecorder.GestureRecordEventHandler += GestureRecordEventHandler;
            //gestureRecorder.RecordInitialPosition(true);
            //StartRecordingTimer();
          }
          break;
        case VoiceCommand.Stop:
          break;
      }
    }

    private void UpdateGestureList(List<string> gestures) {
      gestureList.Items.Clear();
      foreach (string gesture in gestures) {
        gestureList.Items.Add(gesture);
      }
    }

    private void LoadDatabase() {
      gestureIndex = new GestureIndex();
      gestureIndex.LoadDB();
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


    private void addGestureBtn_Click(object sender, RoutedEventArgs e) {
      this.Dispatcher.Invoke((Action)(() => { // update from any thread
        repetitionsLbl.Content = "0";
        countdownTimer.ResetTimer();
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
    private KinectManager kinectManager;
    private GestureIndex gestureIndex;
  }
}
