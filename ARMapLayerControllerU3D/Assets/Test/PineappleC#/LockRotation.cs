using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;
using UnityEngine.Events;

public class LockRotation : MonoBehaviour
{
    protected InteractionBehaviour _intObj;

    protected void Start()
    {
        _intObj = GetComponent<InteractionBehaviour>();
        _intObj.OnGraspedMovement += onGraspedMovement;
    }

    protected void Update()
    {
        //transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);
    }


    protected void onGraspedMovement(Vector3 presolvePos, Quaternion presolveRot,
                                   Vector3 solvedPos, Quaternion solvedRot,
                                   List<InteractionController> controllers)
    {
        try
        {
            _intObj.rigidbody.position = solvedPos;
            _intObj.rigidbody.rotation = presolveRot;
        }
        catch
        {

        }
    }
}
