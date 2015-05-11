using GestureRecognition.Managers;
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

    private KinectManager kinectManager;
    private SpeechRecognitionManager speechRecognitionManager;
  }
}
