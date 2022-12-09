using SimpleInputNamespace;
using UnityEngine;

namespace _Game.CodeBase.Input
{
	public class PlayerInput : MonoBehaviour
	{
		[SerializeField] public Joystick _joystick;

		public float Horizontal;
		public float Vertical;
		
		public bool isRestartBike;
		private void Update()
		{
			Horizontal = _joystick.xAxis.value;
			Vertical = _joystick.yAxis.value;
		}

		public void RestartBikeDownEvent()
		{
			isRestartBike = true;
		}
		
		public void RestartBikeDownUp()
		{
			isRestartBike = false;
		}
	}
}

