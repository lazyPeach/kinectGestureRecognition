using Microsoft.Kinect;
using SkeletonModel.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonModel.Managers {
  public delegate void KinectSkeletonEventHandler(object sender, KinectSkeletonEventArgs e);
  public delegate void KinectColorFrameEventHandler(object sender, KinectColorFrameEventArgs e);

  public class KinectManager {
    public event KinectSkeletonEventHandler KinectSkeletonEventHandler;
    public event KinectColorFrameEventHandler KinectColorFrameEventHandler;

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
      kinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

      skeletonData = new Skeleton[kinectSensor.SkeletonStream.FrameSkeletonArrayLength];

      kinectSensor.SkeletonFrameReady += SkeletonFrameReady;
      kinectSensor.ColorFrameReady += ColorFrameReady;
    }

    // listen to SkeletonFrameReady events
    private void SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e) {
      if (!((KinectSensor)sender).SkeletonStream.IsEnabled) {
        return;
      }

      using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) {                               // Open the Skeleton frame

        if (skeletonFrame != null && skeletonData != null) {
          skeletonFrame.CopySkeletonDataTo(skeletonData);                                         // get the skeletal information in this frame

          foreach (Skeleton skeleton in skeletonData) {                                           // iterate through the 6 skeletons that sensor is able to track
            if (skeleton.TrackingState == SkeletonTrackingState.Tracked) {
              FireSkeletonEvent(new KinectSkeletonEventArgs(skeleton));
              break;                                                                              // once you find a skeleton that is tracked don't care about others
            }
          }
        }
      }
    }

    private void ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e) {
      using (ColorImageFrame imageFrame = e.OpenColorImageFrame()) {
        if (imageFrame != null) {
          FireColorFrameEvent(new KinectColorFrameEventArgs(imageFrame));
        }
      }
    }

    protected virtual void FireSkeletonEvent(KinectSkeletonEventArgs e) {
      if (KinectSkeletonEventHandler != null) {
        KinectSkeletonEventHandler(this, e);
      }
    }

    protected virtual void FireColorFrameEvent(KinectColorFrameEventArgs e) {
      if (KinectColorFrameEventHandler != null) {
        KinectColorFrameEventHandler(this, e);
      }
    }


    private KinectSensor kinectSensor;
    private Skeleton[] skeletonData;
  }
}
