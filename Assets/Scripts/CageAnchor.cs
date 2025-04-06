using TMPro;
using UnityEngine;

public class CageAnchor : MonoBehaviour
{
    public Transform pivot;
    public Collider2D myCollider;

    public void Update()
    {
        // if player is in the collider and presses "E", reposition the ship
        if (myCollider.OverlapPoint(World.Instance.player.transform.position) && Input.GetKeyDown(KeyCode.E))
        {
            if (World.Instance.currentAnchor != this || World.Instance.cage.state == CageState.OnShip)
            {
                World.Instance.currentAnchor = this;
                World.Instance.RepositionShip();
                World.Instance.cage.SetState(CageState.SinkingWithoutPlayer);
            }
            else
            {
                // move up, as normal
                World.Instance.cage.SetState(CageState.Rising);
            }
        }
    }
}
