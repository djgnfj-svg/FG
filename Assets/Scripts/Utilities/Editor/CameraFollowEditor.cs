using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraFollow))]
public class CameraFollowEditor : Editor
{
    void OnSceneGUI()
    {
        CameraFollow cameraFollow = (CameraFollow)target;

        // useBoundaries가 false면 핸들 안 그리기
        if (!cameraFollow.GetComponent<CameraFollow>().enabled) return;

        // Reflection으로 private 필드 접근
        var minBoundsField = typeof(CameraFollow).GetField("minBounds",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var maxBoundsField = typeof(CameraFollow).GetField("maxBounds",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var useBoundariesField = typeof(CameraFollow).GetField("useBoundaries",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (minBoundsField == null || maxBoundsField == null || useBoundariesField == null) return;

        bool useBoundaries = (bool)useBoundariesField.GetValue(cameraFollow);
        if (!useBoundaries) return;

        Vector2 minBounds = (Vector2)minBoundsField.GetValue(cameraFollow);
        Vector2 maxBounds = (Vector2)maxBoundsField.GetValue(cameraFollow);

        // 핸들 색상
        Handles.color = Color.yellow;

        // 각 핸들을 개별적으로 처리
        EditorGUI.BeginChangeCheck();

        // 현재 경계값들
        float leftX = minBounds.x;
        float rightX = maxBounds.x;
        float bottomY = minBounds.y;
        float topY = maxBounds.y;

        // 4개 모서리 핸들 (각각 독립적으로)
        Vector3 topLeft = Handles.FreeMoveHandle(
            new Vector3(leftX, topY, 0), 0.5f, Vector3.zero, Handles.DotHandleCap);

        Vector3 topRight = Handles.FreeMoveHandle(
            new Vector3(rightX, topY, 0), 0.5f, Vector3.zero, Handles.DotHandleCap);

        Vector3 bottomLeft = Handles.FreeMoveHandle(
            new Vector3(leftX, bottomY, 0), 0.5f, Vector3.zero, Handles.DotHandleCap);

        Vector3 bottomRight = Handles.FreeMoveHandle(
            new Vector3(rightX, bottomY, 0), 0.5f, Vector3.zero, Handles.DotHandleCap);

        // 경계 박스 선 그리기
        Handles.DrawLine(topLeft, topRight);
        Handles.DrawLine(topRight, bottomRight);
        Handles.DrawLine(bottomRight, bottomLeft);
        Handles.DrawLine(bottomLeft, topLeft);

        // 변경사항 적용 - 직접적으로 계산
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(cameraFollow, "Change Camera Bounds");

            // 드래그된 점들을 그대로 사용 (Min/Max 제한 없이)
            float newLeftX = topLeft.x;   // 왼쪽 위 점의 X
            float newRightX = topRight.x; // 오른쪽 위 점의 X
            float newBottomY = bottomLeft.y; // 왼쪽 아래 점의 Y
            float newTopY = topLeft.y;    // 왼쪽 위 점의 Y

            // 만약 순서가 바뀌면 자동으로 교체
            if (newLeftX > newRightX)
            {
                float temp = newLeftX;
                newLeftX = newRightX;
                newRightX = temp;
            }

            if (newBottomY > newTopY)
            {
                float temp = newBottomY;
                newBottomY = newTopY;
                newTopY = temp;
            }

            Vector2 newMinBounds = new Vector2(newLeftX, newBottomY);
            Vector2 newMaxBounds = new Vector2(newRightX, newTopY);

            minBoundsField.SetValue(cameraFollow, newMinBounds);
            maxBoundsField.SetValue(cameraFollow, newMaxBounds);

            // 즉시 리프레시
            EditorUtility.SetDirty(cameraFollow);
        }

        // 라벨 표시
        Vector3 center = (topLeft + bottomRight) * 0.5f;
        Handles.Label(center, "Camera Boundary");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Scene 뷰에서 노란색 점들을 드래그하여 카메라 경계를 조절하세요!", MessageType.Info);
    }
}