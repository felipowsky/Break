using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public Transform blockPrefab;
	public int rows = 5;
	public int columns = 5;

	public Vector2 scaleSize = new Vector2(1, 1);

	public Transform startPointMatrix;

	private Transform[,] matrix;
	private Color[] colors = new Color[] {Color.red, Color.green, Color.blue, Color.cyan};
	private Vector2 blockSize;

	void Awake () {
		instance = this;
	}

	void Start () {
		blockPrefab.localScale = scaleSize;
		blockSize = blockPrefab.GetComponent<SpriteRenderer>().bounds.size;

		matrix = new Transform[rows,columns];

		for (int row = 0; row < rows; row++) {
			for (int column = 0; column < columns; column++) {

				int colorIndex = Random.Range(0, colors.Length);

				Color color = colors[colorIndex];

				Vector2 position = new Vector2(startPointMatrix.position.x + (blockSize.x * column),
				                               startPointMatrix.position.y - (blockSize.y * row));

				Transform newBlock = Instantiate(blockPrefab, position, Quaternion.identity) as Transform;
				Block properties = newBlock.GetComponent<Block>();

				properties.matrixIndex = new Vector2(row, column);
				properties.colorIndex = colorIndex;
				properties.color = color;

				newBlock.GetComponent<SpriteRenderer>().color = color;

				matrix[row, column] = newBlock;
			}
		}
	}
	
	void Update () {
	}

	public void BreakBlocks (Vector2 origin) {
		int minimumCount = 3;
		HashSet<Transform> blocksToBreak = new HashSet<Transform>();

		Block blockOrigin = matrix[(int) origin.x, (int) origin.y].GetComponent<Block>();

		BreakBlockAndAdjacents(origin, blockOrigin.colorIndex, ref blocksToBreak);

		if (blocksToBreak.Count >= minimumCount) {
			foreach (Transform block in blocksToBreak) {
				Destroy(block.gameObject);
			}
		}
	}

	void BreakBlockAndAdjacents (Vector2 index, int colorIndex, ref HashSet<Transform> blocksToBreak) {
		if (index.x < 0) {
			return;
		}

		if (index.y < 0) {
			return;
		}

		if (index.x >= rows) {
			return;
		}

		if (index.y >= columns) {
			return;
		}

		Transform blockTransform = matrix[(int) index.x, (int) index.y];

		if (blocksToBreak.Contains(blockTransform)) {
			return;
		}

		Block block = blockTransform.GetComponent<Block>();

		if (block.colorIndex != colorIndex) {
			return;
		}

		blocksToBreak.Add(blockTransform);

		BreakBlockAndAdjacents(new Vector2(index.x - 1, index.y), colorIndex, ref blocksToBreak);
		BreakBlockAndAdjacents(new Vector2(index.x + 1, index.y), colorIndex, ref blocksToBreak);
		BreakBlockAndAdjacents(new Vector2(index.x, index.y - 1), colorIndex, ref blocksToBreak);
		BreakBlockAndAdjacents(new Vector2(index.x, index.y + 1), colorIndex, ref blocksToBreak);
	}
}
