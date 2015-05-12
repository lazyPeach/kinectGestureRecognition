using Microsoft.Kinect;
using SkeletonModel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonModel.Model {
  public class BoneSkeleton {
    public static int BONES_NR = 16;

    public BoneSkeleton() {
      for (int i = 0; i < BONES_NR; i++) {
        bones[i] = new Bone();
      }
    }

    // create a BoneSkeleton based on a Skeleton instance received from Kinect sensor
    public BoneSkeleton(Skeleton skeleton) {
      foreach (BoneOrientation boneOrientation in skeleton.BoneOrientations) {
        if (!Mapper.JointTypeJointNameMap.ContainsKey(boneOrientation.StartJoint) ||
            !Mapper.JointTypeJointNameMap.ContainsKey(boneOrientation.EndJoint)) continue;

        Tuple<JointName, JointName> jointTuple = new Tuple<JointName, JointName>(
          Mapper.JointTypeJointNameMap[boneOrientation.StartJoint],
          Mapper.JointTypeJointNameMap[boneOrientation.EndJoint]);

        if (!Mapper.JointBoneMap.ContainsKey(jointTuple)) continue;

        BoneName boneName = Mapper.JointBoneMap[jointTuple];
        Rotation rotation = new Rotation(boneOrientation.HierarchicalRotation.Quaternion.W
                                       , boneOrientation.HierarchicalRotation.Quaternion.X
                                       , boneOrientation.HierarchicalRotation.Quaternion.Y
                                       , boneOrientation.HierarchicalRotation.Quaternion.Z);

        bones[Mapper.BoneIndexMap[boneName]] = new Bone(rotation, boneName); ;
      }
    }

    public Bone GetBone(BoneName boneName) {
      if (!Mapper.BoneIndexMap.ContainsKey(boneName)) return new Bone();

      return bones[Mapper.BoneIndexMap[boneName]];
    }

    public Bone[] Bones { get { return bones; } set { bones = value; } }


    private Bone[] bones = new Bone[BONES_NR];
  }
}
