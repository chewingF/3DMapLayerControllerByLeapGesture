using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap;
using Leap.Unity;

public class LeapGesturesDetecter : MonoBehaviour {

    protected LeapXRServiceProvider controller;
    [SerializeField]
    GameObject leftHandInfo, rightHandInfo;

    //gesture models
    LeapGestureModel gestureSplit, gestureClap, gestureLeftSwap, gestureRightSwap, 
        gestureUpSwap, gestureDownSwap,gestureGrab, gesturePinch;

    // Use this for initialization
    void Start ()
    {
        //set up all gesture models
        gestureSplit = new GestureSplit();
        gestureClap = new GestureClap();
        gestureLeftSwap = new GestureLeftSwap();
        gestureRightSwap = new GestureRightSwap();
        gestureUpSwap = new GestureUpSwap();
        gestureDownSwap = new GestureDownSwap();
        gestureGrab = new GestureGrab();
        gesturePinch = new GesturePinch();
    }
	
	// Update is called once per frame
	void Update () {
        controller = new LeapXRServiceProvider();
        Frame frame = controller.GetLeapController().Frame();
        List<Hand> hands = frame.Hands;
        Hand leftHand = new Hand();
        bool leftHandExist = false;
        Hand rightHand = new Hand();
        bool rightHandExist = false;
        foreach (Hand hand in hands)
        {
            if (hand.IsLeft)
            {
                leftHand = hand;
                setText(hand, leftHandInfo);
                leftHandExist = true;
            }
            else
            {
                rightHand = hand;
                setText(hand, rightHandInfo);
                rightHandExist = true;
            }
        }
        if (leftHandExist & rightHandExist)
        {
            //check gesture models states
            gestureSplit.Check(leftHand, rightHand, Time.deltaTime);
            gestureClap.Check(leftHand, rightHand, Time.deltaTime);
        }
        if (leftHandExist & !rightHandExist)
        {
            gestureLeftSwap.Check(leftHand, null, Time.deltaTime);
            gestureRightSwap.Check(leftHand, null, Time.deltaTime);
            gestureUpSwap.Check(leftHand, null, Time.deltaTime);
            gestureDownSwap.Check(leftHand, null, Time.deltaTime);
            gestureGrab.Check(leftHand, null, Time.deltaTime);
            gesturePinch.Check(leftHand, null, Time.deltaTime);
        }
        if (!leftHandExist & rightHandExist)
        {
            gestureLeftSwap.Check(null, rightHand, Time.deltaTime);
            gestureRightSwap.Check(null, rightHand, Time.deltaTime);
            gestureUpSwap.Check(null, rightHand, Time.deltaTime);
            gestureDownSwap.Check(null, rightHand, Time.deltaTime);
            gestureGrab.Check(null, rightHand, Time.deltaTime);
            gesturePinch.Check(null, rightHand, Time.deltaTime);
        }
    }

    protected void setText(Hand hand, GameObject handInfo)
    {
        if (handInfo == null)
        {
            return;
        }
        string handInfo_str = "";
        /*
        foreach (Finger finger in hand.Fingers)
        {
            //handInfo_str += (finger.Type + ":" + Vector3.Angle(finger.Direction.ToVector3(), hand.Direction.ToVector3()) + "\n");
            handInfo_str += (finger.Type + ":" + finger.TipPosition.ToVector3() + "\n");
        }
        */
        Vector3 thumbTip = new Vector3();
        Vector3 indexTip = new Vector3();
        foreach(Finger finger in hand.Fingers)
        {
            //handInfo_str += ("\n" + finger.Type + ":" + finger.IsExtended);
            if (finger.Type == Finger.FingerType.TYPE_THUMB)
            {
                thumbTip = UnityVectorExtension.ToVector3(finger.TipPosition);
            }
            else if (finger.Type == Finger.FingerType.TYPE_INDEX)
            {
                indexTip = UnityVectorExtension.ToVector3(finger.TipPosition);
            }
        }
        //handInfo_str += ("PalmarAxis = " + hand.PalmarAxis());
        handInfo_str += ("\nPalmWidh = " + hand.PalmWidth);
        handInfo_str += ("\nGrabAngle = " + hand.GrabAngle);
        //handInfo_str += ("\nGrabStrength = " + hand.GrabStrength);
        //handInfo_str += ("\nPalmPosition = " + UnityVectorExtension.ToVector3(hand.PalmPosition));
        //handInfo_str += ("\nPalmVelocity = " + hand.PalmVelocity.ToVector3());
        handInfo_str += ("\nTipGap = " + Vector3.Distance(thumbTip, indexTip));
        handInfo_str += ("\nRate = " + Vector3.Distance(thumbTip, indexTip)/hand.PalmWidth);

        try
        {
            handInfo.GetComponent<Text>().text = handInfo_str;
        }
        catch
        {
            handInfo.GetComponent<TextMesh>().text = handInfo_str; 
        } 
    }
}
