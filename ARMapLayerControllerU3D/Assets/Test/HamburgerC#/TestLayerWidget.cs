using Leap;
using Mapbox.Map;
using Mapbox.Unity.MeshGeneration;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mapbox.Unity.MeshGeneration.Factories;
using System.Linq;
using System.Threading;
using System;

public class TestLayerWidget : MonoBehaviour {

    public bool deleteBefore = false;

    protected GameObject mapObj = null;
    protected AbstractMap mapToEdit;
    protected List<MapARLayer> mapARLayersActived = new List<MapARLayer>();
    protected List<MapARLayer> mapARLayersDisActived = new List<MapARLayer>();
    protected List<MapARLayerDup> mapARLayersActivedDup = new List<MapARLayerDup>();
    protected int currentSelectedIndex = 0;
    protected bool Lock = true;//lock other operation when the map is building or animating
    protected bool dupActived = false;
    protected MenuController menuController= new MenuController();
    protected enum FacingTarget{
        LeftMenu,
        Map,
        RightMenu,
        RightMenuSub,
        None
    }

    [SerializeField]
    FacingTarget facingTarget;

    [SerializeField]
    GameObject layerLabel;

    [SerializeField]
    Camera camera;

    [SerializeField]
    float dupGap = 0.3f, dupMovementTime = 0.5f, dupHLGap = 3f, menuGap = 0.3f, leftMenuDegree = -30, rightMenuDegree = 30, rightMenuSubDegree = 60;

    //Leap stuffs
    protected Controller controller;
    //gesture models
    LeapGestureModel gestureSplit, gestureClap,
        gestureLeftSwapLH, gestureRightSwapLH, /*gestureUpSwapLH, gestureDownSwapLH*/gestureUpLongSwapLH, gestureDownLongSwapLH, gestureGrabLH, gestureGrabbingLH,
        gestureLeftSwapRH, gestureRightSwapRH, /*gestureUpSwapRH, gestureDownSwapRH*/gestureUpLongSwapRH, gestureDownLongSwapRH, gestureGrabRH, gestureGrabbingRH;

    // Use this for initialization
    void Start () {
        MapLayerInit();
        LeapGestureInit();
        //TestInit();
    }
	
	// Update is called once per frame
	void Update ()
    {
        TargetUpdate();
        LayerUpdate();
        OnClickListener();
        GestureListener();
    }

    protected void FixedUpdate()
    {

    }

    protected void MapLayerInit()
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
            List<VectorSubLayerProperties> mapLayerProperties = (List < VectorSubLayerProperties >) mapToEdit.VectorData.GetAllFeatureSubLayers();
            foreach (VectorSubLayerProperties vsp in mapLayerProperties)
            {

                MapARLayer newLayer = new MapARLayer(vsp);
                if (vsp.coreOptions.isActive)
                {
                    mapARLayersActived.Add(newLayer);
                }
            }
            if (mapARLayersActived.Count != 0)
            {
                layerLabel.GetComponent<TextMesh>().text = ("Current Layer: " + mapARLayersActived[currentSelectedIndex].getLayerName());
            }
            else
            {
                layerLabel.GetComponent<TextMesh>().text = ("No Actived Layer found.");
            }
        }
    }

    protected void LeapGestureInit()
    {
        //set up all gesture models
        gestureSplit = new GestureSplit();
        gestureClap = new GestureClap();
        gestureLeftSwapLH = new GestureLeftSwap();
        gestureRightSwapLH = new GestureRightSwap();
        //gestureUpSwapLH = new GestureUpSwap();
        //gestureDownSwapLH = new GestureDownSwap();
        gestureUpLongSwapLH = new GestureUpLongSwap();
        gestureDownLongSwapLH = new GestureDownLongSwap();
        gestureGrabLH = new GestureGrab();
        gestureGrabbingLH = new GestureGrabbing();

        gestureLeftSwapRH = new GestureLeftSwap();
        gestureRightSwapRH = new GestureRightSwap();
        //gestureUpSwapRH = new GestureUpSwap();
        //gestureDownSwapRH = new GestureDownSwap();
        gestureUpLongSwapRH = new GestureUpLongSwap();
        gestureDownLongSwapRH = new GestureDownLongSwap();
        gestureGrabRH = new GestureGrab();
        gestureGrabbingRH = new GestureGrabbing();
    }

    //update which trget the camera is looking at
    public void TargetUpdate()
    {
        Vector3 dirCameraRay = camera.transform.forward.normalized;
        Vector3 dirToMap = mapObj.transform.position - camera.transform.position.normalized;
        Vector3 baseSurfaceCross = Vector3.Cross(dirToMap, Vector3.right).normalized;
        Vector3 dirCameraRayOnSurface = Vector3.Cross(Vector3.Cross(baseSurfaceCross,dirCameraRay),baseSurfaceCross).normalized;
        float angle = Vector3.Angle(dirToMap,dirCameraRayOnSurface);
        if (Vector3.Cross(dirToMap,dirCameraRayOnSurface).y < 0) angle = -angle;
        float leftMenuDegree = menuController.getLeftMenuDegree();
        float rightMenuDegree = menuController.getRightMenuDegree();
        float rightMenuSubDegree = menuController.getRightMenuSubDegree();
        FacingTarget oldFacingTarget = facingTarget;
        if (leftMenuDegree*3/2 <= angle & angle < leftMenuDegree/2)
        {
            facingTarget = FacingTarget.LeftMenu;
        } else if (leftMenuDegree/2 <= angle & angle < rightMenuDegree/2)
        {
            facingTarget = FacingTarget.Map;
         } else if (rightMenuDegree/2 <= angle & angle < (rightMenuDegree+rightMenuSubDegree)/2) 
        {
            facingTarget = FacingTarget.RightMenu;
        }else if ((rightMenuDegree + rightMenuSubDegree) / 2 <= angle & angle < (rightMenuDegree/2 + rightMenuSubDegree*3/2))
        {
            facingTarget = FacingTarget.RightMenuSub;
        }
        else
        {
            facingTarget = FacingTarget.None;
        }
        if (oldFacingTarget != facingTarget)
        {
            switch (facingTarget)
            {
                case FacingTarget.None:
                    menuController.WatchOn(-1);
                    return;
                case FacingTarget.Map:
                    menuController.WatchOn(-1);
                    return;
                case FacingTarget.LeftMenu:
                    menuController.WatchOn(0);
                    return;
                case FacingTarget.RightMenu:
                    menuController.WatchOn(1);
                    return;
                case FacingTarget.RightMenuSub:
                    menuController.WatchOn(2);
                    return;
            }
        }
    }

    //update layer gameobject content when map initialized , moved.
    protected void LayerUpdate()
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
                    foreach (MapARLayer layer in this.mapARLayersActived)
                    {
                        layer.updateLayer(tileIds);
                    }
                    this.Lock = false;
                }
            }
            else if (s == ModuleState.Working)
            {
                this.Lock = true;
            }
        };
    }

    protected void GestureListener()
    { //not editable when building
        if (this.Lock)
        {
            return;
        }
        else
        {
            controller = new Controller();
            Frame frame = controller.Frame();
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
            if (dupActived)
            {
                //Two-Hands Gestures
                if (leftHandExist & rightHandExist)
                {
                    if (gestureClap.Check(leftHand, rightHand, Time.deltaTime) == LeapGestureModel.State.End)
                    {
                        TwoHandsClap();
                    }
                }
                //One-Hand Gestures for Left Hand
                else if (leftHandExist & !rightHandExist)
                {
                    LeapGestureModel.State stateRightSwapLH = gestureRightSwapLH.Check(leftHand, null, Time.deltaTime);
                    LeapGestureModel.State stateLeftSwapLH = gestureLeftSwapLH.Check(leftHand, null, Time.deltaTime);
                    LeapGestureModel.State stateUpLongSwapLH = gestureUpLongSwapLH.Check(leftHand, null, Time.deltaTime);
                    LeapGestureModel.State stateDownLongSwapLH = gestureDownLongSwapLH.Check(leftHand, null, Time.deltaTime);
                    LeapGestureModel.State stateGrabLH = gestureGrabLH.Check(leftHand, null, Time.deltaTime);
                    LeapGestureModel.State stateGrabbingLH = gestureGrabbingLH.Check(leftHand, null, Time.deltaTime);

                    if (stateRightSwapLH == LeapGestureModel.State.End)
                    {
                        OneHandRightSwap();
                    }
                    if (stateLeftSwapLH == LeapGestureModel.State.End)
                    {
                        OneHandLeftSwap();
                    }/*
                    if ((gestureUpSwapLH.Check(leftHand, null, Time.deltaTime) == LeapGestureModel.State.End))
                    {
                        OneHandUpSwap();
                    }
                    if ((gestureDownSwapLH.Check(leftHand, null, Time.deltaTime) == LeapGestureModel.State.End))
                    {
                        OneHandDownSwap();
                    }*/
                    if (stateUpLongSwapLH == LeapGestureModel.State.InProcess)
                    {
                        if (stateGrabbingLH == LeapGestureModel.State.InProcess)
                        {
                            OneHandUpGrabbing((GestureUpLongSwap)gestureUpLongSwapLH);
                        }
                        else
                        {
                            OneHandUpSwap((GestureUpLongSwap)gestureUpLongSwapLH);
                        }
                    }
                    if (stateDownLongSwapLH == LeapGestureModel.State.InProcess)
                    {
                        if (stateGrabbingLH == LeapGestureModel.State.InProcess)
                        {
                            OneHandDownGrabbing((GestureDownLongSwap)gestureDownLongSwapLH);
                        }
                        else
                        {
                            OneHandDownSwap((GestureDownLongSwap)gestureDownLongSwapLH);
                        }
                    }
                    if (stateGrabLH == LeapGestureModel.State.End)
                    {
                        OneHandGrab();
                    }
                }
                //One0Hand Gestures for Right Hand
                else if (!leftHandExist & rightHandExist)
                {
                    LeapGestureModel.State stateRightSwapRH = gestureRightSwapRH.Check(null, rightHand, Time.deltaTime);
                    LeapGestureModel.State stateLeftSwapRH = gestureLeftSwapRH.Check(null, rightHand, Time.deltaTime);
                    LeapGestureModel.State stateUpLongSwapRH = gestureUpLongSwapRH.Check(null, rightHand, Time.deltaTime);
                    LeapGestureModel.State stateDownLongSwapRH = gestureDownLongSwapRH.Check(null, rightHand, Time.deltaTime);
                    LeapGestureModel.State stateGrabRH = gestureGrabRH.Check(null, rightHand, Time.deltaTime);
                    LeapGestureModel.State stateGrabbingRH = gestureGrabbingRH.Check(null, rightHand, Time.deltaTime);

                    if (stateRightSwapRH == LeapGestureModel.State.End)
                    {
                        OneHandRightSwap();
                    }
                    if (stateLeftSwapRH == LeapGestureModel.State.End)
                    {
                        OneHandLeftSwap();
                    }/*
                    if ((gestureUpSwapRH.Check(null, rightHand, Time.deltaTime) == LeapGestureModel.State.End))
                    {
                        OneHandUpSwap();
                    }
                    if ((gestureDownSwapRH.Check(null, rightHand, Time.deltaTime) == LeapGestureModel.State.End))
                    {
                        OneHandDownSwap();
                    }*/
                    if (stateUpLongSwapRH  == LeapGestureModel.State.InProcess)
                    {
                        if (stateGrabbingRH == LeapGestureModel.State.InProcess)
                        {
                            OneHandUpGrabbing((GestureUpLongSwap)gestureUpLongSwapRH);
                        }
                        else
                        {
                            OneHandUpSwap((GestureUpLongSwap)gestureUpLongSwapRH);
                        }
                    }
                    if (stateDownLongSwapRH == LeapGestureModel.State.InProcess)
                    {
                        if (stateGrabbingRH == LeapGestureModel.State.InProcess)
                        {
                            OneHandDownGrabbing((GestureDownLongSwap)gestureDownLongSwapRH);
                        }
                        else
                        {
                            OneHandDownSwap((GestureDownLongSwap)gestureDownLongSwapRH);
                        }
                    }
                    if (stateGrabRH == LeapGestureModel.State.End)
                    {
                        OneHandGrab();
                    }
                }
            }
            else
            {
                if (leftHandExist & rightHandExist)
                {
                    //check gesture models states
                    if (gestureSplit.Check(leftHand, rightHand, Time.deltaTime) == LeapGestureModel.State.End)
                    {
                        TwoHandsSplit();
                    }
                }
            }
        }
    }

    protected void OnClickListener()
    {
        //not editable when building
        if (this.Lock)
        {
            return;
        }
        else
        {
            if (dupActived)
            {
                if (Input.GetKeyDown(KeyCode.I))
                {
                    print("key I is held down");
                    Invisual();
                }
                if (Input.GetKeyDown(KeyCode.V))
                {
                    print("key V is held down");
                    Visual();
                }
                if (Input.GetKeyDown(KeyCode.T))
                {
                    print("key T is held down");
                    setTransparency(0.5f);
                }
                if (Input.GetKeyDown(KeyCode.N))
                {
                    print("key N is press down");
                    MapLayerSelectNext();
                }
                if (Input.GetKeyDown(KeyCode.M))
                {
                    print("key M is press down");
                    TwoHandsClap();
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    print("key right Arrow is press down");
                    OneHandRightSwap();
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    print("key left Arrow is press down");
                    OneHandLeftSwap();
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    print("key up Arrow is press down");
                    //OneHandUpSwap();
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    print("key down Arrow is press down");
                    //OneHandDownSwap();
                }
                if (Input.GetKeyDown(KeyCode.G))
                {
                    print("key G is press down");
                    OneHandGrab();
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    print("key A is held down");
                    try{
                        AddLayer(mapARLayersDisActived[0]);
                    }
                    catch
                    {
                        Debug.Log("No Layer to be added");
                    }
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    print("key D is held down");
                    try
                    {
                        DeleteLayer(mapARLayersActived[currentSelectedIndex]);
                    }
                    catch
                    {
                        Debug.Log("No Layer to be Delete");
                    }
                }
                if (Input.GetKeyUp(KeyCode.S))
                {
                    print("key s is held down");
                    try
                    {
                        DeletingLayersForTest();
                    }
                    catch
                    {
                        Debug.Log("No Layer to be Delete");
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    print("key M is press down");
                    TwoHandsSplit();
                }
            }
        }
    }

    //==============================Gestures Methods================================
    protected void TwoHandsClap()
    {
        DuplicateClose();
        menuController.RightMenuClose();
        menuController.RightMenuSubClose();
        menuController.LeftMenuClose();
    }

    protected void TwoHandsSplit()
    {
        Duplicate();
        LeftMenuSetUp();
        menuController.WatchOn(-1);
        if (deleteBefore)
        {
            DeletingLayersForTest();
        }
    }

    protected void OneHandUpSwap(GestureUpLongSwap gesture)
    {
        if (!gesture.SwapLevelAdded)
        {
            return;
        }
        switch (facingTarget)
        {
            case FacingTarget.LeftMenu:
                LeftMenuSelectNext();
                return;
            case FacingTarget.Map:
                MapLayerSelectNext();
                return;
            case FacingTarget.RightMenu:
                RightMenuSelectNext();
                return;
            case FacingTarget.RightMenuSub:
                RightMenuSubSelectNext();
                return;
        }
    }

    protected void OneHandDownSwap(GestureDownLongSwap gesture)
    {
        if (!gesture.SwapLevelAdded)
        {
            return;
        }
        switch (facingTarget)
        {
            case FacingTarget.LeftMenu:
                LeftMenuSelectLast();
                return;
            case FacingTarget.Map:
                MapLayerSelectLast();
                return;
            case FacingTarget.RightMenu:
                RightMenuSelectLast();
                return;
            case FacingTarget.RightMenuSub:
                RightMenuSubSelectLast();
                return;
        }
    }

    protected void OneHandUpGrabbing(GestureUpLongSwap gesture)
    {
        if (!gesture.SwapLevelAdded)
        {
            return;
        }
        switch (facingTarget)
        {
            case FacingTarget.LeftMenu:
                LeftMenuSelectNext();
                return;
            case FacingTarget.Map:
                MapLayerSwitchNext();
                return;
            case FacingTarget.RightMenu:
                RightMenuSelectNext();
                return;
            case FacingTarget.RightMenuSub:
                RightMenuSubSelectNext();
                return;
        }
    }

    protected void OneHandDownGrabbing(GestureDownLongSwap gesture)
    {
        if (!gesture.SwapLevelAdded)
        {
            return;
        }
        switch (facingTarget)
        {
            case FacingTarget.LeftMenu:
                LeftMenuSelectLast();
                return;
            case FacingTarget.Map:
                MapLayerSwitchLast();
                return;
            case FacingTarget.RightMenu:
                RightMenuSelectLast();
                return;
            case FacingTarget.RightMenuSub:
                RightMenuSubSelectLast();
                return;
        }
    }

    protected void OneHandRightSwap()
    {
        switch (facingTarget)
        {
            case FacingTarget.LeftMenu:
                //LeftMenuClose();
                LeftMenuSelect();
                return;
            case FacingTarget.Map:
                MapLayerSelect();
                return;
            case FacingTarget.RightMenu:
                RightMenuSelect();
                return;
        }
    }

    protected void OneHandLeftSwap()
    {
        switch (facingTarget)
        {
            case FacingTarget.Map:
                if (menuController.isLefttMenuOn())
                {
                    DeleteLayer(mapARLayersActived[currentSelectedIndex]);
                    LeftMenuRefresh();
                    menuController.WatchOn(-1);
                }
                else
                {
                    LeftMenuSetUp();
                }
                return;
            case FacingTarget.RightMenu:
                RightMenuClose();
                return;
            case FacingTarget.RightMenuSub:
                RightMenuSubClose();
                return;
        }
    }

    protected void OneHandGrab()
    {
        switch (facingTarget)
        {
            case FacingTarget.LeftMenu:
                LeftMenuSelect();
                return;
            case FacingTarget.Map:
                MapLayerSelect();
                return;
            case FacingTarget.RightMenu:
                RightMenuSelect();
                return;
            case FacingTarget.RightMenuSub:
                RightMenuSubSelect();
                return;
        }
    }

    //=============================Detailed commands methods=================================
    protected void RightMenuSetUp()
    {
        List<string> rightMenuItems = mapARLayersActived[currentSelectedIndex].getMapAttributes();
        menuController.RightMenuCreate("Propertity",rightMenuItems, camera.transform, rightMenuDegree, menuGap);
    }

    protected void RightMenuSubSetUp(string CMD)
    {
        List<string> rightMenuSubItems = new List<string>();
        switch (CMD)
        {
            case "Color":
                rightMenuSubItems.Add("Red");
                rightMenuSubItems.Add("Blue");
                rightMenuSubItems.Add("Yellow");
                menuController.RightMenuSubCreate("Color",rightMenuSubItems, camera.transform, rightMenuSubDegree, menuGap);
                return;
            case "Transparency":
                rightMenuSubItems.Add("25%");
                rightMenuSubItems.Add("50%");
                rightMenuSubItems.Add("75%");
                rightMenuSubItems.Add("100%");
                menuController.RightMenuSubCreate("Transparency", rightMenuSubItems, camera.transform, rightMenuSubDegree, menuGap);
                return;
        }
    }

    protected void LeftMenuSetUp()
    {
        List<string> leftMenuItems = new List<string>();
        foreach(MapARLayer layer in mapARLayersDisActived){
            leftMenuItems.Add(layer.getLayerName());
        }
        menuController.LeftMenuCreate("Other Layers", leftMenuItems, camera.transform, leftMenuDegree, menuGap);
    }

    protected void RightMenuRefresh()
    {
        List<string> rightMenuItems = mapARLayersActived[currentSelectedIndex].getMapAttributes();
        menuController.RightMenuRefresh(rightMenuItems);
        RightMenuSubRefresh(menuController.getRightMenuCMD());
    }
    
    protected void RightMenuSubRefresh(string CMD)
    {
        List<string> rightMenuSubItems = new List<string>();
        switch (CMD)
        {
            case "Color":
                rightMenuSubItems.Add("Red");
                rightMenuSubItems.Add("Yellow");
                rightMenuSubItems.Add("Blue");
                menuController.RightMenuSubRefresh(rightMenuSubItems);
                return;
            case "Transparency":
                rightMenuSubItems.Add("25%");
                rightMenuSubItems.Add("50%");
                rightMenuSubItems.Add("75%");
                rightMenuSubItems.Add("100%");
                menuController.RightMenuSubRefresh(rightMenuSubItems);
                return;
        }
    }

    protected void LeftMenuRefresh()
    {
        List<string> leftMenuItems = new List<string>();
        foreach (MapARLayer layer in mapARLayersDisActived)
        {
            leftMenuItems.Add(layer.getLayerName());
        }
        menuController.LeftMenuRefresh(leftMenuItems);
    }

    protected void RightMenuClose()
    {
        menuController.RightMenuClose();

        menuController.RightMenuSubClose();
    }

    protected void RightMenuSubClose()
    {
        menuController.RightMenuSubClose();
    }

    protected void LeftMenuClose()
    {
        menuController.LeftMenuClose();
    }

    /*protected void MapLayerSelectNext(int level)
    {
        int boundary = this.mapARLayersActived.Count - 1 - currentSelectedIndex;
        if (boundary < level)
        {
            level = boundary;
        }
        this.currentSelectedIndex += level;
        // all up one
        foreach (MapARLayerDup layerDup in mapARLayersActivedDup)
        {
            StartCoroutine(DuplicationMovement(layerDup, Vector3.down, level * dupGap, level * dupMovementTime, 1));
        }
        MapHightLightUpdate();
        RightMenuRefresh();
    }*/
    protected void MapLayerSelectNext()
    {
        if (this.currentSelectedIndex < mapARLayersActivedDup.Count -1)
        {
            this.currentSelectedIndex += 1;
            // all drop one
            foreach (MapARLayerDup layerDup in mapARLayersActivedDup)
            {
                StartCoroutine(DuplicationMovement(layerDup, Vector3.down, dupGap, dupMovementTime, 1));
            }
        }/*
        else
        {
            this.currentSelectedIndex = 0;
        }*/
        MapHightLightUpdate();
        RightMenuRefresh();
        menuController.WatchOn(-1);
    }

    protected void RightMenuSelectNext()
    {
        menuController.RightMenuSelectNext();
        RightMenuSubRefresh(menuController.getRightMenuCMD());
        menuController.WatchOn(1);
    }

    protected void RightMenuSubSelectNext()
    {
        menuController.RightMenuSubSelectNext();
    }
    protected void LeftMenuSelectNext()
    {
        menuController.LeftMenuSelectNext();
    }

    protected void MapLayerSelectLast()
    {
        if (this.currentSelectedIndex > 0)
        {
            this.currentSelectedIndex -= 1;
            // all up one
            foreach (MapARLayerDup layerDup in mapARLayersActivedDup)
            {
                StartCoroutine(DuplicationMovement(layerDup, Vector3.up, dupGap, dupMovementTime, 1));
            }
        }/*
        else
        {
            this.currentSelectedIndex = (this.mapARLayersActived.Count - 1);
        }*/
        MapHightLightUpdate();
        RightMenuRefresh();
        menuController.WatchOn(-1);
    }

    protected void RightMenuSelectLast()
    {
        menuController.RightMenuSelectLast();
        RightMenuSubRefresh(menuController.getRightMenuCMD());
        menuController.WatchOn(1);
    }

    protected void RightMenuSubSelectLast()
    {
        menuController.RightMenuSubSelectLast();
    }

    protected void LeftMenuSelectLast()
    {
        menuController.LeftMenuSelectLast();
    }

    protected void MapLayerSelect()
    {
        RightMenuSetUp();
        menuController.WatchOn(-1);
    }

    protected void RightMenuSelect()
    {
        string selectedCMD = menuController.getRightMenuCMD();
        Debug.Log(selectedCMD);
        switch (selectedCMD)
        {
            case "Show/Hide":
                HideOrShow();
                return;
            case "Color":
                RightMenuSubSetUp(selectedCMD);
                menuController.WatchOn(1);
                return;
            case "Transparency":
                RightMenuSubSetUp(selectedCMD);
                menuController.WatchOn(1);
                return;
        }
    }

    protected void RightMenuSubSelect()
    {
        string selectedCMD = menuController.getRightMenuSubCMD();
        Debug.Log(selectedCMD);
        switch (selectedCMD)
        {
            case "Red":
                setColor(Color.red);
                return;
            case "Blue":
                setColor(Color.blue);
                return;
            case "Yellow":
                setColor(Color.yellow);
                return;
            case "25%":
                setTransparency(.25f);
                return;
            case "50%":
                setTransparency(.50f);
                return;
            case "75%":
                setTransparency(.75f);
                return;
            case "100%":
                setTransparency(1f);
                return;
        }
    }

    protected void LeftMenuSelect()
    {
        string selectedCMD = menuController.getLeftMenuCMD();
        Debug.Log(selectedCMD);
        foreach(MapARLayer layer in mapARLayersDisActived)
        {
            if (layer.nameIs(selectedCMD))
            {
                AddLayer(layer);
                LeftMenuRefresh();
                MapHightLightUpdate();
                return;
            }
        }
    }

    protected void MapLayerSwitchNext()
    {
        if (this.currentSelectedIndex < mapARLayersActivedDup.Count - 1)
        {
            this.currentSelectedIndex += 1;
            var temp = mapARLayersActivedDup[currentSelectedIndex - 1];
            mapARLayersActivedDup[currentSelectedIndex - 1] = mapARLayersActivedDup[currentSelectedIndex];
            mapARLayersActivedDup[currentSelectedIndex] = temp;
            StartCoroutine(DuplicationMovement(mapARLayersActivedDup[currentSelectedIndex - 1], Vector3.down, dupGap, dupMovementTime, 1));
            StartCoroutine(DuplicationMovement(mapARLayersActivedDup[currentSelectedIndex], Vector3.up, dupGap, dupMovementTime, 1));
            // all drop one
            foreach (MapARLayerDup layerDup in mapARLayersActivedDup)
            {
                StartCoroutine(DuplicationMovement(layerDup, Vector3.down, dupGap, dupMovementTime, 1));
            }
        }/*
        else
        {
            this.currentSelectedIndex = 0;
        }*/
        MapHightLightUpdate();
        RightMenuRefresh();
        menuController.WatchOn(-1);
    }

    protected void MapLayerSwitchLast()
    {
        if (this.currentSelectedIndex > 0)
        {
            this.currentSelectedIndex -= 1;
            var temp = mapARLayersActivedDup[currentSelectedIndex + 1];
            mapARLayersActivedDup[currentSelectedIndex + 1] = mapARLayersActivedDup[currentSelectedIndex];
            mapARLayersActivedDup[currentSelectedIndex] = temp;
            StartCoroutine(DuplicationMovement(mapARLayersActivedDup[currentSelectedIndex + 1], Vector3.up, dupGap, dupMovementTime, 1));
            StartCoroutine(DuplicationMovement(mapARLayersActivedDup[currentSelectedIndex], Vector3.down, dupGap, dupMovementTime, 1));
            // all up one
            foreach (MapARLayerDup layerDup in mapARLayersActivedDup)
            {
                StartCoroutine(DuplicationMovement(layerDup, Vector3.up, dupGap, dupMovementTime, 1));
            }
        }/*
        else
        {
            this.currentSelectedIndex = (this.mapARLayersActived.Count - 1);
        }*/
        MapHightLightUpdate();
        RightMenuRefresh();
        menuController.WatchOn(-1);
    }

    protected void AddLayer(MapARLayer layer)
    {
        MapARLayerDup mapARLayerDup = layer.getDuplication();
        mapARLayerDup.show();
        mapARLayersActived.Insert(0, layer);
        mapARLayersDisActived.Remove(layer);
        StartCoroutine(DuplicationMovement(mapARLayerDup, Vector3.up, dupGap * 5, dupMovementTime, 1));
        mapARLayersActivedDup.Insert(currentSelectedIndex, mapARLayerDup);
        foreach (MapARLayerDup layerDup in mapARLayersActivedDup)
        {
            if (mapARLayersActivedDup.IndexOf(layerDup) > currentSelectedIndex)
            {
                StartCoroutine(DuplicationMovement(layerDup, Vector3.up, dupGap, dupMovementTime, 1));
            }
        }
        MapHightLightUpdate();
    }

    protected void DeleteLayer(MapARLayer layer)
    {
        MapARLayerDup mapARLayerDup = layer.getDuplication();
        mapARLayersActived.Remove(layer);
        mapARLayersDisActived.Insert(0, layer);
        StartCoroutine(DuplicationMovement(mapARLayerDup, Vector3.down, dupGap * 5, dupMovementTime, 1));
        mapARLayersActivedDup.Remove(mapARLayerDup);
        //mapARLayerDup.destory(); 
        foreach (MapARLayerDup layerDup in mapARLayersActivedDup)
        {
            if (mapARLayersActivedDup.IndexOf(layerDup)>=currentSelectedIndex)
            {
                StartCoroutine(DuplicationMovement(layerDup, Vector3.down, dupGap, dupMovementTime, 1));
            }
        }
        mapARLayerDup.hide();
        MapHightLightUpdate();
    }

    protected void MapHightLightUpdate()
    {
        if (currentSelectedIndex >= mapARLayersActived.Count)
        {
            currentSelectedIndex = mapARLayersActived.Count - 1;
        }
        if(currentSelectedIndex<0 && mapARLayersActived.Count > 0)
        {
            currentSelectedIndex = 0;
        }
        if (layerLabel != null)
        {
            if (currentSelectedIndex >= 0)
            {
                layerLabel.GetComponent<TextMesh>().text = ("Current Layer: " + mapARLayersActived[currentSelectedIndex].getLayerName());
                foreach (MapARLayerDup layerDup in mapARLayersActivedDup) 
                {
                    if (layerDup.isHighLighted())
                    {
                        StartCoroutine(DuplicationMovement(layerDup, Vector3.forward, dupHLGap, dupMovementTime
                            , 1));
                        layerDup.cancelHighLight();
                    }
                    layerDup.showSelf();
                    layerDup.hideText();
                    if ((currentSelectedIndex - mapARLayersActivedDup.IndexOf(layerDup) > 2))
                    {
                        layerDup.hideSelf();
                        }
                }
                try
                {
                    mapARLayersActivedDup[currentSelectedIndex].highLight();
                    StartCoroutine(DuplicationMovement(mapARLayersActivedDup[currentSelectedIndex], -Vector3.forward, dupHLGap, dupMovementTime
                        , 1));
                }
                catch
                {
                }
            }
            else
            {
                layerLabel.GetComponent<TextMesh>().text = ("No Layer on the Map ");
            }
        }
    }

    //=====================Functional methods==========================
    protected void HideOrShow()
    {
        if (mapARLayersActivedDup[currentSelectedIndex].getVisualState())
        {
            Invisual();
        }
        else
        {
            Visual();
        }
    }

    protected void Visual()
    {
        mapARLayersActivedDup[currentSelectedIndex].show();
    }

    protected void Invisual()
    {
        mapARLayersActivedDup[currentSelectedIndex].hide();
    }

    protected void setTransparency(float rate)
    {
        mapARLayersActivedDup[currentSelectedIndex].setTransparency(rate);
    }

    protected void setColor(Color color)
    {
        mapARLayersActivedDup[currentSelectedIndex].setColor(color);
    }

    public void Duplicate()
    {
        //get all duplication
        foreach (MapARLayer layer in mapARLayersActived)
        {
            mapARLayersActivedDup.Add(layer.getDuplication());
        }
        //move up all duplication
        float distance = (5-currentSelectedIndex) * dupGap;
        foreach (MapARLayerDup layerDup in mapARLayersActivedDup)
        {
            StartCoroutine(DuplicationMovement(layerDup, Vector3.up, distance, dupMovementTime, 1));
            distance += dupGap;
        }
        MapHightLightUpdate();
        dupActived = true;
    }

    public void DuplicateClose()
    {
        //move back all duplication
        float distance = (currentSelectedIndex - 5) * dupGap;
        foreach (MapARLayerDup layerDup in mapARLayersActivedDup)
        {
            if (layerDup.isHighLighted())
            {
                StartCoroutine(DuplicationMovement(layerDup, Vector3.forward, dupHLGap, dupMovementTime
                    , 1));
                layerDup.cancelHighLight();
            }
            StartCoroutine(DuplicationMovement(layerDup, Vector3.up, distance, dupMovementTime
                , 2));
            distance -= dupGap;
        }
        //clean data
        mapARLayersActivedDup.Clear();
        dupActived = false;
    }

    protected IEnumerator DuplicationMovement(MapARLayerDup targetLayer, Vector3 movementDirection, float movementDistance, float movementTime, int mode)
    {
        //this.Lock = true;
        for (float t = 0.0f; t < movementTime;)
        {
            float td = Time.deltaTime;
            //targetLayer.moveUp(movementDistance * Time.deltaTime / movementTime);
            targetLayer.moveTowards(movementDirection, movementDistance * Time.deltaTime / movementTime);
            t += td;
            yield return null;
        }
        if (mode == 2)
        {
            targetLayer.destory();
        }
        //this.Lock = false;
    }


    protected void DeletingLayersForTest()
    {
        while (mapARLayersActived.Count > 11)
        {
            DeleteLayer(mapARLayersActived[0]);
        }
        LeftMenuRefresh();
        menuController.WatchOn(-1);
    }

}
