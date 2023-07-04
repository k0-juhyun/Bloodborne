using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Tail : MonoBehaviour
{
    public float speed = 4.5f;
    public float amplitude = 0.5f;
    public float frequency = 1f;
    public float offsetMultiplier = 1f;

    private Vector3[] startPositions;
    private float[] offsets;

    void Start()
    {
        // 자식 오브젝트의 시작 위치를 저장
        startPositions = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            startPositions[i] = transform.GetChild(i).position;
        }

        // 자식 오브젝트들의 초기 오프셋을 설정
        offsets = new float[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            offsets[i] = Random.Range(0f, 2f * Mathf.PI);
        }
    }

    void Update()
    {
        // 시간에 따른 흔들림을 계산
        float time = Time.time * frequency;

        // 각 자식 오브젝트를 독립적으로 움직이도록 함
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            float offset = offsets[i] + offsetMultiplier * i;
            float angle = Mathf.Sin(time + offset) * amplitude;
            Vector3 newPosition = startPositions[i] + Vector3.right * angle;
            child.position = newPosition;
        }
    }
}
