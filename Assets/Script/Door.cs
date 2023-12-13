using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEditor.Progress;

public class Door : MonoBehaviour
{
    public int Id = 0;
    public int DoorDepth = 0;
    public List<int> previousDoors = new List<int>();
    public List<Door> previousDoorsComponent = new List<Door> ();
    public TMPro.TextMeshProUGUI m_TextMeshPro;
    public bool isLastDoor = false;
    public Light2D _light;
    public bool doorPassed;

    public SpriteRenderer obj;

    public List<Color> depthColor;

    private void Update()
    {
        m_TextMeshPro.text = Id.ToString();
        obj.color = depthColor[DoorDepth];
        foreach (var item in previousDoorsComponent)
        {
            
        }
    }

    public void OnPlayerEnter(Player player)
    {
        if(player.currentKey == Id)
        {
            if(isLastDoor)
            {
                GameManager.Instance.Win();
            }
            else
            {
                //player.currentKey = NextDoorId;
            }
        }
        else
        {
            player.Die();
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var item in previousDoorsComponent)
        {
            Gizmos.color = depthColor[DoorDepth];
            Gizmos.DrawLine(gameObject.transform.position, item.gameObject.transform.position);
        }
    }
}
