using CodeBase.Core;
using CodeBase.LevelMechanics;
using UnityEngine;

namespace CodeBase.UI
{
    public class FinishViewController : MonoBehaviour
    {
        //===================================================================================

        [SerializeField] private GameObject _winPanel;
        [SerializeField] private GameObject _losePanel;
        private bool _isRematch;

        //===================================================================================

        private void OnEnable()
        {
            LevelManager.Instance.controller.OnLevelCompleted += OnLevelCompleted;
            LevelManager.Instance.controller.OnLevelFailed += OnLevelFailed;
        }

        //===================================================================================

        private void OnDisable()
        {
            LevelManager.Instance.controller.OnLevelCompleted -= OnLevelCompleted;
            LevelManager.Instance.controller.OnLevelFailed -= OnLevelFailed;
        }

        //===================================================================================

        private void OnLevelCompleted()
        {
            _winPanel.SetActive(true);
            _losePanel.SetActive(false);
            _isRematch = false;
        }

        //===================================================================================

        private void OnLevelFailed()
        {
            _winPanel.SetActive(false);
            _losePanel.SetActive(true);
            _isRematch = true;
        }

        //===================================================================================

        public void OnFinishButtonClick()
        {
            GameMotor.Instance.StartGameInstantly(_isRematch);
        }

        //===================================================================================

    }
}