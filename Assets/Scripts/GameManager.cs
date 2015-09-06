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

	private Block[,] matrix;
	private Color[] colors = new Color[] {Color.red, Color.green, Color.blue, Color.cyan};
	private Vector2 blockSize;

	void Awake () {
		instance = this;
	}

	void Start () {
		blockPrefab.localScale = scaleSize;
		blockSize = blockPrefab.GetComponent<SpriteRenderer>().bounds.size;

		matrix = new Block[rows, columns];

		for (int row = 0; row < rows; row++) {
			for (int column = 0; column < columns; column++) {
				Block newBlock = CreateBlock(row, column);
				matrix[row, column] = newBlock;
			}
		}
	}
	
	void Update () {
	}

	Block CreateBlock (int row, int column) {

		int colorIndex = Random.Range(0, colors.Length);
		
		Color color = colors[colorIndex];

		Vector2 position = PositionByIndex(row, column);

		Transform newTransform = Instantiate(blockPrefab, position, Quaternion.identity) as Transform;
		Block newBlock = newTransform.GetComponent<Block>();
		
		newBlock.matrixIndex = new IntVector2(row, column);
		newBlock.colorIndex = colorIndex;
		newBlock.color = color;
		newBlock.scaleSize = scaleSize;
		newBlock.finalPosition = position;

		return newBlock;
	}

	public void BreakBlocks (IntVector2 origin) {

		canInteract = false;

		try {
			int minimumCount = 3;

			HashSet<Block> blocksToBreak = new HashSet<Block>();

			Block blockOrigin = matrix[origin.x, origin.y].GetComponent<Block>();

			BreakBlockAndAdjacents(origin, blockOrigin.colorIndex, ref blocksToBreak);

			if (blocksToBreak.Count >= minimumCount) {
				foreach (Block block in blocksToBreak) {
					matrix[block.matrixIndex.x, block.matrixIndex.y] = null;

					Destroy(block.gameObject);
				}

				List<IntVector2> emptySpaces = new List<IntVector2>();

				RearrangeMatrix(ref emptySpaces);

				if (emptySpaces.Count > 0) {
					foreach (IntVector2 index in emptySpaces) {
						Block newBlock = CreateBlock(index.x, index.y);

						newBlock.ShowAnimated();

						matrix[index.x, index.y] = newBlock;
					}
				}
			}
		} finally {
			canInteract = true;
		}
	}

	void BreakBlockAndAdjacents (IntVector2 index, int colorIndex, ref HashSet<Block> blocksToBreak) {
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

		Block block = matrix[index.x, index.y];

		if (block == null) {
			return;
		}

		if (blocksToBreak.Contains(block)) {
			return;
		}

		if (block.colorIndex != colorIndex) {
			return;
		}

		blocksToBreak.Add(block);

		BreakBlockAndAdjacents(new IntVector2(index.x - 1, index.y), colorIndex, ref blocksToBreak);
		BreakBlockAndAdjacents(new IntVector2(index.x + 1, index.y), colorIndex, ref blocksToBreak);
		BreakBlockAndAdjacents(new IntVector2(index.x, index.y - 1), colorIndex, ref blocksToBreak);
		BreakBlockAndAdjacents(new IntVector2(index.x, index.y + 1), colorIndex, ref blocksToBreak);
	}

	void RearrangeMatrix (ref List<IntVector2> emptySpaces) {
		for (int row = rows - 1; row >= 0; row--) {
			for (int column = columns - 1; column >= 0; column--) {
				Block block = matrix[row, column];

				if (block == null) {
					Block replaceBlock = null;

					for (int i = row - 1; i >= 0 && replaceBlock == null; i--) {
						Block candidate = matrix[i, column];

						if (candidate != null) {
							replaceBlock = candidate;
						}
					}

					if (replaceBlock != null) {
						matrix[replaceBlock.matrixIndex.x, replaceBlock.matrixIndex.y] = null;
						replaceBlock.matrixIndex = new IntVector2(row, column);
						matrix[row, column] = replaceBlock;

						replaceBlock.finalPosition = PositionByIndex(row, column);
					
					} else if (emptySpaces != null) {
						emptySpaces.Add(new IntVector2(row, column));
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
