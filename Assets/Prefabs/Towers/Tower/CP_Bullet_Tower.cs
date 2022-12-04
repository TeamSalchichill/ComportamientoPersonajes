using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Bullet_Tower : MonoBehaviour
{
    private Transform target;

    public int speed = 70;
    public int damage = 100;

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
                target.gameObject.GetComponent<CP_EnemigoMediano>().health -= damage;
                Destroy(gameObject);
            }
            print("Ha golpeado");
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }
}
