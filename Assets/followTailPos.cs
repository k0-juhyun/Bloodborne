using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followTailPos : MonoBehaviour
{
    public Transform tail;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = tail.position;
    }
}
