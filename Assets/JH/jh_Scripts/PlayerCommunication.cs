using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommunication : MonoBehaviour
{
    public Transform GermanChair;
    public GameObject canvas;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float dis = Vector3.Distance(transform.position, GermanChair.position);

        if(dis < 3) 
        {
            canvas.SetActive(true);
        }
        else
        {
            canvas.SetActive(false);
        }
    }
}
