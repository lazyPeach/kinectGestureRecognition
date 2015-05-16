using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureRecognition.Events {
  public class GestureRecordEventArgs : EventArgs {
    public GestureRecordEventArgs(int sampleNr) {
      this.sampleNr = sampleNr;
    }

    public int StampleNr { get { return sampleNr; } }

    private int sampleNr;
  }
}
