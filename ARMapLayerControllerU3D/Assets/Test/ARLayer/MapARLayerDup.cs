using Mapbox.Unity.MeshGeneration.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapARLayerDup : MapARLayer
{
    private MapARLayer originalLayer;
    private bool highLighted = false;
    private GameObject allRelatedObj;
    private GameObject nameText;
    public GameObject NameText
    {
        get
        {
            return nameText;
        }
    }
    Vector3 renderCenter = new Vector3();

    float TranRateHL = 0.8f;
    float TranRateOg = 0.5f;

    public MapARLayerDup(MapARLayer originalLayer)
    {
        this.originalLayer = originalLayer;
        this.allRelatedObj = new GameObject();
        GameObject layerGameObjectsDup = new GameObject();
        List<GameObject> basementGameObjectsDup = new List<GameObject>();
        layerGameObjectsDup = UnityEngine.Object.Instantiate(originalLayer.getLayerGameObject(), originalLayer.getLayerGameObject().transform);
        layerGameObjectsDup.transform.parent = this.allRelatedObj.transform;
        Material newmaterial = new Material(Shader.Find("Transparent/Diffuse"));
        newmaterial.color = new Color(1f, 1f, 1f, TranRateOg);
        Vector3 baseCenterSum = new Vector3();
        foreach (GameObject gameObj in originalLayer.getBaseMentGameObject())
        {
            GameObject newGameObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newGameObj.transform.parent = gameObj.transform.parent;
            newGameObj.transform.position = gameObj.transform.position;
            newGameObj.transform.localScale = gameObj.transform.localScale;
            baseCenterSum += newGameObj.GetComponent<Renderer>().bounds.center;
            newGameObj.GetComponent<Renderer>().material = newmaterial;
            GameObject.Destroy(newGameObj.GetComponent<BoxCollider>());
            MeshFilter mf = newGameObj.GetComponent<MeshFilter>();
            mf.mesh = gameObj.GetComponent<MeshFilter>().mesh;
            newGameObj.transform.parent = this.allRelatedObj.transform;
            basementGameObjectsDup.Add(newGameObj);
        }
        this.renderCenter = baseCenterSum / originalLayer.getBaseMentGameObject().Count;
        this.setLayerGameObject(layerGameObjectsDup);
        this.setBasementGameObject(basementGameObjectsDup);

        this.nameText = new GameObject("DupName_" + originalLayer.getLayerName());
        TextMesh tm = nameText.AddComponent<TextMesh>();
        tm.text = originalLayer.getLayerName();
        tm.transform.Translate(1, 0, -1);
        tm.transform.localScale = new Vector3(.1f, .1f, .1f);
        this.nameText.transform.parent = allRelatedObj.transform;
        //this.allRelatedObj.AddComponent<MeshRenderer>();
        //Rigidbody rig = this.allRelatedObj.AddComponent<Rigidbody>();
        //rig.useGravity = false;
    }

    public bool isHighLighted()
    {
        return highLighted;
    }

    public void moveUp(float distance)
    {
        GameObject gameObj = this.getLayerGameObject();
        Vector3 orgPos = gameObj.transform.position;
        Vector3 trgPos = orgPos;
        trgPos.y += distance;
        gameObj.transform.position = trgPos;

        foreach (GameObject gameObjB in this.getBaseMentGameObject())
        {
            orgPos = gameObjB.transform.position;
            trgPos = orgPos;
            trgPos.y += distance;
            gameObjB.transform.position = trgPos;
        }

        nameText.transform.Translate(Vector3.up * distance);
    }

    public void moveTowards(Vector3 direction, float distance)
    {
        direction = direction.normalized;
        GameObject gameObj = this.getLayerGameObject();
        Vector3 orgPos = gameObj.transform.position;
        Vector3 trgPos = orgPos + direction * distance;
        gameObj.transform.position = trgPos;

        foreach (GameObject gameObjB in this.getBaseMentGameObject())
        {
            orgPos = gameObjB.transform.position;
            trgPos = orgPos + direction * distance;
            gameObjB.transform.position = trgPos;
        }

        nameText.transform.Translate(direction * distance);
    }

    public override void show()
    {
        showSelf();
        originalLayer.show();
    }

    public override void hide()
    {
        hideSelf();
        originalLayer.hide();
    }

    public void showSelf()
    {
        nameText.active = true;
        showAll(this.getLayerGameObject());
        foreach (GameObject gameObj in this.getBaseMentGameObject())
        {
            showAll(gameObj);
        }
    }

    public void hideSelf()
    {
        nameText.active = false;
        hideAll(this.getLayerGameObject());
        foreach (GameObject gameObj in this.getBaseMentGameObject())
        {
            hideAll(gameObj);
        }
    }

    public override void setTransparency(float rate)
    {
        setSelfTransparency(rate);
        originalLayer.setTransparency(rate);
    }

    public void setSelfTransparency(float rate)
    {
        setAllTransparency(this.getLayerGameObject(), rate);
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

    public override void setColor(Color color)
    {
        setSelfColor(color);
        originalLayer.setColor(color);
    }

    public void setSelfColor(Color color)
    {
        setAllColor(this.getLayerGameObject(), color);
    }


    public void hideText()
    {
        nameText.SetActive(false);
        //Debug.Log(nameText.name + nameText.active);
    }

    public void destory()
    {
        try
        {
            UnityEngine.Object.Destroy(this.getLayerGameObject());
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        foreach (GameObject gameObj in this.getBaseMentGameObject())
        {
            try
            {
                UnityEngine.Object.Destroy(gameObj);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        try
        {
            UnityEngine.Object.Destroy(nameText);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        originalLayer.removeDuplication();
    }


    public void highLight()
    {
        Material newmaterial = new Material(Shader.Find("Transparent/Diffuse"));
        newmaterial.color = new Color(1f, 1f, 1f, TranRateHL);
        foreach (GameObject gameObj in this.getBaseMentGameObject())
        {
            gameObj.GetComponent<MeshRenderer>().material = newmaterial;
        }
        highLighted = true;
    }

    public void cancelHighLight()
    {
        Material newmaterial = new Material(Shader.Find("Transparent/Diffuse"));
        newmaterial.color = new Color(1f, 1f, 1f, TranRateOg);
        foreach (GameObject gameObj in this.getBaseMentGameObject())
        {
            gameObj.GetComponent<MeshRenderer>().material = newmaterial;
        }
        highLighted = false;
    }

    public void setDupObjParent(Transform parTransform)
    {
        this.allRelatedObj.transform.parent = parTransform;
    }

    public void resetLocalScaleTo(Vector3 newScale)
    {
        Vector3 orgScale = this.allRelatedObj.transform.localScale;
        this.renderCenter = new Vector3(this.renderCenter.x * newScale.x / orgScale.x, this.renderCenter.x * newScale.y / orgScale.y, this.renderCenter.x * newScale.z / orgScale.z);
        this.allRelatedObj.transform.localScale = newScale;
    }

    public void resetLocalScaleBy(float rate)
    {
        Vector3 orgScale = this.allRelatedObj.transform.localScale;
        this.renderCenter = new Vector3(this.renderCenter.x * orgScale.x * rate, this.renderCenter.y * orgScale.y * rate, this.renderCenter.z * orgScale.z * rate);
        this.allRelatedObj.transform.localScale = orgScale * rate;
    }

    public void resetLocalPos(Vector3 newPos)
    {

        Vector3 gap = this.allRelatedObj.transform.position - renderCenter;
        this.allRelatedObj.transform.localPosition = newPos + gap;
    }

    public void resetLocalPos(Pose newPos)
    {

        Vector3 gap = this.allRelatedObj.transform.position - renderCenter;
        this.allRelatedObj.transform.localPosition = newPos.position + gap;
        this.allRelatedObj.transform.rotation = newPos.rotation;
    }
}
