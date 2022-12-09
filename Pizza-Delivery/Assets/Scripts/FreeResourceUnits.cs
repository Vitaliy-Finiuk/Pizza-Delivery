using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeResourceUnits : MonoBehaviour
{
	private Dictionary<Resource, List<ResourceUnit>> _resourceResourceUnitPairs;

	[SerializeField] private int _maxUnitCountOfEveryResource;
	[SerializeField] private MyDictionary<Resource, ResourceUnit> _resourceUnitTemplatePairs;

	private void Awake() => Init();

	private void Init()
	{
		_resourceResourceUnitPairs = new Dictionary<Resource, List<ResourceUnit>>();
	}

	public ResourceUnit GetResourceUnit(Resource resource)
	{
		if (_resourceResourceUnitPairs.ContainsKey(resource) && _resourceResourceUnitPairs[resource].Count > 0)
		{
			var resourceUnit = _resourceResourceUnitPairs[resource][0];
			_resourceResourceUnitPairs[resource].RemoveAt(0);
			resourceUnit.transform.parent = null;
			resourceUnit.gameObject.SetActive(true);
			return resourceUnit;
		}
		else
		{
			return Instantiate(_resourceUnitTemplatePairs[resource], null);
		}
	}

	public void AddResourceUnit(ResourceUnit resourceUnit)
	{
		if (_resourceResourceUnitPairs.ContainsKey(resourceUnit.Resource) == false)
			_resourceResourceUnitPairs[resourceUnit.Resource] = new List<ResourceUnit>();

		if (_resourceResourceUnitPairs[resourceUnit.Resource].Count < _maxUnitCountOfEveryResource)
		{
			_resourceResourceUnitPairs[resourceUnit.Resource].Add(resourceUnit);
			resourceUnit.transform.parent = this.transform;
			resourceUnit.transform.localPosition = Vector3.zero;
			resourceUnit.transform.localScale = Vector3.zero;
			resourceUnit.gameObject.SetActive(false);
		}
		else
		{
			Destroy(resourceUnit.gameObject);
		}
	}
}
