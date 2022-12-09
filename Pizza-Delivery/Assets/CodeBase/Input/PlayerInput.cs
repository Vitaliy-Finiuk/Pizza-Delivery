using CodeBase.Player;
using SimpleInputNamespace;
using UnityEngine;

namespace CodeBase.Input
{
	public class PlayerInput : MonoBehaviour {

		[SerializeField] public Joystick _joystick;
		public controlHub _controlHub;
		
		private void Update()
		{
			_controlHub.Horizontal = _joystick.xAxis.value;
			_controlHub.Vertical = _joystick.yAxis.value;
		}
	}
}

