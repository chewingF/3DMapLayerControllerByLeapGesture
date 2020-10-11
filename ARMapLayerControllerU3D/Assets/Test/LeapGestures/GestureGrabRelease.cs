//Harry Chen 2018/07/15

using Leap;
using Leap.Unity;
using Leap.Unity.Infix;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GestureGrabRelease : LeapGestureModel
{
    private float currentGrabAngle = 0;
    //private Dictionary<Finger.FingerType, float> currentFingerAngle = new Dictionary<Finger.FingerType, float>();
    public GestureGrabRelease()
    {
        setProcessTime(1f);
    }

    //to be override as detecting the start signal of a gusture
    public override bool checkStart()
    {
        //1. Only one hand input
        //2. The direction of fingers (no thumb) are away from the palm direction
        Hand hand = new Hand();
        if ((leftHand == null) == (rightHand == null))
        {
            //Debug.Log("(Release)Start hand number problem ");
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
            return (currentGrabAngle < 3);
            /*
            currentFingerAngle.Clear();
            foreach (Finger finger in hand.Fingers)
            {
                if (finger.Type != Finger.FingerType.TYPE_THUMB && finger.Type != Finger.FingerType.TYPE_PINKY)
                {
                    float angle = Vector3.Angle(hand.Direction.ToVector3(), finger.Direction.ToVector3());
                    currentFingerAngle.Add(finger.Type, angle);
                    if ( angle < 120.0f)
                    {
                        Debug.Log("(ReleaseGrab)Start finger direction problem ");
                        return false;
                    }
                }
            }
            Debug.Log("(ReleaseGrab)Gesture start");
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
        //2. The fingers are moving away from palm
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
            if (countTime > ProcessTime)
            {
                //Debug.Log("(Release)InProcess time limit problem");
                return false;
            }
            if (hand.GrabAngle > currentGrabAngle)
            {
                Debug.Log("(ReleaseGrab)InProcess finger movement problem ");
                return false;
            }
            else
            {
                currentGrabAngle = hand.GrabAngle;
                Debug.Log("(ReleaseGrab)Gesture in process");
                return true;
            }
            /*
            foreach (Finger finger in hand.Fingers)
            {
                if (finger.Type != Finger.FingerType.TYPE_THUMB && finger.Type != Finger.FingerType.TYPE_PINKY)
                {
                    float angle = Vector3.Angle(hand.Direction.ToVector3(), finger.Direction.ToVector3());
                    if (angle > currentFingerAngle[finger.Type])
                    {
                        Debug.Log("(ReleaseGrab)InProcess finger movement problem ");
                        return false;
                    }
                    else
                    {
                        currentFingerAngle.Remove(finger.Type);
                        currentFingerAngle.Add(finger.Type, angle);
                    }
                }
            }*/
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
        //2. The fingers have moved to palm direction
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
            currentGrabAngle = hand.GrabAngle;
            return (currentGrabAngle < 2);
                /*;
            foreach (Finger finger in hand.Fingers)
            {
                if (finger.Type != Finger.FingerType.TYPE_THUMB && finger.Type != Finger.FingerType.TYPE_PINKY)
                {
                    float angle = Vector3.Angle(hand.Direction.ToVector3(), finger.Direction.ToVector3());
                    if (angle > 90)
                    {
                        Debug.Log("(ReleaseGrab)End finger movement problem ");
                        return false;
                    }
                }
            }
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
        Debug.Log("ReleaseGrab detected");
        return;
    }
}
