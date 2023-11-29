using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using Unity.VisualScripting;

public class HandPoseTracker : MonoBehaviour
{
    [SerializeField] bool right = true;

    public HandVisual handVisual;
    public IHand handCurrent;

    Quaternion indexRot;

    public UnityEvent OnPoseEnter;
    public UnityEvent OnPoseStay;
    public UnityEvent OnPoseExit;

    bool inPose = false;

    float toleranceAngle = 20f;

    private void Start()
    {
        handVisual = GetComponentInParent<HandVisual>();
        handCurrent = handVisual.Hand;
        
        handCurrent.GetJointPoseLocal(Oculus.Interaction.Input.HandJointId.HandIndex1, out Pose index);
        indexRot = index.rotation;
    }

    private void Update()
    {
        handCurrent.GetJointPoseLocal(Oculus.Interaction.Input.HandJointId.HandIndex1, out Pose index);

        if(Mathf.Abs(Quaternion.Angle(indexRot, index.rotation)) < toleranceAngle)
        {
            if(!inPose)
            {
                OnPoseEnter?.Invoke();
                inPose = true;
            }

            OnPoseStay?.Invoke();
        }
        else if(inPose)
        {
            OnPoseExit?.Invoke();
            inPose = false;
        }
    }
}
