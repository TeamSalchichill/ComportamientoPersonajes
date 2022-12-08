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
        /*
        if (Input.GetButtonDown("Fire1"))
        {
            //Instantiate(enemyMedium, spawns[Random.Range(0, spawns.Length - 1)].transform.position, transform.rotation);
            Instantiate(enemyMedium, new Vector3(spawns[0].transform.position.x, 0, spawns[0].transform.position.z), transform.rotation);
            numEnemiesMedium++;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            //Instantiate(enemySmall, spawns[Random.Range(0, spawns.Length - 1)].transform.position, transform.rotation);
            Instantiate(enemySmall, new Vector3(spawns[0].transform.position.x, 0, spawns[0].transform.position.z), transform.rotation);
        }
        if (Input.GetButtonDown("Fire3"))
        {
            //Instantiate(boss1, spawns[Random.Range(0, spawns.Length - 1)].transform.position, transform.rotation);
            Instantiate(boss1, new Vector3(spawns[0].transform.position.x, 0, spawns[0].transform.position.z), transform.rotation);
        }
        */
    }

    public void InvokeEnemySmall()
    {
        int randomID = Random.Range(1, spawns.Length);
        Vector3 localSpawn = new Vector3(spawns[randomID].transform.position.x, 0, spawns[randomID].transform.position.z);
        Instantiate(enemySmall, localSpawn, transform.rotation);
    }
    public void InvokeEnemyMedium()
    {
        int randomID = Random.Range(1, spawns.Length);
        Vector3 localSpawn = new Vector3(spawns[randomID].transform.position.x, 0, spawns[randomID].transform.position.z);
        Instantiate(enemyMedium, localSpawn, transform.rotation);
        numEnemiesMedium++;
    }
    public void InvokeBoss1()
    {
        int randomID = Random.Range(1, spawns.Length);
        Vector3 localSpawn = new Vector3(spawns[randomID].transform.position.x, 0, spawns[randomID].transform.position.z);
        Instantiate(boss1, localSpawn, transform.rotation);
    }
    public void InvokeBoss2()
    {
        int randomID = Random.Range(1, spawns.Length);
        Vector3 localSpawn = new Vector3(spawns[randomID].transform.position.x, 0, spawns[randomID].transform.position.z);
        Instantiate(boss2, localSpawn, transform.rotation);
    }
    public void InvokeBossFinal()
    {
        int randomID = 0;
        Vector3 localSpawn = new Vector3(spawns[randomID].transform.position.x, 0, spawns[randomID].transform.position.z);
        GameObject instBossFinal = Instantiate(bossFinal, localSpawn, transform.rotation);
        instBossFinal.transform.rotation = Quaternion.AngleAxis(270, Vector3.up);
    }
}
