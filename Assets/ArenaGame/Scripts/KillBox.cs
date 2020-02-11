using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple class that kills the player if he falls out of boundss
/// </summary>
public class KillBox : MonoBehaviour
{
    void OnTriggerExit(Collider col)
    {
        HealthSystem unluckyFella = col.GetComponent<HealthSystem>();
        if (unluckyFella)
        {
            unluckyFella.TakeDamage(1000, "Out of map", Vector3.zero, Vector3.zero);
        }
    }
}
