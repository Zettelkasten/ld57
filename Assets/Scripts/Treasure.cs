using UnityEngine;

public class Treasure : MonoBehaviour
{
    public int value;
    public float displayScale = 1f;

    private bool movingToSellBox = false;
    [SerializeField] private float moveTimer = 0;
	[SerializeField] public float moveEndTime = 1;

    private Rigidbody2D myRB;
    private Collider2D myCollider;
    private SpriteRenderer mySpriteRenderer;
    private Vector3 startPos;
    private Quaternion startRot;
    private Vector3 startScale;

    public bool triggersGameEnd;

	public void Start()
	{
		myRB = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
	    mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

	public void Update()
	{
		if (movingToSellBox)
        {
            if (moveTimer >= moveEndTime)
            {
                movingToSellBox=false;
                // animation ended
                movingToSellBox = false;
				Debug.Log("Finished Move Anim");
			}
            else
            {
                moveTimer += Time.deltaTime;
                // rotate and move
                float t = Mathf.Clamp(moveTimer / moveEndTime, 0.0f, 1.0f);
                float smooth_t = t * t * t * (t * (6.0f * t - 15.0f) + 10.0f);
                Transform sellBox = World.Instance.sellBox.transform;
                transform.position = Vector3.Lerp(startPos, sellBox.position, smooth_t);
                transform.rotation = Quaternion.Lerp(startRot, Quaternion.identity, smooth_t);
                transform.localScale = Vector3.Lerp(startScale, displayScale * Vector3.one, smooth_t);
            }
        }
	}

	public void SellDialogue()
    {
        // check dialogues
        var attachedDialogues = GetComponents<AttachedDialogue>();
        foreach (var attachedDialogue in attachedDialogues)
        {
            if (attachedDialogue.trigger == DialogueTrigger.PlayWhenItemIsSold)
            {
                attachedDialogue.PlayDialogue();
            }
        }
    }

    public void Sell()
    {

        // get the money
        World.Instance.player.money += value;
		// cash particles
		if (!triggersGameEnd) {
			World.Instance.sellBox.MakeItRain(value);
		}
		Destroy(gameObject);
    }

    public void MoveToSellBoxAndPlayCoolParticleAnimation(float timeGiven)
    {
        Debug.Log("Starting Move Anim");
        movingToSellBox = true;
        moveEndTime = timeGiven;
        moveTimer = 0;
        myCollider.enabled = false;
		startPos = transform.position;
        startRot = transform.localRotation;
        startScale = transform.localScale;
        myRB.bodyType = RigidbodyType2D.Static; // I hope this works
        mySpriteRenderer.sortingLayerName = "Foreground";
        mySpriteRenderer.sortingOrder = 10; // infront of everything!!!!
        gameObject.layer = 5; // set this to UI layer
    }
}
