using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Hero1_Invocador : MonoBehaviour
{
    StateMachineEngine FSM_Vivo;
    //StateMachineEngine FSM_Nivel1;
    

    public int health = 1000;                      //vida del heroe
    public bool hayEnemigos = false;        //comprobamos si hay enemigos dentro del rango del heroe
    public int vidaEnemigos = 0;            //comprobamos si la vida de los enemigos a rango en menor de 4000
    public bool muroVivo = false;           //comprobamos si ya hay un muro colocado


    void Start()
    {
        //Crear maquinas de estado
        FSM_Vivo = new StateMachineEngine(BehaviourEngine.IsNotASubmachine);
        //FSM_Nivel1 = new StateMachineEngine(BehaviourEngine.IsNotASubmachine);

        //Creacion de estados

        //estados FSM_Vivo
        State Idle = FSM_Vivo.CreateEntryState("Idle", accionIdle);
        State Invocar =FSM_Vivo.CreateState("Invocar", accionInvocar);
        State Atacar =FSM_Vivo.CreateState("Atacar", accionAtacar);

        //estados FSM_Nivel1
       //State Vivo =  FSM_Nivel1.CreateSubStateMachine("Vivo", FSM_Vivo);
        //State Muerto = FSM_Nivel1.CreateState("Muerto", accionMorir);

        
        //Crear percepciones
        //Percepciones FSM_Nivel1
        //Perception tengo_vida = FSM_Nivel1.CreatePerception<ValuePerception>(() => health <= 0); //comprobamos si la vida del heroe es menor o igual a cero

        //Percepciones FSM_Vivo
        Perception enemigos_debiles = FSM_Vivo.CreatePerception<ValuePerception>(() => hayEnemigos == true,() => vidaEnemigos < 4000);
        Perception no_hay_enemigos = FSM_Vivo.CreatePerception<ValuePerception>(() => hayEnemigos == false);
        Perception debo_invocar = FSM_Vivo.CreatePerception<ValuePerception>(() => hayEnemigos == true, () => vidaEnemigos > 4000, () => muroVivo == false);
        Perception ya_invoque_y_hay_enemigos = FSM_Vivo.CreatePerception<ValuePerception>(() => hayEnemigos == true, () => muroVivo == true);



        //Creacion de transiciones
        //FSM_Nivel1.CreateTransition("Morir", Vivo, tengo_vida, Muerto);

        FSM_Vivo.CreateTransition("aInvocar", Idle, debo_invocar, Invocar);
        FSM_Vivo.CreateTransition("aInvocar1", Atacar, debo_invocar, Invocar);

        FSM_Vivo.CreateTransition("aAtacar", Idle, enemigos_debiles, Atacar);
        FSM_Vivo.CreateTransition("aAtacar1", Invocar, ya_invoque_y_hay_enemigos, Atacar);

        FSM_Vivo.CreateTransition("aIdle", Invocar, no_hay_enemigos, Idle);
        FSM_Vivo.CreateTransition("aIdle1", Atacar, no_hay_enemigos, Idle);




    }

    void Update()
    {
        //FSM_Nivel1.Update();
        FSM_Vivo.Update();

        //aqui van todas las comprobaciones para cambiar los valores de las variables
        if(health <= 0)
        {
            print("muere");
        }
    }

    void accionIdle()
    {
        Debug.Log("estoy en idle");
        print("hola");
    }
    void accionInvocar()
    {
        print("invocar");
    }
    void accionAtacar()
    {
        print("ataco");
    }
    
}
