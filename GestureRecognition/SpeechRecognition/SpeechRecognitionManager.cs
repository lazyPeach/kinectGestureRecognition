using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;

namespace SpeechRecognition {
  public delegate void RecognizedCommandEventHandler(object sender, RecognizedCommandEventArgs e);

  public class SpeechRecognitionManager {
    public event RecognizedCommandEventHandler RecognizedCommandEventHandler;

    public SpeechRecognitionManager(KinectSensor kinectSensor) {
      sensor = kinectSensor;
      RecognizerInfo recognizerInfo = GetKinectRecognizer();

      if (null != recognizerInfo) {
        this.speechEngine = new SpeechRecognitionEngine(recognizerInfo.Id);
        speechEngine.LoadGrammar(CreateGrammar(recognizerInfo));

        SetupSpeechEngine();
      } else {
        Console.WriteLine("Recognizer info is null");
      }
    }

    private void SetupSpeechEngine() {
      speechEngine.SpeechRecognized += SpeechRecognized;
      speechEngine.SpeechRecognitionRejected += SpeechRejected;

      speechEngine.SetInputToAudioStream(
          sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
      speechEngine.RecognizeAsync(RecognizeMode.Multiple);
    }

    private Grammar CreateGrammar(RecognizerInfo recognizerInfo) {
      Choices options = new Choices();
      options.Add(new string[] { "exercise", "train", "test", "back", "start", "stop", "up", "down" });


      GrammarBuilder grammarBuilder = new GrammarBuilder { Culture = recognizerInfo.Culture };
      grammarBuilder.Append(options);

      Grammar grammar = new Grammar(grammarBuilder);
      return grammar;
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
      const double confidenceThreshold = 0.7;

      if (e.Result.Confidence >= confidenceThreshold) {
        OnEvent(new RecognizedCommandEventArgs(e.Result.Text));
      }
    }

    protected virtual void OnEvent(RecognizedCommandEventArgs e) {
      if (RecognizedCommandEventHandler != null) {
        RecognizedCommandEventHandler(this, e);
      }
    }

    private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e) {
      Console.WriteLine("Dictionary does not contain this word");
    }

    private KinectSensor sensor;
    private SpeechRecognitionEngine speechEngine;
  }
}
