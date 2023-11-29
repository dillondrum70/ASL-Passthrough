using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwap : MonoBehaviour
{
    [SerializeField] Material matOff, matOn;

    MeshRenderer meshRend;

    [SerializeField] HandPoseTracker handPoseTracker;

    private void Start()
    {
        meshRend = GetComponent<MeshRenderer>();
        meshRend.material = matOff;
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

    public void OnEnter()
    {
        meshRend.material = matOn;
    }

    public void OnExit()
    {
        meshRend.material = matOff;
    }
}
