using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace SpeechRecognition {
  public delegate void RecognizedCommandEventHandler(object sender, RecognizedCommandEventArgs e);

  public class SpeechRecognitionManager {
    public event RecognizedCommandEventHandler RecognizedCommandEventHandler;

    public SpeechRecognitionManager() {
      //// Create a new SpeechRecognitionEngine instance.
      SpeechRecognizer recognizer = new SpeechRecognizer();

      // Create a simple grammar that recognizes "red", "green", or "blue".
      Choices options = new Choices();
      options.Add(new string[] { "exercise", "train", "test" });

      // Create a GrammarBuilder object and append the Choices object.
      GrammarBuilder gb = new GrammarBuilder();
      gb.Append(options);

      // Create the Grammar instance and load it into the speech recognition engine.
      Grammar g = new Grammar(gb);
      recognizer.LoadGrammar(g);

      // Register a handler for the SpeechRecognized event.
      recognizer.SpeechRecognized += RecognizedSpeech;

    }

    void RecognizedSpeech(object sender, SpeechRecognizedEventArgs e) {
      if (e.Result.Confidence >= 0.5) {
        OnEvent(new RecognizedCommandEventArgs(e.Result.Text));
      }
    }

    protected virtual void OnEvent(RecognizedCommandEventArgs e) {
      if (RecognizedCommandEventHandler != null) {
        RecognizedCommandEventHandler(this, e);
      }
    }
  }
}
