using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������Ʈ Ǯ�� ������ ��ü���� �̸� ����� ������� ������ �ְ� �ʹ�
// ��Ȱ��ȭ�� ��ü�� ��ȯ�ϰ� �ʹ�
// ������, �� ���庰 ��ü ���, ���庰 ������
// ������ 3 �Ҳ� ����Ʈ
namespace bloodborne
{
    public class ObjectPool : MonoBehaviour
    {

        private GameObject phase3Effect;    // ������3 �Ҳ� ����Ʈ ������
        //public Transform GehrmanPos;        // �Ը��� ��ġ


        public int poolCount = 10;
        public List<GameObject> pool;

        // Start is called before the first frame update
        void Start()
        {
            phase3Effect = Resources.Load<GameObject>("Phase3 Fire Particle System");    // ������3 ����Ʈ �ҷ�����
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
