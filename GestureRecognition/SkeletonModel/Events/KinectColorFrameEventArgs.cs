using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonModel.Events {
  public class KinectColorFrameEventArgs : EventArgs {
    public KinectColorFrameEventArgs(ColorImageFrame imageFrame) {
      this.imageFrame = imageFrame;
    }

    public ColorImageFrame ImageFrame { get { return imageFrame; } }

    private ColorImageFrame imageFrame;
  }
}
