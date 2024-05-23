using System;
using System.Collections.Generic;

// Token: 0x02000624 RID: 1572
public class TagDeltaSet
{
	// Token: 0x060044BA RID: 17594 RVA: 0x0014A9DE File Offset: 0x00148BDE
	public List<TagDelta> GetList()
	{
		return this.m_deltas;
	}

	// Token: 0x060044BB RID: 17595 RVA: 0x0014A9E8 File Offset: 0x00148BE8
	public void Add(int tag, int prev, int curr)
	{
		TagDelta tagDelta = new TagDelta();
		tagDelta.tag = tag;
		tagDelta.oldValue = prev;
		tagDelta.newValue = curr;
		this.m_deltas.Add(tagDelta);
	}

	// Token: 0x060044BC RID: 17596 RVA: 0x0014AA1C File Offset: 0x00148C1C
	public void Add<TagEnum>(GAME_TAG tag, TagEnum prev, TagEnum curr)
	{
		TagDelta tagDelta = new TagDelta();
		tagDelta.tag = (int)tag;
		tagDelta.oldValue = Convert.ToInt32(prev);
		tagDelta.newValue = Convert.ToInt32(curr);
		this.m_deltas.Add(tagDelta);
	}

	// Token: 0x060044BD RID: 17597 RVA: 0x0014AA64 File Offset: 0x00148C64
	public void Sort(Comparison<TagDelta> compFunc)
	{
		this.m_deltas.Sort(compFunc);
	}

	// Token: 0x060044BE RID: 17598 RVA: 0x0014AA72 File Offset: 0x00148C72
	public int Size()
	{
		return this.m_deltas.Count;
	}

	// Token: 0x060044BF RID: 17599 RVA: 0x0014AA7F File Offset: 0x00148C7F
	public int Tag(int index)
	{
		return this.m_deltas[index].tag;
	}

	// Token: 0x060044C0 RID: 17600 RVA: 0x0014AA92 File Offset: 0x00148C92
	public int OldValue(int index)
	{
		return this.m_deltas[index].oldValue;
	}

	// Token: 0x060044C1 RID: 17601 RVA: 0x0014AAA5 File Offset: 0x00148CA5
	public int NewValue(int index)
	{
		return this.m_deltas[index].newValue;
	}

	// Token: 0x170004B2 RID: 1202
	public TagDelta this[int index]
	{
		get
		{
			return this.m_deltas[index];
		}
	}

	// Token: 0x04002B9A RID: 11162
	private List<TagDelta> m_deltas = new List<TagDelta>();
}
