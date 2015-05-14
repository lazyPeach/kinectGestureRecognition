using System;

namespace SpeechRecognition {
  public class RecognizedCommandEventArgs : EventArgs {
    public RecognizedCommandEventArgs(string recognizedCommand) {
      this.recognizedCommand = recognizedCommand;
    }

    public string RecognizedCommand { get { return recognizedCommand; } set { recognizedCommand = value; } }

    private string recognizedCommand;
  }
}
