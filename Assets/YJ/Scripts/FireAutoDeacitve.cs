using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace bloodborne
{
    public class FireAutoDeacitve : MonoBehaviour
    {
        public void Play(float time)
        {
            StartCoroutine(IEPlay(time));
        }

        IEnumerator IEPlay(float time)
        {
            yield return new WaitForSeconds(time);
            gameObject.SetActive(false);
        }
    }
}
