using _Game.CodeBase.Services.Input;
using UnityEngine;

namespace _Game.CodeBase.Infrastucture
{
    public class Game
    {
        public static IInputService InputService;
        public GameStateMachine StateMachine;
    
        public Game(ICoroutineRunner coroutineRunner)
        {
            StateMachine = new GameStateMachine(new SceneLoader(coroutineRunner));
        
            RegisterInputService();
        }

        public static void RegisterInputService()
        {
            if (Application.isEditor)
                InputService = new StandaloneInputService();
            else
                InputService = new MobileInputService();
        }
    }
}