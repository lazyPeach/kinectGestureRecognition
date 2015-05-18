using DaveFitness.Events;
using SkeletonModel.Managers;
using SpeechRecognition;
using System;
using System.Windows;


namespace DaveFitness {
  enum FocusedPanel { Main, Test, Train, Exercise }
  public enum VoiceCommand { Exercise, Train, Test, Back, Start, Up, Down, Select }

  public partial class MainWindow : Window {
    public MainWindow() {
      InitializeComponent();
      this.Closed += ClosedWindow;

      kinectManager = new KinectManager();
      kinectManager.Start();
      bodyManager = new BodyManager(kinectManager);

      speechRecognitionManager = new SpeechRecognitionManager(kinectManager.Sensor);
      speechRecognitionManager.RecognizedCommandEventHandler += RecognizedCommand;

      mainPanel.CommandEventHandler += MainPanelCommand;

      trainPanel.KinectManager = kinectManager;
      trainPanel.BodyManager = bodyManager;

      exercisePanel.KinectManager = kinectManager;
      exercisePanel.BodyManager = bodyManager;

      focusedPanel = FocusedPanel.Main;
    }

    private void ClosedWindow(object sender, System.EventArgs e) {
      kinectManager.Stop();
    }

    // refactor: change pannels from here... don't expect events from main panel
    private void RecognizedCommand(object sender, RecognizedCommandEventArgs e) {
      switch (e.RecognizedCommand) {
        case "exercise":
          if (focusedPanel == FocusedPanel.Main) {
            ChangeFocus(FocusedPanel.Exercise);
          }
          break;
        case "train":
          if (focusedPanel == FocusedPanel.Main) {
            ChangeFocus(FocusedPanel.Train);
          }
          break;
        case "test":
          if (focusedPanel == FocusedPanel.Main) {
            ChangeFocus(FocusedPanel.Test);
          }
          break;
        case "back":
          focusedPanel = FocusedPanel.Main;
          exercisePanel.Visibility = System.Windows.Visibility.Hidden;
          trainPanel.Visibility = System.Windows.Visibility.Hidden;
          testPanel.Visibility = System.Windows.Visibility.Hidden;
          break;
        case "start":
          RedirectVoiceCommand(VoiceCommand.Start);
          break;
        case "up":
          RedirectVoiceCommand(VoiceCommand.Up);
          break;
        case "down":
          RedirectVoiceCommand(VoiceCommand.Down);
          break;
        case "select":
          RedirectVoiceCommand(VoiceCommand.Select);
          break;
      }
    }

    private void RedirectVoiceCommand(VoiceCommand command) {
      switch (focusedPanel) {
        case FocusedPanel.Train:
          trainPanel.ExecuteVoiceCommand(command);
          break;
        case FocusedPanel.Exercise:
          exercisePanel.ExecuteVoiceCommand(command);
          break;
      }
    }

    private void ChangeFocus(FocusedPanel newFocusedPanel) {
      focusedPanel = newFocusedPanel;
      switch (focusedPanel) {
        case FocusedPanel.Exercise:
          exercisePanel.Visibility = System.Windows.Visibility.Visible;
          trainPanel.Visibility = System.Windows.Visibility.Hidden;
          testPanel.Visibility = System.Windows.Visibility.Hidden;
          break;
        case FocusedPanel.Train:
          exercisePanel.Visibility = System.Windows.Visibility.Hidden;
          trainPanel.Visibility = System.Windows.Visibility.Visible;
          testPanel.Visibility = System.Windows.Visibility.Hidden;
          break;
        case FocusedPanel.Test:
          exercisePanel.Visibility = System.Windows.Visibility.Hidden;
          trainPanel.Visibility = System.Windows.Visibility.Hidden;
          testPanel.Visibility = System.Windows.Visibility.Visible;
          break;
        case FocusedPanel.Main:
          break;
      }
    }

    private void MainPanelCommand(object sender, CommandEventArgs e) {
      switch (e.Command) {
        case Command.Exercise:
          focusedPanel = FocusedPanel.Exercise;
          exercisePanel.Visibility = System.Windows.Visibility.Visible;
          trainPanel.Visibility = System.Windows.Visibility.Hidden;
          testPanel.Visibility = System.Windows.Visibility.Hidden;
          break;
        case Command.Train:
          focusedPanel = FocusedPanel.Train;
          exercisePanel.Visibility = System.Windows.Visibility.Hidden;
          trainPanel.Visibility = System.Windows.Visibility.Visible;
          testPanel.Visibility = System.Windows.Visibility.Hidden;
          break;
        case Command.Test:
          focusedPanel = FocusedPanel.Test;
          exercisePanel.Visibility = System.Windows.Visibility.Hidden;
          trainPanel.Visibility = System.Windows.Visibility.Hidden;
          testPanel.Visibility = System.Windows.Visibility.Visible;
          break;
      }
    }

    private KinectManager kinectManager;
    private BodyManager bodyManager;
    private SpeechRecognitionManager speechRecognitionManager;
    private FocusedPanel focusedPanel;
  }
}
