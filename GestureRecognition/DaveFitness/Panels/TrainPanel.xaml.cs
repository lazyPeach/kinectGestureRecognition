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
  /// <summary>
  /// Interaction logic for TrainPanel.xaml
  /// </summary>
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
      
      //camera.Source = cameraSource;
    }

    private void SkeletonEventHandler(object sender, KinectSkeletonEventArgs e) {
      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();


      
      
      
      
      
      //foreach (Joint joint in e.Skeleton.Joints) {
        Point p = new Point(/*joint.Position.X, joint.Position.Y*/ 50, 50);
        drawingContext.DrawEllipse(new SolidColorBrush(Color.FromArgb(0, 255, 255, 0)), null, p, 30, 30);
        
        //if (startJoint == null || endJoint == null) continue;

        //double x1 = startJoint.XCoord - centerJointX;
        //double y1 = startJoint.YCoord - centerJointY;
        //double x2 = endJoint.XCoord - centerJointX;
        //double y2 = endJoint.YCoord - centerJointY;

        //DrawLine(centerX + x1 * 200, centerY - y1 * 200, centerX + x2 * 200, centerY - y2 * 200, canvas); // have a mirror display
      //}

      drawingContext.Close();
      RenderTargetBitmap renderBmp = new RenderTargetBitmap(400, 400, 96d, 96d, PixelFormats.Pbgra32);
      renderBmp.Render(drawingVisual);
      skeleton.Source = renderBmp;
      
      Console.WriteLine("skelet");
    }

    private void DrawPoint(double x, double y, Canvas canvas) {
      Ellipse point = new Ellipse {
        Width = 30,
        Height = 30,
        Fill = new SolidColorBrush(Colors.Yellow)
      };

      Canvas.SetLeft(point, x - point.Width / 2);
      Canvas.SetTop(point, y - point.Height / 2);
      canvas.Children.Add(point);
    }

    private byte[] pixels;
    private WriteableBitmap cameraSource;
    private KinectManager kinectManager;
  }
}
