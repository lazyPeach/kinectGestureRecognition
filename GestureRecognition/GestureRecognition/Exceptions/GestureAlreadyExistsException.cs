using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureRecognition.Exceptions {
  public class GestureAlreadyExistsException : Exception{
    public GestureAlreadyExistsException(string gesture) {
      this.gesture = gesture;
    }

    public string Gesture { get { return gesture; } }

    private string gesture;
  }
}
