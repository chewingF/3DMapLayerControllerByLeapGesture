//Harry Chen 2018/07/15

using Leap;
using Leap.Unity;
using Leap.Unity.Infix;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GestureGrabbing : LeapGestureModel
{

    protected Vector3 _lastPalmPos;
    public Vector3 LastPalmPos
    {
        get
        {
            return _lastPalmPos;
        }
    }


    protected Vector3 _palmMovement;
    public Vector3 PalmMovement
    {
        get
        {
            return _palmMovement;
        }
    }

    public GestureGrabbing()
    {
        //setProcessTime(1f);
    }

    //to be override as detecting the start signal of a gusture
    public override bool checkStart()
    {
        //1. Only one hand input
        //2. The fingers have moved around hand
        Hand hand = new Hand();
        if ((leftHand == null) == (rightHand == null))
        {
            //Debug.Log("(Grab)Start hand number problem ");
            return false;
        }
        else
        {
            if (leftHand != null)
            {
                hand = leftHand;
            }
            else
            {
                hand = rightHand;
            }
        }
        try
        {
            _lastPalmPos = UnityVectorExtension.ToVector3(hand.PalmPosition);
            _palmMovement = new Vector3();
            return (hand.GrabAngle > 3);/*
            foreach (Finger finger in hand.Fingers)
            {
                if (finger.Type != Finger.FingerType.TYPE_THUMB && finger.Type != Finger.FingerType.TYPE_PINKY)
                {
                    float angle = Vector3.Angle(hand.Direction.ToVector3(), finger.Direction.ToVector3());
                    if ( angle < 120.0f)
                    {
                        Debug.Log("(Grabbing)Start finger direction problem ");
                        return false;
                    }
                }
            }
            Debug.Log("(Grabbing)Gesture start");
            _lastPalmPos = UnityVectorExtension.ToVector3(hand.PalmPosition);
            return true;*/
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
    }

    //to be override as detecting if the gesture is still in process
    public override bool checkInProcess()
    {
        Hand hand = new Hand();
        if ((leftHand == null) == (rightHand == null))
        {
            //Debug.Log("(Grab)Start hand number problem ");
            return false;
        }
        else
        {
            if (leftHand != null)
            {
                hand = leftHand;
            }
            else
            {
                hand = rightHand;
            }
        }
        _palmMovement = UnityVectorExtension.ToVector3(hand.PalmPosition) - _lastPalmPos;
        _lastPalmPos = UnityVectorExtension.ToVector3(hand.PalmPosition);
        Debug.Log("(Grabbing) Gesture InProcess");
        return true;
    }

    //to be override as detecting if the gesture is finished
    public override bool checkEnd()
    {
        //1. Only one hand input
        //2. The direction of fingers (no thumb) are close to the palm direction
        Hand hand = new Hand();
        if ((leftHand == null) == (rightHand == null))
        {
            return false;
        }
        else
        {
            if (leftHand != null)
            {
                hand = leftHand;
            }
            else
            {
                hand = rightHand;
            }
        }
        try
        {
            return (hand.GrabAngle <3);/*
            bool grabbingEnd = false;
            foreach (Finger finger in hand.Fingers)
            {
                if (finger.Type != Finger.FingerType.TYPE_THUMB && finger.Type != Finger.FingerType.TYPE_PINKY)
                {
                    float angle = Vector3.Angle(hand.Direction.ToVector3(), finger.Direction.ToVector3());
                    if (angle < 120)
                    {
                        Debug.Log("(Grabbing)End finger movement problem ");
                        grabbingEnd = true;
                    }
                }
            }
            return grabbingEnd;*/
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
    }

    //to be override as the commend give by the gesture
    public override void gestureDetected()
    {
        Debug.Log("Grabbing detected");
        return;
    }
}
