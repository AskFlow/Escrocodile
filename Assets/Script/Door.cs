using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEditor.Progress;

public class Door : MonoBehaviour
{
    public int Id = 0;
    public int DoorDepth = 0;
    public List<int> previousDoors = new List<int>();
    public List<Door> previousDoorsComponent = new List<Door> ();
    public List<GameObject> lineRendererList = new List<GameObject>();
    public SpriteRenderer obj;
    public bool isLastDoor = false;
    public bool doorPassed;
    public bool isDoorOpenable;
    public GameObject Popup;
    public GameObject conditonPrefab;
    public GameObject prefabParent;

    [Space(100)]

    public Light2D _light;
    public GameObject lineRendererParent;
    public GameObject lineRenderer;
    public TMPro.TextMeshProUGUI m_TextMeshPro;
    public TMPro.TextMeshProUGUI m_TextOpenableCondition;
    public List<Color> depthColor;

    //pathfinding

    [HideInInspector] public Door parentDoor;
    [HideInInspector] public int gCost;
    [HideInInspector] public int hCost;
    public int FCost { get { return gCost + hCost; } }

    private void OnEnable()
    {
        GameManager.Instance._enableDelegateSoluce += EnableSoluce;
        GameManager.Instance._disableDelegateSoluce += DisableSoluce;
    }

    private void OnDisable()
    {
        GameManager.Instance._enableDelegateSoluce -= EnableSoluce;
        GameManager.Instance._disableDelegateSoluce -= DisableSoluce;

    }
    private void Update()
    {
        m_TextMeshPro.text = Id.ToString();
        obj.color = depthColor[DoorDepth];

        foreach (var item in previousDoorsComponent)
        {
            if (item.doorPassed)
            {
                isDoorOpenable = true;
            }
        }

        if (isDoorOpenable)
        {
            _light.enabled = true;
            _light.color = Color.green;
        }
        if (doorPassed)
        {
            _light.enabled = false;
        }
    }

    public void Setup()
    {
        foreach (var item in previousDoorsComponent)
        {
            LineRenderer lr = Instantiate(lineRenderer, lineRendererParent.transform).GetComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, item.transform.position);
            lr.startColor = new Color(depthColor[DoorDepth].r, depthColor[DoorDepth].g, depthColor[DoorDepth].b, 0.2f);
            lr.endColor = new Color(depthColor[item.DoorDepth].r, depthColor[item.DoorDepth].g, depthColor[item.DoorDepth].b, 0.2f);

            lr.gameObject.SetActive(false);
            lineRendererList.Add(lr.gameObject);
        }
        PopupCondition();
    }

    public void OnPlayerEnter(Player player)
    {
        if (isDoorOpenable)
        {
            doorPassed = true;
        }
        else
        {
            player.Die();
        }
    }


    public void EnableSoluce()
    {
        foreach (var item in lineRendererList)
        {
            item.gameObject.SetActive(true);
        }
    }

    public void DisableSoluce()
    {
        foreach (var item in lineRendererList)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void OnDisplayPopup()
    {
        Popup.SetActive(true);
    }

    public void OnHidePopup()
    {
        Popup.SetActive(false);
    }


    public void PopupCondition()
    {
        foreach (var item in previousDoors)
        {
            GameObject tempVar = Instantiate(conditonPrefab, prefabParent.transform);
            tempVar.GetComponent<SetText>().Setup(item);
        }
    }

}
