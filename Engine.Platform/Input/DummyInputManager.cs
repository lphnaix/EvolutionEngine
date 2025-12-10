using Engine.Core.Timing;

namespace Engine.Platform.Input;

/// <summary>
/// Scripted input provider for headless demo/testing.
/// </summary>
public sealed class DummyInputManager : IInputManager
{
    private readonly Queue<InputState> _script;
    private InputState _last;

    public DummyInputManager(IEnumerable<InputState> scriptedStates)
    {
        _script = new Queue<InputState>(scriptedStates);
        _last = new InputState(false, false, false, false, exit: false);
    }

    public InputState GetState(GameTime time)
    {
        if (_script.Count > 0)
        {
            _last = _script.Dequeue();
        }

        return _last;
    }
}
