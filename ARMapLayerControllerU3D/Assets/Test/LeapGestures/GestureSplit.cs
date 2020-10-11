//Harry Chen 2018/07/15

using Leap.Unity;
using Leap.Unity.Infix;
using System;
using UnityEngine;

public class GestureSplit : LeapGestureModel
{
    /*
    protected Vector3 _lastPlamPosR;
    public Vector3 LastPlamPosR
    {
        get
        {
            return _lastPlamPosR;
        }
    }

    protected Vector3 _lastPlamPosL;
    public Vector3 LastPlamPosL
    {
        get
        {
            return _lastPlamPosL;
        }
    }*/

    public GestureSplit()
    {
        setProcessTime(1f);
    }

    //to be override as detecting the start signal of a gusture
    public override bool checkStart()
    {
        //1.Two hands palms are facing each other
        //2.(Two hands palms direction are heading top)
        //3.Two hands palms' distance is smaller than the palm width.
        try
        {
            if (Vector3.Angle(leftHand.PalmNormal.ToVector3(), rightHand.PalmNormal.ToVector3()) < 120.0f)
            {
                //Debug.Log("(Split)Start palms facing problem ");
                return false;
            }
            if (Vector3.Distance(leftHand.PalmPosition.ToVector3(), rightHand.PalmPosition.ToVector3()) > (1.5 * leftHand.PalmWidth))
            {
                //Debug.Log("(Split)Start palms distance problem");
                return false;
            }
            Debug.Log("(Split)Gesture start");
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
        //1.Two hands palms are facing each other
        //2.(Two hands palms direction are heading top)
        //3.Two hands palms movement are opposite to the thier palm facing side
        //4.(Two hands palms movement speed is faster than a limit)
        //5.count time is less them process time limitation
        try
        {
            if (Vector3.Angle(leftHand.PalmNormal.ToVector3(), rightHand.PalmNormal.ToVector3()) < 120.0f)
            {
                //Debug.Log("(Split)InProcess palms facing problem");
                return false;
            }
            if (Vector3.Angle(leftHand.PalmVelocity.ToVector3(), leftHand.PalmNormal.ToVector3()) < 120.0f)
            {
                //Debug.Log("(Split)InProcess left hand movement problem");
                return false;
            }
            if (Vector3.Angle(rightHand.PalmVelocity.ToVector3(), rightHand.PalmNormal.ToVector3()) < 120.0f)
            {
                //Debug.Log("(Split)InProcess right hand movement problem");
                return false;
            }
            if (countTime > ProcessTime)
            {
                //Debug.Log("(Split)InProcess time limit problem");
                return false;
            }
            Debug.Log("(Split)Gesture in process");
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
        //1.Two hands palms are facing each other
        //2.(Two hands palms direction are heading top)
        //3.Two hands palms distance is larger than twice of the palm width.
        try
        {
            if (Vector3.Angle(leftHand.PalmNormal.ToVector3(), rightHand.PalmNormal.ToVector3()) < 120.0f)
            {
                //Debug.Log("(Split)End palms facing problem");
                return false;
            }
            if (Vector3.Distance(leftHand.PalmPosition.ToVector3(), rightHand.PalmPosition.ToVector3()) < (3 * leftHand.PalmWidth))
            {
                //Debug.Log("(Split)End palms distance problem");
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
        Debug.Log("split detected");
        return;
    }
}
