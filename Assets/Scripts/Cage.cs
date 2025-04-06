using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CageState
{
    OnShip = 0,
    Sinking = 1,
    Underwater = 2,
    Rising = 3,
    SellingItems = 4,
    SinkingWithoutPlayer = 5
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
    private List<Treasure> sinkingAttachedTreasures;
    
    // selling items
    private float sellingItemProgress;
    public float sellingItemSpeed;
    // items to be sold
    private List<Treasure> itemsToBeSold;
    
    Rigidbody2D cageRigidbody;
    Rigidbody2D playerRigidbody;
    
    private List<FixedJoint2D> joints = new List<FixedJoint2D>();

    void Start()
    {
        state = CageState.OnShip;
        World.Instance.player.transform.position = playerPivot.position;
        cageRigidbody = GetComponentInChildren<Rigidbody2D>();
        playerRigidbody = World.Instance.player.GetComponent<Rigidbody2D>();
    }
    
    public void SetState(CageState newState)
    {
        state = newState;
        switch (state)
        {
            case CageState.Sinking:
            case CageState.SinkingWithoutPlayer:
            case CageState.Rising:
                if (state != CageState.SinkingWithoutPlayer)
                {
                    // make the player a child of the cage
                    //World.Instance.player.transform.SetParent(this.transform);
                    World.Instance.player.fixJoints();

                    FixedJoint2D joint = cageRigidbody.gameObject.AddComponent<FixedJoint2D>();
                    joint.connectedBody = playerRigidbody;
                    joint.autoConfigureConnectedAnchor = false;
                    //joint.anchor = playerPivot.localPosition;
                    joints.Add(joint);

                    //World.Instance.player.transform.SetParent(this.transform);
                    // make all the things on the cage a child of the cage
                    sinkingAttachedTreasures = GetTreasuresOnPlatform();
                    foreach (var thing in sinkingAttachedTreasures)
                    {
                        thing.transform.SetParent(this.transform);
                    }

                    Debug.Log("Sinking treasures: " + sinkingAttachedTreasures.Count);
                    
                    sinkProgress = 0;
                }
                else
                {
                    // don't want the player to get hit by the falling cage
                    cageRigidbody.GetComponent<Collider2D>().enabled = false;
                    // also don't want player to wait so long, so skip forward a bit
                    sinkProgress = 0.7f;
                }

                // Disable the ship collider
                World.Instance.aboveSea.shipCollider.enabled = false;
                break;
            case CageState.SellingItems:
                sellingItemProgress = 0;
                break;
        }
    }

    public void FixedUpdate()
    {
        // update the line renderer
        ropeLineRendererLeft.SetPosition(0, ropePivotLeft.position);
        ropeLineRendererLeft.SetPosition(1, World.Instance.aboveSea.shipRopePivotLeft.position);
        ropeLineRendererRight.SetPosition(0, ropePivotRight.position);
        ropeLineRendererRight.SetPosition(1, World.Instance.aboveSea.shipRopePivotRight.position);
        
        switch (state)
        {
            case CageState.Sinking:
            case CageState.SinkingWithoutPlayer:
            case CageState.Rising:
                var fromPos = World.Instance.currentAnchor.pivot.position + World.Instance.aboveSeaOffset;
                var toPos = World.Instance.currentAnchor.pivot.position;
                if (state == CageState.Rising)
                {
                    (fromPos, toPos) = (toPos, fromPos);
                }

                sinkProgress += Time.fixedDeltaTime * sinkSpeed;
                this.transform.position = Vector3.Lerp(fromPos, toPos, Helpers.EaseInOutQuad(sinkProgress));
                if (sinkProgress >= 1)
                {
                    if (state != CageState.SinkingWithoutPlayer)
                    {
                        // make the player a child of the game scene
                        //World.Instance.player.transform.SetParent(null);
                        // destroy the joints
                        foreach (var joint2D in joints)
                        {
                            Destroy(joint2D);
                        }
                        //World.Instance.player.transform.SetParent(null);
                        World.Instance.player.fixJoints();
                        // make all the things on the cage a child of the game scene
                        foreach (var thing in sinkingAttachedTreasures)
                        {
                            thing.transform.SetParent(null);
                        }
                        // populate items to sell
                        Debug.Log("Sinking treasures: " + sinkingAttachedTreasures.Count);
                        if (state == CageState.Rising)
                        {
                            Debug.Log("Selling them");
                            itemsToBeSold = sinkingAttachedTreasures;
                        }
                    }
                    else
                    {
                        cageRigidbody.GetComponent<Collider2D>().enabled = true;
                    }

                    // Enable the ship collider
                    World.Instance.aboveSea.shipCollider.enabled = true;
                    SetState(state == CageState.Rising ? CageState.SellingItems : CageState.Underwater);
                }
                break;
            case CageState.SellingItems:
                if (itemsToBeSold.Count == 0)
                {
                    SetState(CageState.OnShip);
                }
                else
                {
                    sellingItemProgress += Time.fixedDeltaTime * sellingItemSpeed;
                    if (sellingItemProgress >= 1)
                    {
                        var item = itemsToBeSold[0];
                        itemsToBeSold.RemoveAt(0);
                        // sell the item
                        item.Sell();
                        sellingItemProgress = 0;
                    }
                }
                break;
        }
    }
    
    // get all colliding Treasures
    public List<Treasure> GetTreasuresOnPlatform()
    {
        var contacts = new Collider2D[50];
        var contactCount = myCollider.Overlap(new ContactFilter2D(), contacts);
        var treasures = new List<Treasure>(); 
        for (int i = 0; i < contactCount; i++)
        {
            var contact = contacts[i];
            // check if contact is not null and game object is of class Treasure
            if (contact != null && contact.gameObject.GetComponent<Treasure>() != null) 
            {
                var treasure = contact.gameObject.GetComponent<Treasure>();
                treasures.Add(treasure);
            }
        }
        return treasures;
    }
}
