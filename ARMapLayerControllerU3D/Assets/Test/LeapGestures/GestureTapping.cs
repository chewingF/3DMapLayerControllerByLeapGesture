//Harry Chen 2018/07/15

using Leap;
using Leap.Unity;
using Leap.Unity.Infix;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GestureTapping : LeapGestureModel
{
    
    protected Vector3 _lastTipPos;
    public Vector3 LastTipPos
    {
        get
        {
            return _lastTipPos;
        }
    }
    
    protected Vector3 _tipMovement;
    public Vector3 TipMovement
    {
        get
        {
            return _tipMovement;
        }
    }


    public GestureTapping()
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
            //Debug.Log("(Tapping)Start hand number problem ");
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
            Vector3 thumbTip = new Vector3();
            Vector3 indexTip = new Vector3();
            foreach (Finger finger in hand.Fingers)
            {
                if (finger.Type == Finger.FingerType.TYPE_THUMB)
                {
                    thumbTip = finger.TipPosition.ToVector3();
                }
                else if (finger.Type == Finger.FingerType.TYPE_INDEX)
                {
                    float angle = Vector3.Angle(hand.Direction.ToVector3(), finger.Direction.ToVector3());
                    if (angle > 60.0f)
                    {
                        Debug.Log("(Tapping)Start index finger direction problem ");
                        return false;
                    }
                    indexTip = finger.TipPosition.ToVector3();
                    _lastTipPos = UnityVectorExtension.ToVector3(finger.TipPosition);
                }
                else
                {
                    float angle = Vector3.Angle(hand.Direction.ToVector3(), finger.Direction.ToVector3());
                    if (angle < 120.0f)
                    {
                        Debug.Log("(Tapping)Start other fingers direction problem ");
                        return false;
                    }
                }
            }
            if (Vector3.Distance(thumbTip, indexTip)>hand.PalmWidth)
            {
                Debug.Log("(Tapping)Gesture start");
                return true;
            }
            else
            {
                Debug.Log("(Tapping)Gesture start problem");
                return false;
            }
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
        foreach (Finger finger in hand.Fingers)
        {
            if (finger.Type == Finger.FingerType.TYPE_INDEX)
            {
                _tipMovement = UnityVectorExtension.ToVector3(finger.TipPosition) - _lastTipPos;
                _lastTipPos = UnityVectorExtension.ToVector3(finger.TipPosition);
            }
        }
        Debug.Log("(Tapping)Gesture InProcess");
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
            Vector3 thumbTip = new Vector3();
            Vector3 indexTip = new Vector3();
            bool tappingEnd = false;
            foreach (Finger finger in hand.Fingers)
            {
                if (finger.Type == Finger.FingerType.TYPE_THUMB)
                {
                    thumbTip = finger.TipPosition.ToVector3();
                }
                else if (finger.Type == Finger.FingerType.TYPE_INDEX)
                {
                    float angle = Vector3.Angle(hand.Direction.ToVector3(), finger.Direction.ToVector3());
                    if (angle > 60.0f)
                    {
                        Debug.Log("(Tapping)End index finger direction problem ");
                        tappingEnd = true;
                    }
                    indexTip = finger.TipPosition.ToVector3();
                    _lastTipPos = UnityVectorExtension.ToVector3(finger.TipPosition);
                }/*
                else
                {
                    float angle = Vector3.Angle(hand.Direction.ToVector3(), finger.Direction.ToVector3());
                    if (angle < 120.0f)
                    {
                        Debug.Log("(Tapping)End other fingers direction problem ");
                        tappingEnd = true;
                    }
                }*/
            }
            if (Vector3.Distance(thumbTip, indexTip) < 0.3* hand.PalmWidth)
            {
                Debug.Log("(Tapping)Gesture End problem");
                tappingEnd = true;
            }
            return tappingEnd;
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
        Debug.Log("Tapping detected");
        return;
    }
}
