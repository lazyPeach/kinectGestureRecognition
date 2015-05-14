using Microsoft.Kinect;
using System;

namespace SkeletonModel.Events {
  public class KinectColorFrameEventArgs : EventArgs {
    public KinectColorFrameEventArgs(ColorImageFrame imageFrame) {
      this.imageFrame = imageFrame;
    }

    public ColorImageFrame ImageFrame { get { return imageFrame; } }

    private ColorImageFrame imageFrame;
  }
}
