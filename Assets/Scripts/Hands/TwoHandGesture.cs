using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Two Hand Gesture", menuName = "Signs/Two Hand Gesture")]
public class TwoHandGesture : ScriptableObject, IHandGesture
{
    [SerializeField] string displayName = "";

    [SerializeField] protected HandGesture leftHandGesture;
    [SerializeField] protected HandGesture rightHandGesture;

    public UnityEvent<IHandGesture> OnEnter;

    public string GetDisplayName() { return displayName; }
    public HandGesture GetLeftHandGesture() { return leftHandGesture; }
    public HandGesture GetRightHandGesture() { return rightHandGesture; }


    public bool MatchGesture(HandPoseTracker leftTracker, HandPoseTracker rightTracker)
    {
        List<HandPose> leftPoses = new(leftHandGesture.GetHandPoseList());
        leftPoses.Reverse();

        List<HandPose> rightPoses = new(rightHandGesture.GetHandPoseList());
        rightPoses.Reverse();

        List<HandPoseData> leftStack = leftTracker.GetStack();
        List<HandPoseData> rightStack = rightTracker.GetStack();

        //Check last pose hold time of gesture is shorter than we've been holding this pose
        if (leftPoses.Count != rightPoses.Count ||  //Number of poses in each hand MUST match
            leftStack.Count < leftPoses.Count || leftHandGesture.GetLastPoseHoldTime() > leftStack[0].currentTime ||
            rightStack.Count < rightPoses.Count || rightHandGesture.GetLastPoseHoldTime() > rightStack[0].currentTime)
        {
            //Skip this gesture
            return false;
        }

        //Check order of poses, ensure they match the most recent poses in the stack
        bool match = true;
        for (int i = 0; i < leftPoses.Count; i++)
        {
            //Exit loop if stack is shorter than pose list or a pose does not match, move to next pose or exit and accept if at end of pose list
            if (leftPoses[i] != leftStack[i].pose &&
                rightPoses[i] != rightStack[i].pose)
            {
                match = false;
                break;
            }
        }

        return match;
    }
}