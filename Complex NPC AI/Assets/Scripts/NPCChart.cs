using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCChart : MonoBehaviour
{
    AIScript hostNPC;

    private TextMeshPro npcName;
    private TextMeshPro beverage;
    private TextMeshPro confidence;
    private TextMeshPro gender;
    private TextMeshPro npcLiking;
    private TextMeshPro thirst;
    private TextMeshPro urgency;
    private TextMeshPro hunger;

    private void Awake()
    {
        hostNPC = transform.parent.GetComponent<AIScript>();


        npcName = transform.Find("NpcName").GetComponent<TextMeshPro>();
        beverage = transform.Find("Beverage").GetComponent<TextMeshPro>();
        confidence = transform.Find("Confidence").GetComponent<TextMeshPro>();
        gender = transform.Find("Gender").GetComponent<TextMeshPro>();
        npcLiking = transform.Find("NPC Liking").GetComponent<TextMeshPro>();
        thirst = transform.Find("Thirst").GetComponent<TextMeshPro>();
        urgency = transform.Find("Urgency").GetComponent<TextMeshPro>();
        hunger = transform.Find("Hunger").GetComponent<TextMeshPro>();
    }

    void Start()
    {
        hostNPC = transform.parent.GetComponent<AIScript>();


        npcName = transform.Find("NpcName").GetComponent<TextMeshPro>();
        beverage = transform.Find("Beverage").GetComponent<TextMeshPro>();
        confidence = transform.Find("Confidence").GetComponent<TextMeshPro>();
        gender = transform.Find("Gender").GetComponent<TextMeshPro>();
        npcLiking = transform.Find("NPC Liking").GetComponent<TextMeshPro>();
        thirst = transform.Find("Thirst").GetComponent<TextMeshPro>();
        urgency = transform.Find("Urgency").GetComponent<TextMeshPro>();
        hunger = transform.Find("Hunger").GetComponent<TextMeshPro>();

        npcName.text = transform.parent.name;

        UpdateVars();
    }

    private void FixedUpdate()
    {
        this.transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").transform.position);
        UpdateVars();
    }

    void UpdateVars()
    {
        if (hostNPC.currentBeverage == Beverage.BeverageType.DRINK)
        {
            beverage.text = "Drink";
        }
        else
        {
            beverage.text = "Food";
        }
        confidence.text = "" + hostNPC.confidence;

        if (hostNPC.gender == AIScript.Gender.MAN)
        {
            gender.text = "Man";
        }
        else
        {
            gender.text = "Woman";
        }

        if (hostNPC.genderPreference == AIScript.Gender.MAN)
        {
            npcLiking.text = "Man";
        }
        else
        {
            npcLiking.text = "Woman";
        }

        thirst.text = "" + hostNPC.thirst;
        urgency.text = "" + hostNPC.urgency;
        hunger.text = "" + hostNPC.hunger;
    }
}
