using extOSC;
using TMPro;
using UnityEngine;

public class OSCUITextBehavior : MonoBehaviour
{
    public OSCReceiver receiver;

    public string path;

    private OSCBind bindingAction;

    private TMP_Text textUi;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        textUi = GetComponent<TMP_Text>();
        bindingAction = receiver.Bind(path, messageUpdate);
        Debug.Log(textUi);
    }

    void messageUpdate(OSCMessage message)
    {
        float messageValue = 0f;
        if (message.ToFloat(out messageValue)){
            textUi.SetText(path + " " + messageValue.ToString());
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
