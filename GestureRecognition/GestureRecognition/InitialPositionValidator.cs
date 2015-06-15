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
