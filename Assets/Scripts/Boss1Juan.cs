using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Juan : MonoBehaviour
{
    double vida = 10;
    BehaviourTreeEngine Boss1;

    void Start()
    {
        CreateBT();
    }

    // Update is called once per frame
    void Update()
    {
        Boss1.Update();
        if (vida > 0)
        {
            vida = vida - 0.1;
        }
    }
    void CreateBT()
    {
        Boss1= new BehaviourTreeEngine(false);

        //CONTENIDO 1
        //Vida<=0
        LeafNode VidaActual = Boss1.CreateLeafNode("VidaActual", AccionNula, VidaActualP);
        //AccionMorir
        LeafNode Morir = Boss1.CreateLeafNode("Morir", MorirA, EvaluacionNula);

        //CONTENIDO 2
        //Torre principal en rango
        LeafNode TorrePrinEnRango = Boss1.CreateLeafNode("TorrePrinRango", AccionNula, TorrePrinEnRangoP);
        //AccionCorrer
        LeafNode Correr = Boss1.CreateLeafNode("Correr", CorrerA, EvaluacionNula);

        //CONTENIDO 3
        //Torreta en alcance
        LeafNode TorreEnCamino = Boss1.CreateLeafNode("TorreCamino", AccionNula, TorreEnCaminoP);
        //Accion Correr e Invocar
        LeafNode Correr1 = Boss1.CreateLeafNode("Correr1", CorrerA1, EvaluacionNula);
        LeafNode Invocar = Boss1.CreateLeafNode("Invocar", InvocarA, EvaluacionNula);

        //CONTENIDO 4
        //Almacenamiento maximo alcanzado
        LeafNode AlmacMaxAlcan = Boss1.CreateLeafNode("Almacenamiento", AccionNula, AlmacMaxAlcanP);
        //Accion Correr e Invocar
        LeafNode Correr2 = Boss1.CreateLeafNode("Correr2", CorrerA2, EvaluacionNula);
        LeafNode Invocar1 = Boss1.CreateLeafNode("Invocar1", InvocarA1, EvaluacionNula);

        //CONTENIDO 5
        //Flecha que tiene contenido 2
        SequenceNode TorrePrinEnRangoONo = Boss1.CreateSequenceNode("TorrePrin", false);
        TorrePrinEnRangoONo.AddChild(TorrePrinEnRango);
        TorrePrinEnRangoONo.AddChild(Correr);
        //Flecha que tiene contenido 3
        SequenceNode TorreEnCaminoONo = Boss1.CreateSequenceNode("Torre", false);
        TorreEnCaminoONo.AddChild(TorreEnCamino);
        TorreEnCaminoONo.AddChild(Correr1);
        TorreEnCaminoONo.AddChild(Invocar);
        //Flecha que tiene contenido 4
        SequenceNode AlmacenamientoAlMaxONo = Boss1.CreateSequenceNode("AlmacenamientoMax", false);
        AlmacenamientoAlMaxONo.AddChild(AlmacMaxAlcan);
        AlmacenamientoAlMaxONo.AddChild(Correr2);
        AlmacenamientoAlMaxONo.AddChild(Invocar1);
        //Accion Almacenar
        LeafNode Almacenar = Boss1.CreateLeafNode("Almacenar", AlmacenarA, EvaluacionNula);

        //CONTENIDO 6
        //Temporizado r==0
        LeafNode Temporizador = Boss1.CreateLeafNode("Temporizador", AccionNula, TemporizadorP);
        //Interrogacion que tiene contenido 5
        SelectorNode NodoTemporizador = Boss1.CreateSelectorNode("NodoTemporizador");
        NodoTemporizador.AddChild(TorrePrinEnRangoONo);
        NodoTemporizador.AddChild(TorreEnCaminoONo);
        NodoTemporizador.AddChild(AlmacenamientoAlMaxONo);
        NodoTemporizador.AddChild(Almacenar);


        //CONTENIDO 7
        //Torre en rango
        LeafNode TorreEnRango = Boss1.CreateLeafNode("TorreRango", AccionNula, TorreEnRangoP);
        //En movimiento
        LeafNode EnMovimiento = Boss1.CreateLeafNode("EnMovimiento", AccionNula, EnMovimientoP);
        //Parar
        LeafNode Parar = Boss1.CreateLeafNode("Parar", PararA, EvaluacionNula);

        //CONTENIDO 8
        //Torre en rango
        LeafNode TorreEnRango1 = Boss1.CreateLeafNode("TorreRango1", AccionNula, TorreEnRangoP1);
        //Accion Atacar
        LeafNode Atacar = Boss1.CreateLeafNode("Atacar", AtacarA, EvaluacionNula);

        //CONTENIDO 9
        //Torre en rango inverter
        LeafNode TorreNoEnRango = Boss1.CreateLeafNode("TorreRango2", AccionNula, TorreNoEnRangoP);
        //Accion Avanzar
        LeafNode Avanzar = Boss1.CreateLeafNode("Avanzar", AvanzarA, EvaluacionNula);

        //CONTENIDO 10
        //Flecha que tiene contenido 7
        SequenceNode ParoAnteTorre = Boss1.CreateSequenceNode("ParoAnteTorree", false);
        ParoAnteTorre.AddChild(TorreEnRango);
        ParoAnteTorre.AddChild(EnMovimiento);
        ParoAnteTorre.AddChild(Parar);
        //Flecha que tiene contenido 8
        SequenceNode AtacoTorre = Boss1.CreateSequenceNode("AtacoTorre", false);
        AtacoTorre.AddChild(TorreEnRango1);
        AtacoTorre.AddChild(Atacar);


        //Flecha que tiene contenido 9
        SequenceNode Avanzo = Boss1.CreateSequenceNode("Avanzo", false);
        Avanzo.AddChild(TorreNoEnRango);
        Avanzo.AddChild(Avanzar);


        //CONTENIDO 11
        //Flecha que tiene contenido 1
        SequenceNode MuerteONo = Boss1.CreateSequenceNode("MuerteONo", false);
        MuerteONo.AddChild(VidaActual);
        MuerteONo.AddChild(Morir);
        //Flecha que tiene contenido 6
        SequenceNode TemporizadorA0 = Boss1.CreateSequenceNode("Temporizador0", false);
        TemporizadorA0.AddChild(Temporizador);
        TemporizadorA0.AddChild(NodoTemporizador);
        //Interrogacion que tiene contenido 10
        SelectorNode NodoUltimo = Boss1.CreateSelectorNode("NodoUltimo");
        NodoUltimo.AddChild(ParoAnteTorre);
        NodoUltimo.AddChild(AtacoTorre);
        NodoUltimo.AddChild(Avanzo);


        //Interrogacion que tiene Contenido 11
        SelectorNode NodoPadre = Boss1.CreateSelectorNode("NodoPapa");
        NodoPadre.AddChild(MuerteONo);
        NodoPadre.AddChild(TemporizadorA0);
        NodoPadre.AddChild(NodoUltimo);

        //Loop
        TreeNode Loop = Boss1.CreateLoopNode("Loop", NodoPadre);
        Boss1.SetRootNode(Loop);

    }
    void AccionNula()
    {
        //No hace nada
    }

    ReturnValues VidaActualP()
    {
        return ReturnValues.Failed;
    }

    ReturnValues TemporizadorP()
    {
        return ReturnValues.Failed;
    }

    ReturnValues TorrePrinEnRangoP()
    {
        return ReturnValues.Failed;
    }

    ReturnValues TorreEnRangoP()
    {
        return ReturnValues.Succeed;
    }

    ReturnValues TorreEnRangoP1()
    {
        return ReturnValues.Failed;
    }

    ReturnValues TorreNoEnRangoP()
    {
        return ReturnValues.Succeed;
    }

    ReturnValues AlmacMaxAlcanP()
    {
        return ReturnValues.Failed;
    }

    ReturnValues TorreEnCaminoP()
    {
        return ReturnValues.Failed;
    }

    ReturnValues EnMovimientoP()
    {
        return ReturnValues.Failed;
    }

    ReturnValues EvaluacionNula()
    {
        return ReturnValues.Succeed;
    }

    void MorirA()
    {
        print("Morir");
    }

    void CorrerA()
    {
        print("Correr");
    }

    void CorrerA1()
    {
        print("Correr1");
    }
    void CorrerA2()
    {
        print("Correr2");
    }


    void InvocarA()
    {
        print("Invocar");
    }

    void InvocarA1()
    {
        print("Invocar1");
    }

    void AlmacenarA()
    {
        print("Almacenar");
    }

    void PararA()
    {
        print("Parar");
    }

    void AtacarA()
    {
        print("Atacar");
    }

    void AvanzarA()
    {
        print("Avanzar");
    }

}
