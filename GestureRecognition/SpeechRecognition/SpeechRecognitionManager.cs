using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechRecognition {
  public delegate void RecognizedCommandEventHandler(object sender, RecognizedCommandEventArgs e);

  public class SpeechRecognitionManager {
    public event RecognizedCommandEventHandler RecognizedCommandEventHandler;

    public SpeechRecognitionManager(KinectSensor kinectSensor) {
      sensor = kinectSensor;

      RecognizerInfo recognizerInfo = GetKinectRecognizer();


      if (null != recognizerInfo) {
        this.speechEngine = new SpeechRecognitionEngine(recognizerInfo.Id);

        Choices options = new Choices();
        options.Add(new string[] { "exercise", "train", "test", "start", "back", "up", "down", "select" });

        
        GrammarBuilder grammarBuilder = new GrammarBuilder { Culture = recognizerInfo.Culture };
        grammarBuilder.Append(options);

        Grammar grammar = new Grammar(grammarBuilder);
        speechEngine.LoadGrammar(grammar);

        speechEngine.SpeechRecognized += SpeechRecognized;
        speechEngine.SpeechRecognitionRejected += SpeechRejected;

        speechEngine.SetInputToAudioStream(
            sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
        speechEngine.RecognizeAsync(RecognizeMode.Multiple);
      } else {
      }
    }

    protected virtual void OnEvent(RecognizedCommandEventArgs e) {
      if (RecognizedCommandEventHandler != null) {
        RecognizedCommandEventHandler(this, e);
      }
    }

    private RecognizerInfo GetKinectRecognizer() {
      foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers()) {
        string value;
        recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
        if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase)) {
          return recognizer;
        }
      }

      return null;
    }


    private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e) {
      const double confidenceThreshold = 0.5;

      if (e.Result.Confidence >= confidenceThreshold) {
        OnEvent(new RecognizedCommandEventArgs(e.Result.Text));
      }
    }

    private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e) {
      Console.WriteLine("Rejected");
    }

    private KinectSensor sensor;
    private SpeechRecognitionEngine speechEngine;
  }
}
