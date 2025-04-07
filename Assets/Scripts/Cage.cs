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
    private List<Rigidbody2D> sinkingAttachedObjects;
    
    private Transform cameraPosition;
    
    // selling items
    private float sellingItemProgress;
    public float sellingItemTime;
    private float showShopAfterDelay;
    // items to be sold
    private List<Treasure> itemsToBeSold;
    // wait for this item to end its dialogue
    private Treasure waitingForSellDialogue = null;
    
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
                    sinkingAttachedObjects = GetObjectsToAttachToPlatform();
                    joints.Clear();
                    foreach (var thing in sinkingAttachedObjects)
                    {
                        // add a joint
                        FixedJoint2D joint2D = cageRigidbody.gameObject.AddComponent<FixedJoint2D>();
                        joint2D.connectedBody = thing;
                        joint2D.autoConfigureConnectedAnchor = false;
                        joints.Add(joint2D);
                    }
                    // for all real-sub rigid body of the player, disable simulation
                    foreach (var thing in World.Instance.player.GetComponentsInChildren<Rigidbody2D>())
                    {
                        thing.simulated = false;
                    }
                    World.Instance.player.GetComponent<Rigidbody2D>().simulated = true;

                    Debug.Log("Sinking treasures: " + sinkingAttachedObjects.Count);
                    
                    sinkProgress = 0;
                }
                else
                {
                    // don't want the player to get hit by the falling cage
                    cageRigidbody.GetComponent<Collider2D>().enabled = false;
                    // also don't want player to wait so long, so skip forward a bit
                    sinkProgress = 0.7f;
                }

                if (state == CageState.Sinking)
                {
                    // instantiate a dummy object
                    cameraPosition = new GameObject("CameraPosition").transform;
                    cameraPosition.position = World.Instance.player.transform.position;
                    World.Instance.cinemachineVirtualCamera.Follow = cameraPosition;
                }

                // Disable the ship collider
                World.Instance.aboveSea.shipCollider.enabled = false;
                break;
            case CageState.SellingItems:
                World.Instance.sellBox.displayBox.SetActive(true);
                sellingItemProgress = 0;
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
            case CageState.OnShip:
                if (showShopAfterDelay > 0)
                {
                    showShopAfterDelay -= Time.deltaTime;
                    if (showShopAfterDelay <= 0)
                    {
                        World.Instance.upgradeScreen.SetActive(true);
                        World.Instance.sellBox.displayBox.SetActive(false);
                    }
                }
                break;
			case CageState.SellingItems:
				if (waitingForSellDialogue != null)
				{
					if (!World.Instance.DialogueUI.activeSelf)
					{
						waitingForSellDialogue.Sell();
						waitingForSellDialogue = null;
					}
					else
					{
						return;
					}
				}
				if (itemsToBeSold.Count == 0)
				{
					SetState(CageState.OnShip);
                    showShopAfterDelay = 1.5f;
				}
				else
				{
					if (sellingItemProgress == 0)
					{
						var item = itemsToBeSold[0];
						item.MoveToSellBoxAndPlayCoolParticleAnimation(sellingItemTime);
					}

					sellingItemProgress += Time.deltaTime;
					if (sellingItemProgress >= sellingItemTime)
					{
						var item = itemsToBeSold[0];
						itemsToBeSold.RemoveAt(0);
						// sell the item
						item.SellDialogue();
						waitingForSellDialogue = item;
						sellingItemProgress = 0;
					}
				}
				break;
		}

	}

	public void FixedUpdate()
    {
        
        
        switch (state)
        {
            case CageState.Sinking:
            case CageState.SinkingWithoutPlayer:
            case CageState.Rising:
                var fromPos = World.Instance.currentAnchor.pivot.position;
                fromPos.y = Waterphysics.waterlevel;
                var toPos = World.Instance.currentAnchor.pivot.position;
                if (state == CageState.Rising)
                {
                    (fromPos, toPos) = (toPos, fromPos);
                }

                sinkProgress += Time.fixedDeltaTime * sinkSpeed;
                var lastY = transform.position.y;
                this.transform.position = Vector3.Lerp(fromPos, toPos, Helpers.EaseInOutQuad(sinkProgress));
                var thisY = transform.position.y;
                
                var waterLevelY = World.Instance.aboveSea.waterSplashParticles.transform.position.y;
                if (state == CageState.Sinking && thisY < waterLevelY && lastY >= waterLevelY)
                {
                    // splash
                    World.Instance.aboveSea.waterSplashParticles.Play();
                }
                else if (state == CageState.Rising && thisY > waterLevelY && lastY <= waterLevelY)
                {
                    // splash
                    World.Instance.aboveSea.waterSplashParticles.Play();
                }

                if (cameraPosition != null)
                {
                    // until 0.3, don't move it, then move it to the player
                    if (sinkProgress >= 0.3f)
                    {
                        cameraPosition.position = Vector3.Lerp(cameraPosition.position, World.Instance.player.transform.position, Helpers.EaseInOutQuad((sinkProgress - 0.3f) / 0.7f));
                    }
                }
                
                if (sinkProgress >= 1)
                {
                    World.Instance.cinemachineVirtualCamera.Follow = World.Instance.player.transform;
                    if (cameraPosition != null)
                    {
                        Destroy(cameraPosition.gameObject);
                        cameraPosition = null;
                    }

                    if (state != CageState.SinkingWithoutPlayer)
                    {
                        // make the player a child of the game scene
                        //World.Instance.player.transform.SetParent(null);
                        // destroy the joints
                        foreach (var joint in joints)
                        {
                            Destroy(joint);
                        }
                        joints.Clear();
                        // re-enable simulations
                        foreach (var playerthing in World.Instance.player.GetComponentsInChildren<Rigidbody2D>())
                        {
                            playerthing.simulated = true;
                        }
                        // populate items to sell
                        Debug.Log("Sinking treasures: " + sinkingAttachedObjects.Count);
                        if (state == CageState.Rising)
                        {
                            Debug.Log("Selling them");
                            // filter all rigidbodies which are treasures
                            itemsToBeSold = new List<Treasure>();
                            foreach (var thing in sinkingAttachedObjects)
                            {
                                var treasure = thing.GetComponent<Treasure>();
                                if (treasure != null)
                                {
                                    itemsToBeSold.Add(treasure);
                                }
                            }
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
        }
    }
    
    public List<Rigidbody2D> GetObjectsToAttachToPlatform()
    {
        var contacts = new Collider2D[50];
        var contactCount = myCollider.Overlap(new ContactFilter2D(), contacts);
        var treasures = new List<Rigidbody2D>(); 
        for (int i = 0; i < contactCount; i++)
        {
            var contact = contacts[i];
            // check if contact is not null and game object is of class Treasure
            if (contact != null && contact.gameObject.GetComponent<Rigidbody2D>() != null && (
                    // contact.gameObject.layer == LayerMask.NameToLayer("Playerconstruction")
                    // || 
                    contact.gameObject.layer == LayerMask.NameToLayer("Player")
                    || contact.gameObject.GetComponent<Treasure>() != null)) 
            {
                treasures.Add(contact.gameObject.GetComponent<Rigidbody2D>());
            }
        }
        return treasures;
    }
}
