using SkeletonModel.Events;
using SkeletonModel.Managers;
using SkeletonModel.Model;
using System.Collections.Generic;
using System.Linq;

namespace GestureRecognition {
  public class BodyRecorder {
    public BodyRecorder(BodyManager bodyManager) {
      bodySamples = new Queue<Body>();
      this.bodyManager = bodyManager;
    }

    public Body[] BodySamples { get { return bodySamples.ToArray<Body>(); } }

    // in the handler of body manager events each recorded body posture is enqueued
    public void StartRecording() {
      bodySamples.Clear();
      this.bodyManager.BodyEventHandler += BodyEventHandler;
    }

    public void StopRecording() {
      this.bodyManager.BodyEventHandler -= BodyEventHandler;
    }

    private void BodyEventHandler(object sender, BodyEventArgs e) {
      bodySamples.Enqueue(e.Body);
    }

    private BodyManager bodyManager;
    private Queue<Body> bodySamples;
  }
}
