using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(bossFov))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        // bossFOV 클래스 참조
        bossFov fov = (bossFov)target;

        // 원주 위의 시작점의 좌표를 계산 (주어진 각도의 1/2)
        Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f);

        // 원의 색상 지정
        Handles.color = new Color(1, 1, 1, 0.2f);

        Handles.DrawWireDisc(fov.transform.position
                             , Vector3.up
                             , fov.viewRange);

        Handles.DrawSolidArc(fov.transform.position
                             , Vector3.up
                             , fromAnglePos
                             , fov.viewAngle
                             , fov.viewRange);
    }
}
