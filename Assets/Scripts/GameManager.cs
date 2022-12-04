using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("External GameObjects")]
    public GameObject mainTower;
    public GameObject[] spawns;
    [Space]
    public GameObject enemySmall;
    public GameObject enemyMedium;
    public GameObject boss1;
    public GameObject boss2;
    public GameObject bossFinal;
    public GameObject tower;
    public GameObject hero1;
    public GameObject hero2;

    [Header("Global variables")]
    public GameObject[] towers;
    public GameObject[] enemies;

    public bool help;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        towers = GameObject.FindGameObjectsWithTag("Tower");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (Input.GetButtonDown("Fire1"))
        {
            //Instantiate(enemyMedium, spawns[Random.Range(0, spawns.Length - 1)].transform.position, transform.rotation);
            Instantiate(enemyMedium, spawns[0].transform.position, transform.rotation);
        }
    }
}
