using UnityEngine;

namespace _Game.CodeBase.ResourcesLogic
{
	public class ResourceUnit : MonoBehaviour
	{
		[SerializeField] private Resource _resource;

		public Resource Resource => _resource;
	}
}
