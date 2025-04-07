using UnityEngine;

public class Treasure : MonoBehaviour
{
    public int value;

    private bool movingToSellBox = false;
    private float moveTimer = 0;
    private float moveEndTime = 0;

    private Rigidbody2D myRB;
    private Collider2D myCollider;
    private SpriteRenderer mySpriteRenderer;

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
                // Tell the 
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
        Destroy(gameObject);
    }

    public void MoveToSellBoxAndPlayCoolPartivleAnimation(float timeGiven)
    {
        movingToSellBox = true;
        moveEndTime = timeGiven;
        moveTimer = 0;
        myCollider.enabled = false;
        myRB.bodyType = RigidbodyType2D.Kinematic; // I hope this works
        mySpriteRenderer.sortingLayerName = "Foreground";
        mySpriteRenderer.sortingOrder = 10; // infront of everything!!!!
        gameObject.layer = 5; // set this to UI layer
    }
}
