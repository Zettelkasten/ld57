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
    public Transform ropePivotLeft;
    public Transform ropePivotRight;

    public LineRenderer ropeLineRendererLeft;
    public LineRenderer ropeLineRendererRight;

    public Collider2D myCollider;
    
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
                // make all the things on the cage a child of the cage
                var things = GetThingsOnPlatform();
                foreach (var thing in things)
                {
                    thing.transform.SetParent(this.transform);
                }
                // Disable the ship collider
                World.Instance.aboveSea.shipCollider.enabled = false;
                sinkProgress = 0;
                break;
        }
    }

    public void Update()
    {
        // update the line renderer
        ropeLineRendererLeft.SetPosition(0, ropePivotLeft.position);
        ropeLineRendererLeft.SetPosition(1, World.Instance.aboveSea.shipRopePivotLeft.position);
        ropeLineRendererRight.SetPosition(0, ropePivotRight.position);
        ropeLineRendererRight.SetPosition(1, World.Instance.aboveSea.shipRopePivotRight.position);
        
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
                if (sinkProgress >= 1)
                {
                    SetState(state == CageState.Sinking ? CageState.Underwater : CageState.OnShip);
                    // make the player a child of the game scene
                    World.Instance.player.transform.SetParent(null);
                    // make all the things on the cage a child of the game scene
                    var things = GetThingsOnPlatform();
                    foreach (var thing in things)
                    {
                        thing.transform.SetParent(null);
                    }
                    // Enable the ship collider
                    World.Instance.aboveSea.shipCollider.enabled = true;
                }
                break;
        }
    }
    
    // get all colliding "Things" (layer) on the cage
    // todo, need to add a separate collider
    public Collider2D[] GetThingsOnPlatform()
    {
        var contacts = new Collider2D[50];
        var contactCount = myCollider.Overlap(new ContactFilter2D(), contacts);
        Collider2D[] things = new Collider2D[contactCount];
        int index = 0;
        for (int i = 0; i < contactCount; i++)
        {
            var contact = contacts[i];
            if (contact != null && contact.gameObject.layer == LayerMask.NameToLayer("Things"))
            {
                things[index] = contact;
                index++;
            }
        }
        // resize the array to the number of things found
        System.Array.Resize(ref things, index);
        return things;
    }
}
