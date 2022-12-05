using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Boss2_Atacante : MonoBehaviour
{
    StateMachineEngine SM_Boss2;
    UtilitySystemEngine US_Boss2 = new UtilitySystemEngine(true);
    [Header("Stats")]
    public float Health;
    public int healthMax;
    public int range;
    public int damage;
    public float fireRate;
    float fireRateTimer;
    public int kills;
    public int Fury;
    public int Towers;
    public int TiempoFury;
    public bool PuedoRabia;
    public bool HayTorres;
    [Header("Checks variables")]
    public bool enemyInRangeCheck;
    public GameObject enemyInRange;
    void Start()
    {
        CreateSM();
        CreateUS();
    }
    void Update()
    {
        SM_Boss2.Update();
        US_Boss2.Update();
    }
    void CreateSM()
    {
        SM_Boss2 = new StateMachineEngine(false);
        //estados
        State Boss2_Avanzar = SM_Boss2.CreateEntryState("Avanzar", B2_Avanzar);
        State Boss2_Atacar = SM_Boss2.CreateState("Ataque", B2_Atacar);
        State Boss2_Morir = SM_Boss2.CreateState("Morir", B2_Morir);
        State Boss2_UsarRabia = SM_Boss2.CreateState("usarRabia", B2_Rabia);//CAMBIAR ESTA PARA QUE CREE EL UTILITY
        //percepciones
        Perception Boss2_hay_enemigos = SM_Boss2.CreatePerception<ValuePerception>(() => HayTorres == true);
        Perception Boss2_no_hay_enemigos = SM_Boss2.CreatePerception<ValuePerception>(() => HayTorres == false);
        Perception Boss2_sin_vida = SM_Boss2.CreatePerception<ValuePerception>(() => Health <= 0);
        Perception Boss2_tengo_rabia = SM_Boss2.CreatePerception<ValuePerception>(() => PuedoRabia == true);
        Perception Boss2_no_tengo_rabia = SM_Boss2.CreatePerception<ValuePerception>(() => PuedoRabia == false);
        Perception Boss2_no_tengo_rabia_y_enemigo= SM_Boss2.CreateAndPerception<AndPerception>(Boss2_no_tengo_rabia, Boss2_hay_enemigos);
        Perception Boss2_no_tengo_rabia_y_no_enemigo = SM_Boss2.CreateAndPerception<AndPerception>(Boss2_no_tengo_rabia, Boss2_no_hay_enemigos);
        //transiciones
        SM_Boss2.CreateTransition("Avanzar->Ataque", Boss2_Avanzar, Boss2_hay_enemigos , Boss2_Atacar);
        SM_Boss2.CreateTransition("Ataque->Avanzar", Boss2_Atacar, Boss2_no_hay_enemigos, Boss2_Avanzar);
        SM_Boss2.CreateTransition("Ataque->Morir", Boss2_Atacar, Boss2_sin_vida, Boss2_Morir);
        SM_Boss2.CreateTransition("Avanzar->Morir", Boss2_Avanzar, Boss2_sin_vida, Boss2_Morir);
        SM_Boss2.CreateTransition("UsarRabia->Morir", Boss2_UsarRabia, Boss2_sin_vida, Boss2_Morir);
        SM_Boss2.CreateTransition("Ataque->UsarRabia", Boss2_Atacar, Boss2_tengo_rabia, Boss2_UsarRabia);
        SM_Boss2.CreateTransition("UsarRabia->Avanzar", Boss2_UsarRabia, Boss2_no_tengo_rabia_y_no_enemigo, Boss2_Avanzar);
        SM_Boss2.CreateTransition("Avanzar->UsarRabia", Boss2_Avanzar, Boss2_tengo_rabia, Boss2_UsarRabia);
        SM_Boss2.CreateTransition("UsarRabia->Ataque", Boss2_UsarRabia, Boss2_no_tengo_rabia_y_enemigo, Boss2_Atacar);

    }
    void CreateUS()
    {
        US_Boss2 = new UtilitySystemEngine(true);
        /*
        Factor Cant_Vida = new LeafVariable(Vida, 1000, 0); //como pasar datos para los factor
        Factor Cant_Furia = new LeafVariable(Fury, 100, 0);
        Factor Cant_Torres = new LeafVariable(Towers, 10, 0);
        Factor Tiempo_Sin = new LeafVariable(TiempoFury, 30, 0);
        
        Factor ImportanciaCurarse = new LinearCurve(Cant_Vida);
        Factor F_Rabia = new LinearCurve(Cant_Furia);
        Factor Tiempo_Sin_Rabia = new LinearCurve(Tiempo_Sin);
        Factor Daño_Pot = new ExpCurve(Cant_Torres);
        
        // Factores rabia
        List<Factor> factores_R = new List<Factor>();
        factores_R.Add(F_Rabia); factores_R.Add(Tiempo_Sin_Rabia);
        // Pesos Rabia
        List<float> weights_R = new List<float>();
        weights_R.Add(0.5f); weights_R.Add(0.5f);
        // Weighted Sum 2 opciones Rabia
        Factor Ganas_Rabia = new WeightedSumFusion(factores_R, weights_R);

        // Factores Area
        List<Factor> factores_A = new List<Factor>();
        factores_A.Add(Ganas_Rabia); factores_A.Add(Daño_Pot);
        // Pesos Area
        List<float> weights_A = new List<float>();
        weights_A.Add(0.2f); weights_A.Add(0.8f);
        // Weighted Sum 2 opciones Area
        Factor Ganas_Area = new WeightedSumFusion(factores_A, weights_A);

        // Factores Finisher
        List<Factor> factores_F = new List<Factor>();
        factores_F.Add(Ganas_Rabia); factores_F.Add(Daño_Pot);
        // Pesos Finisher
        List<float> weights_F = new List<float>();
        weights_F.Add(0.8f); weights_F.Add(0.2f);
        // Weighted Sum 2 opciones Finisher
        Factor Ganas_Finisher = new WeightedSumFusion(factores_F, weights_F);

        UtilityAction Curar = US_Boss2.CreateUtilityAction("heal", B2_Heal, ImportanciaCurarse);
        UtilityAction Area = US_Boss2.CreateUtilityAction("AOE", B2_Area, Ganas_Area);
        UtilityAction Finisher = US_Boss2.CreateUtilityAction("FIN", B2_Finisher, Ganas_Finisher);
        UtilityAction Nothing = US_Boss2.CreateUtilityAction("idle", B2_Idle, Daño_Pot);
        */
    }
    float Vida ( float health)
    {
        return health;
    }
    void B2_Avanzar()
    {
        print("AVANZO");
    }
    void B2_Atacar()
    {
        print("ATACO");
    }
    void B2_Morir()
    {
        print("ME MUERO");
        Destroy(gameObject);
    }
    void B2_Heal()
    {
        print("ME CURO");
    }
    void B2_Area()
    {
        print("AREO");
    }
    void B2_Finisher()
    {
        print("FINIQUITEO");
    }
    void B2_Idle()
    {
        print("no hago nada");
    }
    void B2_Rabia()
    {
        print("rabia");
    }

}