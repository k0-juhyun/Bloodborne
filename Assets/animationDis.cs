using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationDis : MonoBehaviour
{
    private Transform playerPos;

    // Start is called before the first frame update
    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerPos = player.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3.Distance(this.transform.position, playerPos.position);


    }
}
