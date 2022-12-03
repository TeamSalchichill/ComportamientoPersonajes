using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Boss1_Invocador : MonoBehaviour
{
    BehaviourTreeEngine Boss1BT;

    public int health = 100;

    public float timer = 0;

    public GameObject mainTower;

    public GameObject tower;

    public int numEnemiesStored = 0;

    public bool pathTower;
    public bool isMove;

    void Start()
    {
        CreateBT();
    }

    void Update()
    {
        timer += Time.deltaTime;

        Boss1BT.Update();
    }

    void CreateBT()
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////// Creación del árbol de arriba a abajo //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        print("Árbol creado");
        Boss1BT = new BehaviourTreeEngine(false);

        // Base
        SequenceNode DadSequence = Boss1BT.CreateSequenceNode("DadSequence", false);
        LoopUntilFailDecoratorNode UntilFail = Boss1BT.CreateLoopUntilFailNode("UntilFail", DadSequence);

        Boss1BT.SetRootNode(UntilFail);

        // Dead
        SequenceNode DeadSequence = Boss1BT.CreateSequenceNode("DeadSequence", false);
        InverterDecoratorNode DeadInverter = Boss1BT.CreateInverterNode("DeadInverter", DeadSequence);
        LeafNode DeadLeafCheck = Boss1BT.CreateLeafNode("DeadLeafCheck", NullAction, CheckHealth);
        LeafNode DeadLeaf = Boss1BT.CreateLeafNode("DeadLeaf", Dead, AlwaysSucced);

        DadSequence.AddChild(DeadInverter);

        DeadSequence.AddChild(DeadLeafCheck);
        DeadSequence.AddChild(DeadLeaf);

        // Timer
        SequenceNode TimerSequence = Boss1BT.CreateSequenceNode("TimerSequence", false);
        SucceederDecoratorNode TimerSucceeder = Boss1BT.CreateSucceederNode("TimerSucceeder", TimerSequence);
        LeafNode TimerLeafCheck = Boss1BT.CreateLeafNode("TimerLeafCheck", NullAction, CheckTimer);
        SelectorNode TimerSelector = Boss1BT.CreateSelectorNode("TimerSelector");

        DadSequence.AddChild(TimerSucceeder);

        TimerSequence.AddChild(TimerLeafCheck);
        TimerSequence.AddChild(TimerSelector);

        // Main Tower
        SequenceNode MainTowerSequence = Boss1BT.CreateSequenceNode("MainTowerSequence", false);
        LeafNode MainTowerDistanceLeafCheck = Boss1BT.CreateLeafNode("MainTowerDistanceLeafCheck", NullAction, CheckMainTowerDistance);
        LeafNode MainTowerRunLeaf = Boss1BT.CreateLeafNode("MainTowerRunLeaf", RunMainTower, AlwaysSucced);

        TimerSelector.AddChild(MainTowerSequence);

        MainTowerSequence.AddChild(MainTowerDistanceLeafCheck);
        MainTowerSequence.AddChild(MainTowerRunLeaf);

        // Run Invoke Tower
        SequenceNode TowerSequence = Boss1BT.CreateSequenceNode("TowerSequence", false);
        LeafNode TowerDistanceLeafCheck = Boss1BT.CreateLeafNode("TowerDistanceLeafCheck", NullAction, CheckTowerDistance);
        LeafNode TowerRunInvokeLeaf = Boss1BT.CreateLeafNode("TowerRunInvokeLeaf", RunInvokeTower, AlwaysSucced);

        TimerSelector.AddChild(TowerSequence);

        TowerSequence.AddChild(TowerDistanceLeafCheck);
        TowerSequence.AddChild(TowerRunInvokeLeaf);

        // Store
        SequenceNode StoreSequence = Boss1BT.CreateSequenceNode("StoreSequence", false);
        LeafNode StoreMaxLeafCheck = Boss1BT.CreateLeafNode("StoreMaxLeafCheck", NullAction, CheckStorageDistance);
        LeafNode StoreRunInvokeLeaf = Boss1BT.CreateLeafNode("StoreRunInvokeLeaf", RunInvokeStore, AlwaysSucced);

        TimerSelector.AddChild(StoreSequence);

        StoreSequence.AddChild(StoreMaxLeafCheck);
        StoreSequence.AddChild(StoreRunInvokeLeaf);

        // Store final
        LeafNode StoreFinalRunInvokeLeaf = Boss1BT.CreateLeafNode("StoreFinalRunInvokeLeaf", StoreFinal, AlwaysSucced);

        TimerSelector.AddChild(StoreFinalRunInvokeLeaf);

        // Movement
        SelectorNode MovementSelector = Boss1BT.CreateSelectorNode("MovementSelector");
        SucceederDecoratorNode MovementSucceeder = Boss1BT.CreateSucceederNode("MovementSucceeder", MovementSelector);

        DadSequence.AddChild(MovementSucceeder);

        // Stop
        SequenceNode StopSequence = Boss1BT.CreateSequenceNode("StopSequence", false);
        LeafNode StopTowerLeafCheck = Boss1BT.CreateLeafNode("StopTowerLeafCheck", NullAction, CheckStopTower);
        LeafNode StopMoveLeafCheck = Boss1BT.CreateLeafNode("StopMoveLeafCheck", NullAction, CheckStopMove);
        LeafNode StopInvokeLeaf = Boss1BT.CreateLeafNode("StopInvokeLeaf", Stop, AlwaysSucced);

        MovementSelector.AddChild(StopSequence);

        StopSequence.AddChild(StopTowerLeafCheck);
        StopSequence.AddChild(StopMoveLeafCheck);
        StopSequence.AddChild(StopInvokeLeaf);

        // Attack
        SequenceNode AttackSequence = Boss1BT.CreateSequenceNode("AttackSequence", false);
        LeafNode AttackTowerLeafCheck = Boss1BT.CreateLeafNode("AttackTowerLeafCheck", NullAction, CheckAttackTower);
        LeafNode AttackInvokeLeaf = Boss1BT.CreateLeafNode("AttackInvokeLeaf", Attack, AlwaysSucced);

        MovementSelector.AddChild(AttackSequence);

        AttackSequence.AddChild(AttackTowerLeafCheck);
        AttackSequence.AddChild(AttackInvokeLeaf);

        // Move
        SequenceNode MoveSequence = Boss1BT.CreateSequenceNode("MoveSequence", false);
        LeafNode MoveTowerLeafCheck = Boss1BT.CreateLeafNode("MoveTowerLeafCheck", NullAction, CheckMoveTower);
        LeafNode MoveInvokeLeaf = Boss1BT.CreateLeafNode("MoveInvokeLeaf", Move, AlwaysSucced);
        InverterDecoratorNode MoveInverter = Boss1BT.CreateInverterNode("MoveInverter", MoveTowerLeafCheck);

        MovementSelector.AddChild(MoveSequence);

        MoveSequence.AddChild(MoveInverter);
        MoveSequence.AddChild(MoveInvokeLeaf);

        /*
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////// Creación del árbol de abajo a arriba 1 /////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        print("Árbol creado");
        Boss1BT = new BehaviourTreeEngine(false);

        // Dead
        LeafNode DeadLeafCheck = Boss1BT.CreateLeafNode("DeadLeafCheck", NullAction, CheckHealth);
        LeafNode DeadLeaf = Boss1BT.CreateLeafNode("DeadLeaf", Dead, AlwaysSucced);
        SequenceNode DeadSequence = Boss1BT.CreateSequenceNode("DeadSequence", false);

        DeadSequence.AddChild(DeadLeafCheck);
        DeadSequence.AddChild(DeadLeaf);

        InverterDecoratorNode DeadInverter = Boss1BT.CreateInverterNode("DeadInverter", DeadSequence);

        // Main Tower
        LeafNode MainTowerDistanceLeafCheck = Boss1BT.CreateLeafNode("MainTowerDistanceLeafCheck", NullAction, CheckMainTowerDistance);
        LeafNode MainTowerRunLeaf = Boss1BT.CreateLeafNode("MainTowerRunLeaf", RunMainTower, AlwaysSucced);
        SequenceNode MainTowerSequence = Boss1BT.CreateSequenceNode("MainTowerSequence", false);

        MainTowerSequence.AddChild(MainTowerDistanceLeafCheck);
        MainTowerSequence.AddChild(MainTowerRunLeaf);

        // Run Invoke Tower
        LeafNode TowerDistanceLeafCheck = Boss1BT.CreateLeafNode("TowerDistanceLeafCheck", NullAction, CheckTowerDistance);
        LeafNode TowerRunInvokeLeaf = Boss1BT.CreateLeafNode("TowerRunInvokeLeaf", RunInvokeTower, AlwaysSucced);
        SequenceNode TowerSequence = Boss1BT.CreateSequenceNode("TowerSequence", false);

        TowerSequence.AddChild(TowerDistanceLeafCheck);
        TowerSequence.AddChild(TowerRunInvokeLeaf);

        // Store
        LeafNode StoreMaxLeafCheck = Boss1BT.CreateLeafNode("StoreMaxLeafCheck", NullAction, CheckStorageDistance);
        LeafNode StoreRunInvokeLeaf = Boss1BT.CreateLeafNode("StoreRunInvokeLeaf", RunInvokeStore, AlwaysSucced);
        SequenceNode StoreSequence = Boss1BT.CreateSequenceNode("StoreSequence", false);

        StoreSequence.AddChild(StoreMaxLeafCheck);
        StoreSequence.AddChild(StoreRunInvokeLeaf);

        // Store final
        LeafNode StoreFinalRunInvokeLeaf = Boss1BT.CreateLeafNode("StoreFinalRunInvokeLeaf", StoreFinal, AlwaysSucced);

        // Timer
        LeafNode TimerLeafCheck = Boss1BT.CreateLeafNode("TimerLeafCheck", NullAction, CheckTimer);
        SelectorNode TimerSelector = Boss1BT.CreateSelectorNode("TimerSelector");
        SequenceNode TimerSequence = Boss1BT.CreateSequenceNode("TimerSequence", false);

        TimerSelector.AddChild(MainTowerSequence);
        TimerSelector.AddChild(TowerSequence);
        TimerSelector.AddChild(StoreSequence);
        TimerSelector.AddChild(StoreFinalRunInvokeLeaf);

        TimerSequence.AddChild(TimerLeafCheck);
        TimerSequence.AddChild(TimerSelector);

        SucceederDecoratorNode TimerSucceeder = Boss1BT.CreateSucceederNode("TimerSucceeder", TimerSequence);

        // Stop
        LeafNode StopTowerLeafCheck = Boss1BT.CreateLeafNode("StopTowerLeafCheck", NullAction, CheckStopTower);
        LeafNode StopMoveLeafCheck = Boss1BT.CreateLeafNode("StopMoveLeafCheck", NullAction, CheckStopMove);
        LeafNode StopInvokeLeaf = Boss1BT.CreateLeafNode("StopInvokeLeaf", Stop, AlwaysSucced);
        SequenceNode StopSequence = Boss1BT.CreateSequenceNode("StopSequence", false);

        StopSequence.AddChild(StopTowerLeafCheck);
        StopSequence.AddChild(StopMoveLeafCheck);
        StopSequence.AddChild(StopInvokeLeaf);

        // Attack
        LeafNode AttackTowerLeafCheck = Boss1BT.CreateLeafNode("AttackTowerLeafCheck", NullAction, CheckAttackTower);
        LeafNode AttackInvokeLeaf = Boss1BT.CreateLeafNode("AttackInvokeLeaf", Attack, AlwaysSucced);
        SequenceNode AttackSequence = Boss1BT.CreateSequenceNode("AttackSequence", false);

        AttackSequence.AddChild(AttackTowerLeafCheck);
        AttackSequence.AddChild(AttackInvokeLeaf);

        // Move
        LeafNode MoveTowerLeafCheck = Boss1BT.CreateLeafNode("MoveTowerLeafCheck", NullAction, CheckMoveTower);
        LeafNode MoveInvokeLeaf = Boss1BT.CreateLeafNode("MoveInvokeLeaf", Move, AlwaysSucced);
        SequenceNode MoveSequence = Boss1BT.CreateSequenceNode("MoveSequence", false);
        InverterDecoratorNode MoveInverter = Boss1BT.CreateInverterNode("MoveInverter", MoveTowerLeafCheck);

        MoveSequence.AddChild(MoveInverter);
        MoveSequence.AddChild(MoveInvokeLeaf);

        // Movement
        SelectorNode MovementSelector = Boss1BT.CreateSelectorNode("MovementSelector");

        MovementSelector.AddChild(StopSequence);
        MovementSelector.AddChild(AttackSequence);
        MovementSelector.AddChild(MoveSequence);
        
        SucceederDecoratorNode MovementSucceeder = Boss1BT.CreateSucceederNode("MovementSucceeder", MovementSelector);

        // Base
        SequenceNode DadSequence = Boss1BT.CreateSequenceNode("DadSequence", false);
        DadSequence.AddChild(DeadInverter);
        DadSequence.AddChild(TimerSucceeder);
        DadSequence.AddChild(MovementSucceeder);

        LoopDecoratorNode UntilFail = Boss1BT.CreateLoopNode("UntilFail", DadSequence);
        Boss1BT.SetRootNode(UntilFail);
        */
        /*
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////// Creación del árbol de abajo a arriba 2 /////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        print("Árbol creado");
        Boss1BT = new BehaviourTreeEngine(false);

        // Dead
        LeafNode DeadLeafCheck = Boss1BT.CreateLeafNode("DeadLeafCheck", NullAction, CheckHealth);
        LeafNode DeadLeaf = Boss1BT.CreateLeafNode("DeadLeaf", Dead, AlwaysSucced);
        SequenceNode DeadSequence = Boss1BT.CreateSequenceNode("DeadSequence", false);
        InverterDecoratorNode DeadInverter = Boss1BT.CreateInverterNode("DeadInverter", DeadSequence);

        // Main Tower
        LeafNode MainTowerDistanceLeafCheck = Boss1BT.CreateLeafNode("MainTowerDistanceLeafCheck", NullAction, CheckMainTowerDistance);
        LeafNode MainTowerRunLeaf = Boss1BT.CreateLeafNode("MainTowerRunLeaf", RunMainTower, AlwaysSucced);
        SequenceNode MainTowerSequence = Boss1BT.CreateSequenceNode("MainTowerSequence", false);

        // Run Invoke Tower
        LeafNode TowerDistanceLeafCheck = Boss1BT.CreateLeafNode("TowerDistanceLeafCheck", NullAction, CheckTowerDistance);
        LeafNode TowerRunInvokeLeaf = Boss1BT.CreateLeafNode("TowerRunInvokeLeaf", RunInvokeTower, AlwaysSucced);
        SequenceNode TowerSequence = Boss1BT.CreateSequenceNode("TowerSequence", false);

        // Store
        LeafNode StoreMaxLeafCheck = Boss1BT.CreateLeafNode("StoreMaxLeafCheck", NullAction, CheckStorageDistance);
        LeafNode StoreRunInvokeLeaf = Boss1BT.CreateLeafNode("StoreRunInvokeLeaf", RunInvokeStore, AlwaysSucced);
        SequenceNode StoreSequence = Boss1BT.CreateSequenceNode("StoreSequence", false);

        // Store final
        LeafNode StoreFinalRunInvokeLeaf = Boss1BT.CreateLeafNode("StoreFinalRunInvokeLeaf", StoreFinal, AlwaysSucced);

        // Timer
        LeafNode TimerLeafCheck = Boss1BT.CreateLeafNode("TimerLeafCheck", NullAction, CheckTimer);
        SelectorNode TimerSelector = Boss1BT.CreateSelectorNode("TimerSelector");
        SequenceNode TimerSequence = Boss1BT.CreateSequenceNode("TimerSequence", false);
        SucceederDecoratorNode TimerSucceeder = Boss1BT.CreateSucceederNode("TimerSucceeder", TimerSequence);

        // Stop
        LeafNode StopTowerLeafCheck = Boss1BT.CreateLeafNode("StopTowerLeafCheck", NullAction, CheckStopTower);
        LeafNode StopMoveLeafCheck = Boss1BT.CreateLeafNode("StopMoveLeafCheck", NullAction, CheckStopMove);
        LeafNode StopInvokeLeaf = Boss1BT.CreateLeafNode("StopInvokeLeaf", Stop, AlwaysSucced);
        SequenceNode StopSequence = Boss1BT.CreateSequenceNode("StopSequence", false);

        // Attack
        LeafNode AttackTowerLeafCheck = Boss1BT.CreateLeafNode("AttackTowerLeafCheck", NullAction, CheckAttackTower);
        LeafNode AttackInvokeLeaf = Boss1BT.CreateLeafNode("AttackInvokeLeaf", Attack, AlwaysSucced);
        SequenceNode AttackSequence = Boss1BT.CreateSequenceNode("AttackSequence", false);

        // Move
        LeafNode MoveTowerLeafCheck = Boss1BT.CreateLeafNode("MoveTowerLeafCheck", NullAction, CheckMoveTower);
        LeafNode MoveInvokeLeaf = Boss1BT.CreateLeafNode("MoveInvokeLeaf", Move, AlwaysSucced);
        SequenceNode MoveSequence = Boss1BT.CreateSequenceNode("MoveSequence", false);
        InverterDecoratorNode MoveInverter = Boss1BT.CreateInverterNode("MoveInverter", MoveTowerLeafCheck);

        // Movement
        SelectorNode MovementSelector = Boss1BT.CreateSelectorNode("MovementSelector");
        SucceederDecoratorNode MovementSucceeder = Boss1BT.CreateSucceederNode("MovementSucceeder", MovementSelector);

        // Base
        SequenceNode DadSequence = Boss1BT.CreateSequenceNode("DadSequence", false);
        

        
        MainTowerSequence.AddChild(MainTowerDistanceLeafCheck);
        MainTowerSequence.AddChild(MainTowerRunLeaf);
        TowerSequence.AddChild(TowerDistanceLeafCheck);
        TowerSequence.AddChild(TowerRunInvokeLeaf);
        StoreSequence.AddChild(StoreMaxLeafCheck);
        StoreSequence.AddChild(StoreRunInvokeLeaf);

        StopSequence.AddChild(StopTowerLeafCheck);
        StopSequence.AddChild(StopMoveLeafCheck);
        StopSequence.AddChild(StopInvokeLeaf);
        AttackSequence.AddChild(AttackTowerLeafCheck);
        AttackSequence.AddChild(AttackInvokeLeaf);
        MoveSequence.AddChild(MoveInverter);
        MoveSequence.AddChild(MoveInvokeLeaf);


        TimerSelector.AddChild(MainTowerSequence);
        TimerSelector.AddChild(TowerSequence);
        TimerSelector.AddChild(StoreSequence);
        TimerSelector.AddChild(StoreFinalRunInvokeLeaf);

        MovementSelector.AddChild(StopSequence);
        MovementSelector.AddChild(AttackSequence);
        MovementSelector.AddChild(MoveSequence);
        
        
        DeadSequence.AddChild(DeadLeafCheck);
        DeadSequence.AddChild(DeadLeaf);

        TimerSequence.AddChild(TimerLeafCheck);
        TimerSequence.AddChild(TimerSelector);

        DadSequence.AddChild(DeadInverter);
        DadSequence.AddChild(TimerSucceeder);
        DadSequence.AddChild(MovementSucceeder);


        LoopDecoratorNode UntilFail = Boss1BT.CreateLoopNode("UntilFail", DadSequence);
        Boss1BT.SetRootNode(UntilFail);
        */
    }

    // Base
    void NullAction()
    {

    }
    ReturnValues AlwaysSucced()
    {
        return ReturnValues.Succeed;
    }

    // Dead
    ReturnValues CheckHealth()
    {
        print("Check Health");
        if (health > 0)
        {
            print("Fail Health");
            return ReturnValues.Failed;
        }
        else
        {
            print("Succed Health");
            return ReturnValues.Succeed;
        }
    }
    void Dead()
    {
        print("Dead");
    }

    // Timer
    ReturnValues CheckTimer()
    {
        print("Check Timer");
        if (timer < 3)
        {
            print("Fail Timer");
            return ReturnValues.Failed;
        }
        else
        {
            print("Succed Timer");
            return ReturnValues.Succeed;
        }
    }

    // Main Tower
    ReturnValues CheckMainTowerDistance()
    {
        print("Check Main Tower Distance");
        if (Vector3.Distance(transform.position, mainTower.transform.position) > 5)
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
    }

    // Tower
    ReturnValues CheckTowerDistance()
    {
        print("Check Tower Distance");
        if (Vector3.Distance(transform.position, tower.transform.position) > 5)
        {
            print("Fail Tower Distance");
            return ReturnValues.Failed;
        }
        else
        {
            print("Succed Tower Distance");
            return ReturnValues.Succeed;
        }
    }
    void RunInvokeTower()
    {
        print("Run Invoke Tower");
    }

    // Store
    ReturnValues CheckStorageDistance()
    {
        print("Check Store");
        if (numEnemiesStored < 5)
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
    void RunInvokeStore()
    {
        print("Run Invoke Store");
    }

    // Final Store
    void StoreFinal()
    {
        print("Enemies Storaged");
    }

    // Movement
    ReturnValues CheckStopTower()
    {
        print("Check Stop Tower");
        if (!pathTower)
        {
            print("Fail Stop Tower");
            return ReturnValues.Failed;
        }
        else
        {
            print("Succed Stop Tower");
            return ReturnValues.Succeed;
        }
    }
    ReturnValues CheckStopMove()
    {
        print("Check Stop Move");
        if (!isMove)
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
    }

    // Attack
    ReturnValues CheckAttackTower()
    {
        print("Check Attack Tower");
        if (Vector3.Distance(transform.position, tower.transform.position) > 5)
        {
            print("Fail Attack Tower");
            return ReturnValues.Failed;
        }
        else
        {
            print("Succed Attack Tower");
            return ReturnValues.Succeed;
        }
    }
    void Attack()
    {
        print("Attack");
    }

    // Move
    ReturnValues CheckMoveTower()
    {
        print("Check Move Tower");
        if (Vector3.Distance(transform.position, tower.transform.position) > 5)
        {
            print("Fail Move Tower");
            return ReturnValues.Failed;
        }
        else
        {
            print("Succed Move Tower");
            return ReturnValues.Succeed;
        }
    }
    void Move()
    {
        print("Move");
    }
}
