using SkeletonModel.Events;
using SkeletonModel.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SkeletonModel.Managers {
  public delegate void BodyEventHandler(object sender, BodyEventArgs e);

  public class BodyManager {
    public Body[] RecordedData { get { return recordedData.ToArray<Body>(); } }

    public event BodyEventHandler BodyEventHandler;

    public BodyManager() {
      recordedData = new Queue<Body>();
    }

    public BodyManager(KinectManager kinectManager) {
      this.kinectManager = kinectManager;
      kinectManager.KinectSkeletonEventHandler += KinectSkeletonEventHandler;

      recordedData = new Queue<Body>();
    }

    // serialization and deserialization of data
    public void SaveBodyData(Stream file) {
      XmlSerializer serializer = new XmlSerializer(typeof(Queue<Body>));
      TextWriter textWriter = new StreamWriter(file);
      serializer.Serialize(textWriter, recordedData);
      textWriter.Close();
    }

    public void LoadBodyData(Stream file) {
      XmlSerializer deserializer = new XmlSerializer(typeof(Queue<Body>));
      TextReader textReader = new StreamReader(file);
      recordedData = (Queue<Body>)deserializer.Deserialize(textReader);
      textReader.Close();
    }

    public void ClearBodyData() {
      recordedData.Clear();
    }

    private void KinectSkeletonEventHandler(object sender, KinectSkeletonEventArgs e) {
      Body body = new Body(e.Skeleton);
      FireEvent(new BodyEventArgs(body));
      recordedData.Enqueue(body);
    }

    protected virtual void FireEvent(BodyEventArgs e) {
      if (BodyEventHandler != null) {
        BodyEventHandler(this, e);
      }
    }


    private KinectManager kinectManager;
    private Queue<Body> recordedData; // we are interested in keeping the order of the recording samples
  }
}
