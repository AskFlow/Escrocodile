using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollider : MonoBehaviour
{
    [SerializeField] private Door parentDoor;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        parentDoor.OnPlayerEnter(collision.gameObject.GetComponent<Player>());

    }
}
