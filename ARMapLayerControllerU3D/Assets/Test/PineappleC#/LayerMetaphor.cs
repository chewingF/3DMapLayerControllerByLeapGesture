using Mapbox.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity.Interaction;
using Leap.Unity;

public class LayerMetaphor : TouchableObject {

    private MapARLayer layer;
    private List<SelectionMetaphor> selectionObjects = new List<SelectionMetaphor>();
    private InteractionBehaviour interactionBehaviour;
    private HandleMetaphor handle;
    public HandleMetaphor Handle
    {
        get
        {
            return handle;
        }
    }
    private GameObject goText;
    float quickGraspTimeMin = 0.05f;
    float quickGraspTimeMax = 0.2f;

    private Vector3 localScaleOrg = new Vector3(2.5f, 0.15f, 2.5f);
    private float reScaleRate = .11f;
    private bool crowed = false;
    public bool IsCrowed
    {
        get
        {
            return crowed;
        }
    }

    private float graspedTime = 0f;
    private bool _isGrasped = false;
    public bool isGrasped
    {
        get
        {
            return _isGrasped;
        }
    }
    private bool _isLongGrasped = false;
    public bool isLongGrasped
    {
        get
        {
            return _isLongGrasped;
        }
    }
    private bool _isLocked = false;
    public bool isLocked
    {
        get
        {
            return _isLocked;
        }
    }
    private bool _oldVersion = false;
    public bool oldVersion
    {
        set
        {
            _oldVersion = value;
        }
    }


    public LayerMetaphor()
    {
    }

    public LayerMetaphor(MapARLayer layer)
    {
        this.layer = layer;
        initMetaphorObject();
    }

    public LayerMetaphor(MapARLayer layer, bool oldVersion)
    {
        this.layer = layer;
        _oldVersion = oldVersion;
        initMetaphorObject();
    }

    public LayerMetaphor(MapARLayer layer, GameObject metaphorObject)
    {
        this.layer = layer;
        this.worldObject = metaphorObject;
    }

    private void initMetaphorObject()
    {
        if (layer == null)
        {
            return;
        }
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = layer.getLayerName().ToUpper();
        cube.transform.localScale = 0.1f * localScaleOrg;
        cube.GetComponent<MeshRenderer>().enabled = true;
        //GameObject.Destroy(Cube.GetComponent<Collider>());
        //MeshCollider mc = Cube.AddComponent<MeshCollider>();
        //mc.convex = true;
        BoxCollider bc = cube.GetComponent<BoxCollider>();
        bc.center = new Vector3(0.5f,0f,0f);
        bc.size = new Vector3(2.0f,1f,1f);
        Material newMaterial = (Material)Resources.Load("LayerMetaphorDefault", typeof(Material));

        cube.GetComponent<Renderer>().material = newMaterial;
        goText = new GameObject("Text_" + layer.getLayerName());
        TextMesh tm = goText.AddComponent<TextMesh>();
        tm.text = layer.getLayerName();
        tm.characterSize = 2;
        goText.transform.parent = cube.transform;
        goText.transform.localPosition = new Vector3(.6f, 0f, 0f);
        goText.transform.localScale = new Vector3(.1f, .8f, .1f);

        if (!_oldVersion)
        {
            cube.GetComponent<MeshRenderer>().enabled = false;
            MapARLayerDup layerDup = layer.getDuplication();
            layerDup.setDupObjParent(cube.transform);
            layerDup.hideText();
        }

        Rigidbody rb = cube.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        cube.AddComponent<LockRotation>();
        //InteractionBehaviour ib = cylinder.AddComponent<InteractionBehaviour>();

        //System.Action a = () => highLightDup();
        //ib.OnGraspBegin = a;
        //a = () => cancelHighLightDup();
        //ib.OnGraspEnd = a;

        worldObject = cube;
        interactionBehaviour = worldObject.AddComponent<InteractionBehaviour>();// (standard_ib);
        interactionBehaviour.moveObjectWhenGrasped = false;

        //worldObject.transform.Rotate(Vector3.up, 120);
    }

    public void setHandle(HandleMetaphor hm)
    {
        handle = hm;
    }

    public void update(List<UnwrappedTileId> tileIds)
    {
        layer.updateLayer(tileIds);
        if (!_oldVersion)
        {
            MapARLayerDup layerDup = layer.getDuplication();
            layerDup.hideText();
            layerDup.setDupObjParent(this.worldObject.transform);
            layerDup.resetLocalScaleBy(reScaleRate);
            //layerDup.resetLocalPos(new Vector3(0, 0, 0));
            layerDup.resetLocalPos(new UnityEngine.Pose());
        }
    }

    public void enableGraspMovement()
    {
        interactionBehaviour.moveObjectWhenGrasped = true;
    }

    public void enableActivedGraspActions()
    {
        setOnContactEndAction(() => onContactEndActived());
        setOnGraspBeginAction(() => onGraspBeginActived());
        setOnGraspStayAction(() => onGraspStayActived());
        setOnGraspEndAction(() => onGraspEndActived());
    }

    public void enableRemovedGraspActions()
    {
        setOnContactEndAction(() => onContactEndRemoved());
        setOnGraspBeginAction(() => onGraspBeginRemoved());
        setOnGraspStayAction(() => onGraspStayRemoved());
        setOnGraspEndAction(() => onGraspEndRemoved());
    }

    public void disableGraspMovement()
    {
        interactionBehaviour.moveObjectWhenGrasped = false;
    }

    public void disableGraspActions()
    {
        setOnGraspBeginAction(null);
        setOnGraspStayAction(null);
        setOnGraspEndAction(null);
    }

    public void setOnContactEndAction(System.Action a)
    {
        interactionBehaviour.OnContactEnd = a;
    }

    public void setOnGraspBeginAction(System.Action a)
    {
        interactionBehaviour.OnGraspBegin = a;
    }

    public void setOnGraspStayAction(System.Action a)
    {
        interactionBehaviour.OnGraspStay = a;
    }

    public void setOnGraspEndAction(System.Action a)
    {
        interactionBehaviour.OnGraspEnd = a;
    }

    private void onContactEndActived()
    {
        //cancelAllGraspState();
        //cancelAllHighLight();
    }

    private void onContactEndRemoved()
    {
        //moveObjectTo(LayerManager.bin.safeUpdatePos);
        //cancelAllGraspState();
        //cancelAllHighLight();
    }

    private void onGraspBeginActived()
    {
        _isLocked = true;
        graspedTime = 0;
        _isGrasped = true;
        highLightText(Color.red);
        _isLocked = false;
    }

    private void onGraspBeginRemoved()
    {
        _isGrasped = true;
        _isLocked = true;
        graspedTime = 0;
        highLightText(Color.red);
        _isLocked = false;
    }

    protected void onGraspStayActived()
    {
        _isLocked = true;
        graspedTime += Time.deltaTime;
        if (interactionBehaviour.closestHoveringHand == null)
        {
            return;
        }
        if (graspedTime < quickGraspTimeMax)
        {
            _isLocked = false;
            return;
        }
        else
        {
            if (!_isLongGrasped)
            {
                _isLongGrasped = true;
                unselectActions();
                enableGraspMovement();
                //InteractionController c = interactionBehaviour.graspedPoseHandler.co
                interactionBehaviour.graspedPoseHandler.AddController(interactionBehaviour.graspingController);
                //enableGraspActions();
                LayerManager.goSelected = null;
            }
        }
        bool inRange = LayerManager.handle.updateLayerExisted(this);
        if (LayerManager.layerMetaphorsActived.Contains(this))
        {
            if (!inRange)
            {
                LayerManager.layerMetaphorsActived.Remove(this);
                LayerManager.layerMetaphorsDisActived.Add(this);
                this.getLayer().hide();
                if (!_oldVersion)
                {
                    this.getLayer().getDuplication().hideSelf();
                    this.worldObject.GetComponent<MeshRenderer>().enabled = true;
                    LayerManager.handle.removeLayer(this);
                    LayerManager.bin.addLayer(this);
                }
            }
            else
            {
                if (handle != null && !IsSelected)
                {
                    Hand h = interactionBehaviour.closestHoveringHand;
                    if (_oldVersion)
                    {
                        handle.updateIndex(worldObject.transform.position);
                    }
                    else
                    {
                        handle.updateIndex(UnityVectorExtension.ToVector3(h.PalmPosition));
                    }
                }
            }
        }
        else
        {
            if (inRange)
            {
                LayerManager.layerMetaphorsDisActived.Remove(this);
                LayerManager.layerMetaphorsActived.Add(this);
                this.getLayer().show();
                LayerManager.handle.addLayer(this);
                if (!_oldVersion)
                {
                    LayerManager.bin.removeLayer(this);
                    this.getLayer().getDuplication().showSelf();
                    this.getLayer().getDuplication().hideText();
                    this.worldObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }

        if (handle != null && !IsSelected)
        {
            Hand h = interactionBehaviour.closestHoveringHand;
            handle.updateObjectsOrder();
            if (_oldVersion)
            {
                handle.updateObjectsPositions(worldObject.transform.position);
            }
            else
            {
                handle.updateObjectsPositions(UnityVectorExtension.ToVector3(h.PalmPosition));
            }
        }
        _isLocked = false;
    }


    protected void onGraspStayRemoved()
    {
        _isLocked = true;
        graspedTime += Time.deltaTime;
        if (interactionBehaviour.closestHoveringHand == null)
        {
            return;
        }
        if (graspedTime < quickGraspTimeMax)
        {
            _isLocked = false;
            return;
        }
        else
        {
            if (!_isLongGrasped)
            {
                _isLongGrasped = true;
                unselectActions();
                enableGraspMovement();
                //enableGraspActions();
                LayerManager.goSelected = null;
            }
        }
        bool inRange = LayerManager.handle.updateLayerExisted(this);
        if (LayerManager.layerMetaphorsDisActived.Contains(this))
        {
            if (inRange)
            {
                LayerManager.layerMetaphorsDisActived.Remove(this);
                LayerManager.layerMetaphorsActived.Add(this);
                if (_oldVersion)
                {
                    LayerManager.handle.updateIndex(worldObject.transform.position);
                }
                else
                {
                    LayerManager.handle.updateIndex(LayerManager.handle.safeUpdatePos);
                }
                this.getLayer().show();
                if (!_oldVersion)
                {
                    LayerManager.bin.removeLayer(this);
                    this.getLayer().getDuplication().showSelf();
                    this.getLayer().getDuplication().hideText();
                    this.worldObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }
            else
            {
                if (handle != null && !IsSelected)
                {
                    Hand h = interactionBehaviour.closestHoveringHand;
                    if (_oldVersion)
                    {
                        handle.updateIndex(worldObject.transform.position);
                    }
                    else
                    {
                        handle.updateIndex(UnityVectorExtension.ToVector3(h.PalmPosition));
                    }
                }
            }
        }
        else
        {
            if (inRange)
            {
                if (handle != null && !IsSelected)
                {
                    Hand h = interactionBehaviour.closestHoveringHand;
                    if (_oldVersion)
                    {
                        handle.updateIndex(worldObject.transform.position);
                    }
                    else
                    {
                        handle.updateIndex(UnityVectorExtension.ToVector3(h.PalmPosition));
                    }
                }
            }
            else
            {
                LayerManager.layerMetaphorsActived.Remove(this);
                LayerManager.layerMetaphorsDisActived.Add(this);
                this.getLayer().hide();
                if (!_oldVersion)
                {
                    this.getLayer().getDuplication().hideSelf();
                    this.worldObject.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }

        if (handle != null && !IsSelected)
        {
            Hand h = interactionBehaviour.closestHoveringHand;
            handle.updateObjectsOrder();
            if (_oldVersion)
            {
                handle.updateObjectsPositions(worldObject.transform.position);
            }
            else
            {
                handle.updateObjectsPositions(UnityVectorExtension.ToVector3(h.PalmPosition));
            }
        }
        _isLocked = false;
    }

    protected void onGraspEndActived()
    {
        _isLocked = true;
        _isGrasped = false;
        cancelHighLightText();
        if (_isLongGrasped)
        {
            _isLongGrasped = false;
            if (!LayerManager.handle.updateLayerExisted(this))
            {
                this.cancelHighLightText();
                if (!_oldVersion)
                {
                    LayerManager.bin.addLayer(this);
                    this.moveObjectTo(LayerManager.bin.safeUpdatePos);
                    LayerManager.bin.updateObjectsOrder();
                    LayerManager.bin.updateIndex(LayerManager.bin.safeUpdatePos);
                    LayerManager.bin.updateObjectsPositions(LayerManager.bin.safeUpdatePos);
                }
            }
            _isLocked = false;
            return;
        }
        else if (graspedTime >= quickGraspTimeMin && handle != null)
        {
            Hand h = interactionBehaviour.closestHoveringHand;
            handle.updateObjectsOrder();
            if (_oldVersion)
            {
                handle.updateObjectsPositions(worldObject.transform.position);
            }
            else
            {
                handle.updateObjectsPositions(UnityVectorExtension.ToVector3(h.PalmPosition));
            }
            changeSelectState();
            if (IsSelected)
            {
                selectActions();
                disableGraspMovement();
                //disableGraspActions();
                if (LayerManager.goSelected == null)
                {
                    LayerManager.goSelected = this;
                }
                else if (LayerManager.goSelected != this)
                {
                    //LayerManager.goSelected.reColorWorldObj(Color.white);
                    LayerManager.goSelected.unselectActions();
                    LayerManager.goSelected = this;
                }
            }
            else
            {
                unselectActions();
                //enableGraspMovement();
                //enableGraspActions();
                LayerManager.goSelected = null;
            }
        }
        /*
        if (handle != null && !IsSelected)
        {
            Hand h = interactionBehaviour.closestHoveringHand;
            handle.updateObjectsPositions(UnityVectorExtension.ToVector3(h.PalmPosition));
        }*/
        _isLocked = false;
    }


    protected void onGraspEndRemoved()
    {
        _isLocked = true;
        _isGrasped = false;
        cancelHighLightText();
        if (_isLongGrasped)
        {
            _isLongGrasped = false;
            _isLocked = false;
            return;
        }
        _isLocked = false;/*
        changeSelectState();
        if (IsSelected)
        {
            selectActions();
            disableGraspMovement();
            //disableGraspActions();
            if (LayerManager.goSelected == null)
            {
                LayerManager.goSelected = this;
            }
            else if (LayerManager.goSelected != this)
            {
                //LayerManager.goSelected.reColorWorldObj(Color.white);
                LayerManager.goSelected.unselectActions();
                LayerManager.goSelected = this;
            }
        }
        else
        {
            unselectActions();
            //enableGraspMovement();
            //enableGraspActions();
            LayerManager.goSelected = null;
        }*/
        /*
        if (handle != null && !IsSelected)
        {
            Hand h = interactionBehaviour.closestHoveringHand;
            handle.updateObjectsPositions(UnityVectorExtension.ToVector3(h.PalmPosition));
        }*/
    }

    public void selectActions()
    {
        if (_oldVersion)
        {
            reColorWorldObj(Color.red);
        }
        else
        {
            highLightDup();
        }
        createSelections();
        disableGraspMovement();
    }

    public void unselectActions()
    {
        if (_oldVersion)
        {
            reColorWorldObj(Color.white);
        }
        else
        {
            cancelHighLightDup();
        }
        removeSelections();
        endSelectState();
    }

    public void createSelections()
    {
        List<string> attributes = layer.getMapAttributes();
        int count = 0;
        foreach (string attStr in attributes)
        {
            SelectionMetaphor sm = new SelectionMetaphor(this, -0.7f, attStr);
            //sm.rotateBy(30 * count);
            sm.pushTo(-0.7f - 0.4f * count);
            selectionObjects.Add(sm);
            count++;
        }
    }

    public void createSelections(string selectionStr)
    {
        removeSelections();
        List<string> attributes = new List<string>();
        if (selectionStr == "COLOR")
        {
            attributes.Add("Red");
            attributes.Add("Yellow");
            attributes.Add("Blue");
        }
        else if (selectionStr == "TRANSPARENCY")
        {
            attributes.Add("25");
            attributes.Add("50");
            attributes.Add("75");
            attributes.Add("100");
        }
        else if (_oldVersion)
        {
            if (selectionStr == "RED")
            {
                layer.setColor(Color.red);
            }
            else if (selectionStr == "YELLOW")
            {
                layer.setColor(Color.yellow);
            }
            else if (selectionStr == "BLUE")
            {
                layer.setColor(Color.blue);
            }
            else if (selectionStr == "25")
            {
                layer.setTransparency(25);
            }
            else if (selectionStr == "25")
            {
                layer.setTransparency(25);
            }
            else if (selectionStr == "50")
            {
                layer.setTransparency(50);
            }
            else if (selectionStr == "100")
            {
                layer.setTransparency(100);
            }
            endSelectState();
            unselectActions();
            LayerManager.goSelected = null;
        }
        else
        {
            MapARLayerDup layerdup = layer.getDuplication();
            if (selectionStr == "RED")
            {
                layerdup.setColor(Color.red);
            }
            else if (selectionStr == "YELLOW")
            {
                layerdup.setColor(Color.yellow);
            }
            else if (selectionStr == "BLUE")
            {
                layerdup.setColor(Color.blue);
            }
            else if (selectionStr == "25")
            {
                layerdup.setTransparency(25);
            }
            else if (selectionStr == "25")
            {
                layerdup.setTransparency(25);
            }
            else if (selectionStr == "50")
            {
                layerdup.setTransparency(50);
            }
            else if (selectionStr == "100")
            {
                layerdup.setTransparency(100);
            }
            endSelectState();
            unselectActions();
            LayerManager.goSelected = null;
        }
        int count = 0;
        foreach (string attStr in attributes)
        {
            SelectionMetaphor sm = new SelectionMetaphor(this, -0.7f, attStr);
            //sm.rotateBy(30 * count);
            sm.pushTo(-0.7f - 0.4f * count);
            selectionObjects.Add(sm);
            count++;
        }
    }

    public void removeSelections() {
        foreach (SelectionMetaphor sm in selectionObjects)
        {
            GameObject.Destroy(sm.getMetaphorObject());
        }
        selectionObjects.Clear();
    }

    public MapARLayer getLayer()
    {
        return layer;
    }
    

    public void reScaleOrginal()
    {
        worldObject.transform.localScale = localScaleOrg;
        crowed = false;
    }


    public void reScaleByRate(float rate)
    {
        this.reScaleRate = rate;
        worldObject.transform.localScale = localScaleOrg * rate;
        crowed = (rate < 1);
    }

    public void highLightDup()
    {
        if (layer != null && !_oldVersion)
        {
            MapARLayerDup layerdup = layer.getDuplication();
            layerdup.highLight();
        }
    }

    public void cancelHighLightDup()
    {
        if (layer != null && !_oldVersion)
        {
            MapARLayerDup layerdup = layer.getDuplication();
            layerdup.cancelHighLight();
        }
    }

    public void highLightText()
    {
        TextMesh tm = goText.GetComponent<TextMesh>();
        tm.color = Color.black;
    }

    public void highLightText(Color c)
    {
        TextMesh tm = goText.GetComponent<TextMesh>();
        tm.color = c;
    }

    public void cancelHighLightText()
    {
        TextMesh tm = goText.GetComponent<TextMesh>();
        tm.color = Color.white;
    }

    public void cancelAllHighLight()
    {
        cancelHighLightText();
        if (_oldVersion)
        {

        }
        else
        {
            cancelHighLightDup();
        }
    }

    public void cancelAllGraspState()
    {
        _isGrasped = false;
        _isLongGrasped = false;
    }

    public List<SelectionMetaphor> getSelectionObjects()
    {
        return selectionObjects;
    }

    public override float distanceToCenter(Vector3 pinchingPos)
    {
        if (handle == null)
        {
            return base.distanceToCenter(pinchingPos);
        }
        else
        {
            Collider coll = worldObject.GetComponent<Collider>();
            Vector3 handlePos = handle.getMetaphorObject().transform.position;
            Vector3 collCenter = coll.bounds.center;
            float distance = Vector3.Distance(new Vector3(handlePos.x,collCenter.y,handlePos.z), pinchingPos);
            return distance;
        }
    }
}
