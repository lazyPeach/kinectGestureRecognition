using DaveFitness.Events;
using GestureRecognition;
using GestureRecognition.Events;
using Microsoft.Kinect;
using SkeletonModel.Events;
using SkeletonModel.Managers;
using SkeletonModel.Model;
using SkeletonModel.Util;
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
  public partial class ExercisePanel : UserControl {
    public ExercisePanel() {
      InitializeComponent();
      InitializeGestureList();
      countdownTimer.CountdownEventHandler += CountdownEventHandler;

      //gestureDetector = new GestureDetector(gestureIndex);

      //PlaySelectedGesture();
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

    private void CountdownEventHandler(object sender, CountdownEventArgs e) {
      gestureManager.StopRecordingInitialPosition();
      string gestureName = "";
      this.Dispatcher.Invoke((Action)(() => { // update from any thread
        gestureName = (string)gestureList.SelectedItem;
      }));
       
      gestureManager.StartValidateGesture(gestureName);
      //gestureManager.GestureRecordEventHandler += GestureRecordEventHandler;
    }

    public void ExecuteVoiceCommand(VoiceCommand command) {
      switch (command) {
        case VoiceCommand.Start:
          gestureManager.GestureRecognizeEventHandler -= GestureRecognizeEventHandler;

          if (bodyManager != null) {
            countdownTimer.ResetTimer();
            countdownTimer.StartCountdown();
            gestureManager.StartRecordingInitialPosition();
            gestureManager.GestureRecognizeEventHandler += GestureRecognizeEventHandler;
            // fa subscribe la gesture recognized
          }
          break;
        case VoiceCommand.Stop:
          gestureManager.GestureRecognizeEventHandler -= GestureRecognizeEventHandler;
          gestureManager.StopValidateGesture();
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

    private int correctExecutions = 0;

    private void GestureRecognizeEventHandler(object sender, GestureRecognizeEventArgs e) {
      if (e.IsGestureRecognized) {
        correctExecutions++;
        this.Dispatcher.Invoke((Action)(() => { // update from any thread
          repetitionsLbl.Content = correctExecutions.ToString();
        }));
      } else {
        feedbackPlayer.PlayFeedback(gestureManager);//.GestureDetector.RecordSample, gestureManager.GestureDetector.ReferenceSample);
        //while(feedbackPlayer.IsFeedbackPlaying);
      }
    }

    private void InitializeGestureList() {
      GestureIndex tempGestureIndex = new GestureIndex(); // figure out how to get rid of this too
      tempGestureIndex.LoadDB();
      UpdateGestureList(tempGestureIndex.GetAllGestures());
      gestureList.SelectedIndex = 0;
    }

    private void UpdateGestureList(List<string> gestures) {
      gestureList.Items.Clear();
      foreach (string gesture in gestures) {
        gestureList.Items.Add(gesture);
      }
    }


    private void gestureList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      gesturePlayer.GestureSamples = GetSelectedGestureSample();
    }

    private Body[] GetSelectedGestureSample() {
      try {
        string gestureName = (string)gestureList.SelectedItem;
        return gestureManager.GetGestureSample(gestureName);
      } catch (Exception ex) {
        Console.WriteLine(ex.Message);
        return null;
      }
    }


    private BodyManager bodyManager;
    private KinectManager kinectManager;
    private GestureManager gestureManager;
  }
}
