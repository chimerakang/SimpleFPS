using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base projectile class, all projectiles should derive from this
/// </summary>
public class BaseProjectile : MonoBehaviour
{

    //The speed of the bullet
    public float ProjectileSpeed = 10;

    //The target of which the projectile calls "OnReachedTarget"
    [HideInInspector]
    public Vector3 target;

    //The hitnormal 
    [HideInInspector]
    public Vector3 hitNormal;

    //Damage, hidden in inspector as it's set by the weapon controller
    [HideInInspector]
    public int damage;

    //The lifetime of the projectile
    [HideInInspector]
    public float lifeTime = 10.0f;

    //who shot the projectile?
    [HideInInspector]
    public string attackerName;

    //If the projectile should stop moving
    protected bool stopMove = false;

    // Use this for initialization
    public virtual void Start()
    {
        //Destroy the gameobjects after the defined lifetime
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (!stopMove)
        {
            //if the distance between the projectile's position and the target is bigger than one
            if (Vector3.Distance(transform.position, target) > 1)
            {
                //move the bullet
                transform.position += (transform.forward * ProjectileSpeed * Time.deltaTime);                
            }
            else // the distance is less than one
            {
                //call OnReachedTarget and stop moving
                OnReachedTarget();
                stopMove = true;
            }
        }        
    }
    
    public virtual void OnReachedTarget()
    {
        //Override this in the custom projectile classes
    }   
}
