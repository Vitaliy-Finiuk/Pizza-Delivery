using UnityEngine;

namespace _Game.CodeBase.Infrastucture
{
    public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
    {
        private Game _game;

        private void Awake()
        {
            _game = new Game(this);
            _game.StateMachine.Enter<BootstrapState>();

            DontDestroyOnLoad(this);
        }
    }
}