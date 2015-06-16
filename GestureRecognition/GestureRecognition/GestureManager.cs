using GestureRecognition.DynamicTimeWarp;
using GestureRecognition.Events;
using SkeletonModel.Events;
using SkeletonModel.Managers;
using SkeletonModel.Model;
using SkeletonModel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureRecognition {
  public delegate void GestureRecordEventHandler(object sender, GestureRecordEventArgs e);

  public class GestureManager {
    public GestureManager(BodyManager bodyManager) {
      this.bodyManager = bodyManager;
      gestureIndex = new GestureIndex();
      gestureIndex.LoadDB();
      bodyRecorder = new BodyRecorder(bodyManager);
    }

    public event GestureRecordEventHandler GestureRecordEventHandler;

    public void AddGesture(string gestureName) {
      gestureSamples = new List<Body[]>();
      sampleNr = 0;
      newGestureName = gestureName;
    }

    public void RemoveGesture(string gestureName) {
      gestureIndex.RemoveGesture(gestureName);
      gestureIndex.SaveDB();
    }

    public List<string> GetGesturesList() {
      return gestureIndex.GetAllGestures();
    }

    public Body[] GetGestureSample(string gestureName) {
      string gestureFileName = @"..\..\..\..\database\" + gestureIndex.GestureDB[gestureName].fileName + "0.xml";
      return bodyManager.LoadBodyData(gestureFileName);
    }

    public void StartRecordingInitialPosition() {
      bodyRecorder.StartRecording();
    }

    public void StopRecordingInitialPosition() {
      bodyRecorder.StopRecording();
      initialPositionValidator = new InitialPositionValidator(bodyRecorder.BodySamples);
      
    }

    // when gesture recording is started gesture manager checks the initial position
    // when body has exited the initial position start recording gesture. when eneterd, stop recording
    public void StartRecordGesture() {
      this.bodyManager.BodyEventHandler += BodyEventHandler;
    }

    public void StopRecordGesture() {
      this.bodyManager.BodyEventHandler -= BodyEventHandler;
      SaveSamples();
      gestureIndex.AddGesture(newGestureName, ComputeThreshold());
      gestureIndex.SaveDB();
    }

    public void StartValidateGesture(string gestureName) {
      gestureDetector = new GestureDetector(gestureIndex, gestureName);
      //return gestureManager.GetGestureSample(gestureName);

      this.bodyManager.BodyEventHandler += BodyGestureValidationEventHandler;
    }

    public void StopValidateGesture() {
      this.bodyManager.BodyEventHandler -= BodyGestureValidationEventHandler;
    }

    private void SaveSamples() {
      int index = 0;
      foreach (Body[] bodySequence in gestureSamples) {
        string filePath = @"..\..\..\..\database\" + newGestureName.Replace(" ", "_") + index + ".xml";
        bodyManager.SaveBodyData(filePath, bodySequence);
        index++;
      }
    }

    private float ComputeThreshold() {
      float maxSum = 0;
      DTWComputer computer = new DTWComputer();

      foreach (Body[] reference in gestureSamples) {
        foreach (Body[] sample in gestureSamples) {
          float sum = 0;
          computer.ComputeDTW(reference, sample);

          foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
            for (int k = 0; k < 4; k++) {
              sum += computer.Result.Data[Mapper.BoneIndexMap[boneName]].BestCost[k];
            }
          }

          if (sum > maxSum) maxSum = sum;
        }
      }

      //maxSum *= 1.5f; // offset maxSum a little bit :)

      return maxSum;
    }



    private void AddNewGestureSequence(Body[] bodySequence) {
      if (bodySequence.Length < minRecordPostures) { // discard any data smaller than 50 samples
        return;
      }

      FireEvent(new GestureRecordEventArgs(++sampleNr));
      gestureSamples.Add(bodySequence);

      if (sampleNr == 5) {
        StopRecordGesture();
      }
    }

    protected virtual void FireEvent(GestureRecordEventArgs e) {
      if (GestureRecordEventHandler != null) {
        GestureRecordEventHandler(this, e);
      }
    }

    private void BodyGestureValidationEventHandler(object sender, BodyEventArgs e) {
      InitialPositionState state = initialPositionValidator.GetInitialPositionState(e.Body);
      switch (state) {
        case InitialPositionState.Neutral:
          //Console.WriteLine("neutral");
          break;
        case InitialPositionState.Exit:
          bodyRecorder.StartRecording();
          break;
        case InitialPositionState.Enter:
          bodyRecorder.StopRecording();
          if (bodyRecorder.BodySamples.Length < minRecordPostures) { // discard any data smaller than 50 samples
            return;
          }
          gestureDetector.IsCorrectGesture(bodyRecorder.BodySamples);
          break;
      }
    }

    private void BodyEventHandler(object sender, BodyEventArgs e) {
      InitialPositionState state = initialPositionValidator.GetInitialPositionState(e.Body);
      switch (state) {
        case InitialPositionState.Neutral:
          //Console.WriteLine("neutral");
          break;
        case InitialPositionState.Exit:
          bodyRecorder.StartRecording();
          break;
        case InitialPositionState.Enter:
          bodyRecorder.StopRecording();
          AddNewGestureSequence(bodyRecorder.BodySamples);
          break;
      }
    }

    private InitialPositionValidator initialPositionValidator;
    private BodyRecorder bodyRecorder;
    private BodyManager bodyManager;
    private GestureIndex gestureIndex;
    private GestureDetector gestureDetector;
    private List<Body[]> gestureSamples;
    private int sampleNr = 0;
    private int minRecordPostures = 50;
    private string newGestureName;
  }
}
