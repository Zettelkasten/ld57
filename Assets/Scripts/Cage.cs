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
                sinkProgress = 0;
                break;
        }
    }

    public void Update()
    {
        switch (state)
        {
            case CageState.Sinking:
                var fromPos = World.Instance.currentAnchor.pivot.position + World.Instance.aboveSeaOffset;
                var toPos = World.Instance.currentAnchor.pivot.position;
                sinkProgress += Time.deltaTime * sinkSpeed;
                this.transform.position = Vector3.Lerp(fromPos, toPos, Helpers.EaseInOutQuad(sinkProgress));
                if (sinkProgress > 1)
                {
                    SetState(CageState.Underwater);
                }
                break;
        }
    }
}
