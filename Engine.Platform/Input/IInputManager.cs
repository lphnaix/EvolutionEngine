using Engine.Core.Timing;

namespace Engine.Platform.Input;

public interface IInputManager
{
    InputState GetState(GameTime time);
}
