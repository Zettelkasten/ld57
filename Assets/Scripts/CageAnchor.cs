using TMPro;
using UnityEngine;

public class CageAnchor : MonoBehaviour
{
    public Transform pivot;
    public Collider2D myCollider;

    public void Update()
    {
        // if player is in the collider and presses "E", reposition the ship
        if (myCollider.OverlapPoint(World.Instance.player.transform.position) && World.Instance.BottomUIAvailable())
        {
            if (World.Instance.currentAnchor != this || World.Instance.cage.state == CageState.OnShip)
            {
                World.Instance.ShowBottomText("Press [E] to call cage");
                if (Input.GetKeyDown(KeyCode.E) || World.Instance.CheckBottomButtonClicked())
                {
                    World.Instance.currentAnchor = this;
                    World.Instance.RepositionShip();
                    World.Instance.cage.SetState(CageState.SinkingWithoutPlayer);
                }
            }
            else if (World.Instance.cage.state == CageState.Underwater)
            {
                // move up, as normal
                World.Instance.ShowBottomText("Press [E] to return to ship");
                if (Input.GetKeyDown(KeyCode.E) || World.Instance.CheckBottomButtonClicked())
                {
                    World.Instance.cage.SetState(CageState.Rising);
                }
            }
        }
    }
}
