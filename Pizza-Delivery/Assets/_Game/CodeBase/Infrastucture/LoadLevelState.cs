using UnityEngine;

namespace _Game.CodeBase.Infrastucture
{
    public class LoadLevelState : IPayLoadedState<string>
    {
        private const string Initialpoint = "InitialPoint";
        private const string PlayerPlayer = "Player/Player";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;

        public LoadLevelState(GameStateMachine stateMachine, SceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter(string sceneName) => 
            _sceneLoader.Load(sceneName, OnLoaded);
        
        public void Exit()
        {
        }

        private void OnLoaded()
        {
            GameObject initialPoint = GameObject.FindWithTag(Initialpoint);
            GameObject player = Instantiate(PlayerPlayer, initialPoint.transform.position);
            
            Instantiate("Hud/Hud");
        }

        private static GameObject Instantiate(string path)
        {
            var prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab);
        }
        
        private static GameObject Instantiate(string path, Vector3 at)
        {
            var prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab, at, Quaternion.identity);
        }
    }
}