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


      bodyRecorder = new BodyRecorder(bodyManager);
    }

    public event GestureRecordEventHandler GestureRecordEventHandler;

    public void AddNewGesture(string gestureName) {
      gestureSamples = new List<Body[]>();
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

    //private void ProcessSample() {
    //  if (bodyManager.RecordedData.Count > 50) { // consider each gesture with less than 50 samples incorrect
    //    //XmlSerializer serializer = new XmlSerializer(typeof(Queue<Body>));
    //    string filePath = @"..\..\..\..\database\" + gestureFileName + samplesCount + ".xml";
    //    bodyManager.SaveBodyData(filePath);


    //    if (++samplesCount == 5) {
    //      initialComputer.InitialPositionEventHandler -= InitialPositionEventHandler;
    //    }

    //    FireEvent(new GestureRecordEventArgs(samplesCount));
    //  }
    //}

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
    private List<Body[]> gestureSamples;
    private int sampleNr = 0;
    private int minRecordPostures = 50;
  }
}
