using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Oculus.Interaction.Input;
using UnityEditor;
using System;

[ExecuteInEditMode]
public class HandPose : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _jointTransforms = new List<Transform>();

    [Header("Pose Joints")]

    [SerializeField] Transform handWristRoot;
    [SerializeField] Transform handForearmStub;
    [SerializeField] Transform handThumb0;
    [SerializeField] Transform handThumb1;
    [SerializeField] Transform handThumb2;
    [SerializeField] Transform handThumb3;
    [SerializeField] Transform handIndex1;
    [SerializeField] Transform handIndex2;
    [SerializeField] Transform handIndex3;
    [SerializeField] Transform handMiddle1;
    [SerializeField] Transform handMiddle2;
    [SerializeField] Transform handMiddle3;
    [SerializeField] Transform handRing1;
    [SerializeField] Transform handRing2;
    [SerializeField] Transform handRing3;
    [SerializeField] Transform handPinky0;
    [SerializeField] Transform handPinky1;
    [SerializeField] Transform handPinky2;
    [SerializeField] Transform handPinky3;
    [SerializeField] Transform handThumbTip;
    [SerializeField] Transform handIndexTip;
    [SerializeField] Transform handMiddleTip;
    [SerializeField] Transform handRingTip;
    [SerializeField] Transform handPinkyTip;

    Dictionary<HandJointId, Pose> localPoses;
    Dictionary<HandJointId, Pose> worldPoses;

    [Header("Pose Properties")]

    [SerializeField] float toleranceAngle = 20f;

    [Header("Debug Values")]

    [SerializeField] bool debugDrawBones = true;
    [SerializeField] float debugFingertipLength = 0.025f;

    [SerializeField] bool debugDrawJoints = true;
    [SerializeField] float debugJointRadius = .005f;

    [SerializeField] bool debugDrawTolerance = true;
    [SerializeField] float debugToleranceLength = .01f;


    public float GetToleranceAngle() { return toleranceAngle; }

    /// <summary>
    /// Clears joint transforms, resets those references, resets local poses, and resets world poses
    /// </summary>
    public void ClearPose()
    {
        _jointTransforms.Clear();
        _jointTransforms.Add(handWristRoot);
        _jointTransforms.Add(handForearmStub);
        _jointTransforms.Add(handThumb0);
        _jointTransforms.Add(handThumb1);
        _jointTransforms.Add(handThumb2);
        _jointTransforms.Add(handThumb3);
        _jointTransforms.Add(handIndex1);
        _jointTransforms.Add(handIndex2);
        _jointTransforms.Add(handIndex3);
        _jointTransforms.Add(handMiddle1);
        _jointTransforms.Add(handMiddle2);
        _jointTransforms.Add(handMiddle3);
        _jointTransforms.Add(handRing1);
        _jointTransforms.Add(handRing2);
        _jointTransforms.Add(handRing3);
        _jointTransforms.Add(handPinky0);
        _jointTransforms.Add(handPinky1);
        _jointTransforms.Add(handPinky2);
        _jointTransforms.Add(handPinky3);

        localPoses = new Dictionary<HandJointId, Pose>();
        worldPoses = new Dictionary<HandJointId, Pose>();
    }

    /// <summary>
    /// Used to set joints in hand pose to match those in the passed hand
    /// </summary>
    /// <param name="hand">Hand to be copied</param>
    public void SetHandPose(IHand hand)
    {
        //for (int i = 1; i < (int)HandJointId.HandMaxSkinnable; i++)
        //{
        //    hand.GetJointPoseFromWrist((HandJointId)i, out Pose wristLocalPose);
        //    _jointTransforms[i].transform.position = wristLocalPose.position * 1000;
        //    _jointTransforms[i].transform.rotation = wristLocalPose.rotation;

        //    hand.GetJointPoseLocal((HandJointId)i, out Pose localPose);
        //    localPoses.Add((HandJointId)i, localPose);

        //    hand.GetJointPose((HandJointId)i, out Pose worldPose);
        //    worldPoses.Add((HandJointId)i, worldPose);
        //}

        // Get the Prefab Asset root GameObject and its asset path.
        string assetPath = AssetDatabase.GetAssetPath(gameObject);

        // Load the contents of the Prefab Asset.
        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(assetPath);

        // Modify Prefab contents.
        HandPose rootHandPose = contentsRoot.GetComponent<HandPose>();

        if (rootHandPose != null)
        {
            rootHandPose.ClearPose();

            for (int i = 0; i < (int)HandJointId.HandMaxSkinnable; i++)
            {
                //hand.GetJointPoseFromWrist((HandJointId)i, out Pose wristLocalPose);
                //rootHandPose._jointTransforms[i].transform.position = wristLocalPose.position;
                //rootHandPose._jointTransforms[i].transform.rotation = wristLocalPose.rotation;

                hand.GetJointPoseLocal((HandJointId)i, out Pose localPose);
                rootHandPose.localPoses.Add((HandJointId)i, localPose);

                rootHandPose._jointTransforms[i].transform.localPosition = localPose.position;
                rootHandPose._jointTransforms[i].transform.localRotation = localPose.rotation;

                hand.GetJointPose((HandJointId)i, out Pose worldPose);
                rootHandPose.worldPoses.Add((HandJointId)i, worldPose);
            }
        }
        else
        {
            Debug.LogError("contentsRoot does not have a HandPose component");
        }

        // Save contents back to Prefab Asset and unload contents.
        PrefabUtility.SaveAsPrefabAsset(contentsRoot, assetPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
    }


    public bool CheckHandMatch(IHand hand, float toleranceMultiplier)
    {
        for (int i = 0; i < (int)HandJointId.HandMaxSkinnable; i++)
        {
            //Check each joint local angle
            hand.GetJointPoseLocal((HandJointId)i, out Pose localPose);
            if (Mathf.Abs(Quaternion.Angle(localPose.rotation, _jointTransforms[i].transform.localRotation)) > toleranceAngle * toleranceMultiplier)
            {
                Debug.Log(Mathf.Abs(Quaternion.Angle(localPose.rotation, _jointTransforms[i].transform.localRotation)));
                return false;
            }
        }

        return true;
    }


    private void OnDrawGizmos()
    {
        if (_jointTransforms == null) return;
        if (_jointTransforms.Count <= 0) return;

        Gizmos.color = Color.red;

        if (debugDrawBones)
        {
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandWristRoot].position, _jointTransforms[(int)HandJointId.HandThumb0].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandThumb0].position, _jointTransforms[(int)HandJointId.HandThumb1].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandThumb1].position, _jointTransforms[(int)HandJointId.HandThumb2].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandThumb2].position, _jointTransforms[(int)HandJointId.HandThumb3].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandThumb3].position, 
                _jointTransforms[(int)HandJointId.HandThumb3].position + _jointTransforms[(int)HandJointId.HandThumb3].right * debugFingertipLength);
                
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandWristRoot].position, _jointTransforms[(int)HandJointId.HandIndex1].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandIndex1].position, _jointTransforms[(int)HandJointId.HandIndex2].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandIndex2].position, _jointTransforms[(int)HandJointId.HandIndex3].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandIndex3].position,
                _jointTransforms[(int)HandJointId.HandIndex3].position + _jointTransforms[(int)HandJointId.HandIndex3].right * debugFingertipLength);

            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandWristRoot].position, _jointTransforms[(int)HandJointId.HandMiddle1].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandMiddle1].position, _jointTransforms[(int)HandJointId.HandMiddle2].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandMiddle2].position, _jointTransforms[(int)HandJointId.HandMiddle3].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandMiddle3].position,
                _jointTransforms[(int)HandJointId.HandMiddle3].position + _jointTransforms[(int)HandJointId.HandMiddle3].right * debugFingertipLength);

            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandWristRoot].position, _jointTransforms[(int)HandJointId.HandRing1].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandRing1].position, _jointTransforms[(int)HandJointId.HandRing2].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandRing2].position, _jointTransforms[(int)HandJointId.HandRing3].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandRing3].position,
                _jointTransforms[(int)HandJointId.HandRing3].position + _jointTransforms[(int)HandJointId.HandRing3].right * debugFingertipLength);

            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandWristRoot].position, _jointTransforms[(int)HandJointId.HandPinky0].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandPinky0].position, _jointTransforms[(int)HandJointId.HandPinky1].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandPinky1].position, _jointTransforms[(int)HandJointId.HandPinky2].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandPinky2].position, _jointTransforms[(int)HandJointId.HandPinky3].position);
            Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandPinky3].position,
                _jointTransforms[(int)HandJointId.HandPinky3].position + _jointTransforms[(int)HandJointId.HandPinky3].right * debugFingertipLength);
        }

        Gizmos.color = Color.blue;

        if (debugDrawJoints)
        {
            for (int i = 1; i < (int)HandJointId.HandMaxSkinnable; i++)
            {
                Gizmos.DrawSphere(_jointTransforms[i].position, debugJointRadius);
            }
        }

        //Orange
        Gizmos.color = new Color(1, .3f, 0, .5f);

        if (debugDrawTolerance)
        {
            for (int i = 1; i < (int)HandJointId.HandMaxSkinnable; i++)
            {
                Gizmos.matrix = Matrix4x4.TRS(_jointTransforms[i].position, _jointTransforms[i].rotation * Quaternion.Euler(new Vector3(0, 90, 0)), new Vector3(1.0f, 1.0f, 1.0f));
                Gizmos.DrawFrustum(Vector3.zero, toleranceAngle * 2, debugToleranceLength, 0, 1);
            }
        }
    }
}
