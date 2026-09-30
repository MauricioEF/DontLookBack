using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class SceneFlowService : MonoBehaviour
{
    private AsyncOperation currentOperation;
    public bool IsLoading
    {
        get;
        private set;
    }

    public float CurrentLoadProgress
    {

        get; private set;
    }

    public void LoadScene(string sceneName, bool allowSceneActivation)
    {
        if (IsLoading)
        {
            Debug.LogWarning("[SceneFlowService] Already loading scene");
            return;
        }
        StartCoroutine(LoadSceneRoutine(sceneName, allowSceneActivation));
    }

    private IEnumerator LoadSceneRoutine(string sceneName, bool allowSceneActivation)
    {
        IsLoading = true;
        currentOperation = SceneManager.LoadSceneAsync(sceneName);
        currentOperation.allowSceneActivation = allowSceneActivation;
        if (currentOperation == null)
        {
            Debug.LogError($"[SceneFlowService] Failed to load scene: {sceneName}");
            IsLoading = false;
            yield break;
        }
        while (!currentOperation.isDone)
        {
            CurrentLoadProgress = currentOperation.progress;
            yield return null;
        }
        IsLoading = false;
    }
    public void ActiveLoad()
    {
        currentOperation.allowSceneActivation = true;
    }
}
