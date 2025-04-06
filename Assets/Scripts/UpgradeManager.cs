using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UpgradeManager : MonoBehaviour
{
	// Singleton instance.
	public static UpgradeManager Instance = null;

	public Upgrade[] knownUpgrades = new Upgrade[0];

	private Dictionary<string, int> upgradeLevelsOfKnownUpgrades = new Dictionary<string, int>();
	private Dictionary<string, int> selectedLevelsOfKnownUpgrades = new Dictionary<string, int>();

	// Initialize the singleton instance.
	private void Awake()
	{
		// If there is not already an instance of SoundManager, set it to this.
		if (Instance == null)
		{
			Instance = this;
		}
		//If an instance already exists, destroy whatever this object is to enforce the singleton.
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}

	public void Start()
	{
        upgradeLevelsOfKnownUpgrades.Clear();
        selectedLevelsOfKnownUpgrades.Clear();
		foreach (Upgrade currUpgrade in knownUpgrades)
        {
            upgradeLevelsOfKnownUpgrades.Add(currUpgrade.upgradeID, 0);
            selectedLevelsOfKnownUpgrades.Add(currUpgrade.upgradeID, 0);
        }
	}

    public int GetUpgradeLevelOfUpgrade(Upgrade upgrade)
    {
        return upgradeLevelsOfKnownUpgrades[upgrade.upgradeID];
    }

    public int GetSelectedLevelOfUpgrade(Upgrade upgrade)
    {
        return selectedLevelsOfKnownUpgrades[upgrade.upgradeID];
    }

    public int GetNextCostOfUpgrade(Upgrade upgrade)
    {
		string upgradeID = upgrade.upgradeID;
        int currentLevel = upgradeLevelsOfKnownUpgrades[upgradeID];
        if (currentLevel >= upgrade.levelCosts.Count)
        {
            return -1; // negative costs for finished upgrades
        }
        return upgrade.levelCosts[currentLevel];
	}

	public void UpgradeAnUpgrade(Upgrade targetUpgrade)
    {
        string upgradeID = targetUpgrade.upgradeID;
        if (upgradeLevelsOfKnownUpgrades.ContainsKey(upgradeID))
        {
            int currentLevel = upgradeLevelsOfKnownUpgrades[upgradeID];
            if (currentLevel < targetUpgrade.levelCosts.Count)
            {
                upgradeLevelsOfKnownUpgrades[upgradeID]++;
                selectedLevelsOfKnownUpgrades[upgradeID] = upgradeLevelsOfKnownUpgrades[upgradeID];
				targetUpgrade.Raise(); // Tell all listeners that an upgrade happened!
				Debug.Log("Upgraded: " + upgradeID);
			}
		}
        else
        {
            Debug.Log("Tried to upgrade a missing upgrade!");
        }        
    }
    
    public void SelectAnUpgradeLevel(Upgrade targetUpgrade, int targetLevel)
    {
		string upgradeID = targetUpgrade.upgradeID;
		if (upgradeLevelsOfKnownUpgrades.ContainsKey(upgradeID))
		{
			int currentMaxLevel = upgradeLevelsOfKnownUpgrades[upgradeID];
            int currentSelection = selectedLevelsOfKnownUpgrades[upgradeID];
            if (targetLevel == currentSelection)
            {
                return; // nothing happens
            }
            if (targetLevel > currentMaxLevel)
            {
                Debug.Log("Tried to select an upgrade not unlocked yet!");
                return;
            }
            selectedLevelsOfKnownUpgrades[upgradeID] = targetLevel;
            targetUpgrade.Raise();
			Debug.Log("Selection changed: " + upgradeID);
		}
		else
		{
			Debug.Log("Tried to upgrade a missing upgrade!");
		}
	}

}
