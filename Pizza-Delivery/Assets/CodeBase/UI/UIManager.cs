using CodeBase.Core;
using CodeBase.Extension;
using UnityEngine;

namespace CodeBase.UI
{
    public class UIManager : Singleton<UIManager>
    {
        //===================================================================================

        public Canvas mainCanvas;

        [SerializeField] private GameObject _ingameView;
        [SerializeField] private GameObject _finishView;
        [SerializeField] private GameObject _loadingView;

        //===================================================================================

        private void OnEnable()
        {
            GameMotor.Instance.OnPrepareNewGame += OnPrepareNewGame;
            GameMotor.Instance.OnReadyToRun += OnReadyToRun;
            GameMotor.Instance.OnStartGame += OnStartGame;
            GameMotor.Instance.OnFinishGame += OnFinishGame;
        }

        //===================================================================================

        private void OnDisable()
        {
            GameMotor.Instance.OnPrepareNewGame -= OnPrepareNewGame;
            GameMotor.Instance.OnReadyToRun -= OnReadyToRun;
            GameMotor.Instance.OnStartGame -= OnStartGame;
            GameMotor.Instance.OnFinishGame -= OnFinishGame;
        }

        //===================================================================================

        private void ShowView(GameObject uiView)
        {
            if (uiView)
            {
                uiView.SetActive(true);
            }
        }

        //===================================================================================

        private void HideView(GameObject uiView)
        {
            if (uiView)
            {
                uiView.SetActive(false);
            }
        }

        //===================================================================================

        private void HideVisibleViews()
        {
            foreach (Transform view in mainCanvas.transform)
            {
                view.gameObject.SetActive(false);
            }
        }

        //===================================================================================

        private void HideVisibleViewsExcept(GameObject uiView)
        {
            foreach (Transform view in mainCanvas.transform)
            {
                if (uiView)
                {
                    if (view != uiView)
                    {
                        view.gameObject.SetActive(false);
                    }
                }
            }
        }

        //===================================================================================

        private void ShowSolo(GameObject uiView)
        {
            HideVisibleViewsExcept(uiView);

            if (uiView)
            {
                uiView.SetActive(true);
            }
        }

        //===================================================================================

        private void OnPrepareNewGame(bool b)
        {
            ShowView(_loadingView);
        }

        //===================================================================================

        private void OnReadyToRun()
        {
            HideView(_loadingView);
        }

        //===================================================================================

        private void OnStartGame()
        {
            ShowSolo(_ingameView);
        }

        //===================================================================================

        private void OnFinishGame(bool b)
        {
            ShowSolo(_finishView);
        }

        //===================================================================================

    }
}