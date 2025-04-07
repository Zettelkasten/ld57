using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum DialogueTrigger
{
    PlayWhenItemIsFound = 1,
    PlayWhenItemIsSold = 2,
    Special = 3,
}

public class AttachedDialogue : MonoBehaviour
{
    public List<string> dialogue;

    private bool playing = false;
    private int currentLine = 0;
    private float currentLineProgress = 0;
    
    public DialogueTrigger trigger;
    private bool alreadyFound;

    void Start()
    {
        alreadyFound = false;
    }
    
    public void PlayDialogue()
    {
        playing = true;
        currentLine = 0;
        currentLineProgress = 0;
        // if there is a : in the line, set currentLineProgress based on that
        if (currentLine < dialogue.Count)
        {
            if (dialogue[currentLine].Contains(":"))
            {
                var split = dialogue[currentLine].Split(':');
                currentLineProgress = World.Instance.timePerDialogueChar * split[0].Length;
            }
            else
            {
                currentLineProgress = 0;
            }
        }
        
        World.Instance.DialogueUI.SetActive(true);
        World.Instance.ShowDialogueText("");
    }

    void Update()
    {
        // if the player comes close, it will count as "found"
        if (!alreadyFound && trigger == DialogueTrigger.PlayWhenItemIsFound)
        {
            // distance to player
            var distance = Vector2.Distance(World.Instance.player.transform.position, transform.position);
            if (distance < World.Instance.discoverTreasureDistance)
            {
                alreadyFound = true;
                PlayDialogue();
            }
        }

        if (!playing)
        {
            return;
        }

        if (currentLine >= dialogue.Count)
        {
            playing = false;
            World.Instance.DialogueUI.SetActive(false);
            World.Instance.player.dontMoveTime = 0.5f;
            return;
        }

        var continueInput = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) ||
                            Input.GetKeyDown(KeyCode.Return) || World.Instance.CheckDialogueButtonClicked();
        currentLineProgress += Time.deltaTime;
        var charsShown = (int)(currentLineProgress / World.Instance.timePerDialogueChar);
        if (charsShown >= dialogue[currentLine].Length)
        {
            World.Instance.ShowDialogueText(dialogue[currentLine]);
            if (continueInput)
            {
                currentLine++;
                currentLineProgress = 0;
                // if there is a : in the line, set currentLineProgress based on that
                if (currentLine < dialogue.Count)
                {
                    if (dialogue[currentLine].Contains(":"))
                    {
                        var split = dialogue[currentLine].Split(':');
                        currentLineProgress = World.Instance.timePerDialogueChar * split[0].Length;
                    }
                    else
                    {
                        currentLineProgress = 0;
                    }
                }
            }
        }
        else
        {
            if (continueInput)
            {
                currentLineProgress = World.Instance.timePerDialogueChar * dialogue[currentLine].Length;
                charsShown = dialogue[currentLine].Length;
            }

            World.Instance.ShowDialogueText(dialogue[currentLine].Substring(0, charsShown));
        }
    }
}
