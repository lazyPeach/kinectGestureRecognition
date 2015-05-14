using SkeletonModel.Model;
using SkeletonModel.Util;
using System;

namespace GestureRecognition {
  public class BodyDeviation {
    public BodyDeviation() {
      minBound = new Body();
      maxBound = new Body();

      foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
        minBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.W = 1 / 0f;
        minBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.X = 1 / 0f;
        minBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Y = 1 / 0f;
        minBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Z = 1 / 0f;

        maxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.W = -1 / 0f;
        maxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.X = -1 / 0f;
        maxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Y = -1 / 0f;
        maxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Z = -1 / 0f;
      }
    }

    public Body MinBound { get { return minBound; } set { maxBound = value; } }
    public Body MaxBound { get { return maxBound; } set { maxBound = value; } }

    private Body minBound;
    private Body maxBound;
  }
}
