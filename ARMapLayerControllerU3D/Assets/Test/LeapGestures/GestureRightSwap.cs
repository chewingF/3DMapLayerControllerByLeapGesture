//Harry Chen 2018/07/19

using Leap;
using Leap.Unity;
using Leap.Unity.Infix;
using System;
using UnityEngine;

public class GestureRightSwap : LeapGestureModel
{

    protected Vector3 orgPosition;
    public GestureRightSwap()
    {
        setProcessTime(1f);
    }

    //to be override as detecting the start signal of a gusture
    public override bool checkStart()
    {
        //1. Only one hand input
        //2. The palm is facing right
        Hand hand = new Hand();
        if ((leftHand == null) == (rightHand == null))
        {
            Debug.Log("(RightSwap)Start hand number problem ");
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
            if (Vector3.Angle(hand.PalmNormal.ToVector3(), new Vector3(-1, 0, 0)) > 60.0f)
            {
                Debug.Log("(RightSwap)Start palms facing problem ");
                return false;
            }
            Debug.Log("(RightSwap)Gesture start");
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
        //2. The palm is facing right
        //3. The plam is moving to right
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
            if (Vector3.Angle(hand.PalmNormal.ToVector3(), new Vector3(-1, 0, 0)) > 60.0f)
            {
                Debug.Log("(RightSwap)InProcess palm facing problem ");
                return false;
            }
            if (Vector3.Angle(hand.PalmVelocity.ToVector3(), new Vector3(-1, 0, 0)) > 60.0f)
            {
                Debug.Log("(RightSwap)InProcess palm movement problem ");
                return false;
            }
            if (countTime > ProcessTime)
            {
                Debug.Log("(RightSwap)InProcess time limit problem");
                return false;
            }
            Debug.Log("(RightSwap)Gesture in process");
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
        //2. The palm is facing right
        //3. The plam is moving to right
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
            if (Vector3.Angle(hand.PalmNormal.ToVector3(),  new Vector3(-1, 0, 0)) > 60.0f)
            {
                //Debug.Log("(RightSwap)End palm facing problem");
                return false;
            }
            if (Vector3.Angle(hand.PalmVelocity.ToVector3(), new Vector3(-1, 0, 0)) > 60.0f)
            {
                //Debug.Log("(RightSwap)Start palm movement problem ");
                return false;
            }
            if (Vector3.Distance(hand.PalmPosition.ToVector3(), orgPosition) < (2 * hand.PalmWidth))
            {
                //Debug.Log("(RightSwap)End palm distance problem");
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
        Debug.Log("RightSwap detected");
        return;
    }
}
