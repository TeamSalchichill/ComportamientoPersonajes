using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_BossFinal : MonoBehaviour
{
    BehaviourTreeEngine BT_BossFinal;

    GameManager gameManager;

    [Header("Components")]
    Animator anim;

    [Header("External GameObjects")]
    public GameObject enemySmall;
    public GameObject enemyMedium;
    public GameObject[] bosses;
    public GameObject fireBall;
    public GameObject crossDisable;
    public GameObject fly;

    [Header("Stats")]
    public int health;
    int healthMax;
    [Space]
    public float abilityRate;
    float abilityRateTimer;
    [Space]
    public float fireBallRate;
    float fireBallRateTimer;

    public float disableTowerRate;
    float disableTowerRateTimer;
    GameObject instCross;

    public float invokeFlyRate;
    float invokeFlyRateTimer;
    [Space]
    public int totalResources;
    public float actualResources;
    public float resourceRate;
    float resourceRateTimer;
    [Space]
    public float bossRate;
    float bossRateTimer;

    [Header("Checks variables")]
    public bool canFireBall = true;
    public int bestZoneFireBallMax;
    public Vector3 bestZoneFireBall;
    public bool canDisableTower = true;
    public int bestTowerMax;
    public GameObject bestTower;
    public bool canFlies = true;
    public int bestFliesMax;
    public Vector3 bestFlies;

    GameObject bestTowerAux;

    [Header("Particles")]
    public GameObject particleBoost;

    void Start()
    {
        gameManager = GameManager.instance;

        anim = GetComponent<Animator>();

        healthMax = health;

        abilityRateTimer = abilityRate;

        fireBallRateTimer = fireBallRate / 2;
        disableTowerRateTimer = disableTowerRate / 2;
        invokeFlyRateTimer = invokeFlyRate / 2;

        resourceRateTimer = resourceRate;

        bossRateTimer = bossRate;

        CreateBT();
    }

    void Update()
    {
        abilityRateTimer += Time.deltaTime;

        fireBallRateTimer += Time.deltaTime;
        disableTowerRateTimer += Time.deltaTime;
        invokeFlyRateTimer += Time.deltaTime;

        resourceRateTimer += Time.deltaTime;
        if (health != 0)
        {
            actualResources += (Time.deltaTime * (healthMax / health));
        }

        bossRateTimer += Time.deltaTime;

        bestZoneFireBallMax = 0;
        bestZoneFireBall = Vector3.zero;
        bestTowerMax = 0;
        bestTower = null;
        bestFliesMax = 0;
        bestFlies = Vector3.zero;
        foreach (GameObject tower in gameManager.towers)
        {
            if (tower)
            {
                if (tower.GetComponent<CP_Torres>())
                {
                    CP_Torres towerScript = tower.GetComponent<CP_Torres>();

                    if (towerScript.numTowerNear >= bestZoneFireBallMax)
                    {
                        bestZoneFireBallMax = towerScript.numTowerNear;
                        bestZoneFireBall = tower.transform.position;
                    }
                    if (towerScript.kills >= bestTowerMax)
                    {
                        bestTowerMax = towerScript.kills;
                        bestTower = tower;
                    }
                    if ((towerScript.numTowerNear + towerScript.numEnemiesNear) >= bestFliesMax)
                    {
                        bestFliesMax = (towerScript.numTowerNear + towerScript.numEnemiesNear);
                        bestFlies = tower.transform.position;
                    }
                }
            }
        }

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
        if (abilityRateTimer >= abilityRate)
        {
            abilityRateTimer = 0;
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }

    ReturnValues GastarRecursosP()
    {
        if (totalResources > 0 && resourceRateTimer >= resourceRate && actualResources > 30)
        {
            resourceRateTimer = 0;
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }

    ReturnValues InvocarBossP()
    {
        if (bossRateTimer >= bossRate)
        {
            bossRateTimer = 0;
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }

    ReturnValues BolaDeFuegoP()
    {
        if (fireBallRateTimer >= fireBallRate && bestZoneFireBallMax > 3 && canFireBall)
        {
            fireBallRateTimer = 0;
            canFireBall = false;
            canDisableTower = true;
            canFlies = true;
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }

    ReturnValues InhabilitarTorreP()
    {
        if (disableTowerRateTimer >= disableTowerRate && bestTowerMax > 5 && canDisableTower && bestTower)
        {
            disableTowerRateTimer = 0;
            canFireBall = true;
            canDisableTower = false;
            canFlies = true;
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Failed;
        }
    }

    ReturnValues InvocarMoscasP()
    {
        if (invokeFlyRateTimer >= invokeFlyRate && bestFliesMax > 0 && canFlies)
        {
            invokeFlyRateTimer = 0;
            canFireBall = true;
            canDisableTower = true;
            canFlies = false;
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
        Destroy(gameObject);
    }

    void InvocarPM()
    {
        print("He invocado enemigos pequeños y medianos");
        int delayTime;
        if (bestTower)
        {
            delayTime = (int)(Vector3.Distance(transform.position, bestTower.transform.position) / 10);
        }
        else
        {
            delayTime = 1;
        }

        int numMedium = (gameManager.towers.Length / 2);
        numMedium = (int)Mathf.Min(numMedium, (actualResources - 10) / 5);
        actualResources -= (numMedium * 5);

        int numSmall = (int)actualResources;
        actualResources = 0;

        anim.SetTrigger("doInvoke");

        StartCoroutine(InvokeEnemies(numMedium, numSmall, delayTime));
    }
    IEnumerator InvokeEnemies(int numMedium, int numSmall, int time)
    {
        for (int i = 0; i < numMedium; i++)
        {
            Instantiate(enemyMedium, new Vector3(transform.position.x, 0, transform.position.z) + (transform.forward * 2), transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(time);
        for (int i = 0; i < numSmall; i++)
        {
            Instantiate(enemySmall, new Vector3(transform.position.x, 0, transform.position.z) + (transform.forward * 2), transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }
    }

    void InvocarBoss()
    {
        print("He invocado Boss");
        anim.SetTrigger("doInvoke");
        GameObject instBoss = Instantiate(bosses[Random.Range(0, bosses.Length)], new Vector3(transform.position.x, 0, transform.position.z) + (transform.forward * 2), transform.rotation);
        if (instBoss.GetComponent<CP_Boss1_Invocador>())
        {
            instBoss.GetComponent<CP_Boss1_Invocador>().myBossFinal = this;
        }
        if (instBoss.GetComponent<CP_Boss1_Invocador>())
        {
            instBoss.GetComponent<CP_Boss1_Invocador>().myBossFinal = this;
        }
    }

    void BolaDeFuego()
    {
        print("He lanzado bola de fuego");
        anim.SetTrigger("doFireBall");
        Instantiate(fireBall, new Vector3(bestZoneFireBall.x, 25, bestZoneFireBall.z), transform.rotation);
    }

    void InhabilitarTorre()
    {
        print("He inhabilitado torre");
        bestTowerAux = bestTower;
        bestTowerAux.GetComponent<CP_Torres>().fireRate += 100;
        instCross = Instantiate(crossDisable, new Vector3(bestTowerAux.transform.position.x, 5, bestTowerAux.transform.position.z), transform.rotation);
        anim.SetTrigger("doDisableTower");
        Invoke("EnableTower", 5);
    }
    void EnableTower()
    {
        bestTowerAux.GetComponent<CP_Torres>().fireRate -= 100;
        Destroy(instCross);
    }

    void InvocarMoscas()
    {
        print("He invocado moscas");
        anim.SetTrigger("doInvoke");
        StartCoroutine(InvokeFlies());
    }
    IEnumerator InvokeFlies()
    {
        for (int i = 0; i < 5; i++)
        {
            Instantiate(fly, new Vector3(transform.position.x, 0, transform.position.z) + (transform.forward * 2), transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }
    }

    void AumentarVelocidad()
    {
        print("He aumentado velocidad");
        anim.SetTrigger("doBoostSpeed");
        foreach (var enemy in gameManager.enemies)
        {
            if (enemy.GetComponent<CP_EnemigoEnano>())
            {
                enemy.GetComponent<CP_EnemigoEnano>().nav.speed += 2;
            }
            if (enemy.GetComponent<CP_EnemigoMediano>())
            {
                enemy.GetComponent<CP_EnemigoMediano>().nav.speed += 2;
            }
            if (enemy.GetComponent<CP_Boss1_Invocador>())
            {
                enemy.GetComponent<CP_Boss1_Invocador>().nav.speed += 2;
            }
        }

        Instantiate(particleBoost, transform.position + new Vector3(0, 3, 0), transform.rotation);
        Invoke("NerfSpeed", 5);
    }
    void NerfSpeed()
    {
        foreach (var enemy in gameManager.enemies)
        {
            if (enemy.GetComponent<CP_EnemigoEnano>())
            {
                enemy.GetComponent<CP_EnemigoEnano>().nav.speed -= 2;
            }
            if (enemy.GetComponent<CP_EnemigoMediano>())
            {
                enemy.GetComponent<CP_EnemigoMediano>().nav.speed -= 2;
            }
        }
    }

    void Idle()
    {
        print("Estoy en Idle");
    }

    private void OnMouseDown()
    {
        gameManager.infoPanel.SetActive(true);
        gameManager.info.text =
            "Nombre: Boss final \n" + 
            "Vida: " + health + "\n" +
            "Vida máxima: " + healthMax + "\n" +
            "Tiempo para lanzar habilidad: " + abilityRate + "\n" +
            "Tiempo para lanzar habilidad (Temporizador): " + abilityRateTimer + "\n" +
            "Tiempo para lanzar bola de fuego: " + fireBallRate + "\n" +
            "Tiempo para lanzar bola de fuego (Temporizador): " + fireBallRateTimer + "\n" +
            "Tiempo para deshabilitar torre: " + disableTowerRate + "\n" +
            "Tiempo para deshabilitar torre (Temporizador): " + disableTowerRateTimer + "\n" +
            "Tiempo para invocar moscas: " + invokeFlyRate + "\n" +
            "Tiempo para invocar moscas (Temporizador): " + invokeFlyRateTimer + "\n" +
            "Cantidad total de recursos: " + totalResources + "\n" +
            "Cantidad de recursos actual: " + actualResources + "\n" +
            "Tiempo para invocar un boss: " + bossRate + "\n" +
            "Tiempo para invocar un boss (Temporizador): " + bossRateTimer + "\n"
        ;
    }
}
