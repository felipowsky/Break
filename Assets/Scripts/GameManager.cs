using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public Transform block;
	public int rows = 5;
	public int columns = 5;

	public Vector2 scaleSize = new Vector2(1,1);

	public Transform startPointMatrix;

	private Block[,] matrix;
	private Color[] colors = new Color[] {Color.red, Color.green, Color.blue, Color.cyan};
	private Vector2 blockSize;

	void Start () {
		block.localScale = scaleSize;
		blockSize = block.GetComponent<SpriteRenderer>().bounds.size;

		matrix = new Block[rows,columns];

		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {
				Block properties = new Block();

				int colorIndex = Random.Range(0, colors.Length);

				Color color = colors[colorIndex];

				properties.colorIndex = colorIndex;
				properties.color = color;

				matrix[i,j] = properties;

				Vector2 position = new Vector2(startPointMatrix.position.x + (blockSize.x * j),
				                               startPointMatrix.position.y - (blockSize.y * i));

				Transform newBlock = Instantiate(block, position, Quaternion.identity) as Transform;

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
