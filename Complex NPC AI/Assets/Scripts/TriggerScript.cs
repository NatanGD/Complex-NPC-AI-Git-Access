using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC") &&
            other.GetComponent<AIScript>().isToileting)
        {
            if (other.GetComponent<AIScript>().gender == AIScript.Gender.MAN && transform.name == "ManPoint")
            {
                other.GetComponent<AIScript>().ToiletBreak();
            }
            else if (other.GetComponent<AIScript>().gender == AIScript.Gender.WOMAN && transform.name == "WomanPoint")
            {
                other.GetComponent<AIScript>().ToiletBreak();
            }
        }

            if (other.CompareTag("NPC") && this.transform.gameObject.CompareTag("Bin") &&
            other.GetComponent<AIScript>().isBinning)
        {
            other.GetComponent<AIScript>().hasBinned = true;
            other.GetComponent<AIScript>().isBinning = false;
            other.GetComponent<AIScript>().collectedBeverage = false;

            Destroy(other.GetComponent<AIScript>().beverageObject);
        }

        if (other.CompareTag("NPC") && this.transform.gameObject.CompareTag("Chair")
            && other.GetComponent<AIScript>().collectedBeverage && !other.GetComponent<AIScript>().isLeaving)
        {
            other.GetComponent<AIScript>().isBeveraging = true;
            other.GetComponent<AIScript>().startAction = true; 
            //this.transform.parent.parent.GetComponent<BeverageSource>().currentStaying++;
        }
        if (other.CompareTag("NPC") && this.transform.gameObject.CompareTag("QueuePoint")
            && !other.GetComponent<AIScript>().collectedBeverage)
        {
            this.transform.GetComponent<PlacementScript>().occupied = true;
        }
        if (other.CompareTag("NPC") && this.transform.gameObject.CompareTag("OrderPoint")
            && other.GetComponent<AIScript>().isQueueing && !other.GetComponent<AIScript>().collectedBeverage)
        {
            other.GetComponent<AIScript>().startCollection = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC") && this.transform.gameObject.CompareTag("Bin") &&
            other.GetComponent<AIScript>().isLeaving)
        {

        }

        if (other.CompareTag("NPC") && this.transform.gameObject.CompareTag("Chair") &&
            transform.GetComponent<PlacementScript>().currentNPC  != null && 
            other.name == transform.GetComponent<PlacementScript>().currentNPC.name)
        {
            other.GetComponent<AIScript>().isBeveraging = false;
            //this.transform.parent.parent.GetComponent<BeverageSource>().currentStaying--;
            this.transform.GetComponent<PlacementScript>().occupied = false;
            this.transform.GetComponent<PlacementScript>().currentNPC = null;
        }
        if (other.CompareTag("NPC") && this.transform.gameObject.CompareTag("QueuePoint") &&
            other.GetComponent<AIScript>().isQueueing && !other.GetComponent<AIScript>().collectedBeverage)
        {
            this.transform.GetComponent<PlacementScript>().occupied = false;
        }
    }
}
