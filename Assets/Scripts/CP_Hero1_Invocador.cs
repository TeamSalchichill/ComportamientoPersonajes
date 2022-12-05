using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Hero1_Invocador : MonoBehaviour
{
    StateMachineEngine FSM_Hero1;

    GameManager gameManager;

    [Header("External GameObjects")]
    public GameObject bullet;
    public GameObject wall;

    [Header("Stats")]
    public int health;
    public int range;
    public int damage;
    public float fireRate;
    float fireRateTimer;
    [Space]
    public int enemiesHealthToinvoke;
    int enemiesHealth;
    public bool wallAlive;
    [Space]
    public float wallRate;
    public float wallRateTimer;

    [Header("Checks variables")]
    public bool enemyInRangeCheck;
    public GameObject enemyInRange;

    void Start()
    {
        gameManager = GameManager.instance;

        fireRateTimer = fireRate;
        wallRateTimer = wallRate;

        CreateFMS();
    }

    void Update()
    {
        fireRateTimer += Time.deltaTime;

        if (!wallAlive)
        {
            wallRateTimer += Time.deltaTime;
        }

        enemyInRangeCheck = false;
        enemyInRange = null;
        enemiesHealth = 0;
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

                    if (enemy.GetComponent<CP_EnemigoEnano>())
                    {
                        enemiesHealth += enemy.GetComponent<CP_EnemigoEnano>().health;
                    }
                    if (enemy.GetComponent<CP_EnemigoMediano>())
                    {
                        enemiesHealth += enemy.GetComponent<CP_EnemigoMediano>().health;
                    }
                    if (enemy.GetComponent<CP_Boss1_Invocador>())
                    {
                        enemiesHealth += enemy.GetComponent<CP_Boss1_Invocador>().health;
                    }
                }
            }
        }

        // Detectar muerte
        if (health <= 0)
        {
            print("Muere");
            Destroy(gameObject);
        }

        FSM_Hero1.Update();
    }

    void CreateFMS()
    {
        //Crear maquinas de estado
        FSM_Hero1 = new StateMachineEngine(BehaviourEngine.IsNotASubmachine);

        //Creacion de estados

        //estados FSM_Hero1
        State Idle = FSM_Hero1.CreateEntryState("Idle", accionIdle);
        State Invocar = FSM_Hero1.CreateState("Invocar", accionInvocar);
        State Atacar = FSM_Hero1.CreateState("Atacar", accionAtacar);

        //Percepciones FSM_Hero1
        Perception enemigos_debiles = FSM_Hero1.CreatePerception<ValuePerception>(() => enemyInRangeCheck, () => enemiesHealth < enemiesHealthToinvoke);
        Perception no_hay_enemigos = FSM_Hero1.CreatePerception<ValuePerception>(() => !enemyInRangeCheck);
        Perception debo_invocar = FSM_Hero1.CreatePerception<ValuePerception>(() => enemyInRangeCheck, () => enemiesHealth > enemiesHealthToinvoke, () => !wallAlive, () => wallRateTimer >= wallRate);
        Perception ya_invoque_y_hay_enemigos = FSM_Hero1.CreatePerception<ValuePerception>(() => enemyInRangeCheck, () => wallAlive);

        //Creacion de transiciones
        FSM_Hero1.CreateTransition("aInvocar", Idle, debo_invocar, Invocar);
        FSM_Hero1.CreateTransition("aInvocar1", Atacar, debo_invocar, Invocar);

        FSM_Hero1.CreateTransition("aAtacar", Idle, enemigos_debiles, Atacar);
        FSM_Hero1.CreateTransition("aAtacar1", Invocar, ya_invoque_y_hay_enemigos, Atacar);

        FSM_Hero1.CreateTransition("aIdle", Invocar, no_hay_enemigos, Idle);
        FSM_Hero1.CreateTransition("aIdle1", Atacar, no_hay_enemigos, Idle);

        FSM_Hero1.CreateTransition("volver a atacar", Atacar, enemigos_debiles, Atacar);
        FSM_Hero1.CreateTransition("volver a atacar muro", Atacar, ya_invoque_y_hay_enemigos, Atacar);
    }

    void accionIdle()
    {
        print("Estoy en idle");
    }
    void accionInvocar()
    {
        print("invocar");

        RaycastHit[] groundTilesInRange = Physics.SphereCastAll(transform.position, range, transform.forward, 0, LayerMask.GetMask("Ground"));
        GameObject bestGroundTile = groundTilesInRange[0].collider.gameObject;
        float distanceBestGroundTile = groundTilesInRange[0].collider.transform.position.x;

        foreach (var groundTile in groundTilesInRange)
        {
            if (groundTile.collider.gameObject.transform.position.x < distanceBestGroundTile)
            {
                bestGroundTile = groundTile.collider.gameObject;
                distanceBestGroundTile = groundTile.collider.gameObject.transform.position.x;
            }
        }

        GameObject instWall = Instantiate(wall, bestGroundTile.transform.position, transform.rotation);
        instWall.GetComponent<Wall>().myHero = this;
        wallRateTimer = 0;
        wallAlive = true;
    }
    void accionAtacar()
    {
        print("Recargando");

        if (fireRateTimer >= fireRate)
        {
            print("Ataco");
            Shoot();
        }
    }
    void Shoot()
    {
        fireRateTimer = 0;

        GameObject instBullet = Instantiate(bullet, transform.position, transform.rotation);
        instBullet.GetComponent<CP_Bullet_Tower>().Seek(enemyInRange.transform);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ImpactDamage")
        {
            health -= 250;
        }
    }
}
