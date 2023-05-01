using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{
    // References
    private Transform[] roads;

    // Attributes
    private const int NUM_OF_EVENTS = 2;
    private const int MAX_METEOR_SHOWERS = 5;

    // Meteor
    [Header("Meteor")]
    [SerializeField] private Transform meteor;
    [SerializeField] private float timeBetweenMeteors;
    [SerializeField] private float meteorSpeed;
    [SerializeField] private float meteorHeight;
    [SerializeField] private float meteorAngle;
    private float currentMeteorTime;
    private int meteorShowers;

    [Header("Black Hole")]
    [SerializeField] private Transform blackHole;
    [SerializeField] private float blackHoleHeight;

    // Singleton
    public static Events instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);
    }

    void Start()
    {
        Transform rs = GameObject.Find("Roads").transform;
        roads = new Transform[rs.childCount];

        for (int i = 0; i < rs.childCount; i++)
            roads[i] = rs.GetChild(i);

        currentMeteorTime = timeBetweenMeteors;
    }

    void Update()
    {
        currentMeteorTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F))
            meteorShowers++;

        if (Input.GetKeyDown(KeyCode.G))
            SpawnBlackHole();

        if (currentMeteorTime >= timeBetweenMeteors && meteorShowers > 0)
        {
            for (int i = 0; i < meteorShowers; i++)
            {
                SpawnMeteor();
            }

            currentMeteorTime = 0;
        }
    }

    public void CauseEvent()
    {
        int low = meteorShowers == MAX_METEOR_SHOWERS ? 1 : 0;

        int e = Random.Range(low, NUM_OF_EVENTS);

        switch (e)
        {
            case 0:
            {
                meteorShowers++;
                break;
            }
            case 1:
            {
                SpawnBlackHole();
                break;
            }
        };
    }

    public Vector3 PickRandomPoint()
    {
        Transform road = roads[Random.Range(0, roads.Length)];

        Vector3 scale = road.localScale;
        Vector3 pos = road.position;

        float distZ = scale.z * 5;
        float distX = scale.x * 5;

        float upperZ = pos.z + distZ;
        float lowerZ = pos.z - distZ;

        float upperX = pos.x + distX;
        float lowerX = pos.x - distX;

        float x = Random.Range(lowerX, upperX);
        float z = Random.Range(lowerZ, upperZ);

        return new Vector3(x, 0, z);
    }

    private void SpawnMeteor()
    {
        Vector3 point = PickRandomPoint();
        point.y = meteorHeight;

        float x = Random.Range(-meteorAngle, meteorAngle);
        float y = Random.Range(-meteorAngle, meteorAngle);

        Quaternion rotation = Quaternion.Euler(-90 + x, y, 0);

        Transform m = Instantiate(meteor, point, rotation);
        Rigidbody meteorRb = m.GetComponent<Rigidbody>();

        meteorRb.velocity = -m.forward * meteorSpeed;
    }

    private void SpawnBlackHole()
    {
        Vector3 point = PickRandomPoint();
        point.y = blackHoleHeight;

        Instantiate(blackHole, point, blackHole.rotation);
    }
}
