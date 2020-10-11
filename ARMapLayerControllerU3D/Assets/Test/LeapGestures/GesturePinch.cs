//Harry Chen 2018/07/15

using Leap;
using Leap.Unity;
using Leap.Unity.Infix;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GesturePinch : LeapGestureModel
{
    protected float lastTipGap;
    protected Vector3 _lastTipPos;
    public Vector3 LastTipPos
    {
        get
        {
            return _lastTipPos;
        }
    }

    public GesturePinch()
    {
        setProcessTime(1f);
    }

    //to be override as detecting the start signal of a gusture
    public override bool checkStart()
    {
        //1. Only one hand input
        //2. Thumb far from Index
        Hand hand = new Hand();
        if ((leftHand == null) == (rightHand == null))
        {
            Debug.Log("(Pinch)Start hand number problem ");
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
                    indexTip = finger.TipPosition.ToVector3();
                    _lastTipPos = UnityVectorExtension.ToVector3(finger.TipPosition);
                }
            }
            float lastTipGap = Vector3.Distance(thumbTip, indexTip);
            if (lastTipGap > 0.8 * hand.PalmWidth)
            {
                Debug.Log("(Pinch)Gesture Start");
                return true;
            }
            else
            {
                Debug.Log("(Pinch)Gesture Start fingers distance problem");
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
        //1. Only one hand input
        //2. The fingers are moving closer
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
                    indexTip = finger.TipPosition.ToVector3();
                    _lastTipPos = UnityVectorExtension.ToVector3(finger.TipPosition);
                }
            }
            if (lastTipGap >= Vector3.Distance(thumbTip, indexTip))
            {
                lastTipGap = Vector3.Distance(thumbTip, indexTip);
                Debug.Log("(Pinch)Gesture InProcess");
                return true;
            }
            else
            {
                lastTipGap = Vector3.Distance(thumbTip, indexTip);
                return false;
            }
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
        //2. The fingers have moved together
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
            foreach (Finger finger in hand.Fingers)
            {
                if (finger.Type == Finger.FingerType.TYPE_THUMB)
                {
                    thumbTip = finger.TipPosition.ToVector3();
                }
                else if (finger.Type == Finger.FingerType.TYPE_INDEX)
                {
                    indexTip = finger.TipPosition.ToVector3();
                    _lastTipPos = UnityVectorExtension.ToVector3(finger.TipPosition);
                }
            }
            lastTipGap = Vector3.Distance(thumbTip, indexTip);
            if (lastTipGap < 0.3*hand.PalmWidth)
            {
                return true;
            }
            else
            {
                Debug.Log("Pinch End Problem");
                return false;
            }
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
        Debug.Log("Pinch detected");
        return;
    }
}
