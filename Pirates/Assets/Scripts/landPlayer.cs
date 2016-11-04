using UnityEngine;
using System.Collections;

public class landPlayer : MonoBehaviour {

	public Sprite upSprite;
	public Sprite leftSprite;
	public Sprite rightSprite;
	public Sprite downSprite;

	private SpriteRenderer spriteRenderer;

	public Rigidbody2D rb;
	public float runSpeed;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
				spriteRenderer.sprite = upSprite;
				//display up sprite
		}
		else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
			spriteRenderer.sprite = leftSprite;
			//display left sprite
		}
		else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
				spriteRenderer.sprite = downSprite;
				//display down sprite
		}
		else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
			spriteRenderer.sprite = rightSprite;
			//display right sprite
		}
		var x = Input.GetAxis("Horizontal") * Time.deltaTime * runSpeed;
		var y = Input.GetAxis("Vertical") * Time.deltaTime * runSpeed;
		transform.Translate(x, y, 0);
	}
}
