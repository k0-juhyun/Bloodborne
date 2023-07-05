using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        var _player = GameObject.FindGameObjectWithTag("Player");

        // 인스턴스 체크
        if (_player != null)
            player = player.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player);
        print("p");
    }
}
