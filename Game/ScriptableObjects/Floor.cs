using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Floor", menuName = "ScriptableObjects/Floor", order = 1)]
public class Floor : ScriptableObject
{
    public Zone zone;

    [SerializeField] int[] propCounts;
    [SerializeField] int[] hazardCounts;

    //[SerializeField] int enemyCount;

    [SerializeField] int entryCount = 1;

    public float timeBetweenMeteorites = 5;

    public int totalCount { get { return propCounts.Sum() + hazardCounts.Sum() +/*enemyCount+*/entryCount; } }

    public GameObject[] totalPrefabs
    {
        get
        {
            return ((((zone.propPrefabs).MultiplyEach(propCounts)).ToList()).Concat((zone.hazardPrefabs).MultiplyEach(hazardCounts))).ToArray();
        }
    }
}

// zone.propPrefabs.MultiplyEach(propCounts)
// zone.hazardPrefabs.MultiplyEach(hazardCounts)
// zone.(new GameObject [enemyPrefab]).MultiplyEach(new int [enemyCount])