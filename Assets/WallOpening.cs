using UnityEngine;
using VIVE.OpenXR.CompositionLayer.Samples.Passthrough;
using VIVE.OpenXR.Passthrough;
using VIVE.OpenXR.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using VIVE.OpenXR.Toolkits.Common;
using UnityEngine.InputSystem;
namespace VIVE.OpenXR.CompositionLayer.Samples.Passthrough
{
    public class WallOpening : MonoBehaviour
    {
        public InputActionReference testValue;

        OpenXR.Passthrough.XrPassthroughHTC leftPassthroughID = 0;
        OpenXR.Passthrough.XrPassthroughHTC rightPassthroughID = 0;

        [Header("動畫設定")]
        public float splitDistance = 0f; // 裂開的距離（透過 Animation 或其他腳本來遞增這個值）
        public float wallWidth;
        public float wallHeight;

        public Transform mainCameraTransform = null; // 拖入場景中的 Main Camera
        public Vector3 relativePosition = new Vector3(0, 0, 2f);


        private Mesh leftMesh = null;
        private Mesh rightMesh = null;

        private void TestValueAction(InputAction.CallbackContext ctx)
        {
            Vector2 s = ctx.ReadValue<Vector2>();
            splitDistance = s.y;
            Debug.Log(s.y);
        }

        private void Start()
        {
            relativePosition.x -= wallWidth / 2;
            relativePosition.y -= wallHeight / 2;
            testValue.action.performed += TestValueAction;

            if (mainCameraTransform == null)
            {
                mainCameraTransform = Camera.main.transform;
            }

            // 1. 建立左半邊牆的網格 (以物件中心為原點，往左延伸)
            leftMesh = new Mesh();
            leftMesh.vertices = new Vector3[] {
                new Vector3(0, 0, 0), // A 0
                new Vector3(wallWidth / 5 * 2, 0, 0), // B 1
                new Vector3(0,  wallHeight / 3 * 2, 0), // C 2
                new Vector3(wallWidth / 5 * 2,  wallHeight / 4 * 3, 0), // D 3
                new Vector3(0, wallHeight, 0), // E 4
                new Vector3(wallWidth / 3, wallHeight, 0), // F 5
                new Vector3(wallWidth / 3 * 2, wallHeight, 0), // G 6
                new Vector3(wallWidth, wallHeight, 0), // H 7
                new Vector3(wallWidth / 5 * 3, wallHeight / 4 * 3, 0), // I 8
                new Vector3(wallWidth, wallHeight / 3 * 2, 0), // J 9
                new Vector3(wallWidth / 5 * 3, 0, 0), // K 10
                new Vector3(wallWidth, 0, 0) // L 11
            };

            leftMesh.triangles = new int[] {
                0, 1, 2, // A, B, C
                1, 2, 3, // B, C, D
                2, 3, 4, // C, D, E,
                3, 4, 5, // D, E, F
                3, 5, 6, // D, F, G
                3, 8, 6, // D, I, J
                6, 7, 8, // G, H, I
                7, 8, 9, // H, I, J
                8, 9, 10, // I, J, K
                9, 10, 11 // J, K, L
            };
         
            PassthroughAPI.CreateProjectedPassthrough(out leftPassthroughID, LayerType.Overlay, OnDestroyPassthroughFeatureSession);
            bool leftResult = PassthroughAPI.SetProjectedPassthroughMesh(leftPassthroughID, leftMesh.vertices, leftMesh.triangles, true);

            // 2. 建立右半邊牆的網格 (以物件中心為原點，往右延伸)
            rightMesh = new Mesh();
            rightMesh.vertices = new Vector3[] {
                new Vector3(wallWidth / 5 * 2, 0, 0), // B 0
                new Vector3(wallWidth / 5 * 2,  wallHeight / 4 * 3, 0), // D 1
                new Vector3(wallWidth / 5 * 3, wallHeight / 4 * 3, 0), // I 2
                new Vector3(wallWidth / 5 * 3, 0, 0), // K 3
            };
            rightMesh.triangles = new int[] {
                0, 1, 2, // B, D, I
                0, 2, 3 // B, I, K
            };

            PassthroughAPI.CreateProjectedPassthrough(out rightPassthroughID, LayerType.Overlay, OnDestroyPassthroughFeatureSession);
            bool rightResult =  PassthroughAPI.SetProjectedPassthroughMesh(rightPassthroughID, rightMesh.vertices, rightMesh.triangles, true);
            Debug.Log(
            $"[WallOpening] Passthrough IDs: " +
            $"left={leftPassthroughID}, right={rightPassthroughID}"
        );
        }

        private void Update()
        {
            Vector3 basePosition = mainCameraTransform.TransformPoint(relativePosition);

            basePosition.y -= 1.5f;

            Quaternion baseRotation = mainCameraTransform.rotation;

            // Mesh 最左側的樞紐點：B–D 邊
            Vector3 pivotLocal = new Vector3(
                wallWidth / 5f * 2f,
                0f,
                0f
            );

            // 旋轉軸方向：Mesh 局部 Y 軸
            Vector3 axisWorld = baseRotation * Vector3.up;

            // 旋轉角度
            float angle = 90f * splitDistance;

            Quaternion deltaRotation =
                Quaternion.AngleAxis(angle, axisWorld);

            // 樞紐點在旋轉前的世界座標
            Vector3 pivotWorld =
                basePosition + baseRotation * pivotLocal;

            // 旋轉後的整體方向
            Quaternion rightRotation =
                deltaRotation * baseRotation;

            // 讓樞紐點在旋轉前後保持不動
            Vector3 rightPosition =
                pivotWorld - rightRotation * pivotLocal;

            // 每影格更新兩面牆的位置
            PassthroughAPI.SetProjectedPassthroughMeshTransform(leftPassthroughID, ProjectedPassthroughSpaceType.Worldlock, basePosition, baseRotation, Vector3.one);
            PassthroughAPI.SetProjectedPassthroughMeshTransform(rightPassthroughID, ProjectedPassthroughSpaceType.Worldlock, rightPosition, rightRotation, Vector3.one);
            
        }


        void OnDestroyPassthroughFeatureSession(OpenXR.Passthrough.XrPassthroughHTC passthroughID)
        {
            PassthroughAPI.DestroyPassthrough(passthroughID);
        }

        private void OnDestroy()
        {
            testValue.action.performed -= TestValueAction;
        }
    }
}