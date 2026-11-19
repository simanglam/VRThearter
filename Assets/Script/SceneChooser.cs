using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class SceneChooser : MonoBehaviour
{
    public Button[] sceneButtons;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var i in sceneButtons)
        {
            i.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.SceneManager.LoadScene(i.name, UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
