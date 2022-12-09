using System.Collections.Generic;
using System.Linq;
using _Game.CodeBase.FactoryAndStorage.Resources;
using UnityEngine;

namespace _Game.CodeBase.FactoryAndStorage.Storage
{
	public class Acceptor : MonoBehaviour
	{
		[SerializeField] private Storage _acceptingStorage;

		public int FreeCellsCount => _acceptingStorage.FreeCellsCount;

		public bool HasFreeCells => _acceptingStorage.HasFreeCells;

		public bool CompatibleWith(Resource resource) => _acceptingStorage.CompatibleWith(resource);

		public IEnumerable<ResourceUnit> AcceptAndReturnExtra(IEnumerable<ResourceUnit> resourceUnits)
		{
			if (_acceptingStorage.HasFreeCells == false)
				return resourceUnits;

			var compatible = resourceUnits.Where(x => _acceptingStorage.CompatibleWith(x.Resource));

			if (compatible.Count() == 0)
				return resourceUnits;

			var extra = resourceUnits.Where(x => _acceptingStorage.CompatibleWith(x.Resource) == false).ToList();
			extra.AddRange(_acceptingStorage.PutInAndReturnExtra(compatible));

			return extra;
		}
	}
}
