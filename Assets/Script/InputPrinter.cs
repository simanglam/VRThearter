using UnityEngine;
using UnityEngine.InputSystem;

public class InputPrinter : MonoBehaviour
{
    public InputActionReference[] testValue;

    private void TestValueAction(InputAction.CallbackContext ctx)
    {
        switch (ctx.ReadValueAsObject())
        {
            case float f:
                Debug.Log(f);
                break;
            case Vector2 v2:
                Debug.Log(v2);
                break;
            case Vector3 v3:
                Debug.Log(v3);
                break;
            case Quaternion q:
                Debug.Log(q);
                break;
            default:
                Debug.Log(ctx.ReadValueAsObject().GetType().ToString());
                break;
        }
        
    }

    void Start()
    {
        foreach (var i in testValue)
        {
            i.action.performed += TestValueAction;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        foreach (var i in testValue)
        {
            i.action.performed -= TestValueAction;
        }
    }
}
