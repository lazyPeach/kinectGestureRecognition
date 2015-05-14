using GestureRecognition.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GestureRecognition {
  public class GestureIndex {
    public GestureIndex() {
      gestureDB = new Dictionary<string, string>();
    }
    
    public bool IsGestureInDB() {
      return false;
    }

    public void AddGesture(string gestureName) {
      string gestureFileName = gestureName.Replace(" ", "_");
      try {
        gestureDB.Add(gestureName, gestureFileName);
      } catch (Exception ex) {
        throw new GestureAlreadyExistsException(gestureName);
      }
    }

    public void RemoveGesture(string gestureName) {
      if (!gestureDB.ContainsKey(gestureName))
        return;

      gestureDB.Remove(gestureName);
    }

    public void LoadDB() {
      string[][] db;
      XmlSerializer deserializer = new XmlSerializer(typeof(string[][]));
      using (TextReader reader = new StreamReader(@"..\..\..\..\database\gestures.xml")) {
        db = (string[][])deserializer.Deserialize(reader);
        reader.Close();
      }

      int length = db[0].Length;
      for (int i = 0; i < length; i++) {
        gestureDB.Add(db[0][i], db[1][i]);
      }

    }

    public void SaveDB() {
      // convert dictionary in an array
      string[][] db = new string[2][];
      db[0] = new string[gestureDB.Count];
      db[1] = new string[gestureDB.Count];
      int i = 0;
      foreach (KeyValuePair<string, string> entry in gestureDB) {
        db[0][i] = entry.Key;
        db[1][i] = entry.Value;
        i++;
      }

      XmlSerializer serializer = new XmlSerializer(typeof(string[][]));
      using (TextWriter writer = new StreamWriter(@"..\..\..\..\database\gestures.xml")) {
        serializer.Serialize(writer, db);
        writer.Close();
      }
    }

    public List<string> GetAllGestures() {
      List<string> gestures = new List<string>();
      foreach (KeyValuePair<string, string> entry in gestureDB) {
        gestures.Add(entry.Key);
      }

      return gestures;
    }

    public Dictionary<string, string> GestureDB { get { return gestureDB; } set { gestureDB = value; } }
    private Dictionary<string, string> gestureDB;
  }
}
