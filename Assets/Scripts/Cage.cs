using System.Collections.Generic;
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
    
    // sinking up or raising down
    private float sinkProgress;
    public float sinkSpeed;
    
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
            case CageState.Rising:
                // make the player a child of the cage
                //World.Instance.player.transform.SetParent(this.transform);
                FixedJoint2D joint = cageRigidbody.gameObject.AddComponent<FixedJoint2D>();
                joint.connectedBody = playerRigidbody;
                joint.autoConfigureConnectedAnchor = false;
                //joint.anchor = playerPivot.localPosition;
                joints.Add(joint);
                
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
                    //World.Instance.player.transform.SetParent(null);
                    // destroy the joints
                    foreach (var joint2D in joints)
                    {
                        Destroy(joint2D);
                    }
                    // Enable the ship collider
                    World.Instance.aboveSea.shipCollider.enabled = true;
                }
                break;
        }
    }
}
