using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceUnit : MonoBehaviour
{
	[SerializeField] private Resource _resource;

	public Resource Resource => _resource;
}
