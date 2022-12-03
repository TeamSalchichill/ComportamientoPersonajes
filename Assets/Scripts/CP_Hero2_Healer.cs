using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Hero2_Healer : MonoBehaviour
{
    public int health = 1000;
    public bool hayEnemigos = false;
    public bool torreDañada = false;
    public bool aumentar = false;
    public bool paralizar = false;

    int vuelta = 0;

    BehaviourTreeEngine BT_Heroe2;
    void Start()
    {
        BT_Heroe2 = new BehaviourTreeEngine(BehaviourEngine.IsNotASubmachine);

        //nodo selector
        SelectorNode selector1 = BT_Heroe2.CreateSelectorNode("MuertoVivo");
        SelectorNode selector2 = BT_Heroe2.CreateSelectorNode("AccionesVivo");

        //nodo secuencia (arriba a abajo , izquierda derecha)
        SequenceNode secuencia1 = BT_Heroe2.CreateSequenceNode("SecuenciaComprobarMuerte", false);
        SequenceNode secuencia2 = BT_Heroe2.CreateSequenceNode("SecuenciaComprobarEnemigo", false);
        SequenceNode secuencia3 = BT_Heroe2.CreateSequenceNode("SecuenciaComprobarHayTorreDañada", false);
        SequenceNode secuencia4 = BT_Heroe2.CreateSequenceNode("SecuenciaComprobarAccionHeroe", false);
        SequenceNode secuencia5 = BT_Heroe2.CreateSequenceNode("SecuenciaComprobarAumentoVel", false);
        SequenceNode secuencia6 = BT_Heroe2.CreateSequenceNode("SecuenciaComprobarParalizar", false);

        //nodos hoja
        LeafNode comprobarVida = BT_Heroe2.CreateLeafNode("¿Vida <= 0?", circuloAmarillo, ComprobarVida);
        LeafNode NodoMuerte = BT_Heroe2.CreateLeafNode("Muero", Morir, alwaysSuccedeed);

        LeafNode NodoParar = BT_Heroe2.CreateLeafNode("Parar", Idle, alwaysSuccedeed);

        LeafNode comprobarHayEnemigo = BT_Heroe2.CreateLeafNode("¿Hay Enemigo?", circuloAmarillo, ComprobarHayEnemigo);
        LeafNode NodoAtacar = BT_Heroe2.CreateLeafNode("Atacar",Atacar, alwaysSuccedeed);
        LeafNode comprobarTorre = BT_Heroe2.CreateLeafNode("¿Hay torre dañada?", circuloAmarillo, ComprobarHayTorreDañada);
        LeafNode NodoCurar = BT_Heroe2.CreateLeafNode("Curar",Curar, alwaysSuccedeed);

        LeafNode comprobaraumentoVel = BT_Heroe2.CreateLeafNode("¿Puedo aumentar velocidad?", circuloAmarillo, ComprobarPuedoAumentarVel);
        LeafNode NodoAumentarVel = BT_Heroe2.CreateLeafNode("AumentarVel", Aumento, alwaysSuccedeed);
        LeafNode comprobarGolpeado = BT_Heroe2.CreateLeafNode("¿Me han golpeado?", circuloAmarillo, ComprobarMeHanGolpeado);
        LeafNode NodoParalizar =  BT_Heroe2.CreateLeafNode("Paralizar", Paralizo, alwaysSuccedeed);

        //añadimos hijos a los nodos (debajo a arriba)

        secuencia5.AddChild(comprobaraumentoVel);
        secuencia5.AddChild(NodoAumentarVel);

        secuencia6.AddChild(comprobarGolpeado);
        secuencia6.AddChild(NodoParalizar);

        //los succeder 
        SucceederDecoratorNode exitoAumento = BT_Heroe2.CreateSucceederNode("ExitoAumento", secuencia5);
        SucceederDecoratorNode exitoParalizar = BT_Heroe2.CreateSucceederNode("ExitoParalizar", secuencia6);

        secuencia4.AddChild(exitoAumento);
        secuencia4.AddChild(exitoParalizar);

        secuencia2.AddChild(comprobarHayEnemigo);
        secuencia2.AddChild(secuencia4);
        secuencia2.AddChild(NodoAtacar);

        secuencia3.AddChild(comprobarTorre);
        secuencia3.AddChild(NodoCurar);

        selector2.AddChild(secuencia2);
        selector2.AddChild(secuencia3);
        selector2.AddChild(NodoParar);

        secuencia1.AddChild(comprobarVida);
        secuencia1.AddChild(NodoMuerte);

        selector1.AddChild(secuencia1);
        selector1.AddChild(selector2);

        //Nodo inicial 
        LoopUntilFailDecoratorNode NodoInicial= BT_Heroe2.CreateLoopUntilFailNode("LoopUntilFail", selector1);

        //Definir el nodo raiz
        BT_Heroe2.SetRootNode(NodoInicial);
    }

    void Update()
    {
        BT_Heroe2.Update();

    }

    void circuloAmarillo()
    {
        print("soy una bola amarilla, es decir una pregunta ");
    }
    void Morir()
    {
        print("estoy muerto");
    }
    void Idle()
    {
        print("estoy en idle");
    }
    void Atacar()
    {
        print("ataco");
    }
    void Curar()
    {
        print("curar");
    }
    void Aumento()
    {
        print("aumento");
    }
    void Paralizo()
    {
        print("paralizo");
    }

    ReturnValues ComprobarVida()
    {
        if(health <= 0)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }
    ReturnValues alwaysSuccedeed()
    {
        return ReturnValues.Succeed;
    }
    ReturnValues ComprobarHayEnemigo()
    {
        if (hayEnemigos == true)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }
    ReturnValues ComprobarHayTorreDañada()
    {
        //si hay una torre dañada
        //return ReturnValues.Succeed;
        //si no hay torre danada
        //return ReturnValues.Failed;

        if (torreDañada == true)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }

    }
    ReturnValues ComprobarPuedoAumentarVel()
    {
        //si ha pasado el tiempo
        //comprobar que torre ha matado mas
        //return ReturnValues.Succeed;
        //si no ha pasado el tiempo
        //return ReturnValues.Failed;

        if (aumentar == true)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }

    }
    ReturnValues ComprobarMeHanGolpeado()
    {
        //si me golpean
        //se selcciona a quien a golpeado
        //return ReturnValues.Succeed;
        //si no me golpean
        //return ReturnValues.Failed;

        if (paralizar == true)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }

    }
}
