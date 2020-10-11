using Mapbox.Map;
using Mapbox.Unity.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapARLayer
{
    protected VectorSubLayerProperties vectorSubLayerProperties;
    protected string layerName;
    protected GameObject layerGameObjects = new GameObject();
    protected List<GameObject> basementGameObjects = new List<GameObject>();
    protected List<string> layerAttributes;
    protected bool visualState = true;
    protected MapARLayerDup mapARLayerDup = null;


    public MapARLayer()
    {
    }

    public MapARLayer(VectorSubLayerProperties vslp)
    {
        this.vectorSubLayerProperties = vslp;
        this.layerName = vslp.coreOptions.sublayerName;
        this.layerGameObjects.name = "LayerGameObjects_" + this.layerName;
        this.updateLayerGameObjects();
        this.setLayerAttributes();
    }
    
    protected void updateLayerGameObjects()
    {
        foreach (GameObject gameObj in GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.Contains(this.layerName + " - ")))
        {
            gameObj.transform.parent = layerGameObjects.transform;
        }
    }

    protected void updateLayerBasement(List<UnwrappedTileId> tileIDs)
    {
        foreach (UnwrappedTileId id in tileIDs){
            basementGameObjects.Add(GameObject.Find(id.ToString()));
        }
        if (mapARLayerDup != null)
        {
            mapARLayerDup.destory();
            mapARLayerDup = new MapARLayerDup(this);
        }
    }

    public void updateLayer(List<UnwrappedTileId> tileIDs)
    {
        updateLayerGameObjects();
        updateLayerBasement(tileIDs);
        if (this.mapARLayerDup != null)
        {
            this.mapARLayerDup.destory();
            this.mapARLayerDup = new MapARLayerDup(this);
        }
    }

    protected void setLayerName(string layerName)
    {
        this.layerName = layerName;
    }

    protected void setLayerGameObject(GameObject layerGameObjects)
    {
        this.layerGameObjects = layerGameObjects;
    }

    protected void setLayerGameObject(List<GameObject> layerGameObjects)
    {
        this.layerGameObjects = new GameObject("LayerGameObjects_" + this.layerName);
        foreach (GameObject gameObj in layerGameObjects)
        {
            gameObj.transform.parent = this.layerGameObjects.transform.parent;
        }
    }

    protected void setBasementGameObject(List<GameObject> basementGameObjects)
    {
        this.basementGameObjects.Clear();
        foreach (GameObject gameObj in basementGameObjects)
        {
            this.basementGameObjects.Add(gameObj);
        }
    }

    protected void setLayerAttributes()
    {
        layerAttributes = new List<string>();
        //this.layerAttributes.Add("Show/Hide");
        this.layerAttributes.Add("Color");
        this.layerAttributes.Add("Transparency");
        /*
        switch (this.layerName)
        {
            case "road":
                this.layerAttributes.Add("Color");
                this.layerAttributes.Add("Transparency");
                return;
            case "building":
                return;
            case "water":
                this.layerAttributes.Add("Color");
                this.layerAttributes.Add("Transparency");
                return;
        }
        */
    }

    public string getLayerName()
    {
        return layerName;
    }
    
    public GameObject getLayerGameObject()
    {
        return layerGameObjects;
    }
    
    public List<GameObject> getBaseMentGameObject()
    {
        return basementGameObjects;
    }

    public MapARLayerDup getDuplication()
    {
        if (this.mapARLayerDup == null)
        {
            this.mapARLayerDup = new MapARLayerDup(this);
        }
        return this.mapARLayerDup;
    }

    public void removeDuplication()
    {
        this.mapARLayerDup = null;
    }

    public bool getVisualState()
    {
        return visualState;
    }

    public List<string> getMapAttributes()
    {
        return layerAttributes;
    }

    public VectorSubLayerProperties GetVectorSubLayerProperties()
    {
        return vectorSubLayerProperties;
    }

    public bool nameIs(string name)
    {
        return this.layerName == name;
    }

    public virtual void changeVisualState()
    {
        if (visualState)
        {
            hide();
        }
        else
        {
            show();
        }
    }

    public virtual void show()
    {
        /*vectorSubLayerProperties.SetActive(true);
        vectorSubLayerProperties.HasChanged = true;*/
        showAll(layerGameObjects);
        visualState = true;
    }


    protected void showAll(GameObject gameObj)
    {
        try
        {
            MeshRenderer mr = gameObj.GetComponent<MeshRenderer>();
            if (!mr.enabled)
            {
                mr.enabled = true;
            }
        }
        catch
        {

        }
        foreach (Transform t in gameObj.transform)
        {
            showAll(t.gameObject);
        }
    }

    public virtual void hide()
    {
        /*vectorSubLayerProperties.SetActive(false);
        vectorSubLayerProperties.HasChanged = true;*/
        hideAll(layerGameObjects);
        visualState = false;
    }

    protected void hideAll(GameObject gameObj)
    {
        try
        {
            MeshRenderer mr = gameObj.GetComponent<MeshRenderer>();
            if (mr.enabled)
            {
                mr.enabled = false;
            }
        }
        catch
        {

        }
        foreach (Transform t in gameObj.transform)
        {
            hideAll(t.gameObject);
        }
    }

    public virtual void setTransparency(float rate)
    {
        /*
        Color c = vectorSubLayerProperties.Texturing.ColorStyle.FeatureColor;
        c.a = rate;
        vectorSubLayerProperties.Texturing.ColorStyle.FeatureColor = c;
        //vectorSubLayerProperties.coreOptions.HasChanged = true;
        */
        setAllTransparency(layerGameObjects, rate);
    }

    protected void setAllTransparency(GameObject gameObj, float rate)
    {
        try
        {
            MeshRenderer mr = gameObj.GetComponent<MeshRenderer>();
            foreach (Material m in mr.materials)
            {
                var color = m.color;
                var newColor = new Color(color.r, color.g, color.b, rate);
                m.color = newColor;
            }
        }
        catch
        {
        }
        foreach (Transform t in gameObj.transform)
        {
            setAllTransparency(t.gameObject, rate);
        }
    }

    public virtual void setColor(Color color)
    {
        /*float a = vectorSubLayerProperties.Texturing.ColorStyle.FeatureColor.a;
        color.a = a;
        vectorSubLayerProperties.Texturing.ColorStyle.FeatureColor = color;
        //vectorSubLayerProperties.coreOptions.HasChanged = true;
        */
        setAllColor(layerGameObjects, color);
    }

    protected void setAllColor(GameObject gameObj, Color color)
    {
        try
        {
            MeshRenderer mr = gameObj.GetComponent<MeshRenderer>();
            foreach (Material m in mr.materials)
            {
                float a = m.color.a;
                color.a = a;
                m.color = color;
            }
        }
        catch
        {
        }
        foreach (Transform t in gameObj.transform)
        {
            setAllColor(t.gameObject, color);
        }
    }
    /*
    */
    /*
    public virtual void destoryAll()
    {
        foreach (GameObject go in layerGameObjects)
        {
            GameObject.Destroy(go);
        }
        layerGameObjects = new List<GameObject>();
    }*/

}
