using Microsoft.Kinect;
using System;

namespace SkeletonModel.Events {
  public class KinectSkeletonEventArgs : EventArgs {
    public KinectSkeletonEventArgs(Skeleton skeleton) {
      this.skeleton = skeleton;
    }

    public Skeleton Skeleton { get { return skeleton; } }

    private Skeleton skeleton;
  }
}
