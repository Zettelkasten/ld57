using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]
public class Upgrade : ScriptableObject
{
	public List<int> levelCosts = new List<int> { 100, 200, 500 };

	public List<Sprite> sprites = new List<Sprite>();

    private List<UpgradeEventListener> listeners = new List<UpgradeEventListener>();

	public bool CheckIfEnoughMoneyForUpgrade(int money, int currentLevel)
	{
		if (currentLevel >= levelCosts.Count)
		{
			return true;
		}
		return levelCosts[currentLevel] <= money;
	}

	public void RegisterListener(UpgradeEventListener listener)
	{
		listeners.Add(listener);
	}

	public void UnregisterListener(UpgradeEventListener listener)
	{
		listeners.Remove(listener);
	}

	public void Raise()
	{
		for (int i = listeners.Count - 1; i >= 0; i--)
		{
			listeners[i].OnEventRaised();
		}
	}
}
