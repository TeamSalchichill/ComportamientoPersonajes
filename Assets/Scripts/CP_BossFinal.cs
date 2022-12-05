using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_BossFinal : MonoBehaviour
{
    BehaviourTreeEngine BT_BossFinal;
    public int health;
    public bool lanzarHechizo = false;
    public bool gastarRecursos = false;
    public bool invocarBoss = false;
    public bool bolaDeFuego = false;
    public bool inhabilitarTorre = false;
    public bool invocarMoscas = false;

    void Start()
    {
        CreateBT();
    }

    void Update()
    {
        BT_BossFinal.Update();
    }

    void CreateBT()
    {
        BT_BossFinal = new BehaviourTreeEngine(false);

        //Flecha
        //SequenceNode SecuenciaPadre = BT_BossFinal.CreateSequenceNode("SecuenciaPadre", false);
        SequenceNode SecuenciaMorir = BT_BossFinal.CreateSequenceNode("SecuenciaMorir", false);
        SequenceNode SecuenciaHechizo = BT_BossFinal.CreateSequenceNode("SecuenciaHechizo", false);
        SequenceNode SecuenciaRecursos = BT_BossFinal.CreateSequenceNode("SecuenciaRecursos", false);
        SequenceNode SecuenciaInvocarBoss = BT_BossFinal.CreateSequenceNode("SecuenciaInvocarBoss", false);
        SequenceNode SecuenciaBolaDeFuego = BT_BossFinal.CreateSequenceNode("SecuenciaBolaDeFuego", false);
        SequenceNode SecuenciaInhabilitarTorre = BT_BossFinal.CreateSequenceNode("SecuenciaInhabilitarTorre", false);
        SequenceNode SecuenciaInvocarMoscas = BT_BossFinal.CreateSequenceNode("SecuenciaInvocarMoscas", false);

        //Interrogaciones
        SelectorNode selectorHechizos = BT_BossFinal.CreateSelectorNode("selectorHechizos");
        SelectorNode selectorPadre = BT_BossFinal.CreateSelectorNode("selectorPadre");

        //Acciones
        LeafNode AccionMorir = BT_BossFinal.CreateLeafNode("Morir", Morir, AlwaysSucceed);
        LeafNode AccionInvocarPequeMedia = BT_BossFinal.CreateLeafNode("InvocarPM", InvocarPM, AlwaysSucceed);
        LeafNode AccionGastarRecursos = BT_BossFinal.CreateLeafNode("GastarRecursos", GastarRecursos, AlwaysSucceed);
        LeafNode AccionInvocarBoss = BT_BossFinal.CreateLeafNode("InvocarBoss", InvocarBoss, AlwaysSucceed);
        LeafNode AccionBolaDeFuego = BT_BossFinal.CreateLeafNode("BolaDeFuego", BolaDeFuego, AlwaysSucceed);
        LeafNode AccionInhabilitarTorre = BT_BossFinal.CreateLeafNode("InhabilitarTorre", InhabilitarTorre, AlwaysSucceed);
        LeafNode AccionInvocarMoscas = BT_BossFinal.CreateLeafNode("InvocarMoscas", InvocarMoscas, AlwaysSucceed);
        LeafNode AccionAumentarVelocidad = BT_BossFinal.CreateLeafNode("AumentarVelocidad", AumentarVelocidad, AlwaysSucceed);
        LeafNode AccionIdle = BT_BossFinal.CreateLeafNode("Idle", Idle, AlwaysSucceed);

        //Percepciones
        LeafNode PercepcionVida = BT_BossFinal.CreateLeafNode("Vida<=0", AlwaysUseless, EvaluacionVida);
        LeafNode PercepcionLanzarHechizo = BT_BossFinal.CreateLeafNode("PuedoLanzarHechizoP", AlwaysUseless, LanzarHechizo);
        LeafNode PercepcionGastarRecursos = BT_BossFinal.CreateLeafNode("GastarRecursosP", AlwaysUseless, GastarRecursosP);
        LeafNode PercepcionPuedoInvocarBoss = BT_BossFinal.CreateLeafNode("PuedoInvocarBossP", AlwaysUseless, InvocarBossP);
        LeafNode PercepcionBolaDeFuego = BT_BossFinal.CreateLeafNode("PuedoLanzarBolaDeFuegoP", AlwaysUseless, BolaDeFuegoP);
        LeafNode PercepcionInhabilitarTorre = BT_BossFinal.CreateLeafNode("PuedoInhabilitarTorreP", AlwaysUseless, InhabilitarTorreP);
        LeafNode PercepcionInvocarMoscas = BT_BossFinal.CreateLeafNode("PuedoInvocarMoscasP", AlwaysUseless, InvocarMoscasP);


        //Añadir hijos
        SecuenciaBolaDeFuego.AddChild(PercepcionBolaDeFuego);
        SecuenciaBolaDeFuego.AddChild(AccionBolaDeFuego);

        SecuenciaInhabilitarTorre.AddChild(PercepcionInhabilitarTorre);
        SecuenciaInhabilitarTorre.AddChild(AccionInhabilitarTorre);

        SecuenciaInvocarMoscas.AddChild(PercepcionInvocarMoscas);
        SecuenciaInvocarMoscas.AddChild(AccionInvocarMoscas);

        selectorHechizos.AddChild(SecuenciaBolaDeFuego);
        selectorHechizos.AddChild(SecuenciaInhabilitarTorre);
        selectorHechizos.AddChild(SecuenciaInvocarMoscas);
        selectorHechizos.AddChild(AccionAumentarVelocidad);

        SecuenciaMorir.AddChild(PercepcionVida);
        SecuenciaMorir.AddChild(AccionMorir);

        SecuenciaHechizo.AddChild(PercepcionLanzarHechizo);
        SecuenciaHechizo.AddChild(selectorHechizos);

        SecuenciaRecursos.AddChild(PercepcionGastarRecursos);
        SecuenciaRecursos.AddChild(AccionInvocarPequeMedia);

        SecuenciaInvocarBoss.AddChild(PercepcionPuedoInvocarBoss);
        SecuenciaInvocarBoss.AddChild(AccionInvocarBoss);

        /*
        InverterDecoratorNode InverterVida= BT_BossFinal.CreateInverterNode("InverterVida", SecuenciaMorir);

        SucceederDecoratorNode SuccederHechizos=BT_BossFinal.CreateSucceederNode("SucceederHechizos", SecuenciaHechizo);
        SucceederDecoratorNode SuccederRecursos = BT_BossFinal.CreateSucceederNode("SuccederRecursos", SecuenciaRecursos);
        SucceederDecoratorNode SuccederInvocarBoss = BT_BossFinal.CreateSucceederNode("SuccederInvocarBoss", SecuenciaInvocarBoss);
        */

        selectorPadre.AddChild(SecuenciaMorir);
        selectorPadre.AddChild(SecuenciaHechizo);
        selectorPadre.AddChild(SecuenciaRecursos);
        selectorPadre.AddChild(SecuenciaInvocarBoss);
        selectorPadre.AddChild(AccionIdle);

        TreeNode Loop = BT_BossFinal.CreateLoopNode("Loop", selectorPadre);
        BT_BossFinal.SetRootNode(Loop);

    }

    ReturnValues AlwaysSucceed()
    {
        return ReturnValues.Succeed;
    }

    ReturnValues EvaluacionVida()
    {
        if (health <= 0)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }

    ReturnValues LanzarHechizo()
    {
        if (lanzarHechizo == true)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }

    ReturnValues GastarRecursosP()
    {
        if (gastarRecursos == true)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }

    ReturnValues InvocarBossP()
    {
        if (invocarBoss == true)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }

    ReturnValues BolaDeFuegoP()
    {
        if (bolaDeFuego == true)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }

    ReturnValues InhabilitarTorreP()
    {
        if (inhabilitarTorre == true)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }

    ReturnValues InvocarMoscasP()
    {
        if (invocarMoscas == true)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }


    void AlwaysUseless()
    {

    }

    void Morir()
    {
        print("He muerto");
    }

    void InvocarPM()
    {
        print("He invocado enemigos pequeños y medianos");
    }

    void GastarRecursos()
    {
        print("He gastado recursos");
    }

    void InvocarBoss()
    {
        print("He invocado Boss");
    }

    void BolaDeFuego()
    {
        print("He lanzado bola de fuego");
    }

    void InhabilitarTorre()
    {
        print("He inhabilitado torre");
    }

    void InvocarMoscas()
    {
        print("He invocado moscas");
    }

    void AumentarVelocidad()
    {
        print("He aumentado velocidad");
    }

    void Idle()
    {
        print("Estoy en Idle");
    }

}
