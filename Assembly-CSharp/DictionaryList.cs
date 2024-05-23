using System;
using System.Collections.Generic;

// Token: 0x02000EDD RID: 3805
[Serializable]
public class DictionaryList<T, U> : List<DictionaryListItem<T, U>>
{
	// Token: 0x06007209 RID: 29193 RVA: 0x002188AC File Offset: 0x00216AAC
	public bool TryGetValue(T key, out U value)
	{
		foreach (DictionaryListItem<T, U> dictionaryListItem in this)
		{
			if (dictionaryListItem.m_key.Equals(key))
			{
				value = dictionaryListItem.m_value;
				return true;
			}
		}
		value = default(U);
		return false;
	}

	// Token: 0x170009E0 RID: 2528
	public U this[T key]
	{
		get
		{
			U result;
			if (!this.TryGetValue(key, out result))
			{
				throw new KeyNotFoundException(string.Format("{0} key does not exist in ListDict.", key));
			}
			return result;
		}
		set
		{
			foreach (DictionaryListItem<T, U> dictionaryListItem in this)
			{
				if (dictionaryListItem.m_key.Equals(key))
				{
					dictionaryListItem.m_value = value;
					return;
				}
			}
			this.Add(new DictionaryListItem<T, U>
			{
				m_key = key,
				m_value = value
			});
		}
	}
}
