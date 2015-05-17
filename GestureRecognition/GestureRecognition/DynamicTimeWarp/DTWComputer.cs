using SkeletonModel.Model;
using SkeletonModel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureRecognition.DynamicTimeWarp {
  public class DTWComputer {
    public void ComputeDTW(Body[] reference, Body[] record) {
      result = new DTWResult(reference.Length, record.Length);

      foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
        result.Data[Mapper.BoneIndexMap[boneName]].BoneName = boneName;

        result.Data[Mapper.BoneIndexMap[boneName]].ReferenceSignal = GetFilteredSignal(GetRawSignal(reference, boneName));
        result.Data[Mapper.BoneIndexMap[boneName]].RecordSignal = GetFilteredSignal(GetRawSignal(record, boneName));

        result.Data[Mapper.BoneIndexMap[boneName]].Matrix =
            ComputeDTWMatrix(result.Data[Mapper.BoneIndexMap[boneName]].ReferenceSignal,
            result.Data[Mapper.BoneIndexMap[boneName]].RecordSignal);

        result.Data[Mapper.BoneIndexMap[boneName]].BestCost[0] =
          result.Data[Mapper.BoneIndexMap[boneName]].Matrix[0][reference.Length - 1][record.Length - 1];
        result.Data[Mapper.BoneIndexMap[boneName]].BestCost[1] =
          result.Data[Mapper.BoneIndexMap[boneName]].Matrix[1][reference.Length - 1][record.Length - 1];
        result.Data[Mapper.BoneIndexMap[boneName]].BestCost[2] =
          result.Data[Mapper.BoneIndexMap[boneName]].Matrix[2][reference.Length - 1][record.Length - 1];
        result.Data[Mapper.BoneIndexMap[boneName]].BestCost[3] =
          result.Data[Mapper.BoneIndexMap[boneName]].Matrix[3][reference.Length - 1][record.Length - 1];
      }
    }

    // look on wikipedia for a faster way to compute only the slice needed http://en.wikipedia.org/wiki/Dynamic_time_warping
    private float[][][] ComputeDTWMatrix(float[][] reference, float[][] record) {
      float[][][] res = new float[4][][];
      float slope = (float)reference[0].Length / (float)record[0].Length;
      // r is the window size; if abs(i - j) > r put infinity on that cell
      int r = (int)(0.1 * Math.Max(reference[0].Length, record[0].Length));

      for (int i = 0; i < 4; i++) {
        res[i] = new float[reference[i].Length][];

        for (int j = 0; j < reference[i].Length; j++) {
          res[i][j] = new float[record[i].Length];
          int lineJ = (int)((float)j / slope); // for each i, get the j on the line and compute the dtw only for j +- r

          for (int k = 0; k < record[i].Length; k++) {
            if (Math.Abs(lineJ - k) <= r) {
              res[i][j][k] = Math.Abs(reference[i][j] - record[i][k]);
            } else {
              res[i][j][k] = 1f / 0f;
            }
          }
        }

        res[i][0][0] = 0;

        for (int j = 1; j < reference[i].Length; j++) {
          res[i][j][0] += res[i][j - 1][0];
        }

        for (int k = 1; k < record[i].Length; k++) {
          res[i][0][k] += res[i][0][k - 1];
        }

        for (int j = 2; j < reference[i].Length; j++) {
          for (int k = 2; k < record[i].Length; k++) {
            res[i][j][k] += GetMin(res[i][j-1][k], res[i][j][k-1], res[i][j-1][k-1]);
          }
        }

      }

      return res;
    }

    private float GetMin(float x, float y, float z) {
      if (x <= y && x <= z) return x;
      if (y <= x && y <= z) return y;
      if (z <= x && z <= y) return z;
      return 0;
    }

    private float[][] GetRawSignal(Body[] bodySignal, BoneName boneName) {
      float[][] res = new float[4][];
      for (int i = 0; i < 4; i++) {
        res[i] = new float[bodySignal.Length];
      }

      for (int i = 0; i < bodySignal.Length; i++) {
        res[0][i] = bodySignal[i].BoneSkeleton.GetBone(boneName).Rotation.W;
        res[1][i] = bodySignal[i].BoneSkeleton.GetBone(boneName).Rotation.X;
        res[2][i] = bodySignal[i].BoneSkeleton.GetBone(boneName).Rotation.Y;
        res[3][i] = bodySignal[i].BoneSkeleton.GetBone(boneName).Rotation.Z;
      }

      return res;
    }

    private float[][] GetFilteredSignal(float[][] rawSignal) {
      float[][] res = new float[4][];
      for (int i = 0; i < 4; i++) {
        res[i] = FilterSignal(rawSignal[i]);
      }

      return res;
    }

    private float[] FilterSignal(float[] signal) {
      float[] filteredSignal = new float[signal.Length];
      float cutoff = 0.1f;

      filteredSignal[0] = signal[0];
      for (int i = 1; i < signal.Length; i++) {
        filteredSignal[i] = cutoff * signal[i] + (1 - cutoff) * filteredSignal[i - 1];
      }

      return filteredSignal;
    }

    public DTWResult Result { get { return result; } }

    private DTWResult result;
  }
}
