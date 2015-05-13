using Microsoft.Kinect;
using SkeletonModel.Events;
using SkeletonModel.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DaveFitness.Panels {
  public partial class TrainPanel : UserControl {
    public TrainPanel() {
      InitializeComponent();
    }

    public KinectManager KinectManager { 
      set { 
        kinectManager = value;
        kinectManager.KinectColorFrameEventHandler += ColorFrameHandler;
        kinectManager.KinectSkeletonEventHandler += SkeletonEventHandler;
      }
    }


    private void ColorFrameHandler(object sender, KinectColorFrameEventArgs e) {
      if (pixels == null) {
        pixels = new byte[e.ImageFrame.PixelDataLength];
      }
      e.ImageFrame.CopyPixelDataTo(pixels);

      //// A WriteableBitmap is a WPF construct that enables resetting the Bits of the image.
      //// This is more efficient than creating a new Bitmap every frame.
      if (cameraSource == null) {
        cameraSource = new WriteableBitmap(e.ImageFrame.Width, e.ImageFrame.Height, 96, 96,
          PixelFormats.Bgr32, null);
      }

      int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
      cameraSource.WritePixels( new Int32Rect(0, 0, e.ImageFrame.Width, e.ImageFrame.Height),
            pixels, e.ImageFrame.Width * Bgr32BytesPerPixel, 0);

      cameraFrame.Source = cameraSource;
    }

    private void SkeletonEventHandler(object sender, KinectSkeletonEventArgs e) {
      KinectSensor sensor = GetKinectSensor(sender);
      DrawSkeleton(new CoordinateMapper(sensor), e.Skeleton);
    }

    private KinectSensor GetKinectSensor(object eventSender) {
      KinectManager manager = eventSender as KinectManager;
      return manager.Sensor;
    }

    private void DrawSkeleton(CoordinateMapper coordinateMapper, Skeleton skeleton) {
      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();

      foreach (Joint joint in skeleton.Joints) {
        ColorImagePoint colorImagePoint = coordinateMapper.MapSkeletonPointToColorPoint(
          joint.Position, ColorImageFormat.RgbResolution640x480Fps30);

        Point point = new Point(colorImagePoint.X, colorImagePoint.Y);
        drawingContext.DrawEllipse(new SolidColorBrush(Color.FromArgb(255, 255, 255, 0)),
          null, point, 5, 5);
      }

      drawingContext.Close();
      RenderTargetBitmap renderBmp = new RenderTargetBitmap(640, 480, 96d, 96d, PixelFormats.Pbgra32);
      renderBmp.Render(drawingVisual);
      skeletonFrame.Source = renderBmp;
    }

    
    private byte[] pixels;
    private WriteableBitmap cameraSource;
    private KinectManager kinectManager;

    private void addGestureBtn_Click(object sender, RoutedEventArgs e) {

    }

    private void removeGestureBtn_Click(object sender, RoutedEventArgs e) {

    }

    private void NewGestureTxtGotFocus(object sender, RoutedEventArgs e) {
      if (newGestureTxt.Text == "gesture name") {
        newGestureTxt.Text = "";
        newGestureTxt.Foreground = new SolidColorBrush(Colors.Black);
      }
    }
  }
}
