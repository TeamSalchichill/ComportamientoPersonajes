using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Hero2_Healer : MonoBehaviour
{
    public int health = 1000;
    public bool enemigos = false;
    public bool torreDanada = false;
    public bool timerAumento = false;
    public bool timerParalizar = false;

    BehaviourTreeEngine BT_Heroe2;

    void Start()
    {
        //definimos el arbol

        BT_Heroe2 = new BehaviourTreeEngine(BehaviourEngine.IsNotASubmachine);

        //nodos selectores
        SelectorNode selector1 = BT_Heroe2.CreateSelectorNode("MuertoVivo");
        SelectorNode selector2 = BT_Heroe2.CreateSelectorNode("AccionesVivo");

        //nodos secuencia
        SequenceNode secuencia1 = BT_Heroe2.CreateSequenceNode("SecuenciaComprobarMuerte", false);
        SequenceNode secuencia2 = BT_Heroe2.CreateSequenceNode("SecuenciaComprobarAumento", false);
        SequenceNode secuencia3 = BT_Heroe2.CreateSequenceNode("SecuenciaComprobarParalizar", false);
        SequenceNode secuencia4 = BT_Heroe2.CreateSequenceNode("SecuenciaAtacar", false);
        SequenceNode secuencia5 = BT_Heroe2.CreateSequenceNode("SecuenciaComprobarTorreDebil", false);

        //nodos hoja
        LeafNode comprobarVida = BT_Heroe2.CreateLeafNode("¿Vida<= 0?", circuloAmarillo, ComprobarVida);
        LeafNode NodoMuerte = BT_Heroe2.CreateLeafNode("Muero", Morir, alwaysSuccedeed);

        LeafNode comprobarAumento = BT_Heroe2.CreateLeafNode("¿AumentarVelocidad?", circuloAmarillo, ComprobarAumento);
        LeafNode comprobarEnemigo1 = BT_Heroe2.CreateLeafNode("¿HayEnemigo1?", circuloAmarillo, ComprobarEnemigo);
        LeafNode NodoAumento = BT_Heroe2.CreateLeafNode("Aumento", Aumentar, alwaysSuccedeed);

        LeafNode comprobarEnemigo2 = BT_Heroe2.CreateLeafNode("¿HayEnemigo2?", circuloAmarillo, ComprobarEnemigo);
        LeafNode comprobarGolpe = BT_Heroe2.CreateLeafNode("¿MeHanGolpeado?", circuloAmarillo, ComprobarGolpe);
        LeafNode NodoParalizar = BT_Heroe2.CreateLeafNode("Palarizar", Paralizar, alwaysSuccedeed);

        LeafNode comprobarEnemigo3 = BT_Heroe2.CreateLeafNode("¿HayEnemigo3?", circuloAmarillo, ComprobarEnemigo);
        LeafNode NodoAtacar = BT_Heroe2.CreateLeafNode("Atacar", Atacar, alwaysSuccedeed);

        LeafNode comprobarTorre = BT_Heroe2.CreateLeafNode("¿HayTorreDanada?", circuloAmarillo, ComprobarHayTorreDanada);
        LeafNode NodoCurar = BT_Heroe2.CreateLeafNode("Curar", Curar, alwaysSuccedeed);

        LeafNode NodoParar = BT_Heroe2.CreateLeafNode("Parar", Idle, alwaysSuccedeed);

        //añadimos hijos a los nodos (debajo a arriba)
        secuencia2.AddChild(comprobarAumento);
        secuencia2.AddChild(comprobarEnemigo1);
        secuencia2.AddChild(NodoAumento);

        secuencia3.AddChild(comprobarEnemigo2);
        secuencia3.AddChild(comprobarGolpe);
        secuencia3.AddChild(NodoParalizar);

        secuencia4.AddChild(comprobarEnemigo3);
        secuencia4.AddChild(NodoAtacar);

        secuencia5.AddChild(comprobarTorre);
        secuencia5.AddChild(NodoCurar);

        selector2.AddChild(secuencia2);
        selector2.AddChild(secuencia3);
        selector2.AddChild(secuencia4);
        selector2.AddChild(secuencia5);
        selector2.AddChild(NodoParar);

        secuencia1.AddChild(comprobarVida);
        secuencia1.AddChild(NodoMuerte);

        selector1.AddChild(secuencia1);
        selector1.AddChild(selector2);

        //Nodo inicial 
        LoopUntilFailDecoratorNode NodoInicial = BT_Heroe2.CreateLoopUntilFailNode("LoopUntilFail", selector1);

        //Definir el nodo raiz
        BT_Heroe2.SetRootNode(NodoInicial);


    }
    void Update()
    {
        BT_Heroe2.Update();

    }
    void circuloAmarillo()
    {
        // print("soy una bola amarilla, es decir una pregunta ");
    }
    void Morir()
    {
        print("estoy muerto");
    }
    void Aumentar()
    {
        print("aumento velocidad");
    }
    void Paralizar()
    {
        print("paralizo");
    }
    void Atacar()
    {
        print("atacar");
    }
    void Curar()
    {
        print("curar");
    }
    void Idle()
    {
        print("estoy en idle");
    }
    ReturnValues alwaysSuccedeed()
    {
        return ReturnValues.Succeed;
    }
    ReturnValues ComprobarVida()
    {
        if (health <= 0)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            print("mi vida es:" + health);
            return ReturnValues.Failed;
        }
    }
    ReturnValues ComprobarAumento()
    {
        if (timerAumento == true)
        {
            print("puedo aumentar");
            return ReturnValues.Succeed;
        }
        else
        {
            print("no puedo aumentar");
            return ReturnValues.Failed;
        }
    }
    ReturnValues ComprobarEnemigo()
    {
        if (enemigos == true)
        {
            print("hay enemigos");
            return ReturnValues.Succeed;
        }
        else
        {
            print("no hay enemigos");
            return ReturnValues.Failed;
        }
    }
    ReturnValues ComprobarGolpe()
    {
        if (timerParalizar == true)
        {
            print("me han golpeado");
            return ReturnValues.Succeed;
        }
        else
        {
            print("no me han golpeado");
            return ReturnValues.Failed;
        }
    }
    ReturnValues ComprobarHayTorreDanada()
    {
        //si hay una torre dañada
        //return ReturnValues.Succeed;
        //si no hay torre danada
        //return ReturnValues.Failed;

        if (torreDanada == true)
        {
            print("hay torre dañada");
            return ReturnValues.Succeed;
        }
        else
        {
            print("No hay torre dañada");
            return ReturnValues.Failed;
        }

    }

}