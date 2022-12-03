using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_EnemigoMediano : MonoBehaviour
{
    StateMachineEngine FMS_EnMediano;

    public int health;
    public int rango;
    public GameObject torreta;
    public bool pequeñosLlaman;

    void Start()
    {
        FMS_EnMediano = new StateMachineEngine(false);

        //Creación de estados
        State EMs_avanzar = FMS_EnMediano.CreateEntryState("avanzar", EM_avanzar);
        State EMs_cambiar_camino = FMS_EnMediano.CreateState("cambiar camino", EM_cambiar_camino);
        State EMs_atacar = FMS_EnMediano.CreateState("atacar", EM_atacar);
        State EMs_morir = FMS_EnMediano.CreateState("morir", EM_morir);

        //Creación de percepciones
        Perception EMp_pequeños_llaman = FMS_EnMediano.CreatePerception<ValuePerception>(() => pequeñosLlaman == true);
        Perception EMp_hay_torreta = FMS_EnMediano.CreatePerception<ValuePerception>(() => Vector3.Distance(transform.position, torreta.transform.position) < rango);
        Perception EMp_no_hay_torreta = FMS_EnMediano.CreatePerception<ValuePerception>(() => Vector3.Distance(transform.position, torreta.transform.position) > rango);
        Perception EMp_sin_vida = FMS_EnMediano.CreatePerception<ValuePerception>(() => health <= 0);

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
    }

    void Update()
    {
        FMS_EnMediano.Update();
    }

    void EM_avanzar()
    {
        print("avanzo");
        avanzar();
    }

    void EM_cambiar_camino()
    {
        print("cambio camino");
        avanzar();
    }
    void EM_atacar()
    {
        print("ataco");
    }
    void EM_morir()
    {
        print("hasta luego");
        Destroy(gameObject);
    }

    //Método que hace que controla avance del enemigo
    void avanzar()
    {
        print("voy camninando por la vida, sin pausa pero sin prisa");
    }

}

