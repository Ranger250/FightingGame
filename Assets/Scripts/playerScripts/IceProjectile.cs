using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceProjectile : ProjectileScript
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().TakeIceDamage(damage, owner);
        }
        Destroy(gameObject);
    }

}
