using System.Collections;
using CodeBase.Core;
using CodeBase.LevelMechanics;
using TMPro;
using UnityEngine;

namespace CodeBase.UI
{
    public class IngameViewController : MonoBehaviour
    {
        //===================================================================================

        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private TextMeshProUGUI _moneyIncreaseText;
        [SerializeField] private GameObject _controlHelper;
        [SerializeField] private RectTransform _controlHelperHand;

        private WaitForSeconds _controlHelperShowTime = new WaitForSeconds(4);

        private const float _helperHandRightPos = 200f;
        private const float _helperHandLeftPos = -200f;
        private const float _helperHandMoveTime = 1.3f;

        private const string _level = "LEVEL";
        private const string _plus = "+";
        private const string _minus = "-";


        //===================================================================================

        private void OnEnable()
        {
            GameMotor.Instance.OnStartGame += OnStartGame;
            GameMotor.Instance.OnPrepareNewGame += OnPrepareNewGame;
            LevelManager.Instance.controller.OnLevelIsCreated += OnLevelIsCreated;
            LevelManager.Instance.controller.OnMoneyChanged += OnMoneyChanged;
        }

        private void OnDisable()
        {
            GameMotor.Instance.OnStartGame -= OnStartGame;
            GameMotor.Instance.OnPrepareNewGame -= OnPrepareNewGame;
            LevelManager.Instance.controller.OnLevelIsCreated -= OnLevelIsCreated;
            LevelManager.Instance.controller.OnMoneyChanged -= OnMoneyChanged;
        }

        //===================================================================================

        private void OnStartGame()
        {
            //bool showControlsHelpers = LevelManager.Instance.data.GetCurrentLevel() <= 3;

            StartCoroutine(ShowControlsHelpers(true));
        }

        //===================================================================================

        private void OnPrepareNewGame(bool isRematch)
        {
            UpdateLevelLabels();
        }

        //===================================================================================

        private void OnLevelIsCreated()
        {

        }

        //===================================================================================

        private void OnMoneyChanged(double newAmount, double previousAmount)
        {

            double difference = newAmount - previousAmount;
            string operative = string.Empty;

            if (difference > 0)
            {
                operative = _plus;
            }

            if (operative != "")
            {
                _moneyIncreaseText.text = operative + difference;
            }
            else
            {
                _moneyIncreaseText.text = difference.ToString();
            }

        }

        //===================================================================================

        private void OnMoneyValueChanged(float value)
        {
            if (_moneyText)
            {
                _moneyText.SetText(value.ToString("F0"));
            }
        }

        //===================================================================================

        private IEnumerator ShowControlsHelpers(bool show = true)
        {
            if (_controlHelper)
            {
                if (show)
                {
                    _controlHelper.SetActive(true);



                    yield return _controlHelperShowTime;
                }

                _controlHelper.SetActive(false);
            }
        }

        //===================================================================================

        private void UpdateLevelLabels()
        {
            int currentLevel = LevelManager.Instance.data.GetCurrentLevel();

            if (_levelText)
            {
                _levelText.SetText(_level + " " + currentLevel);
            }
        }

        //===================================================================================


        //===================================================================================

    }
}