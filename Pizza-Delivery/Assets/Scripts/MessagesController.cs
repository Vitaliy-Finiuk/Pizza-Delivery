using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagesController : MonoBehaviour
{
	private List<Message> _messages;
	private List<float> _timeMessagesDisplaying;

	[SerializeField] private Message _messageTemplate;
	[SerializeField] private float _timeMessageDisplaying;

	private void Awake() => Init();

	private void Init()
	{
		_messages = new List<Message>();
		_timeMessagesDisplaying = new List<float>();
	}

	public void DisplayMessage(string text)
	{
		Message message = Instantiate(_messageTemplate, this.transform);
		message.gameObject.SetActive(true);
		message.SetText(text);

		_messages.Add(message);
		_timeMessagesDisplaying.Add(0);
	}

	private void Update()
	{
		for (int i = _timeMessagesDisplaying.Count - 1; i >= 0; i--)
		{
			if (_timeMessagesDisplaying[i] >= _timeMessageDisplaying)
			{
				Destroy(_messages[i].gameObject);
				_timeMessagesDisplaying.RemoveAt(i);
				_messages.RemoveAt(i);
			}
			else
			{
				_timeMessagesDisplaying[i] += Time.deltaTime;
			}
		}
	}
}
