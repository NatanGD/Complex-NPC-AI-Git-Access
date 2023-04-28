using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    public List<GameObject> npcList;
    public List<GameObject> tableList;
    public List<GameObject> standList;

    public bool showDebugChart = false; 

    public GameObject npcPrefab;
    private GameObject spawner; 
    private GameObject spawnPoint;
    private GameObject npcParent;

    [Range(1, 10)] public float spawnTimer;
    [Range(2.5f, 10)] public float spawnFrequency;
    [Range(1, 20)] public int maxAmount;

    private float frequency = 0;
    private int npcCounter = 0;

    private void Awake()
    {

        npcList = new List<GameObject>();
        tableList = new List<GameObject>();
        standList = new List<GameObject>(); 
        tableList.AddRange(GameObject.FindGameObjectsWithTag("Table"));
        standList.AddRange(GameObject.FindGameObjectsWithTag("Stand"));
        spawner = GameObject.FindGameObjectWithTag("Spawner");
        spawnPoint = spawner.transform.Find("SpawnPoint").gameObject;
        SpawnParent(); 
    }

    private void Start()
    {
        frequency = Random.Range(spawnFrequency - 2, spawnFrequency + 2); 
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer > frequency)
        {
            frequency = Random.Range(spawnFrequency - 2, spawnFrequency + 2);
            spawnTimer = 0;
            if (npcList.Count < maxAmount)
            {
                SpawnNPC();
            }
        }
    }

    public void SpawnParent()
    {
        npcParent = this.transform.Find("NPCs").gameObject;
        npcParent.tag = "NPCParent";
        npcParent.name = "NPC Parent"; 
    }

    public void SpawnNPC()
    {
        if(npcList.Count < maxAmount)
        {
            GameObject npc = Instantiate(npcPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            
            npc.name = "NPC " + npcCounter;
            npcCounter++;

            if (showDebugChart)
            {
                npc.GetComponent<AIScript>().InititateChart(true);
            }
            else
            {
                npc.GetComponent<AIScript>().InititateChart(false);
            }

            npc.transform.position = spawnPoint.transform.position;
            npc.transform.rotation = spawnPoint.transform.rotation;
            npc.transform.parent = GameObject.FindGameObjectWithTag("Manager").transform.GetChild(0).transform;

            npcList.Add(npc);
        }
    }
}
