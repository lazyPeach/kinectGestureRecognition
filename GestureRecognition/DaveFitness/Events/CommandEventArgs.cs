using System;

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
