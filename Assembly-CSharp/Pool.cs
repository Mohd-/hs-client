using System;
using System.Collections.Generic;

// Token: 0x0200071B RID: 1819
public class Pool<T>
{
	// Token: 0x06004A47 RID: 19015 RVA: 0x0016387B File Offset: 0x00161A7B
	public int GetExtensionCount()
	{
		return this.m_extensionCount;
	}

	// Token: 0x06004A48 RID: 19016 RVA: 0x00163883 File Offset: 0x00161A83
	public void SetExtensionCount(int count)
	{
		this.m_extensionCount = count;
	}

	// Token: 0x06004A49 RID: 19017 RVA: 0x0016388C File Offset: 0x00161A8C
	public int GetMaxReleasedItemCount()
	{
		return this.m_maxReleasedItemCount;
	}

	// Token: 0x06004A4A RID: 19018 RVA: 0x00163894 File Offset: 0x00161A94
	public void SetMaxReleasedItemCount(int count)
	{
		this.m_maxReleasedItemCount = count;
	}

	// Token: 0x06004A4B RID: 19019 RVA: 0x0016389D File Offset: 0x00161A9D
	public Pool<T>.CreateItemCallback GetCreateItemCallback()
	{
		return this.m_createItemCallback;
	}

	// Token: 0x06004A4C RID: 19020 RVA: 0x001638A5 File Offset: 0x00161AA5
	public void SetCreateItemCallback(Pool<T>.CreateItemCallback callback)
	{
		this.m_createItemCallback = callback;
	}

	// Token: 0x06004A4D RID: 19021 RVA: 0x001638AE File Offset: 0x00161AAE
	public Pool<T>.DestroyItemCallback GetDestroyItemCallback()
	{
		return this.m_destroyItemCallback;
	}

	// Token: 0x06004A4E RID: 19022 RVA: 0x001638B6 File Offset: 0x00161AB6
	public void SetDestroyItemCallback(Pool<T>.DestroyItemCallback callback)
	{
		this.m_destroyItemCallback = callback;
	}

	// Token: 0x06004A4F RID: 19023 RVA: 0x001638C0 File Offset: 0x00161AC0
	public T Acquire()
	{
		if (this.m_freeList.Count == 0)
		{
			if (this.m_extensionCount == 0)
			{
				return default(T);
			}
			if (!this.AddFreeItems(this.m_extensionCount))
			{
				return default(T);
			}
		}
		int num = this.m_freeList.Count - 1;
		T t = this.m_freeList[num];
		this.m_freeList.RemoveAt(num);
		this.m_activeList.Add(t);
		return t;
	}

	// Token: 0x06004A50 RID: 19024 RVA: 0x00163944 File Offset: 0x00161B44
	public List<T> AcquireBatch(int count)
	{
		List<T> list = new List<T>();
		for (int i = 0; i < count; i++)
		{
			T t = this.Acquire();
			list.Add(t);
		}
		return list;
	}

	// Token: 0x06004A51 RID: 19025 RVA: 0x00163978 File Offset: 0x00161B78
	public bool Release(T item)
	{
		if (!this.m_activeList.Remove(item))
		{
			return false;
		}
		if (this.m_freeList.Count < this.m_maxReleasedItemCount)
		{
			this.m_freeList.Add(item);
			return true;
		}
		if (this.m_destroyItemCallback != null)
		{
			return false;
		}
		this.m_destroyItemCallback(item);
		return true;
	}

	// Token: 0x06004A52 RID: 19026 RVA: 0x001639D8 File Offset: 0x00161BD8
	public bool ReleaseBatch(int activeListStart, int count)
	{
		if (count <= 0)
		{
			return true;
		}
		if (activeListStart >= this.m_activeList.Count)
		{
			return false;
		}
		int num = this.m_activeList.Count - activeListStart;
		if (count > num)
		{
			count = num;
		}
		int num2 = count;
		int num3 = this.m_maxReleasedItemCount - this.m_freeList.Count;
		if (num2 > num3)
		{
			num2 = num3;
		}
		if (num2 > 0)
		{
			List<T> range = this.m_activeList.GetRange(activeListStart, num2);
			this.m_activeList.RemoveRange(activeListStart, num2);
			this.m_freeList.AddRange(range);
		}
		int num4 = count - num2;
		if (num4 > 0)
		{
			if (this.m_destroyItemCallback == null)
			{
				return false;
			}
			for (int i = 0; i < num4; i++)
			{
				T item = this.m_activeList[activeListStart];
				this.m_activeList.RemoveAt(activeListStart);
				this.m_destroyItemCallback(item);
			}
		}
		return true;
	}

	// Token: 0x06004A53 RID: 19027 RVA: 0x00163ABE File Offset: 0x00161CBE
	public bool ReleaseAll()
	{
		return this.ReleaseBatch(0, this.m_activeList.Count);
	}

	// Token: 0x06004A54 RID: 19028 RVA: 0x00163AD4 File Offset: 0x00161CD4
	public bool AddFreeItems(int count)
	{
		if (this.m_createItemCallback == null)
		{
			return false;
		}
		for (int i = 0; i < count; i++)
		{
			int freeListIndex = this.m_activeList.Count + this.m_freeList.Count + 1;
			T t = this.m_createItemCallback(freeListIndex);
			this.m_freeList.Add(t);
		}
		return true;
	}

	// Token: 0x06004A55 RID: 19029 RVA: 0x00163B34 File Offset: 0x00161D34
	public void Clear()
	{
		if (this.m_destroyItemCallback == null)
		{
			this.m_activeList.Clear();
			this.m_freeList.Clear();
			return;
		}
		for (int i = 0; i < this.m_activeList.Count; i++)
		{
			this.m_destroyItemCallback(this.m_activeList[i]);
		}
		this.m_activeList.Clear();
		for (int j = 0; j < this.m_freeList.Count; j++)
		{
			this.m_destroyItemCallback(this.m_freeList[j]);
		}
		this.m_freeList.Clear();
	}

	// Token: 0x06004A56 RID: 19030 RVA: 0x00163BDF File Offset: 0x00161DDF
	public List<T> GetFreeList()
	{
		return this.m_freeList;
	}

	// Token: 0x06004A57 RID: 19031 RVA: 0x00163BE7 File Offset: 0x00161DE7
	public List<T> GetActiveList()
	{
		return this.m_activeList;
	}

	// Token: 0x04003175 RID: 12661
	public const int DEFAULT_EXTENSION_COUNT = 5;

	// Token: 0x04003176 RID: 12662
	public const int DEFAULT_MAX_RELEASED_ITEM_COUNT = 5;

	// Token: 0x04003177 RID: 12663
	private List<T> m_freeList = new List<T>();

	// Token: 0x04003178 RID: 12664
	private List<T> m_activeList = new List<T>();

	// Token: 0x04003179 RID: 12665
	private int m_extensionCount = 5;

	// Token: 0x0400317A RID: 12666
	private int m_maxReleasedItemCount = 5;

	// Token: 0x0400317B RID: 12667
	private Pool<T>.CreateItemCallback m_createItemCallback;

	// Token: 0x0400317C RID: 12668
	private Pool<T>.DestroyItemCallback m_destroyItemCallback;

	// Token: 0x0200071C RID: 1820
	// (Invoke) Token: 0x06004A59 RID: 19033
	public delegate T CreateItemCallback(int freeListIndex);

	// Token: 0x0200071D RID: 1821
	// (Invoke) Token: 0x06004A5D RID: 19037
	public delegate void DestroyItemCallback(T item);
}
