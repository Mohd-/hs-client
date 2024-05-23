using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x0200019D RID: 413
public class QueueList<T> : IEnumerable, IEnumerable<!0>
{
	// Token: 0x06001AF9 RID: 6905 RVA: 0x0007EC5E File Offset: 0x0007CE5E
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	// Token: 0x06001AFA RID: 6906 RVA: 0x0007EC68 File Offset: 0x0007CE68
	public int Enqueue(T item)
	{
		int count = this.m_list.Count;
		this.m_list.Add(item);
		return count;
	}

	// Token: 0x06001AFB RID: 6907 RVA: 0x0007EC90 File Offset: 0x0007CE90
	public T Dequeue()
	{
		T result = this.m_list[0];
		this.m_list.RemoveAt(0);
		return result;
	}

	// Token: 0x06001AFC RID: 6908 RVA: 0x0007ECB7 File Offset: 0x0007CEB7
	public T Peek()
	{
		return this.m_list[0];
	}

	// Token: 0x06001AFD RID: 6909 RVA: 0x0007ECC5 File Offset: 0x0007CEC5
	public int GetCount()
	{
		return this.m_list.Count;
	}

	// Token: 0x1700031F RID: 799
	// (get) Token: 0x06001AFE RID: 6910 RVA: 0x0007ECD2 File Offset: 0x0007CED2
	public int Count
	{
		get
		{
			return this.m_list.Count;
		}
	}

	// Token: 0x06001AFF RID: 6911 RVA: 0x0007ECDF File Offset: 0x0007CEDF
	public T GetItem(int index)
	{
		return this.m_list[index];
	}

	// Token: 0x17000320 RID: 800
	public T this[int index]
	{
		get
		{
			return this.m_list[index];
		}
		set
		{
			this.m_list[index] = value;
		}
	}

	// Token: 0x06001B02 RID: 6914 RVA: 0x0007ED0A File Offset: 0x0007CF0A
	public void Clear()
	{
		this.m_list.Clear();
	}

	// Token: 0x06001B03 RID: 6915 RVA: 0x0007ED18 File Offset: 0x0007CF18
	public T RemoveAt(int position)
	{
		if (this.m_list.Count <= position)
		{
			return default(T);
		}
		T result = this.m_list[position];
		this.m_list.RemoveAt(position);
		return result;
	}

	// Token: 0x06001B04 RID: 6916 RVA: 0x0007ED5A File Offset: 0x0007CF5A
	public bool Remove(T item)
	{
		return this.m_list.Remove(item);
	}

	// Token: 0x06001B05 RID: 6917 RVA: 0x0007ED68 File Offset: 0x0007CF68
	public List<T> GetList()
	{
		return this.m_list;
	}

	// Token: 0x06001B06 RID: 6918 RVA: 0x0007ED70 File Offset: 0x0007CF70
	public bool Contains(T item)
	{
		return this.m_list.Contains(item);
	}

	// Token: 0x06001B07 RID: 6919 RVA: 0x0007ED7E File Offset: 0x0007CF7E
	public IEnumerator<T> GetEnumerator()
	{
		return this.Enumerate().GetEnumerator();
	}

	// Token: 0x06001B08 RID: 6920 RVA: 0x0007ED8B File Offset: 0x0007CF8B
	public override string ToString()
	{
		return string.Format("Count={0}", this.Count);
	}

	// Token: 0x06001B09 RID: 6921 RVA: 0x0007EDA4 File Offset: 0x0007CFA4
	protected IEnumerable<T> Enumerate()
	{
		for (int i = 0; i < this.m_list.Count; i++)
		{
			yield return this.m_list[i];
		}
		yield break;
	}

	// Token: 0x04000D5A RID: 3418
	protected List<T> m_list = new List<T>();
}
