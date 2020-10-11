//Harry Chen 2018/07/15

using Leap;
using Leap.Unity;
using Leap.Unity.Infix;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GestureGrab : LeapGestureModel
{
    protected Vector3 _lastPalmPos;
    public Vector3 LastPalmPos
    {
        get
        {
            return _lastPalmPos;
        }
    }

    protected Dictionary<Finger.FingerType, float> currentFingerAngle = new Dictionary<Finger.FingerType, float>();
    protected float currentGrabAngle = 0;
    public GestureGrab()
    {
        setProcessTime(1f);
    }

    //to be override as detecting the start signal of a gusture
    public override bool checkStart()
    {
        //1. Only one hand input
        //2. The direction of fingers (no thumb) are close to the palm direction
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
            currentGrabAngle = hand.GrabAngle;
            _lastPalmPos = UnityVectorExtension.ToVector3(hand.PalmPosition);
            return (currentGrabAngle < 2);
            /*
            currentFingerAngle.Clear();
            foreach (Finger finger in hand.Fingers)
            {
                if (finger.Type != Finger.FingerType.TYPE_THUMB && finger.Type != Finger.FingerType.TYPE_PINKY)
                {
                    float angle = Vector3.Angle(hand.Direction.ToVector3(), finger.Direction.ToVector3());
                    currentFingerAngle.Add(finger.Type, angle);
                    if ( angle > 90.0f)
                    {
                        Debug.Log("(Grab)Start finger direction problem ");
                        return false;
                    }
                }
            }
            Debug.Log("(Grab)Gesture start");
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
        //1. Only one hand input
        //2. The fingers are moving into palm
        //3. Time is in limit
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
            if (hand.GrabAngle >= currentGrabAngle)
            {
                currentGrabAngle = hand.GrabAngle;
                _lastPalmPos = UnityVectorExtension.ToVector3(hand.PalmPosition);
                return true;
            }
            return false;/*
            foreach (Finger finger in hand.Fingers)
            {
                if (finger.Type != Finger.FingerType.TYPE_THUMB && finger.Type != Finger.FingerType.TYPE_PINKY)
                {
                    float angle = Vector3.Angle(hand.Direction.ToVector3(), finger.Direction.ToVector3());
                    if (angle < currentFingerAngle[finger.Type])
                    {
                        Debug.Log("(Grab)InProcess finger movement problem ");
                        return false;
                    }
                    else
                    {
                        currentFingerAngle.Remove(finger.Type);
                        currentFingerAngle.Add(finger.Type, angle);
                    }
                }
            }
            if (countTime > ProcessTime)
            {
                //Debug.Log("(Grab)InProcess time limit problem");
                return false;
            }
            Debug.Log("(Grab)Gesture in process");
            _lastPalmPos = UnityVectorExtension.ToVector3(hand.PalmPosition);
            return true;*/
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
    }

    //to be override as detecting if the gesture is finished
    public override bool checkEnd()
    {
        //1. Only one hand input
        //2. The fingers have moved around hand
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
            return (currentGrabAngle > 3);
            /*
            foreach (Finger finger in hand.Fingers)
            {
                if (finger.Type != Finger.FingerType.TYPE_THUMB && finger.Type != Finger.FingerType.TYPE_PINKY)
                {
                    float angle = Vector3.Angle(hand.Direction.ToVector3(), finger.Direction.ToVector3());
                    if (angle < 120)
                    {
                        Debug.Log("(Grab)End finger movement problem ");
                        return false;
                    }
                }
            }
            _lastPalmPos = UnityVectorExtension.ToVector3(hand.PalmPosition);
            return true;*/
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
        Debug.Log("Grab detected");
        return;
    }
}
