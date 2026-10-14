using UnityEngine;

public sealed class ScrollingBootstrap : MonoBehaviour
{
    [SerializeField] private CameraScrollMotor scrollMotor;
    [SerializeField] private Transform scrollRig;
    [SerializeField] private Transform initialCameraEntry;

    private void Start()
    {
        scrollRig.position = new Vector3(initialCameraEntry.position.x, initialCameraEntry.position.y, scrollRig.position.z);
        scrollMotor.SetDirection(ScrollDirection.Horizontal);
        scrollMotor.StartScrolling();
    }
}
