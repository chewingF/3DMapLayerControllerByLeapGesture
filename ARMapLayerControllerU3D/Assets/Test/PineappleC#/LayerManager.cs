 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Leap;
using Mapbox.Map;
using System.Linq;
using Leap.Unity;
using UnityEngine.UI;

public class LayerManager : MonoBehaviour {

    public bool deleteBefore = false;
    public bool oldVersion = false;
    private GameObject mapObj = null;
    private AbstractMap mapToEdit;
    private bool Lock = true;//lock other operation when the map is building or animating

    //private List<MapARLayer> mapARLayersActived = new List<MapARLayer>();
    //private List<MapARLayer> mapARLayersDisActived = new List<MapARLayer>();

    public static HandleMetaphor handle;
    public static BinMetaphor bin;
    public static List<LayerMetaphor> layerMetaphorsActived = new List<LayerMetaphor>();
    public static List<LayerMetaphor> layerMetaphorsDisActived = new List<LayerMetaphor>();


    //Leap stuffs
    private Controller controller;
    public LeapProvider _leapProvider;
    //gesture models
    private LeapGestureModel //gestureGrabbingRH, gestureGrabbingLH,
                             //gestureGrabRH, gestureGrabLH,
                             //gestureGrabReleaseRH, gestureGrabReleaseLH,
        gesturePinchRH, gesturePinchLH,
        //gesturePinchingRH, gesturePinchingLH,
        //gesturePinchReleaseRH, gesturePinchReleaseLH,
        //gestureTappingRH, gestureTappingLH,
        gestureHoveringRH, gestureHoveringLH,
        gestureSplit;

    //Grab stuffs

    public static LayerMetaphor goSelected;//goPinchedRH, goPinchedLH, , goTappedRH, goTappedLH, goTappingRH, goTappingLH;
    //private SelectionMetaphor goSelectionSelected;

    [SerializeField]
    //float pinchingTimeMin = 0.2f;

    //Info Panel
    public GameObject infoPanel;

    //Control mode   
    private enum ControlMode
    {
        tappingRotation,
        tapMenu
    }
    [SerializeField]
    ControlMode controlMode;


    // Use this for initialization
    void Start()
    {
        MapLayerInit();
        LeapInit();
        HandleInit();
    }

    // Update is called once per frame
    void Update()
    {
        LayerUpdate();
        if (deleteBefore)
        {
            DeletingLayersForTest();
        }
        GestureListener();
        //HandleUpdate();
        InfoUpdate();
    }
    /*
    private void OnDrawGizmos()
    {
        Controller controller = new Controller();
        Frame frame = controller.Frame();
        List<Hand> hands = frame.Hands;
        foreach (Hand h in hands)
        {
            Gizmos.DrawSphere(UnityVectorExtension.ToVector3(h.Fingers[1].TipPosition), 0.1f);
        }
    }*/

    private void MapLayerInit()
    {
        foreach (GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (gameObj.name == "Map")
            {
                mapObj = gameObj;
            }
        }

        if (mapObj != null)
        {
            mapToEdit = mapObj.GetComponent<AbstractMap>();
            List<VectorSubLayerProperties> mapLayerProperties = (List < VectorSubLayerProperties > ) mapToEdit.VectorData.GetAllFeatureSubLayers();
            foreach (VectorSubLayerProperties vsp in mapLayerProperties)
            {
                MapARLayer newLayer = new MapARLayer(vsp);
                if (vsp.coreOptions.isActive)
                {
                    LayerMetaphor newMetaphor = new LayerMetaphor(newLayer,oldVersion);
                    newMetaphor.oldVersion = oldVersion;
                    layerMetaphorsActived.Add(newMetaphor);
                }
            }
        }
    }

    private void LeapInit()
    {
        _leapProvider = GameObject.FindObjectOfType<LeapProvider>(); //GetTransformGestureManagerBasedMode().GetComponentInChildren<LeapProvider>();

        //set up all gesture models
        //gestureGrabbingLH = new GestureGrabbing();
        //gestureGrabbingRH = new GestureGrabbing();
        //gestureGrabLH = new GestureGrab();
        //gestureGrabRH = new GestureGrab();
        //gestureGrabReleaseLH = new GestureGrabRelease();
        //gestureGrabReleaseRH = new GestureGrabRelease();
        //gesturePinchingLH = new GesturePinching();
        //gesturePinchingRH = new GesturePinching();
        gesturePinchLH = new GesturePinch();
        gesturePinchRH = new GesturePinch();
        //gesturePinchReleaseLH = new GesturePinchRelease();
        //gesturePinchReleaseRH = new GesturePinchRelease();
        //gestureTappingLH = new GestureTapping();
        //gestureTappingRH = new GestureTapping();
        gestureHoveringLH = new GestureHovering();
        gestureHoveringRH = new GestureHovering();
        gestureSplit = new GestureSplit();
    }

    private void HandleInit()
    {
        GameObject handleGo = GameObject.Find("Handle");
        handle = new HandleMetaphor(handleGo);
        handle.oldVersion = oldVersion;
        GameObject binGo = GameObject.Find("Bin");
        bin = new BinMetaphor(binGo);
        if (oldVersion)
        {
            bin.getMetaphorObject().active = false;
        }
        foreach (LayerMetaphor layer in layerMetaphorsActived)
        {
            handle.addLayer(layer);
        }
        //handle.setOnGraspBeginActionForAllLayers(() => OneHandPinch(1));
        //handle.setOnGraspStayActionForAllLayers(() => OneHandPinching(1));
        //handle.setOnGraspEndActionForAllLayers(() => OneHandEndPinching(1));
    }

    //update layer gameobject content when map initialized , moved.
    private void LayerUpdate()
    {
        //check initialized
        var visualizer = mapToEdit.MapVisualizer;
        visualizer.OnMapVisualizerStateChanged += (s) =>
        {

            if (this == null)
                return;

            if (s == ModuleState.Finished)
            {
                if (this.Lock)
                {
                    List<UnwrappedTileId> tileIds = mapToEdit.MapVisualizer.ActiveTiles.Keys.ToList();
                    foreach (LayerMetaphor lm in layerMetaphorsActived)
                    {
                        lm.update(tileIds);
                    }
                    foreach (LayerMetaphor lm in layerMetaphorsDisActived)
                    {
                        lm.update(tileIds);
                    }
                    this.Lock = false;
                    if (!oldVersion)
                    {
                        handle.getMetaphorObject().GetComponent<FaceCamera>().enabled = true;
                        bin.getMetaphorObject().GetComponent<FaceCamera>().enabled = true;
                    }
                }
            }
            else if (s == ModuleState.Working)
            {
                this.Lock = true;
                if (!oldVersion)
                {
                    handle.getMetaphorObject().GetComponent<FaceCamera>().enabled = false;
                    bin.getMetaphorObject().GetComponent<FaceCamera>().enabled = false;
                }
            }
        };
    }


    private void GestureListener()
    { //not editable when building
        if (this.Lock)
        {
            return;
        }
        else
        {
            controller = new Controller();
            Frame frame = _leapProvider.CurrentFrame;
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
                    leftHandExist = true;
                }
                else
                {
                    rightHand = hand;
                    rightHandExist = true;
                }
            }
            //One-Hand Gestures for Left Hand
            if (leftHandExist)
            {
                //LeapGestureModel.State stateGrabLH = gestureGrabLH.Check(leftHand, null, Time.deltaTime);
                //LeapGestureModel.State stateGrabbingLH = gestureGrabbingLH.Check(leftHand, null, Time.deltaTime);
                LeapGestureModel.State statePinchLH = gesturePinchLH.Check(leftHand, null, Time.deltaTime);
                //LeapGestureModel.State statePinchingLH = gesturePinchingLH.Check(leftHand, null, Time.deltaTime);
                //LeapGestureModel.State stateTappingLH = gestureTappingLH.Check(leftHand, null, Time.deltaTime);
                LeapGestureModel.State stateHoveringLH = gestureHoveringLH.Check(leftHand, null, Time.deltaTime);
                /*
                if (stateGrabLH == LeapGestureModel.State.End)
                {
                    OneHandGrab(0);
                }
                if (stateGrabbingLH == LeapGestureModel.State.InProcess)
                {
                    OneHandGrabbing(0);
                }
                if (stateGrabbingLH == LeapGestureModel.State.End)
                {
                    OneHandEndGrabbing(0);
                }*/
                if (statePinchLH == LeapGestureModel.State.End)
                {
                    OneHandPinch(0);
                }/*
                if (statePinchingLH == LeapGestureModel.State.InProcess)
                {
                    //OneHandPinching(0);
                }
                if (statePinchingLH== LeapGestureModel.State.End)
                {
                    //OneHandEndPinching(0);
                }
                if (stateTappingLH == LeapGestureModel.State.InProcess)
                {
                    //OneHandTapping(0);
                }
                if (stateTappingLH == LeapGestureModel.State.End)
                {
                    // OneHandEndTapping(0);
                }*/
                if (stateHoveringLH == LeapGestureModel.State.InProcess)
                {
                    OneHandHovering(0);
                }
            }
            //One-Hand Gestures for Right Hand
            if (rightHandExist)
            {
                //LeapGestureModel.State stateGrabRH = gestureGrabRH.Check(null, rightHand, Time.deltaTime);
                //LeapGestureModel.State stateGrabbingRH = gestureGrabbingRH.Check(null, rightHand, Time.deltaTime);
                LeapGestureModel.State statePinchRH = gesturePinchRH.Check(null, rightHand, Time.deltaTime);
                //LeapGestureModel.State statePinchingRH = gesturePinchingRH.Check(null, rightHand, Time.deltaTime);
                //LeapGestureModel.State stateTappingRH = gestureTappingRH.Check(null, rightHand, Time.deltaTime);
                LeapGestureModel.State stateHoveringRH = gestureHoveringRH.Check(null, rightHand, Time.deltaTime);
                /*if (stateGrabRH == LeapGestureModel.State.End)
                {
                    OneHandGrab(1);
                }
                if (stateGrabbingRH == LeapGestureModel.State.InProcess)
                {
                    OneHandGrabbing(1);
                }
                if (stateGrabbingRH == LeapGestureModel.State.End)
                {
                    OneHandEndGrabbing(1);
                }*/
                if (statePinchRH == LeapGestureModel.State.End)
                {
                    OneHandPinch(1);
                }
                /*if (statePinchingRH == LeapGestureModel.State.InProcess) 
                {
                    //OneHandPinching(1);
                }
                if (statePinchingRH == LeapGestureModel.State.End)
                {
                    //OneHandEndPinching(1);
                }
                if (stateTappingRH == LeapGestureModel.State.InProcess)
                {
                    //OneHandTapping(1);
                }
                if (stateTappingRH == LeapGestureModel.State.End)
                {
                    //OneHandEndTapping(1);
                }*/
                if (stateHoveringRH == LeapGestureModel.State.InProcess)
                {
                    OneHandHovering(1);
                }
            }
            if (leftHandExist && rightHandExist)
            {
                LeapGestureModel.State stateSplit = gestureSplit.Check(leftHand, rightHand, Time.deltaTime);
                if (stateSplit == LeapGestureModel.State.End)
                {
                    handle.moveObjectTo(UnityVectorExtension.ToVector3(leftHand.PalmPosition));
                    if (!oldVersion)
                    {
                        bin.moveObjectTo(UnityVectorExtension.ToVector3(rightHand.PalmPosition));
                    }
                }
            }
            if (!leftHandExist && !rightHandExist)
            {
                OneHandHovering(2);
            }
        }
    }
    
    private void InfoUpdate()
    {
        string info = "";
        GestureHovering gp = (GestureHovering)gestureHoveringRH;
        LayerMetaphor lm = layerMetaphorsActived[0];
        if (goSelected == null)
        {
            info += "goSelected: null";
        }
        else
        {
            info += "goSelected: " + goSelected.getLayer().getLayerName();
        }
        info += "lm: " + lm.getLayer().getLayerName();
        info += "disToCenter: " + lm.distanceToCenter(gp.LastPalmPos);
        //info += "RH Hovering State" + (gp.DetectingState == LeapGestureModel.State.InProcess);
        //info += "\nRH HoveringPos: " + gp.LastTipPos;
        //info += "\nRH HoveringPos: " + gp.LastPalmPos;
        //OneHandHovering(1);
        //handle.updateIndex(gp.LastPalmPos);
        //handle.updateObjectsPositions();
        /*
    if (goPinchedRH == null)
    {
        info += "goPinchedRH: null";
    }
    else
    {
        info += "goPinchedRH: " + goPinchedRH.getMetaphorObject().name;
    }
    if (goSelected == null)
    {
        info += "\ngoSelected: null";
    }
    else
    {
        info += "\ngoSelected: " + goSelected.getMetaphorObject().name;
    }
    try
    {
        GestureTapping gesture = (GestureTapping)gestureTappingRH;
        Vector3 tappingPos = gesture.LastTipPos;
        foreach (LayerMetaphor lm in layerMetaphorsActived)
        {
            GameObject worldObject = lm.getMetaphorObject();
            Collider coll = worldObject.GetComponent<Collider>();
            Vector3 closestPoint = coll.ClosestPointOnBounds(tappingPos);
            float distance = Vector3.Distance(closestPoint, tappingPos);
            //info += "\n" + lm.getMetaphorObject().name + " distance: " + distance.ToString();
            info += "\n" + lm.getMetaphorObject().name + " selected:: " + lm.IsSelected;
        }
    }
    catch
    {

    }*/
        infoPanel.GetComponent<Text>().text = info;
    }
    /*(
    private void OneHandGrab(int mode)
    {
        GestureGrab gesture;
        Vector3 grabbingPos;
        switch (mode)
        {
            case 0:
                gesture = (GestureGrab)gestureGrabLH;
                grabbingPos = gesture.LastPalmPos;
                Debug.Log("handle" + handle.getMetaphorObject() == null);
                if (handle.checkGrabbing(grabbingPos))
                {
                    goGrabbedLH = handle;
                    handle.startGrab();
                }
                return;
            case 1:
                gesture = (GestureGrab)gestureGrabRH;
                grabbingPos = gesture.LastPalmPos;
                if (handle.checkGrabbing(grabbingPos))
                {
                    goGrabbedRH = handle;
                    handle.startGrab();
                }
                return;
        }
    }
    */
    /*
    private void OneHandGrabbing(int mode)
    {
        GestureGrabbing gesture;
        Vector3 grabbingPos;
        Vector3 posMovement;
        switch (mode)
        {
            case 0:
                gesture = (GestureGrabbing)gestureGrabbingLH;
                grabbingPos = gesture.LastPalmPos;
                posMovement = gesture.PalmMovement;
                if (goGrabbedLH != null)
                {
                    goGrabbedLH.moveObjectBy(posMovement);
                }
                return;
            case 1:
                gesture = (GestureGrabbing)gestureGrabbingRH;
                grabbingPos = gesture.LastPalmPos;
                posMovement = gesture.PalmMovement;
                if (goGrabbedRH != null)
                {
                    goGrabbedRH.moveObjectBy(posMovement);
                }
                return;
        }
    }
    */
    private void OneHandGrasp()
    {

    }

    private void OneHandPinch(int mode)
    {
        GesturePinch gesture;
        Vector3 pinchingPos;
        List<LayerMetaphor> potentialLayers;
        switch (mode)
        {
            case 0:
                gesture = (GesturePinch)gesturePinchLH;
                pinchingPos = gesture.LastTipPos;
                if (goSelected != null)
                {
                    foreach (SelectionMetaphor sm in goSelected.getSelectionObjects())
                    {
                        if (sm.checkPinching(pinchingPos))
                        {
                            goSelected.createSelections(sm.getMetaphorObject().name);
                            return;
                        }
                    }
                }
                /*
                potentialLayers = new List<LayerMetaphor>();
                foreach (LayerMetaphor lm in layerMetaphorsActived)
                {
                    if (!lm.IsPinched && lm.checkPinching(pinchingPos) && !lm.IsCrowed)
                    {
                        potentialLayers.Add(lm);
                    }
                }
                foreach (LayerMetaphor lm in layerMetaphorsDisActived)
                {
                    if (!lm.IsPinched)
                    {
                        if (lm.checkPinching(pinchingPos))
                        {
                            potentialLayers.Add(lm);
                        }
                    }
                }
                if (potentialLayers.Count > 0)
                {
                    goPinchedLH = potentialLayers[0];
                    goPinchedLH.startPinch();
                    foreach (LayerMetaphor lm in potentialLayers)
                    {
                        if (lm.distanceToBoundary(pinchingPos) < goPinchedLH.distanceToBoundary(pinchingPos))
                        {
                            goPinchedLH.endPinch();
                            goPinchedLH = lm;
                            goPinchedLH.startPinch();
                        }
                    }
                }*/
                return;
            case 1:
                gesture = (GesturePinch)gesturePinchRH;
                pinchingPos = gesture.LastTipPos;
                if (goSelected != null)
                {
                    foreach (SelectionMetaphor sm in goSelected.getSelectionObjects())
                    {
                        if (sm.checkPinching(pinchingPos))
                        {
                            goSelected.createSelections(sm.getMetaphorObject().name);
                            return;
                        }
                    }
                }
                /*
                potentialLayers = new List<LayerMetaphor>();
                foreach (LayerMetaphor lm in layerMetaphorsActived)
                {
                    if (!lm.IsPinched && lm.checkPinching(pinchingPos) && !lm.IsCrowed)
                    {
                        potentialLayers.Add(lm);
                    }
                }
                foreach (LayerMetaphor lm in layerMetaphorsDisActived)
                {
                    if (!lm.IsPinched)
                    {
                        if (lm.checkPinching(pinchingPos))
                        {
                            potentialLayers.Add(lm);
                        }
                    }
                }
                if (potentialLayers.Count > 0)
                {
                    goPinchedRH = potentialLayers[0];
                    goPinchedRH.startPinch();
                    foreach (LayerMetaphor lm in potentialLayers)
                    {
                        if (lm.distanceToBoundary(pinchingPos) < goPinchedRH.distanceToBoundary(pinchingPos))
                        {
                            goPinchedRH.endPinch();
                            goPinchedRH = lm;
                            goPinchedRH.startPinch();
                        }
                    }
                }
                */
                return;
        }
    }

    private void OneHandGrasping()
    {

    }
    /*
    private void OneHandPinching(int mode)
    {
        GesturePinching gesture;
        Vector3 pinchingPos;
        Vector3 posMovement;
        switch (mode)
        {
            case 0:
                gesture = (GesturePinching)gesturePinchingLH;
                pinchingPos = gesture.LastTipPos;
                posMovement = gesture.TipMovement;
                if (goPinchedLH != null && gesture.CountTime >= pinchingTimeMin)
                {
                    if (goPinchedLH.IsSelected)
                    {
                        goPinchedLH.reColorWorldObj(Color.white);
                        goPinchedLH.cancelHighLightDup();
                        goPinchedLH.removeSelections();
                        goPinchedLH.endSelectState();
                        goSelected = null;
                    }
                    //goPinchedLH.moveObjectBy(posMovement);
                    goPinchedLH.moveObjectTo(pinchingPos);
                    LayerMetaphor lm = (LayerMetaphor)goPinchedLH;
                    bool inRange = handle.updateLayerExisted(lm);
                    if (layerMetaphorsActived.Contains(lm))
                    {
                        if (!inRange)
                        {
                            layerMetaphorsActived.Remove(lm);
                            lm.getLayer().hide();
                            layerMetaphorsDisActived.Add(lm);
                        }
                        else
                        {
                            handle.updateIndex(pinchingPos);
                        }
                    }
                    else
                    {
                        if (inRange)
                        {
                            layerMetaphorsActived.Add(lm);
                            lm.getLayer().show();
                            layerMetaphorsDisActived.Remove(lm);
                        }
                    }
                    handle.updateObjectsOrder();
                    handle.updateObjectsPositions(gesture.LastPalmPos);
                }
                return;
            case 1:
                gesture = (GesturePinching)gesturePinchingRH;
                pinchingPos = gesture.LastTipPos;
                posMovement = gesture.TipMovement;
                Debug.Log(posMovement);
                if (goPinchedRH != null && gesture.CountTime >= pinchingTimeMin)
                {
                    if (goPinchedRH.IsSelected)
                    {
                        goPinchedRH.reColorWorldObj(Color.white);
                        goPinchedRH.cancelHighLightDup();
                        goPinchedRH.removeSelections();
                        goPinchedRH.endSelectState();
                        goSelected = null;
                    }
                    //goPinchedRH.moveObjectBy(posMovement);
                    goPinchedRH.moveObjectTo(pinchingPos);
                    LayerMetaphor lm = (LayerMetaphor)goPinchedRH;
                    bool inRange = handle.updateLayerExisted(lm);
                    if (layerMetaphorsActived.Contains(lm))
                    {
                        if (!inRange)
                        {
                            layerMetaphorsActived.Remove(lm);
                            lm.getLayer().hide();
                            layerMetaphorsDisActived.Add(lm);
                        }
                        else
                        {
                            handle.updateIndex(pinchingPos);
                        }
                    }
                    else
                    {
                        if (inRange)
                        {
                            layerMetaphorsActived.Add(lm);
                            lm.getLayer().show();
                            layerMetaphorsDisActived.Remove(lm);
                        }
                    }
                    handle.updateObjectsOrder();
                    handle.updateObjectsPositions(gesture.LastPalmPos);
                }
                return;
        }
    }*/
    /*
    private void OneHandTapping(int mode)
    {
        GestureTapping gesture;
        Vector3 tappingPos;
        Vector3 posMovement;
        List<LayerMetaphor> potentialLayers;
        List<SelectionMetaphor> potentialSelections;
        switch (controlMode)
        {
            case ControlMode.tapMenu:
                switch (mode)
                {
                    case 0:
                        gesture = (GestureTapping)gestureTappingLH;
                        tappingPos = gesture.LastTipPos;
                        posMovement = gesture.TipMovement;
                        if (goTappedLH == null)
                        {
                            potentialLayers = new List<LayerMetaphor>();
                            foreach (LayerMetaphor lm in layerMetaphorsActived)
                            {
                                if (!lm.IsTapped)
                                {
                                    if (lm.checkTapping(tappingPos))
                                    {
                                        potentialLayers.Add(lm);
                                    }
                                }
                            }
                            if (potentialLayers.Count > 0)
                            {
                                goTappedLH = potentialLayers[0];
                                foreach (LayerMetaphor lm in potentialLayers)
                                {
                                    if (lm.checkDistance(tappingPos) < goTappedLH.checkDistance(tappingPos))
                                    {
                                        goTappedLH = lm;
                                    }
                                }
                                goTappedLH.startTap();
                                goTappedLH.reScaleWorldObj(1.25f, 1.25f, 1.25f);
                            }
                        }
                        else
                        {
                            if (goTappedLH.checkTapping(tappingPos))
                            {
                                //ratate the go
                            }
                            else
                            {
                                goTappedLH.endTap();
                                goTappedLH.reScaleWorldObj(0.8f, 0.8f, 0.8f);
                                goTappedLH = null;
                            }
                        }
                        return;
                    case 1:
                        gesture = (GestureTapping)gestureTappingRH;
                        tappingPos = gesture.LastTipPos;
                        posMovement = gesture.TipMovement;
                        if (goTappedRH != null)
                        {
                            foreach (SelectionMetaphor sm in goTappedRH.getSelectionObjects())
                            {
                                if (sm.checkTapping(tappingPos))
                                {
                                    goTappedRH.createSelections(sm.getMetaphorObject().name);
                                    return;
                                }
                            }
                        }
                        if (goTappingRH == null)
                        {
                            potentialLayers = new List<LayerMetaphor>();
                            foreach (LayerMetaphor lm in layerMetaphorsActived)
                            {
                                if (lm.checkTapping(tappingPos))
                                {
                                    potentialLayers.Add(lm);
                                }
                            }
                            if (potentialLayers.Count > 0)
                            {
                                goTappingRH = potentialLayers[0];
                                foreach (LayerMetaphor lm in potentialLayers)
                                {
                                    if (lm.checkDistance(tappingPos) < goTappingRH.checkDistance(tappingPos))
                                    {
                                        goTappingRH = lm;
                                    }
                                }
                                goTappingRH.changeTap();
                                if (goTappingRH.IsTapped)
                                {
                                    if(goTappedRH != null)
                                    {
                                        goTappedRH.endTap();
                                        goTappedRH.reColorWorldObj(Color.white);
                                        goTappedRH.removeSelections();
                                    }
                                    goTappedRH = goTappingRH;
                                    goTappedRH.reColorWorldObj(Color.red);
                                    goTappedRH.createSelections();
                                }
                                else
                                {
                                    goTappedRH.reColorWorldObj(Color.white);
                                    goTappedRH.removeSelections();
                                    goTappedRH = null;
                                }
                            }
                        }
                        else
                        {
                            if (goTappingRH.checkTapping(tappingPos))
                            {
                                //do nothing
                            }
                            else
                            {
                                //goTappedRH.endTap();
                                //goTappedRH.reScaleWorldObj(1.25f, 1.25f, 1.25f);
                                goTappingRH = null;
                            }
                        }
                        return;
                }
                return;
            case ControlMode.tappingRotation:
                switch (mode)
                {
                    case 0:
                        gesture = (GestureTapping)gestureTappingLH;
                        tappingPos = gesture.LastTipPos;
                        posMovement = gesture.TipMovement;
                        if (goTappedLH == null)
                        {
                            potentialLayers = new List<LayerMetaphor>();
                            foreach (LayerMetaphor lm in layerMetaphorsActived)
                            {
                                if (!lm.IsTapped)
                                {
                                    if (lm.checkTapping(tappingPos))
                                    {
                                        potentialLayers.Add(lm);
                                    }
                                }
                            }
                            if (potentialLayers.Count > 0)
                            {
                                goTappedLH = potentialLayers[0];
                                foreach (LayerMetaphor lm in potentialLayers)
                                {
                                    if (lm.checkDistance(tappingPos) < goTappedLH.checkDistance(tappingPos))
                                    {
                                        goTappedLH = lm;
                                    }
                                }
                                goTappedLH.startTap();
                                goTappedLH.reScaleWorldObj(1.25f, 1.25f, 1.25f);
                            }
                        }
                        else
                        {
                            if (goTappedLH.checkTapping(tappingPos))
                            {
                                //ratate the go
                            }
                            else
                            {
                                goTappedLH.endTap();
                                goTappedLH.reScaleWorldObj(0.8f, 0.8f, 0.8f);
                                goTappedLH = null;
                            }
                        }
                        return;
                    case 1:
                        gesture = (GestureTapping)gestureTappingRH;
                        tappingPos = gesture.LastTipPos;
                        posMovement = gesture.TipMovement;
                        if (goTappedRH == null)
                        {
                            potentialLayers = new List<LayerMetaphor>();
                            foreach (LayerMetaphor lm in layerMetaphorsActived)
                            {
                                if (!lm.IsTapped)
                                {
                                    if (lm.checkTapping(tappingPos))
                                    {
                                        potentialLayers.Add(lm);
                                    }
                                }
                            }
                            if (potentialLayers.Count > 0)
                            {
                                goTappedRH = potentialLayers[0];
                                foreach (LayerMetaphor lm in potentialLayers)
                                {
                                    if (lm.checkDistance(tappingPos) < goTappedRH.checkDistance(tappingPos))
                                    {
                                        goTappedRH = lm;
                                    }
                                }
                                goTappedRH.startTap();
                                goTappedRH.reScaleWorldObj(1.25f, 1.25f, 1.25f);
                            }
                        }
                        else
                        {
                            if (goTappedRH.checkTapping(tappingPos))
                            {
                                //ratate the go
                            }
                            else
                            {
                                goTappedRH.endTap();
                                goTappedRH.reScaleWorldObj(0.8f, 0.8f, 0.8f);
                                goTappedRH = null;
                            }
                        }
                        return;
                }
                return;

        }
    }
    */


    private void OneHandHovering(int mode)
    {
        if (goSelected != null)
        {
            return;
        }
        GestureHovering gesture;
        Vector3 hoveringPos;
        switch (mode)
        {
            case 0:
                gesture = (GestureHovering)gestureHoveringLH;
                hoveringPos = (gesture.LastTipPos + gesture.LastPalmPos) / 2;
                handle.updateIndex(hoveringPos);
                handle.updateObjectsPositions(hoveringPos);
                if (!oldVersion)
                {
                    bin.updateIndex(hoveringPos);
                    bin.updateObjectsPositions(hoveringPos);
                }
                return;
            case 1:
                gesture = (GestureHovering)gestureHoveringRH;
                hoveringPos = (gesture.LastTipPos + gesture.LastPalmPos) / 2;
                handle.updateIndex(hoveringPos);
                handle.updateObjectsPositions(hoveringPos);
                if (!oldVersion)
                {
                    bin.updateIndex(hoveringPos);
                    bin.updateObjectsPositions(hoveringPos);
                }
                return;
            case 2:
                foreach (LayerMetaphor lm in layerMetaphorsActived)
                {
                    if (lm.isGrasped)
                    {
                        lm.cancelAllGraspState();
                        lm.cancelAllHighLight();
                    }
                }
                hoveringPos = handle.safeUpdatePos;
                handle.updateIndex(hoveringPos);
                handle.updateObjectsPositions(hoveringPos);
                if (oldVersion)
                {
                    return;
                }
                foreach (LayerMetaphor lm in layerMetaphorsDisActived)
                {
                    if (lm.isGrasped)
                    {
                        goSelected = null;
                        lm.cancelAllGraspState();
                        lm.cancelAllHighLight();
                        lm.moveObjectTo(bin.safeUpdatePos);
                    }
                }
                hoveringPos = bin.safeUpdatePos;
                bin.updateIndex(hoveringPos);
                bin.updateObjectsPositions(hoveringPos);
                return;
        }
    }

    /*
    private void OneHandEndGrabbing(int mode)
    {
        switch (mode)
        {
            case 0:
                if (goGrabbedLH != null)
                {
                    goGrabbedLH.endGrab();
                }
                goGrabbedLH = null;
                return;
            case 1:
                if (goGrabbedRH != null)
                {
                    goGrabbedRH.endGrab();
                }
                goGrabbedRH = null;
                return;
        }
    }*/
    /*
    private void OneHandEndPinching(int mode)
    {
        GesturePinching gesture;
        switch (mode)
        {
            case 0:
                gesture = (GesturePinching)gesturePinchingLH;
                if (goPinchedLH != null)
                {
                    goPinchedLH.endPinch();
                    if (gesture.CountTime < pinchingTimeMin)
                    {
                        goPinchedLH.changeSelectState();
                        if (goPinchedLH.IsSelected)
                        {
                            goPinchedLH.reColorWorldObj(Color.red);
                            goPinchedLH.highLightDup();
                            goPinchedLH.createSelections();
                            if (goSelected == null)
                            {
                                goSelected = goPinchedLH;
                            }
                            else if (goSelected != goPinchedLH)
                            {
                                goSelected.reColorWorldObj(Color.white);
                                goSelected.cancelHighLightDup();
                                goSelected.removeSelections();
                                goSelected.endSelectState();
                                goSelected = goPinchedLH;
                            }
                        }
                        else
                        {
                            goPinchedLH.reColorWorldObj(Color.white);
                            goPinchedLH.cancelHighLightDup();
                            goPinchedLH.removeSelections();
                            goSelected = null;
                        }
                    }
                    goPinchedLH = null;
                    handle.updateObjectsPositions(gesture.LastPalmPos);
                }
                return;
            case 1:
                gesture = (GesturePinching)gesturePinchingRH;
                if (goPinchedRH != null)
                {
                    goPinchedRH.endPinch();
                    if (gesture.CountTime < pinchingTimeMin)
                    {
                        goPinchedRH.changeSelectState();
                        if (goPinchedRH.IsSelected)
                        {
                            goPinchedRH.reColorWorldObj(Color.red);
                            goPinchedRH.highLightDup();
                            goPinchedRH.createSelections();
                            if (goSelected == null)
                            {
                                goSelected = goPinchedRH;
                            }
                            else if (goSelected != goPinchedRH)
                            {
                                goSelected.reColorWorldObj(Color.white);
                                goSelected.cancelHighLightDup();
                                goSelected.removeSelections();
                                goSelected.endSelectState();
                                goSelected = goPinchedRH;
                            }
                        }
                        else
                        {
                            goPinchedRH.reColorWorldObj(Color.white);
                            goPinchedRH.cancelHighLightDup();
                            goPinchedRH.removeSelections();
                            goSelected = null;
                        }
                    }
                    goPinchedRH = null;
                    handle.updateObjectsPositions(gesture.LastPalmPos);
                }
                return;
        }
    }
    */
    /*
    private void OneHandEndTapping(int mode)
    {
        switch (mode)
        {
            case 0:
                if (goTappedLH != null)
                {
                    goTappedLH.endTap();
                    goTappedLH.reScaleWorldObj(0.8f, 0.8f, 0.8f);
                }
                goTappedLH = null;
                return;
            case 1:
                switch (controlMode)
                { 
                    case ControlMode.tapMenu:
                        return;
                    case ControlMode.tappingRotation:
                        if (goTappedRH != null)
                        {
                            goTappedRH.endTap();
                            goTappedRH.reScaleWorldObj(0.8f, 0.8f, 0.8f);
                        }
                        goTappedRH = null;
                        return;
                }
                return;
        }
    }*/
    private void DeletingLayersForTest()
    {
        if (this.Lock) { return; }
        int count = 0;
        while (layerMetaphorsActived.Count > 14)
        {
            count++;
            layerMetaphorsActived[0].moveObjectBy(new Vector3(.5f, 0.06f*count, 0));
            layerMetaphorsActived[0].enableActivedGraspActions();
            layerMetaphorsActived[0].enableGraspMovement();
            LayerMetaphor lm = layerMetaphorsActived[0];
            bool inRange = handle.updateLayerExisted(lm);
            if (inRange)
            {
                Debug.Log("Not successfully removed.");
                return;
            }
            else
            {
                layerMetaphorsActived.Remove(lm);
                lm.getLayer().hide();
                layerMetaphorsDisActived.Add(lm);
                if (!oldVersion)
                {
                    lm.getLayer().getDuplication().hideSelf();
                    lm.getMetaphorObject().GetComponent<MeshRenderer>().enabled = true;
                    bin.addLayer(lm);
                }
            }
            handle.updateIndex(handle.safeUpdatePos);
            handle.updateObjectsPositions(handle.safeUpdatePos);
            if (!oldVersion)
            {
                bin.updateIndex(bin.safeUpdatePos);
                bin.updateObjectsPositions(bin.safeUpdatePos);
            }
            deleteBefore = false;
            /*
            else
            {
                if (inRange)
                {
                    layerMetaphorsActived.Add(lm);
                    lm.getLayer().show();
                    layerMetaphorsDisActived.Remove(lm);
                }
            }*/
        }
    }
}
