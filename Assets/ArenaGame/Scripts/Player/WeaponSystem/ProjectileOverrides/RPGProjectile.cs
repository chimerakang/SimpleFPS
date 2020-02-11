using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A version of the BaseProjectile used for the RPG projectile
/// </summary>
public class RPGProjectile : BaseProjectile
{
    //Radius of the explosion damage
    [SerializeField]
    private float radius = 3;

    //The particle to play on impact
    [SerializeField]
    private ParticleSystem impactParticle;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// When the projectile has reached it's target, for calculation check out BaseProjectile class
    /// </summary>
    public override void OnReachedTarget()
    {
        base.OnReachedTarget();

        if (impactParticle != null)
        {            
            impactParticle.Play();
        }

        //hide the mesh
        GetComponent<MeshRenderer>().enabled = false;

        //Destroy the gameobject after 2 seconds to make sure particle has played
        Destroy(gameObject, 2);

        //Do the explosion damage on the server only
        if (NetworkManager.Instance.IsMaster)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider hitCollider in hitColliders)
            {
                HealthSystem hp = hitCollider.GetComponent<HealthSystem>();
                if (hp)
                {
                    //calculate the actual damage based on the distance within the sphere (damage dropoff)
                    float actualDamage = damage - (Vector3.Distance(hp.transform.position, transform.position) * 10);
                    //call the take damage method
                    hp.TakeDamage((int)actualDamage, attackerName,target,hitNormal);
                }
            }
        }
    }

    /// <summary>
    /// Uncomment this if you want to debug/visualize the explosion damage sphere
    /// </summary>
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, radius);
    //}

}
