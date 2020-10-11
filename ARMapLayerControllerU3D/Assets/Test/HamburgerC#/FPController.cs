using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPController : MonoBehaviour
{
    public float lookY;
    public float Speed = 3f;
    public Camera cam;
    bool activated=false;

    void Start()
    {

    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            activated = !activated;
        }
        if(activated)
        {
            lookY = Input.GetAxis("Mouse X") * Speed;

            Vector3 rot = new Vector3(0, lookY, 0);
            cam.transform.Rotate(rot);
        }
    }
}
