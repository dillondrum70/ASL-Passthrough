using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Oculus.Interaction.Input;
using UnityEditor;
using System;

[ExecuteAlways]
public class HandPose : MonoBehaviour
{
    [SerializeField] string displayName = "";

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

    [Header("Pose Properties")]

    //i.e. If wrist world rotation is ignored, thumbs up and thumbs down would both accept since the hand is the same pose, just the wrist rotates
    [SerializeField] bool ignoreWorldWristRotation = false;

    [SerializeField] float toleranceAngle = 20f;

    [Header("Debug Values")]

    [SerializeField] bool debugDrawBones = true;
    [SerializeField] float debugFingertipLength = 0.025f;

    [SerializeField] bool debugDrawJoints = true;
    [SerializeField] float debugJointRadius = .005f;

    [SerializeField] bool debugDrawTolerance = true;
    [SerializeField] float debugToleranceLength = .01f;


    public string GetDisplayName() { return displayName; }
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
    }

    /// <summary>
    /// Used to set joints in hand pose to match those in the passed hand
    /// </summary>
    /// <param name="hand">Hand to be copied</param>
    /// <param name="physicalHand">The object under which all the hand bones lie</param>
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

            //Get wrist local pose
            hand.GetJointPoseLocal(HandJointId.HandWristRoot, out Pose lPose);

            //Set wrist position
            rootHandPose._jointTransforms[0].transform.localPosition = lPose.position;

            //Set wrist rotation
            if(ignoreWorldWristRotation)
            {
                rootHandPose._jointTransforms[0].transform.localRotation = lPose.rotation;
            }
            else
            {
                hand.GetJointPose(HandJointId.HandWristRoot, out Pose wPose);

                //wrist pose is always static since it is local and it is the root so all the some
                //transformations on the wrist instead come from the world space
                //We must also undo the rotation done by turning your body (head in this case) so it doesn't matter where you're facing when you sign
                Quaternion rot = wPose.rotation;
                rot = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y - Camera.main.transform.eulerAngles.y, rot.eulerAngles.z);
                rootHandPose._jointTransforms[0].transform.rotation = rot;
            }

            //Everything else is local-based because they are all connected to the wrist
            for (int i = 1; i < (int)HandJointId.HandMaxSkinnable; i++)
            {
                //hand.GetJointPoseFromWrist((HandJointId)i, out Pose wristLocalPose);
                //rootHandPose._jointTransforms[i].transform.position = wristLocalPose.position;
                //rootHandPose._jointTransforms[i].transform.rotation = wristLocalPose.rotation;

                hand.GetJointPoseLocal((HandJointId)i, out Pose localPose);

                rootHandPose._jointTransforms[i].transform.localPosition = localPose.position;
                rootHandPose._jointTransforms[i].transform.localRotation = localPose.rotation;
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
        //Get wrist local pose
        //hand.GetJointPoseLocal(HandJointId.HandWristRoot, out Pose lPose);

        //Set wrist rotation
        if (ignoreWorldWristRotation)
        {
            hand.GetJointPoseLocal(HandJointId.HandWristRoot, out Pose localPose);
            if (Mathf.Abs(Quaternion.Angle(localPose.rotation, _jointTransforms[0].transform.localRotation)) > toleranceAngle * toleranceMultiplier)
            {
                return false;
            }
        }
        else
        {
            hand.GetJointPose(HandJointId.HandWristRoot, out Pose wPose);

            //wrist pose is always static since it is local and it is the root so all the some
            //transformations on the wrist instead come from the parent hand
            Quaternion rot = wPose.rotation;
            rot = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y - Camera.main.transform.eulerAngles.y, rot.eulerAngles.z);
            if (Mathf.Abs(Quaternion.Angle(rot, _jointTransforms[0].transform.rotation)) > toleranceAngle * toleranceMultiplier)
            {
                return false;
            }
        }

        for (int i = 1; i < (int)HandJointId.HandMaxSkinnable; i++)
        {
            //Check each joint local angle
            hand.GetJointPoseLocal((HandJointId)i, out Pose localPose);
            if (Mathf.Abs(Quaternion.Angle(localPose.rotation, _jointTransforms[i].transform.localRotation)) > toleranceAngle * toleranceMultiplier)
            {
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
