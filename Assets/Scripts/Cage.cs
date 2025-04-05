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
    public Transform ropePivot;

    public LineRenderer ropeLineRenderer;
    
    // sinking up or raising down
    private float sinkProgress;
    public float sinkSpeed;

    void Start()
    {
        state = CageState.OnShip;
        World.Instance.player.transform.position = playerPivot.position;
    }
    
    public void SetState(CageState newState)
    {
        state = newState;
        switch (state)
        {
            case CageState.Sinking:
            case CageState.Rising:
                // make the player a child of the cage
                World.Instance.player.transform.SetParent(this.transform);
                sinkProgress = 0;
                break;
        }
    }

    public void Update()
    {
        // update the line renderer
        ropeLineRenderer.SetPosition(0, ropePivot.position);
        ropeLineRenderer.SetPosition(1, World.Instance.aboveSea.shipRopePivot.position);
        
        switch (state)
        {
            case CageState.Sinking:
            case CageState.Rising:
                var fromPos = World.Instance.currentAnchor.pivot.position + World.Instance.aboveSeaOffset;
                var toPos = World.Instance.currentAnchor.pivot.position;
                if (state == CageState.Rising)
                {
                    (fromPos, toPos) = (toPos, fromPos);
                }

                sinkProgress += Time.deltaTime * sinkSpeed;
                this.transform.position = Vector3.Lerp(fromPos, toPos, Helpers.EaseInOutQuad(sinkProgress));
                if (sinkProgress > 1)
                {
                    SetState(state == CageState.Sinking ? CageState.Underwater : CageState.OnShip);
                    // make the player a child of the game scene
                    World.Instance.player.transform.SetParent(null);
                }
                break;
        }
    }
}
