using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class World : MonoBehaviour
{
    public static World Instance;

    private World()
    {
        Instance = this;
    }

    public Player player;
    public Cage cage;

    public CageAnchor currentAnchor;
    public AboveSea aboveSea;
    
    public GameObject bottomButton;
    public TextMeshProUGUI bottomButtonText;

    private bool showBottomText = false;

    public CanvasGroup topButton;
    public TextMeshProUGUI topButtonText;
    
    public Transform overwriteSpawnPoint;

    public float timePerDialogueChar;
    
    public GameObject DialogueUI;
    public TextMeshProUGUI DialogueText;

    public float discoverTreasureDistance;

    public GameObject Bubbletail;
    
    public void Start()
    {
        RepositionShip();
        if (overwriteSpawnPoint != null)
        {
            player.transform.position = overwriteSpawnPoint.position;
        }
    }
    public void RepositionShip()
    {
        // move the ship above the current anchor, set y to WaterPhyics.waterlevel.
        aboveSea.transform.position = new Vector3(currentAnchor.pivot.position.x, Waterphysics.waterlevel, 0);
        cage.transform.position = new Vector3(currentAnchor.pivot.position.x, Waterphysics.waterlevel, -2);
        // set z to 0
        aboveSea.transform.position = new Vector3(aboveSea.transform.position.x, aboveSea.transform.position.y, 0);
        cage.SetState(CageState.OnShip);
    }

    public void ShowBottomText(string text)
    {
        bottomButtonText.text = text;
        showBottomText = true;
    }
    
    public void ShowDialogueText(string text)
    {
        DialogueText.text = text;
    }

    public void Update()
    {
        var group = bottomButton.GetComponent<CanvasGroup>();
        group.alpha = Mathf.MoveTowards(group.alpha, showBottomText ? 1 : 0, Time.deltaTime * 5);
        if (DialogueUI.activeSelf)
        {
            // don't overlap with the dialogue UI
            group.alpha = 0;
            showBottomText = false;
        }
        group.interactable = showBottomText;
        group.blocksRaycasts = showBottomText;
        showBottomText = false;
        
        var topButtonAlpha = cage.state is CageState.Sinking or CageState.Rising ? 0 : 1;
        topButton.alpha = Mathf.MoveTowards(topButton.alpha, topButtonAlpha, Time.deltaTime * 5);
        topButtonText.text = player.GetTopText();
        
        // respawn player if cage is in underwater or on ship
        if (Input.GetKeyDown(KeyCode.R) && cage.state is CageState.Underwater or CageState.OnShip)
        {
            RespawnPlayer();
        }
    }

    public bool BottomUIAvailable()
    {
        return !DialogueUI.activeSelf;
    }

    public void RespawnPlayer()
    {
        RepositionShip();
        if (overwriteSpawnPoint != null)
        {
            player.transform.position = overwriteSpawnPoint.position;
        } else {
            player.transform.position = cage.playerPivot.position;
        }
        player.transform.rotation = Quaternion.identity;
        // reset player velocity
        player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        player.GetComponent<Rigidbody2D>().angularVelocity = 0;
    }
}
