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
      InitializeGestureList();
      countdownTimer.CountdownEventHandler += CountdownEventHandler;
    }

    private void CountdownEventHandler(object sender, CountdownEventArgs e) {
      gestureManager.StopRecordingInitialPosition();
      gestureManager.StartRecordGesture();
      gestureManager.GestureRecordEventHandler += GestureRecordEventHandler;
    }

    private void GestureRecordEventHandler(object sender, GestureRecordEventArgs e) {
      this.Dispatcher.Invoke((Action)(() => { // update from any thread
        repetitionsLbl.Content = e.StampleNr.ToString();
      }));
    }

    public BodyManager BodyManager { 
      set { 
        bodyManager = value;
        gestureManager = new GestureManager(value);
      } 
    }

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
            gestureManager.StartRecordingInitialPosition();
          }
          break;
        case VoiceCommand.Stop: // do some cancel here
          break;
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
        gestureManager.AddGesture(newGestureTxt.Text);
        UpdateGestureList(gestureManager.GetGesturesList());
        newGestureTxt.Text = "";
      } catch (GestureAlreadyExistsException ex) {
        MessageBox.Show("Gesture '" + ex.Gesture + "' already exists");
      }
    }

    private void removeGestureBtn_Click(object sender, RoutedEventArgs e) {
      gestureManager.RemoveGesture((string)gestureList.SelectedItem);      
      UpdateGestureList(gestureManager.GetGesturesList());
    }

    private void NewGestureTxtGotFocus(object sender, RoutedEventArgs e) {
      if (newGestureTxt.Text == "gesture name") {
        newGestureTxt.Text = "";
        newGestureTxt.Foreground = new SolidColorBrush(Colors.Black);
      }
    }

    private void InitializeGestureList() {
      GestureIndex tempGestureIndex = new GestureIndex(); // figure out how to get rid of this too
      tempGestureIndex.LoadDB();
      UpdateGestureList(tempGestureIndex.GetAllGestures());
    }

    private void UpdateGestureList(List<string> gestures) {
      gestureList.Items.Clear();
      foreach (string gesture in gestures) {
        gestureList.Items.Add(gesture);
      }
    }


    private BodyManager bodyManager;
    private KinectManager kinectManager;
    private GestureManager gestureManager;
  }
}
