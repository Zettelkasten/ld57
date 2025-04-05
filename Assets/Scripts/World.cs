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
    public Cage cage;

    public CageAnchor currentAnchor;
    public AboveSea aboveSea;

    public Vector3 aboveSeaOffset;

    public void Start()
    {
        RepositionShip();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (cage.state == CageState.OnShip)
                cage.SetState(CageState.Sinking);
            else if (cage.state == CageState.Underwater)
                cage.SetState(CageState.Rising);
        }
    }

    public void RepositionShip()
    {
        // move the ship above the current anchor
        aboveSea.transform.position = currentAnchor.pivot.position + aboveSeaOffset;
        cage.transform.position = currentAnchor.pivot.position + aboveSeaOffset;
        cage.SetState(CageState.OnShip);
    }
}
