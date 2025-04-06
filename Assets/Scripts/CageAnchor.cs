using TMPro;
using UnityEngine;

public class CageAnchor : MonoBehaviour
{
    public Transform pivot;
    public Collider2D myCollider;

    public void Update()
    {
        // if player is in the collider and presses "E", reposition the ship
        if (myCollider.OverlapPoint(World.Instance.player.transform.position))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                World.Instance.currentAnchor = this;
                World.Instance.RepositionShip();
                World.Instance.cage.SetState(CageState.SinkingWithoutPlayer);
            }
        }
    }
}
