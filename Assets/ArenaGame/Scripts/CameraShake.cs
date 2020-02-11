using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for the camera shake caused by recoil and on taken damage
/// </summary>
public class CameraShake : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform camTransform;

    // How long the object should shake for.
    private float shakeDuration;

    // Amplitude of the shake. A larger value shakes the camera harder.
    private float shakeAmount;
    private float decreaseFactor = 1.0f;

    void Awake()
    {
        //"Grabs the gameObject's transform"
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    /// <summary>
    /// Called from the weapon controller
    /// </summary>
    /// <param name="shakeAmount"></param>
    /// <param name="shakeDuration"></param>
    public void DoCameraShake(float shakeAmount, float shakeDuration)
    {
       this.shakeAmount = shakeAmount;
       this.shakeDuration = shakeDuration;
    }

    void Update()
    {
        //Shake the camera
        if (shakeDuration > 0)
        {
            camTransform.localEulerAngles = camTransform.localEulerAngles + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
        }

    }
}