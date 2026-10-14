using UnityEngine;

public sealed class CameraScrollMotor : MonoBehaviour
{
    [Header("Scrolling")]
    [SerializeField] private ScrollDirection direction = ScrollDirection.Horizontal;

    [SerializeField][Min(0f)] private float speed = 3f;

    public ScrollDirection Direction
    {
        get => direction;
        set => direction = value;
    }

    public float Speed
    {
        get => speed;
        set => speed = Mathf.Max(0f, value);
    }

    public bool IsScrolling
    {
        get;
        private set;
    }

    private void Update()
    {
        if (!IsScrolling)
            return;
        Vector2 directionVector = direction.ToVector();
        float distance = speed * Time.deltaTime;
        Vector2 displacement = directionVector * distance;
        transform.position += new Vector3(displacement.x, displacement.y, 0f);
    }

    public void StartScrolling()
    {
        IsScrolling = true;
    }

    public void StopScrolling()
    {
        IsScrolling = false;
    }

    public void SetDirection(ScrollDirection newDirection)
    {
        direction = newDirection;
    }
}
