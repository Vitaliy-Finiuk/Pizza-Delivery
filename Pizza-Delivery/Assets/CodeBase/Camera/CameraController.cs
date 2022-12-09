using System.Collections;
using CodeBase.Extension;
using CodeBase.LevelMechanics;
using CodeBase.Player;
using Dreamteck.Splines;
using UnityEngine;

namespace CodeBase.Camera
{
    public class CameraController : Singleton<CameraController>
    {
        //===================================================================================

        public GameObject targetObject { get; private set; }
        public UnityEngine.Camera _camera { get; private set; }


        [SerializeField] private Vector3 _playerOffsetValue;
        [SerializeField] private ParticleSystem confettiParticle;
        [SerializeField] private float _elevationFactor;
        [SerializeField] private float _followSpeed = 10;
        [SerializeField] private float _playerCameraRotationIncrementRateForStack;


        private SplineFollower _playerSplineFollower;

        private bool isFollowingPlayer = true;

        private float _startElevationFactor;
        private float _stairYDifference;

        private Vector3 _startPlayerOffset;

        //===================================================================================

        private void Awake()
        {
            _camera = UnityEngine.Camera.main;
            _startPlayerOffset = _playerOffsetValue;
            _startElevationFactor = _elevationFactor;
        }

        //===================================================================================

        private void OnEnable()
        {
            LevelManager.Instance.controller.OnLevelIsCreated += OnLevelIsCreated;
            LevelManager.Instance.controller.OnLevelCompleted += OnLevelCompleted;
            LevelManager.Instance.controller.OnPlayerStackChanged += OnPlayerStackChanged;
        }

        //===================================================================================

        private void OnDisable()
        {
            LevelManager.Instance.controller.OnLevelIsCreated -= OnLevelIsCreated;
            LevelManager.Instance.controller.OnLevelCompleted -= OnLevelCompleted;
            LevelManager.Instance.controller.OnPlayerStackChanged -= OnPlayerStackChanged;
        }

        //===================================================================================

        private void LateUpdate()
        {
            if (targetObject == null || !IsFollowingPlayer())
            {
                return;
            }

            Vector3 targetPosition = _playerSplineFollower.result.position +
                                         _playerSplineFollower.result.right * _playerSplineFollower.motion.offset.x +
                                         _playerSplineFollower.result.up * _playerOffsetValue.y +
                                         _playerSplineFollower.result.forward * _playerOffsetValue.z;

            transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);

            float cameraRotationSpeed = 10f;

            float rotationStep = cameraRotationSpeed * Time.deltaTime;

            Vector3 direction = _playerSplineFollower.result.forward + Vector3.up * _elevationFactor;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), rotationStep);
        }

        //===================================================================================

        private bool IsFollowingPlayer()
        {
            return isFollowingPlayer;
        }

        //===================================================================================

        public void ShakeCamera(float delay, float duration, float strength = 3f, int vibrato = 10, float randomness = 90f, bool fadeOut = true)
        {
            StartCoroutine(Shake(delay, duration, strength, vibrato, randomness, fadeOut));
        }

        //===================================================================================

        public IEnumerator Shake(float delay, float duration, float strength = 3f, int vibrato = 10, float randomness = 90f, bool fadeOut = true)
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }

        }

        //===================================================================================

        private void OnLevelIsCreated()
        {
            if (confettiParticle)
            {
                if (confettiParticle.isPlaying)
                {
                    confettiParticle.Stop();
                    confettiParticle.Clear();
                }
                else
                {
                    confettiParticle.Clear();
                }
            }

            if (LevelManager.Instance.creator.player)
            {
                targetObject = LevelManager.Instance.creator.player.gameObject;
                Vector3 v3TargetPosition = targetObject.transform.position + _playerOffsetValue;
                transform.position = v3TargetPosition;

                _playerSplineFollower = PlayerController.Instance.movementController.splineFollower;

                _playerOffsetValue = _startPlayerOffset;
            }
        }

        //===================================================================================

        private void OnLevelCompleted()
        {
            if (confettiParticle)
            {
                confettiParticle.Play();
            }
        }

        //===================================================================================

        private void OnPlayerStackChanged(int currentCount, int previousCount)
        {
            //Possible adjustments
        }

        //===================================================================================

    }
}
