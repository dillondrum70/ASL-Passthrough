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

    //In case we want a difficulty toggle requiring more precise or less precise signs
    float toleranceMultiplier = 1f;


#if UNITY_EDITOR
    //Only used for editor to save current hand pose to this scriptable object
    public HandPose currentEditorHandPose;
#endif

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

        if (currentEditorHandPose.CheckHandMatch(handCurrent, toleranceMultiplier))
        {
            if (!inPose)
            {
                OnPoseEnter?.Invoke();
                inPose = true;
            }

            OnPoseStay?.Invoke();
        }
        else if (inPose)
        {
            OnPoseExit?.Invoke();
            inPose = false;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Return");
            EditorSaveHandPose();
        }
#endif

    }

    void SaveHandPose(HandPose pose)
    {
        pose.SetHandPose(handCurrent);
    }

#if UNITY_EDITOR
    [ContextMenu("Save Hand Pose")]
    void EditorSaveHandPose()
    {
        currentEditorHandPose.SetHandPose(handCurrent);
    }
#endif

}
