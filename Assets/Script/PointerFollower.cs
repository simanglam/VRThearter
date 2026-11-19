using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointerFollower : MonoBehaviour
{
    public InputActionReference rotation;
    public InputActionReference position;
    public Transform targetTransform;
    public Transform selfTransform;

    public float distance = 2.0f;
    public Vector3 forward;
    public Vector3 controllerPosition;
    Quaternion controllerRotation;
    

    private void rotationAction(InputAction.CallbackContext ctx)
    {
        controllerRotation = ctx.ReadValue<Quaternion>();
    }

    private void positionAction(InputAction.CallbackContext ctx)
    {
        controllerPosition = ctx.ReadValue<Vector3>();
    }

    void Start()
    {
        rotation.action.performed += rotationAction;
        position.action.performed += positionAction;
    }


    void Update()
    {
        // Calculate controller forward direction
        Vector3 forward = controllerRotation * this.forward;

        // Move target in front of controller
        targetTransform.position =
            controllerPosition + forward * distance;

        // Follow controller rotation
        targetTransform.rotation = controllerRotation;
    }

    private void OnDestroy()
    {
        rotation.action.performed -= rotationAction;
        position.action.performed -= positionAction;
    }
}
