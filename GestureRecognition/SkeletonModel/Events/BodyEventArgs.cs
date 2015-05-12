using SkeletonModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonModel.Events {
  public class BodyEventArgs : EventArgs {
    public BodyEventArgs(Body body) {
      this.body = body;
    }

    public Body Body { get { return body; } }

    private Body body;
  }
}
