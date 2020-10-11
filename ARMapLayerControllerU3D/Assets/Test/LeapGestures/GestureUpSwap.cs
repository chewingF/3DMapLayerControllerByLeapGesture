//Harry Chen 2018/07/31

using Leap;
using Leap.Unity;
using Leap.Unity.Infix;
using System;
using UnityEngine;

public class GestureUpSwap : LeapGestureModel
{

    protected Vector3 orgPosition;
    public GestureUpSwap()
    {
        setProcessTime(1f);
    }

    //to be override as detecting the start signal of a gusture
    public override bool checkStart()
    {
        //1. Only one hand input
        //2. The palm is facing up
        Hand hand = new Hand();
        if ((leftHand == null) == (rightHand == null))
        {
            //Debug.Log("(UpSwap)Start hand number problem ");
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
            if (Vector3.Angle(hand.PalmNormal.ToVector3(), new Vector3(0, -1, 0)) > 60.0f)
            {
                Debug.Log("(UpSwap)Start palms facing problem ");
                return false;
            }
            Debug.Log("(UpSwap)Gesture start");
            orgPosition = hand.PalmPosition.ToVector3();
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
        //1. Only one hand input
        //2. The palm is facing up
        //3. The plam is moving to up
        //4. Time is in limit
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
            if (Vector3.Angle(hand.PalmNormal.ToVector3(), new Vector3(0, -1, 0)) > 60.0f)
            {
                //Debug.Log("(UpSwap)Start palm facing problem ");
                return false;
            }
            if (Vector3.Angle(hand.PalmVelocity.ToVector3(), new Vector3(0, -1, 0)) > 60.0f)
            {
                //Debug.Log("(UpSwap)Start palm movement problem ");
                return false;
            }
            if (countTime > ProcessTime)
            {
                //Debug.Log("(UpSwap)InProcess time limit problem");
                return false;
            }
            Debug.Log("(UpSwap)Gesture in process");
            return true;
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
        //2. The palm is facing up
        //3. The plam is moving to up
        //4. The distance from current position to original one is larger than twice of palm width.
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
            if (Vector3.Angle(hand.PalmNormal.ToVector3(), new Vector3(0, -1, 0)) > 60.0f)
            {
                //Debug.Log("(UpSwap)End palm facing problem");
                return false;
            }
            if (Vector3.Angle(hand.PalmVelocity.ToVector3(), new Vector3(0, -1, 0)) > 60.0f)
            {
                //Debug.Log("(UpSwap)Start palm movement problem ");
                return false;
            }
            if (Vector3.Distance(hand.PalmPosition.ToVector3(), orgPosition) < (1.5 * hand.PalmWidth))
            {
                //Debug.Log("(UpSwap)End palm distance problem");
                return false;
            }
            return true;
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
        Debug.Log("UpSwap detected");
        return;
    }
}
