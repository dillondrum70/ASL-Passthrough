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
        handPoseTracker.OnPoseEnter.AddListener(OnEnter);
        handPoseTracker.OnPoseExit.AddListener(OnExit);
    }

    private void OnDisable()
    {
        handPoseTracker.OnPoseEnter.RemoveListener(OnEnter);
        handPoseTracker.OnPoseExit.RemoveListener(OnExit);
    }

    public void OnEnter(HandPose pose)
    {
        textMesh.text = pose.GetDisplayName();
    }

    public void OnExit(HandPose pose)
    {
        textMesh.text = "";
    }
}
