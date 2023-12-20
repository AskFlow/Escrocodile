using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{


    private GameManager() { } 
    public static GameManager Instance { get; private set; }

    public Player player;

    public int Seed = 0;

    public delegate void EnableSoluceMode();
    public EnableSoluceMode _enableDelegateSoluce;

    public delegate void DisableSoluceMode();
    public DisableSoluceMode _disableDelegateSoluce;

    public DoorManager doorManager;
    public TextMeshProUGUI input;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);    // Suppression d'une instance pr�c�dente (s�curit�...s�curit�...)
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start()
    {

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

    }

    public void Lose()
    {

    }
    public void ResetDoor()
    {
        if(input.text == "")
        {
            UnityEngine.Random.InitState(DateTime.Now.GetHashCode());
        }
        else
        {
            int result = Int32.Parse(input.text);
            UnityEngine.Random.InitState(result);

        }
        doorManager.Restart();
    }
}
