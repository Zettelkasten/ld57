using UnityEngine;

public enum CageState
{
    OnShip = 0,
    Sinking = 1,
    Underwater = 2,
    Rising = 3
}
public class Cage : MonoBehaviour
{
    public CageState state;
    public Transform playerPivot;

    void Start()
    {
        state = CageState.OnShip;
        World.Instance.player.transform.position = playerPivot.position;
    }
}
