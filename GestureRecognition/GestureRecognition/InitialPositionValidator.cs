using GestureRecognition.Events;
using SkeletonModel.Events;
using SkeletonModel.Managers;
using SkeletonModel.Model;
using SkeletonModel.Util;
using System;
using System.Collections.Generic;

namespace GestureRecognition {
  //public delegate void InitialPositionEventHandler(object sender, InitialPositionEventArgs e);

  public enum InitialPositionState { Enter, Exit };

  public class InitialPositionValidator {
    public InitialPositionValidator(Body[] bodySamples) {
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
    
    //public event InitialPositionEventHandler InitialPositionEventHandler;



    //public InitialPositionValidator(BodyManager bodyManager) {
    //  initialPositionDeviation = new BodyDeviation();
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

    private bool IsInitialPosition(Body body) {
      //if (!isInitialPositionComputed) {
      //  return false;
      //}

      foreach (BoneName boneName in Enum.GetValues(typeof(BoneName))) {
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


    //private bool shouldRecord = false;
    //private bool isInitialPositionComputed = false;
    //private Body previousSample = new Body();
    //private BodyManager bodyManager;
    private BodyDeviation initialPositionDeviation;
    //private List<Body> bodySamples;
  }
}
