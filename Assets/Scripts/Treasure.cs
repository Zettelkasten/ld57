using UnityEngine;

public class Treasure : MonoBehaviour
{
    public int value;

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
}
