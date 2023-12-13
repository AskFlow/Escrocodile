using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    private GameManager() { } 
    public static GameManager Instance { get; private set; }

    public Player player;

    public int Seed = 0;

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

    public void Win()
    {

    }

    public void Lose()
    {

    }

}
