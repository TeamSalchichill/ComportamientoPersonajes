using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Bullet_BossFinal : MonoBehaviour
{
    private Transform target;

    public int speed = 70;
    public int damage = 100;

    void Start()
    {
        RaycastHit[] towersInRange = Physics.SphereCastAll(transform.position, 1000, transform.up, 0, LayerMask.GetMask("Tower"));
        target = towersInRange[Random.Range(0, towersInRange.Length)].transform;
    }

    void Update()
    {
        if (target == null)
        {
            RaycastHit[] towersInRange = Physics.SphereCastAll(transform.position, 1000, transform.up, 0, LayerMask.GetMask("Tower"));

            if (towersInRange.Length == 0)
            {
                return;
            }

            target = towersInRange[Random.Range(0, towersInRange.Length)].transform;
            return;
        }

        transform.LookAt(target.position);

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            if (target.gameObject.GetComponent<CP_Torres>())
            {
                target.gameObject.GetComponent<CP_Torres>().health -= damage;
                target.gameObject.GetComponent<CP_Torres>().fireRate *= 2;
            }
            if (target.gameObject.GetComponent<CP_Hero1_Invocador>())
            {
                target.gameObject.GetComponent<CP_Hero1_Invocador>().health -= damage;
                target.gameObject.GetComponent<CP_Hero1_Invocador>().fireRate *= 2;
            }
            if (target.gameObject.GetComponent<CP_Hero2_Healer>())
            {
                target.gameObject.GetComponent<CP_Hero2_Healer>().health -= damage;
                target.gameObject.GetComponent<CP_Hero2_Healer>().fireRate *= 2;
            }

            Destroy(gameObject);
            print("Bala de boss final: He golpeado");
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }
}
