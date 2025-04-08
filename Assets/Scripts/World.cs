using System;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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
    private bool bottomButtonClicked;

    private bool showBottomText = false;

    public CanvasGroup topButton;
    public TextMeshProUGUI topButtonText;
    
    public Transform overwriteSpawnPoint;

    public float timePerDialogueChar;
    
    public GameObject DialogueUI;
    public TextMeshProUGUI DialogueText;
    private bool dialogueButtonClicked;

    public GameObject[] speakerUIs;
    public string[] speakerUINames;

    public float discoverTreasureDistance;

    public GameObject Bubbletail;
    
    public GameObject upgradeScreen;
    public GameObject openUpgradeScreenButton;

    public DisplayItemBox sellBox;

    public CinemachineCamera cinemachineVirtualCamera;
    
    // audio
    public AudioClip shipMusic;
    public AudioClip underwaterMusic;

    public AudioSource splashAudioSource;
    
    public void Start()
    {
        RepositionShip();
        if (overwriteSpawnPoint != null)
        {
            player.transform.position = overwriteSpawnPoint.position;
        }
        DialogueUI.SetActive(false);
        bottomButtonClicked = false;
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
        // if the text starts with ROBOT: or similar, then show the first part in the SpeakerText:
        // and the rest in the DialogueText
        if (text.Contains(":"))
        {
            var split = text.Split(':');
            var speakerKey = split[0];
            // find the index of the speaker key in the speakerUINames array
            var index = Array.IndexOf(speakerUINames, speakerKey);
            // iterate through all speaker UIs and activate them if they match
            for (int i = 0; i < speakerUIs.Length; i++)
            {
                speakerUIs[i].SetActive(i == index);
            }
            DialogueText.text = split[1];
        }
        else
        {
            Debug.Log("Warning, there is a dialogue without a speaker key: " + text);
            // set all speaker UIs to inactive
            foreach (var speakerUI in speakerUIs)
            {
                speakerUI.SetActive(false);
            }
            DialogueText.text = text;
        }
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
        
        openUpgradeScreenButton.SetActive(cage.state == CageState.OnShip && BottomUIAvailable());
        
        // find all UpgradeCard s in the upgradeScreen
        foreach (var card in upgradeScreen.GetComponentsInChildren<UpgradeCard>())
        {
            if (card.shouldPlayDialogue && !DialogueUI.activeSelf)
            {
                // copy the dialogue component
                var dialogue = card.GetComponent<AttachedDialogue>();
                // copy it
                var newDialogue = Instantiate(dialogue, this.transform);
                newDialogue.PlayDialogue();
                card.shouldPlayDialogue = false;
            }
        }
        
        // audio
        if (SoundManager.Instance != null)
        {
            var waterLevelY = World.Instance.aboveSea.waterSplashParticles.transform.position.y;
            var shouldPlayUnderwaterMusic = cage.transform.position.y < waterLevelY;
            var changed = SoundManager.Instance.PlayMusic(
                shouldPlayUnderwaterMusic ? underwaterMusic : shipMusic);
            if (changed)
            {
                SoundManager.PlaySource(splashAudioSource);
            }
        }
    }

    public bool BottomUIAvailable()
    {
        // also to check if player can press E to interact with the cage
        return !DialogueUI.activeSelf && !upgradeScreen.activeSelf && cage.state != CageState.SellingItems &&
               cage.showShopAfterDelay <= 0;
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

    public void SetBottomButtonClicked()
    {
        bottomButtonClicked = true;
    }

    public bool CheckBottomButtonClicked()
    {
        if (bottomButtonClicked)
        {
            bottomButtonClicked = false;
            return true;
        }
        return false;
    }
    
    public void SetDialogueButtonClicked()
    {
        dialogueButtonClicked = true;
    }

    public bool CheckDialogueButtonClicked()
    {
        if (dialogueButtonClicked)
        {
            dialogueButtonClicked = false;
            return true;
        }
        return false;
    }
}
