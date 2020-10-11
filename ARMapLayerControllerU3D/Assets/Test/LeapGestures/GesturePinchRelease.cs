//Harry Chen 2018/07/15

using Leap;
using Leap.Unity;
using Leap.Unity.Infix;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GesturePinchRelease : LeapGestureModel
{
    protected float lastTipGap;

    public GesturePinchRelease()
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
                }
            }
            lastTipGap = Vector3.Distance(thumbTip, indexTip);
            if (lastTipGap < 0.3 * hand.PalmWidth)
            {
                Debug.Log("(PinchRelease) Gesture Start");
                return true;
            }
            else
            {
                Debug.Log("PinchRelease End Problem");
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
                }
            }
            if (lastTipGap <= Vector3.Distance(thumbTip, indexTip))
            {
                lastTipGap = Vector3.Distance(thumbTip, indexTip);
                Debug.Log("(PinchRelease) Gesture In Process");
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
                }
            }
            float lastTipGap = Vector3.Distance(thumbTip, indexTip);
            if (lastTipGap > 0.8 * hand.PalmWidth)
            {
                Debug.Log("(PinchRelease) Gesture CheckEnd");
                return true;
            }
            else
            {
                Debug.Log("(PinchRelease) Gesture CheckEnd Problem");
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
        Debug.Log("(PinchRelease) Gesture Detected");
        return;
    }
}
