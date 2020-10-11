//Harry Chen 2018/07/15

using Leap;
using Leap.Unity;
using Leap.Unity.Infix;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GestureHovering : LeapGestureModel
{

    protected Vector3 _lastTipPos;
    public Vector3 LastTipPos
    {
        get
        {
            return _lastTipPos;
        }
    }

    protected Vector3 _lastPalmPos;
    public Vector3 LastPalmPos
    {
        get
        {
            return _lastPalmPos;
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


    public GestureHovering()
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
            //Debug.Log("(Hovering)Start hand number problem ");
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
            Vector3 indexTip = new Vector3();
            foreach (Finger finger in hand.Fingers)
            {
                if (finger.Type == Finger.FingerType.TYPE_INDEX)
                {
                    indexTip = finger.TipPosition.ToVector3();
                    _lastTipPos = UnityVectorExtension.ToVector3(finger.TipPosition);
                }
                if (!finger.IsExtended)
                {
                    return false;
                }
            }
            _lastPalmPos = UnityVectorExtension.ToVector3(hand.PalmPosition);
            return true;
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
            //Debug.Log("(Hovering)Start hand number problem ");
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
        _lastPalmPos = UnityVectorExtension.ToVector3(hand.PalmPosition);

        Debug.Log("(Hovering)Gesture InProcess");
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
            foreach (Finger finger in hand.Fingers)
            {
                if (!finger.IsExtended)
                {
                    return true;
                }
            }
            return false;
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
        Debug.Log("Hovering detected");
        return;
    }
}
