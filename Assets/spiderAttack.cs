using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiderAttack : MonoBehaviour
{
    [Header("플레이어")]
    public Transform Player;
    [Header("레이저빔 쏘는시간")]
    public float razerBeamDuration;
    [Header("레이저빔 거리")]
    public float razerDistance;
    [Header("레이저 지속시간")]
    public float razerDuration;
    [Header("레이저 딜레이")]
    [Range(0f, 1f)]
    public float razerDelay;
    Vector3 beamPos;
    bool isRazer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine(playerOneSecBefore(Player.transform.position, beamPos));
        transform.LookAt(Player.transform.position);
        if (Input.GetMouseButton(0) && !isRazer)
        {
            StartCoroutine(razerbeam(transform.position, razerBeamDuration, razerDelay));

        }
        else if (Input.GetMouseButton(0) && isRazer)
        {
            isRazer = false;
        }
    }

    // playerPos : 1초전 플레이어 위치
    IEnumerator razerbeam(Vector3 startPos, float duration, float delayTime)
    {
        isRazer = true;
        transform.position = startPos;
        duration = razerBeamDuration;
        delayTime = razerDelay;
        delayTime = 0.2f;
        for (float curTime = 0; curTime < duration; curTime += Time.deltaTime)
        {
            yield return new WaitForSeconds(delayTime);
            Debug.DrawRay(transform.position, transform.forward * razerDistance, Color.red, razerDuration);
        }
    }

    //IEnumerator playerOneSecBefore(Vector3 playerPreviusPos, Vector3 razerBeamPos)
    //{
    //    playerPreviusPos = Player.transform.position;
    //    yield return new WaitForSeconds(1);
    //    razerBeamPos = playerPreviusPos;
    //    yield return razerBeamPos;
    //}
}
