using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMetaphor : TouchableObject
{
    protected LayerMetaphor parentLayerMetaphor;

    public SelectionMetaphor(LayerMetaphor layerMetaphor, float distance, string selectionName)
    {
        parentLayerMetaphor = layerMetaphor;

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = selectionName.ToUpper();
        sphere.transform.localScale = new Vector3(.03f, .03f, .03f);
        MeshCollider ms = sphere.AddComponent<MeshCollider>();
        //ms.convex = true;
        GameObject.Destroy(sphere.GetComponent<SphereCollider>());
        Material newMaterial = (Material)Resources.Load("Selection_" + selectionName, typeof(Material));
        sphere.GetComponent<Renderer>().material = newMaterial;
        sphere.transform.parent = layerMetaphor.getMetaphorObject().transform;
        sphere.transform.localPosition = new Vector3(distance, 0, 0);
        worldObject = sphere;
    }

    public void rotateBy(float angle)
    {
        worldObject.transform.RotateAround(parentLayerMetaphor.getMetaphorObject().transform.position, Vector3.up, angle);
    }

    public void pushTo(float distance)
    {
        worldObject.transform.localPosition = new Vector3(distance, 0, 0);
    }

}