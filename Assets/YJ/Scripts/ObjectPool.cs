using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 오브젝트 풀로 관리할 객체들을 미리 만들어 목록으로 가지고 있고 싶다
// 비활성화된 객체를 반환하고 싶다
// 공장목록, 각 공장별 객체 목록, 공장별 구분자
// 페이즈 3 불꽃 이펙트
namespace bloodborne
{
    public class ObjectPool : MonoBehaviour
    {

        private GameObject phase3Effect;    // 페이즈3 불꽃 이펙트 프리팹
        //public Transform GehrmanPos;        // 게르만 위치


        public int poolCount = 10;
        public List<GameObject> pool;

        // Start is called before the first frame update
        void Start()
        {
            phase3Effect = Resources.Load<GameObject>("Phase3 Fire Particle System");    // 페이즈3 이펙트 불러오기
            pool = new List<GameObject>();
            for (int i = 0; i < poolCount; i++)
            {
                GameObject obj = Instantiate(phase3Effect);
                obj.SetActive(false);
                pool.Add(obj);
            }
        }

        public GameObject GetDeactiveObject()
        {
            for (int i = 0; i < poolCount; i++)
            {
                if (pool[i].activeSelf == false)
                {
                    pool[i].SetActive(true);
                    return pool[i];
                }
            }
            return null;
        }

    }

}
