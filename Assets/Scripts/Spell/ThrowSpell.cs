using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ThrowSpell : MonoBehaviour
{
    Grabbable grabbable;

    int lastFramePointCount = 0;

    public UnityEvent OnGrab;
    public UnityEvent OnRelease;

    Transform palmTrans;

    Rigidbody rb;

    [SerializeField] float throwForce = 10f;
    [SerializeField] float maxDist = 20f;

    public void SetPalmTransform(Transform palmTrans) { this.palmTrans = palmTrans; }

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        grabbable = GetComponent<Grabbable>();
        OnGrab.AddListener(GrabSpell);
        OnRelease.AddListener(SendSpell);
    }

    private void OnDisable()
    {
        OnGrab.RemoveListener(GrabSpell);
        OnRelease.RemoveListener(SendSpell);
    }

    private void Update()
    {
        if(Vector3.Distance(Camera.main.transform.position, transform.position) > maxDist)
        {
            Destroy(gameObject);
        }

        if(grabbable.PointsCount > 0 && //Grabbed
            lastFramePointCount <= 0)   //Last fame was not grabbed
        {
            OnGrab?.Invoke();
        }
        else if (grabbable.PointsCount <= 0 && //Not grabbed
            lastFramePointCount > 0)   //Last fame was grabbed
        {
            OnRelease?.Invoke();
        }

        lastFramePointCount = grabbable.PointsCount;
    }

    void GrabSpell()
    {
        transform.position = palmTrans.position;
    }

    void SendSpell()
    {
        rb.AddForce(-palmTrans.up * throwForce);
    }
}
