using UnityEngine;

public class AboveSea : MonoBehaviour
{
    public Transform shipRopePivotLeft;
    public Transform shipRopePivotRight;

    public Collider2D shipCollider;
    public Collider2D cageEnterCollider;
    
    public void Update()
    {
        // if player is in the cage enter collider and presses "E", set the cage state to Sinking
        if (cageEnterCollider.OverlapPoint(World.Instance.player.transform.position) && Input.GetKeyDown(KeyCode.E))
        {
            if (World.Instance.cage.state == CageState.OnShip)
            {
                World.Instance.cage.SetState(CageState.Sinking);
            }
            else
            {
                // move up, something is wrong
                World.Instance.cage.SetState(CageState.Rising);
            }
        }
    }

}
