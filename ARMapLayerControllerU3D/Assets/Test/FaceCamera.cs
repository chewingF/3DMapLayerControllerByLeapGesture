using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {
    public Transform userPosGo;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (userPosGo == null)
        {
            userPosGo = Camera.main.transform;
        }
        Vector3 camPos = Camera.main.transform.position;
        Vector3 selfPos = transform.position;
        //Vector3 fixPos = new Vector3(camPos.x, selfPos.y, camPos.z);
        Vector3 fixPos = new Vector3(selfPos.x - camPos.x, 0, selfPos.z - camPos.z);
        //Debug.Log(fixPos);
        transform.rotation = Quaternion.LookRotation(fixPos);
    }
}
