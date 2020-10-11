using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMetaphor:TouchableObject{
    
    protected List<LayerMetaphor> layers = new List<LayerMetaphor>();
    protected GameObject baseObject;
    //protected float generalGap = 0.05f;
    //protected int maxShow = 5;
    protected int centerIndex = 0;
    //protected int centerTopIndex = 0;
    //protected int centerBotIndex = 0;
    //protected List<int> centerAroundIndexs = new List<int>();
    //protected float crowedRate = 0.5f;
    protected float detectRange = 0.1f;
    protected float entireHeight = 0.3f;
    protected float generalGap = 0.05f;
    protected float fixGap = 0f;
    protected int maxNormalLayerNum = 10;
    public Vector3 safeUpdatePos
    {
        get
        {
            return new Vector3(detectRange,0,detectRange);
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

    public HandleMetaphor(GameObject handleGo)
    {
        worldObject = handleGo;
        generalGap = entireHeight / maxNormalLayerNum;
        foreach (Transform t in handleGo.transform)
        {
            if (t.name == "Base")
            {
                baseObject = t.gameObject;
                Debug.Log("Base found");
            }
        }
    }

    public virtual void addLayer(LayerMetaphor layer)
    {
        if (layers.Contains(layer))
        {
            return;
        }
        try
        {
            GameObject layerObject = layer.getMetaphorObject();
            layerObject.transform.parent = baseObject.transform.parent;
            layerObject.transform.rotation = new Quaternion();
        }
        catch
        {
            Debug.Log("Fail to set layer parent");
            return;
        }
        layer.setHandle(this);
        layers.Add(layer);
        if (!_oldVersion)
        {
            if (layers.Count > maxNormalLayerNum)
            {
                generalGap = entireHeight / layers.Count;
            }
        }
    }

    public virtual void removeLayer(LayerMetaphor layer)
    {
        if (!layers.Contains(layer))
        {
            return;
        }
        try
        {
            if (_oldVersion)
            {
                layer.reScaleOrginal();
            }
            GameObject layerObject = layer.getMetaphorObject();
            if (layer.Handle == this)
            {
                layerObject.transform.parent = worldObject.transform.parent;
                layer.setHandle(null);
            }
        }
        catch
        {
            Debug.Log("Fail to set layer parent");
            return;
        }
        layers.Remove(layer);
        if (!_oldVersion)
        {
            if (layers.Count > maxNormalLayerNum)
            {
                generalGap = entireHeight / layers.Count;
            }
        }
    }

    public virtual void updateIndex(Vector3 handPos)
    {
        if (layers.Count == 0)
        {
            return;
        }
        Vector3 handlePos = getMetaphorObject().transform.position;
        Vector3 firstLayerPos = postionForLayerInIndex(0);
        Vector3 lastLayerPos = postionForLayerInIndex(layers.Count - 1);
        float distance = Vector2.Distance(new Vector2(handPos.x, handPos.z), new Vector2(handlePos.x, handlePos.z));
        bool inRange = (distance <= detectRange) && ((handPos.y - lastLayerPos.y) <= (detectRange + fixGap)) && ((firstLayerPos.y - handPos.y) <= (detectRange + fixGap));
        if (!inRange)
        {
            fixGap = 0;
            layers[centerIndex].cancelHighLightText();
            centerIndex = 0;
            return;
        }
        else
        {
            fixGap = Mathf.Sqrt((detectRange - distance) / detectRange) * detectRange * 0.6f;
        }
        //centerAroundIndexs.Clear();
        centerIndex = 0;
        float gap = Mathf.Abs(postionForLayerInIndex(0).y - handPos.y);
        if (_oldVersion)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                float gap_i = Mathf.Abs(postionForLayerInIndex(i).y - handPos.y);
                if (layers[i].isGrasped)
                {
                    gap_i = 0;
                }
                if (gap > gap_i)
                {
                    centerIndex = i;
                    gap = gap_i;
                }
            }
        }
        else
        {
            centerIndex = Mathf.RoundToInt((handPos.y - firstLayerPos.y) / (lastLayerPos.y - firstLayerPos.y) * (layers.Count - 1));
        }
        if (centerIndex < 0)
        {
            centerIndex = 0;
        }
        else if (centerIndex > layers.Count - 1)
        {
            centerIndex = layers.Count - 1;
        }
        LayerMetaphor toEnableLm = layers[centerIndex];
        Color c = Color.black;
        foreach (LayerMetaphor lm in layers)
        {
            if (!lm.isGrasped)
            {
                lm.disableGraspMovement();
                lm.disableGraspActions();
                lm.cancelHighLightText();
            }
            else
            {
                toEnableLm = lm;
                c = Color.red;
            }
        }
        toEnableLm.enableGraspMovement();
        toEnableLm.enableActivedGraspActions();
        toEnableLm.highLightText(c);

        //Debug.Log("bot:" + centerBotIndex + "center" + centerIndex + "top" + centerTopIndex);
    }

    public void updateObjectsPositions(Vector3 handPos)
    {
        //Debug.Log("count"+handPos);
        int i = 0;
        foreach (LayerMetaphor layer in layers){
            if (!layer.IsPinched && !layer.isGrasped)
            {
                layer.moveObjectTo(postionForLayerInIndex(i));
            }
            if (_oldVersion)
            {
                Vector3 handlePos = getMetaphorObject().transform.position;
                Vector3 firstLayerPos = postionForLayerInIndex(0);
                Vector3 lastLayerPos = postionForLayerInIndex(layers.Count - 1);
                float distance = Vector2.Distance(new Vector2(handPos.x, handPos.z), new Vector2(handlePos.x, handlePos.z));
                bool inRange = (distance <= detectRange) && ((handPos.y - lastLayerPos.y) <= (detectRange + fixGap)) && ((firstLayerPos.y - handPos.y) <= (detectRange + fixGap));

                if (i == centerIndex && inRange)
                {
                    layer.reScaleByRate(1f);
                }
                else
                {
                    layer.reScaleByRate(.5f);
                }
            }
            else
            {
                float distanceRate = distanceRateForLayer(layer, handPos);
                float rate = rateForDistance(distanceRate);
                layer.reScaleByRate(rate);
            }
            i++;
        }
    }

    protected float distanceRateForLayer(LayerMetaphor lm, Vector3 handPos)
    {
        float distance = lm.distanceToCenter(handPos);
        float distanceRate = distance / detectRange;
        if (distanceRate < 0)
        {
            distanceRate = 0;
        }
        else if (distanceRate > 1)
        {
            distanceRate = 1;
        }
        return distanceRate;
    }

    protected float rateForDistance(float distanceRate)
    {
        float rate = 1;
        float minRate = .5f;
        float sqt5Minus1 = Mathf.Sqrt(5) - 1;
        rate = (1 / (distanceRate + (sqt5Minus1) / 2) - (2 / sqt5Minus1)) * (2 - minRate) + 2;
        
        return rate;
    }

    protected virtual Vector3 postionForLayerInIndex(int i)
    {
        Vector3 newPos = new Vector3();
        newPos = baseObject.transform.position;
        newPos += Vector3.up * generalGap * (i + 1);
        if (_oldVersion)
        {
            if (i < centerIndex - 1)
            {

            }
            else if(i>= centerIndex-1 && i <= centerIndex + 1)
            {
                newPos += Vector3.up * generalGap * (i - centerIndex + 1) * .5f;
            }
            else
            {
                newPos += Vector3.up * generalGap * 2 * .5f;
            }
        }
        else
        {
            newPos += Vector3.up * fixGap;
            newPos += worldObject.transform.forward * generalGap * (i - centerIndex).ForceNotNegative() * .6f;
            if (i < centerIndex)
            {
                newPos -= Vector3.up * fixGap;
            }
            else if (i > centerIndex)
            {
                newPos += Vector3.up * fixGap;
            }
        }
        return newPos;
    }

    public virtual bool updateLayerExisted(LayerMetaphor lm)
    {
        Vector3 lmPos = lm.getMetaphorObject().transform.position;
        Vector3 handlePos = getMetaphorObject().transform.position;
        float distance = Vector2.Distance(new Vector2(lmPos.x, lmPos.z), new Vector2(handlePos.x, handlePos.z));
        //bool inRange = (distance <= pinchingRange * 2);
        bool inRange = (distance <= baseObject.transform.GetComponent<CapsuleCollider>().radius * baseObject.transform.lossyScale.x);
        if (layers.Contains(lm))
        {
            if (!inRange)
            {
                removeLayer(lm);
            }
        }
        else
        {
            if (inRange)
            {
                addLayer(lm);
            }
        }
        return inRange;
    }

    public virtual void updateObjectsOrder()
    {
        List<LayerMetaphor> newLayers = new List<LayerMetaphor>();
        for(int i = 0; i < layers.Count-1
            ; i++)
        {
            for(int j = i+1; j < layers.Count; j++)
            {
                if(layers[j].getMetaphorObject().transform.position.y < layers[i].getMetaphorObject().transform.position.y)
                {
                    LayerMetaphor temp = layers[j];
                    layers[j] = layers[i];
                    layers[i] = temp;
                    if (centerIndex == i)
                    {
                        centerIndex = j;
                    }else if (centerIndex == j)
                    {
                        centerIndex = i;
                    }
                }
            }
        }
    }

    public void setOnGraspBeginActionForAllLayers(System.Action a)
    {
        foreach (LayerMetaphor lm in layers)
        {
            lm.setOnGraspBeginAction(a);
        }
    }

    public void setOnGraspStayActionForAllLayers(System.Action a)
    {
        foreach (LayerMetaphor lm in layers)
        {
            lm.setOnGraspStayAction(a);
        }
    }

    public void setOnGraspEndActionForAllLayers(System.Action a)
    {
        foreach (LayerMetaphor lm in layers)
        {
            lm.setOnGraspEndAction(a);
        }
    }

    public override float distanceToCenter(Vector3 pinchingPos)
    {
        return base.distanceToCenter(pinchingPos);
    }
}
