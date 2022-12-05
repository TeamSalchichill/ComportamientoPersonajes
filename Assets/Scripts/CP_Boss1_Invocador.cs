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
        LeafNode MaxStoreRunInvokeLeaf = Boss1BT.CreateLeafNode("MaxStoreRunInvokeLeaf", RunInvokeStore, AlwaysSucced);

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

    // Normal Tower
    ReturnValues CheckNormalTowerDistance()
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
        if (Vector3.Distance(transform.position, tower.transform.position) > 5 || numEnemiesStored <= 0)
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
        if (Vector3.Distance(transform.position, tower.transform.position) > 2)
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
        if (Vector3.Distance(transform.position, tower.transform.position) > 2)
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
