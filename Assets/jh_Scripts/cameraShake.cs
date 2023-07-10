using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraShake : MonoBehaviour
{
    private static cameraShake instance;
    public static cameraShake Instance => instance;

    public bool specialShake;
    public bool normalShake;

    private float shakeTime;

    private float shakeIntensity;

    public cameraShake()
    {
        instance = this;
    }

    private void Update()
    {
        if(specialShake)
        {
            ShakeCameraOnSpecial(0.1f,1f);
        }
        if(normalShake) 
        {
            ShakeCameraOnAttack(0.05f, 1f);
        }
    }
    public void ShakeCameraOnSpecial(float shakeTime = 1.0f,float shakeIntensity = 0.1f)
    {
        specialShake = false;

        this.shakeTime = shakeTime;

        this.shakeIntensity = shakeIntensity;

        StopCoroutine("ShakeByPosition");

        StartCoroutine("ShakeByPosition");
    }

    public void ShakeCameraOnAttack(float shakeTime = 1.0f, float shakeIntensity = 0.1f)
    {
        normalShake = false;

        this.shakeTime = shakeTime;

        this.shakeIntensity = shakeIntensity;

        StopCoroutine("ShakeByPosition");

        StartCoroutine("ShakeByPosition");
    }

    private IEnumerator ShakeByPosition()
    {
        Vector3 startPosition = transform.position;

        while (shakeTime > 0.0f) 
        {
            transform.position = startPosition + Random.insideUnitSphere * shakeIntensity;

            shakeTime -= Time.deltaTime;

            yield return null;
        }

        transform.position = startPosition;
    }

    private IEnumerator ShakeByRotation()
    {
        Vector3 startRot = transform.eulerAngles;

        float power = 10f;

        while (shakeTime > 0.0f)
        {
            float x = 0;

            float y = 0;

            float z = Random.Range(-1f, 1f);

            transform.rotation = Quaternion.Euler(startRot + new Vector3(x, y, z) * shakeIntensity * power);

            shakeTime -= Time.deltaTime;

            yield return null;
        }

        transform.rotation = Quaternion.Euler(startRot);
    }
}
