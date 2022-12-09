﻿/*using CodeBase.Input;
using UnityEngine;

namespace CodeBase.Player
{
	public class CamSwitcher : MonoBehaviour
	{
		[SerializeField] private PlayerInput _playerInput;
		
		public Camera backCamera;
		public Camera aroundCamera;
		public Transform cameraTarget;
		private Camera currentCamera;


		[SerializeField] private float _dist = 3.0f;
		[SerializeField] private float _height = 1.0f;


		//////////////////// for around Camera
		private float xSpeed = 10.0f;
		private float ySpeed = 10.0f;

		[SerializeField] private float yMinLimit = -90;
		[SerializeField] private float yMaxLimit = 90;
		[SerializeField] private float _distance = 3.0f;

		[SerializeField] private float distanceMin = 2;
		[SerializeField] private float distanceMax = 10;

		private float x = 0.0f;
		private float y = 0.0f;

		private float smoothTime = 0.2f;

		private float xSmooth = 0.0f;
		private float ySmooth = 0.0f;
		private float xVelocity = 0.0f;
		private float yVelocity = 0.0f;


		//new camera behaviour
		private float currentTargetAngle;

		private GameObject ctrlHub;


		private void Start ()
		{
			backCamera.enabled = true;
			aroundCamera.enabled = false;
			currentCamera = backCamera;
		
			if (GetComponent<Rigidbody> ()) GetComponent<Rigidbody> ().freezeRotation = true;
	
			currentTargetAngle = cameraTarget.transform.eulerAngles.z;
		}
	
		// Update is called once per frame
		void LateUpdate ()
		{
#if UNITY_STANDALONE || UNITY_WEBPLAYER// turn camera rotaion ONLY for mobile for free touch screen anywhere
			if (UnityEngine.Input.GetMouseButton (1)) {

				backCamera.enabled = false;
				aroundCamera.enabled = true;
				backCamera.gameObject.SetActive (false);
				aroundCamera.gameObject.SetActive (true);
				currentCamera = aroundCamera;
			
			
				x += UnityEngine.Input.GetAxis ("Mouse X") * xSpeed;
				y -= UnityEngine.Input.GetAxis ("Mouse Y") * ySpeed;
			
				y = Mathf.Clamp (y, yMinLimit, yMaxLimit);
			
			
			
				xSmooth = Mathf.SmoothDamp (xSmooth, x, ref xVelocity, smoothTime);
				ySmooth = Mathf.SmoothDamp (ySmooth, y, ref yVelocity, smoothTime);
			
			
				_distance = Mathf.Clamp (_distance + UnityEngine.Input.GetAxis ("Mouse ScrollWheel") * _distance, distanceMin, distanceMax);
			
			
				currentCamera.transform.localRotation = Quaternion.Euler (ySmooth, xSmooth, 0);
				currentCamera.transform.position = currentCamera.transform.rotation * new Vector3 (0.0f, 0.0f, -_distance) + cameraTarget.position;


			} else {
#endif
				backCamera.enabled = true;
				aroundCamera.enabled = false;
				backCamera.gameObject.SetActive (true);
				aroundCamera.gameObject.SetActive (false);
				currentCamera = backCamera;
			
				//////////////////// code for back Camera
				backCamera.fieldOfView = backCamera.fieldOfView + _playerInput.Vertical * 20f * Time.deltaTime;
				if (backCamera.fieldOfView > 85) {
					backCamera.fieldOfView = 85;
				}
				if (backCamera.fieldOfView < 50) {
					backCamera.fieldOfView = 50;
				}
				if (backCamera.fieldOfView < 60) {
					backCamera.fieldOfView = backCamera.fieldOfView += 10f * Time.deltaTime;
				}
				if (backCamera.fieldOfView > 60) {
					backCamera.fieldOfView = backCamera.fieldOfView -= 10f * Time.deltaTime;
				}
			
				float wantedRotationAngle = cameraTarget.eulerAngles.y;
				float wantedHeight = cameraTarget.position.y + _height;
				float currentRotationAngle = currentCamera.transform.eulerAngles.y;
				float currentHeight = currentCamera.transform.position.y;
			
				currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, 3 * Time.deltaTime);
				currentHeight = Mathf.Lerp (currentHeight, wantedHeight, 2 * Time.deltaTime);
			
				Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
				currentCamera.transform.position = cameraTarget.position;
				currentCamera.transform.position -= currentRotation * Vector3.forward * _dist;
				currentCamera.transform.position = new Vector3 (currentCamera.transform.position.x, currentHeight, currentCamera.transform.position.z);
				currentCamera.transform.LookAt (cameraTarget);

				//New camera features.
				//Now camera leaning with biker, so horizon is not always horizontal :)
				//If you don't like it, just disable
				//from this -----------------------------------------------------------------------

				// rotate camera according with bike leaning
				if (cameraTarget.transform.eulerAngles.z >0 && cameraTarget.transform.eulerAngles.z < 180) {
					currentTargetAngle = cameraTarget.transform.eulerAngles.z/10;
				}
				if (cameraTarget.transform.eulerAngles.z >180){
					currentTargetAngle = -(360-cameraTarget.transform.eulerAngles.z)/10;
				}
				currentCamera.transform.rotation = Quaternion.Euler (_height*10, currentRotationAngle, currentTargetAngle);
				//to this -------------------------------------------------------------------------
#if UNITY_STANDALONE || UNITY_WEBPLAYER// turn camera rotaion ONLY for mobile for free touch screen anywhere
			}
#endif
		}
	}
}*/