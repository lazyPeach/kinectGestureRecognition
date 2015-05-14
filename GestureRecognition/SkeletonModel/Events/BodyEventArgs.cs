using SkeletonModel.Model;
using System;

namespace SkeletonModel.Events {
  public class BodyEventArgs : EventArgs {
    public BodyEventArgs(Body body) {
      this.body = body;
    }

    public Body Body { get { return body; } }

    private Body body;
  }
}
