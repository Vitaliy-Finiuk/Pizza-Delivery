using UnityEngine;

namespace _Game.CodeBase.ResourcesLogic
{
	[CreateAssetMenu(menuName = "Scriptable Objects/Moon pioneer/New Resource", fileName = "Resource")]
	public class Resource: ScriptableObject
	{
		[SerializeField] private string _name;

		public string Name => _name;
	}
}
