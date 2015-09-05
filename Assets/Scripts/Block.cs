using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	public Vector2 matrixIndex;
	public int colorIndex;
	public Color color;

	void Start () {
	}

	void Update () {
	}

	void OnMouseDown () {
		GameManager.instance.BreakBlocks(matrixIndex);
	}

}
