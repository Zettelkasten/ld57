using System;
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

    public CageAnchor currentAnchor;
    public GameObject aboveSea;

    public Vector3 aboveSeaOffset;

    public CageState cageState;

    public void Start()
    {
        RepositionShip();
    }

    public void RepositionShip()
    {
        // move the ship above the current anchor
        aboveSea.transform.position = currentAnchor.pivot.position + aboveSeaOffset;
    }
}
