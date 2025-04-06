using UnityEngine;
using UnityEngine.UI;

public class UpgradeLevelSelectButton : MonoBehaviour
{
    public UpgradeCard myUpgradeCard;
    public int upgradeLevel = 0;
    public Button selectionButton;
    public Image selectionBorder;

    public void SelectThisLevel()
    {
        myUpgradeCard.SelectCertainLevel(upgradeLevel);
    }
}
