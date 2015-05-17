using SkeletonModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureRecognition.DynamicTimeWarp {
  public enum ExerciseSpeed {
    Slow,
    Normal,
    Fast
  }

  public class DTWResult {
    public DTWResult(int referenceLength, int recordLength) {
      for (int i = 0; i < JointSkeleton.JOINTS_NR; i++) {
        data[i] = new DTWData(referenceLength, recordLength);
      }

      ComputeExerciseSpeed(referenceLength, recordLength);
    }

    public DTWData[] Data { get { return data; } set { data = value; } }
    public ExerciseSpeed Speed { get { return speed; } set { speed = value; } }


    // If the template length is much larger than sample => it took too much to user to make the exercise
    // If the template length is much smaller than sample => user made the exercise too fast
    private void ComputeExerciseSpeed(int referenceLength, int recordLength) {
      double ratio = (double)referenceLength / (double)recordLength;

      if (ratio < 0.5) speed = ExerciseSpeed.Slow;
      else if (ratio > 2) speed = ExerciseSpeed.Fast;
      else speed = ExerciseSpeed.Normal;
    }


    private DTWData[] data = new DTWData[JointSkeleton.JOINTS_NR];
    private ExerciseSpeed speed;
  }
}
