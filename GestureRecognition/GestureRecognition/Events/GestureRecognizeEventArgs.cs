using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureRecognition.Events {
  public class GestureRecognizeEventArgs : EventArgs {
    public GestureRecognizeEventArgs(bool isRecognized) {
      isGestureRecognized = isRecognized;
    }

    public bool IsGestureRecognized { get { return isGestureRecognized; } }

    private bool isGestureRecognized;
  }
}
