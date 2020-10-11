using UnityEngine;
using UnityEditor;

public class TouchableObject
{
    protected GameObject worldObject;

    protected float grabbingRange = 0.01f;
    private bool _isGrabbed;
    public bool IsGrabbed
    {
        get
        {
            return _isGrabbed;
        }
    }

    protected float pinchingRange = 0f;
    private bool _isPinched;
    public bool IsPinched
    {
        get
        {
            return _isPinched;
        }
    }

    protected float tappingRange = 0f;
    private bool _isTapped;
    public bool IsTapped
    {
        get
        {
            return _isTapped;
        }
    }
    private bool _isTapping;
    public bool IsTapping
    {
        get
        {
            return _isTapping;
        }
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get
        {
            return _isSelected;
        }
    }

    public GameObject getMetaphorObject()
    {
        return worldObject;
    }


    public TouchableObject()
    {

    }

    public void moveObjectTo(Vector3 targetPos)
    {
        if (worldObject == null)
        {
            return;
        }
        worldObject.transform.position = targetPos;
    }

    public void moveObjectBy(Vector3 posMovement)
    {
        if (worldObject == null)
        {
            return;
        }
        Vector3 orgPos = worldObject.transform.position;
        worldObject.transform.position = orgPos + posMovement;
    }

    public void rotateObjectBy(Vector3 rotationChange)
    {
        if (worldObject == null)
        {
            return;
        }
        worldObject.transform.Rotate(rotationChange);
    }

    public bool checkGrabbing(Vector3 grabbingPos)
    {
        Collider coll;
        if (worldObject == null)
        {
            _isGrabbed = false;
            return _isGrabbed;
        }
        try
        {
            coll = worldObject.GetComponent<Collider>();
        }
        catch
        {
            Debug.Log("GameObject has no Collider");
            return false;
        }
        Vector3 closestPoint = coll.ClosestPointOnBounds(grabbingPos);
        float distance = Vector3.Distance(closestPoint, grabbingPos);
        //Debug.Log("grabbing distance " + distance);
        return (distance <= grabbingRange);
    }

    public bool checkPinching(Vector3 pinchingPos)
    {
        Collider coll;
        if (worldObject == null)
        {
            return false;
        }
        try
        {
            coll = worldObject.GetComponent<Collider>();
        }
        catch
        {
            Debug.Log("GameObject has no Collider");
            return false;
        }
        Vector3 closestPoint = coll.ClosestPointOnBounds(pinchingPos);
        float distance = Vector3.Distance(closestPoint, pinchingPos);
        //Debug.Log("pinching distance " + distance);
        return (distance <= pinchingRange);
    }

    public bool checkTapping(Vector3 tappingPos)
    {
        Collider coll;
        if (worldObject == null)
        {
            return false;
        }
        try
        {
            coll = worldObject.GetComponent<Collider>();
        }
        catch
        {
            Debug.Log("GameObject has no Collider");
            return false;
        }
        Vector3 closestPoint = coll.ClosestPointOnBounds(tappingPos);
        float distance = Vector3.Distance(closestPoint, tappingPos);
        //Debug.Log("tapping distance " + distance);
        return (distance <= pinchingRange);
    }

    public float distanceToBoundary(Vector3 pinchingPos)
    {
        Collider coll = worldObject.GetComponent<Collider>();
        Vector3 closestPoint = coll.ClosestPointOnBounds(pinchingPos);
        float distance = Vector3.Distance(closestPoint, pinchingPos);
        return distance;
    }

    public virtual float distanceToCenter(Vector3 pinchingPos)
    {
        Collider coll = worldObject.GetComponent<Collider>();
        float distance = Vector3.Distance(coll.bounds.center, pinchingPos);
        return distance;
    }

    public void startGrab()
    {
        _isGrabbed = true;
    }

    public void startPinch()
    {
        _isPinched = true;
    }

    public void startTap()
    {
        _isTapped = true;
    }

    public void startTapping()
    {
        _isTapping = true;
    }

    public void endGrab()
    {
        _isGrabbed = false;
    }

    public void endPinch()
    {
        _isPinched = false;
    }

    public void endTap()
    {
        _isTapped = false;
    }

    public void endTapping()
    {
        _isTapping = false;
    }

    public void endSelectState()
    {
        _isSelected = false;
    }

    public void changeTap()
    {
        _isTapped = !_isTapped;
    }

    public void changeTapping()
    {
        _isTapping = !_isTapping;
    }

    public void changeSelectState()
    {
        _isSelected = !_isSelected;
    }


    public void reColorWorldObj(Color newColor)
    {
        try
        {
            MeshRenderer mr = worldObject.GetComponent<MeshRenderer>();
            mr.material.color = newColor;
        }
        catch
        {

        }
    }
}