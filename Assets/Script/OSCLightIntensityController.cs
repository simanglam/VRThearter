using extOSC;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class OSCLightIntensityController : MonoBehaviour
{
public OSCReceiver receiver;
    public int multiplier;

public string path;

private OSCBind bindingAction;

    private Light pointLight;
void Start()
{
        pointLight = GetComponent<Light>();
    bindingAction = receiver.Bind(path, messageUpdate);
}

void messageUpdate(OSCMessage message)
{
    float messageValue = 0f;
    if (message.ToFloat(out messageValue))
    {
            messageValue = (messageValue - 0.15f) / 0.6f;
            Debug.Log(messageValue);
            pointLight.intensity = messageValue * multiplier;
            if (messageValue >= 0.4)
                Debug.Log("!!Sync!!");
    }
}

// Update is called once per frame
void Update()
{

}

private void OnDisable()
{
    receiver.Unbind(bindingAction); receiver.Unbind(bindingAction);
}

private void OnEnable()
{
    bindingAction = receiver.Bind(path, messageUpdate);
}

private void OnDestroy()
{
    receiver.Unbind(bindingAction);
}
}
