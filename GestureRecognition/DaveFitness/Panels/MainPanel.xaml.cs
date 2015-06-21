using DaveFitness.Events;
using SkeletonModel.Managers;
using SkeletonModel.Model;
using SpeechRecognition;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using System;

namespace DaveFitness.Panels {
  public delegate void CommandEventHandler(object sender, CommandEventArgs e);

  public partial class MainPanel : UserControl {
    public event CommandEventHandler CommandEventHandler;

    public MainPanel() {
      InitializeComponent();
      PlaySampleGestures();
    }

    private void PlaySampleGestures() {
      string gestureFolder = @"..\..\..\..\database\";
      List<string> fileNames = new List<string>();

      foreach (string file in Directory.EnumerateFiles(gestureFolder, "*.xml")) {
        if (!file.Contains("gest")) {// gesture.xml is the serialization of gestureindex
          fileNames.Add(file);
        }
      }

      // need 2 random objects to avoid getting the same random nr... and also avoid complicated code
      Random rnd1 = new Random();
      Random rnd2 = new Random();

      BodyManager bodyMgr = new BodyManager();

      firstPlayer.GestureSamples = bodyMgr.LoadBodyData(fileNames[rnd1.Next(0, fileNames.Count / 2)]);
      secondPlayer.GestureSamples = bodyMgr.LoadBodyData(fileNames[rnd2.Next(fileNames.Count / 2, fileNames.Count)]);
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
