using CodeBase.Core;
using UnityEngine;

namespace CodeBase.LevelMechanics
{
    public class LevelData : MonoBehaviour
    {
        //===================================================================================

        private int _level;
        private int _money;
        private int _levelMoney;

        private const string _levelKey = "game_level";
        private const string _moneyKey = "game_money";

        //===================================================================================

        private void OnEnable()
        {
            GameMotor.Instance.OnPrepareNewGame += OnPrepareNewGame;
            GameMotor.Instance.OnFinishGame += OnFinishGame;
        }

        //===================================================================================

        private void OnDisable()
        {
            GameMotor.Instance.OnPrepareNewGame -= OnPrepareNewGame;
            GameMotor.Instance.OnFinishGame -= OnFinishGame;
        }

        //===================================================================================

        private void OnPrepareNewGame(bool b)
        {
            LoadGameData();

            _levelMoney = 0;
        }

        //===================================================================================

        private void OnFinishGame(bool isWin = true)
        {
            if (isWin)
            {
                SetCurrentLevel(_level + 1);
                SetCurrentMoney(_money + _levelMoney);
            }
            else
            {
                _levelMoney = 0;
            }
        }

        //===================================================================================

        private void LoadGameData()
        {
            LoadLevelData();
            LoadMoneyData();
        }

        //===================================================================================

        private void LoadLevelData()
        {
            if (PlayerPrefs.HasKey(_levelKey))
            {
                _level = PlayerPrefs.GetInt(_levelKey);
            }
            else
            {
                _level = 1;
            }
        }

        //===================================================================================

        private void LoadMoneyData()
        {
            if (PlayerPrefs.HasKey(_moneyKey))
            {
                _money = PlayerPrefs.GetInt(_moneyKey);
            }
            else
            {
                _money = 0;
            }

            LevelManager.Instance.controller.MoneyChanged(_money, 0);
        }

        //===================================================================================

        private void SaveGameData()
        {
            SaveLevelData();
            SaveMoneyData();
        }

        //===================================================================================

        private void SaveLevelData()
        {
            PlayerPrefs.SetInt(_levelKey, _level);
        }

        //===================================================================================

        private void SaveMoneyData()
        {
            PlayerPrefs.SetFloat(_moneyKey, _money);
        }

        //===================================================================================

        public int GetCurrentLevel()
        {
            return _level;
        }

        //===================================================================================

        public void SetCurrentLevel(int level)
        {
            _level = level;
            SaveLevelData();
        }

        //===================================================================================

        public int GetCurrentMoney()
        {
            return _money;
        }

        //===================================================================================

        public void SetCurrentMoney(int value)
        {
            _money = value;
            SaveMoneyData();
        }

        //===================================================================================

        public int GetLevelMoney()
        {
            return _levelMoney;
        }

        //===================================================================================

        public void IncreaseLevelMoney(int value = 1)
        {
            int previousMoney = _levelMoney;
            int newMoney = _levelMoney + value;

            _levelMoney += value;

            LevelManager.Instance.controller.MoneyChanged(newMoney, previousMoney);
        }

        //===================================================================================

    }
}