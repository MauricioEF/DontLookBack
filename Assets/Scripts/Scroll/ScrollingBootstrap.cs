using UnityEngine;

public sealed class ScrollingBootstrap : MonoBehaviour
{
    [SerializeField] private CameraScrollMotor scrollMotor;
    [SerializeField] private Transform scrollRig;
    [SerializeField] private Transform initialCameraEntry;

    [Header("Gameplay generation")]
    [SerializeField] private HorizontalGameplayRegionLifecycle gameplayRegions;

    private void Start()
    {
        scrollRig.position = new Vector3(initialCameraEntry.position.x, initialCameraEntry.position.y, scrollRig.position.z);
        scrollMotor.SetDirection(ScrollDirection.Horizontal);
        //Prepare gameplayCoverage before camera movement begins;

        gameplayRegions.Initialize();
        scrollMotor.StartScrolling();
    }
}
