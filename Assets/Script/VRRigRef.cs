using UnityEngine;

public class VRRigRef : MonoBehaviour
{
    public static VRRigRef ins;
    public Transform head, leftHand, rightHand;

    private void Awake()
    {
        ins = this;
    }

}
