using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Hand Gesture", menuName = "Signs/Hand Gesture")]
public class HandGesture : ScriptableObject, IHandGesture
{
    [SerializeField] string displayName = "";

    [SerializeField] protected List<HandPose> handPoseList;

    //How long to hold on the last position for it to accept (i.e. I pose might be a little longer so it doesn't get confused with J pose)
    [SerializeField] float lastPoseHoldTime = .2f;

    public UnityEvent<IHandGesture> OnEnter;


    public string GetDisplayName() { return displayName; }
    public List<HandPose> GetHandPoseList() { return handPoseList; }
    public float GetLastPoseHoldTime() { return lastPoseHoldTime;}

    public bool MatchGesture(List<HandPoseData> stack)
    {
        List<HandPose> poses = new(handPoseList);
        poses.Reverse();

        //Check last pose hold time of gesture is shorter than we've been holding this pose
        if (stack.Count < poses.Count || lastPoseHoldTime > stack[0].elapsedTime)
        {
            //Skip this gesture
            return false;
        }

        //Check order of poses, ensure they match the most recent poses in the stack
        bool match = true;
        for (int i = 0; i < poses.Count; i++)
        {
            //Exit loop if stack is shorter than pose list or a pose does not match, move to next pose or exit and accept if at end of pose list
            if (poses[i] != stack[i].pose)
            {
                match = false;
                break;
            }
        }

        return match;
    }
}
