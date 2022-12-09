using _Game.CodeBase.Input;
using UnityEngine;

namespace _Game.CodeBase.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
    public class Bicycle : MonoBehaviour
    {
        [SerializeField] private WheelCollider _collFrontWheelCollider;
        [SerializeField] private WheelCollider _collRearWheelWheelCollider;
        
        [SerializeField] private GameObject _meshFrontWheel;
        [SerializeField] private GameObject _meshRearWheel;

        private bool _isFrontWheelInAir = true;

        private float _stiffPowerGain = 0.0f;

        private const float TMPMassShift = 0.0f;

        [Header("Crashed Status")]
        [SerializeField] private bool _crashed = false;
        
        [Range(0f, 360f)] [SerializeField] private float _crashAngleSideFall01 = 50f;												
        [Range(0f, 360f)] [SerializeField] private float _crashAngleSideFall02 = 310f; 								
        [Range(0f, 360f)] [SerializeField] private float _crashAngleFrontFall = 60f; 												
        [Range(0f, 360f)] [SerializeField] private float _crashAngleBackFall = 280f;												
        
        public Transform CenterObjectMass;

        [Range(-10f, 60f)] [SerializeField] private float _normalCoM = 0f;									

        [Range(-10f, 60f)] [SerializeField] private float _coMWhenCrahsed = 0f;										

        [SerializeField] private Transform _rearPendulumn;
        [SerializeField] private Transform _steeringWheel;
        [SerializeField] private Transform _suspensionFrontDown;
        private int _normalFrontSuspSpring;
        private int _normalRearSuspSpring;
        private bool _forgeBlocked = true;


        private float _baseDistance;

        public AnimationCurve wheelbarRestrictCurve = new AnimationCurve(new Keyframe(0f, 20f),
                    new Keyframe(100f, 1f));

        private float _tempMaxWheelAngle;

        private Vector3 _wheelCCenter;
        private RaycastHit _hit;

        [Range(0f, 100f)] [SerializeField] private float _frontBrakePower;
        [Range(0f, 100f)] [SerializeField] private float _legsPower;
        [Range(0f, 100f)] [SerializeField] private float _airResistant; 																										// 1 is neutral

        private float _frontWheelApd;
        [SerializeField] private GameObject _pedals;
        private PedalControls _linkToStunt;
        private bool _rearPend;

        [HideInInspector] public float BikeSpeed;

        private Rigidbody _rigidbody;
        public PlayerInput _playerInput;


        private int _springWeakness = 0;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            _rearPend = _rearPendulumn;
            
            _frontWheelApd = _collFrontWheelCollider.forceAppPointDistance;
            

            _linkToStunt = _pedals.GetComponent<PedalControls>();

            Vector3 setInitialTensor = _rigidbody.inertiaTensor;
            _rigidbody.centerOfMass = new Vector3(CenterObjectMass.localPosition.x, CenterObjectMass.localPosition.y, CenterObjectMass.localPosition.z);
            _rigidbody.inertiaTensor = setInitialTensor; 
            
            _legsPower = _legsPower * 20;
            
            _frontBrakePower = _frontBrakePower * 30;
            
            _normalRearSuspSpring = (int)_collRearWheelWheelCollider.suspensionSpring.spring;
            _normalFrontSuspSpring = (int)_collFrontWheelCollider.suspensionSpring.spring;

            var localPosition = _collRearWheelWheelCollider.transform.localPosition;
            _baseDistance =
                _collFrontWheelCollider.transform.localPosition.z -
                localPosition.z;

            var tmpMeshRWh01 = _meshRearWheel.transform.localPosition;
            tmpMeshRWh01.y = _meshRearWheel.transform.localPosition.y -
                             _collRearWheelWheelCollider.suspensionDistance / 4;
            _meshRearWheel.transform.localPosition = tmpMeshRWh01;


            //and bike's frame direction
            var tmpCollRW01 = localPosition;
            tmpCollRW01.y = localPosition.y - localPosition.y / 20;
            localPosition = tmpCollRW01;
            _collRearWheelWheelCollider.transform.localPosition = localPosition;
        }

        private void FixedUpdate()
        {
            ApplyLocalPositionToVisuals(_collFrontWheelCollider);
            ApplyLocalPositionToVisuals(_collRearWheelWheelCollider);

            if (_rearPend)
            {
                var tmp_cs1 = _rearPendulumn.transform.localRotation;
                var tmp_cs2 = tmp_cs1.eulerAngles;
                tmp_cs2.x = 0 - 8 + (_meshRearWheel.transform.localPosition.y * 100);
                tmp_cs1.eulerAngles = tmp_cs2;
                _rearPendulumn.transform.localRotation = tmp_cs1;
            }

            var transform1 = _suspensionFrontDown.transform;
            var localPosition = transform1.localPosition;
            var tmp_cs3 = localPosition;
            var position = _meshFrontWheel.transform.localPosition;
            
            tmp_cs3.y = (position.y - 0.15f);
            localPosition = tmp_cs3;
            transform1.localPosition = localPosition;


            var tmp_cs4 = position;
            tmp_cs4.z = position.z - (localPosition.y + 0.4f) / 5;
            position = tmp_cs4;
            _meshFrontWheel.transform.localPosition = position;


            if (!_crashed)
            {
                _rigidbody.drag = _rigidbody.velocity.magnitude / 210 * _airResistant;
                _rigidbody.angularDrag = 7 + _rigidbody.velocity.magnitude / 20;
            }

            DetermineFromMetersPerSecondToKilometers();

            _collFrontWheelCollider.forceAppPointDistance = _frontWheelApd - BikeSpeed / 1000;

            if (_collFrontWheelCollider.forceAppPointDistance < 0.001f)
                _collFrontWheelCollider.forceAppPointDistance = 0.001f;

            Accelerate();

            if (!_crashed && _playerInput.Vertical > 0.9f)
                FullThrottle();
            else RearSuspensionRestoration();

            if (!_crashed && _playerInput.Vertical < 0 && !_isFrontWheelInAir)
                FrontBrake();
            else FrontSuspensionRestoration(_springWeakness);

            RearBrake();

            FrontWheelSteeringLimits();

            AutomaticWheelReturn();

            RestartBikePosition();

            CrashHappened();
        }

        private void Accelerate()
        {
            if (!_crashed && _playerInput.Vertical > 0)
            {
                _collFrontWheelCollider.brakeTorque = 0;
                _collRearWheelWheelCollider.motorTorque = _legsPower * _playerInput.Vertical;

                var tmp_cs5 = CenterObjectMass.localPosition;
                tmp_cs5.z = 0.0f + TMPMassShift;
                tmp_cs5.y = _normalCoM;
                CenterObjectMass.localPosition = tmp_cs5;

                _rigidbody.centerOfMass = new Vector3(CenterObjectMass.localPosition.x, CenterObjectMass.localPosition.y, CenterObjectMass.localPosition.z);
            }
        }

        private void DetermineFromMetersPerSecondToKilometers() =>
            BikeSpeed = Mathf.Round((_rigidbody.velocity.magnitude * 3.6f) * 10) * 0.1f; //from m/s to km/h

        private void FrontWheelSteeringLimits()
        {
            _tempMaxWheelAngle = wheelbarRestrictCurve.Evaluate(BikeSpeed);

            if (!_crashed && _playerInput.Horizontal != 0)
            {
                _collFrontWheelCollider.steerAngle = _tempMaxWheelAngle * _playerInput.Horizontal;
                _steeringWheel.rotation = _collFrontWheelCollider.transform.rotation * Quaternion.Euler(0,
                    _collFrontWheelCollider.steerAngle, _collFrontWheelCollider.transform.rotation.z);
            }
            else _collFrontWheelCollider.steerAngle = 0;
        }

        private void AutomaticWheelReturn()
        {
            if (!_crashed && _playerInput.Vertical == 0 && !_linkToStunt.stuntIsOn ||
                (_playerInput.Vertical < 0 && _isFrontWheelInAir))
            {
                var tmp_cs23 = CenterObjectMass.localPosition;

                tmp_cs23.y = _normalCoM;
                tmp_cs23.z = 0.0f + TMPMassShift;
                CenterObjectMass.localPosition = tmp_cs23;

                _collFrontWheelCollider.motorTorque = 0;
                _collFrontWheelCollider.brakeTorque = 0;
                _collRearWheelWheelCollider.motorTorque = 0;
                _collRearWheelWheelCollider.brakeTorque = 0;

                _rigidbody.centerOfMass = new Vector3(CenterObjectMass.localPosition.x, CenterObjectMass.localPosition.y, CenterObjectMass.localPosition.z);
            }
        }

        private void FullThrottle()
        {
            _collFrontWheelCollider.brakeTorque = 0;
            _collRearWheelWheelCollider.motorTorque = _legsPower * 1.2f;
            _rigidbody.angularDrag = 20;

            CenterObjectMass.localPosition = new Vector3(CenterObjectMass.localPosition.z, CenterObjectMass.localPosition.y,
                -(1.38f - _baseDistance / 1.4f) + TMPMassShift);

            float stoppieEmpower = (BikeSpeed / 3) / 100;

            float angleLeanCompensate = 0.0f;

            if (this.transform.localEulerAngles.z < 70)
            {
                angleLeanCompensate = this.transform.localEulerAngles.z / 30;

                if (angleLeanCompensate > 0.5f)
                    angleLeanCompensate = 0.5f;
            }

            if (this.transform.localEulerAngles.z > 290)
            {
                angleLeanCompensate = (360 - this.transform.localEulerAngles.z) / 30;

                if (angleLeanCompensate > 0.5f)
                    angleLeanCompensate = 0.5f;
            }

            if (stoppieEmpower + angleLeanCompensate > 0.5f)
                stoppieEmpower = 0.5f;

            CenterObjectMass.localPosition = new Vector3(CenterObjectMass.localPosition.x, -(0.995f - _baseDistance / 2.8f) - stoppieEmpower,
                CenterObjectMass.localPosition.z);


            _rigidbody.centerOfMass = new Vector3(CenterObjectMass.localPosition.x, CenterObjectMass.localPosition.y, CenterObjectMass.localPosition.z);


            var tmpSpsSprg01 = _collRearWheelWheelCollider.suspensionSpring;
            tmpSpsSprg01.spring = 200000;
            _collRearWheelWheelCollider.suspensionSpring = tmpSpsSprg01;
        }

        private void FrontBrake()
        {
            _collFrontWheelCollider.brakeTorque = _frontBrakePower * -_playerInput.Vertical;
            _collRearWheelWheelCollider.motorTorque = 0;

            if (BikeSpeed > 1)
            {
                float rearBrakeAddon = 0.0f;

                var tmp_cs11 = CenterObjectMass.localPosition;
                tmp_cs11.y += (_frontBrakePower / 200000) + TMPMassShift / 50f - rearBrakeAddon;
                tmp_cs11.z += 0.0025f;
                CenterObjectMass.localPosition = tmp_cs11;
            }
            else if (BikeSpeed <= 1 && !_crashed && this.transform.localEulerAngles.z < 45 ||
                     BikeSpeed <= 1 && !_crashed && this.transform.localEulerAngles.z > 315)
            {
                if (this.transform.localEulerAngles.x < 5 || this.transform.localEulerAngles.x > 355)
                {
                    var tmp_cs12 = CenterObjectMass.localPosition;
                    tmp_cs12.y = _normalCoM;
                    CenterObjectMass.localPosition = tmp_cs12;
                }
            }

            if (CenterObjectMass.localPosition.y >= -0.2f)
            {
                var tmp_cs13 = CenterObjectMass.localPosition;
                tmp_cs13.y = -0.2f;
                CenterObjectMass.localPosition = tmp_cs13;
            }

            if (CenterObjectMass.localPosition.z >= 0.2f + (GetComponent<Rigidbody>().mass / 500))
                CenterObjectMass.localPosition = new Vector3(CenterObjectMass.localPosition.x, 0.2f + (_rigidbody.mass / 500),
                    CenterObjectMass.localPosition.z);

            float maxFrontSuspConstrain;
            maxFrontSuspConstrain = CenterObjectMass.localPosition.z;

            if (maxFrontSuspConstrain >= 0.5f) maxFrontSuspConstrain = 0.5f;
            _springWeakness = (int)(_normalFrontSuspSpring - (_normalFrontSuspSpring * 1.5f) * maxFrontSuspConstrain);

            _rigidbody.centerOfMass = new Vector3(CenterObjectMass.localPosition.x, CenterObjectMass.localPosition.y, CenterObjectMass.localPosition.z);

            _forgeBlocked = true;
        }

        private void RearBrake()
        {
            _collRearWheelWheelCollider.brakeTorque = 0;

            _stiffPowerGain = _stiffPowerGain -= 0.05f;

            if (_stiffPowerGain < 0)
                _stiffPowerGain = 0;

            var tmp_cs17 = _collRearWheelWheelCollider.sidewaysFriction;
            tmp_cs17.stiffness = 1.0f - _stiffPowerGain;
            _collRearWheelWheelCollider.sidewaysFriction = tmp_cs17;

            var tmp_cs18 = _collFrontWheelCollider.sidewaysFriction;
            tmp_cs18.stiffness = 1.0f - _stiffPowerGain;
            _collFrontWheelCollider.sidewaysFriction = tmp_cs18;
        }

        private void RestartBikePosition()
        {
            if (_playerInput.isRestartBike)
            {
                var localPosition = CenterObjectMass.localPosition;
                var tmp_cs26 = localPosition;

                _crashed = false;
                transform.position += new Vector3(0, 0.1f, 0);
                transform.rotation = Quaternion.Euler(0.0f, transform.localEulerAngles.y, 0.0f);

                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;


                tmp_cs26.x = 0.0f;
                tmp_cs26.y = _normalCoM;
                tmp_cs26.z = 0.0f;

                localPosition = tmp_cs26;
                CenterObjectMass.localPosition = localPosition;
                //for fix bug when front wheel IN ground after restart(sorry, I really don't understand why it happens);
                _collFrontWheelCollider.motorTorque = 0;
                _collFrontWheelCollider.brakeTorque = 0;
                _collRearWheelWheelCollider.motorTorque = 0;
                _collRearWheelWheelCollider.brakeTorque = 0;
                _rigidbody.centerOfMass = new Vector3(localPosition.x, localPosition.y, localPosition.z);
            }
        }

        private void CrashHappened()
        {
            if ((this.transform.localEulerAngles.z >= _crashAngleSideFall01 &&
                 this.transform.localEulerAngles.z <= _crashAngleSideFall02) && !_linkToStunt.stuntIsOn ||
                (this.transform.localEulerAngles.x >= _crashAngleFrontFall &&
                 this.transform.localEulerAngles.x <= _crashAngleBackFall && !_linkToStunt.stuntIsOn))
            {
                _rigidbody.drag = 0.1f;
                _rigidbody.angularDrag = 0.01f;
                _crashed = true;
                var tmp_cs27 = CenterObjectMass.localPosition;
                tmp_cs27.x = 0.0f;
                tmp_cs27.y = _coMWhenCrahsed;
                tmp_cs27.z = 0.0f;
                CenterObjectMass.localPosition = tmp_cs27;
                _rigidbody.centerOfMass = new Vector3(CenterObjectMass.localPosition.x, CenterObjectMass.localPosition.y, CenterObjectMass.localPosition.z);
            }

            if (_crashed) _collRearWheelWheelCollider.motorTorque = 0;
        }

        private void ApplyLocalPositionToVisuals(WheelCollider collider)
        {
            if (collider.transform.childCount == 0)
            {
                return;
            }

            Transform visualWheel = collider.transform.GetChild(0);
            _wheelCCenter = collider.transform.TransformPoint(collider.center);

            if (!_rearPend)
            {
                //case where MTB have no rear suspension
                if (collider.gameObject.name != "coll_rear_wheel")
                {
                    if (Physics.Raycast(_wheelCCenter, -collider.transform.up, out _hit,
                            collider.suspensionDistance + collider.radius))
                    {
                        visualWheel.transform.position = _hit.point + (collider.transform.up * collider.radius);
                        if (collider.name == "coll_front_wheel") _isFrontWheelInAir = false;
                    }
                    else
                    {
                        visualWheel.transform.position =
                            _wheelCCenter - (collider.transform.up * collider.suspensionDistance);
                        if (collider.name == "coll_front_wheel") _isFrontWheelInAir = true;
                    }
                }
            }
            else
            {
                if (Physics.Raycast(_wheelCCenter, -collider.transform.up, out _hit,
                        collider.suspensionDistance + collider.radius))
                {
                    visualWheel.transform.position = _hit.point + (collider.transform.up * collider.radius);
                    if (collider.name == "coll_front_wheel") _isFrontWheelInAir = false;
                }
                else
                {
                    visualWheel.transform.position =
                        _wheelCCenter - (collider.transform.up * collider.suspensionDistance);
                    if (collider.name == "coll_front_wheel") _isFrontWheelInAir = true;
                }
            }

            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;

            collider.GetWorldPose(out position, out rotation);

            visualWheel.localEulerAngles = new Vector3(visualWheel.localEulerAngles.x,
                collider.steerAngle - visualWheel.localEulerAngles.z, visualWheel.localEulerAngles.z);
            visualWheel.Rotate(collider.rpm / 60 * 360 * Time.deltaTime, 0.0f, 0.0f);
        }

        private void RearSuspensionRestoration()
        {
            var tmpRearSusp = _collRearWheelWheelCollider.suspensionSpring;
            tmpRearSusp.spring = _normalRearSuspSpring;
            _collRearWheelWheelCollider.suspensionSpring = tmpRearSusp;
        }

        private void FrontSuspensionRestoration(int sprWeakness)
        {
            if (_forgeBlocked)
            {
                var tmpFrntSusp = _collFrontWheelCollider.suspensionSpring;
                tmpFrntSusp.spring = sprWeakness;
                _collFrontWheelCollider.suspensionSpring = tmpFrntSusp;
                _forgeBlocked = false;
            }

            if (_collFrontWheelCollider.suspensionSpring.spring < _normalFrontSuspSpring)
            {
                var tmpFrntSusp2 = _collFrontWheelCollider.suspensionSpring;
                tmpFrntSusp2.spring += 500.0f;
                _collFrontWheelCollider.suspensionSpring = tmpFrntSusp2;
            }
        }
    }
}