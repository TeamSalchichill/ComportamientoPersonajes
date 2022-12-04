using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Torres : MonoBehaviour
{
    StateMachineEngine FMS_Torres;

    public int health;
    //public bool hayenemigo;
    public float range;
    public GameObject emnemigo;

    public GameObject bala;
    int vidaEnemigo = 10000;
    void Start()
    {
        FMS_Torres = new StateMachineEngine(false);

        //Creación de estados
        State Ts_atacar = FMS_Torres.CreateState("atacar", T_atacando);
        State Ts_no_atacar = FMS_Torres.CreateEntryState("no atacar", T_no_atacando);
        State Ts_morir = FMS_Torres.CreateState("morir", T_muerte);

        //Creación de percepciones
        Perception Tp_hay_enemigo = FMS_Torres.CreatePerception<ValuePerception>(() => Vector3.Distance(transform.position, emnemigo.transform.position) < range);
        Perception Tp_no_hay_enemigo = FMS_Torres.CreatePerception<ValuePerception>(() => Vector3.Distance(transform.position, emnemigo.transform.position) > range);
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
    }

    void Update()
    {
        FMS_Torres.Update();
    }

    void T_atacando()
    {
        print("Ataco");
        while (vidaEnemigo > 0)
        {
            Shoot();
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

    void Shoot()
    {
       // vidaEnemigo--;
       // print(vidaEnemigo);

        GameObject instBullet = Instantiate(bala, transform.position, transform.rotation);
        instBullet.GetComponent<CP_Bullet_Tower>().Seek(emnemigo.transform);
    }
}
