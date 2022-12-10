using System;

namespace _Game.CodeBase.Infrastucture
{
    public class BootstrapState : IState
    {
        private const string Initialscene = "InitialScene";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;

        public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }
        
        public void Enter()
        {
            _sceneLoader.Load(Initialscene, onLoaded: EnterLoadLevel);
        }

        private void EnterLoadLevel() => 
            _stateMachine.Enter<LoadLevelState, String>("Main");

        public void Exit()
        {
            
        }
    }
}