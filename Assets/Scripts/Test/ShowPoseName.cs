using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowPoseName : MonoBehaviour
{
    [SerializeField] List<HandPoseTracker> trackers;

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
    }

    private void OnDisable()
    {
        foreach (HandPoseTracker tracker in trackers)
        {
            tracker.OnGestureEnter.RemoveListener(OnEnter);
            //handPoseTracker.OnPoseExit.RemoveListener(OnExit);
        }

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

    public void OnEnter(HandGesture gesture)
    {
        textMesh.text = gesture.GetDisplayName();
        currentTime = displayTime;
    }

    public void OnExit(HandGesture gesture)
    {
        textMesh.text = "";
    }
}
