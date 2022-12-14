using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Torres : MonoBehaviour
{
    StateMachineEngine FMS_Torres;

    GameManager gameManager;

    [Header("External GamObjects")]
    public GameObject bullet;
    public GameObject partToRotate;
    public GameObject bulletPos;

    [Header("Stats")]
    public int health;
    public int healthMax;
    public int range;
    public int damage;
    public float fireRate;
    float fireRateTimer;
    public int kills;
    [Space]
    public int numTowerNear;
    public int numEnemiesNear;

    [Header("Checks variables")]
    public bool enemyInRangeCheck;
    public GameObject enemyInRange;

    [Header("Particles")]
    public GameObject particleDestruction;

    void Start()
    {
        gameManager = GameManager.instance;

        healthMax = health;

        fireRateTimer = fireRate;

        CreateFMS();
    }

    void Update()
    {
        fireRateTimer += Time.deltaTime;

        health = Mathf.Min(health, healthMax);

        enemyInRangeCheck = false;
        enemyInRange = null;
        foreach (GameObject enemy in gameManager.enemies)
        {
            if (enemy)
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) < range)
                {
                    enemyInRangeCheck = true;
                    enemyInRange = enemy;

                    Vector3 dir = enemyInRange.transform.position - transform.position;
                    Quaternion lookRotation = Quaternion.LookRotation(dir);
                    Vector3 rotation = Quaternion.Lerp(partToRotate.transform.rotation, lookRotation, Time.deltaTime * 10).eulerAngles;
                    partToRotate.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

                    break;
                }
            }
        }

        RaycastHit[] towersInRange = Physics.SphereCastAll(transform.position, 8, transform.up, 0, LayerMask.GetMask("Tower"));
        numTowerNear = towersInRange.Length;
        RaycastHit[] enemiesInRange = Physics.SphereCastAll(transform.position, 8, transform.up, 0, LayerMask.GetMask("Enemy"));
        numEnemiesNear = enemiesInRange.Length;

        FMS_Torres.Update();
    }

    void CreateFMS()
    {
        FMS_Torres = new StateMachineEngine(false);

        //Creaci?n de estados
        State Ts_atacar = FMS_Torres.CreateState("atacar", T_atacando);
        State Ts_no_atacar = FMS_Torres.CreateEntryState("no atacar", T_no_atacando);
        State Ts_morir = FMS_Torres.CreateState("morir", T_muerte);

        //Creaci?n de percepciones
        Perception Tp_hay_enemigo = FMS_Torres.CreatePerception<ValuePerception>(() => enemyInRangeCheck);
        Perception Tp_no_hay_enemigo = FMS_Torres.CreatePerception<ValuePerception>(() => !enemyInRangeCheck);
        Perception Tp_sin_vida = FMS_Torres.CreatePerception<ValuePerception>(() => health <= 0);

        //Creaci?n transiciones
        //Si hay enemigo ataco
        FMS_Torres.CreateTransition("atacando", Ts_no_atacar, Tp_hay_enemigo, Ts_atacar);
        //Si no hay enemigo dejo de atacar
        FMS_Torres.CreateTransition("no atacando", Ts_atacar, Tp_no_hay_enemigo, Ts_no_atacar);
        //Si no tengo vida atacando muero
        FMS_Torres.CreateTransition("muriendo atacando", Ts_atacar, Tp_sin_vida, Ts_morir);
        //Si no tengo vida no atacando muero
        FMS_Torres.CreateTransition("muriendo no atacando", Ts_no_atacar, Tp_sin_vida, Ts_morir);
        //Volver a atacar
        FMS_Torres.CreateTransition("volver a atacar", Ts_atacar, Tp_hay_enemigo, Ts_atacar);
    }

    void T_atacando()
    {
        if (fireRateTimer >= fireRate)
        {
            print("Torre normal: Ataco");
            fireRateTimer = 0;

            GameObject instBullet = Instantiate(bullet, bulletPos.transform.position, transform.rotation);
            instBullet.GetComponent<CP_Bullet_Tower>().Seek(enemyInRange.transform);
            instBullet.GetComponent<CP_Bullet_Tower>().myTower = this;
        }
    }

    void T_no_atacando()
    {
        print("Torre normal: No ataco");
    }

    void T_muerte()
    {
        print("Torre normal: Me muero");
        Instantiate(particleDestruction, transform.position + new Vector3(0, 3, 0), transform.rotation);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ImpactDamage")
        {
            health -= 250;
        }
    }

    private void OnMouseDown()
    {
        gameManager.infoPanel.SetActive(true);
        gameManager.info.text =
            "Nombre: Torre \n" +
            "Vida: " + health + "\n" + 
            "Rango: " + range + "\n" + 
            "Da?o: " + damage + "\n" + 
            "Velocidad de disparo: " + fireRate + "\n" + 
            "Muertes: " + kills + "\n" + 
            "Enemigos cerca: " + numEnemiesNear + "\n" + 
            "Torres cerca: " + numTowerNear + "\n"
            ;
    }
}
