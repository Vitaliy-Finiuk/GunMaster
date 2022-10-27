using UnityEngine;
using System.Collections;

public class PrLookToCamera : MonoBehaviour {

 
	void Update () {
        //transform.LookAt(Camera.main.transform);
        if (Camera.main != null)
            transform.rotation = Camera.main.transform.rotation;
       // transform.localRotation = Quaternion.Euler(transform.localEulerAngles + new Vector3(0, 180, 0));
	}
}
