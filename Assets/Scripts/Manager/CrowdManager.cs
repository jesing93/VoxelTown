using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    public int maxCrowd = 0;
    private int currentCrowd = 0;
    private int speedFactor = 1;
    private List<GameObject> civilians = new();

    public GameObject civilianPref;

    public static CrowdManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void FixedUpdate()
    {
        //If crowd can be generated, do it
        if (currentCrowd < maxCrowd && City.Instance.buildings.Count > 1)
        {
            //Max 5 civilians per frame
            int crowdToGenerate = Mathf.Min(5, maxCrowd - currentCrowd);
            for (int i = 0; i < crowdToGenerate; i++)
            {
                //Generate origin and target
                int originIndex = Random.Range(0, City.Instance.buildings.Count);
                Vector3 origin = City.Instance.buildings[originIndex].transform.position;
                int targetIndex = originIndex;
                while (targetIndex == originIndex)
                {
                    targetIndex = Random.Range(0, City.Instance.buildings.Count);
                }
                Vector3 target = City.Instance.buildings[targetIndex].transform.position;
                //Instance and start civilian AI
                GameObject civilian = Instantiate(civilianPref, origin, Quaternion.identity, transform);
                civilians.Add(civilian);
                civilian.GetComponent<AIControl>().GoTo(target, speedFactor);
                currentCrowd++;
            }
        }
    }

    /// <summary>
    /// Destroy civilian on target reach
    /// </summary>
    /// <param name="civilian"></param>
    public void TargetReached(GameObject civilian)
    {
        civilians.Remove(civilian);
        Destroy(civilian);
        currentCrowd--;
        //TODO: Object pool
    }

    /// <summary>
    /// Update the agents speed
    /// </summary>
    /// <param name="newSpeed"></param>
    public void UpdateSpeed(int newSpeed)
    {
        speedFactor = newSpeed;
        foreach (GameObject civilian in civilians)
        {
            civilian.GetComponent<AIControl>().UpdateSpeed(newSpeed);
        }
    }
}
