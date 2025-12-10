namespace Engine.Platform.Input;

public readonly struct InputState
{
    public bool Up { get; }
    public bool Down { get; }
    public bool Left { get; }
    public bool Right { get; }
    public bool Exit { get; }

    public InputState(bool up, bool down, bool left, bool right, bool exit)
    {
        Up = up;
        Down = down;
        Left = left;
        Right = right;
        Exit = exit;
    }
}
