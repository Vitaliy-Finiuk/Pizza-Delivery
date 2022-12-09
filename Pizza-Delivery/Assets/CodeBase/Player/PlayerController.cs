using CodeBase.Extension;
using CodeBase.LevelMechanics;

namespace CodeBase.Player
{
    public class PlayerController : Singleton<PlayerController>
    {
        //===================================================================================

        public PlayerMovementController movementController { get; private set; }

        //===================================================================================

        private void Awake()
        {
            movementController = GetComponent<PlayerMovementController>();
        }

        //===================================================================================

        private void OnEnable()
        {
            LevelManager.Instance.controller.OnPlayerReachedEndOfSpline += OnPlayerReachedEndOfSpline;
        }

        private void OnDisable()
        {
            LevelManager.Instance.controller.OnPlayerReachedEndOfSpline -= OnPlayerReachedEndOfSpline;
        }

        //===================================================================================

        private void OnPlayerReachedEndOfSpline()
        {
            LevelManager.Instance.controller.CompleteLevel();
        }

        //===================================================================================

    }
}