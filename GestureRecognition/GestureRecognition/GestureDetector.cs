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
    public GestureDetector(GestureIndex gestureIndex, string gestureName) {
      gestureThreshold = gestureIndex.GestureDB[gestureName].threshold;
      LoadSamples(gestureIndex.GestureDB[gestureName].fileName);
    }

    private float gestureThreshold = 0;
    private List<Body[]> gestureSamples; 

    private void LoadSamples(string fileName) {
      gestureSamples = new List<Body[]>();
      List<string> files = GetInterestFiles(fileName);

      BodyManager bodyManager = new BodyManager();
      foreach (string file in files) {
        gestureSamples.Add(bodyManager.LoadBodyData(file));
      }
    }

    private static List<string> GetInterestFiles(string fileName) {
      List<string> files = new List<string>();

      foreach (string s in Directory.EnumerateFiles(@"..\..\..\..\database\")) {
        if (s.Contains(fileName)) {
          files.Add(s);
        }
      }
      return files;
    }

    public bool IsCorrectGesture(Body[] record) {
      DTWComputer computer = new DTWComputer();
      float sum = 0;

      foreach (Body[] sample in gestureSamples) {
        sum = 0;
        computer.ComputeDTW(sample, record);

        foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
          for (int k = 0; k < 4; k++) {
            sum += computer.Result.Data[Mapper.BoneIndexMap[boneName]].BestCost[k];
          }
        }

        Console.WriteLine(gestureThreshold + " " + sum);
        // we need a confidence threshold... getting the max difference between samples is not 
        // the best solution... for now * 2 seems to work
        //if (sum < gestureData.threshold * 2) {
        //  Console.WriteLine("correct gesture threshold: " + sum);

        //  closestSample = reference;
        //  return true;
        //}
      }

      //Console.WriteLine("incorrect gesture threshold: " + sum);


      //closestSample = new BodyManager();
      //closestSample.LoadBodyData(files[0]);

      return false;
    }

    public BodyManager ClosestSample { get { return closestSample; } }


    private GestureIndex gestureIndex;
    private BodyManager closestSample;
  }
}
