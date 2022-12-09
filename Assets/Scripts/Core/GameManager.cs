using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    int towerSelected;
    bool canColocate;
    bool canBossFinal = true;

    public GameObject infoPanel;
    public TextMeshProUGUI info;

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

        if ((Input.GetButtonDown("Fire1") || Input.GetButton("Fire1")) && canColocate)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 1000))
            {
                if (rayHit.collider.gameObject.layer == 10)
                {
                    canColocate = false;

                    switch (towerSelected)
                    {
                        case 0:
                            Instantiate(tower, rayHit.collider.gameObject.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                            break;
                        case 1:
                            Instantiate(hero1, rayHit.collider.gameObject.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                            break;
                        case 2:
                            Instantiate(hero2, rayHit.collider.gameObject.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                            break;
                    }
                }
            }
        }

        if (Input.GetButtonDown("Fire2") || Input.GetButton("Fire2"))
        {
            infoPanel.SetActive(false);
        }
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
        if (canBossFinal)
        {
            canBossFinal = false;
            int randomID = 0;
            Vector3 localSpawn = new Vector3(spawns[randomID].transform.position.x, 0, spawns[randomID].transform.position.z);
            GameObject instBossFinal = Instantiate(bossFinal, localSpawn, transform.rotation);
            instBossFinal.transform.rotation = Quaternion.AngleAxis(270, Vector3.up);
        }
    }

    public void SelectTower(int id)
    {
        canColocate = true;
        towerSelected = id;
    }

    public void CloseInfo()
    {
        infoPanel.SetActive(false);
    }
}
