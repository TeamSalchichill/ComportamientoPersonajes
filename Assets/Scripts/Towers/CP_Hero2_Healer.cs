using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Hero2_Healer : MonoBehaviour
{
    BehaviourTreeEngine BT_Heroe2;

    GameManager gameManager;

    Animator anim;

    [Header("External GameObjects")]
    public GameObject bullet;
    public GameObject partToRotate;
    public GameObject bulletPos;

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
    public float paralizeRate;
    public float paralizeRateTimer;
    public GameObject bossToParalize;
    public GameObject bossToParalizeAux;

    [Header("Checks variables")]
    public bool enemyInRangeCheck;
    public GameObject enemyInRange;

    public bool towerInRangeBoostCheck;
    public GameObject towerInRangeBoost;

    public bool towerInRangeCurationCheck;
    public GameObject towerInRangeCuration;

    [Header("Particles")]
    public GameObject particleBoost;
    public GameObject particleCuration;
    public GameObject particleParalize;
    public GameObject particleDestruction;

    void Start()
    {
        gameManager = GameManager.instance;

        anim = GetComponent<Animator>();

        healthMax = health;

        fireRateTimer = fireRate;
        curationRateTimer = curationRate;
        fireRateBoostRateTimer = fireRateBoostRate;
        paralizeRateTimer = paralizeRate;

        CreateBT();
    }
    void Update()
    {
        fireRateTimer += Time.deltaTime;
        curationRateTimer += Time.deltaTime;
        fireRateBoostRateTimer += Time.deltaTime;
        paralizeRateTimer += Time.deltaTime;

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

                        Vector3 dir = enemyInRange.transform.position - transform.position;
                        Quaternion lookRotation = Quaternion.LookRotation(dir);
                        Vector3 rotation = Quaternion.Lerp(partToRotate.transform.rotation, lookRotation, Time.deltaTime * 10).eulerAngles;
                        partToRotate.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
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
            if (tower)
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
        Instantiate(particleDestruction, transform.position + new Vector3(0, 3, 0), transform.rotation);
        print("estoy muerto");
        Destroy(gameObject);
    }
    void Aumentar()
    {
        print("aumento velocidad");
        fireRateBoostRateTimer = 0;
        towerInRangeBoost.GetComponent<CP_Torres>().fireRate *= fireRateBoost;

        anim.SetTrigger("doHit");
        Instantiate(particleBoost, towerInRangeBoost.transform.position + new Vector3(0, 3, 0), transform.rotation);
        Instantiate(particleBoost, transform.position + new Vector3(0, 3, 0), transform.rotation);
    }
    void Paralizar()
    {
        print("paralizo");

        paralizeRateTimer = 0;

        bossToParalizeAux = bossToParalize;

        if (bossToParalizeAux.GetComponent<CP_Boss1_Invocador>())
        {
            bossToParalizeAux.GetComponent<CP_Boss1_Invocador>().nav.speed = 0;
            bossToParalizeAux.GetComponent<CP_Boss1_Invocador>().hitRate += 1000;
            bossToParalizeAux.GetComponent<CP_Boss1_Invocador>().abilityRate += 1000;
            bossToParalizeAux.GetComponent<CP_Boss1_Invocador>().anim.speed = 0;

            bossToParalizeAux.GetComponent<CP_Boss1_Invocador>().DisableParalizePublic();
        }
        if (bossToParalizeAux.GetComponent<CP_Boss2_Atacante>())
        {
            bossToParalizeAux.GetComponent<CP_Boss2_Atacante>().nav.speed = 0;
            bossToParalizeAux.GetComponent<CP_Boss2_Atacante>().hitRate += 1000;
            bossToParalizeAux.GetComponent<CP_Boss2_Atacante>().furyRate += 1000;
            bossToParalizeAux.GetComponent<CP_Boss2_Atacante>().anim.speed = 0;

            bossToParalizeAux.GetComponent<CP_Boss2_Atacante>().DisableParalizePublic();
        }

        Instantiate(particleParalize, bossToParalizeAux.transform.position + new Vector3(0, 3, 0), transform.rotation);
        Instantiate(particleParalize, transform.position + new Vector3(0, 3, 0), transform.rotation);

        //Invoke("DisableParalize", 5);
    }
    /*
    void DisableParalize()
    {
        if (bossToParalizeAux.GetComponent<CP_Boss1_Invocador>())
        {
            bossToParalizeAux.GetComponent<CP_Boss1_Invocador>().nav.speed = 2;
            bossToParalizeAux.GetComponent<CP_Boss1_Invocador>().hitRate -= 1000;
            bossToParalizeAux.GetComponent<CP_Boss1_Invocador>().abilityRate -= 1000;
            bossToParalizeAux.GetComponent<CP_Boss1_Invocador>().anim.speed = 1;
        }
        if (bossToParalizeAux.GetComponent<CP_Boss2_Atacante>())
        {
            bossToParalizeAux.GetComponent<CP_Boss2_Atacante>().nav.speed = 2;
            bossToParalizeAux.GetComponent<CP_Boss2_Atacante>().hitRate -= 1000;
            bossToParalizeAux.GetComponent<CP_Boss2_Atacante>().furyRate -= 1000;
            bossToParalizeAux.GetComponent<CP_Boss2_Atacante>().anim.speed = 1;
        }
    }
    */
    void Atacar()
    {
        print("Recargando");

        print("Ataco");
        fireRateTimer = 0;

        GameObject instBullet = Instantiate(bullet, bulletPos.transform.position, transform.rotation);
        instBullet.GetComponent<CP_Bullet_Tower>().Seek(enemyInRange.transform);

        anim.SetTrigger("doHit");
    }
    void Curar()
    {
        if (curationRateTimer >= curationRate)
        {
            print("curar");
            curationRateTimer = 0;
            towerInRangeCuration.GetComponent<CP_Torres>().health += curation;

            anim.SetTrigger("doHit");
            Instantiate(particleCuration, towerInRangeCuration.transform.position + new Vector3(0, 3, 0), transform.rotation);
            Instantiate(particleCuration, transform.position + new Vector3(0, 3, 0), transform.rotation);
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
        if (health < healthMax && bossToParalize && paralizeRateTimer >= paralizeRate)
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

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ImpactDamage")
        {
            health -= 250;
        }
    }

    private void OnMouseDown()
    {
        gameManager.infoPanel.SetActive(true);
        gameManager.info.text =
            "Nombre: Héroe 2 - Sanador \n" +
            "Vida: " + health + "\n" +
            "Rango: " + range + "\n" +
            "Daño: " + damage + "\n" +
            "Velocidad de disparo: " + fireRate + "\n" +
            "Curación: " + curation + "\n" +
            "Enfriamiento curación: " + curationRate + "\n" + 
            "Enfriamiento aumento: " + fireRateBoostRate + "\n" + 
            "Enfriamiento paralizar: " + paralizeRate + "\n"
            ;
    }
}