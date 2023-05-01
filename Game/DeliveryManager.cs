using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    // References
    private Transform player;
    private AudioSource soundEffect;
    [SerializeField] private Transform[] locations;
    [SerializeField] private Transform deliveryPoint;
    [SerializeField] private Transform pizzaBox;
    private Transform currentLocation;
    private Transform deliveryLocation;

    // Attributes/Data
    [SerializeField] private float timeForDelivery;
    private float currentTime;
    private int deliveriesCompleted;

    // Events
    public delegate void UpdateUI();
    public event UpdateUI NewLocation;
    public delegate void GameFailed();
    public event GameFailed gameFailed;

    // States
    private bool failed;

    // Singleton
    public static DeliveryManager instance;

    // Getters/Setters😩
    public Transform CurrentLocation { get { return deliveryLocation; } }
    public int DeliveriesCompleted { get {return deliveriesCompleted; } }
    public float TimeForDelivery { get {return timeForDelivery; } }
    public float CurrentTime { get {return currentTime; } }
    public bool Failed { get { return failed; } }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        Transform container = GameObject.Find("Locations").transform;
        locations = new Transform[container.childCount];

        for (int i = 0; i < locations.Length; i++)
            locations[i] = container.GetChild(i);

        currentTime = timeForDelivery;

        player = GameObject.Find("Car").transform;
        soundEffect = GetComponent<AudioSource>();
    }

    void Start()
    {
        PickNewLocation();
    }

    void Update()
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
            Fail();
    }

    public void CompleteDelivery()
    {
        deliveriesCompleted++;
        currentTime = timeForDelivery;
        Events.instance.CauseEvent();
        Rigidbody pizzaRb = Instantiate(pizzaBox, player.position, pizzaBox.rotation).GetComponent<Rigidbody>();
        pizzaRb.AddForce((currentLocation.position - pizzaRb.transform.position).normalized * 10);
        soundEffect.Play();
        PickNewLocation();
    }

    private void PickNewLocation()
    {
        int index = Random.Range(0, locations.Length);

        while (locations[index] == currentLocation)
            index = Random.Range(0, locations.Length);

        currentLocation = locations[index];

        Transform point = currentLocation.Find("Point");
        deliveryLocation = point;

        Instantiate(deliveryPoint, point.position, deliveryPoint.rotation, point);

        NewLocation?.Invoke();
    }

    public void Fail()
    {
        if (failed) { return; }

        currentTime = 0;
        failed = true;

        StartCoroutine(ScalePlayer(1, 0));

        gameFailed?.Invoke();
    }

    private IEnumerator ScalePlayer(float from, float to)
    {
        float difference = to - from;

        for (float t = 0; t <= 1; t += Time.deltaTime)
        {
            float scale = from + (t * difference);
            player.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        player.localScale = new Vector3(to, to, to);
    }
}
