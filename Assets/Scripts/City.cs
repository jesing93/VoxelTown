using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class City : MonoBehaviour
{
    [Header("Resources")]
    public int money;
    public int food;
    public int materials;
    public int curJobs;
    public int curPopulation;
    public int maxJobs;
    public int maxPopulation;
    public int incomePerJob;
    public int materialsPerJob;
    [Header("Time")]
    public float curDayTime;
    private float dayTime = 24;
    private float minutes;
    public float speedFactor = 1;
    private int day;

    public TextMeshProUGUI moneyStat;
    public TextMeshProUGUI materialsStat;
    public TextMeshProUGUI foodStat;
    public TextMeshProUGUI popStat;
    public TextMeshProUGUI jobStat;
    public TextMeshProUGUI dayStat;
    public TextMeshProUGUI timeStat;

    public List<Building> buildings = new();
    public List<Building> roads = new();
    public List<Building> altBlocks = new();

    public static City Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        DayCycle();
    }

    private void DayCycle()
    {
        curDayTime += Time.deltaTime * speedFactor;
        if(curDayTime >= dayTime)
        {
            curDayTime -= dayTime;
            day++;
            NextDay();
        }

        int hour = (int)curDayTime;
        minutes += speedFactor * (Time.deltaTime * 6) * 10;
        int minutesDouble = (int)minutes;

        if (minutesDouble >= 60)
        {
            minutes = 0;
        }
        string hourString = hour.ToString("00");
        string minutesString = minutesDouble.ToString("00");
        timeStat.text = hourString + ":" + minutesString;
    }

    public void ChangeSpeed(int newSpeed)
    {
        speedFactor = newSpeed;
        CrowdManager.Instance.UpdateSpeed(newSpeed);
    }

    /// <summary>
    /// Called when a building is placed
    /// </summary>
    /// <param name="building"></param>
    public void OnPlaceBuilding(Building building)
    {
        switch (building.preset.buildingType)
        {
            case BuildingType.Building:
                buildings.Add(building);
                break;
            case BuildingType.Road:
                roads.Add(building);
                break;
            case BuildingType.Block:
                altBlocks.Add(building);
                WorldManager.Instance.RegenerateNavMesh();
                break;
        }
        if(building.preset.resourceCost == ResourceType.money)
        {
            money -= building.preset.cost;
        }
        else if (building.preset.resourceCost == ResourceType.material)
        {
            materials -= building.preset.cost;
        }
        maxPopulation += building.preset.population;
        maxJobs += building.preset.jobs;
        UpdateStatsText();
    }

    /// <summary>
    /// Called when a building is destroyed
    /// </summary>
    /// <param name="building"></param>
    public void OnRemoveBuilding(Building building)
    {
        switch (building.preset.buildingType)
        {
            case BuildingType.Building:
                buildings.Remove(building);
                break;
            case BuildingType.Road:
                roads.Remove(building);
                break;
            case BuildingType.Block:
                altBlocks.Remove(building);
                WorldManager.Instance.RegenerateNavMesh();
                break;
        }

        maxPopulation-= building.preset.population;
        maxJobs-= building.preset.jobs;

        Destroy(building.gameObject);
        UpdateStatsText();
    }

    private void UpdateStatsText()
    {
        moneyStat.text = string.Format("Money: {0}€", money);
        materialsStat.text = string.Format("Materials: {0}", materials);
        foodStat.text = string.Format("Food: {0}", food);
        popStat.text = string.Format("Population: {0}/{1}", new object[2] {curPopulation, maxPopulation});
        jobStat.text = string.Format("Jobs: {0}/{1}", new object[2] { curJobs, maxJobs });
        dayStat.text = string.Format("Day: {0}", day);
    }

    public void NextDay()
    {
        CalculateMoney();
        CalculateMaterials();
        CalculateFood();
        CalculatePop();
        CalculateJobs();

        UpdateStatsText();

        CrowdManager.Instance.maxCrowd = buildings.Count * 5;
    }

    private void CalculateFood()
    {
        foreach (Building building in buildings)
        {
            food += building.preset.food;
        }
    }

    private void CalculateJobs()
    {
        curJobs = Mathf.Min(curPopulation, maxJobs);
    }

    private void CalculatePop()
    {
        if (food < curPopulation)
        {
            curPopulation = food;
            food -= curPopulation;
        }
        else
        {

            food -= curPopulation;
            if (curPopulation < maxPopulation)
            {
                curPopulation = (int)Mathf.Min(maxPopulation, Mathf.Max(curPopulation * 1.25f, curPopulation + 1));
            }
        }
    }

    private void CalculateMoney()
    {
        money += curJobs * incomePerJob;
        foreach(Building building in buildings)
        {
            if (building.preset.isMoneyMaker) {
                money += building.preset.jobs * incomePerJob;
            }
            else
            {
                money -= building.preset.jobs * incomePerJob;
            }
            money -= building.preset.costPerTurn;
        }
        foreach(Building building in roads)
        {
            money -= building.preset.costPerTurn;
        }
    }
    private void CalculateMaterials()
    {
        foreach (Building building in buildings)
        {
            materials += building.preset.materials;
        }
    }
}

public enum ResourceType
{
    money,
    material,
    food
}
