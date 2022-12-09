using System.Collections;
using CodeBase.Core;
using CodeBase.Input;
using CodeBase.LevelMechanics;
using Dreamteck.Splines;
using UnityEngine;


namespace CodeBase.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        //===================================================================================

        [SerializeField][Range(0, 2)] private float _playerPlatformXOffset;
        [SerializeField][Range(0, 90)] private float _maxRotationValue;
        [SerializeField][Range(0, 20)] private float _topSpeed;

        public SplineComputer currentSplineComputer { get; private set; }
        public SplineFollower splineFollower { get; private set; }

        private Vector3 _currentPlatformScale;
        private bool _isMoving;

        //===================================================================================

        private void Awake()
        {
            splineFollower = GetComponent<SplineFollower>();

            if (splineFollower)
            {
                currentSplineComputer = splineFollower.spline;

                if (!currentSplineComputer)
                {
                    StartCoroutine(TryGetCurrentSpline());
                }
            }
        }

        //===================================================================================

        private void OnEnable()
        {
            InputController.OnFirstInput += OnFirstInput;
            InputController.OnInputChanged += OnInputChanged;
            InputController.OnInputEnded += OnInputEnded;

            GameMotor.Instance.OnFinishGame += OnFinishGame;

            splineFollower.onEndReached += OnEndReached;
        }


        private void OnDisable()
        {
            InputController.OnFirstInput -= OnFirstInput;
            InputController.OnInputChanged -= OnInputChanged;
            InputController.OnInputEnded -= OnInputEnded;

            GameMotor.Instance.OnFinishGame -= OnFinishGame;

            splineFollower.onEndReached -= OnEndReached;
        }

        //===================================================================================

        private void Start()
        {
            if (splineFollower)
            {
                splineFollower.followSpeed = 0;
            }

            Vector3 currentSplineScale = LevelManager.Instance.creator.SplineScale;

            _currentPlatformScale = new Vector3(
                currentSplineScale.x + currentSplineScale.x * 2 - _playerPlatformXOffset,
                currentSplineScale.y,
                currentSplineScale.z);
        }

        //===================================================================================

        private void Update()
        {
            if (GameMotor.Instance.IsPlaying())
            {
                if (_isMoving)
                {
                    if (splineFollower)
                    {
                        //If there is a slider on UI
                        CheckLevelProgressValue();
                    }
                }
            }
        }

        //===================================================================================

        private void CheckLevelProgressValue()
        {
            LevelManager.Instance.controller.ChangeLevelProgressValue(0, 1, (float)splineFollower.result.percent);
        }

        //===================================================================================

        private void OnFirstInput()
        {
            _isMoving = true;

            splineFollower.followSpeed = _topSpeed;
        }

        //===================================================================================

        private void OnInputChanged(float xInputValue)
        {
            if (splineFollower)
            {
                if (splineFollower.follow)
                {
                    if (_isMoving)
                    {
                        MovePlayer(xInputValue);
                        RotatePlayer(xInputValue);
                    }
                }
            }
        }

        //===================================================================================

        private void OnInputEnded()
        {
            //DOVirtual.Float(splineFollower.motion.rotationOffset.y, 0, 0.1f,
                //value => splineFollower.motion.rotationOffset = new Vector3(0, value, 0));
        }

        //===================================================================================

        private void OnFinishGame(bool b)
        {
            _isMoving = false;
            splineFollower.follow = false;
        }

        //===================================================================================

        private void OnEndReached(double d)
        {
            LevelManager.Instance.controller.PlayerReachedEndOfSpline();

            splineFollower.follow = false;
            _isMoving = false;
        }

        //===================================================================================

        public void MovePlayer(float xInputValue)
        {
            Vector2 currentPosition = splineFollower.motion.offset;
            float newXValue = currentPosition.x + xInputValue;
            newXValue = Mathf.Clamp(newXValue, -_currentPlatformScale.x, _currentPlatformScale.x);

            Vector2 targetMotionPos = new Vector2(newXValue, currentPosition.y);

            if (xInputValue != 0f)
            {
                splineFollower.motion.offset = Vector2.Lerp(splineFollower.motion.offset, targetMotionPos, 3000f * Time.deltaTime);
            }
        }

        //===================================================================================

        private void RotatePlayer(float deltaX)
        {
            if (deltaX > 0)
            {
                splineFollower.motion.rotationOffset = Vector2.Lerp(splineFollower.motion.rotationOffset, TargetRotationMotion(true), 10f * Time.deltaTime);
            }

            else if (deltaX < 0)
            {
                splineFollower.motion.rotationOffset = Vector2.Lerp(splineFollower.motion.rotationOffset, TargetRotationMotion(false), 10f * Time.deltaTime);
            }
        }

        //===================================================================================

        private Vector3 TargetRotationMotion(bool isLeft)
        {
            return isLeft
                ? Vector3.up * _maxRotationValue
                : Vector3.down * _maxRotationValue;
        }

        //===================================================================================
        //===================================================================================
        //===================================================================================

        private IEnumerator TryGetCurrentSpline()
        {
            int tryGetCount = 0;

            while (tryGetCount < Screen.currentResolution.refreshRate)
            {
                while (!currentSplineComputer)
                {
                    currentSplineComputer = splineFollower.spline;

                    if (currentSplineComputer)
                    {
                        tryGetCount = Screen.currentResolution.refreshRate;

                        StartCoroutine(SetStartPosition());

                        break;
                    }

                    tryGetCount++;

                    yield return null;
                }
            }
        }

        //===================================================================================

        private IEnumerator SetStartPosition()
        {
            if (!splineFollower) yield break;

            while (splineFollower.result.percent != 0)
            {
                yield return null;
                splineFollower.SetPercent(0);
            }

            LevelManager.Instance.controller.ChangeLevelProgressValue(0, 1, (float)splineFollower.result.percent);

        }

        //===================================================================================

    }
}