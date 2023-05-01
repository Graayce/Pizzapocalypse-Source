using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailOnTouch : MonoBehaviour
{
    [SerializeField] private bool destroy;

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DeliveryManager.instance.Fail();
            if (destroy)
                Destroy(this.gameObject);
        }
    }
}
