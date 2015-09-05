using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public Transform blockPrefab;
	public Transform startPointMatrix;

	public Vector2 scaleSize = new Vector2(1, 1);

	public int rows = 5;
	public int columns = 5;

	public bool canInteract = true;

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
				Transform newBlock = CreateBlock(row, column);
				matrix[row, column] = newBlock;
			}
		}
	}
	
	void Update () {
	}

	Transform CreateBlock (int row, int column) {

		int colorIndex = Random.Range(0, colors.Length);
		
		Color color = colors[colorIndex];

		Vector2 position = PositionByIndex(row, column);

		Transform newBlock = Instantiate(blockPrefab, position, Quaternion.identity) as Transform;
		Block properties = newBlock.GetComponent<Block>();
		
		properties.matrixIndex = new Vector2(row, column);
		properties.colorIndex = colorIndex;
		properties.color = color;
		properties.scaleSize = scaleSize;

		newBlock.GetComponent<SpriteRenderer>().color = color;

		return newBlock;
	}

	public void BreakBlocks (Vector2 origin) {

		canInteract = false;

		try {
			int minimumCount = 3;

			HashSet<Transform> blocksToBreak = new HashSet<Transform>();

			Block blockOrigin = matrix[(int) origin.x, (int) origin.y].GetComponent<Block>();

			BreakBlockAndAdjacents(origin, blockOrigin.colorIndex, ref blocksToBreak);

			if (blocksToBreak.Count >= minimumCount) {
				foreach (Transform blockTransform in blocksToBreak) {
					Block block = blockTransform.GetComponent<Block>();

					matrix[(int) block.matrixIndex.x, (int) block.matrixIndex.y] = null;

					Destroy(blockTransform.gameObject);
				}

				List<Vector2> emptySpaces = new List<Vector2>();

				RearrangeMatrix(ref emptySpaces);

				if (emptySpaces.Count > 0) {
					foreach (Vector2 index in emptySpaces) {
						int row = (int) index.x;
						int column = (int) index.y;

						Transform newBlock = CreateBlock(row, column);

						newBlock.GetComponent<Block>().ShowAnimated();

						matrix[row, column] = newBlock;
					}
				}
			}
		} finally {
			canInteract = true;
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

		if (blockTransform == null) {
			return;
		}

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

	void RearrangeMatrix (ref List<Vector2> emptySpaces) {
		for (int row = rows - 1; row >= 0; row--) {
			for (int column = columns - 1; column >= 0; column--) {
				Transform block = matrix[row, column];

				if (block == null) {
					Transform replaceBlock = null;

					for (int i = row - 1; i >= 0 && replaceBlock == null; i--) {
						Transform candidate = matrix[i, column];

						if (candidate != null) {
							replaceBlock = candidate;
						}
					}

					if (replaceBlock != null) {
						Block replaceProperties = replaceBlock.GetComponent<Block>();
						matrix[(int) replaceProperties.matrixIndex.x, (int) replaceProperties.matrixIndex.y] = null;
						replaceProperties.matrixIndex = new Vector2(row, column);
						matrix[row, column] = replaceBlock;

						replaceBlock.position = PositionByIndex(row, column);
					
					} else if (emptySpaces != null) {
						emptySpaces.Add(new Vector2(row, column));
					}
				}
			}
		}
	}

	Vector2 PositionByIndex (int row, int column) {
		return new Vector2(startPointMatrix.position.x + (blockSize.x * column),
		                   startPointMatrix.position.y - (blockSize.y * row));
	}
}
