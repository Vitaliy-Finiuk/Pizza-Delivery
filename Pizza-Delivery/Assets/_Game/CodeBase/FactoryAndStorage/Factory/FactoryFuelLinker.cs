using System.Linq;
using _Game.CodeBase.ResourcesLogic;
using UnityEngine;

namespace _Game.CodeBase.Factory
{
	public class FactoryFuelLinker : MonoBehaviour
	{
		[SerializeField] private Factory _factory;
		[SerializeField] private Storage.Storage[] _fuelStorages;

		private void OnEnable()
		{
			foreach (var storage in _fuelStorages)
			{
				storage.PutInEvent.AddListener(OnSomethingAddedToFuelStorage);
				_factory.FuelDestroyedEvent.AddListener(OnFuelDestroyed);
			}
		}
	
		private void OnDisable()
		{
			foreach (var storage in _fuelStorages)
			{
				storage.PutInEvent.RemoveListener(OnSomethingAddedToFuelStorage);
			}
		}

		private void OnSomethingAddedToFuelStorage(object sender, ResourceUnit resourceUnit)
		{
			if (_factory.FuelAndHasFuelPairs[resourceUnit.Resource] == false)
			{
				ResourceUnit fuel = (sender as Storage.Storage).PullOut(resourceUnit.Resource, 1).FirstOrDefault();

				if (fuel != null)
				{
					ResourceUnit _ = null;
					_factory.TryDownloadFuel(fuel, ref _);
				}
			}
		}

		private void OnFuelDestroyed(object sender)
		{
			foreach (var resource in _factory.FuelAndHasFuelPairs.Keys.ToArray())
			{
				if (_factory.FuelAndHasFuelPairs[resource] == false)
				{
					foreach (var storage in _fuelStorages)
					{
						var resourceUnit = storage.PullOut(resource, 1).FirstOrDefault();

						if (resourceUnit != null)
						{
							ResourceUnit _ = null;

							if (_factory.TryDownloadFuel(resourceUnit, ref _))
								break;
						}
					}
				}
			}
		}
	}
}
