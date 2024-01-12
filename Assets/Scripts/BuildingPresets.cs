using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building Preset", menuName = "New Building Preset", order = 0)]
public class BuildingPresets : ScriptableObject
{
    public int cost;
    public int costPerTurn;
    public GameObject prefab;

    public int population;
    public int jobs;
    public int food;
}
