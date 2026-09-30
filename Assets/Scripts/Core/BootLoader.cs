using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class BootLoader : MonoBehaviour
{
    [SerializeField] private SceneFlowService sceneFlowService;

    [SerializeField] private bool loadMainMenuOnStart = true;

    private IEnumerator Start()
    {
        yield return null;

        InitializeCoreSystems();

        yield return new WaitForSeconds(1f);

        if (loadMainMenuOnStart)
        {
            yield return LoadMainMenuAsync();
        }
    }
    private void InitializeCoreSystems()
    {
        //Initialize Managers
    }

    private IEnumerator LoadMainMenuAsync()
    {
        sceneFlowService.LoadScene(SceneNames.TestsDau, false);
        while (sceneFlowService.IsLoading)
        {
            if (sceneFlowService.CurrentLoadProgress >= 0.9f)
            {
                yield return new WaitForSeconds(1f);
                sceneFlowService.ActiveLoad();
            }
            yield return null;
        }
    }
}
