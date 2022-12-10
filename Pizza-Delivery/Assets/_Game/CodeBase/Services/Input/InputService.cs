using UnityEngine;

namespace _Game.CodeBase.Services.Input
{
    public abstract class InputService : IInputService

    {
        protected const string Horizontal = "Horizontal";
        protected const string Vertical = "Vertical";

        public abstract Vector2 Axis { get; }
        
        public bool IsRestartButtonUp() => 
            SimpleInput.GetButtonUp("Fire");

        protected static Vector2 SimpleInputAxis() => 
            new Vector2(SimpleInput.GetAxis(Horizontal), SimpleInput.GetAxis(Vertical));
    }
}