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
            myHero.wallAlive = false;
            Destroy(gameObject);
        }
    }
}
