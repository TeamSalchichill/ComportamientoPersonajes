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

    public int totalKilss;
    public bool help;
    public List<GameObject> enemiesHelp;
    public int numEnemiesMedium;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        towers = GameObject.FindGameObjectsWithTag("Tower");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (Input.GetButtonDown("Fire1"))
        {
            //Instantiate(enemyMedium, spawns[Random.Range(0, spawns.Length - 1)].transform.position, transform.rotation);
            Instantiate(enemyMedium, spawns[0].transform.position, transform.rotation);
            numEnemiesMedium++;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            //Instantiate(enemySmall, spawns[Random.Range(0, spawns.Length - 1)].transform.position, transform.rotation);
            Instantiate(enemySmall, spawns[0].transform.position, transform.rotation);
        }
        if (Input.GetButtonDown("Fire3"))
        {
            //Instantiate(boss1, spawns[Random.Range(0, spawns.Length - 1)].transform.position, transform.rotation);
            Instantiate(boss2, spawns[0].transform.position, transform.rotation);
        }
    }
}
