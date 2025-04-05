using System;
using UnityEngine;

public class CageController : MonoBehaviour
{
    public Transform shipPivot;
    public Transform underwaterPivot;

    public void Start()
    {
        this.transform.position = shipPivot.position;
    }

    public void PrepositionShip()
    {
        
    }
}
