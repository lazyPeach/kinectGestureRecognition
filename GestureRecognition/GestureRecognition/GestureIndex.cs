using GestureRecognition.DynamicTimeWarp;
using GestureRecognition.Exceptions;
using SkeletonModel.Managers;
using SkeletonModel.Model;
using SkeletonModel.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GestureRecognition {
  // todo get rid of gestureName... you already have it in key
  public struct GestureData {
    public string gestureName;
    public string fileName;
    public float threshold;

    public GestureData(string gestureName, string fileName, float threshold = 0) {
      this.gestureName = gestureName;
      this.fileName = fileName;
      this.threshold = threshold;
    }
  }

  public class GestureIndex {
    public GestureIndex() {
      gestureDB = new Dictionary<string, GestureData>();
    }

    public void AddGesture(string gestureName, float threshold) {
      GestureData gesture = new GestureData(gestureName, gestureName.Replace(" ", "_"), threshold);

      try {
        gestureDB.Add(gestureName, gesture);
      } catch (Exception ex) {
        Console.WriteLine(ex.Message);
        throw new GestureAlreadyExistsException(gestureName);
      }
    }

    public void RemoveGesture(string gestureName) {
      if (!gestureDB.ContainsKey(gestureName))
        return;

      foreach (string s in Directory.EnumerateFiles(@"..\..\..\..\database\")) {
        if (s.Contains(gestureName.Replace(" ", "_"))) {
          File.Delete(s);
        }
      }

      gestureDB.Remove(gestureName);
      SaveDB();
    }

    public void LoadDB() {
      GestureData[] db;
      XmlSerializer deserializer = new XmlSerializer(typeof(GestureData[]));
      using (TextReader reader = new StreamReader(@"..\..\..\..\database\gestures.xml")) {
        db = (GestureData[])deserializer.Deserialize(reader);
        reader.Close();
      }

      foreach (GestureData gestureData in db) {
        gestureDB.Add(gestureData.gestureName, gestureData);
      }
    }

    public void SaveDB() {
      // dictionary must be turned into an array sicnce Dictionary is not serializable
      GestureData[] db = new GestureData[gestureDB.Count];
      int i = 0;

      foreach (KeyValuePair<string, GestureData> entry in gestureDB) {
        db[i++] = entry.Value;
      }

      XmlSerializer serializer = new XmlSerializer(typeof(GestureData[]));
      using (TextWriter writer = new StreamWriter(@"..\..\..\..\database\gestures.xml")) {
        serializer.Serialize(writer, db);
        writer.Close();
      }
    }

    public List<string> GetAllGestures() {
      List<string> gestures = new List<string>();
      foreach (KeyValuePair<string, GestureData> entry in gestureDB) {
        gestures.Add(entry.Key);
      }

      return gestures;
    }

    public Dictionary<string, GestureData> GestureDB { get { return gestureDB; } set { gestureDB = value; } }


    private Dictionary<string, GestureData> gestureDB;
  }
}
