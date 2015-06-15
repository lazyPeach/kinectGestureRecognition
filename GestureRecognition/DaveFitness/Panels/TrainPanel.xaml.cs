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
        gestureManager.AddNewGesture(newGestureTxt.Text);
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

    private BodyManager bodyManager;
    private KinectManager kinectManager;
    private GestureIndex gestureIndex;
    private GestureManager gestureManager;
  }
}
