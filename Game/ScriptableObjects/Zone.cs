using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Zone", menuName = "ScriptableObjects/Zone", order = 1)]
public class Zone : ScriptableObject
{
    [SerializeField] public GameObject[] propPrefabs;
    [SerializeField] public GameObject[] hazardPrefabs;

    //[SerializeField] public GameObject enemyPrefab;

    public Material frontPlanetMaterial;
    public Material backPlanetMaterial;
}