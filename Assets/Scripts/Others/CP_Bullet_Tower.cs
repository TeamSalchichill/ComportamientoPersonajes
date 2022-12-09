using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Bullet_Tower : MonoBehaviour
{
    private Transform target;

    public int speed = 70;
    public int damage = 100;

    public CP_Torres myTower;

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.LookAt(target.position);

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            if (target.gameObject.GetComponent<CP_EnemigoMediano>())
            {
                if (target.gameObject.GetComponent<CP_EnemigoMediano>().health <= damage)
                {
                    if (myTower)
                    {
                        myTower.kills++;
                    }
                }

                target.gameObject.GetComponent<CP_EnemigoMediano>().health -= damage;
                
            }
            if (target.gameObject.GetComponent<CP_EnemigoEnano>())
            {
                if (target.gameObject.GetComponent<CP_EnemigoEnano>().health <= damage)
                {
                    if (myTower)
                    {
                        myTower.kills++;
                    }
                }

                target.gameObject.GetComponent<CP_EnemigoEnano>().health -= damage;
            }
            if (target.gameObject.GetComponent<CP_Boss1_Invocador>())
            {
                if (target.gameObject.GetComponent<CP_Boss1_Invocador>().health <= damage)
                {
                    if (myTower)
                    {
                        myTower.kills++;
                    }
                }

                target.gameObject.GetComponent<CP_Boss1_Invocador>().health -= damage;
            }
            if (target.gameObject.GetComponent<CP_Boss2_Atacante>())
            {
                if (target.gameObject.GetComponent<CP_Boss2_Atacante>().health <= damage)
                {
                    if (myTower)
                    {
                        myTower.kills++;
                    }
                }

                target.gameObject.GetComponent<CP_Boss2_Atacante>().health -= damage;
            }

            Destroy(gameObject);
            print("Bala de torre: He golpeado");
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }
}
