using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public Transform block;
	public int columns = 5;
	public int rows = 5;

	private Block[,] matrix;
	private Color[] colors = new Color[] {Color.red, Color.gray, Color.green};

	void Start () {
		matrix = new Block[columns,rows];

		for (int i = 0; i < columns; i++) {
			for (int j = 0; j < rows; j++) {
				Block properties = new Block();

				Color color = colors[Random.Range(0, colors.Length)];

				properties.color = color;

				matrix[i,j] = properties;

				Transform newBlock = Instantiate(block);
				newBlock.GetComponent<SpriteRenderer>().color = color;	
			}
		}
	}
	
	void Update () {
		if (Input.GetButtonDown("Jump")) {
			Instantiate(block);
		}
	}
}
