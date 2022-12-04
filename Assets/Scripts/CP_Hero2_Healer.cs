using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Hero2_Healer : MonoBehaviour
{
    BehaviourTreeEngine BT_Heroe2;

    GameManager gameManager;

    [Header("External GameObjects")]
    public GameObject bullet;

    [Header("Stats")]
    public int health;
    int healthMax;
    public int range;
    public int damage;
    public float fireRate;
    float fireRateTimer;
    [Space]
    public int curation;
    public float curationRate;
    float curationRateTimer;
    List<GameObject> enemiesInRange;
    [Space]
    public float fireRateBoost;
    public float fireRateBoostRate;
    float fireRateBoostRateTimer;
    [Space]
    public GameObject bossToParalize;

    [Header("Checks variables")]
    public bool enemyInRangeCheck;
    public GameObject enemyInRange;

    public bool towerInRangeBoostCheck;
    public GameObject towerInRangeBoost;

    public bool towerInRangeCurationCheck;
    public GameObject towerInRangeCuration;

    void Start()
    {
        gameManager = GameManager.instance;

        healthMax = health;

        fireRateTimer = fireRate;
        curationRateTimer = curationRate;
        fireRateBoostRateTimer = fireRateBoostRate;

        CreateBT();
    }
    void Update()
    {
        fireRateTimer += Time.deltaTime;
        curationRateTimer += Time.deltaTime;
        fireRateBoostRateTimer += Time.deltaTime;

        enemyInRangeCheck = false;
        enemyInRange = null;
        enemiesInRange = new List<GameObject>();
        foreach (GameObject enemy in gameManager.enemies)
        {
            if (enemy)
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) < range)
                {
                    if (!enemyInRangeCheck)
                    {
                        enemyInRangeCheck = true;
                        enemyInRange = enemy;
                    }

                    if (enemy.GetComponent<CP_Boss1_Invocador>() || enemy.GetComponent<CP_Boss2_Atacante>())
                    {
                        bossToParalize = enemy;
                    }

                    enemiesInRange.Add(enemy);
                }
            }
        }

        towerInRangeBoostCheck = false;
        towerInRangeBoost = null;
        int numMaxKills = 0;
        towerInRangeCurationCheck = false;
        towerInRangeCuration = null;
        int numMaxHealth = 1000000;
        foreach (GameObject tower in gameManager.towers)
        {
            if (Vector3.Distance(transform.position, tower.transform.position) < range)
            {
                if (tower.GetComponent<CP_Torres>())
                {
                    if (numMaxKills <= tower.GetComponent<CP_Torres>().kills)
                    {
                        numMaxKills = tower.GetComponent<CP_Torres>().kills;

                        towerInRangeBoostCheck = true;
                        towerInRangeBoost = tower;
                    }
                    if (numMaxHealth >= tower.GetComponent<CP_Torres>().health && tower.GetComponent<CP_Torres>().health < tower.GetComponent<CP_Torres>().healthMax)
                    {
                        numMaxHealth = tower.GetComponent<CP_Torres>().health;

                        towerInRangeCurationCheck = true;
                        towerInRangeCuration = tower;
                    }
                }
            }
        }

        BT_Heroe2.Update();
    }

    void CreateBT()
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

    void circuloAmarillo()
    {
        //print("soy una bola amarilla, es decir una pregunta ");
    }
    void Morir()
    {
        print("estoy muerto");
        Destroy(gameObject);
    }
    void Aumentar()
    {
        print("aumento velocidad");
        fireRateBoostRateTimer = 0;
        towerInRangeBoost.GetComponent<CP_Torres>().fireRate *= fireRateBoost;
    }
    void Paralizar()
    {
        print("paralizo");

    }
    void Atacar()
    {
        print("Recargando");

        print("Ataco");
        fireRateTimer = 0;

        GameObject instBullet = Instantiate(bullet, transform.position, transform.rotation);
        instBullet.GetComponent<CP_Bullet_Tower>().Seek(enemyInRange.transform);
    }
    void Curar()
    {
        if (curationRateTimer >= curationRate)
        {
            print("curar");
            curationRateTimer = 0;
            towerInRangeCuration.GetComponent<CP_Torres>().health += curation;
        }
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
        if (fireRateBoostRateTimer >= fireRateBoostRate)
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
        if (enemyInRange && fireRateTimer >= fireRate)
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
        if (health < healthMax && bossToParalize)
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

        if (towerInRangeCuration && enemiesInRange.Count == 0)
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