using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeCard : MonoBehaviour
{
    public Upgrade myUpgrade;

	public Button upgradeButton;

	public Image displayImage;

	public TMPro.TextMeshProUGUI upgradeButtonText;

    private List<UpgradeLevelSelectButton> upgradeLevelSelectButtons = new List<UpgradeLevelSelectButton>();

	public GameObject upgradeSelectButtonPrefab;

	public Transform upgradeSelectButtonsContainer;
	
	public AttachedDialogue dialogueAfterFirstUpgrade;
	
	public bool shouldPlayDialogue = false;
	private bool alreadyPlayedDialogue = false;

	public void Start()
	{
		// spawn the correct number of upgrade select buttons
		upgradeLevelSelectButtons.Clear();
		for (int i = 0; i <= myUpgrade.levelCosts.Count; i++)
		{
			GameObject buttonInstance = Instantiate(upgradeSelectButtonPrefab, upgradeSelectButtonsContainer);
			UpgradeLevelSelectButton currLevelSelectButton = buttonInstance.GetComponent<UpgradeLevelSelectButton>();
			upgradeLevelSelectButtons.Add(currLevelSelectButton);
			currLevelSelectButton.myUpgradeCard = this;
			currLevelSelectButton.upgradeLevel = i;
		}
		// choose the correct display icon
		AdjustUpgradeIcon();
		
		dialogueAfterFirstUpgrade = GetComponent<AttachedDialogue>();
	}

	public void Update()
	{
		// upgrade button
		int currentUpgradeCost = UpgradeManager.Instance.GetNextCostOfUpgrade(myUpgrade);
		if (currentUpgradeCost < 0)
		{
			upgradeButton.interactable = false;
			upgradeButtonText.text = "MAX";
		}
		else
		{
			upgradeButtonText.text = "Cost: " + currentUpgradeCost.ToString();
			upgradeButton.interactable = (World.Instance.player.money >= currentUpgradeCost);
		}
		// upgrade select buttons
		int currentMaxLevel = UpgradeManager.Instance.GetUpgradeLevelOfUpgrade(myUpgrade);
		int currentSelectedLevel = UpgradeManager.Instance.GetSelectedLevelOfUpgrade(myUpgrade);
		foreach (UpgradeLevelSelectButton currButton in upgradeLevelSelectButtons)
		{ 
			int currLevel = currButton.upgradeLevel;
			currButton.selectionButton.interactable = (currLevel <= currentMaxLevel);
			currButton.selectionBorder.enabled = (currLevel == currentSelectedLevel);
		}
	}

	private void AdjustUpgradeIcon()
	{
		int currentSelectedLevel = UpgradeManager.Instance.GetSelectedLevelOfUpgrade(myUpgrade);
		if (currentSelectedLevel < myUpgrade.sprites.Count) 
		{
			displayImage.sprite = myUpgrade.sprites[currentSelectedLevel];
		}
	}

	public void DoUpgrade()
	{
		int currentUpgradeCost = UpgradeManager.Instance.GetNextCostOfUpgrade(myUpgrade);
		World.Instance.player.money -= currentUpgradeCost;
		UpgradeManager.Instance.UpgradeAnUpgrade(myUpgrade);
		AdjustUpgradeIcon();
		if (!alreadyPlayedDialogue)
		{
			shouldPlayDialogue = true;
			alreadyPlayedDialogue = true;
		}
	}

	public void SelectCertainLevel(int upgradeLevel)
	{
		UpgradeManager.Instance.SelectAnUpgradeLevel(myUpgrade, upgradeLevel);
		AdjustUpgradeIcon();
	}

}
