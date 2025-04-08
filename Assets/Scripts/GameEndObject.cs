using UnityEngine;

public class GameEndObject : MonoBehaviour
{
    public AttachedDialogue endGameDialogue;
    
    void Update()
    {
        // check if below sea level
        var waterLevelY = World.Instance.aboveSea.waterSplashParticles.transform.position.y;
        if (transform.position.y < waterLevelY)
        {
            // trigger game end
            endGameDialogue.PlayDialogue();
        }
    }
}
