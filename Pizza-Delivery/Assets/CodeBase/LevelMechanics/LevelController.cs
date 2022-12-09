using CodeBase.Core;
using UnityEngine;

namespace CodeBase.LevelMechanics
{
    public class LevelController : MonoBehaviour
    {
        //===================================================================================

        private bool _isLevelEnded;
        private int _diamondAmount;

        //
        // Level
        //
        public delegate void OnLevelFailedDelegate();
        public event OnLevelFailedDelegate OnLevelFailed;

        public delegate void OnLevelCompletedDelegate();
        public event OnLevelCompletedDelegate OnLevelCompleted;

        public delegate void OnLevelDrawDelegate();
        public event OnLevelDrawDelegate OnLevelDraw;

        public delegate void OnStageCompletedDelegate(int nStage);
        public event OnStageCompletedDelegate OnStageCompleted;

        public delegate void OnStageChangedDelegate(int nStage);
        public event OnStageChangedDelegate OnStageChanged;

        public delegate void OnLevelChangedDelegate(int nLevel);
        public event OnLevelChangedDelegate OnLevelChanged;

        //
        // Generic
        //
        public delegate void OnLevelProgressValueChangedDelegate(float min, float max, float value);
        public event OnLevelProgressValueChangedDelegate OnLevelProgressValueChanged;

        public delegate void OnScoreValueChangedDelegate();
        public event OnScoreValueChangedDelegate OnScoreValueUpdated;

        public delegate void OnDiamondAmountChangedDelegate(int amount);
        public event OnDiamondAmountChangedDelegate OnDiamondAmountChanged;

        //
        // In Game
        //
        public delegate void OnLevelIsCreatedDelegate();
        public event OnLevelIsCreatedDelegate OnLevelIsCreated;

        public delegate void OnMoneyChangedDelegate(double newAmount = 0, double previousAmount = 0);
        public event OnMoneyChangedDelegate OnMoneyChanged;

        //
        // Player

        public delegate void OnPlayerStackChangedDelegate(int currentCount, int previousCount);
        public event OnPlayerStackChangedDelegate OnPlayerStackChanged;

        public delegate void OnPlayerStackLevelChangedDelegate(int currentLevel = 0, int previousLevel = 0);
        public event OnPlayerStackLevelChangedDelegate OnPlayerStackLevelChanged;

        public delegate void OnPlayerTitleChangedDelegate(string currentTitle);
        public event OnPlayerTitleChangedDelegate OnPlayerTitleChanged;

        public delegate void OnPlayerReachedEndOfSplineDelegate();
        public event OnPlayerReachedEndOfSplineDelegate OnPlayerReachedEndOfSpline;

        // public delegate void OnPlayerCollidedWithGateDelegate(GateController gate);
        // public event OnPlayerCollidedWithGateDelegate OnPlayerCollidedWithGate;

        //===================================================================================

        private void OnEnable()
        {
            GameMotor.Instance.OnPrepareNewGame += OnPrepareNewGame;
        }

        //===================================================================================

        private void OnDisable()
        {
            GameMotor.Instance.OnPrepareNewGame -= OnPrepareNewGame;
        }

        //===================================================================================

        private void OnPrepareNewGame(bool bIsRematch = false)
        {
            _diamondAmount = PlayerPrefs.GetInt("diamond");
            ChangeDiamondAmount(_diamondAmount);

            _isLevelEnded = false;
        }

        //===================================================================================

        public void FailLevel()
        {
            if (!_isLevelEnded)
            {
                _isLevelEnded = true;
                OnLevelFailed?.Invoke();
            }
        }

        //===================================================================================

        public void CompleteLevel()
        {
            if (!_isLevelEnded)
            {
                _isLevelEnded = true;
                OnLevelCompleted?.Invoke();
            }
        }

        //===================================================================================

        public void DrawLevel()
        {
            if (!_isLevelEnded)
            {
                _isLevelEnded = true;
                OnLevelDraw?.Invoke();
            }
        }

        //===================================================================================

        public void CompleteStage(int nStage)
        {
            OnStageCompleted?.Invoke(nStage);
        }

        //===================================================================================

        public void ChangeStage(int nStage)
        {
            OnStageChanged?.Invoke(nStage);
        }

        //===================================================================================

        public void ChangeLevel(int nLevel)
        {
            OnLevelChanged?.Invoke(nLevel);
        }

        //===================================================================================

        public void ChangeLevelProgressValue(float min, float max, float value)
        {
            OnLevelProgressValueChanged?.Invoke(min, max, value);
        }

        //===================================================================================

        public void UpdateScoreValue()
        {
            OnScoreValueUpdated?.Invoke();
        }

        //===================================================================================

        public void DiamondIncreased(int amount = 1)
        {
            _diamondAmount++;
            ChangeDiamondAmount(_diamondAmount);
        }

        //===================================================================================

        private void ChangeDiamondAmount(int amount)
        {
            OnDiamondAmountChanged?.Invoke(amount);
        }

        //===================================================================================

        public void LevelIsCreated()
        {
            OnLevelIsCreated?.Invoke();
        }

        //===================================================================================

        public void MoneyChanged(double newAmount = 0, double previousAmount = 0)
        {
            OnMoneyChanged?.Invoke(newAmount, previousAmount);
        }

        //===================================================================================

        public void PlayerReachedEndOfSpline()
        {
            OnPlayerReachedEndOfSpline?.Invoke();
        }

        // //===================================================================================


        //===================================================================================

        public void PlayerStackChanged(int currentCount, int previousCount)
        {
            OnPlayerStackChanged?.Invoke(currentCount, previousCount);
        }

        //===================================================================================

        public void PlayerStackLevelChanged(int level = 0, int previousLevel = 0)
        {
            OnPlayerStackLevelChanged?.Invoke(level, previousLevel);
        }

        //===================================================================================

        public void PlayerTitleChanged(string title)
        {
            OnPlayerTitleChanged?.Invoke(title);
        }

        //===================================================================================

        // public void PlayerCollidedWithGate(GateController gate)
        // {
        //     OnPlayerCollidedWithGate?.Invoke(gate);
        // }

        //===================================================================================

    }
}