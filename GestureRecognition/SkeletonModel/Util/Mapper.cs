using Microsoft.Kinect;
using SkeletonModel.Model;
using System;
using System.Collections.Generic;

namespace SkeletonModel.Util {
  public class Mapper {
    // map elements that don't have a correspondent, to its closest
    public static Dictionary<JointType, JointName> JointTypeJointNameMap = new Dictionary<JointType, JointName>() {
      {JointType.HipCenter      , JointName.HipCenter       },
      {JointType.Spine          , JointName.Spine           },
      {JointType.ShoulderCenter , JointName.ShoulderCenter  },
      {JointType.Head           , JointName.Head            },
      {JointType.ShoulderLeft   , JointName.ShoulderLeft    },
      {JointType.ElbowLeft      , JointName.ElbowLeft       },
      {JointType.WristLeft      , JointName.WristLeft       },
      {JointType.HandLeft       , JointName.WristLeft       },
      {JointType.ShoulderRight  , JointName.ShoulderRight   },
      {JointType.ElbowRight     , JointName.ElbowRight      },
      {JointType.WristRight     , JointName.WristRight      },
      {JointType.HandRight      , JointName.WristRight      },
      {JointType.HipLeft        , JointName.HipLeft         },
      {JointType.KneeLeft       , JointName.KneeLeft        },
      {JointType.AnkleLeft      , JointName.AnkleLeft       },
      {JointType.FootLeft       , JointName.AnkleLeft       },
      {JointType.HipRight       , JointName.HipRight        },
      {JointType.KneeRight      , JointName.KneeRight       },
      {JointType.AnkleRight     , JointName.AnkleRight      },
      {JointType.FootRight      , JointName.AnkleRight      }
    };

    public static Dictionary<JointName, JointType> JointNameJointTypeMap = new Dictionary<JointName, JointType>() {
      {JointName.HipCenter      , JointType.HipCenter       },
      {JointName.Spine          , JointType.Spine           },
      {JointName.ShoulderCenter , JointType.ShoulderCenter  },
      {JointName.Head           , JointType.Head            },
      {JointName.ShoulderLeft   , JointType.ShoulderLeft    },
      {JointName.ElbowLeft      , JointType.ElbowLeft       },
      {JointName.WristLeft      , JointType.WristLeft       },
      {JointName.ShoulderRight  , JointType.ShoulderRight   },
      {JointName.ElbowRight     , JointType.ElbowRight      },
      {JointName.WristRight     , JointType.WristRight      },
      {JointName.HipLeft        , JointType.HipLeft         },
      {JointName.KneeLeft       , JointType.KneeLeft        },
      {JointName.AnkleLeft      , JointType.AnkleLeft       },
      {JointName.HipRight       , JointType.HipRight        },
      {JointName.KneeRight      , JointType.KneeRight       },
      {JointName.AnkleRight     , JointType.AnkleRight      }
    };

    // maps a bone to its extremity joints
    public static Dictionary<BoneName, Tuple<JointName, JointName>> BoneJointMap = new Dictionary<BoneName, Tuple<JointName, JointName>>() {
      {BoneName.BodyCenter    , new Tuple<JointName, JointName>(JointName.HipCenter, JointName.HipCenter)           },
      {BoneName.LowerSpine    , new Tuple<JointName, JointName>(JointName.HipCenter, JointName.Spine)               },
      {BoneName.UpperSpine    , new Tuple<JointName, JointName>(JointName.Spine, JointName.ShoulderCenter)          },
      {BoneName.Neck          , new Tuple<JointName, JointName>(JointName.ShoulderCenter, JointName.Head)           },
      {BoneName.ClavicleLeft  , new Tuple<JointName, JointName>(JointName.ShoulderCenter, JointName.ShoulderLeft)   },
      {BoneName.ArmLeft       , new Tuple<JointName, JointName>(JointName.ShoulderLeft, JointName.ElbowLeft)        },
      {BoneName.ForearmLeft   , new Tuple<JointName, JointName>(JointName.ElbowLeft, JointName.WristLeft)           },
      {BoneName.ClavicleRight , new Tuple<JointName, JointName>(JointName.ShoulderCenter, JointName.ShoulderRight)  },
      {BoneName.ArmRight      , new Tuple<JointName, JointName>(JointName.ShoulderRight, JointName.ElbowRight)      },
      {BoneName.ForearmRight  , new Tuple<JointName, JointName>(JointName.ElbowRight, JointName.WristRight)         },
      {BoneName.HipLeft       , new Tuple<JointName, JointName>(JointName.HipCenter, JointName.HipLeft)             },
      {BoneName.FemurusLeft   , new Tuple<JointName, JointName>(JointName.HipLeft, JointName.KneeLeft)              },
      {BoneName.TibiaLeft     , new Tuple<JointName, JointName>(JointName.KneeLeft, JointName.AnkleLeft)            },
      {BoneName.HipRight      , new Tuple<JointName, JointName>(JointName.HipCenter, JointName.HipRight)            },
      {BoneName.FemurusRight  , new Tuple<JointName, JointName>(JointName.HipRight, JointName.KneeRight)            },
      {BoneName.TibiaRight    , new Tuple<JointName, JointName>(JointName.KneeRight, JointName.AnkleRight)          }
    };


    public static Dictionary<Tuple<JointName, JointName>, BoneName> JointBoneMap = new Dictionary<Tuple<JointName, JointName>, BoneName>() {
      {new Tuple<JointName, JointName>(JointName.HipCenter, JointName.HipCenter)          , BoneName.BodyCenter     },
      {new Tuple<JointName, JointName>(JointName.HipCenter, JointName.Spine)              , BoneName.LowerSpine     },
      {new Tuple<JointName, JointName>(JointName.Spine, JointName.ShoulderCenter)         , BoneName.UpperSpine     },
      {new Tuple<JointName, JointName>(JointName.ShoulderCenter, JointName.Head)          , BoneName.Neck           },
      {new Tuple<JointName, JointName>(JointName.ShoulderCenter, JointName.ShoulderLeft)  , BoneName.ClavicleLeft   },
      {new Tuple<JointName, JointName>(JointName.ShoulderLeft, JointName.ElbowLeft)       , BoneName.ArmLeft        },
      {new Tuple<JointName, JointName>(JointName.ElbowLeft, JointName.WristLeft)          , BoneName.ForearmLeft    },
      {new Tuple<JointName, JointName>(JointName.ShoulderCenter, JointName.ShoulderRight) , BoneName.ClavicleRight  },
      {new Tuple<JointName, JointName>(JointName.ShoulderRight, JointName.ElbowRight)     , BoneName.ArmRight       },
      {new Tuple<JointName, JointName>(JointName.ElbowRight, JointName.WristRight)        , BoneName.ForearmRight   },
      {new Tuple<JointName, JointName>(JointName.HipCenter, JointName.HipLeft)            , BoneName.HipLeft        },
      {new Tuple<JointName, JointName>(JointName.HipLeft, JointName.KneeLeft)             , BoneName.FemurusLeft    },
      {new Tuple<JointName, JointName>(JointName.KneeLeft, JointName.AnkleLeft)           , BoneName.TibiaLeft      },
      {new Tuple<JointName, JointName>(JointName.HipCenter, JointName.HipRight)           , BoneName.HipRight       },
      {new Tuple<JointName, JointName>(JointName.HipRight, JointName.KneeRight)           , BoneName.FemurusRight   },
      {new Tuple<JointName, JointName>(JointName.KneeRight, JointName.AnkleRight)         , BoneName.TibiaRight     }
    };

    public static Dictionary<BoneName, int> BoneIndexMap = new Dictionary<BoneName, int>() {
      {BoneName.BodyCenter    , 0},
      {BoneName.LowerSpine    , 1},
      {BoneName.UpperSpine    , 2},
      {BoneName.Neck          , 3},
      {BoneName.ClavicleLeft  , 4},
      {BoneName.ArmLeft       , 5},
      {BoneName.ForearmLeft   , 6},
      {BoneName.ClavicleRight , 7},
      {BoneName.ArmRight      , 8},
      {BoneName.ForearmRight  , 9},
      {BoneName.HipLeft       , 10},
      {BoneName.FemurusLeft   , 11},
      {BoneName.TibiaLeft     , 12},
      {BoneName.HipRight      , 13},
      {BoneName.FemurusRight  , 14},
      {BoneName.TibiaRight    , 15},
    };

    public static Dictionary<JointName, int> JointIndexMap = new Dictionary<JointName, int>() {
      {JointName.HipCenter      , 0},
      {JointName.Spine          , 1},
      {JointName.ShoulderCenter , 2},
      {JointName.Head           , 3},
      {JointName.ShoulderLeft   , 4},
      {JointName.ElbowLeft      , 5},
      {JointName.WristLeft      , 6},
      {JointName.ShoulderRight  , 7},
      {JointName.ElbowRight     , 8},
      {JointName.WristRight     , 9},
      {JointName.HipLeft        , 10},
      {JointName.KneeLeft       , 11},
      {JointName.AnkleLeft      , 12},
      {JointName.HipRight       , 13},
      {JointName.KneeRight      , 14},
      {JointName.AnkleRight     , 15}
    };
  }
}
