using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowPoseName : MonoBehaviour
{
    [SerializeField] HandPoseTracker handPoseTracker;

    TextMeshProUGUI textMesh;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        handPoseTracker.OnGestureEnter.AddListener(OnEnter);
        //handPoseTracker.OnPoseExit.AddListener(OnExit);
    }

    private void OnDisable()
    {
        handPoseTracker.OnGestureEnter.RemoveListener(OnEnter);
        //handPoseTracker.OnPoseExit.RemoveListener(OnExit);
    }

    public void OnEnter(HandGesture gesture)
    {
        textMesh.text = gesture.GetDisplayName();
    }

    public void OnExit(HandGesture gesture)
    {
        textMesh.text = "";
    }
}
