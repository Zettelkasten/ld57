using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum DialogueTrigger
{
    PlayWhenItemIsFound = 1,
    PlayWhenItemIsSold = 2,
}

public class AttachedDialogue : MonoBehaviour
{
    public List<string> dialogue;

    private bool playing = false;
    private int currentLine = 0;
    private float currentLineProgress = 0;
    
    public DialogueTrigger trigger;
    
    public void PlayDialogue()
    {
        playing = true;
        World.Instance.DialogueUI.SetActive(true);
        World.Instance.ShowDialogueText("");
    }

    void Update()
    {
        if (!playing)
        {
            return;
        }
        if (currentLine >= dialogue.Count)
        {
            playing = false;
            World.Instance.DialogueUI.SetActive(false);
            return;
        }

        var continueInput = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) ||
                            Input.GetKeyDown(KeyCode.KeypadEnter);
        currentLineProgress += Time.deltaTime;
        var charsShown = (int)(currentLineProgress / World.Instance.timePerDialogueChar);
        if (charsShown >= dialogue[currentLine].Length)
        {
            World.Instance.ShowDialogueText(dialogue[currentLine]);
            if (continueInput)
            {
                currentLine++;
                currentLineProgress = 0;
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
