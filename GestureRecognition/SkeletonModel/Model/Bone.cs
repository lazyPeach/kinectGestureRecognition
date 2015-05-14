
namespace SkeletonModel.Model {
  public class Bone {
    public Bone() : this(new Rotation(), BoneName.BodyCenter) { }

    public Bone(Rotation rotation, BoneName boneName) {
      this.rotation = rotation;
      this.boneName = boneName;
    }

    public Rotation Rotation { get { return rotation; } set { rotation = value; } }
    public BoneName BoneName { get { return boneName; } set { boneName = value; } }

    private Rotation rotation;
    private BoneName boneName;
  }


  public class Rotation {
    public Rotation() : this(0, 0, 0, 0) { }

    public Rotation(float w, float x, float y, float z) {
      this.w = w;
      this.x = x;
      this.y = y;
      this.z = z;
    }

    public float W { get { return w; } set { w = value; } }
    public float X { get { return x; } set { x = value; } }
    public float Y { get { return y; } set { y = value; } }
    public float Z { get { return z; } set { z = value; } }

    private float w;
    private float x;
    private float y;
    private float z;
  }

  public enum BoneName {
    BodyCenter,
    LowerSpine,
    UpperSpine,
    Neck,
    ClavicleLeft,
    ArmLeft,
    ForearmLeft,
    ClavicleRight,
    ArmRight,
    ForearmRight,
    HipLeft,
    FemurusLeft,
    TibiaLeft,
    HipRight,
    FemurusRight,
    TibiaRight
  }
}
