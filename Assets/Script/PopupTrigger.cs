using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTrigger : MonoBehaviour
{
    [SerializeField] private Door parentDoor;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        parentDoor.OnDisplayPopup();
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        parentDoor.OnHidePopup();
    }
}
