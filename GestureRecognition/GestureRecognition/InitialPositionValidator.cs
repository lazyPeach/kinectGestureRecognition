using GestureRecognition.Events;
using SkeletonModel.Events;
using SkeletonModel.Managers;
using SkeletonModel.Model;
using SkeletonModel.Util;
using System;
using System.Collections.Generic;

namespace GestureRecognition {
  // used to get the change in initial position
  public enum InitialPositionState { Enter, Exit, Neutral };

  public class InitialPositionValidator {
    public InitialPositionValidator(Body[] bodySamples) {
      initialPositionDeviation = new BodyDeviation();
      ComputeInitialPosition(bodySamples);
    }

    public void ComputeInitialPosition(Body[] bodySamples) {
      foreach (Body body in bodySamples) {
        foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
          ComputeMinBounds(body, boneName);
          ComputeMaxBounds(body, boneName);
        }
      }

      foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
        OffsetBounds(boneName);
      }
    }

    // presupunem din start ca e in initial position (previous state = true)
    public InitialPositionState GetInitialPositionState(Body currentSample) {
      if (!isPreviousSampleInInitialPosition && IsInitialPosition(currentSample)) {
        isPreviousSampleInInitialPosition = IsInitialPosition(currentSample);
        return InitialPositionState.Enter;
      }

      if (isPreviousSampleInInitialPosition && !IsInitialPosition(currentSample)) {
        isPreviousSampleInInitialPosition = IsInitialPosition(currentSample);
        return InitialPositionState.Exit;
      }

      isPreviousSampleInInitialPosition = IsInitialPosition(currentSample);
      return InitialPositionState.Neutral;
    }

    private bool IsInitialPosition(Body body) {
      foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
        // replace this godCondition with operator overloading => define > and < on BodyClass
        bool godCondition =
          body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.W > initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.W &&
          body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.W < initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.W &&
          body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.X > initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.X &&
          body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.X < initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.X &&
          body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Y > initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Y &&
          body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Y < initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Y &&
          body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Z > initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Z &&
          body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Z < initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Z;

        if (!godCondition) return false;
      }

      return true;
    }

    private void OffsetBounds(BoneName boneName) {
      float offset;
      if (boneName == BoneName.BodyCenter || boneName == BoneName.Neck) {
        offset = 0.5f;
      } else {
        offset = 0.1f;
      }

      OffsetMinBound(boneName, offset);
      OffsetMaxBound(boneName, offset);
    }

    private void OffsetMaxBound(BoneName boneName, float offset) {
      initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.W += offset;
      initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.X += offset;
      initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Y += offset;
      initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Z += offset;
    }

    private void OffsetMinBound(BoneName boneName, float offset) {
      initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.W -= offset;
      initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.X -= offset;
      initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Y -= offset;
      initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Z -= offset;
    }

    private void ComputeMaxBounds(Body body, BoneName boneName) {
      initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.W =
        Math.Max(body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.W,
          initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.W);

      initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.X =
        Math.Max(body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.X,
          initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.X);

      initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Y =
        Math.Max(body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Y,
          initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Y);

      initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Z =
        Math.Max(body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Z,
          initialPositionDeviation.MaxBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Z);
    }

    private void ComputeMinBounds(Body body, BoneName boneName) {
      initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.W =
        Math.Min(body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.W,
          initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.W);

      initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.X =
        Math.Min(body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.X,
          initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.X);

      initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Y =
        Math.Min(body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Y,
          initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Y);

      initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Z =
        Math.Min(body.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Z,
          initialPositionDeviation.MinBound.BoneSkeleton.Bones[Mapper.BoneIndexMap[boneName]].Rotation.Z);
    }


    private BodyDeviation initialPositionDeviation;
    private bool isPreviousSampleInInitialPosition = true;
  }
}


//public InitialPositionValidator(BodyManager bodyManager) {
//  
//  bodySamples = new List<Body>();

//  this.bodyManager = bodyManager;
//  bodyManager.BodyEventHandler += BodyEventHandler;
//}

//public void ComputeInitialPosition() {
//  foreach (Body body in bodySamples) {
//    foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
//      ComputeMinBounds(body, boneName);
//      ComputeMaxBounds(body, boneName);
//    }
//  }

//  foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
//    OffsetBounds(boneName);
//  }

//  isInitialPositionComputed = true;
//}

//public bool Record { set { shouldRecord = value; } }

// InitialPosition event should be fired only when body enters in initial position or exits the
// initial position. To achieve this, a flag will be used to store the last known state of the 
// body. If the last known state is
//private void BodyEventHandler(object sender, BodyEventArgs e) {
//  if (shouldRecord) {
//    bodySamples.Add(e.Body);
//    return;
//  }

//  if (IsInitialPosition(e.Body) && !IsInitialPosition(previousSample)) {
//    FireEvent(new InitialPositionEventArgs(InitialPositionState.Enter));
//  }

//  if (!IsInitialPosition(e.Body) && IsInitialPosition(previousSample)){
//    FireEvent(new InitialPositionEventArgs(InitialPositionState.Exit));
//  }

//  previousSample = e.Body;
//}

//protected virtual void FireEvent(InitialPositionEventArgs e) {
//  if (InitialPositionEventHandler != null) {
//    InitialPositionEventHandler(this, e);
//  }
//}

