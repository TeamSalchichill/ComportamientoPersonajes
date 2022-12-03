using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_EnemigoEnano : MonoBehaviour
{
    double vida=10;
    BehaviourTreeEngine enemigoEnano;

    void Start()
    {
        CreateBT();
    }

    void Update()
    {
        enemigoEnano.Update();
        if (vida > 0)
        {
            vida = vida - 0.1;
        }
    }
    void CreateBT()
    {
        enemigoEnano = new BehaviourTreeEngine(false);

        //CONTENIDO 1 , Fila 7
        //¿Hay peligro?
        LeafNode HayPeligro = enemigoEnano.CreateLeafNode("HayPeligro", AccionNula, HayPeligros);
        //¿Me cortan el paso?
        LeafNode MeCortanPaso = enemigoEnano.CreateLeafNode("MeCortanPaso", AccionNula, MeCortanElPaso);
        //¿Me pueden ayudar?
        LeafNode MeAyudan = enemigoEnano.CreateLeafNode("MeAyudan", AccionNula, MePuedenAyudar);
        //Accion de parar
        LeafNode Parar = enemigoEnano.CreateLeafNode("Parar", PararA, EvaluacionNula);

        //CONTENIDO 2 Fila 6
        //Flecha que tiene el contenido 1
        SequenceNode AvanzoONo = enemigoEnano.CreateSequenceNode("AvanzoONo", false);
        AvanzoONo.AddChild(HayPeligro);
        AvanzoONo.AddChild(MeCortanPaso);
        AvanzoONo.AddChild(MeAyudan);
        AvanzoONo.AddChild(Parar);
        //Accion Avanzar
        LeafNode Avanzar = enemigoEnano.CreateLeafNode("Avanzar", AvanzarA, EvaluacionNula);
        
        //CONTENIDO 3 Fila 6
        //¿Tengo que atacar?
        LeafNode TengoAtacar = enemigoEnano.CreateLeafNode("TengoAtacar", AccionNula, Atacando);
        //Atacar
        LeafNode Atacar = enemigoEnano.CreateLeafNode("Atacar", AtacarA, EvaluacionNula);

        //CONTENIDO 4 Fila 5
        //Flecha que tiene el contenido 3
        SequenceNode AtacarONo = enemigoEnano.CreateSequenceNode("AtacoONo", false);
        AtacarONo.AddChild(TengoAtacar);
        AtacarONo.AddChild(Atacar);
        //Interrogacion que tiene el contenido 2
        SelectorNode SiNoAtaco = enemigoEnano.CreateSelectorNode("NodoNoAtaque");
        SiNoAtaco.AddChild(AvanzoONo);
        SiNoAtaco.AddChild(Avanzar);


        //CONTENIDO 5 Fila 4
        //¿Tengo vida?
        LeafNode TengoVida = enemigoEnano.CreateLeafNode("TengoVida", AccionNula, VidaActual);
        //Interrogacion que tiene contenido 4
        SelectorNode SiEstoyVivo = enemigoEnano.CreateSelectorNode("NodoVida");
        SiEstoyVivo.AddChild(AtacarONo);
        SiEstoyVivo.AddChild(SiNoAtaco);


        //CONTENIDO 6 Fila 3
        //Flecha que tiene el contenido 5
        SequenceNode VivirOMorir = enemigoEnano.CreateSequenceNode("VivoOMuero", false);
        VivirOMorir.AddChild(TengoVida);
        VivirOMorir.AddChild(SiEstoyVivo);
        //Accion morir
        LeafNode Morir = enemigoEnano.CreateLeafNode("Morir", MorirA, EvaluacionNula);

        //Interrogacion que tiene contenido 6 Fila 2
        SelectorNode NodoPadre = enemigoEnano.CreateSelectorNode("NodoPadre");
        NodoPadre.AddChild(VivirOMorir);
        NodoPadre.AddChild(Morir);
        //Loop Fila 1
        TreeNode Loop = enemigoEnano.CreateLoopNode("Loop", NodoPadre);
        enemigoEnano.SetRootNode(Loop);
    }

    void MorirA()
    {
        print("Morir");
    }
    void AtacarA()
    {
        print("Atacar");
    }
    void AvanzarA()
    {
        print("Avanzar");
    }
    void PararA()
    {
        print("Parar");
    }
    void AccionNula()
    {
        //No hace nada
    }

    ReturnValues EvaluacionNula()
    {
        return ReturnValues.Succeed;
    }
    ReturnValues VidaActual()
    {
        print("ComprobandoVida");
        if (vida < 0)
        {
            print("Vida=0");
            return ReturnValues.Failed;
        }
        else
        {
            print("Vida>0");
            return ReturnValues.Succeed;
        }
    }

    ReturnValues Atacando()
    {
        return ReturnValues.Failed;
    }

    ReturnValues HayPeligros()
    {
        return ReturnValues.Succeed;
    }

    ReturnValues MeCortanElPaso()
    {
        return ReturnValues.Succeed;
    }

    ReturnValues MePuedenAyudar()
    {
        return ReturnValues.Succeed;
    }
}
