using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowPoseName : MonoBehaviour
{
    [SerializeField] HandPoseTracker handPoseTracker;

    TextMeshProUGUI textMesh;

    [SerializeField] float displayTime = 2f;

    float currentTime = 0;

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
