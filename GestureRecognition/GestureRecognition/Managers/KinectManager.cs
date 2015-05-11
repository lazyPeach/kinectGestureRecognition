using GestureRecognition.Events;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureRecognition.Managers {
  public delegate void KinectManagerEventHandler(object sender, KinectManagerEventArgs e);
  
  public class KinectManager {
    public KinectManager() {
      InitializeKinect();
    }

    public void Start() {
      kinectSensor.Start();
    }

    public void Stop() {
      kinectSensor.Stop();
    }

    public KinectSensor Sensor { get { return kinectSensor; } }

    private void InitializeKinect() {
      kinectSensor = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);
      kinectSensor.SkeletonStream.Enable();

      skeletonData = new Skeleton[kinectSensor.SkeletonStream.FrameSkeletonArrayLength];
      kinectSensor.SkeletonFrameReady += skeletonFrameReady;
    }

    private void skeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e) {
      if (!((KinectSensor)sender).SkeletonStream.IsEnabled) {
        return;
      }

      using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) {                                 // open skeleton frame
        if (skeletonFrame != null && skeletonData != null) {
          skeletonFrame.CopySkeletonDataTo(skeletonData);                                           // get the skeletal information in this frame

          foreach (Skeleton skeleton in skeletonData) {                                             // iterate through the 6 skeletons that sensor is able to track
            if (skeleton.TrackingState == SkeletonTrackingState.Tracked) {
              OnEvent(new KinectManagerEventArgs(skeleton));
              break;                                                                                // once you find a skeleton that is tracked don't care about others
            }
          }
        }
      }
    }

    protected virtual void OnEvent(KinectManagerEventArgs e) {
      if (KinectManagerHandler != null) {
        KinectManagerHandler(this, e);
      }
    }

    private KinectSensor kinectSensor;
    private Skeleton[] skeletonData;

    public event KinectManagerEventHandler KinectManagerHandler;

  }
}
