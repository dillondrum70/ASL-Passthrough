using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHandGesture : MonoBehaviour
{
    [SerializeField] HandPoseTracker leftTracker;
    [SerializeField] HandPoseTracker rightTracker;

    private void Update()
    {
        rightTracker.GetHandPose();
        leftTracker.GetHandPose();

        rightTracker.UpdateStack();
        leftTracker.UpdateStack();

        //Check two handed poses

        //If no two handed poses
        rightTracker.CheckSingleHandGesture();
        leftTracker.CheckSingleHandGesture();
    }
}
