using UnityEngine;
using DG.Tweening;

public class DOTweenBootstrapper : MonoBehaviour
{
    private void Awake()
    {
        if (FindObjectsOfType<DOTweenBootstrapper>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        DOTween.Init(true, true, LogBehaviour.Verbose);
        Debug.Log("DOTween safely initialized.");
    }
}
