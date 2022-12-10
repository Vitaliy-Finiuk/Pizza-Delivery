using _Game.CodeBase.Infrastucture;
using _Game.CodeBase.Services.Input;
using UnityEngine;

namespace _Game.CodeBase.Player.CameraLogic
{
	public class CameraFollower : MonoBehaviour
	{
		private IInputService _inputService;
		
		[SerializeField] private Camera _backCamera;
		[SerializeField] private Camera _aroundCamera;
		[SerializeField] private Transform _cameraTarget;
		private Camera _currentCamera;

		[SerializeField] private float _dist = 3.0f;
		[SerializeField] private float _height = 1.0f;

		private float _xSpeed = 10.0f;
		private float _ySpeed = 10.0f;

		private float _currentTargetAngle;

		private void Awake()
		{
			_inputService = Game.InputService;
		}

		private void Start ()
		{
			_backCamera.enabled = true;
			_aroundCamera.enabled = false;
			_currentCamera = _backCamera;
		
			if (GetComponent<Rigidbody> ()) GetComponent<Rigidbody> ().freezeRotation = true;
	
			_currentTargetAngle = _cameraTarget.transform.eulerAngles.z;
		}

		private void LateUpdate ()
		{
			_backCamera.enabled = true;
				_aroundCamera.enabled = false;
				_backCamera.gameObject.SetActive (true);
				_aroundCamera.gameObject.SetActive (false);
				_currentCamera = _backCamera;
			
				//////////////////// code for back Camera
				_backCamera.fieldOfView = _backCamera.fieldOfView + _inputService.Axis.y * 20f * Time.deltaTime;
				if (_backCamera.fieldOfView > 85) {
					_backCamera.fieldOfView = 85;
				}
				if (_backCamera.fieldOfView < 80) {
					_backCamera.fieldOfView = 80;
				}
				if (_backCamera.fieldOfView < 80) {
					_backCamera.fieldOfView = _backCamera.fieldOfView += 10f * Time.deltaTime;
				}
				if (_backCamera.fieldOfView > 80) {
					_backCamera.fieldOfView = _backCamera.fieldOfView -= 10f * Time.deltaTime;
				}
			
				float wantedRotationAngle = _cameraTarget.eulerAngles.y;
				float wantedHeight = _cameraTarget.position.y + _height;
				float currentRotationAngle = _currentCamera.transform.eulerAngles.y;
				float currentHeight = _currentCamera.transform.position.y;
			
				currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, 3 * Time.deltaTime);
				currentHeight = Mathf.Lerp (currentHeight, wantedHeight, 2 * Time.deltaTime);
			
				Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
				_currentCamera.transform.position = _cameraTarget.position;
				_currentCamera.transform.position -= currentRotation * Vector3.forward * _dist;
				_currentCamera.transform.position = new Vector3 (_currentCamera.transform.position.x, currentHeight, _currentCamera.transform.position.z);
				_currentCamera.transform.LookAt (_cameraTarget);
				
				
				if (_cameraTarget.transform.eulerAngles.z >0 && _cameraTarget.transform.eulerAngles.z < 180) {
					_currentTargetAngle = _cameraTarget.transform.eulerAngles.z/10;
				}
				if (_cameraTarget.transform.eulerAngles.z >180){
					_currentTargetAngle = -(360-_cameraTarget.transform.eulerAngles.z)/10;
				}
				_currentCamera.transform.rotation = Quaternion.Euler (_height*10, currentRotationAngle, _currentTargetAngle);
			
		}
	}
}