using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CP_EnemigoMediano : MonoBehaviour
{
    StateMachineEngine FMS_EnMediano;

    GameManager gameManager;

    [Header("Components")]
    public NavMeshAgent nav;

    [Header("Stats")]
    public int health;
    public int damage;
    public int rangeDetect;
    public int rangeAttack;
    public float hitRate;
    float hitRateTimer;
    public int speed;

    [Header("Checks variables")]
    public bool towerInRangeCheck;
    public GameObject towerInRange;

    public bool enemyInRangeCheck;
    public GameObject enemyInRange;
    [Space]
    public GameObject enemySmall;
    Vector3 enemySmallPos;

    void Start()
    {
        gameManager = GameManager.instance;

        nav = GetComponent<NavMeshAgent>();
        //nav.destination = gameManager.mainTower.transform.position;
        nav.SetDestination(Vector3.zero);
        nav.speed = speed;

        hitRateTimer = hitRate;

        CreateFMS();
    }

    void Update()
    {
        hitRateTimer += Time.deltaTime;

        towerInRangeCheck = false;
        towerInRange = null;
        foreach (GameObject tower in gameManager.towers)
        {
            if (Vector3.Distance(transform.position, tower.transform.position) < rangeDetect)
            {
                towerInRangeCheck = true;
                towerInRange = tower;
                break;
            }
        }
        enemyInRangeCheck = false;
        enemyInRange = null;
        foreach (GameObject enemy in gameManager.enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < rangeDetect)
            {
                enemyInRangeCheck = true;
                enemyInRange = enemy;
                break;
            }
        }

        if (enemySmall)
        {
            if (Vector3.Distance(transform.position, enemySmallPos) < 3)
            {
                enemySmall.GetComponent<CP_EnemigoEnano>().enemyMedium = null;
                enemySmall = null;
                nav.SetDestination(Vector3.zero);
            }
        }

        if (Vector3.Distance(transform.position, Vector3.zero) < 3)
        {
            print("Llegué");
            gameManager.numEnemiesMedium--;
            Destroy(gameObject);
        }

        FMS_EnMediano.Update();
    }

    void CreateFMS()
    {
        FMS_EnMediano = new StateMachineEngine(false);

        //Creación de estados
        State EMs_avanzar = FMS_EnMediano.CreateEntryState("avanzar", EM_avanzar);
        State EMs_cambiar_camino = FMS_EnMediano.CreateState("cambiar camino", EM_cambiar_camino);
        State EMs_atacar = FMS_EnMediano.CreateState("atacar", EM_atacar);
        State EMs_morir = FMS_EnMediano.CreateState("morir", EM_morir);

        //Creación de percepciones
        Perception EMp_pequeños_llaman = FMS_EnMediano.CreatePerception<ValuePerception>(() => gameManager.enemiesHelp.Count > 0);
        Perception EMp_hay_torreta = FMS_EnMediano.CreatePerception<ValuePerception>(() => towerInRangeCheck);
        Perception EMp_no_hay_torreta = FMS_EnMediano.CreatePerception<ValuePerception>(() => !towerInRangeCheck);
        Perception EMp_sin_vida = FMS_EnMediano.CreatePerception<ValuePerception>(() => health <= 0);
        Perception EMp_sin_enano = FMS_EnMediano.CreatePerception<ValuePerception>(() => !enemySmall, () => !towerInRange);

        //Creación transiciones
        //Si me llaman enemigos pequeños mientras avanzo cambio de camino
        FMS_EnMediano.CreateTransition("cambio direccion", EMs_avanzar, EMp_pequeños_llaman, EMs_cambiar_camino);

        //Si hay torre en rango ataco
        FMS_EnMediano.CreateTransition("ataco", EMs_avanzar, EMp_hay_torreta, EMs_atacar);
        FMS_EnMediano.CreateTransition("ataco2", EMs_cambiar_camino, EMp_hay_torreta, EMs_atacar);
        //Si deja de haber torre en rango avanzo
        FMS_EnMediano.CreateTransition("dejo de atacar", EMs_atacar, EMp_no_hay_torreta, EMs_avanzar);
        //Si vida = 0 
        FMS_EnMediano.CreateTransition("muriendo1", EMs_avanzar, EMp_sin_vida, EMs_morir);
        FMS_EnMediano.CreateTransition("muriendo2", EMs_cambiar_camino, EMp_sin_vida, EMs_morir);
        FMS_EnMediano.CreateTransition("muriendo3", EMs_atacar, EMp_sin_vida, EMs_morir);
        //Volver a atacar
        FMS_EnMediano.CreateTransition("volver a atacar", EMs_atacar, EMp_hay_torreta, EMs_atacar);
        //Volver a andar
        FMS_EnMediano.CreateTransition("volver a andar", EMs_cambiar_camino, EMp_sin_enano, EMs_avanzar);
    }

    void EM_avanzar()
    {
        print("avanzo");
        avanzar(Vector3.zero);
    }

    void EM_cambiar_camino()
    {
        print("cambio camino");

        if (gameManager.enemiesHelp.Count > 0)
        {
            GameObject enemyHelp = gameManager.enemiesHelp[0];

            if (!enemyHelp.GetComponent<CP_EnemigoEnano>().enemyMedium)
            {
                gameManager.enemiesHelp.Remove(enemyHelp);

                enemySmall = enemyHelp;
                enemyHelp.GetComponent<CP_EnemigoEnano>().enemyMedium = gameObject;
                enemySmallPos = enemySmall.transform.position;

                avanzar(enemySmallPos);
            }
        }
    }
    void EM_atacar()
    {
        print("ataco");
        if (towerInRange)
        {
            avanzar(towerInRange.transform.position);

            if (Vector3.Distance(transform.position, towerInRange.transform.position) < rangeAttack)
            {
                if (towerInRange.gameObject.GetComponent<CP_Torres>())
                {
                    if (hitRateTimer >= hitRate)
                    {
                        hitRateTimer = 0;
                        towerInRange.gameObject.GetComponent<CP_Torres>().health -= damage;
                    }
                }
                if (towerInRange.gameObject.GetComponent<Wall>())
                {
                    if (hitRateTimer >= hitRate)
                    {
                        hitRateTimer = 0;
                        towerInRange.gameObject.GetComponent<Wall>().health -= damage;
                    }
                }
            }
        }
        else
        {
            avanzar(gameManager.mainTower.transform.position);
        }
    }
    void EM_morir()
    {
        print("hasta luego");

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out hit, 1000, LayerMask.GetMask("Ground")))
        {
            GameObject deadTile = hit.collider.gameObject;

            deadTile.GetComponent<GroundInfo>().numKills++;
        }

        gameManager.numEnemiesMedium--;
        gameManager.totalKilss++;

        Destroy(gameObject);
    }

    //Método que hace que controla avance del enemigo
    void avanzar(Vector3 targetPos)
    {
        print(targetPos);
        print("voy camninando por la vida, sin pausa pero sin prisa");
        //nav.destination = targetPos;
        //nav.SetDestination(targetPos);
        nav.SetDestination(new Vector3(targetPos.x, 0, targetPos.z));
    }
}

