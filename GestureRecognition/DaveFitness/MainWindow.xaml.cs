using DaveFitness.Events;
using SkeletonModel.Managers;
using SpeechRecognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace DaveFitness {
  public partial class MainWindow : Window {
    public MainWindow() {
      InitializeComponent();
      this.Closed += ClosedWindow;


      kinectManager = new KinectManager();
      kinectManager.Start();

      speechRecognitionManager = new SpeechRecognitionManager(kinectManager.Sensor);
      speechRecognitionManager.RecognizedCommandEventHandler += RecognizedCommand;

      mainPanel.SpeechManager = speechRecognitionManager;
      mainPanel.CommandEventHandler += MainPanelCommand;

      trainPanel.KinectManager = kinectManager;
    }

    private void ClosedWindow(object sender, System.EventArgs e) {
      kinectManager.Stop();
    }

    private void RecognizedCommand(object sender, RecognizedCommandEventArgs e) {
      switch (e.RecognizedCommand) {
        case "exercise" :
          break;
        case "train" :
          break;
        case "test" :
          break;
      }
    }

    private void MainPanelCommand(object sender, CommandEventArgs e) {
      switch (e.Command) {
        case Command.Exercise:
          exercisePanel.Visibility = System.Windows.Visibility.Visible;
          trainPanel.Visibility = System.Windows.Visibility.Hidden;
          testPanel.Visibility = System.Windows.Visibility.Hidden;
          break;
        case Command.Train:
          exercisePanel.Visibility = System.Windows.Visibility.Hidden;
          trainPanel.Visibility = System.Windows.Visibility.Visible;
          testPanel.Visibility = System.Windows.Visibility.Hidden;
          break;
        case Command.Test:
          exercisePanel.Visibility = System.Windows.Visibility.Hidden;
          trainPanel.Visibility = System.Windows.Visibility.Hidden;
          testPanel.Visibility = System.Windows.Visibility.Visible;
          break;
      }
    }

    private KinectManager kinectManager;
    private SpeechRecognitionManager speechRecognitionManager;
  }
}
