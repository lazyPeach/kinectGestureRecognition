using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechRecognition {
  public class RecognizedCommandEventArgs : EventArgs {
    public RecognizedCommandEventArgs(string recognizedCommand) {
      this.recognizedCommand = recognizedCommand;
    }

    public string RecognizedCommand { get { return recognizedCommand; } set { recognizedCommand = value; } }

    private string recognizedCommand;
  }
}
