using SkeletonModel.Events;
using SkeletonModel.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureRecognition {
  public class GestureManager {
    public GestureManager(BodyManager bodyManager) {
      this.bodyManager = bodyManager;
      this.bodyManager.BodyEventHandler += BodyEventHandler;


      bodyRecorder = new BodyRecorder(bodyManager);
    }

    public void StartRecordingInitialPosition() {
      Console.WriteLine("start rec initial position");
      bodyRecorder.StartRecording();
    }

    public void StopRecordingInitialPosition() {
      Console.WriteLine("stop reco initial position");
      bodyRecorder.StopRecording();
      initialPositionValidator = new InitialPositionValidator(bodyRecorder.BodySamples);
    }

    public void StartRecordGesture() {

    }

    public void StopRecordGesture() {

    }

    public void ComputeInitialComputerDeviation() {

    }

    private void BodyEventHandler(object sender, BodyEventArgs e) {

    }

    private InitialPositionValidator initialPositionValidator;
    private BodyRecorder bodyRecorder;
    private BodyManager bodyManager;
  }
}
