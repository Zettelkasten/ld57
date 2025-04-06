using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public int lightUpgradeLevel = 0;
    public Upgrade lightUpgrade;

    public void IncreaseLightsLevel()
    {
        if (lightUpgradeLevel >= lightUpgrade.levelCosts.Count)
        {
            return; // max level, no upgrade
        }
        lightUpgradeLevel++;
        lightUpgrade.Raise(); // Tell all listeners that the light upgrade changed!
        Debug.Log("Lights upgraded!");
    }
    
}
