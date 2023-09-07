using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class tettt : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene("jh_MoonPresenceOpeningScene");
        }
    }
}
