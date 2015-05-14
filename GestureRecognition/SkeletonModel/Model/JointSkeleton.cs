using Microsoft.Kinect;
using SkeletonModel.Util;
using System;

namespace SkeletonModel.Model {
  public class JointSkeleton {
    public static int JOINTS_NR = 16;

    public JointSkeleton() {
      for (int i = 0; i < JOINTS_NR; i++) {
        joints[i] = new Joint();
      }
    }

    // create a JointSkeleton based on a Skeleton instance received from Kinect sensor
    public JointSkeleton(Skeleton skeleton) {
      foreach (JointType jointType in Enum.GetValues(typeof(JointType))) {
        if (!Mapper.JointTypeJointNameMap.ContainsKey(jointType)) continue;

        SkeletonPoint pt = skeleton.Joints[jointType].Position;
        JointName jointName = Mapper.JointTypeJointNameMap[jointType];
        Joint joint = new Joint(pt.X, pt.Y, pt.Z, jointName);

        joints[Mapper.JointIndexMap[jointName]] = joint;
      }
    }

    public Joint GetJoint(JointName jointName) {
      return joints[Mapper.JointIndexMap[jointName]];
    }

    public Joint GetJoint(JointType jointType) {
      if (!Mapper.JointTypeJointNameMap.ContainsKey(jointType)) return null;

      return GetJoint(Mapper.JointTypeJointNameMap[jointType]);
    }

    public Joint[] Joints { get { return joints; } set { joints = value; } }


    private Joint[] joints = new Joint[JOINTS_NR];
  }
}
