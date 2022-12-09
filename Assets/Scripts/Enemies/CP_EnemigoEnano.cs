using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CP_EnemigoEnano : MonoBehaviour
{
    BehaviourTreeEngine BT_enEnano;

    GameManager gameManager;

    [Header("Components")]
    public NavMeshAgent nav;
    Animator anim;

    [Header("External GameObjects")]
    public GameObject alert;

    [Header("Stats")]
    public int health;
    public int range;
    public int speed;
    public int damage;
    public float hitRate;
    float hitRateTimer;
    public float scareRate;
    float scareRateTimer;

    [Header("Checks variables")]
    public bool towerInRangeCheck;
    public GameObject towerInRange;
    [Space]
    public Vector3 spawnPos;
    public GameObject enemyMedium;

    void Start()
    {
        gameManager = GameManager.instance;

        nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(Vector3.zero);
        nav.speed = speed;

        anim = GetComponent<Animator>();
        anim.SetBool("isWalk", true);
        anim.SetBool("isHit", false);

        hitRateTimer = hitRate;
        scareRateTimer = scareRate;

        spawnPos = new Vector3(transform.position.x, 0, transform.position.z);

        CreateBT();
    }

    void Update()
    {
        hitRateTimer += Time.deltaTime;

        alert.SetActive(enemyMedium);

        if (!enemyMedium)
        {
            nav.SetDestination(Vector3.zero);
            scareRateTimer += Time.deltaTime;
        }

        towerInRangeCheck = false;
        towerInRange = null;
        foreach (GameObject tower in gameManager.towers)
        {
            if (tower.GetComponent<Wall>())
            {
                if (Vector3.Distance(transform.position, tower.transform.position) < range)
                {
                    towerInRangeCheck = true;
                    towerInRange = tower;
                    break;
                }
            }
        }

        if (Vector3.Distance(transform.position, Vector3.zero) < 3)
        {
            print("Llegué");
            Destroy(gameObject);
        }

        BT_enEnano.Update();
    }

    void CreateBT()
    {
        BT_enEnano = new BehaviourTreeEngine(false);

        //CONTENIDO 1 , Fila 7
        //¿Hay peligro?
        LeafNode HayPeligro = BT_enEnano.CreateLeafNode("HayPeligro", AccionNula, HayPeligros);
        //¿Me cortan el paso?
        //LeafNode MeCortanPaso = BT_enEnano.CreateLeafNode("MeCortanPaso", AccionNula, MeCortanElPaso);
        //¿Me pueden ayudar?
        //LeafNode MeAyudan = BT_enEnano.CreateLeafNode("MeAyudan", AccionNula, MePuedenAyudar);
        //Accion de parar
        LeafNode Parar = BT_enEnano.CreateLeafNode("Parar", PararA, EvaluacionNula);

        //CONTENIDO 2 Fila 6
        //Flecha que tiene el contenido 1
        SequenceNode AvanzoONo = BT_enEnano.CreateSequenceNode("AvanzoONo", false);
        AvanzoONo.AddChild(HayPeligro);
        //AvanzoONo.AddChild(MeCortanPaso);
        //AvanzoONo.AddChild(MeAyudan);
        AvanzoONo.AddChild(Parar);
        //Accion Avanzar
        LeafNode Avanzar = BT_enEnano.CreateLeafNode("Avanzar", AvanzarA, EvaluacionNula);
        
        //CONTENIDO 3 Fila 6
        //¿Tengo que atacar?
        LeafNode TengoAtacar = BT_enEnano.CreateLeafNode("TengoAtacar", AccionNula, Atacando);
        //Atacar
        LeafNode Atacar = BT_enEnano.CreateLeafNode("Atacar", AtacarA, EvaluacionNula);

        //CONTENIDO 4 Fila 5
        //Flecha que tiene el contenido 3
        SequenceNode AtacarONo = BT_enEnano.CreateSequenceNode("AtacoONo", false);
        AtacarONo.AddChild(TengoAtacar);
        AtacarONo.AddChild(Atacar);
        //Interrogacion que tiene el contenido 2
        SelectorNode SiNoAtaco = BT_enEnano.CreateSelectorNode("NodoNoAtaque");
        SiNoAtaco.AddChild(AvanzoONo);
        SiNoAtaco.AddChild(Avanzar);


        //CONTENIDO 5 Fila 4
        //¿Tengo vida?
        LeafNode TengoVida = BT_enEnano.CreateLeafNode("TengoVida", AccionNula, VidaActual);
        //Interrogacion que tiene contenido 4
        SelectorNode SiEstoyVivo = BT_enEnano.CreateSelectorNode("NodoVida");
        SiEstoyVivo.AddChild(AtacarONo);
        SiEstoyVivo.AddChild(SiNoAtaco);


        //CONTENIDO 6 Fila 3
        //Flecha que tiene el contenido 5
        SequenceNode VivirOMorir = BT_enEnano.CreateSequenceNode("VivoOMuero", false);
        VivirOMorir.AddChild(TengoVida);
        VivirOMorir.AddChild(SiEstoyVivo);
        //Accion morir
        LeafNode Morir = BT_enEnano.CreateLeafNode("Morir", MorirA, EvaluacionNula);

        //Interrogacion que tiene contenido 6 Fila 2
        SelectorNode NodoPadre = BT_enEnano.CreateSelectorNode("NodoPadre");
        NodoPadre.AddChild(VivirOMorir);
        NodoPadre.AddChild(Morir);
        //Loop Fila 1
        TreeNode Loop = BT_enEnano.CreateLoopNode("Loop", NodoPadre);
        BT_enEnano.SetRootNode(Loop);
    }

    void MorirA()
    {
        print("Morir");

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out hit, 1000, LayerMask.GetMask("Ground")))
        {
            GameObject deadTile = hit.collider.gameObject;

            deadTile.GetComponent<GroundInfo>().numKills++;
        }

        gameManager.totalKilss++;

        Destroy(gameObject);
    }
    void AtacarA()
    {
        print("Atacar");
        if (towerInRange)
        {
            nav.SetDestination(towerInRange.transform.position);
        }
        
        if (hitRateTimer >= hitRate)
        {
            if (Vector3.Distance(transform.position, towerInRange.transform.position) < range)
            {
                hitRateTimer = 0;

                towerInRange.gameObject.GetComponent<Wall>().health -= damage;

                anim.SetBool("isWalk", false);
                anim.SetBool("isHit", true);
            }
        }
    }
    void AvanzarA()
    {
        if (!enemyMedium)
        {
            print("Avanzar");
            nav.SetDestination(Vector3.zero);
            nav.speed = speed;

            anim.SetBool("isWalk", true);
            anim.SetBool("isHit", false);
        }
    }
    void PararA()
    {
        print("Reculando");
        nav.SetDestination(spawnPos);
        Invoke("Stop", 2);
    }
    void Stop()
    {
        print("Parar");
        anim.SetBool("isWalk", false);
        anim.SetBool("isHit", false);
        nav.speed = 0;
    }
    void AccionNula()
    {
        // No hace nada
    }

    ReturnValues EvaluacionNula()
    {
        return ReturnValues.Succeed;
    }
    ReturnValues VidaActual()
    {
        print("ComprobandoVida");
        if (health <= 0)
        {
            print("Vida == 0");
            return ReturnValues.Failed;
        }
        else
        {
            print("Vida > 0");
            return ReturnValues.Succeed;
        }
    }
    ReturnValues Atacando()
    {
        if (towerInRange)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }

    ReturnValues HayPeligros()
    {
        RaycastHit[] groundTilesInRange = Physics.SphereCastAll(transform.position, 6, transform.forward, 5, LayerMask.GetMask("Ground"));
        foreach (var groundTile in groundTilesInRange)
        {
            if (groundTile.collider.gameObject.GetComponent<GroundInfo>().numKills > gameManager.totalKilss * 0.05f)
            {
                if (!gameManager.enemiesHelp.Contains(gameObject) && gameManager.numEnemiesMedium > gameManager.enemiesHelp.Count && scareRateTimer >= scareRate)
                {
                    scareRateTimer = 0;

                    gameManager.enemiesHelp.Add(gameObject);

                    return ReturnValues.Succeed;
                }
            }
        }

        if (enemyMedium)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }
}
