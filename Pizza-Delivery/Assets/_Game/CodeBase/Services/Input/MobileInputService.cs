using UnityEngine;

namespace _Game.CodeBase.Services.Input
{
    public class MobileInputService : InputService
    {
        public override Vector2 Axis => SimpleInputAxis();
    }
}