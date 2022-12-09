using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public CP_Hero1_Invocador myHero;

    public int health;

    void Update()
    {
        if (health <= 0)
        {
            print("Muro: Me muero");
            myHero.wallAlive = false;
            Destroy(gameObject);
        }
    }
}
