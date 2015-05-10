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
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    public MainWindow() {
      InitializeComponent();

      speechRecognitionManager = new SpeechRecognitionManager();
      speechRecognitionManager.RecognizedCommandEventHandler += RecognizedCommand;
    }

    private void RecognizedCommand(object sender, RecognizedCommandEventArgs e) {
      switch (e.RecognizedCommand) {
        case "exercise" :
          HighlightLabel(exerciseLbl);
          break;
        case "train" :
          HighlightLabel(trainLbl);
          break;
        case "test" :
          HighlightLabel(testLbl);
          break;
      }
    }

    private void HighlightLabel(Label label) {
      SolidColorBrush brush = new SolidColorBrush(Colors.LightGreen);
      label.Background = brush;
    }

    private SpeechRecognitionManager speechRecognitionManager;
  }
}
