using System;

namespace GestureRecognition.Events {
  public class InitialPositionEventArgs : EventArgs {
    public InitialPositionEventArgs(InitialPositionState positionState) {
      this.positionState = positionState;
    }

    public InitialPositionState PositionState { get { return positionState; } }

    private InitialPositionState positionState;
  }
}
