using UnityEngine;

public class AboveSea : MonoBehaviour
{
    public Transform shipRopePivotLeft;
    public Transform shipRopePivotRight;

    public Collider2D shipCollider;
    public Collider2D cageEnterCollider;
    
    public ParticleSystem waterSplashParticles;
    
    public void Update()
    {
        // if player is in the cage enter collider and presses "E", set the cage state to Sinking
        if (cageEnterCollider.OverlapPoint(World.Instance.player.transform.position) && World.Instance.BottomUIAvailable())
        {
            if (World.Instance.cage.state == CageState.OnShip)
            {
                World.Instance.ShowBottomText("Press [E] to start mission");
                if (Input.GetKeyDown(KeyCode.E) || World.Instance.CheckBottomButtonClicked())
                    World.Instance.cage.SetState(CageState.Sinking);
            }
            else if (World.Instance.cage.state == CageState.Underwater)
            {
                // move up, something is wrong
                World.Instance.ShowBottomText("Press [E] to recall cage");
                if (Input.GetKeyDown(KeyCode.E) || World.Instance.CheckBottomButtonClicked())
                    World.Instance.cage.SetState(CageState.Rising);
            }
        }
    }

}
