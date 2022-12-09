using CodeBase.Core;
using UnityEngine;

namespace CodeBase.Input
{
    public class InputController : MonoBehaviour
    {
        //===================================================================================

        public delegate void OnFirstInputDelegate();
        public static event OnFirstInputDelegate OnFirstInput;

        public delegate void OnInputChangedDelegate(float targetPosX);
        public static event OnInputChangedDelegate OnInputChanged;

        public delegate void OnInputEndedDelegate();
        public static event OnInputEndedDelegate OnInputEnded;

        [SerializeField][Range(0, 5)] private float _inputSpeed = 2f;
        [SerializeField] private float _leftPosX = -3f;
        [SerializeField] private float _rightPosX = 3f;

        private float _targetPosX;
        private float _distanceX;
        private float _screenRateWithPose;

        private bool _firstInput = false;

        private Vector3 _prevPos;
        private Vector3 _currentPos;

        //========================================================================

        private void Awake()
        {
            CalculateScreenRate();
        }

        //========================================================================

        private void OnEnable()
        {
            GameMotor.Instance.OnPrepareNewGame += OnPrepareNewGame;
        }

        private void OnDisable()
        {
            GameMotor.Instance.OnPrepareNewGame -= OnPrepareNewGame;
        }

        //========================================================================

        private void OnPrepareNewGame(bool b)
        {
            _firstInput = false;
        }

        //========================================================================

        private void Update()
        {
            if (UnityEngine.Input.touchCount > 1)
            {
                return;
            }

            if (!_firstInput)
            {
                if (UnityEngine.Input.GetMouseButtonDown(0))
                {
                    OnFirstInput?.Invoke();
                    _firstInput = true;
                }
            }

            if (_firstInput)
            {
                if (UnityEngine.Input.GetMouseButtonDown(0))
                {
                    _prevPos = UnityEngine.Input.mousePosition;
                }

                if (UnityEngine.Input.GetMouseButton(0))
                {
                    _currentPos = UnityEngine.Input.mousePosition;
                    _distanceX = _currentPos.x - _prevPos.x;
                    _targetPosX = ScreenRateWithPoseCalculateEditor(_distanceX);
                    _prevPos = _currentPos;
                }

                if (UnityEngine.Input.GetMouseButtonUp(0))
                {
                    OnInputEnded?.Invoke();
                    _targetPosX = 0;
                }
            }

            if (_firstInput)
            {
                OnInputChanged?.Invoke(_targetPosX);
            }
        }

        //========================================================================

        private float ScreenRateWithPoseCalculateEditor(float distanceX)
        {
            return _screenRateWithPose * distanceX;
        }

        //========================================================================

        private void CalculateScreenRate()
        {
            _screenRateWithPose = ((_rightPosX - _leftPosX) / (Screen.width /* -0 */)) * _inputSpeed;
        }

        //========================================================================
    }
}
