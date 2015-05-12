using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveFitness.Events {
  public enum Command {
    Exercise,
    Train,
    Test,
    Start,
    Stop,
    Back
  }

  public class CommandEventArgs : EventArgs {
    public CommandEventArgs(Command command) {
      this.command = command;
    }

    public Command Command { get { return command; } }

    private Command command;
  }
}
