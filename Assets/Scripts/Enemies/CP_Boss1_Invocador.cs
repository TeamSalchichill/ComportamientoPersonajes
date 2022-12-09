using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CP_Boss1_Invocador : MonoBehaviour
{
    BehaviourTreeEngine Boss1BT;

    GameManager gameManager;

    [Header("Components")]
    public NavMeshAgent nav;
    public Animator anim;

    [Header("External GameObjects")]
    public GameObject enemyToInvoke;
    public CP_BossFinal myBossFinal;

    [Header("Stats")]
    public int health;
    public int damage;
    public int range;
    public int speed;
    public float abilityRate;
    float abilityRateTimer;
    public float hitRate;
    float hitRateTimer;
    [Space]
    public int rangeDetectMainTower;
    public int rangeDetectTower;

    [Header("Checks variables")]
    public bool towerInRangeCheck;
    public GameObject towerInRange;

    public bool towerInRangeRunCheck;
    public GameObject towerInRangeRun;

    public bool wallInRangeCheck;
    public GameObject wallInRange;
    [Space]
    public int numEnemiesStored;
    public int numEnemiesStoredMax;

    void Start()
    {
        gameManager = GameManager.instance;

        anim = GetComponent<Animator>();
        anim.SetBool("isHit", false);

        nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(Vector3.zero);
        nav.speed = speed;

        abilityRateTimer = abilityRate;
        hitRateTimer = hitRate;

        CreateBT();
    }

    void Update()
    {
        abilityRateTimer += Time.deltaTime;
        hitRateTimer += Time.deltaTime;

        towerInRangeCheck = false;
        towerInRange = null;
        towerInRangeRunCheck = false;
        towerInRangeRun = null;
        wallInRangeCheck = false;
        wallInRange = null;
        foreach (GameObject tower in gameManager.towers)
        {
            if (tower)
            {
                if (Vector3.Distance(transform.position, tower.transform.position) < range)
                {
                    if (tower.GetComponent<CP_Torres>())
                    {
                        if (!towerInRangeCheck)
                        {
                            towerInRangeCheck = true;
                            towerInRange = tower;
                        }
                    }
                    if (tower.GetComponent<Wall>())
                    {
                        if (!wallInRangeCheck)
                        {
                            wallInRangeCheck = true;
                            wallInRange = tower;
                        }
                    }
                    if (tower.GetComponent<CP_Hero1_Invocador>())
                    {
                        if (!towerInRangeCheck)
                        {
                            towerInRangeCheck = true;
                            towerInRange = tower;
                        }
                    }
                    if (tower.GetComponent<CP_Hero2_Healer>())
                    {
                        if (!towerInRangeCheck)
                        {
                            towerInRangeCheck = true;
                            towerInRange = tower;
                        }
                    }
                }
                if (Vector3.Distance(transform.position, tower.transform.position) < rangeDetectTower)
                {
                    towerInRangeRunCheck = true;
                    towerInRangeRun = tower;
                }
            }
        }

        if (Vector3.Distance(transform.position, Vector3.zero) < 3)
        {
            print("Llegué");

            if (myBossFinal)
            {
                myBossFinal.health -= 100;
            }

            Destroy(gameObject);
        }

        Boss1BT.Update();
    }

    void CreateBT()
    {
        Boss1BT = new BehaviourTreeEngine(false);

        // MiniTree 1 - Dead
        // Check health <= 0
        LeafNode DeadCheckLeaf = Boss1BT.CreateLeafNode("DeadCheckLeaf", NullAction, CheckHealth);
        // Dead
        LeafNode DeadLeaf = Boss1BT.CreateLeafNode("DeadLeaf", Dead, AlwaysSucced);

        // MiniTree 2 - Main Tower in range
        // Check if Main Tower is in range
        LeafNode MainTowerDistanceCheckLeaf = Boss1BT.CreateLeafNode("MainTowerDistanceCheckLeaf", NullAction, CheckMainTowerDistance);
        // Run to Main Tower
        LeafNode MainTowerRunLeaf = Boss1BT.CreateLeafNode("MainTowerRunLeaf", RunMainTower, AlwaysSucced);

        // MiniTree 3 - Normal Tower in range
        // Check if any Main Tower is in range
        LeafNode NormalTowerDistanceCheckLeaf = Boss1BT.CreateLeafNode("NormalTowerDistanceCheckLeaf", NullAction, CheckNormalTowerDistance);
        // Run to one Normal Tower in range and invoke enemies
        LeafNode NormalTowerRunInvokeLeaf = Boss1BT.CreateLeafNode("NormalTowerRunInvokeLeaf", RunInvokeTower, AlwaysSucced);

        // MiniTree 4 - Max Store
        //Almacenamiento maximo alcanzado
        LeafNode MaxStoreCheckLeaf = Boss1BT.CreateLeafNode("MaxStoreCheckLeaf", NullAction, CheckStorageDistance);
        //Accion Correr e Invocar
        LeafNode MaxStoreRunInvokeLeaf = Boss1BT.CreateLeafNode("MaxStoreRunInvokeLeaf", InvokeStore, AlwaysSucced);

        // MiniTree 5 - SubTimer Childs
        // Add child Main Tower
        SequenceNode MainTowerSequence = Boss1BT.CreateSequenceNode("MainTowerSequence", false);
        MainTowerSequence.AddChild(MainTowerDistanceCheckLeaf);
        MainTowerSequence.AddChild(MainTowerRunLeaf);
        // Add child Normal Tower
        SequenceNode NormalTowerSequence = Boss1BT.CreateSequenceNode("NormalTowerSequence", false);
        NormalTowerSequence.AddChild(NormalTowerDistanceCheckLeaf);
        NormalTowerSequence.AddChild(NormalTowerRunInvokeLeaf);
        // Add child Max Store
        SequenceNode MaxStoreSequence = Boss1BT.CreateSequenceNode("MaxStoreSequence", false);
        MaxStoreSequence.AddChild(MaxStoreCheckLeaf);
        MaxStoreSequence.AddChild(MaxStoreRunInvokeLeaf);
        // Add child Store
        LeafNode StoreSequence = Boss1BT.CreateLeafNode("StoreSequence", StoreFinal, AlwaysFailed);

        // MiniTree 6 - Timer
        // Check if timer <= 0
        LeafNode TimerCheck = Boss1BT.CreateLeafNode("TimerCheck", NullAction, CheckTimer);
        // Add child Timer
        SelectorNode TimerSubSequence = Boss1BT.CreateSelectorNode("TimerSubSequence");
        TimerSubSequence.AddChild(MainTowerSequence);
        TimerSubSequence.AddChild(NormalTowerSequence);
        TimerSubSequence.AddChild(MaxStoreSequence);
        TimerSubSequence.AddChild(StoreSequence);

        // MiniTree 7 - Stop
        // Check if any Normal Tower is in range
        LeafNode StopNormalTowerCheckLeaf = Boss1BT.CreateLeafNode("StopNormalTowerCheckLeaf", NullAction, CheckStopTower);
        // Check if i am in movement
        LeafNode StopMoveCheckLeaf = Boss1BT.CreateLeafNode("StopMoveCheckLeaf", NullAction, CheckStopMove);
        // Stop
        LeafNode StopLeaf = Boss1BT.CreateLeafNode("StopLeaf", Stop, AlwaysSucced);

        // MiniTree 8 - Attack
        // Check if any Normal Tower is in range
        LeafNode AttackNormalTowerCheckLeaf = Boss1BT.CreateLeafNode("AttackNormalTowerCheckLeaf", NullAction, CheckAttackTower);
        // Attack
        LeafNode AttackLeaf = Boss1BT.CreateLeafNode("AttackLeaf", Attack, AlwaysSucced);

        // MiniTree 9 - Walk
        // Check if any Normal Tower is in range
        LeafNode TorreNoEnRango = Boss1BT.CreateLeafNode("TorreRango2", NullAction, CheckMoveTower);
        // Walk
        LeafNode Avanzar = Boss1BT.CreateLeafNode("Avanzar", Move, AlwaysSucced);

        // MiniTree 10 - SubMovement Childs
        // Add Stop childs
        SequenceNode StopSequence = Boss1BT.CreateSequenceNode("StopSequence", false);
        StopSequence.AddChild(StopNormalTowerCheckLeaf);
        StopSequence.AddChild(StopMoveCheckLeaf);
        StopSequence.AddChild(StopLeaf);
        // Add Attack childs
        SequenceNode AttackSequence = Boss1BT.CreateSequenceNode("AttackSequence", false);
        AttackSequence.AddChild(AttackNormalTowerCheckLeaf);
        AttackSequence.AddChild(AttackLeaf);
        // Add Walk childs
        SequenceNode WalkSequence = Boss1BT.CreateSequenceNode("WalkSequence", false);
        WalkSequence.AddChild(TorreNoEnRango);
        WalkSequence.AddChild(Avanzar);

        // MiniTree 11 - Tree Root
        // Add Dead childs
        SequenceNode DeadSequence = Boss1BT.CreateSequenceNode("DeadSequence", false);
        DeadSequence.AddChild(DeadCheckLeaf);
        DeadSequence.AddChild(DeadLeaf);
        // Add Timer childs
        SequenceNode TimerSequence = Boss1BT.CreateSequenceNode("TimerSequence", false);
        TimerSequence.AddChild(TimerCheck);
        TimerSequence.AddChild(TimerSubSequence);
        // Add Movement childs
        SelectorNode MovementSequence = Boss1BT.CreateSelectorNode("MovementSequence");
        MovementSequence.AddChild(StopSequence);
        MovementSequence.AddChild(AttackSequence);
        MovementSequence.AddChild(WalkSequence);

        // Add Root childs
        SelectorNode RootSelector = Boss1BT.CreateSelectorNode("RootSelector");
        RootSelector.AddChild(DeadSequence);
        RootSelector.AddChild(TimerSequence);
        RootSelector.AddChild(MovementSequence);

        //Loop
        TreeNode Loop = Boss1BT.CreateLoopNode("Loop", RootSelector);
        Boss1BT.SetRootNode(Loop);
    }

    // Base
    void NullAction()
    {

    }
    ReturnValues AlwaysSucced()
    {
        return ReturnValues.Succeed;
    }
    ReturnValues AlwaysFailed()
    {
        return ReturnValues.Failed;
    }

    // Dead
    ReturnValues CheckHealth()
    {
        print("Check Health");
        if (health <= 0)
        {
            print("Succeed Health");
            return ReturnValues.Succeed;
        }
        else
        {
            print("Fail Health");
            return ReturnValues.Failed;
        }
    }
    void Dead()
    {
        print("Dead");
        Destroy(gameObject);
    }

    // Timer
    ReturnValues CheckTimer()
    {
        print("Check Timer");
        if (abilityRateTimer < abilityRate)
        {
            print("Fail Timer");
            return ReturnValues.Failed;
        }
        else
        {
            print("Succed Timer");
            abilityRateTimer = 0;
            return ReturnValues.Succeed;
        }
    }

    // Main Tower
    ReturnValues CheckMainTowerDistance()
    {
        print("Check Main Tower Distance");
        if (Vector3.Distance(transform.position, gameManager.mainTower.transform.position) > rangeDetectMainTower)
        {
            print("Fail Main Tower Distance");
            return ReturnValues.Failed;
        }
        else
        {
            print("Succed Main Tower Distance");
            return ReturnValues.Succeed;
        }
    }
    void RunMainTower()
    {
        print("Run Main Tower");
        nav.speed = speed * 4;
        anim.SetBool("isHit", false);
        anim.speed = 4;
    }

    // Normal Tower
    ReturnValues CheckNormalTowerDistance()
    {
        print("Check Tower Distance");
        if (towerInRangeRun)
        {
            print("Succed Tower Distance");
            return ReturnValues.Succeed;
        }
        else
        {
            print("Fail Tower Distance");
            return ReturnValues.Failed;
        }
        
    }
    void RunInvokeTower()
    {
        print("Run Invoke Tower");
        nav.speed = speed * 4;
        anim.SetBool("isHit", false);
        anim.speed = 4;
        Invoke("NormalSpeed", 2);
    }
    void NormalSpeed()
    {
        nav.speed = speed / 4;
        anim.speed = 1;
        anim.SetTrigger("doInvoke");
        StartCoroutine(SpawnEnemies(numEnemiesStored));
        numEnemiesStored = 0;
    }

    // Store Max
    ReturnValues CheckStorageDistance()
    {
        print("Check Store");
        if (numEnemiesStored < numEnemiesStoredMax)
        {
            print("Fail Store");
            return ReturnValues.Failed;
        }
        else
        {
            print("Succed Store");
            return ReturnValues.Succeed;
        }
    }
    void InvokeStore()
    {
        print("Run Invoke Store");
        anim.SetTrigger("doInvoke");
        StartCoroutine(SpawnEnemies(numEnemiesStored));
        numEnemiesStored = 0;
    }
    IEnumerator SpawnEnemies(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Instantiate(enemyToInvoke, new Vector3(transform.position.x, 0, transform.position.z) + (transform.forward * 2), transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Final Store
    void StoreFinal()
    {
        print("Enemies Storaged");
        numEnemiesStored += 2;
        numEnemiesStored = Mathf.Min(numEnemiesStored, numEnemiesStoredMax);
    }

    // Movement
    ReturnValues CheckStopTower()
    {
        print("Check Stop Tower");
        if (wallInRange)
        {
            print("Succed Stop Tower");
            return ReturnValues.Succeed;
        }
        else
        {
            print("Fail Stop Tower");
            return ReturnValues.Failed;
        }
        
    }
    ReturnValues CheckStopMove()
    {
        print("Check Stop Move");
        if (nav.speed == 0)
        {
            print("Fail Stop Move");
            return ReturnValues.Failed;
        }
        else
        {
            print("Succed Stop Move");
            return ReturnValues.Succeed;
        }
    }
    void Stop()
    {
        print("Stop");
        nav.speed = 0;
    }

    // Attack
    ReturnValues CheckAttackTower()
    {
        if (towerInRange && hitRateTimer >= hitRate)
        {
            print("Succed Attack Tower");
            hitRateTimer = 0;
            return ReturnValues.Succeed;
        }
        else
        {
            print("Fail Attack Tower");
            return ReturnValues.Failed;
        }
    }
    void Attack()
    {
        print("Attack");

        nav.SetDestination(new Vector3(towerInRange.transform.position.x, 0, towerInRange.transform.position.z));

        if (towerInRange.GetComponent<CP_Torres>())
        {
            towerInRange.GetComponent<CP_Torres>().health -= damage;
        }
        if (towerInRange.GetComponent<CP_Hero1_Invocador>())
        {
            towerInRange.GetComponent<CP_Hero1_Invocador>().health -= damage;
        }
        if (towerInRange.GetComponent<CP_Hero2_Healer>())
        {
            towerInRange.GetComponent<CP_Hero2_Healer>().health -= damage;
        }
        if (towerInRange.GetComponent<Wall>())
        {
            towerInRange.GetComponent<Wall>().health -= damage;
        }

        anim.SetBool("isHit", true);
    }

    // Move
    ReturnValues CheckMoveTower()
    {
        print("Check Move Tower");
        if (!towerInRange)
        {
            print("Succed Move Tower");
            return ReturnValues.Succeed;
        }
        else
        {
            print("Fail Move Tower");
            return ReturnValues.Failed;
        }
    }
    void Move()
    {
        print("Move");
        if (nav.speed == 0)
        {
            nav.speed = speed;
        }
        nav.SetDestination(Vector3.zero);

        anim.SetBool("isHit", false);
    }

    public void DisableParalizePublic()
    {
        Invoke("DisableParalize", 5);
    }
    void DisableParalize()
    {
        nav.speed = 2;
        hitRate -= 1000;
        abilityRate -= 1000;
        anim.speed = 1;
    }

    private void OnMouseDown()
    {
        gameManager.infoPanel.SetActive(true);
        gameManager.info.text =
            "Nombre: Boss 1 - Invocador \n" +
            "Vida: " + health + "\n" +
            "Rango: " + range + "\n" +
            "Daño: " + damage + "\n" +
            "Velocidad de movimiento: " + speed + "\n" +
            "Velocidad de ataque: " + hitRate + "\n" +
            "Rango detección torre principal: " + rangeDetectMainTower + "\n" +
            "Rango detección torretas: " + rangeDetectTower + "\n" +
            "Número personajes almacenados " + numEnemiesStored + "\n" +
            "Número personajes máximos de almacenamiento " + numEnemiesStoredMax + "\n"
        ;
    }
}
