using _Game.CodeBase.ResourcesLogic;
using UnityEngine;
using UnityEngine.Events;

namespace _Game.CodeBase.Factory
{
	public class FactoryWithOutputStorageLinker : MonoBehaviour
	{
		[SerializeField] private Factory _factory;
		[SerializeField] private Storage.Storage _storage;

		[Header("Events")]
		[SerializeField] private UnityEvent<object, Factory> _turnOffFactoryBecauseOfFullStorageEvent;

		public UnityEvent<object, Factory> TurnOffFactoryBecauseOffFullStorageEvent => _turnOffFactoryBecauseOfFullStorageEvent;

		private void OnEnable()
		{
			_storage.StorageIsFullEvent.AddListener(OnStorageBecomeFull);
			_storage.PullOutEvent.AddListener(OnStorageHadFreeCells);
			_factory.ProducedEvent.AddListener(OnFactoryProduce);
		}

		private void OnDisable()
		{
			_storage.StorageIsFullEvent.RemoveListener(OnStorageBecomeFull);
			_storage.PullOutEvent.RemoveListener(OnStorageHadFreeCells);
			_factory.ProducedEvent.RemoveListener(OnFactoryProduce);
		}

		private void OnStorageBecomeFull(object sender)
		{
			if (_factory.TurnedOn)
			{
				_factory.TurnOff();
				_turnOffFactoryBecauseOfFullStorageEvent.Invoke(this, _factory);
			}
		}

		private void OnStorageHadFreeCells(object sender, ResourceUnit resourceUnit)
		{
			if (_factory.TurnedOn == false)
			{
				_factory.TurnOn();
			}
		}

		private void OnFactoryProduce(object sender)
		{
			ResourceUnit resourceUnit = null;
			_factory.TryPullOutProducedResourceUnit(ref resourceUnit);
			_storage.PutInAndReturnExtra(resourceUnit);
		}
	}
}
