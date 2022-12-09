using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryEventsNotificator : MonoBehaviour
{
	[SerializeField] private MessagesController _messagesController;

	public void OnStorageIsFullAndFactoryStopped(object sender, Factory factory)
	{
		string notification = $"{factory.Resource.Name} producing stopped: Output storage is full";
		_messagesController.DisplayMessage(notification);
	}

	public void OnFuelIsOverAndFactoryStopped(object sender)
	{
		string notification = $"{(sender as Factory).Resource.Name} producing stopped: Fuel is over";
		_messagesController.DisplayMessage(notification);
	}

	public void OnProducedResourceIsntPulledOutAndFactoryStopped(object sender)
	{
		string notification = $"{(sender as Factory).Resource.Name} producing stopped: Produced resource isnt pulled out";
		_messagesController.DisplayMessage(notification);
	}

	public void OnFactoryWorkAgain(object sender)
	{
		string notification = $"{(sender as Factory).Resource.Name} producing again";
		_messagesController.DisplayMessage(notification);
	}
}
