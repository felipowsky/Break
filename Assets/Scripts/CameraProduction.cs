using UnityEngine;
using System.Collections;

public class CameraProduction : MonoBehaviour {

	void Start () {
		Camera camera = GetComponent<Camera>();
		camera.cullingMask ^= 1 <<  LayerMask.NameToLayer("Debug");
	}

}
