using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Oculus.Interaction.Input;
using UnityEditor;

[ExecuteInEditMode]
public class HandPose : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _jointTransforms = new List<Transform>();

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

            for (int i = 1; i < (int)HandJointId.HandMaxSkinnable; i++)
            {
                hand.GetJointPoseFromWrist((HandJointId)i, out Pose wristLocalPose);
                rootHandPose._jointTransforms[i].transform.position = wristLocalPose.position;
                rootHandPose._jointTransforms[i].transform.rotation = wristLocalPose.rotation;

                hand.GetJointPoseLocal((HandJointId)i, out Pose localPose);
                rootHandPose.localPoses.Add((HandJointId)i, localPose);

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (_jointTransforms == null) return;
        if (_jointTransforms.Count <= 0) return;

        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandWristRoot].position, _jointTransforms[(int)HandJointId.HandThumb0].position);
        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandThumb0].position, _jointTransforms[(int)HandJointId.HandThumb1].position);
        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandThumb1].position, _jointTransforms[(int)HandJointId.HandThumb2].position);
        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandThumb2].position, _jointTransforms[(int)HandJointId.HandThumb3].position);

        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandWristRoot].position, _jointTransforms[(int)HandJointId.HandIndex1].position);
        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandIndex1].position, _jointTransforms[(int)HandJointId.HandIndex2].position);
        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandIndex2].position, _jointTransforms[(int)HandJointId.HandIndex3].position);

        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandWristRoot].position, _jointTransforms[(int)HandJointId.HandMiddle1].position);
        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandMiddle1].position, _jointTransforms[(int)HandJointId.HandMiddle2].position);
        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandMiddle2].position, _jointTransforms[(int)HandJointId.HandMiddle3].position);

        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandWristRoot].position, _jointTransforms[(int)HandJointId.HandRing1].position);
        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandRing1].position, _jointTransforms[(int)HandJointId.HandRing2].position);
        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandRing2].position, _jointTransforms[(int)HandJointId.HandRing3].position);

        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandWristRoot].position, _jointTransforms[(int)HandJointId.HandIndex1].position);
        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandIndex1].position, _jointTransforms[(int)HandJointId.HandIndex2].position);
        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandIndex2].position, _jointTransforms[(int)HandJointId.HandIndex3].position);

        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandWristRoot].position, _jointTransforms[(int)HandJointId.HandPinky0].position);
        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandPinky0].position, _jointTransforms[(int)HandJointId.HandPinky1].position);
        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandPinky1].position, _jointTransforms[(int)HandJointId.HandPinky2].position);
        Gizmos.DrawLine(_jointTransforms[(int)HandJointId.HandPinky2].position, _jointTransforms[(int)HandJointId.HandPinky3].position);
    }
}
