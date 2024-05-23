using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x0200000A RID: 10
public class Map<TKey, TValue> : IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>>
{
	// Token: 0x06000053 RID: 83 RVA: 0x00003B2A File Offset: 0x00001D2A
	public Map()
	{
		this.Init(4, null);
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00003B3A File Offset: 0x00001D3A
	public Map(int count)
	{
		this.Init(count, null);
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00003B4A File Offset: 0x00001D4A
	public Map(IEqualityComparer<TKey> comparer)
	{
		this.Init(4, comparer);
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00003B5C File Offset: 0x00001D5C
	public Map(IEnumerable<KeyValuePair<TKey, TValue>> copy)
	{
		this.Init(4, null);
		foreach (KeyValuePair<TKey, TValue> keyValuePair in copy)
		{
			this[keyValuePair.Key] = keyValuePair.Value;
		}
	}

	// Token: 0x06000057 RID: 87 RVA: 0x00003BCC File Offset: 0x00001DCC
	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Map<TKey, TValue>.Enumerator(this);
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00003BD9 File Offset: 0x00001DD9
	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<!0, !1>>.GetEnumerator()
	{
		return new Map<TKey, TValue>.Enumerator(this);
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000059 RID: 89 RVA: 0x00003BE6 File Offset: 0x00001DE6
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	// Token: 0x1700000B RID: 11
	public TValue this[TKey key]
	{
		get
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = this.hcp.GetHashCode(key) | int.MinValue;
			for (int num2 = this.table[(num & int.MaxValue) % this.table.Length] - 1; num2 != -1; num2 = this.linkSlots[num2].Next)
			{
				if (this.linkSlots[num2].HashCode == num && this.hcp.Equals(this.keySlots[num2], key))
				{
					return this.valueSlots[num2];
				}
			}
			throw new KeyNotFoundException();
		}
		set
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = this.hcp.GetHashCode(key) | int.MinValue;
			int num2 = (num & int.MaxValue) % this.table.Length;
			int num3 = this.table[num2] - 1;
			int num4 = -1;
			if (num3 != -1)
			{
				while (this.linkSlots[num3].HashCode != num || !this.hcp.Equals(this.keySlots[num3], key))
				{
					num4 = num3;
					num3 = this.linkSlots[num3].Next;
					if (num3 == -1)
					{
						break;
					}
				}
			}
			if (num3 == -1)
			{
				if (++this.count > this.threshold)
				{
					this.Resize();
					num2 = (num & int.MaxValue) % this.table.Length;
				}
				num3 = this.emptySlot;
				if (num3 == -1)
				{
					num3 = this.touchedSlots++;
				}
				else
				{
					this.emptySlot = this.linkSlots[num3].Next;
				}
				this.linkSlots[num3].Next = this.table[num2] - 1;
				this.table[num2] = num3 + 1;
				this.linkSlots[num3].HashCode = num;
				this.keySlots[num3] = key;
			}
			else if (num4 != -1)
			{
				this.linkSlots[num4].Next = this.linkSlots[num3].Next;
				this.linkSlots[num3].Next = this.table[num2] - 1;
				this.table[num2] = num3 + 1;
			}
			this.valueSlots[num3] = value;
			this.generation++;
		}
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00003E81 File Offset: 0x00002081
	private void Init(int capacity, IEqualityComparer<TKey> hcp)
	{
		this.hcp = (hcp ?? EqualityComparer<TKey>.Default);
		capacity = Math.Max(1, (int)((float)capacity / 0.9f));
		this.InitArrays(capacity);
	}

	// Token: 0x0600005D RID: 93 RVA: 0x00003EB0 File Offset: 0x000020B0
	private void InitArrays(int size)
	{
		this.table = new int[size];
		this.linkSlots = new Link[size];
		this.emptySlot = -1;
		this.keySlots = new TKey[size];
		this.valueSlots = new TValue[size];
		this.touchedSlots = 0;
		this.threshold = (int)((float)this.table.Length * 0.9f);
		if (this.threshold == 0 && this.table.Length > 0)
		{
			this.threshold = 1;
		}
	}

	// Token: 0x0600005E RID: 94 RVA: 0x00003F34 File Offset: 0x00002134
	private void CopyToCheck(Array array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (index > array.Length)
		{
			throw new ArgumentException("index larger than largest valid index of array");
		}
		if (array.Length - index < this.Count)
		{
			throw new ArgumentException("Destination array cannot hold the requested elements!");
		}
	}

	// Token: 0x0600005F RID: 95 RVA: 0x00003F9C File Offset: 0x0000219C
	private void CopyKeys(TKey[] array, int index)
	{
		for (int i = 0; i < this.touchedSlots; i++)
		{
			if ((this.linkSlots[i].HashCode & -2147483648) != 0)
			{
				array[index++] = this.keySlots[i];
			}
		}
	}

	// Token: 0x06000060 RID: 96 RVA: 0x00003FF4 File Offset: 0x000021F4
	private void CopyValues(TValue[] array, int index)
	{
		for (int i = 0; i < this.touchedSlots; i++)
		{
			if ((this.linkSlots[i].HashCode & -2147483648) != 0)
			{
				array[index++] = this.valueSlots[i];
			}
		}
	}

	// Token: 0x06000061 RID: 97 RVA: 0x0000404C File Offset: 0x0000224C
	private static KeyValuePair<TKey, TValue> make_pair(TKey key, TValue value)
	{
		return new KeyValuePair<TKey, TValue>(key, value);
	}

	// Token: 0x06000062 RID: 98 RVA: 0x00004055 File Offset: 0x00002255
	private static TKey pick_key(TKey key, TValue value)
	{
		return key;
	}

	// Token: 0x06000063 RID: 99 RVA: 0x00004058 File Offset: 0x00002258
	private static TValue pick_value(TKey key, TValue value)
	{
		return value;
	}

	// Token: 0x06000064 RID: 100 RVA: 0x0000405C File Offset: 0x0000225C
	private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
	{
		this.CopyToCheck(array, index);
		for (int i = 0; i < this.touchedSlots; i++)
		{
			if ((this.linkSlots[i].HashCode & -2147483648) != 0)
			{
				array[index++] = new KeyValuePair<TKey, TValue>(this.keySlots[i], this.valueSlots[i]);
			}
		}
	}

	// Token: 0x06000065 RID: 101 RVA: 0x000040D4 File Offset: 0x000022D4
	private void Do_ICollectionCopyTo<TRet>(Array array, int index, Map<TKey, TValue>.Transform<TRet> transform)
	{
		Type typeFromHandle = typeof(TRet);
		Type elementType = array.GetType().GetElementType();
		try
		{
			if ((typeFromHandle.IsPrimitive || elementType.IsPrimitive) && !elementType.IsAssignableFrom(typeFromHandle))
			{
				throw new Exception();
			}
			object[] array2 = (object[])array;
			for (int i = 0; i < this.touchedSlots; i++)
			{
				if ((this.linkSlots[i].HashCode & -2147483648) != 0)
				{
					array2[index++] = transform(this.keySlots[i], this.valueSlots[i]);
				}
			}
		}
		catch (Exception ex)
		{
			throw new ArgumentException("Cannot copy source collection elements to destination array", "array", ex);
		}
	}

	// Token: 0x06000066 RID: 102 RVA: 0x000041B4 File Offset: 0x000023B4
	private void Resize()
	{
		int num = HashPrimeNumbers.ToPrime(this.table.Length << 1 | 1);
		int[] array = new int[num];
		Link[] array2 = new Link[num];
		for (int i = 0; i < this.table.Length; i++)
		{
			for (int num2 = this.table[i] - 1; num2 != -1; num2 = this.linkSlots[num2].Next)
			{
				int num3 = array2[num2].HashCode = (this.hcp.GetHashCode(this.keySlots[num2]) | int.MinValue);
				int num4 = (num3 & int.MaxValue) % num;
				array2[num2].Next = array[num4] - 1;
				array[num4] = num2 + 1;
			}
		}
		this.table = array;
		this.linkSlots = array2;
		TKey[] array3 = new TKey[num];
		TValue[] array4 = new TValue[num];
		Array.Copy(this.keySlots, 0, array3, 0, this.touchedSlots);
		Array.Copy(this.valueSlots, 0, array4, 0, this.touchedSlots);
		this.keySlots = array3;
		this.valueSlots = array4;
		this.threshold = (int)((float)num * 0.9f);
	}

	// Token: 0x06000067 RID: 103 RVA: 0x000042E8 File Offset: 0x000024E8
	public void Add(TKey key, TValue value)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		int num = this.hcp.GetHashCode(key) | int.MinValue;
		int num2 = (num & int.MaxValue) % this.table.Length;
		int num3;
		for (num3 = this.table[num2] - 1; num3 != -1; num3 = this.linkSlots[num3].Next)
		{
			if (this.linkSlots[num3].HashCode == num && this.hcp.Equals(this.keySlots[num3], key))
			{
				throw new ArgumentException("An element with the same key already exists in the dictionary.");
			}
		}
		if (++this.count > this.threshold)
		{
			this.Resize();
			num2 = (num & int.MaxValue) % this.table.Length;
		}
		num3 = this.emptySlot;
		if (num3 == -1)
		{
			num3 = this.touchedSlots++;
		}
		else
		{
			this.emptySlot = this.linkSlots[num3].Next;
		}
		this.linkSlots[num3].HashCode = num;
		this.linkSlots[num3].Next = this.table[num2] - 1;
		this.table[num2] = num3 + 1;
		this.keySlots[num3] = key;
		this.valueSlots[num3] = value;
		this.generation++;
	}

	// Token: 0x06000068 RID: 104 RVA: 0x00004468 File Offset: 0x00002668
	public void Clear()
	{
		if (this.count == 0)
		{
			return;
		}
		this.count = 0;
		Array.Clear(this.table, 0, this.table.Length);
		Array.Clear(this.keySlots, 0, this.keySlots.Length);
		Array.Clear(this.valueSlots, 0, this.valueSlots.Length);
		Array.Clear(this.linkSlots, 0, this.linkSlots.Length);
		this.emptySlot = -1;
		this.touchedSlots = 0;
		this.generation++;
	}

	// Token: 0x06000069 RID: 105 RVA: 0x000044F4 File Offset: 0x000026F4
	public bool ContainsKey(TKey key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		int num = this.hcp.GetHashCode(key) | int.MinValue;
		for (int num2 = this.table[(num & int.MaxValue) % this.table.Length] - 1; num2 != -1; num2 = this.linkSlots[num2].Next)
		{
			if (this.linkSlots[num2].HashCode == num && this.hcp.Equals(this.keySlots[num2], key))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600006A RID: 106 RVA: 0x0000459C File Offset: 0x0000279C
	public bool ContainsValue(TValue value)
	{
		IEqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
		for (int i = 0; i < this.table.Length; i++)
		{
			for (int num = this.table[i] - 1; num != -1; num = this.linkSlots[num].Next)
			{
				if (@default.Equals(this.valueSlots[num], value))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600006B RID: 107 RVA: 0x0000460C File Offset: 0x0000280C
	public bool Remove(TKey key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		int num = this.hcp.GetHashCode(key) | int.MinValue;
		int num2 = (num & int.MaxValue) % this.table.Length;
		int num3 = this.table[num2] - 1;
		if (num3 == -1)
		{
			return false;
		}
		int num4 = -1;
		while (this.linkSlots[num3].HashCode != num || !this.hcp.Equals(this.keySlots[num3], key))
		{
			num4 = num3;
			num3 = this.linkSlots[num3].Next;
			if (num3 == -1)
			{
				IL_A4:
				if (num3 == -1)
				{
					return false;
				}
				this.count--;
				if (num4 == -1)
				{
					this.table[num2] = this.linkSlots[num3].Next + 1;
				}
				else
				{
					this.linkSlots[num4].Next = this.linkSlots[num3].Next;
				}
				this.linkSlots[num3].Next = this.emptySlot;
				this.emptySlot = num3;
				this.linkSlots[num3].HashCode = 0;
				this.keySlots[num3] = default(TKey);
				this.valueSlots[num3] = default(TValue);
				this.generation++;
				return true;
			}
		}
		goto IL_A4;
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00004788 File Offset: 0x00002988
	public bool TryGetValue(TKey key, out TValue value)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		int num = this.hcp.GetHashCode(key) | int.MinValue;
		for (int num2 = this.table[(num & int.MaxValue) % this.table.Length] - 1; num2 != -1; num2 = this.linkSlots[num2].Next)
		{
			if (this.linkSlots[num2].HashCode == num && this.hcp.Equals(this.keySlots[num2], key))
			{
				value = this.valueSlots[num2];
				return true;
			}
		}
		value = default(TValue);
		return false;
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x0600006D RID: 109 RVA: 0x0000484E File Offset: 0x00002A4E
	public Map<TKey, TValue>.KeyCollection Keys
	{
		get
		{
			return new Map<TKey, TValue>.KeyCollection(this);
		}
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x0600006E RID: 110 RVA: 0x00004856 File Offset: 0x00002A56
	public Map<TKey, TValue>.ValueCollection Values
	{
		get
		{
			return new Map<TKey, TValue>.ValueCollection(this);
		}
	}

	// Token: 0x0600006F RID: 111 RVA: 0x0000485E File Offset: 0x00002A5E
	public Map<TKey, TValue>.Enumerator GetEnumerator()
	{
		return new Map<TKey, TValue>.Enumerator(this);
	}

	// Token: 0x0400001C RID: 28
	private const int INITIAL_SIZE = 4;

	// Token: 0x0400001D RID: 29
	private const float DEFAULT_LOAD_FACTOR = 0.9f;

	// Token: 0x0400001E RID: 30
	private const int NO_SLOT = -1;

	// Token: 0x0400001F RID: 31
	private const int HASH_FLAG = -2147483648;

	// Token: 0x04000020 RID: 32
	private int[] table;

	// Token: 0x04000021 RID: 33
	private Link[] linkSlots;

	// Token: 0x04000022 RID: 34
	private TKey[] keySlots;

	// Token: 0x04000023 RID: 35
	private TValue[] valueSlots;

	// Token: 0x04000024 RID: 36
	private IEqualityComparer<TKey> hcp;

	// Token: 0x04000025 RID: 37
	private int touchedSlots;

	// Token: 0x04000026 RID: 38
	private int emptySlot;

	// Token: 0x04000027 RID: 39
	private int count;

	// Token: 0x04000028 RID: 40
	private int threshold;

	// Token: 0x04000029 RID: 41
	private int generation;

	// Token: 0x02000012 RID: 18
	public sealed class ValueCollection : IEnumerable, ICollection, ICollection<TValue>, IEnumerable<TValue>
	{
		// Token: 0x06000227 RID: 551 RVA: 0x0000B0FF File Offset: 0x000092FF
		public ValueCollection(Map<TKey, TValue> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			this.dictionary = dictionary;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000B11F File Offset: 0x0000931F
		void ICollection<!1>.Add(TValue item)
		{
			throw new NotSupportedException("this is a read-only collection");
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000B12B File Offset: 0x0000932B
		void ICollection<!1>.Clear()
		{
			throw new NotSupportedException("this is a read-only collection");
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000B137 File Offset: 0x00009337
		bool ICollection<!1>.Contains(TValue item)
		{
			return this.dictionary.ContainsValue(item);
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000B145 File Offset: 0x00009345
		bool ICollection<!1>.Remove(TValue item)
		{
			throw new NotSupportedException("this is a read-only collection");
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000B151 File Offset: 0x00009351
		IEnumerator<TValue> IEnumerable<!1>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000B160 File Offset: 0x00009360
		void ICollection.CopyTo(Array array, int index)
		{
			TValue[] array2 = array as TValue[];
			if (array2 != null)
			{
				this.CopyTo(array2, index);
				return;
			}
			this.dictionary.CopyToCheck(array, index);
			this.dictionary.Do_ICollectionCopyTo<TValue>(array, index, new Map<TKey, TValue>.Transform<TValue>(Map<TKey, TValue>.pick_value));
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000B1A9 File Offset: 0x000093A9
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600022F RID: 559 RVA: 0x0000B1B6 File Offset: 0x000093B6
		bool ICollection<!1>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000230 RID: 560 RVA: 0x0000B1B9 File Offset: 0x000093B9
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000231 RID: 561 RVA: 0x0000B1BC File Offset: 0x000093BC
		object ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)this.dictionary).SyncRoot;
			}
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000B1CE File Offset: 0x000093CE
		public void CopyTo(TValue[] array, int index)
		{
			this.dictionary.CopyToCheck(array, index);
			this.dictionary.CopyValues(array, index);
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000B1EA File Offset: 0x000093EA
		public Map<TKey, TValue>.ValueCollection.Enumerator GetEnumerator()
		{
			return new Map<TKey, TValue>.ValueCollection.Enumerator(this.dictionary);
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000234 RID: 564 RVA: 0x0000B1F7 File Offset: 0x000093F7
		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		// Token: 0x04000084 RID: 132
		private Map<TKey, TValue> dictionary;

		// Token: 0x0200002F RID: 47
		public struct Enumerator : IDisposable, IEnumerator, IEnumerator<TValue>
		{
			// Token: 0x0600041F RID: 1055 RVA: 0x00012D4C File Offset: 0x00010F4C
			internal Enumerator(Map<TKey, TValue> host)
			{
				this.host_enumerator = host.GetEnumerator();
			}

			// Token: 0x1700005C RID: 92
			// (get) Token: 0x06000420 RID: 1056 RVA: 0x00012D5A File Offset: 0x00010F5A
			object IEnumerator.Current
			{
				get
				{
					return this.host_enumerator.CurrentValue;
				}
			}

			// Token: 0x06000421 RID: 1057 RVA: 0x00012D6C File Offset: 0x00010F6C
			void IEnumerator.Reset()
			{
				this.host_enumerator.Reset();
			}

			// Token: 0x06000422 RID: 1058 RVA: 0x00012D79 File Offset: 0x00010F79
			public void Dispose()
			{
				this.host_enumerator.Dispose();
			}

			// Token: 0x06000423 RID: 1059 RVA: 0x00012D86 File Offset: 0x00010F86
			public bool MoveNext()
			{
				return this.host_enumerator.MoveNext();
			}

			// Token: 0x1700005D RID: 93
			// (get) Token: 0x06000424 RID: 1060 RVA: 0x00012D93 File Offset: 0x00010F93
			public TValue Current
			{
				get
				{
					return this.host_enumerator.current.Value;
				}
			}

			// Token: 0x040001D3 RID: 467
			private Map<TKey, TValue>.Enumerator host_enumerator;
		}
	}

	// Token: 0x02000030 RID: 48
	public struct Enumerator : IDisposable, IEnumerator, IEnumerator<KeyValuePair<TKey, TValue>>
	{
		// Token: 0x06000425 RID: 1061 RVA: 0x00012DA5 File Offset: 0x00010FA5
		internal Enumerator(Map<TKey, TValue> dictionary)
		{
			this.dictionary = dictionary;
			this.stamp = dictionary.generation;
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000426 RID: 1062 RVA: 0x00012DBA File Offset: 0x00010FBA
		object IEnumerator.Current
		{
			get
			{
				this.VerifyCurrent();
				return this.current;
			}
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x00012DCD File Offset: 0x00010FCD
		void IEnumerator.Reset()
		{
			this.Reset();
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00012DD8 File Offset: 0x00010FD8
		public bool MoveNext()
		{
			this.VerifyState();
			if (this.next < 0)
			{
				return false;
			}
			while (this.next < this.dictionary.touchedSlots)
			{
				int num = this.next++;
				if ((this.dictionary.linkSlots[num].HashCode & -2147483648) != 0)
				{
					this.current = new KeyValuePair<TKey, TValue>(this.dictionary.keySlots[num], this.dictionary.valueSlots[num]);
					return true;
				}
			}
			this.next = -1;
			return false;
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000429 RID: 1065 RVA: 0x00012E7E File Offset: 0x0001107E
		public KeyValuePair<TKey, TValue> Current
		{
			get
			{
				return this.current;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600042A RID: 1066 RVA: 0x00012E86 File Offset: 0x00011086
		internal TKey CurrentKey
		{
			get
			{
				this.VerifyCurrent();
				return this.current.Key;
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600042B RID: 1067 RVA: 0x00012E99 File Offset: 0x00011099
		internal TValue CurrentValue
		{
			get
			{
				this.VerifyCurrent();
				return this.current.Value;
			}
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x00012EAC File Offset: 0x000110AC
		internal void Reset()
		{
			this.VerifyState();
			this.next = 0;
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x00012EBB File Offset: 0x000110BB
		private void VerifyState()
		{
			if (this.dictionary == null)
			{
				throw new ObjectDisposedException(null);
			}
			if (this.dictionary.generation != this.stamp)
			{
				throw new InvalidOperationException("out of sync");
			}
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x00012EF0 File Offset: 0x000110F0
		private void VerifyCurrent()
		{
			this.VerifyState();
			if (this.next <= 0)
			{
				throw new InvalidOperationException("Current is not valid");
			}
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x00012F0F File Offset: 0x0001110F
		public void Dispose()
		{
			this.dictionary = null;
		}

		// Token: 0x040001D4 RID: 468
		private Map<TKey, TValue> dictionary;

		// Token: 0x040001D5 RID: 469
		private int next;

		// Token: 0x040001D6 RID: 470
		private int stamp;

		// Token: 0x040001D7 RID: 471
		internal KeyValuePair<TKey, TValue> current;
	}

	// Token: 0x02000031 RID: 49
	// (Invoke) Token: 0x06000431 RID: 1073
	private delegate TRet Transform<TRet>(TKey key, TValue value);

	// Token: 0x02000033 RID: 51
	public sealed class KeyCollection : IEnumerable, ICollection, ICollection<TKey>, IEnumerable<TKey>
	{
		// Token: 0x06000434 RID: 1076 RVA: 0x00012F18 File Offset: 0x00011118
		public KeyCollection(Map<TKey, TValue> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			this.dictionary = dictionary;
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x00012F38 File Offset: 0x00011138
		void ICollection<!0>.Add(TKey item)
		{
			throw new NotSupportedException("this is a read-only collection");
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x00012F44 File Offset: 0x00011144
		void ICollection<!0>.Clear()
		{
			throw new NotSupportedException("this is a read-only collection");
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x00012F50 File Offset: 0x00011150
		bool ICollection<!0>.Contains(TKey item)
		{
			return this.dictionary.ContainsKey(item);
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x00012F5E File Offset: 0x0001115E
		bool ICollection<!0>.Remove(TKey item)
		{
			throw new NotSupportedException("this is a read-only collection");
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x00012F6A File Offset: 0x0001116A
		IEnumerator<TKey> IEnumerable<!0>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x00012F78 File Offset: 0x00011178
		void ICollection.CopyTo(Array array, int index)
		{
			TKey[] array2 = array as TKey[];
			if (array2 != null)
			{
				this.CopyTo(array2, index);
				return;
			}
			this.dictionary.CopyToCheck(array, index);
			this.dictionary.Do_ICollectionCopyTo<TKey>(array, index, new Map<TKey, TValue>.Transform<TKey>(Map<TKey, TValue>.pick_key));
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x00012FC1 File Offset: 0x000111C1
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x00012FCE File Offset: 0x000111CE
		bool ICollection<!0>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600043D RID: 1085 RVA: 0x00012FD1 File Offset: 0x000111D1
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x00012FD4 File Offset: 0x000111D4
		object ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)this.dictionary).SyncRoot;
			}
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x00012FE6 File Offset: 0x000111E6
		public void CopyTo(TKey[] array, int index)
		{
			this.dictionary.CopyToCheck(array, index);
			this.dictionary.CopyKeys(array, index);
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x00013002 File Offset: 0x00011202
		public Map<TKey, TValue>.KeyCollection.Enumerator GetEnumerator()
		{
			return new Map<TKey, TValue>.KeyCollection.Enumerator(this.dictionary);
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000441 RID: 1089 RVA: 0x0001300F File Offset: 0x0001120F
		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		// Token: 0x040001DA RID: 474
		private Map<TKey, TValue> dictionary;

		// Token: 0x02000034 RID: 52
		public struct Enumerator : IDisposable, IEnumerator, IEnumerator<TKey>
		{
			// Token: 0x06000442 RID: 1090 RVA: 0x0001301C File Offset: 0x0001121C
			internal Enumerator(Map<TKey, TValue> host)
			{
				this.host_enumerator = host.GetEnumerator();
			}

			// Token: 0x17000066 RID: 102
			// (get) Token: 0x06000443 RID: 1091 RVA: 0x0001302A File Offset: 0x0001122A
			object IEnumerator.Current
			{
				get
				{
					return this.host_enumerator.CurrentKey;
				}
			}

			// Token: 0x06000444 RID: 1092 RVA: 0x0001303C File Offset: 0x0001123C
			void IEnumerator.Reset()
			{
				this.host_enumerator.Reset();
			}

			// Token: 0x06000445 RID: 1093 RVA: 0x00013049 File Offset: 0x00011249
			public void Dispose()
			{
				this.host_enumerator.Dispose();
			}

			// Token: 0x06000446 RID: 1094 RVA: 0x00013056 File Offset: 0x00011256
			public bool MoveNext()
			{
				return this.host_enumerator.MoveNext();
			}

			// Token: 0x17000067 RID: 103
			// (get) Token: 0x06000447 RID: 1095 RVA: 0x00013063 File Offset: 0x00011263
			public TKey Current
			{
				get
				{
					return this.host_enumerator.current.Key;
				}
			}

			// Token: 0x040001DB RID: 475
			private Map<TKey, TValue>.Enumerator host_enumerator;
		}
	}
}
