using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	public float speedAnimation = 10f;

	public Vector2 matrixIndex;
	public int colorIndex;
	public Color color;
	public Vector2 scaleSize;

	private bool animating = false;

	private SpriteRenderer spriteRenderer;

	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Start () {
		if (!animating) {
			transform.localScale = scaleSize;
		}
	}

	void Update () {
		if (animating) {
			Vector2 currentScale = transform.localScale;

			Vector2 newScale = new Vector2(Mathf.Lerp(currentScale.x, scaleSize.x, Time.deltaTime * speedAnimation),
			                               Mathf.Lerp(currentScale.y, scaleSize.y, Time.deltaTime * speedAnimation));

			transform.localScale = newScale;

			Color newColor = spriteRenderer.color;
			newColor.a = Mathf.Lerp(newColor.a, 1.0f, Time.deltaTime * speedAnimation);

			spriteRenderer.color = newColor;
		}
	}

	void OnMouseDown () {
		GameManager manager = GameManager.instance;

		if (!manager.canInteract) {
			return;
		}

		manager.BreakBlocks(matrixIndex);
	}

	public void ShowAnimated () {
		if (animating) {
			return;
		}

		transform.localScale = Vector2.zero;

		Color newColor = spriteRenderer.color;
		newColor.a = 0.0f;

		spriteRenderer.color = newColor;

		animating = true;
	}

}
