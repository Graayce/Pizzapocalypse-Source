using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    // References
    private Transform player;
    private Rigidbody playerRb;

    //Attributes
    [SerializeField] private float force;

    // States
    private bool attractingPlayer;

    void Start()
    {
        player = GameObject.Find("Car").transform;
        playerRb = player.GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //StartCoroutine(ScalePlayer(1, 0));
            attractingPlayer = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            attractingPlayer = false;
    }

    void Update()
    {
        if (attractingPlayer)
        {
            Vector3 direction =  playerRb.transform.position - transform.position;
            playerRb.velocity = -direction * force;
        }
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
