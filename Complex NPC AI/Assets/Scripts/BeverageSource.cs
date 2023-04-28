using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeverageSource : MonoBehaviour
{
    public GameObject drinkPrefab;
    public GameObject foodPrefab;
    public GameObject chairPrefab;
    private GameObject beverageObject;
    private GameObject table;

    [SerializeField] public List<GameObject> chairs = new List<GameObject>(); 

    [Range(0.1f, 5)] public float chairScale; 

    public bool full;
    [Range(1, 6)] public int capacity;
    public int currentStaying; 

    public Beverage beverage;
    public Beverage.BeverageType beverageType;

    private void Awake()
    {
        full = false;
        currentStaying = 0;
        beverageObject = this.transform.Find("Beverage").gameObject; 
        if (beverageType == Beverage.BeverageType.DRINK)
        {
            Instantiate(drinkPrefab, beverageObject.transform); 
        }
        else
        {
            Instantiate(foodPrefab, beverageObject.transform);
        }
    }
    void Start()
    {
        table = transform.Find("Body").transform.Find("Table").gameObject;
        PlaceChairs(); 
    }

    void FixedUpdate()
    {
        int numberOccupied = 0;
        foreach(GameObject chair in chairs)
        {
            if (chair.GetComponent<PlacementScript>().occupied)
            {
                numberOccupied++;
            }
        }
        currentStaying = numberOccupied;

        if (currentStaying >= capacity)
        {
            full = true;
        }
        else
        {
            full = false; 
        }
    }

    public void PlaceChairs()
    {
        for (int i = 0; i < capacity; i++)
        {
            chairs.Add(Instantiate(chairPrefab));
            chairs[i].AddComponent<PlacementScript>();
            chairs[i].AddComponent<TriggerScript>();
            chairs[i].transform.parent = transform.Find("Chairs");
            chairs[i].name = "Chair_" + (i + 1);
        }
        setChairs();
    }

    public void setChairs()
    {
        int frontCapacity = capacity / 2;
        int backCapacity;

        float length = table.GetComponent<Renderer>().bounds.size.x;
        float width = table.GetComponent<Renderer>().bounds.size.z;

        bool rotated = false;

        if (length < width)
        {
            rotated = true;
            float temp = width;
            width = length;
            length = temp;
        }

        Vector3 tempPosition = new Vector3(0, 0, 0);

        if (capacity % 2 == 1)
        {
            backCapacity = frontCapacity + 1;
        }
        else
        {
            backCapacity = frontCapacity;
        }

        int frontPointer = 0;
        int backPointer = 0;

        float frontSpacing = length / frontCapacity;
        float backSpacing = length / backCapacity;

        for (int i = 0; i < capacity; i++)
        {
            if (!rotated)
            {
                float startingX = table.transform.position.x - length / 2;

                if (i % 2 == 1)
                {
                    tempPosition = new Vector3(startingX + (frontPointer * frontSpacing) + frontSpacing / 2, transform.position.y, table.transform.position.z + (width / 3 * 2));
                    chairs[i].transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
                    frontPointer++;
                }
                else
                {
                    tempPosition = new Vector3(startingX + (backPointer * backSpacing) + backSpacing / 2, transform.position.y, table.transform.position.z - (width / 3 * 2));
                    chairs[i].transform.rotation = Quaternion.Euler(0, transform.rotation.y - 180, 0);
                    backPointer++;
                }
            }
            else
            {
                float startingZ = table.transform.position.z - length / 2;

                if (i % 2 == 1)
                {
                    tempPosition = new Vector3(table.transform.position.x + (width / 3 * 2), transform.position.y, startingZ + (frontPointer * frontSpacing) + frontSpacing / 2);
                    chairs[i].transform.rotation = Quaternion.Euler(0, transform.rotation.y + 90, 0);
                    frontPointer++;
                }
                else
                {
                    tempPosition = new Vector3(table.transform.position.x - (width / 3 * 2), transform.position.y, startingZ + (backPointer * backSpacing) + backSpacing / 2);
                    chairs[i].transform.rotation = Quaternion.Euler(0, transform.rotation.y - 90, 0);
                    backPointer++;
                }
            }
            chairs[i].GetComponent<PlacementScript>().placementId = i;
            chairs[i].transform.position = tempPosition;
            chairs[i].transform.localScale = new Vector3(chairScale, chairScale, chairScale);
        }
    }
}
