using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Moon pioneer/New Resource", fileName = "Resource")]
public class Resource: ScriptableObject
{
	[SerializeField] private string _name;

	public string Name => _name;
}
