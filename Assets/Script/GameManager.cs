using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
