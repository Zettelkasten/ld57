using UnityEngine;
using UnityEngine.Events;

public class UpgradeEventListener : MonoBehaviour
{
    public Upgrade upgradeEvent;

    public UnityEvent response;

	private void OnEnable()
	{
		upgradeEvent.RegisterListener(this);
	}

	private void OnDisable()
	{
		upgradeEvent.UnregisterListener(this);
	}

	public void OnEventRaised()
	{
		response.Invoke();
	}
}
