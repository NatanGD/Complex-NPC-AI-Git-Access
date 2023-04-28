using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementScript : MonoBehaviour
{
    public PlacementScript placementScript;
    public GameObject currentNPC;

    public int placementId = 0;
    public bool inFrontEmpty = false;
    public bool occupied = false;
    public Vector3 scaling = new Vector3(1, 1, 1);
    public Vector3 chairPosition = new Vector3(0, 0, 0);

    public PlacementScript()
    {
        occupied = false;
        scaling = new Vector3(1, 1, 1);
        chairPosition = new Vector3(0, 0, 0);
    }

    private void Awake()
    {

    }

}
