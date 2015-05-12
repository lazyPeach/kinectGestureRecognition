using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonModel.Events {
  public class KinectSkeletonEventArgs : EventArgs {
    public KinectSkeletonEventArgs(Skeleton skeleton) {
      this.skeleton = skeleton;
    }

    public Skeleton Skeleton { get { return skeleton; } }

    private Skeleton skeleton;
  }
}
