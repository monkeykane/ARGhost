using UnityEngine;
using System.Collections;

public class DragonCameraController : MonoBehaviour
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Quaternion gyro = Input.gyro.attitude;
        gyro.y *= -1;
        transform.rotation = gyro;
	}
}
