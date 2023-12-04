using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using Unity.VisualScripting;
using static OVRPlugin;

public class HandPoseTracker : MonoBehaviour
{
    [SerializeField] bool right = true;

    [SerializeField] public List<HandPose> handPoseList;

    public IHand handCurrent;       //Hand script, RightHand under OVRHands
    public HandVisual handVisual;   //Visual under handCurrent

    //Generic events fired for every pose
    public UnityEvent<HandPose> OnPoseEnter;
    public UnityEvent<HandPose> OnPoseStay;
    public UnityEvent<HandPose> OnPoseExit;

    //Determines if hand is making a pose currently or not
    bool inPose = false;

    //In case we want a difficulty toggle requiring more precise or less precise signs
    float toleranceMultiplier = 1f;

    [Header("Debug")]

    [SerializeField] bool debugDisplayHandPose = true;

    HandPose debugPose;

    [SerializeField] GameObject handPositionMarkerPrefab;
    GameObject debugHandPositionMarker;

#if UNITY_EDITOR
    [Header("Editor")]

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
    }

    private void OnEnable()
    {
        OnPoseEnter.AddListener(DisplayHandPose);
    }

    private void OnDisable()
    {
        OnPoseEnter.RemoveListener(DisplayHandPose);
    }

    private void Update()
    {
#if UNITY_EDITOR
        //Save current hand pose to prefab in currentEditorHandPose
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Return");
            EditorSaveHandPose();
        }
#endif

        //handCurrent.GetJointPoseLocal(Oculus.Interaction.Input.HandJointId.HandIndex1, out Pose index);

        //NOT PERFORMANT, lots of loops as you add more poses
        //Maybe try a system that uses a temp list and goes through all wrist bones in each pose, remove any hand poses that don't match,
        //then continue to the next pose node but only for the HandPoses that matched at the wrist
        foreach (HandPose pose in handPoseList)
        {
            //Check if HandPose matches current hand
            if (pose.CheckHandMatch(handCurrent, toleranceMultiplier))
            {
                //OnEnter
                if (!pose.GetInPose())
                {
                    OnPoseEnter?.Invoke(pose);
                    pose.OnPoseEnter?.Invoke(pose);
                    inPose = true; //Hand tracker has logged a pose, general
                    pose.SetInPose(true); //This pose is the one that is logged
                }

                //OnStay
                OnPoseStay?.Invoke(pose);
                pose.OnPoseStay?.Invoke(pose);
            }
            else if (pose.GetInPose()) //OnExit
            {
                OnPoseExit?.Invoke(pose);
                pose.OnPoseExit?.Invoke(pose);
                inPose = false;
                pose.SetInPose(false);
            }
        }

        UpdateDisplayHandPose();
    }

    //Display the hand target hand pose
    void DisplayHandPose(HandPose pose)
    {
        if (debugPose)
        {
            Destroy(debugHandPositionMarker.gameObject);
            Destroy(debugPose.gameObject);
        }

        debugPose = Instantiate(pose);
        debugHandPositionMarker = Instantiate(handPositionMarkerPrefab);

        float toleranceRadius = debugPose.GetToleranceRadius();
        debugHandPositionMarker.transform.localScale = new Vector3(toleranceRadius, toleranceRadius, toleranceRadius);
    }

    void UpdateDisplayHandPose()
    {
        if (debugDisplayHandPose &&
            debugPose != null)
        {
            debugPose.transform.position = Camera.main.transform.position;
            debugPose.transform.rotation = Quaternion.Euler(
                debugPose.transform.rotation.eulerAngles.x,
                Camera.main.transform.eulerAngles.y,
                debugPose.transform.rotation.eulerAngles.z);

            Transform jointTransform = debugPose.GetJointTransform(0);
            debugHandPositionMarker.transform.position = jointTransform.position;
        }
        else if (debugPose)
        {
            debugPose.enabled = false;
            debugHandPositionMarker.SetActive(false);
        }
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
#endif

    /// <summary>
    /// Search list of valid poses based on the display names in each pose
    /// </summary>
    /// <param name="displayName">Name to search for</param>
    /// <returns></returns>
    public HandPose GetHandPose(string displayName)
    {
        foreach(HandPose pose in handPoseList)
        {
            if (pose.GetDisplayName() == displayName) return pose;
        }

        return null;
    }
}
