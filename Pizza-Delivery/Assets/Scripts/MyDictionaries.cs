using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MyKeyValuePair<TKey, TValue>
{
	public TKey key;
	public TValue value;

	public MyKeyValuePair(TKey key, TValue value)
	{
		this.key = key;
		this.value = value;
	}
}

[Serializable]
public class MyDictionary<TKey, TValue>
{
	public List<MyKeyValuePair<TKey, TValue>> pairs;

	public TValue this[TKey key]
	{
		get => pairs.First(x => x.key.Equals(key)).value;
		set
		{
			if (ContainsKey(key))
				GetPairByKey(key).value = value;
			else
				pairs.Add(new MyKeyValuePair<TKey, TValue>(key, value));
		}
	}

	public void Remove(TKey key) => pairs.Remove(pairs.First(pair => pair.key.Equals(key)));

	public bool ContainsKey(TKey key) => pairs.Any(pair => pair.key.Equals(key));

	public MyKeyValuePair<TKey, TValue> GetPairByKey(TKey key) => pairs.First(pair => pair.key.Equals(key));
}
