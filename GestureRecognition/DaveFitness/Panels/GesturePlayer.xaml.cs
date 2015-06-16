using SkeletonModel.Model;
using SkeletonModel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
  public partial class GesturePlayer : UserControl {
    public GesturePlayer() {
      InitializeComponent();
      PlayGesture();
    }

    public Body[] GestureSamples { set { gestureSamples = value; } }



    private void PlayGesture() {
      gestureTimer = new System.Timers.Timer { Interval = 40 };
      gestureTimer.Elapsed += UpdateGesturePlayer;
      gestureTimer.Start();
    }

    private void UpdateGesturePlayer(object sender, ElapsedEventArgs e) {
      if (gestureSamples != null) {
        this.Dispatcher.Invoke((Action)(() => { // update from any thread
          centerX = (int)gestureCanvas.Width / 2;
          centerY = (int)gestureCanvas.Height / 2; 
          DrawSkeleton(gestureSamples[sampleIndex++]);
        }));

        if (sampleIndex == gestureSamples.Length) { // start from the beginning
          sampleIndex = 0;
        }
      }
    }

    private void DrawSkeleton(Body body) {
      gestureCanvas.Children.Clear();
      DrawSampleGestureBones(body.JointSkeleton);
      DrawSampleGestureJoints(body.JointSkeleton);
    }

    private void DrawSampleGestureBones(JointSkeleton jointSkeleton) {
      SkeletonModel.Model.Joint centerJoint = jointSkeleton.GetJoint(JointName.HipCenter);

      foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
        Tuple<JointName, JointName> boneExtremities = Mapper.BoneJointMap[boneName];
        SkeletonModel.Model.Joint startJoint = jointSkeleton.GetJoint(boneExtremities.Item1);
        SkeletonModel.Model.Joint endJoint = jointSkeleton.GetJoint(boneExtremities.Item2);

        if (startJoint == null || endJoint == null) continue;

        double x1 = startJoint.XCoord - centerJoint.XCoord;
        double y1 = startJoint.YCoord - centerJoint.YCoord;
        double x2 = endJoint.XCoord - centerJoint.XCoord;
        double y2 = endJoint.YCoord - centerJoint.YCoord;

        DrawLine(centerX + x1 * 150, centerY - y1 * 150, centerX + x2 * 150, centerY - y2 * 150, Colors.OrangeRed);
      }
    }

    private void DrawSampleGestureJoints(JointSkeleton jointSkeleton) {
      DrawPoint(centerX, centerY, Colors.Yellow);

      SkeletonModel.Model.Joint centerJoint = jointSkeleton.GetJoint(JointName.HipCenter);

      foreach (JointName jointType in Enum.GetValues(typeof(JointName))) {
        SkeletonModel.Model.Joint joint = jointSkeleton.GetJoint(jointType);

        if (joint == null) continue;

        double x = joint.XCoord - centerJoint.XCoord;
        double y = joint.YCoord - centerJoint.YCoord;

        DrawPoint(centerX + x * 150, centerY - y * 150, Colors.Red); // have a mirror display
      }
    }



    private void DrawPoint(double x, double y, Color color) {
      Ellipse point = new Ellipse {
        Width = 7,
        Height = 7,
        Fill = new SolidColorBrush(color)
      };

      Canvas.SetLeft(point, x - point.Width / 2);
      Canvas.SetTop(point, y - point.Height / 2);
      gestureCanvas.Children.Add(point);
    }

    private void DrawLine(double x1, double y1, double x2, double y2, Color color) {
      Line myLine = new Line();
      myLine.Stroke = new SolidColorBrush(color);
      myLine.X1 = x1;
      myLine.X2 = x2;
      myLine.Y1 = y1;
      myLine.Y2 = y2;
      myLine.HorizontalAlignment = HorizontalAlignment.Left;
      myLine.VerticalAlignment = VerticalAlignment.Top;
      myLine.StrokeThickness = 5;
      gestureCanvas.Children.Add(myLine);
    }



    private Timer gestureTimer;
    private Body[] gestureSamples;
    private int sampleIndex = 0;
    private int centerX;
    private int centerY;
  }
}
