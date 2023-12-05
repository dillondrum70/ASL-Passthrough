using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using static OVRPlugin;

public struct HandPoseData
{
    public float currentTime;
    public HandPose pose;
}

public class HandPoseTracker : MonoBehaviour
{
    [SerializeField] bool right = true;

    [SerializeField] public List<HandPose> handPoseList;
    [SerializeField] public List<HandGesture> handGestureList;

    List<HandPoseData> handPoseDataStack = new List<HandPoseData>();

    public IHand handCurrent;       //Hand script, RightHand under OVRHands
    public HandVisual handVisual;   //Visual under handCurrent

    HandPose currentPose = null;

    //Generic events fired for every pose
    public UnityEvent<HandPose> OnPoseEnter;
    public UnityEvent<HandPose> OnPoseStay;
    public UnityEvent<HandPose> OnPoseExit;

    //Generic events fired for every gesture
    public UnityEvent<HandGesture> OnGestureEnter;

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
#endif

    private void Start()
    {
        handVisual = GetComponentInParent<HandVisual>();
        handCurrent = handVisual.Hand;
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

        //Get pose of current hand
        foreach(HandPose pose in handPoseList)
        {
            //Check if HandPose matches current hand
            if (pose.CheckHandMatch(handCurrent, toleranceMultiplier))
            {
                //OnEnter
                if (!pose.GetInPose())
                {
                    OnPoseEnter?.Invoke(pose);
                    //pose.OnPoseEnter?.Invoke(pose);
                    inPose = true; //Hand tracker has logged a pose, general
                    pose.SetInPose(true); //This pose is the one that is logged

                    currentPose = pose;
                }

                //OnStay
                OnPoseStay?.Invoke(pose);
                //pose.OnPoseStay?.Invoke(pose);
            }
            else if (pose.GetInPose()) //OnExit
            {
                OnPoseExit?.Invoke(pose);
                //pose.OnPoseExit?.Invoke(pose);
                inPose = false;
                pose.SetInPose(false);

                currentPose = null;
            }
        }

        //Do nothing if current pose is null
        if (currentPose != null)
        {
            if (handPoseDataStack.Count <= 0 ||             //Stack is empty
            handPoseDataStack[0].pose != currentPose)   //or Current pose does not match top of stack
            {
                //Add new pose data struct
                HandPoseData poseData = new HandPoseData();

                poseData.pose = currentPose;
                poseData.currentTime = Time.deltaTime;

                handPoseDataStack.Insert(0, poseData);
            }
            else if (handPoseDataStack[0].pose == currentPose) //Current pose matches pose on top of stack
            {
                //Pop off top element, increase time, push back to stack
                HandPoseData data = handPoseDataStack[0];
                data.currentTime += Time.deltaTime;
                handPoseDataStack[0] = data;
            }

            //NOT PERFORMANT, lots of loops as you add more poses
            //Maybe try a system that uses a temp list and goes through all wrist bones in each pose, remove any hand poses that don't match,
            //then continue to the next pose node but only for the HandPoses that matched at the wrist
            foreach (HandGesture gesture in handGestureList)
            {
                List<HandPose> poses = new(gesture.GetHandPoseList());
                poses.Reverse();

                //Check last pose hold time of gesture is shorter than we've been holding this pose
                if(handPoseDataStack.Count < poses.Count || gesture.GetLastPoseHoldTime() > handPoseDataStack[0].currentTime)
                {
                    //Skip this gesture
                    continue;
                }
                
                //Check order of poses, ensure they match the most recent poses in the stack
                bool match = true;
                for(int i = 0; i < poses.Count; i++)
                {
                    //Exit loop if stack is shorter than pose list or a pose does not match, move to next pose or exit and accept if at end of pose list
                    if (poses[i] != handPoseDataStack[i].pose)
                    {
                        match = false;
                        break;
                    }
                }

                //All poses match
                if(match)
                {
                    OnGestureEnter?.Invoke(gesture);
                    handPoseDataStack.Clear();
                }
            }
        }

        UpdateDisplayHandPose();
    }

    /// <summary>
    /// Display the hand target hand pose
    /// </summary>
    /// <param name="pose">Target pose to display</param>
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

    /// <summary>
    /// Update position and rotation of the displayed hand pose
    /// </summary>
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

#if UNITY_EDITOR

    /// <summary>
    /// Save current hand pose to the passed HandPose, for use when calibrating signs
    /// </summary>
    /// <param name="pose"></param>
    void SaveHandPose(HandPose pose)
    {
        pose.SetHandPose(handCurrent);
    }

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
        foreach (HandPose pose in handPoseList)
        {
            if (pose.GetDisplayName() == displayName) return pose;
        }

        return null;
    }

    /// <summary>
    /// Search list of valid gestures based on the display names in each gesture
    /// </summary>
    /// <param name="displayName">Name to search for</param>
    /// <returns></returns>
    public HandGesture GetHandGesture(string displayName)
    {
        foreach (HandGesture gesture in handGestureList)
        {
            if (gesture.GetDisplayName() == displayName) return gesture;
        }

        return null;
    }
}
