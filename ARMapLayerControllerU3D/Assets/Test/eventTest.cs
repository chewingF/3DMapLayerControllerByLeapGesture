using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eventTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void colorRed()
    {
        transform.GetComponent<MeshRenderer>().material.color = Color.red;
    }
    public void colorYellow()
    {
        transform.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }
    public void colorBlue()
    {
        transform.GetComponent<MeshRenderer>().material.color = Color.blue;
    }
    public void colorWhite()
    {
        transform.GetComponent<MeshRenderer>().material.color = Color.white;
    }
    public void colorTo(Color c)
    {
        transform.GetComponent<MeshRenderer>().material.color = c;
    }

    public void transparent50()
    {
        Color c = transform.GetComponent<MeshRenderer>().material.color;
        c.a = .5f;
        transform.GetComponent<MeshRenderer>().material.color = c;
    }

    public void transparent100()
    {
        Color c = transform.GetComponent<MeshRenderer>().material.color;
        c.a = 1f;
        transform.GetComponent<MeshRenderer>().material.color = c;
    }
}
