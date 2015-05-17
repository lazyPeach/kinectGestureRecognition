using DaveFitness.Events;
using SpeechRecognition;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DaveFitness.Panels {
  public delegate void CommandEventHandler(object sender, CommandEventArgs e);

  public partial class MainPanel : UserControl {
    public event CommandEventHandler CommandEventHandler;

    public MainPanel() {
      InitializeComponent();
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
  }
}
