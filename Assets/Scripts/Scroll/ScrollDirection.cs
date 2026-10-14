

using UnityEngine;

public enum ScrollDirection
{
    Horizontal,
    VerticalUp,
    VerticalDown
}


public static class ScrollDirectionExtensions
{
    public static Vector2 ToVector(this ScrollDirection direction)
    {
        return direction switch
        {
            ScrollDirection.Horizontal => Vector2.right,
            ScrollDirection.VerticalUp => Vector2.up,
            ScrollDirection.VerticalDown => Vector2.down,
            _ => Vector2.zero
        };
    }
}