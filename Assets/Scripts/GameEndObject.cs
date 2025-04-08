using System;
using UnityEngine;

public class GameEndObject : MonoBehaviour
{
    private AttachedDialogue endGameDialogue;
    private bool dialoguePlayed = false;

    private void Start()
    {
        endGameDialogue = GetComponent<AttachedDialogue>();
    }

    void Update()
    {
        // check if below sea level
        var waterLevelY = World.Instance.aboveSea.waterSplashParticles.transform.position.y;
        if (transform.position.y < waterLevelY && !dialoguePlayed && !World.Instance.DialogueUI.activeSelf)
        {
            // play dialogue
            endGameDialogue.PlayDialogue();
            dialoguePlayed = true;
        }
    }
}
