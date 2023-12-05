using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwap : MonoBehaviour
{
    [SerializeField] Material matOff, matOn, matRed;

    MeshRenderer meshRend;

    [SerializeField] HandPoseTracker handPoseTracker;

    [SerializeField] string redPoseName = "";

    [SerializeField] float displayTime = 2f;
    float currentTime = 0;

    private void Start()
    {
        meshRend = GetComponent<MeshRenderer>();
        meshRend.material = matOff;
    }

    private void OnEnable()
    {
        handPoseTracker.OnGestureEnter.AddListener(OnEnter);
        //handPoseTracker.OnPoseExit.AddListener(OnExit);

        HandPose redPose = handPoseTracker.GetHandPose(redPoseName);

        if(redPose != null)
        {
            //redPose.OnPoseEnter.AddListener(OnEnterRed);
            //redPose.OnPoseExit.AddListener(OnExit);
        }
    }

    private void OnDisable()
    {
        handPoseTracker.OnGestureEnter.RemoveListener(OnEnter);
        //handPoseTracker.OnPoseExit.RemoveListener(OnExit);

        HandPose redPose = handPoseTracker.GetHandPose(redPoseName);

        if(redPose != null )
        {
            //redPose.OnPoseEnter.RemoveListener(OnEnterRed);
            //redPose.OnPoseExit.RemoveListener(OnExit);
        }
    }

    private void Update()
    {
        if (currentTime <= 0)
        {
            meshRend.material = matOff;
        }
        else
        {
            currentTime -= Time.deltaTime;
        }
    }

    public void OnEnter(HandGesture gesture)
    {
        currentTime = displayTime;
        meshRend.material = matOn;
    }

    public void OnExit(HandPose pose)
    {
        meshRend.material = matOff;
    }

    public void OnEnterRed(HandPose pose)
    {
        currentTime = displayTime;
        meshRend.material = matRed;
    }
}
