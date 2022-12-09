using CodeBase.Extension;
using UnityEngine;

namespace CodeBase.Core
{
    public enum GameState
    {
        NONE,
        INITIALIZING,
        INITIALIZED,
        PREPARING_NEW_GAME,
        PLAYING,
        PLAYING_TUTORIAL,
        PAUSED,
        FINISHED_WAITING_USER_ACTION,
        FINISHED,
        RESUMING,
        REVIVEING,
        READY_TO_RUN,
        STARTING_TO_RUN
    };

    [DefaultExecutionOrder(-10)]
    public class GameMotor : Singleton<GameMotor>
    {
        //===================================================================================

        public GameState gameState;

        //===================================================================================

        public delegate void OnStateChangeDelegate(GameState oldGameState, GameState newGameState);
        public event OnStateChangeDelegate OnStateChange;

        public delegate void OnPrepareNewGameDelegate(bool isRematch = false);
        public event OnPrepareNewGameDelegate OnPrepareNewGame;

        public delegate void OnReadyToRunDelegate();
        public event OnReadyToRunDelegate OnReadyToRun;

        public delegate void OnStartGameDelegate();
        public event OnStartGameDelegate OnStartGame;

        public delegate void OnPauseGameDelegate();
        public event OnPauseGameDelegate OnPauseGame;

        public delegate void OnResumeGameDelegate();
        public event OnResumeGameDelegate OnResumeGame;

        public delegate void OnReviveGameDelegate();
        public event OnReviveGameDelegate OnReviveGame;

        public delegate void OnFinishGameWaitUserActionDelegate(bool win = true);
        public event OnFinishGameWaitUserActionDelegate OnFinishGameWaitUserAction;

        public delegate void OnFinishGameDelegate(bool win = true);
        public event OnFinishGameDelegate OnFinishGame;

        public delegate void OnExitGameDelegate();
        public event OnExitGameDelegate OnExitGame;

        //===================================================================================

        private void Start()
        {
            // May want to change this for specific targets
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
        }

        //===================================================================================

        public GameState GetState()
        {
            return gameState;
        }

        //===================================================================================

        public void SetState(GameState gameState)
        {
            GameState oldGameState = GetState();
            GameState newGameState = gameState;

            this.gameState = newGameState;

            OnStateChange?.Invoke(oldGameState, newGameState);
        }

        //===================================================================================

        public void PrepareNewGame(bool isRematch = false)
        {
            if (GetState() != GameState.READY_TO_RUN)
            {
                SetState(GameState.PREPARING_NEW_GAME);
                OnPrepareNewGame?.Invoke(isRematch);

                SetState(GameState.READY_TO_RUN);
                OnReadyToRun?.Invoke();
            }
        }

        //===================================================================================

        public void StartGame()
        {
            if (GetState() == GameState.READY_TO_RUN)
            {
                SetState(GameState.STARTING_TO_RUN);
                OnStartGame?.Invoke();

                SetState(GameState.PLAYING);
            }
        }

        //===================================================================================

        public void StartGameInstantly(bool bIsRematch = false)
        {
            PrepareNewGame(bIsRematch);

            StartGame();
        }

        //===================================================================================

        public void PauseGame()
        {
            if (IsPlaying())
            {
                SetState(GameState.PAUSED);

                OnPauseGame?.Invoke();
            }
        }

        //===================================================================================

        public void ResumeGame()
        {
            if (GetState() == GameState.PAUSED)
            {
                SetState(GameState.RESUMING);
                OnResumeGame?.Invoke();

                SetState(GameState.PLAYING);
            }
        }

        //===================================================================================

        public void ReviveGame()
        {
            if (GetState() == GameState.FINISHED_WAITING_USER_ACTION)
            {
                SetState(GameState.REVIVEING);
                OnReviveGame?.Invoke();

                SetState(GameState.PLAYING);
            }
        }

        //===================================================================================

        public void FinishGameWaitUserAction(bool bWin = true)
        {
            if (GetState() != GameState.FINISHED_WAITING_USER_ACTION)
            {
                SetState(GameState.FINISHED_WAITING_USER_ACTION);
                OnFinishGameWaitUserAction?.Invoke(bWin);
            }
        }

        //===================================================================================

        public void FinishGame(bool bWin = true)
        {
            if (GetState() != GameState.FINISHED)
            {
                SetState(GameState.FINISHED);
                OnFinishGame?.Invoke(bWin);
            }
        }

        //===================================================================================

        public void ExitGame(bool bPrepareNewGame = true)
        {
            SetState(GameState.NONE);
            OnExitGame?.Invoke();

            if (bPrepareNewGame)
            {
                PrepareNewGame();
            }
        }

        //===================================================================================

        public bool IsPlaying()
        {
            GameState gsCurrent = GetState();
            if (gsCurrent == GameState.PLAYING || gsCurrent == GameState.PLAYING_TUTORIAL)
            {
                return true;
            }

            return false;
        }

        //===================================================================================

    }
}