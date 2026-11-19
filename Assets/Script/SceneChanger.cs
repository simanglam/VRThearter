using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string nextScene;
    public void OnAnimationFinished()
    {
        SceneManager.LoadScene(nextScene);
    }
}