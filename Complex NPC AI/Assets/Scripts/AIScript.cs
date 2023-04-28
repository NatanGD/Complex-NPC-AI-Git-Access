using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIScript : MonoBehaviour
{
    private NavMeshAgent agent; 
    private GameObject npcManager;

    private bool showDebugChart = false; 

    public Material[] skinColours;
    public Material selfSkin;


    [Range(0, 5)] public float maxCollectionDuration;
    [Range(0, 30)] public float maxActionDuration = 20;
    [Range(0, 30)] public float maxToiletDuration = 30;
    [Range(0.05f, 1)] public float rateOfConsumption = 0.5f;

    public bool startAction;
    public bool startCollection;

    [Range(1, 10)] public float hunger;
    [Range(1, 10)] public float thirst;
    [Range(1, 10)] public float urgency;
    [Range(1, 10)] public float confidence;
    [Range(0, 1)] public float drunkenness;

    public bool isSitting; 
    public bool isBeveraging;
    public bool isQueueing; 
    public bool collectedBeverage;
    public bool destinationFound;
    public bool isLeaving;
    public bool isToileting;
    public bool inToilet;
    public bool hasBinned;
    public bool isBinning = false;

    private Vector3 destination;

    private GameObject chosenStandObject;
    private GameObject chosenChairObject;
    private GameObject chosenTableObject;
    public int currentQueue = 0, queueTarget = 0, chosenStand = 0;

    public GameObject beverageObject; 

    private Vector3 groupMidPoint = Vector3.zero;

    private List<GameObject> tables;
    private List<GameObject> stands; 
    private List<float> tableDistances;

    private float collectionTimer; 
    private float actionTimer;
    private float toiletTimer = 0;
    private float beveragingTimer = 0;

    public enum Gender
    {
        MAN = 0,
        WOMAN = 1
    }

    public Gender gender;
    public Gender genderPreference;

    public Beverage.BeverageType beveragePreference;
    public Beverage.BeverageType currentBeverage; 

    private void Awake()
    {
        npcManager = GameObject.FindGameObjectWithTag("Manager");
        tables = npcManager.GetComponent<ManagerScript>().tableList;
        stands = npcManager.GetComponent<ManagerScript>().standList; 
        agent = GetComponent<NavMeshAgent>(); 
        if (Random.Range(0f, 1f) > 0.5)
        {
            hunger = Random.Range(4, 7);
            thirst = Random.Range(1, 7);
        }
        else
        {
            hunger = Random.Range(1, 7);
            thirst = Random.Range(4, 7);
        }

        urgency = Random.Range(1, 5);
        confidence = Random.Range(1, 10);
        drunkenness = 0;
        isBeveraging = false;
        isSitting = false; 
        isQueueing = true;
        isLeaving = false;
        isToileting = false;
        inToilet = false; 
        hasBinned = false; 
        destinationFound = false;

        destination = new Vector3();

        gender = (Gender)Random.Range(0, 2);
        genderPreference = (Gender)Random.Range(0, 2);
        selfSkin = skinColours[Random.Range(0, skinColours.Length)];
        this.transform.Find("Body/Head").GetComponent<Renderer>().material = selfSkin;

        beveragePreference = (Beverage.BeverageType)Random.Range(0, 2);

        startCollection = false; 
        collectionTimer = maxCollectionDuration;
        startAction = false;
        actionTimer = Random.Range(maxActionDuration / 2, maxActionDuration); 
    }

    void Start()
    {
        if (showDebugChart)
        {
            transform.Find("Chart").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Chart").gameObject.SetActive(false);
        }
        InvokeRepeating("Satiation", 10, 2);
    }

    public void Satiation()
    {
        if (!inToilet)
        {
            hunger += 0.01f;
            thirst += 0.01f;
        } 
    }

    public void InititateChart(bool isActive)
    {
        showDebugChart = isActive;
    }

    void FixedUpdate()
    {
        ChooseBeverage();
        if (!isLeaving)
        {
            if (!isToileting)
            {
                if (isQueueing)
                {
                    if (startCollection)
                    {
                        Collecting();
                    }
                    else
                    {
                        Queueing();
                    }
                }
                else
                {
                    if (!isBinning)
                    {
                        if (!isBeveraging)
                        {
                            Navigation();
                        }
                        else
                        {
                            Beveraging();
                        }
                    }
                    else
                    {
                        GoBin();
                    }
                }
            }
            else
            {
                if (!inToilet)
                {
                    GoToilet();
                }
                else
                {
                    ToiletBreak();
                }
            }
        }
        else
        {
            if (!hasBinned)
            {
                GoBin();
            }
            else
            {
                Leave();
            }
        }
    }

    public void ChooseBeverage()
    {
        int beverageWant = 0;
        if (beveragePreference == Beverage.BeverageType.FOOD)
        {
            beverageWant += 5; 
        }
        else
        {
            beverageWant -= 5; 
        }
        beverageWant = (int)(beverageWant + hunger - thirst);

        if (beverageWant > 0)
        {
            currentBeverage = Beverage.BeverageType.FOOD;
        }
        else if (beverageWant < 0)
        {
            currentBeverage = Beverage.BeverageType.DRINK;
        }
        else
        {
            currentBeverage = beveragePreference; 
        }
    }

    public int CompareDistance(GameObject first, GameObject second)
    {
        float firstDis = Vector3.Distance(this.transform.position, first.transform.position);
        float secondDis = Vector3.Distance(this.transform.position, second.transform.position);

        return firstDis.CompareTo(secondDis);
    }
    
    public int ChairSort(GameObject first, GameObject second)
    {
        float firstDis = Vector3.Distance(groupMidPoint, first.transform.localPosition);
        float secondDis = Vector3.Distance(groupMidPoint, second.transform.localPosition);

        return firstDis.CompareTo(secondDis);
    }

    public void Queueing()
    {
        if (!collectedBeverage)
        {
            if (!destinationFound)
            {
                stands.Sort(CompareDistance);
                for (int i = 0; i < stands.Count; i++)
                {
                    if (stands[i].GetComponent<StandScript>().beverageType == currentBeverage && !collectedBeverage
                        && !stands[i].GetComponent<StandScript>().isFull)
                    {
                        FindQueue(stands[i], i);
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("destination is found " + stands[chosenStand].name);
                FindQueue(stands[chosenStand], chosenStand);
            }
        }
    }

    public void Collecting()
    {
        if (startCollection)
        {
            transform.LookAt(new Vector3(stands[chosenStand].transform.Find("Clerk").position.x, transform.position.y, stands[chosenStand].transform.Find("Clerk").position.z));
            collectionTimer -= Time.deltaTime;
            if (collectionTimer < 0)
            {
                destinationFound = false;
                collectedBeverage = true;
                isQueueing = false;
                startCollection = false;
                collectionTimer = maxCollectionDuration;

                beverageObject = Instantiate(stands[chosenStand].GetComponent<StandScript>().beverageObject, transform.Find("BeverageHolder"));
                beverageObject.transform.localPosition = new Vector3(0, 0.7f, 0.35f);
            }
        }
    }

    public void Navigation()
    {
        tables.Sort(CompareDistance);

        if (collectedBeverage)
        {
            if (!destinationFound)
            {
                bool willSit = false;
                for (int i = 0; i < tables.Count; i++)
                {
                    if (tables[i].GetComponent<BeverageSource>().beverageType == currentBeverage &&
                        !tables[i].GetComponent<BeverageSource>().full && !isBeveraging)
                    {
                        GameObject[] tableChairs = new GameObject[tables[i].GetComponent<BeverageSource>().capacity];

                        Vector3 crowdMidPoint = Vector3.zero;
                        List<GameObject> occupiedList = new List<GameObject>();
                        List<GameObject> freeList = new List<GameObject>();

                        Vector3 tempSum = Vector3.zero;
                        int totalPreferences = 0;

                        for (int x = 0; x < tables[i].GetComponent<BeverageSource>().capacity; x++)
                        {
                            if (tables[i].GetComponent<BeverageSource>().chairs[x].GetComponent<PlacementScript>().occupied)
                            {
                                tempSum += tables[i].GetComponent<BeverageSource>().chairs[x].transform.localPosition; 

                                if (tables[i].GetComponent<BeverageSource>().chairs[x].GetComponent<PlacementScript>().currentNPC.GetComponent<AIScript>().gender == genderPreference)
                                {
                                    totalPreferences++;
                                }

                                occupiedList.Add(tables[i].GetComponent<BeverageSource>().chairs[x]);
                            }
                            else
                            {
                                freeList.Add(tables[i].GetComponent<BeverageSource>().chairs[x]);
                            }
                        }

                        crowdMidPoint = tempSum / occupiedList.Count;
                        float tempC = confidence + totalPreferences + drunkenness;

                        if (tempC / 10 > (float)occupiedList.Count / tables[i].GetComponent<BeverageSource>().capacity)
                        {
                            destinationFound = true;
                            chosenTableObject = tables[i];
                            freeList.Sort(ChairSort);

                            if (freeList.Count > 0)
                            {
                                if (tempC > 0.5f)
                                {
                                    chosenChairObject = freeList[0]; 
                                }
                                else
                                {
                                    chosenChairObject = freeList[freeList.Count - 1];
                                }
                                willSit = true;
                                chosenChairObject.GetComponent<PlacementScript>().occupied = true;
                                chosenChairObject.GetComponent<PlacementScript>().currentNPC = this.gameObject;
                                Vector3 tempDes = destination;
                                destination = chosenChairObject.transform.position;
                                break;
                            }
                        }
                    }
                    if (i == tables.Count - 1 && !willSit)
                    {
                        ResetNPC();
                        isLeaving = true;
                    }
                }
            }

            agent.destination = destination;
        }
    }

    public void FindQueue(GameObject stand, int standPointer)
    {
        bool found = false;
        foreach (GameObject npc in stand.GetComponent<StandScript>().npcQueue)
        {
            if (this.name == npc.name)
            {
                found = true;
            }
        }
        if (!found && stand.GetComponent<StandScript>().npcQueue.Count < 
            stand.GetComponent<StandScript>().queue.Count)
        {
            stand.GetComponent<StandScript>().npcQueue.Add(this.gameObject);
            isQueueing = true;
            chosenStand = standPointer;
            chosenStandObject = stand;
            destinationFound = true;
        }

        if (destinationFound)
        {
            transform.LookAt(new Vector3(stand.transform.Find("Clerk").position.x, transform.position.y, stand.transform.Find("Clerk").position.z));
            stand.GetComponent<StandScript>().UpdateQueue();
            agent.destination = stand.GetComponent<StandScript>().queue[currentQueue].transform.position;
        }
    }

    public void ResetNPC()
    {
        currentQueue = 0; 

        isSitting = false;
        isLeaving = false;
        isQueueing = false;
        isBeveraging = false; 
        destinationFound = false;
        hasBinned = false;
        isBinning = false;
        inToilet = false;
        isToileting = false;

        startAction = false;
        actionTimer = maxActionDuration;
        beveragingTimer = 0;
        toiletTimer = 0;

        if (collectedBeverage)
        {
            GoBin();
        }

        ChooseBeverage();

        if (hunger >= 10 || thirst <= 0)
        {
            beveragePreference = Beverage.BeverageType.FOOD;

        }
        else if (thirst >= 10 || hunger <= 0)
        {
            beveragePreference = Beverage.BeverageType.DRINK;
        }

        if (urgency > 10)
        {
            GoToilet();
        }

        if (hunger <= 0.5f || thirst <= 0.5f)
        {
            if (Random.Range(0f,1f) > 0.5f)
            {
                isLeaving = true;
            }
            else
            {
                isQueueing = true; 
            }
        }
    }
    public void Beveraging()
    {
        agent.destination = chosenChairObject.transform.position;
        isSitting = true; 

        transform.LookAt(chosenChairObject.transform.Find("Front").transform);
        if (beverageObject != null)
        {
            beverageObject.transform.localPosition = new Vector3(0, 0.52f, 0.5f);
        }

        if (startAction)
        {
            actionTimer -= Time.deltaTime;
            beveragingTimer += Time.deltaTime;

            if (beveragingTimer > maxActionDuration / 10)
            {
                if (currentBeverage == Beverage.BeverageType.FOOD)
                {
                    hunger -= rateOfConsumption;
                    urgency += rateOfConsumption / 2;
                    if (hunger <= 0)
                    {
                        hunger = 0;
                        chosenChairObject.GetComponent<PlacementScript>().currentNPC = null;
                        chosenChairObject.GetComponent<PlacementScript>().occupied = false;
                        ResetNPC();
                    }
                    if (hunger >= 10)
                    {
                        hunger = 10;
                        chosenChairObject.GetComponent<PlacementScript>().currentNPC = null;
                        chosenChairObject.GetComponent<PlacementScript>().occupied = false;
                        ResetNPC();
                    } 
                }
                else
                {
                    thirst -= rateOfConsumption;
                    if (drunkenness < 0.9)
                    {
                        drunkenness += 0.1f;
                    }
                        
                    urgency += rateOfConsumption; 
                    if (thirst <= 0)
                    {
                        thirst = 0;
                        chosenChairObject.GetComponent<PlacementScript>().currentNPC = null;
                        chosenChairObject.GetComponent<PlacementScript>().occupied = false;
                        ResetNPC();
                    }
                    if (thirst >= 10)
                    {
                        thirst = 10;
                        chosenChairObject.GetComponent<PlacementScript>().currentNPC = null;
                        chosenChairObject.GetComponent<PlacementScript>().occupied = false;
                        ResetNPC();
                    }
                }
                beveragingTimer = 0;
            }

            if (actionTimer < 0)
            {
                beverageObject.transform.Find("Beverage").gameObject.SetActive(false);

                startAction = false;
                actionTimer = maxActionDuration;
                beveragingTimer = 0;
                ResetNPC();

                if (Random.Range(0f,1f) > 0.5f)
                {
                    isLeaving = true;
                }
                else
                {
                    isQueueing = true; 
                }
                isSitting = false; 
            }
        }
        if (urgency > 10)
        {
            ResetNPC();
            GoToilet();
        }
    }
    
    public void Leave()
    {
        agent.destination = GameObject.FindGameObjectWithTag("Spawner").transform.position; 
        if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Spawner").transform.position) < 0.5f)
        {
            GameObject.FindGameObjectWithTag("Manager").GetComponent<ManagerScript>().npcList.Remove(this.gameObject);
            foreach (GameObject stand in GameObject.FindGameObjectsWithTag("Stand"))
            {
                if (stand.GetComponent<StandScript>().npcQueue.Contains(this.gameObject))
                {
                    stand.GetComponent<StandScript>().npcQueue.Remove(this.gameObject);
                }
            }
            Destroy(this.gameObject);
        }
    }

    public void GoBin()
    {
        isBinning = true;
        agent.destination = GameObject.FindGameObjectWithTag("Bin").transform.position;
    }

    public void GoToilet()
    {
        isToileting = true;
        if (!hasBinned)
        {
            GoBin();
        }
        else
        {
            if (gender == Gender.MAN)
            {
                agent.destination = GameObject.FindGameObjectWithTag("Toilet").transform.Find("ManPoint").position;
            }
            else
            {
                agent.destination = GameObject.FindGameObjectWithTag("Toilet").transform.Find("WomanPoint").position;
            }
        }
    }

    public void ToiletBreak()
    {
        inToilet = true;

        toiletTimer += Time.deltaTime;
        if (toiletTimer < maxToiletDuration)
        {
            transform.GetComponent<CapsuleCollider>().enabled = false;
            for (int i = 0; i < transform.childCount; i ++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        if (toiletTimer > maxToiletDuration)
        {
            transform.GetComponent<CapsuleCollider>().enabled = true; 
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }

            if (gender == Gender.MAN)
            {
                transform.position = GameObject.FindGameObjectWithTag("Toilet").transform.Find("ManPoint").position;
            }
            else
            {
                transform.position = GameObject.FindGameObjectWithTag("Toilet").transform.Find("WomanPoint").position;
            }

            urgency = 0;

            ResetNPC();
            isQueueing = true;
        }
    }
}
