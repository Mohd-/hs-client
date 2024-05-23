using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class Dbf<T> : IDbf where T : DbfRecord, new()
{
	// Token: 0x0600024F RID: 591 RVA: 0x0000B7F2 File Offset: 0x000099F2
	private Dbf(string name)
	{
		this.m_name = name;
	}

	// Token: 0x06000250 RID: 592 RVA: 0x0000B817 File Offset: 0x00009A17
	private Dbf(string name, Dbf<T>.RecordAddedListener addedListener, Dbf<T>.RecordsRemovedListener removedListener) : this(name)
	{
		this.m_recordAddedListener = addedListener;
		this.m_recordsRemovedListener = removedListener;
	}

	// Token: 0x06000251 RID: 593 RVA: 0x0000B830 File Offset: 0x00009A30
	public DbfRecord CreateNewRecord()
	{
		T t = Activator.CreateInstance<T>();
		return t;
	}

	// Token: 0x06000252 RID: 594 RVA: 0x0000B84C File Offset: 0x00009A4C
	public void AddRecord(DbfRecord record)
	{
		T t = (T)((object)record);
		this.m_records.Add(t);
		this.m_recordsById[record.ID] = t;
		if (this.m_recordAddedListener != null)
		{
			this.m_recordAddedListener(t);
		}
	}

	// Token: 0x06000253 RID: 595 RVA: 0x0000B895 File Offset: 0x00009A95
	public Type GetRecordType()
	{
		return typeof(T);
	}

	// Token: 0x06000254 RID: 596 RVA: 0x0000B8A1 File Offset: 0x00009AA1
	public List<T> GetRecords()
	{
		return this.m_records;
	}

	// Token: 0x06000255 RID: 597 RVA: 0x0000B8A9 File Offset: 0x00009AA9
	public List<T> GetRecords(Predicate<T> predicate)
	{
		return this.m_records.FindAll(predicate);
	}

	// Token: 0x06000256 RID: 598 RVA: 0x0000B8B7 File Offset: 0x00009AB7
	public static Dbf<T> Load(string name)
	{
		return Dbf<T>.Load(name, Dbf<T>.GetAssetPath(name), null, null);
	}

	// Token: 0x06000257 RID: 599 RVA: 0x0000B8C7 File Offset: 0x00009AC7
	public static Dbf<T> Load(string name, Dbf<T>.RecordAddedListener addedListener, Dbf<T>.RecordsRemovedListener removedListener)
	{
		return Dbf<T>.Load(name, Dbf<T>.GetAssetPath(name), addedListener, removedListener);
	}

	// Token: 0x06000258 RID: 600 RVA: 0x0000B8D7 File Offset: 0x00009AD7
	public static Dbf<T> Load(string name, string xml)
	{
		return Dbf<T>.Load(name, xml, null, null);
	}

	// Token: 0x06000259 RID: 601 RVA: 0x0000B8E4 File Offset: 0x00009AE4
	private static Dbf<T> Load(string name, string xml, Dbf<T>.RecordAddedListener addedListener, Dbf<T>.RecordsRemovedListener removedListener)
	{
		Dbf<T> dbf = new Dbf<T>(name, addedListener, removedListener);
		dbf.Clear();
		if (!DbfXml.Load(xml, dbf))
		{
			dbf.Clear();
			Debug.LogError(string.Format("Dbf.Load() - failed to load {0}", name));
		}
		return dbf;
	}

	// Token: 0x0600025A RID: 602 RVA: 0x0000B923 File Offset: 0x00009B23
	public string GetName()
	{
		return this.m_name;
	}

	// Token: 0x0600025B RID: 603 RVA: 0x0000B92B File Offset: 0x00009B2B
	public void Clear()
	{
		this.m_records.Clear();
	}

	// Token: 0x0600025C RID: 604 RVA: 0x0000B938 File Offset: 0x00009B38
	public T GetRecord(int id)
	{
		T result;
		if (this.m_recordsById.TryGetValue(id, out result))
		{
			return result;
		}
		return this.m_records.Find((T r) => r.ID == id);
	}

	// Token: 0x0600025D RID: 605 RVA: 0x0000B983 File Offset: 0x00009B83
	public T GetRecord(Predicate<T> match)
	{
		return this.m_records.Find(match);
	}

	// Token: 0x0600025E RID: 606 RVA: 0x0000B994 File Offset: 0x00009B94
	public bool HasRecord(int id)
	{
		T t = (T)((object)null);
		if (!this.m_recordsById.TryGetValue(id, out t))
		{
			t = this.m_records.Find((T r) => r.ID == id);
		}
		return t != null;
	}

	// Token: 0x0600025F RID: 607 RVA: 0x0000B9F1 File Offset: 0x00009BF1
	public bool HasRecord(Predicate<T> match)
	{
		return this.GetRecord(match) != null;
	}

	// Token: 0x06000260 RID: 608 RVA: 0x0000BA08 File Offset: 0x00009C08
	public void ReplaceRecordByRecordId(T record)
	{
		int num = this.m_records.FindIndex((T r) => r.ID == record.ID);
		if (num == -1)
		{
			this.AddRecord(record);
		}
		else
		{
			T t = this.m_records[num];
			bool flag = t != record;
			if (flag && this.m_recordsRemovedListener != null)
			{
				List<T> list = new List<T>();
				list.Add(t);
				this.m_recordsRemovedListener(list);
			}
			this.m_records[num] = record;
			this.m_recordsById[record.ID] = record;
			if (flag && this.m_recordAddedListener != null)
			{
				this.m_recordAddedListener(record);
			}
		}
	}

	// Token: 0x06000261 RID: 609 RVA: 0x0000BB04 File Offset: 0x00009D04
	public void RemoveRecordsWhere(Predicate<T> match)
	{
		List<int> list = null;
		for (int i = 0; i < this.m_records.Count; i++)
		{
			if (match.Invoke(this.m_records[i]))
			{
				if (list == null)
				{
					list = new List<int>();
				}
				list.Add(i);
			}
		}
		if (list != null)
		{
			List<T> list2 = null;
			if (this.m_recordsRemovedListener != null)
			{
				list2 = new List<T>(list.Count);
			}
			for (int j = list.Count - 1; j >= 0; j--)
			{
				int num = list[j];
				T t = this.m_records[num];
				if (list2 != null && t != null)
				{
					list2.Add(t);
				}
				T t2;
				if (this.m_recordsById.TryGetValue(t.ID, out t2))
				{
					this.m_recordsById.Remove(t2.ID);
				}
				this.m_records.RemoveAt(num);
			}
			if (this.m_recordsRemovedListener != null)
			{
				this.m_recordsRemovedListener(list2);
			}
		}
	}

	// Token: 0x06000262 RID: 610 RVA: 0x0000BC1F File Offset: 0x00009E1F
	public override string ToString()
	{
		return this.m_name;
	}

	// Token: 0x06000263 RID: 611 RVA: 0x0000BC28 File Offset: 0x00009E28
	private static string GetAssetPath(string name)
	{
		string subPath = string.Format("Assets/Game/DBF/{0}.xml", name);
		string assetPath;
		if (ApplicationMgr.TryGetStandaloneLocalDataPath(subPath, out assetPath))
		{
			return assetPath;
		}
		assetPath = FileUtils.GetAssetPath(string.Format("DBF/{0}.xml", name));
		return assetPath;
	}

	// Token: 0x0400009B RID: 155
	private string m_name;

	// Token: 0x0400009C RID: 156
	private List<T> m_records = new List<T>();

	// Token: 0x0400009D RID: 157
	private Map<int, T> m_recordsById = new Map<int, T>();

	// Token: 0x0400009E RID: 158
	private Dbf<T>.RecordAddedListener m_recordAddedListener;

	// Token: 0x0400009F RID: 159
	private Dbf<T>.RecordsRemovedListener m_recordsRemovedListener;

	// Token: 0x0200012D RID: 301
	// (Invoke) Token: 0x06000F79 RID: 3961
	public delegate void RecordAddedListener(T record);

	// Token: 0x0200012E RID: 302
	// (Invoke) Token: 0x06000F7D RID: 3965
	public delegate void RecordsRemovedListener(List<T> removedRecords);
}
