using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static OVRPlugin;

public class ShowPoseName : MonoBehaviour
{
    [SerializeField] List<HandPoseTracker> trackers;
    [SerializeField] TwoHandPoseTracker twoHandPoseTracker;

    TextMeshProUGUI textMesh;

    [SerializeField] float displayTime = 2f;

    float currentTime = 0;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        foreach (HandPoseTracker tracker in trackers)
        {
            tracker.OnGestureEnter.AddListener(OnEnter);
            //handPoseTracker.OnPoseExit.AddListener(OnExit);
        }

        twoHandPoseTracker.OnGestureEnter.AddListener(OnEnter);
    }

    private void OnDisable()
    {
        foreach (HandPoseTracker tracker in trackers)
        {
            tracker.OnGestureEnter.RemoveListener(OnEnter);
            //handPoseTracker.OnPoseExit.RemoveListener(OnExit);
        }

        twoHandPoseTracker.OnGestureEnter.RemoveListener(OnEnter);
    }

    private void Update()
    {
        if(currentTime <= 0)
        {
            textMesh.text = "";
        }
        else
        {
            currentTime -= Time.deltaTime;
        }
    }

    public void OnEnter(IHandGesture gesture)
    {
        textMesh.text = gesture.GetDisplayName();
        currentTime = displayTime;
    }

    public void OnExit(IHandGesture gesture)
    {
        textMesh.text = "";
    }
}
