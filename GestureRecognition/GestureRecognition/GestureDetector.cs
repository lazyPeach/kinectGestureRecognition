using GestureRecognition.DynamicTimeWarp;
using SkeletonModel.Managers;
using SkeletonModel.Model;
using SkeletonModel.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureRecognition {
  public class GestureDetector {
    public GestureDetector(GestureIndex gestureIndex) {
      this.gestureIndex = gestureIndex;
    }

    public bool IsCorrectGesture(string gestureName, Body[] record) {
      GestureData gestureData = gestureIndex.GestureDB[gestureName];

      List<string> files = new List<string>();

      foreach (string s in Directory.EnumerateFiles(@"..\..\..\..\database\")) {
        if (s.Contains(gestureData.fileName)) {
          files.Add(s);
        }
      }

      DTWComputer computer = new DTWComputer();

      for (int i = 0; i < files.Count; i++) {
        float sum = 0;

        BodyManager reference = new BodyManager();
        reference.LoadBodyData(files[i]);

        computer.ComputeDTW(reference.RecordedDataAsArray, record);

        foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
          for (int k = 0; k < 4; k++) {
            sum += computer.Result.Data[Mapper.BoneIndexMap[boneName]].BestCost[k];
          }
        }

        Console.WriteLine("for sample " + i + ": " + sum);

        // we need a confidence threshold... getting the max difference between samples is not 
        // the best solution... for now * 2 seems to work
        if (sum < gestureData.threshold * 2) {
          closestSample = reference;
          return true;
        }
      }

      closestSample = new BodyManager();
      closestSample.LoadBodyData(files[0]);

      return false;
    }

    public BodyManager ClosestSample { get { return closestSample; } }


    private GestureIndex gestureIndex;
    private BodyManager closestSample;
  }
}
