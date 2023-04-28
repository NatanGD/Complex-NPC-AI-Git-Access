using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beverage : MonoBehaviour
{
    public enum BeverageType
    {
        FOOD = 0, 
        DRINK = 1
    }

    public BeverageType beverageType;
}
