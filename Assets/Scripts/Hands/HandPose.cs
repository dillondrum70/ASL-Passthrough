using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Oculus.Interaction.Input;

public class HandPose
{
    Dictionary<HandJointId, Pose> localPoses;
    Dictionary<HandJointId, Pose> worldPoses;

    public void Set(IHand hand)
    {

    }
}
