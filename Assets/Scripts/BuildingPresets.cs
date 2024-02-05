using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building Preset", menuName = "New Building Preset", order = 0)]
public class BuildingPresets : ScriptableObject
{
    public int cost;
    public ResourceType resourceCost;
    public int costPerTurn;
    public GameObject prefab;
    public bool isMoneyMaker;

    public int population;
    public int jobs;
    public int food;
    public int materials;

    public BuildingType buildingType;
}

public enum BuildingType
{
    Building,
    Road,
    Block
}
