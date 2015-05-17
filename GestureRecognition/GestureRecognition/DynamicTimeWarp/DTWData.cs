using SkeletonModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureRecognition.DynamicTimeWarp {
  public class DTWData {
    public DTWData(int referenceLength, int recordLength) {
      for (int i = 0; i < 4; i++) {
        referenceSignal[i] = new float[referenceLength];
        recordSignal[i] = new float[recordLength];
      }

      for (int i = 0; i < 4; i++) {
        matrix[i] = new float[referenceLength][];

        for (int j = 0; j < referenceLength; j++) {
          matrix[i][j] = new float[referenceLength];
        }
      }
    }

    public BoneName BoneName { get { return boneName; } set { boneName = value; } }
    public float[][] ReferenceSignal { get { return referenceSignal; } set { referenceSignal = value; } }
    public float[][] RecordSignal { get { return recordSignal; } set { recordSignal = value; } }
    public float[][][] Matrix { get { return matrix; } set { matrix = value; } }
    public float BestCost { get { return bestCost; } set { bestCost = value; } }

    private BoneName boneName;
    private float[][] referenceSignal = new float[4][];
    private float[][] recordSignal = new float[4][];
    private float[][][] matrix = new float[4][][];
    private float bestCost = 0;
  }
}
