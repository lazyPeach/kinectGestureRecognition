using System;

namespace SpeechRecognition {
  public enum VoiceCommand { Exercise, Train, Test, Back, Start, Stop, Up, Down, Select }

  public class RecognizedCommandEventArgs : EventArgs {
    public RecognizedCommandEventArgs(string command) {
      switch (command) {
        case "exercise":
          Console.WriteLine("exercise");
          recognizedCommand = VoiceCommand.Exercise;
          break;
        case "train":
          Console.WriteLine("train");
          recognizedCommand = VoiceCommand.Train;
          break;
        case "test":
          Console.WriteLine("test");
          recognizedCommand = VoiceCommand.Test;
          break;
        case "back":
          Console.WriteLine("back");
          recognizedCommand = VoiceCommand.Back;
          break;
        case "start":
          Console.WriteLine("start");
          recognizedCommand = VoiceCommand.Start;
          break;
        case "stop":
          Console.WriteLine("stop");
          recognizedCommand = VoiceCommand.Stop;
          break;
        case "up":
          Console.WriteLine("up");
          recognizedCommand = VoiceCommand.Up;
          break;
        case "down":
          Console.WriteLine("down");
          recognizedCommand = VoiceCommand.Down;
          break;
      }
    }

    public VoiceCommand RecognizedCommand { get { return recognizedCommand; } set { recognizedCommand = value; } }

    private VoiceCommand recognizedCommand;
  }
}
