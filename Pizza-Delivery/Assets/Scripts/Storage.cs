using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Storage : MonoBehaviour
{
	private List<ResourceUnit> _resourceUnits;
	private static UnityException _incompatibleResourcesTypesException;
	private static UnityException _cantAddResourceStorageIsFullException;

	[SerializeField] private int _cellsCount;
	[SerializeField] private Resource[] _compatibleResources;

	[Header("Events")]
	[SerializeField] private UnityEvent<object, ResourceUnit> _putInEvent;
	[SerializeField] private UnityEvent<object, ResourceUnit> _pullOutEvent;
	[SerializeField] private UnityEvent<object> _storageIsFullEvent;

	public UnityEvent<object, ResourceUnit> PutInEvent => _putInEvent;

	public UnityEvent<object, ResourceUnit> PullOutEvent => _pullOutEvent;

	public UnityEvent<object> StorageIsFullEvent => _storageIsFullEvent;

	public int CellsCount => _cellsCount;

	public int FreeCellsCount => _cellsCount - _resourceUnits.Count;

	public bool HasFreeCells => FreeCellsCount > 0;

	public int ResourcesCount => _resourceUnits.Count;

	static Storage()
	{
		_incompatibleResourcesTypesException = new UnityException("Incompatible resources types");
		_cantAddResourceStorageIsFullException = new UnityException("Cant add resource, storage iss full");
	}

	private void Awake() => Init();

	private void Init()
	{
		_resourceUnits = new List<ResourceUnit>();
	}

	public bool CompatibleWith(Resource resource) => _compatibleResources.Contains(resource);

	public IEnumerable<ResourceUnit> PutInAndReturnExtra(ResourceUnit resourceUnit)
	{
		return PutInAndReturnExtra(new[] { resourceUnit });
	}

	public IEnumerable<ResourceUnit> PutInAndReturnExtra(IEnumerable<ResourceUnit> resourceUnits)
	{
		List<ResourceUnit> extra = new List<ResourceUnit>();

		foreach (var resource in resourceUnits)
		{
			if (HasFreeCells)
			{
				AddResourceUnit(resource);
			}
			else
			{
				extra.Add(resource);
			}
		}

		return extra;
	}

	public IEnumerable<ResourceUnit> PullOutAll()
	{
		var resourceUnits = new List<ResourceUnit>();

		for (int i = _resourceUnits.Count - 1; i >= 0; i--)
			resourceUnits.Add(PullOutResourceUnit(i));

		return resourceUnits;
	}

	public IEnumerable<ResourceUnit> PullOutAll(System.Func<ResourceUnit, bool> predicate)
	{
		var resourceUnits = new List<ResourceUnit>();

		for (int i = _resourceUnits.Count - 1; i >= 0; i--)
			if (predicate.Invoke(_resourceUnits[i]))
				resourceUnits.Add(PullOutResourceUnit(i));

		return resourceUnits;
	}

	public IEnumerable<ResourceUnit> PullOut(Resource resource, int desiredCount)
	{
		List<ResourceUnit> resources = new List<ResourceUnit>();
		int pulledOut = 0;

		for (int i = _resourceUnits.Count - 1; i >= 0; i--)
		{
			if (pulledOut >= desiredCount)
				break;

			if (_resourceUnits[i].Resource == resource)
			{
				resources.Add(PullOutResourceUnit(i));
				pulledOut++;
			}
		}

		return resources;
	}

	private void AddResourceUnit(ResourceUnit resourceUnit)
	{
		if (!HasFreeCells)
			throw _cantAddResourceStorageIsFullException;

		if (resourceUnit == null)
			throw new System.NullReferenceException("Resource unit cant be added to storage as null");
		
		if (CompatibleWith(resourceUnit.Resource) == false)
			throw _incompatibleResourcesTypesException;

		_resourceUnits.Add(resourceUnit);

		try
		{
			_putInEvent.Invoke(this, resourceUnit);
		}
		catch { }

		if (HasFreeCells == false)
			_storageIsFullEvent.Invoke(this);
	}

	private ResourceUnit PullOutResourceUnit(int index)
	{
		ResourceUnit resourceUnit = _resourceUnits[index];

		_resourceUnits.RemoveAt(index);
		_pullOutEvent.Invoke(this, resourceUnit);

		return resourceUnit;
	}
}
