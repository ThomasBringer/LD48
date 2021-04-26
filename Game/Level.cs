using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Level : MonoBehaviour
{
    [SerializeField] float firstRadius = 5;
    [SerializeField] float finalRadius = 2;
    [SerializeField] float skyRadiusMultiplier = 2;
    [SerializeField] float minSkyRadius = 6;
    [SerializeField] float levelTransitionTime = 1;
    /*[SerializeField]*/
    int levelCount; // = 10;

    [SerializeField] float spawnDistance = 1.5f;

    [SerializeField] Floor[] floors;

    float radiusStep;

    [HideInInspector] public int floorCount = 0;
    [HideInInspector] public float radius;

    [HideInInspector] public Vector3[] entries;

    [SerializeField] GameObject planetPrefab;
    GameObject planetToDelete;
    GameObject lastPlanet;
    GameObject currentPlanet;

    [SerializeField] GameObject meteoritePrefab;
    [SerializeField] float meteoriteCircleSpawnRadius = 5;

    Player player;
    VirtualCamera vcam;



    [SerializeField] Transform uiPawn;
    [SerializeField] TextMeshProUGUI depthText;
    // float uiPawnStartPos;

    void Awake()
    {
        levelCount = floors.Length;
        radiusStep = (firstRadius - finalRadius) / levelCount;
        radius = firstRadius;

        vcam = FindObjectOfType<VirtualCamera>();
        player = FindObjectOfType<Player>();

        // uiPawnStartPos = uiPawn.localPosition.y;
        // print(uiPawnStartPos);
        //entries = GenerateEntries(5);
    }

    void Start()
    {
        SpawnPlanet();
    }

    // Vector3[] GenerateEntries(int count)
    // {
    //     Vector3[] theseEntries = new Vector3[count];
    //     for (int i = 0; i < count; i++)
    //         theseEntries[i] = Random.onUnitSphere;
    //     return theseEntries;
    // }

    List<Vector3> planetSpots;

    void MakeFloor(Floor floor)
    {
        planetSpots = GenerateSeparatePoints(floor.totalCount).ToList();

        GameObject[] prefabs = floor.totalPrefabs;
        int prefabCount = prefabs.Length;

        for (int i = 0; i < prefabCount; i++)
        {
            SpawnOnPlanet(prefabs[i], planetSpots[i + 1]);
        }

        var newEntries = new List<Vector3>();
        for (int i = prefabCount; i < planetSpots.Count - 1; i++)
        {
            newEntries.Add(planetSpots[i + 1]);
        }
        entries = newEntries.ToArray();

        planetSpots.RemoveAt(0);

        //foreach (var e in entries) print("entry " + e);
    }

    void SpawnOnPlanet(GameObject prefab, Vector3 spherePos)
    {
        Transform instance = Instantiate(prefab).transform;

        instance.position = spherePos * radius;
        instance.up = spherePos;

        // instance.up = spherePos;
        // instance.Translate(radius * Vector3.up, Space.Self);

        instance.SetParent(currentPlanet.transform);
    }

    Vector3[] GenerateSeparatePoints(int count)
    {
        var points = new List<Vector3>();
        points.Add(player.playerChild.position.normalized);
        for (int i = 1; i <= count; i++)
        {
            points.Add(FindSeparatePoint(points.ToArray()));
        }
        return points.ToArray();
    }

    Vector3 FindSeparatePoint(Vector3[] points)
    {
        Vector3 newPoint;
        do
        {
            newPoint = Random.onUnitSphere;
        }
        while (!PossibleSpawn(points, newPoint));

        return newPoint;
    }

    bool PossibleSpawn(Vector3[] points, Vector3 newPoint)
    {
        foreach (Vector3 point in points)
        {
            if (Vector3.Distance(point * radius, newPoint * radius) < spawnDistance) return false;
        }
        return true;
    }

    // void OnEnable()
    // {
    //     entries = GenerateEntries(5);
    // }

    public void Enter()
    {
        floorCount++;

        uiPawn.Translate(Vector3.down * 90 / (levelCount - 1), Space.Self);
        depthText.text = Mathf.RoundToInt(1000 * (1.0f - ((float)floorCount) / ((float)(levelCount - 1)))) + " m";

        // print(levelCount - 1);
        // print(floorCount);
        // print(floorCount / (levelCount - 1));
        // print(1000 * (1.0f - floorCount / (levelCount - 1)));

        radius -= radiusStep;

        planetToDelete = lastPlanet;
        lastPlanet = currentPlanet;

        vcam.AdaptOffset();

        SpawnPlanet();
        StartCoroutine(ScalePlanet());

        //entries = GenerateEntries(5);
    }

    void SpawnPlanet()
    {
        currentPlanet = Instantiate(planetPrefab);
        currentPlanet.GetComponent<MeshRenderer>().material = CurrentFloor.zone.backPlanetMaterial;
        currentPlanet.transform.GetChild(0).GetComponent<MeshRenderer>().material = CurrentFloor.zone.frontPlanetMaterial;
        currentPlanet.Scalate(radius);
        //currentPlanet.transform.localScale = Vector3.one * radius;
        MakeFloor(CurrentFloor);
    }

    IEnumerator ScalePlanet()
    {
        float scale = radius + radiusStep;
        float finalScale = Mathf.Clamp(scale * skyRadiusMultiplier, minSkyRadius, float.MaxValue);

        float percentage = 0;
        float speed = 1 / levelTransitionTime;

        lastPlanet.transform.GetChild(0).gameObject.SetActive(true);

        lastPlanet.GetComponent<MeshRenderer>().enabled = true;
        MeshRenderer rdrr = lastPlanet.transform.GetChild(0).GetComponent<MeshRenderer>();

        while (percentage < 1)
        {
            percentage += speed * Time.deltaTime;
            lastPlanet.Scalate(scale + (finalScale - scale) * percentage);

            Color color = rdrr.material.color;
            rdrr.material.color = new Color(color.r, color.g, color.b, 1 - percentage);

            yield return null;
        }

        player.EndEnter();

        lastPlanet.Scalate(finalScale);

        //rdrr.enabled = false;
        foreach (Transform child in lastPlanet.transform)
            Destroy(child.gameObject);
        Destroy(planetToDelete);

        StartCoroutine(SpawnMeteorites());

        // float scale = radius + radiusStep;
        // float speed = (skyRadius - scale) / levelTransitionTime;
        // while (scale < skyRadius)
        // {
        //     scale += speed * Time.deltaTime;
        //     lastPlanet.Scalate(scale);
        //     yield return null;
        // }
        // lastPlanet.Scalate(skyRadius);
        // Destroy(planetToDelete);
        // //lastPlanet.transform.localScale=Vector3.one*skyRadius;

        if (FinalLevel) StartCoroutine(Win());
    }

    public bool FinalLevel { get { return floorCount == levelCount - 1; } }

    [SerializeField] float timeAfterWin = 6;
    /*[SerializeField]*/
    string menuTextAfterWin = "Congrats, you reached the crystal core!\nThanks for playing!\nMade in 48 hours by Thomas Bringer for Ludum Dare 48 (Theme: Deeper and deeper)";

    IEnumerator Win()
    {
        MenuText(menuTextAfterWin);
        yield return new WaitForSeconds(timeAfterWin);
        Utilities.LoadPreviousScene();
    }

    Floor CurrentFloor { get { return floors[floorCount]; } }

    IEnumerator SpawnMeteorites()
    {
        int savedFloorCount = floorCount;
        float delay = CurrentFloor.timeBetweenMeteorites;

        yield return new WaitForSeconds(delay);

        while (floorCount == savedFloorCount)
        {
            SpawnMeteorite();
            yield return new WaitForSeconds(delay);
        }
    }

    void SpawnMeteorite()
    {
        Vector3 meteoritePoint = FindMeteoritePoint();
        if (meteoritePoint != Vector3.zero) SpawnOnPlanet(meteoritePrefab, meteoritePoint);
    }

    Vector3 FindMeteoritePoint()
    {
        Vector3 playerPos = player.playerChild.position;
        Vector3 newPoint;

        int i = 0;
        do
        {
            Vector2 inCircle = Random.insideUnitCircle * meteoriteCircleSpawnRadius;

            newPoint = (playerPos + player.playerChild.TransformDirection(Vector3.right * inCircle.x + Vector3.forward * inCircle.y)).normalized;

            i++;
            if (i >= 100)
            {
                print("Failed to find meteorite spot after 100 iteratons");
                return Vector3.zero;
            }
        }
        while (!PossibleSpawn(planetSpots.ToArray(), newPoint));

        planetSpots.Add(newPoint);
        return newPoint;
    }

    void MenuText(string text)
    {
        Save.menuText = text;

        // PlayerPrefs.SetString("MenuText", text);
        // PlayerPrefs.Save();
    }
}