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

    //[Header("Debug Values")]

    //[SerializeField] bool debugDrawBones = true;
    //[SerializeField] float debugFingertipLength = 0.025f;

    //[SerializeField] bool debugDrawJoints = true;
    //[SerializeField] float debugJointRadius = .005f;

    //[SerializeField] bool debugDrawTolerance = true;
    //[SerializeField] float debugToleranceLength = .01f;
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
        //handCurrent.GetJointPoseLocal(Oculus.Interaction.Input.HandJointId.HandIndex1, out Pose index);

        //Check if HandPose matches current hand
        if (currentEditorHandPose.CheckHandMatch(handCurrent, toleranceMultiplier))
        {
            //OnEnter
            if (!inPose)
            {
                OnPoseEnter?.Invoke();
                inPose = true;
            }

            //OnStay
            OnPoseStay?.Invoke();
        }
        else if (inPose) //OnExit
        {
            OnPoseExit?.Invoke();
            inPose = false;
        }

#if UNITY_EDITOR
        //Save current hand pose to prefab in currentEditorHandPose
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Return");
            EditorSaveHandPose();
        }
#endif

    }

    /// <summary>
    /// Save current hand pose to the passed HandPose, for use when calibrating signs
    /// </summary>
    /// <param name="pose"></param>
    void SaveHandPose(HandPose pose)
    {
        pose.SetHandPose(handCurrent);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Save current hand pose to prefab in currentEditorHandPose
    /// </summary>
    [ContextMenu("Save Hand Pose")]
    void EditorSaveHandPose()
    {
        currentEditorHandPose.SetHandPose(handCurrent);
    }

    //private void OnDrawGizmos()
    //{
    //    if (!Application.isPlaying) return;

    //    Gizmos.color = Color.yellow;
        
    //    if (debugDrawBones)
    //    {
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandWristRoot, out Pose HandWristRoot);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandThumb0, out Pose HandThumb0);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandThumb1, out Pose HandThumb1);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandThumb2, out Pose HandThumb2);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandThumb3, out Pose HandThumb3);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandThumbTip, out Pose HandThumbTip);
    //        Gizmos.DrawLine(HandWristRoot.position, HandThumb0.position);
    //        Gizmos.DrawLine(HandThumb0.position, HandThumb1.position);
    //        Gizmos.DrawLine(HandThumb1.position, HandThumb2.position);
    //        Gizmos.DrawLine(HandThumb2.position, HandThumb3.position);
    //        Gizmos.DrawLine(HandThumb3.position, HandThumbTip.position);

    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandIndex1, out Pose HandIndex1);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandIndex2, out Pose HandIndex2);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandIndex3, out Pose HandIndex3);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandIndexTip, out Pose HandIndexTip);
    //        Gizmos.DrawLine(HandWristRoot.position, HandIndex1.position);
    //        Gizmos.DrawLine(HandIndex1.position, HandIndex2.position);
    //        Gizmos.DrawLine(HandIndex2.position, HandIndex3.position);
    //        Gizmos.DrawLine(HandIndex3.position, HandIndexTip.position);

    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandMiddle1, out Pose HandMiddle1);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandMiddle2, out Pose HandMiddle2);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandMiddle3, out Pose HandMiddle3);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandMiddleTip, out Pose HandMiddleTip);
    //        Gizmos.DrawLine(HandWristRoot.position, HandMiddle1.position);
    //        Gizmos.DrawLine(HandMiddle1.position, HandMiddle2.position);
    //        Gizmos.DrawLine(HandMiddle2.position, HandMiddle3.position);
    //        Gizmos.DrawLine(HandMiddle3.position, HandMiddleTip.position);

    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandRing1, out Pose HandRing1);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandRing2, out Pose HandRing2);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandRing3, out Pose HandRing3);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandRingTip, out Pose HandRingTip);
    //        Gizmos.DrawLine(HandWristRoot.position, HandRing1.position);
    //        Gizmos.DrawLine(HandRing1.position, HandRing2.position);
    //        Gizmos.DrawLine(HandRing2.position, HandRing3.position);
    //        Gizmos.DrawLine(HandRing3.position, HandRingTip.position);

    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandPinky0, out Pose HandPinky0);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandPinky1, out Pose HandPinky1);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandPinky2, out Pose HandPinky2);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandPinky3, out Pose HandPinky3);
    //        handCurrent.GetJointPose(Oculus.Interaction.Input.HandJointId.HandPinkyTip, out Pose HandPinkyTip);
    //        Gizmos.DrawLine(HandWristRoot.position, HandPinky0.position);
    //        Gizmos.DrawLine(HandPinky0.position, HandPinky1.position);
    //        Gizmos.DrawLine(HandPinky1.position, HandPinky2.position);
    //        Gizmos.DrawLine(HandPinky2.position, HandPinky3.position);
    //        Gizmos.DrawLine(HandPinky3.position, HandPinkyTip.position);
    //    }

    //    Gizmos.color = Color.green;

    //    if (debugDrawJoints)
    //    {
    //        for (int i = 1; i < (int)HandJointId.HandMaxSkinnable; i++)
    //        {
    //            handCurrent.GetJointPose((HandJointId)i, out Pose pose);
    //            Gizmos.DrawSphere(pose.position, debugJointRadius);
    //        }
    //    }

    //    ////Orange
    //    //Gizmos.color = new Color(1, .3f, 0, .5f);

    //    //if (debugDrawTolerance)
    //    //{
    //    //    for (int i = 1; i < (int)HandJointId.HandMaxSkinnable; i++)
    //    //    {
    //    //        handCurrent.GetJointPose((HandJointId)i, out Pose pose);
    //    //        Gizmos.matrix = Matrix4x4.TRS(pose.position, pose.rotation * Quaternion.Euler(new Vector3(0, 90, 0)), new Vector3(1.0f, 1.0f, 1.0f));
    //    //        Gizmos.DrawFrustum(Vector3.zero, currentEditorHandPose.GetToleranceAngle() * 2, debugToleranceLength, 0, 1);
    //    //    }
    //    //}

        
    //}
#endif

}
