using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwap : MonoBehaviour
{
    [SerializeField] Material matOff, matOn, matRed;

    MeshRenderer meshRend;

    [SerializeField] HandPoseTracker handPoseTracker;

    [SerializeField] string redPoseName = "";

    private void Start()
    {
        meshRend = GetComponent<MeshRenderer>();
        meshRend.material = matOff;
    }

    private void OnEnable()
    {
        //handPoseTracker.OnPoseEnter.AddListener(OnEnter);
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
        //handPoseTracker.OnPoseEnter.RemoveListener(OnEnter);
        //handPoseTracker.OnPoseExit.RemoveListener(OnExit);

        HandPose redPose = handPoseTracker.GetHandPose(redPoseName);

        if(redPose != null )
        {
            //redPose.OnPoseEnter.RemoveListener(OnEnterRed);
            //redPose.OnPoseExit.RemoveListener(OnExit);
        }
    }

    public void OnEnter(HandPose pose)
    {
        meshRend.material = matOn;
    }

    public void OnExit(HandPose pose)
    {
        meshRend.material = matOff;
    }

    public void OnEnterRed(HandPose pose)
    {
        meshRend.material = matRed;
    }
}
