using UnityEngine;

namespace _Game.CodeBase.FactoryAndStorage.Resources
{
	public class ResourceUnit : MonoBehaviour
	{
		[SerializeField] private Resource _resource;

		public Resource Resource => _resource;
	}
}
