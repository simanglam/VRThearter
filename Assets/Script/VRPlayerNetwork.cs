using UnityEngine;
using Unity.Netcode;

public class VRPlayerNetwork : NetworkBehaviour
{
    public Transform head, leftHand, rightHand;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            head.transform.position = VRRigRef.ins.head.position;
            head.transform.rotation = VRRigRef.ins.head.rotation;

            leftHand.transform.position = VRRigRef.ins.leftHand.position;
            leftHand.transform.rotation = VRRigRef.ins.leftHand.rotation;

            rightHand.transform.position = VRRigRef.ins.rightHand.position;
            rightHand.transform.rotation = VRRigRef.ins.rightHand.rotation;
        }
    }
}
