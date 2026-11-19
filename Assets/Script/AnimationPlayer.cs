using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] public String[] animations;
    [SerializeField] public String nextScene;
    private void Start()
    {
        StartCoroutine(PlayAnimation());
        
    }

    IEnumerator PlayAnimation()
    {
        foreach (var animation in animations)
        {
            animator.Play(animation);
            yield return null;
            yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName(animation) &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
        );
        }

        if (nextScene != "")
            SceneManager.LoadScene(nextScene);
    }

    private void Update()
    {
        
    }
}
