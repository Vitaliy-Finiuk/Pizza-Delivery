using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Factory : MonoBehaviour
{
	private Dictionary<Resource, bool> _fuelAndHasFuelPairs;
	private float _lastResourceProducingTime;
	private ResourceUnit _producedResourceUnit;

	[SerializeField] private Resource _resource;
	[SerializeField] private float _secondsForProductResourceUnit;
	[SerializeField] private Storage _storageForFuelLoadedToFactory;
	[SerializeField] private FreeResourceUnits _freeResourceUnits;
	[SerializeField] private Resource[] _fuelTypes;

	[Header("Events")]
	[SerializeField] private UnityEvent<object> _turnedOffEvent;
	[SerializeField] private UnityEvent<object> _turnedOnEvent;
	[SerializeField] private UnityEvent<object> _fuelIsOverEvent;
	[SerializeField] private UnityEvent<object> _workAgainEvent;
	[SerializeField] private UnityEvent<object> _producedEvent;
	[SerializeField] private UnityEvent<object> _producedResourceUnitNotPulledOutEvent;
	[SerializeField] private UnityEvent<object> _fuelDestroyedEvent;

	public UnityEvent<object> TurnedOffEvent => _turnedOffEvent;

	public UnityEvent<object> TurnedOnEvent => _turnedOnEvent;

	public UnityEvent<object> FuelIsOverEvent => _fuelIsOverEvent;

	public UnityEvent<object> WorkAgainEvent => _workAgainEvent;

	public UnityEvent<object> ProducedEvent => _producedEvent;

	public UnityEvent<object> ProducedResourceUnitNotPulledOutEvent => _producedResourceUnitNotPulledOutEvent;

	public UnityEvent<object> FuelDestroyedEvent => _fuelDestroyedEvent;

	public Dictionary<Resource, bool> FuelAndHasFuelPairs => _fuelAndHasFuelPairs;

	public Resource Resource => _resource;

	public bool TurnedOn { get; private set; }

	public bool Producing { get; private set; }

	public float Progress => Mathf.Clamp01(_lastResourceProducingTime / _secondsForProductResourceUnit);

	public bool HasFuel
	{
		get
		{
			bool result = true;

			foreach (var hasFuel in _fuelAndHasFuelPairs.Values)
				result &= hasFuel;

			return result;
		}
	}

	private void Awake() => Init();

	private void Init()
	{
		TurnOn();

		_fuelAndHasFuelPairs = new Dictionary<Resource, bool>();

		foreach (var fuel in _fuelTypes)
			_fuelAndHasFuelPairs[fuel] = false;
	}

	public void TurnOn()
	{
		if (TurnedOn)
			return;

		TurnedOn = true;
		_turnedOnEvent.Invoke(this);
	}

	public void TurnOff()
	{
		if (TurnedOn == false)
			return;

		TurnedOn = false;
		_turnedOffEvent.Invoke(this);
	}

	public bool TryDownloadFuel(ResourceUnit fuelUnit, ref ResourceUnit returnedResourceUnit)
	{
		if (_fuelAndHasFuelPairs[fuelUnit.Resource] == true)
		{
			returnedResourceUnit = fuelUnit;
			return false;
		}

		_storageForFuelLoadedToFactory.PutInAndReturnExtra(fuelUnit);
		_fuelAndHasFuelPairs[fuelUnit.Resource] = true;

		return true;
	}

	public bool TryPullOutProducedResourceUnit(ref ResourceUnit resourceUnit)
	{
		if (_producedResourceUnit == null)
			return false;

		resourceUnit = _producedResourceUnit;
		_producedResourceUnit = null;

		return true;
	}

	private void DestroyFuel()
	{
		var usedFuel = _storageForFuelLoadedToFactory.PullOutAll();

		foreach (var fuel in usedFuel)
		{
			_freeResourceUnits.AddResourceUnit(fuel);
			_fuelAndHasFuelPairs[fuel.Resource] = false;
		}

		_fuelDestroyedEvent.Invoke(this);
	}

	private void Produce()
	{
		_producedResourceUnit = _freeResourceUnits.GetResourceUnit(_resource);
		_producedResourceUnit.transform.parent = this.transform;
		_producedResourceUnit.transform.position = this.transform.position;
		_producedResourceUnit.transform.localScale = Vector3.zero;
		_producedEvent.Invoke(this);
	}

	private void Update()
	{
		if (TurnedOn == false)
		{
			Producing = false;
		}
		else if (HasFuel == false)
		{
			if (Producing)
				_fuelIsOverEvent.Invoke(this);

			Producing = false;
		}
		else if (_producedResourceUnit != null)
		{
			if (Producing)
				_producedResourceUnitNotPulledOutEvent.Invoke(this);

			Producing = false;
		}
		else
		{
			if (Producing == false)
				_workAgainEvent.Invoke(this);

			Producing = true;
		}

		if (Producing)
		{
			_lastResourceProducingTime += Time.deltaTime;

			if (Progress == 1)
			{
				Produce();
				DestroyFuel();
				_lastResourceProducingTime = 0;
			}
		}
	}
}
