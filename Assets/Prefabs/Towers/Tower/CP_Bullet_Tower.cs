using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_Bullet_Tower : MonoBehaviour
{
    private Transform target;
    public float speed = 70f;
    public int healthDamage = 100;

    public void Seek(Transform _target)
    {
        target = _target;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
            // HitTarget();
            print("Ha golpeado");
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }
}
