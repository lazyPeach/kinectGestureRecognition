using GestureRecognition.Events;
using SkeletonModel.Events;
using SkeletonModel.Managers;
using SkeletonModel.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GestureRecognition {
  public delegate void GestureRecordEventHandler(object sender, GestureRecordEventArgs e);

  public class GestureRecorder {
    public event GestureRecordEventHandler GestureRecordEventHandler;

    public GestureRecorder(BodyManager bodyManager, InitialPositionComputer initialComputer, string gestureFileName) {
      //record = new Queue<Body>();
      this.bodyManager = bodyManager;
      this.initialComputer = initialComputer;
      this.gestureFileName = gestureFileName;
    }

    public void RecordInitialPosition(bool rec = false) {
      if (rec) {
        initialComputer.Record = true;  // start recording
      } else {
        initialComputer.Record = false; // stop recording
        bodyManager.ClearBodyData();
        initialComputer.ComputeInitialPosition();
        initialComputer.InitialPositionEventHandler += InitialPositionEventHandler;
        //start listen to body events
        //bodyManager.BodyEventHandler += BodyEventHandler;
      }
    }

    private void InitialPositionEventHandler(object sender, InitialPositionEventArgs e) {
      if (e.PositionState == InitialPositionState.Enter) {
        ProcessSample(); 
      } 
      
      if (e.PositionState == InitialPositionState.Exit) {
        RecordSample();
      }
    }

    private void RecordSample() {
      bodyManager.ClearBodyData();
      //record = new Queue<Body>();
      //shouldRecord = true;
    }

    // use serializer from body manager instead of this shit
    private void ProcessSample() {
      //shouldRecord = false;
      if (bodyManager.RecordedData.Count > 50) { // consider each gesture with less than 50 samples incorrect
        //XmlSerializer serializer = new XmlSerializer(typeof(Queue<Body>));
        string filePath = @"..\..\..\..\database\" + gestureFileName + samplesCount + ".xml";
        bodyManager.SaveBodyData(filePath);
        

        if (++samplesCount == 5) {
          //bodyManager.BodyEventHandler -= BodyEventHandler;
          initialComputer.InitialPositionEventHandler -= InitialPositionEventHandler;
        }

        FireEvent(new GestureRecordEventArgs(samplesCount));
      }
    }

    protected virtual void FireEvent(GestureRecordEventArgs e) {
      if (GestureRecordEventHandler != null) {
        GestureRecordEventHandler(this, e);
      }
    }

    //private void BodyEventHandler(object sender, BodyEventArgs e) {
    //  Body body = e.Body;
    //  if (shouldRecord) {
    //    record.Enqueue(body);
    //  }
    //}

    //private bool shouldRecord = false;
    private int samplesCount = 0;
    private BodyManager bodyManager;
    private InitialPositionComputer initialComputer;
    private string gestureFileName;
    //private Queue<Body> record;
  }
}
