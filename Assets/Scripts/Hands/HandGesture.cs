using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Hand Gesture", menuName = "Signs/Hand Gesture")]
public class HandGesture : ScriptableObject
{
    [SerializeField] string displayName = "";

    [SerializeField] List<HandPose> handPoseList;

    //How long to hold on the last position for it to accept (i.e. I pose might be a little longer so it doesn't get confused with J pose)
    [SerializeField] float lastPoseHoldTime = .2f;

    public UnityEvent<HandGesture> OnEnter;


    public string GetDisplayName() { return displayName; }
    public List<HandPose> GetHandPoseList() { return handPoseList; }
    public float GetLastPoseHoldTime() { return lastPoseHoldTime;}


}
