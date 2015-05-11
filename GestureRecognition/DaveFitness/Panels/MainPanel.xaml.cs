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

namespace DaveFitness.Panels {
  public partial class MainPanel : UserControl {
    public MainPanel() {
      InitializeComponent();
    }


    public SpeechRecognitionManager SpeechManager {
      set {
        speechManager = value;
        speechManager.RecognizedCommandEventHandler += RecognizedCommand;
      }
    }

    private void RecognizedCommand(object sender, RecognizedCommandEventArgs e) {
      ClearLabelsBackground();

      switch (e.RecognizedCommand) {
        case "exercise":
          ClearLabelsBackground();
          HighlightLabel(ExerciseLbl);
          break;
        case "train":
          ClearLabelsBackground();
          HighlightLabel(TrainLbl);
          break;
        case "test":
          ClearLabelsBackground();
          HighlightLabel(TestLbl);
          break;
      }
    }


    private void ClearLabelsBackground() {
      ClearLabelBackground(ExerciseLbl);
      ClearLabelBackground(TrainLbl);
      ClearLabelBackground(TestLbl);
    }

    private void ClearLabelBackground(Label label) {
      SolidColorBrush brush = new SolidColorBrush(Colors.White);
      brush.Opacity = 0.0;
      label.Background = brush;
    }

    private void HighlightLabel(Label label) {
      SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF88FF00"));
      label.Background = brush;
    }

    private void ExerciseLblMouseMove(object sender, MouseEventArgs e) {
      HighlightLabel(ExerciseLbl);
    }

    private void ExerciseLblMouseLeave(object sender, MouseEventArgs e) {
      ClearLabelBackground(ExerciseLbl);
    }

    private void ExerciseLblMouseClick(object sender, MouseButtonEventArgs e) {
      Console.WriteLine("click on exercise");
    }

    private void TrainLblMouseMove(object sender, MouseEventArgs e) {
      HighlightLabel(TrainLbl);
    }

    private void TrainLblMouseLeave(object sender, MouseEventArgs e) {
      ClearLabelBackground(TrainLbl);
    }

    private void TrainLblMouseClick(object sender, MouseButtonEventArgs e) {
      Console.WriteLine("click on train");
    }

    private void TestLblMouseMove(object sender, MouseEventArgs e) {
      HighlightLabel(TestLbl);
    }

    private void TestLblMouseLeave(object sender, MouseEventArgs e) {
      ClearLabelBackground(TestLbl);
    }

    private void TestLblMouseClick(object sender, MouseButtonEventArgs e) {
      Console.WriteLine("click on test");
    }


    private SpeechRecognitionManager speechManager; 
  }
}
