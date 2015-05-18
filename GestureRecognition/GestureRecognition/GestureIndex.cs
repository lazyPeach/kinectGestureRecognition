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
  // filename is actually a part of the filename to which a number is appended
  public struct GestureData {
    public string name;
    public string fileName;
    public float threshold;

    public GestureData(string name, string fileName, float threshold = 0) {
      this.name = name;
      this.fileName = fileName;
      this.threshold = threshold;
    }
  }

  public class GestureIndex {
    public GestureIndex() {
      gestureDB = new Dictionary<string, GestureData>();
    }
    
    public bool IsGestureInDB() {
      return false;
    }

    public void AddGesture(string gestureName) {
      newGesture = gestureName;
      GestureData gesture = new GestureData(gestureName, gestureName.Replace(" ", "_"));
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
        gestureDB.Add(gestureData.name, gestureData);
      }

    }

    public void SaveDB() {
      // convert dictionary in an array
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

    public void SaveNewGesture() {
      GestureData gesture = new GestureData(newGesture, newGesture.Replace(" ", "_"), ComputeDTWThreshold());
      gestureDB.Remove(newGesture);
      gestureDB.Add(newGesture, gesture);

      SaveDB();
    }

    // to refactor
    private float ComputeDTWThreshold() {
      float maxSum = 0;
      List<string> files = new List<string>();

      foreach (string s in Directory.EnumerateFiles(@"..\..\..\..\database\")) {
        if (s.Contains(gestureDB[newGesture].fileName)) {
          files.Add(s);
        }
      }

      DTWComputer computer = new DTWComputer();

      for (int i = 0; i < files.Count; i++) {
        BodyManager reference = new BodyManager();
        reference.LoadBodyData(files[i]);
        
        for (int j = 0; j < files.Count; j++) {
          float sum = 0;
          FileStream recordFileStream = new FileStream(files[j], FileMode.Open, FileAccess.Read);

          BodyManager record = new BodyManager();
          record.LoadBodyData(files[j]);
          computer.ComputeDTW(reference.RecorderDataAsArray, record.RecorderDataAsArray);

          foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
            for (int k = 0; k < 4; k++) {
              sum += computer.Result.Data[Mapper.BoneIndexMap[boneName]].BestCost[k];
            }
          }

          Console.WriteLine(sum);

          if (sum > maxSum) maxSum = sum;
        }
      }

      return maxSum;
    }

    public string NewGesture { get { return newGesture; } }

    public Dictionary<string, GestureData> GestureDB { get { return gestureDB; } set { gestureDB = value; } }
    
    private Dictionary<string, GestureData> gestureDB;
    private string newGesture;
  }
}
