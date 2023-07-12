using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main != null)
        {
            Vector3 cameraDir = Camera.main.transform.forward;
            cameraDir.y = 0;
            cameraDir.Normalize();
            transform.rotation = Quaternion.LookRotation(cameraDir);
        }
    }
}
