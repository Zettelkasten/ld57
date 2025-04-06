using UnityEngine;

public class Treasure : MonoBehaviour
{
    public int value;

    private bool alreadyFound;

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

    public void Update()
    {
        // if the player comes close, it will cound as "found"
        if (alreadyFound)
        {
            return;
        }
        // distance to player
        var distance = Vector2.Distance(World.Instance.player.transform.position, transform.position);
        if (distance < World.Instance.discoverTreasureDistance)
        {
            alreadyFound = true;
            // if we have a AttachedDialogue with a trigger of PlayWhenItemIsFound, play it
            var attachedDialogues = GetComponents<AttachedDialogue>();
            foreach (var attachedDialogue in attachedDialogues)
            {
                if (attachedDialogue.trigger == DialogueTrigger.PlayWhenItemIsFound)
                {
                    attachedDialogue.PlayDialogue();
                }
            }
        }
    }
}
