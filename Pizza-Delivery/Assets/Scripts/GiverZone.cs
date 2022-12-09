using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GiverZone : MonoBehaviour
{
	private List<Acceptor> _acceptors;

	[SerializeField] private Storage _storage;

	private void OnTriggerEnter(Collider other)
	{
		Acceptor acceptor = other.GetComponent<Acceptor>();

		if (acceptor != null)
			_acceptors.Add(acceptor);
	}

	private void OnTriggerExit(Collider other)
	{
		Acceptor acceptor = other.GetComponent<Acceptor>();

		if (acceptor != null)
			_acceptors.Remove(acceptor);
	}

	private void Awake() => Init();

	private void Init()
	{
		_acceptors = new List<Acceptor>();
	}

	private void Update()
	{
		foreach (var acceptor in _acceptors)
			Give(acceptor);
	}

	private void Give(Acceptor acceptor)
	{
		if (acceptor.HasFreeCells == false)
			return;

		var acceptable = _storage.PullOutAll(x => acceptor.CompatibleWith(x.Resource));
		var extra = acceptor.AcceptAndReturnExtra(acceptable);
		_storage.PutInAndReturnExtra(extra);
	}
}
