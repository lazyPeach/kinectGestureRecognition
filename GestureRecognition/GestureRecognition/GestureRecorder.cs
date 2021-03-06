﻿using GestureRecognition.Events;
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
      }
    }

    private void InitialPositionEventHandler(object sender, InitialPositionEventArgs e) {
      if (e.PositionState == InitialPositionState.Enter) {
        ProcessSample(); 
      } 
      
      if (e.PositionState == InitialPositionState.Exit) {
        // to start recording a new sample, just clear body data and body manager will accumulate 
        // the new samples in a queue
        bodyManager.ClearBodyData();
      }
    }

    // use serializer from body manager instead of this shit
    private void ProcessSample() {
      if (bodyManager.RecordedData.Count > 50) { // consider each gesture with less than 50 samples incorrect
        //XmlSerializer serializer = new XmlSerializer(typeof(Queue<Body>));
        string filePath = @"..\..\..\..\database\" + gestureFileName + samplesCount + ".xml";
        bodyManager.SaveBodyData(filePath);
        

        if (++samplesCount == 5) {
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

    private int samplesCount = 0;
    private BodyManager bodyManager;
    private InitialPositionComputer initialComputer;
    private string gestureFileName;
  }
}
