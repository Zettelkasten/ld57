using UnityEngine;

public class Treasure : MonoBehaviour
{
    public int value;
    
    public void Sell()
    {
        World.Instance.player.money += value;
        Destroy(gameObject);
    }
}
