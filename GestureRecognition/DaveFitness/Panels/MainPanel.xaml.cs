using DaveFitness.Events;
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
  public delegate void CommandEventHandler(object sender, CommandEventArgs e);

  public partial class MainPanel : UserControl {
    public event CommandEventHandler CommandEventHandler;

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
          FireEvent(new CommandEventArgs(Command.Exercise));
          break;
        case "train":
          ClearLabelsBackground();
          HighlightLabel(TrainLbl);
          FireEvent(new CommandEventArgs(Command.Train));
          break;
        case "test":
          ClearLabelsBackground();
          HighlightLabel(TestLbl);
          FireEvent(new CommandEventArgs(Command.Test));
          break;
      }
    }

    protected virtual void FireEvent(CommandEventArgs e) {
      if (CommandEventHandler != null) {
        CommandEventHandler(this, e);
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
      FireEvent(new CommandEventArgs(Command.Exercise));
    }

    private void TrainLblMouseMove(object sender, MouseEventArgs e) {
      HighlightLabel(TrainLbl);
    }

    private void TrainLblMouseLeave(object sender, MouseEventArgs e) {
      ClearLabelBackground(TrainLbl);
    }

    private void TrainLblMouseClick(object sender, MouseButtonEventArgs e) {
      FireEvent(new CommandEventArgs(Command.Train));
    }

    private void TestLblMouseMove(object sender, MouseEventArgs e) {
      HighlightLabel(TestLbl);
    }

    private void TestLblMouseLeave(object sender, MouseEventArgs e) {
      ClearLabelBackground(TestLbl);
    }

    private void TestLblMouseClick(object sender, MouseButtonEventArgs e) {
      FireEvent(new CommandEventArgs(Command.Test));
    }


    private SpeechRecognitionManager speechManager; 
  }
}
