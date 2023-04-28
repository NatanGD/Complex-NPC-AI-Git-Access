using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandScript : MonoBehaviour
{
    public Beverage beverage;
    public GameObject queuePrefab; 
    public Beverage.BeverageType beverageType;
    public GameObject beverageObject;
    public List<GameObject> queue = new List<GameObject>();
    public List<GameObject> npcQueue = new List<GameObject>();

    [Range(0f, 2)] public float queueSpacing;
    [Range(1, 10)] public int capacity;
    public int currentStaying = 0;

    public bool isFull = false;

    private void Awake()
    {
        
    }

    void Start()
    {
        AddQueue();
        InvokeRepeating("UpdateQueue", 0, 1f);
    }

    public void AddQueue()
    {
        GameObject tempQueue = Instantiate(queuePrefab, transform.Find("Queue"));
        tempQueue.transform.position = transform.Find("Queue").position;
        tempQueue.name = "Queue " + queue.Count;
        tempQueue.transform.LookAt(transform);
        Vector3 backVector = Quaternion.Euler(tempQueue.transform.eulerAngles) * -Vector3.forward;
        backVector.Normalize();
        if (queue.Count > 0)
        {
            tempQueue.transform.position = tempQueue.transform.position + backVector * queueSpacing * queue.Count;
        }
        else
        {
            tempQueue.transform.position = tempQueue.transform.parent.transform.position; 
        }
        tempQueue.GetComponent<PlacementScript>().placementId = queue.Count;
        queue.Add(tempQueue);
    }

    public void Shuffle(bool type)
    {
        if (!type && npcQueue.Count > 0)
        {
            npcQueue.RemoveAt(0);
            currentStaying = npcQueue.Count;
        }
    }
    public int CompareDistance(GameObject first, GameObject second)
    {
        float firstDis = Vector3.Distance(transform.Find("Queue").transform.position, first.transform.position);
        float secondDis = Vector3.Distance(transform.Find("Queue").transform.position, second.transform.position);

        return firstDis.CompareTo(secondDis);
    }

    public void UpdateQueue()
    {
        for (int i = 0; i < queue.Count; i++)
        {            
            if (npcQueue.Count >= queue.Count)
            {
                if (i == queue.Count - 1 && i < capacity)
                {
                    AddQueue();
                }
            }
            if (i < npcQueue.Count)
            {
                npcQueue[i].GetComponent<AIScript>().queueTarget = i;
                npcQueue[i].GetComponent<AIScript>().currentQueue = i;

                if (npcQueue[i].gameObject == null || !npcQueue[i].GetComponent<AIScript>().isQueueing)
                {
                    npcQueue.RemoveAt(i);
                    queue[i].GetComponent<PlacementScript>().occupied = false;
                }
            }
        }
        npcQueue.Sort(CompareDistance);
        for (int x = 0; x < npcQueue.Count; x++)
        {
            npcQueue[x].GetComponent<AIScript>().queueTarget = x;
            npcQueue[x].GetComponent<AIScript>().currentQueue = x;
        }

        if (npcQueue.Count >= capacity)
        {
            isFull = true;
        }
        else
        {
            isFull = false;
        }
    }
}
