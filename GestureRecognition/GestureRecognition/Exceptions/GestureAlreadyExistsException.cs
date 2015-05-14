using System;

namespace GestureRecognition.Exceptions {
  public class GestureAlreadyExistsException : Exception{
    public GestureAlreadyExistsException(string gesture) {
      this.gesture = gesture;
    }

    public string Gesture { get { return gesture; } }

    private string gesture;
  }
}
