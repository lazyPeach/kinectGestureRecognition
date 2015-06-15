using SkeletonModel.Events;
using SkeletonModel.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SkeletonModel.Managers {
  public delegate void BodyEventHandler(object sender, BodyEventArgs e);

  public class BodyManager {
    //public Queue<Body> RecordedData { get { return recordedData; } }
    //public Body[] RecordedDataAsArray { get { return recordedData.ToArray<Body>(); } }

    public event BodyEventHandler BodyEventHandler;

    public BodyManager() {
      //recordedData = new Queue<Body>();
    }

    public BodyManager(KinectManager kinectManager) {
      this.kinectManager = kinectManager;
      kinectManager.KinectSkeletonEventHandler += KinectSkeletonEventHandler;

      //recordedData = new Queue<Body>();
    }

    // serialization and deserialization of data
    //public void SaveBodyData(string filePath) {
    //  XmlSerializer serializer = new XmlSerializer(typeof(Body[]));
    //  TextWriter textWriter = new StreamWriter(filePath);
    //  serializer.Serialize(textWriter, recordedData.ToArray<Body>());
    //  textWriter.Close();
    //}

    //public void LoadBodyData(string filePath) {
    //  XmlSerializer deserializer = new XmlSerializer(typeof(Body[]));
    //  TextReader textReader = new StreamReader(filePath);
    //  Body[] temp = (Body[])deserializer.Deserialize(textReader);
    //  recordedData = new Queue<Body>(temp);
    //  textReader.Close();
    //}

    //public void ClearBodyData() {
    //  recordedData.Clear();
    //}
    public void SaveBodyData(string filePath, Body[] bodySamples) {
      XmlSerializer serializer = new XmlSerializer(typeof(Body[]));
      TextWriter textWriter = new StreamWriter(filePath);
      serializer.Serialize(textWriter, bodySamples);
      textWriter.Close();
    }

    public Body[] LoadBodyData(string filePath) {
      XmlSerializer deserializer = new XmlSerializer(typeof(Body[]));
      TextReader textReader = new StreamReader(filePath);
      Body[] ret = (Body[])deserializer.Deserialize(textReader);
      textReader.Close();
      return ret;
    }

    private void KinectSkeletonEventHandler(object sender, KinectSkeletonEventArgs e) {
      Body body = new Body(e.Skeleton);
      FireEvent(new BodyEventArgs(body));
    }

    protected virtual void FireEvent(BodyEventArgs e) {
      if (BodyEventHandler != null) {
        BodyEventHandler(this, e);
      }
    }


    private KinectManager kinectManager;
    //private Queue<Body> recordedData; // we are interested in keeping the order of the recording samples
  }
}
