using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{


    private GameManager() { } 
    public static GameManager Instance { get; private set; }

    public Player player;

    public int Seed = 0;
    public TextMeshProUGUI seedText;

    public delegate void EnableSoluceMode();
    public EnableSoluceMode _enableDelegateSoluce;

    public delegate void DisableSoluceMode();
    public DisableSoluceMode _disableDelegateSoluce;

    public DoorManager doorManager;
    public TMP_InputField input;

    public GameObject VictoryMenu;
    public GameObject parameterMenu;



    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);    // Suppression d'une instance précédente (sécurité...sécurité...)
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start()
    {

    }

    private void Update()
    {
        seedText.text = Seed.ToString();
    }

    public void ChangeSoluce(bool newSoluce)
    {
        if (newSoluce)
        {
            _enableDelegateSoluce();
        }
        else
        {
            _disableDelegateSoluce();
        }
    }

    public void Win()
    {
        VictoryMenu.SetActive(true);
    }

    public void Lose()
    {

    }
    
    public void ToggleParameter(bool value)
    {
        parameterMenu.SetActive(value);
    }

    public void ResetDoor()
    {
        VictoryMenu.SetActive(false);
        if(input.text == "")
        {
            int result = DateTime.Now.GetHashCode();
            UnityEngine.Random.InitState(result);
            Seed = result;
        }
        else
        {
            int result;
            int.TryParse(input.text, out result);
            Seed = result;
            UnityEngine.Random.InitState(result);

        }
        doorManager.Restart();
    }
}
