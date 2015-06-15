using GestureRecognition.Events;
using SkeletonModel.Events;
using SkeletonModel.Managers;
using SkeletonModel.Model;
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
      gestureIndex.AddGesture(gestureName);
      gestureSamples = new List<Body[]>();
    }

    public void RemoveGesture(string gestureName) {
      gestureIndex.RemoveGesture(gestureName);
      gestureIndex.SaveDB();
    }

    public List<string> GetGesturesList() {
      return gestureIndex.GetAllGestures();
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
    }

    private void AddNewGestureSequence(Body[] bodySequence) {
      if (bodySequence.Length < minRecordPostures) { // discard any data smaller than 50 samples
        return;
      }

      FireEvent(new GestureRecordEventArgs(++sampleNr));
      gestureSamples.Add(bodySequence);

      if (sampleNr > 5) {
        StopRecordGesture();
      }
    }

    protected virtual void FireEvent(GestureRecordEventArgs e) {
      if (GestureRecordEventHandler != null) {
        GestureRecordEventHandler(this, e);
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
    private List<Body[]> gestureSamples;
    private int sampleNr = 0;
    private int minRecordPostures = 50;
  }
}
