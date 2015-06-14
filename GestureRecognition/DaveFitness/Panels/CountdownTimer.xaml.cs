using DaveFitness.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
  public delegate void CountdownEventHandler(object sender, CountdownEventArgs e);

  public partial class CountdownTimer : UserControl {
    public event CountdownEventHandler CountdownEventHandler;

    public CountdownTimer() {
      InitializeComponent();
      CreateTimeRectangles();
    }

    public void ResetTimer() {
      countdownSec = 4;
      for (int i = 0; i < 5; i++) {
        timeRect[i].Visibility = System.Windows.Visibility.Visible;
      }
    }

    public void StartCountdown() {
      countdownSec = 4;
      timer = new System.Timers.Timer { Interval = 1000 };
      timer.Elapsed += UpdateTimerBar;
      timer.Start();
    }

    private void CreateTimeRectangles() {
      timeRect = new Rectangle[5];
      SolidColorBrush fillBrush = new SolidColorBrush(Colors.Red);

      for (int i = 0; i < 5; i++) {
        timeRect[i] = new Rectangle();
        timeRect[i].Height = 50;
        timeRect[i].Width = 30;
        timeRect[i].Fill = fillBrush;
        timerGrid.Children.Add(timeRect[i]);
        Grid.SetColumn(timeRect[i], i);
      }
    }

    private void UpdateTimerBar(object sender, ElapsedEventArgs e) {
      this.Dispatcher.Invoke((Action)(() => {
        timeRect[countdownSec--].Visibility = System.Windows.Visibility.Hidden;
      }));

      if (countdownSec < 0) {
        timer.Stop();
        timer.Elapsed -= UpdateTimerBar;
        FireEvent(new CountdownEventArgs());
        return;
      }
    }

    private void FireEvent(CountdownEventArgs e) {
      if (CountdownEventHandler != null) {
        CountdownEventHandler(this, e);
      }

    }

    private Rectangle[] timeRect;
    private int countdownSec;
    private Timer timer;
  }
}
