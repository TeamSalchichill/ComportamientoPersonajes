using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CP_Boss2_Atacante : MonoBehaviour
{
    StateMachineEngine SM_Boss2;
    UtilitySystemEngine US_Boss2;

    GameManager gameManager;

    [Header("Components")]
    public NavMeshAgent nav;

    [Header("Stats")]
    public int health;
    public int healthMax;
    public int damage;
    public int range;
    public int speed;
    public float hitRate;
    float hitRateTimer;
    [Space]
    public float furyRate;
    float furyRateTimer;
    public float fury;
    public int furyIncrement;
    public int furyIncrementTime;
    public int minFury;
    [Space]
    public int curation;

    [Header("Checks variables")]
    public bool towerInRangeCheck;
    public GameObject towerInRange;

    public bool towerInRangeRunCheck;
    public GameObject towerInRangeRun;

    public bool wallInRangeCheck;
    public GameObject wallInRange;

    List<GameObject> towersInRange;

    void Start()
    {
        gameManager = GameManager.instance;

        nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(Vector3.zero);
        nav.speed = speed;

        hitRateTimer = hitRate;
        //furyRateTimer = furyRate;

        SM_Boss2 = new StateMachineEngine(false);
        US_Boss2 = new UtilitySystemEngine(true);
        CreateSM();
        CreateUS();
    }

    void Update()
    {
        hitRateTimer += Time.deltaTime;
        furyRateTimer += Time.deltaTime;
        fury += (Time.deltaTime * furyIncrementTime);

        towersInRange = new List<GameObject>();
        towerInRangeCheck = false;
        towerInRange = null;
        towerInRangeRunCheck = false;
        towerInRangeRun = null;
        wallInRangeCheck = false;
        wallInRange = null;
        foreach (GameObject tower in gameManager.towers)
        {
            if (tower)
            {
                if (Vector3.Distance(transform.position, tower.transform.position) < range)
                {
                    if (tower.GetComponent<CP_Torres>())
                    {
                        if (!towerInRangeCheck)
                        {
                            towerInRangeCheck = true;
                            towerInRange = tower;
                        }
                        towersInRange.Add(tower);
                    }
                    if (tower.GetComponent<Wall>())
                    {
                        if (!wallInRangeCheck)
                        {
                            wallInRangeCheck = true;
                            wallInRange = tower;
                        }
                        towersInRange.Add(tower);
                    }
                    if (tower.GetComponent<CP_Hero1_Invocador>())
                    {
                        if (!towerInRangeCheck)
                        {
                            towerInRangeCheck = true;
                            towerInRange = tower;
                        }
                        towersInRange.Add(tower);
                    }
                    if (tower.GetComponent<CP_Hero2_Healer>())
                    {
                        if (!towerInRangeCheck)
                        {
                            towerInRangeCheck = true;
                            towerInRange = tower;
                        }
                        towersInRange.Add(tower);
                    }
                }
            }
        }

        if (Vector3.Distance(transform.position, Vector3.zero) < 3)
        {
            print("Llegué");
            Destroy(gameObject);
        }

        SM_Boss2.Update();
        US_Boss2.Update();
    }
    void CreateSM()
    {
        State Boss2_Avanzar = SM_Boss2.CreateEntryState("Avanzar", B2_Avanzar);
        State Boss2_Atacar = SM_Boss2.CreateState("Ataque", B2_Atacar);
        State Boss2_Morir = SM_Boss2.CreateState("Morir", B2_Morir);
        State Boss2_Rabia = SM_Boss2.CreateSubStateMachine("Rabia", US_Boss2);

        Perception Boss2_hay_enemigos = SM_Boss2.CreatePerception<ValuePerception>(() => towersInRange.Count > 0);
        Perception Boss2_no_hay_enemigos = SM_Boss2.CreatePerception<ValuePerception>(() => towersInRange.Count == 0);
        Perception Boss2_sin_vida = SM_Boss2.CreatePerception<ValuePerception>(() => health <= 0);
        Perception Boss2_tengo_rabia = SM_Boss2.CreatePerception<ValuePerception>(() => fury >= minFury, () => furyRateTimer >= furyRate);
        Perception Boss2_no_tengo_rabia = SM_Boss2.CreatePerception<ValuePerception>(() => fury < minFury);
        Perception Boss2_enemigo_rabia_disable = SM_Boss2.CreatePerception<ValuePerception>(() => towersInRange.Count > 0, () => furyRateTimer < furyRate, () => fury >= 0);

        Perception Boss2_enemigo_rabia = SM_Boss2.CreatePerception<ValuePerception>(() => towersInRange.Count > 0, () => fury > 0);////

        Perception Boss2_no_tengo_rabia_y_enemigo = SM_Boss2.CreateAndPerception<AndPerception>(Boss2_no_tengo_rabia, Boss2_hay_enemigos);
        Perception Boss2_no_tengo_rabia_y_no_enemigo = SM_Boss2.CreateAndPerception<AndPerception>(Boss2_no_tengo_rabia, Boss2_no_hay_enemigos);
        Perception Boss2_tengo_rabia_y_enemigo = SM_Boss2.CreateAndPerception<AndPerception>(Boss2_tengo_rabia, Boss2_hay_enemigos);

        SM_Boss2.CreateTransition("Avanzar->Ataque", Boss2_Avanzar, Boss2_no_tengo_rabia_y_enemigo, Boss2_Atacar);

        SM_Boss2.CreateTransition("Avanzar->Ataque2", Boss2_Avanzar, Boss2_enemigo_rabia, Boss2_Atacar);////

        SM_Boss2.CreateTransition("Ataque->Avanzar", Boss2_Atacar, Boss2_no_hay_enemigos, Boss2_Avanzar);
        SM_Boss2.CreateTransition("Ataque->Morir", Boss2_Atacar, Boss2_sin_vida, Boss2_Morir);
        SM_Boss2.CreateTransition("Avanzar->Morir", Boss2_Avanzar, Boss2_sin_vida, Boss2_Morir);
        SM_Boss2.CreateTransition("Ataque->UsarRabia", Boss2_Atacar, Boss2_tengo_rabia_y_enemigo, Boss2_Rabia);
        SM_Boss2.CreateTransition("Avanzar->UsarRabia", Boss2_Avanzar, Boss2_tengo_rabia_y_enemigo, Boss2_Rabia);
        SM_Boss2.CreateTransition("UsarRabia->Ataque", Boss2_Rabia, Boss2_no_tengo_rabia_y_enemigo, Boss2_Atacar);
        SM_Boss2.CreateTransition("UsarRabia->Avanzar", Boss2_Rabia, Boss2_no_tengo_rabia_y_no_enemigo, Boss2_Avanzar);
        SM_Boss2.CreateTransition("UsarRabia->Morir", Boss2_Rabia, Boss2_sin_vida, Boss2_Morir);

        SM_Boss2.CreateTransition("AtacarBucle", Boss2_Atacar, Boss2_no_tengo_rabia_y_enemigo, Boss2_Atacar);
        SM_Boss2.CreateTransition("AtacarBucle2", Boss2_Atacar, Boss2_enemigo_rabia_disable, Boss2_Atacar);
    }
    void CreateUS()
    {
        //FACTORES DE VARIABLES
        Factor Cant_Vida = new LeafVariable(() => health, 1000, 0);
        Factor Cant_Furia = new LeafVariable(() => fury, 100, 0);
        Factor Cant_Torres = new LeafVariable(() => towersInRange.Count, 6, 0);
        //CURVAS
        Factor ImportanciaCurarse = new LinearCurve(Cant_Vida, -1, 1);
        Factor F_Rabia = new LinearCurve(Cant_Torres, -1, 1);
        Factor Damage_Pot = new ExpCurve(Cant_Torres);

        // Factores Area
        List<Factor> factores_A = new List<Factor>();
        factores_A.Add(F_Rabia); factores_A.Add(Damage_Pot);
        // Pesos Area
        List<float> weights_A = new List<float>();
        weights_A.Add(0.2f); weights_A.Add(0.8f);
        // Weighted Sum 2 opciones Area
        Factor Ganas_Area = new WeightedSumFusion(factores_A, weights_A);

        // Factores Finisher
        List<Factor> factores_F = new List<Factor>();
        factores_F.Add(F_Rabia); factores_F.Add(Damage_Pot);
        // Pesos Finisher
        List<float> weights_F = new List<float>();
        weights_F.Add(0.8f); weights_F.Add(0.2f);
        // Weighted Sum 2 opciones Finisher
        Factor Ganas_Finisher = new WeightedSumFusion(factores_F, weights_F);
        //ACCIONES
        US_Boss2.CreateUtilityAction("heal", B2_Heal, ImportanciaCurarse);
        US_Boss2.CreateUtilityAction("AOE", B2_Area, Ganas_Area);
        US_Boss2.CreateUtilityAction("FIN", B2_Finisher, Ganas_Finisher);
    }
    void B2_Morir()
    {
        print("ME MUERO");
        Destroy(gameObject);
    }
    void B2_Avanzar()
    {
        print("AVANZO");
        nav.SetDestination(Vector3.zero);
    }
    void B2_Atacar()
    {
        if (hitRateTimer >= hitRate)
        {
            print("ATACO");
            hitRateTimer = 0;
            fury += furyIncrement;
            nav.SetDestination(new Vector3(towerInRange.transform.position.x, 0, towerInRange.transform.position.z));

            if (towerInRange.GetComponent<CP_Torres>())
            {
                towerInRange.GetComponent<CP_Torres>().health -= damage;
            }
            if (towerInRange.GetComponent<CP_Hero1_Invocador>())
            {
                towerInRange.GetComponent<CP_Hero1_Invocador>().health -= damage;
            }
            if (towerInRange.GetComponent<CP_Hero2_Healer>())
            {
                towerInRange.GetComponent<CP_Hero2_Healer>().health -= damage;
            }
        }
    }
    void B2_Heal()
    {
        print("ME CURO");
        furyRateTimer = 0;
        fury = 0;
        health += curation;
        health = Mathf.Min(health, healthMax);
    }
    void B2_Area()
    {
        print("AREO");
        furyRateTimer = 0;
        fury = 0;

        foreach (GameObject tower in towersInRange)
        {
            if (tower.GetComponent<CP_Torres>())
            {
                tower.GetComponent<CP_Torres>().health -= (damage * 2);
            }
            if (tower.GetComponent<CP_Hero1_Invocador>())
            {
                tower.GetComponent<CP_Hero1_Invocador>().health -= (damage * 2);
            }
            if (tower.GetComponent<CP_Hero2_Healer>())
            {
                tower.GetComponent<CP_Hero2_Healer>().health -= (damage * 2);
            }
        }
    }
    void B2_Finisher()
    {
        print("FINIQUITEO");
        furyRateTimer = 0;
        fury = 0;

        if (towerInRange)
        {
            nav.SetDestination(new Vector3(towerInRange.transform.position.x, 0, towerInRange.transform.position.z));

            if (towerInRange.GetComponent<CP_Torres>())
            {
                towerInRange.GetComponent<CP_Torres>().health -= (damage * 5);
            }
            if (towerInRange.GetComponent<CP_Hero1_Invocador>())
            {
                towerInRange.GetComponent<CP_Hero1_Invocador>().health -= (damage * 5);
            }
            if (towerInRange.GetComponent<CP_Hero2_Healer>())
            {
                towerInRange.GetComponent<CP_Hero2_Healer>().health -= (damage * 5);
            }
        }
    }
}