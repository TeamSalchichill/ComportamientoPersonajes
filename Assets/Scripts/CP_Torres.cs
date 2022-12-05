using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Torres : MonoBehaviour
{
    StateMachineEngine FMS_Torres;

    GameManager gameManager;

    [Header("External GamObjects")]
    public GameObject bullet;

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
                    break;
                }
            }
        }

        ////////////////////////////////////

        RaycastHit[] towersInRange = Physics.SphereCastAll(transform.position, 8, transform.up, 0, LayerMask.GetMask("Tower"));
        numTowerNear = towersInRange.Length;
        RaycastHit[] enemiesInRange = Physics.SphereCastAll(transform.position, 8, transform.up, 0, LayerMask.GetMask("Enemy"));
        numEnemiesNear = enemiesInRange.Length;

        ////////////////////////////////////

        FMS_Torres.Update();
    }

    void CreateFMS()
    {
        FMS_Torres = new StateMachineEngine(false);

        //Creación de estados
        State Ts_atacar = FMS_Torres.CreateState("atacar", T_atacando);
        State Ts_no_atacar = FMS_Torres.CreateEntryState("no atacar", T_no_atacando);
        State Ts_morir = FMS_Torres.CreateState("morir", T_muerte);

        //Creación de percepciones
        Perception Tp_hay_enemigo = FMS_Torres.CreatePerception<ValuePerception>(() => enemyInRangeCheck);
        Perception Tp_no_hay_enemigo = FMS_Torres.CreatePerception<ValuePerception>(() => !enemyInRangeCheck);
        Perception Tp_sin_vida = FMS_Torres.CreatePerception<ValuePerception>(() => health <= 0);

        //Creación transiciones
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
        print("Recargando");

        if (fireRateTimer >= fireRate)
        {
            print("Ataco");
            fireRateTimer = 0;

            GameObject instBullet = Instantiate(bullet, transform.position, transform.rotation);
            instBullet.GetComponent<CP_Bullet_Tower>().Seek(enemyInRange.transform);
            instBullet.GetComponent<CP_Bullet_Tower>().myTower = this;
        }
    }

    void T_no_atacando()
    {
        print("No ataco");
    }

    void T_muerte()
    {
        print("Hasta luego");
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ImpactDamage")
        {
            health -= 250;
        }
    }
}
