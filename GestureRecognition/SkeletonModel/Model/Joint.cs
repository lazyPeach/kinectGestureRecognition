
namespace SkeletonModel.Model {
  public class Joint {
    public Joint() : this(0, 0, 0, JointName.HipCenter) { }

    public Joint(float xCoord, float yCoord, float zCoord, JointName jointName) {
      this.xCoord = xCoord;
      this.yCoord = yCoord;
      this.zCoord = zCoord;
      this.jointName = jointName;
    }

    public float XCoord { get { return xCoord; } set { xCoord = value; } }
    public float YCoord { get { return yCoord; } set { yCoord = value; } }
    public float ZCoord { get { return zCoord; } set { zCoord = value; } }
    public JointName JointName { get { return jointName; } set { jointName = value; } }

    private float xCoord;
    private float yCoord;
    private float zCoord;
    private JointName jointName;
  }

  public enum JointName {
    HipCenter,
    Spine,
    ShoulderCenter,
    Head,
    ShoulderLeft,
    ElbowLeft,
    WristLeft,
    ShoulderRight,
    ElbowRight,
    WristRight,
    HipLeft,
    KneeLeft,
    AnkleLeft,
    HipRight,
    KneeRight,
    AnkleRight
  }
}
