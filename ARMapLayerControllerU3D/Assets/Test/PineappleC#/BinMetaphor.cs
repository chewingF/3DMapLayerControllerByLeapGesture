using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BinMetaphor:HandleMetaphor{

    public BinMetaphor(GameObject handleGo) : base(handleGo)
    {
    }

    public override void addLayer(LayerMetaphor layer)
    {
        base.addLayer(layer);
        //layer.rotateObjectBy(new Vector3(90,90,0));
    }

    public override void removeLayer(LayerMetaphor layer)
    {
        base.removeLayer(layer);
        //layer.rotateObjectBy(new Vector3(0, -90, 90));
    }

    override public void updateIndex(Vector3 handPos)
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
            fixGap = 0.5f * (detectRange - distance);
        }
        //centerAroundIndexs.Clear();
        centerIndex = Mathf.RoundToInt((handPos.y - firstLayerPos.y) / (lastLayerPos.y - firstLayerPos.y) * (layers.Count - 1));
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
        toEnableLm.enableRemovedGraspActions();
        toEnableLm.highLightText(c);

    }
    /*
    protected override Vector3 postionForLayerInIndex(int i)
    {
        Vector3 newPos = new Vector3();
        newPos = baseObject.transform.position;
        newPos += Vector3.right * generalGap * (i + 1);
        newPos += Vector3.right * fixGap;
        if (i < centerIndex)
        {
            newPos -= Vector3.right * fixGap;
        }
        else if (i > centerIndex)
        {
            newPos += Vector3.right * fixGap;
        }
        return newPos;
    }*/
    /*
    public override bool updateLayerExisted(LayerMetaphor lm)
    {
        Vector3 lmPos = lm.getMetaphorObject().transform.position;
        Vector3 handlePos = getMetaphorObject().transform.position;
        float distance = Vector2.Distance(new Vector2(lmPos.y, lmPos.z), new Vector2(handlePos.y, handlePos.z));
        //bool inRange = (distance <= pinchingRange * 2);
        bool inRange = (distance <= baseObject.transform.GetComponent<CapsuleCollider>().radius * baseObject.transform.lossyScale.y);
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
    */
    /*
    public override void updateObjectsOrder()
    {
        List<LayerMetaphor> newLayers = new List<LayerMetaphor>();
        for(int i = 0; i < layers.Count-1
            ; i++)
        {
            for(int j = i+1; j < layers.Count; j++)
            {
                if(layers[j].getMetaphorObject().transform.position.x < layers[i].getMetaphorObject().transform.position.x)
                {
                    LayerMetaphor temp = layers[j];
                    layers[j] = layers[i];
                    layers[i] = temp;
                }
            }
        }
    }*/
}
