using System;
using System.Collections.Generic;

// Token: 0x02000344 RID: 836
public class TagSet
{
	// Token: 0x06002BB5 RID: 11189 RVA: 0x000D9689 File Offset: 0x000D7889
	public void SetTag(int tag, int tagValue)
	{
		this.m_values[tag] = tagValue;
	}

	// Token: 0x06002BB6 RID: 11190 RVA: 0x000D9698 File Offset: 0x000D7898
	public void SetTag(GAME_TAG tag, int tagValue)
	{
		this.SetTag((int)tag, tagValue);
	}

	// Token: 0x06002BB7 RID: 11191 RVA: 0x000D96A2 File Offset: 0x000D78A2
	public void SetTag<TagEnum>(GAME_TAG tag, TagEnum tagValue)
	{
		this.SetTag((int)tag, Convert.ToInt32(tagValue));
	}

	// Token: 0x06002BB8 RID: 11192 RVA: 0x000D96B8 File Offset: 0x000D78B8
	public void SetTags(Map<int, int> tagMap)
	{
		foreach (KeyValuePair<int, int> keyValuePair in tagMap)
		{
			this.SetTag(keyValuePair.Key, keyValuePair.Value);
		}
	}

	// Token: 0x06002BB9 RID: 11193 RVA: 0x000D971C File Offset: 0x000D791C
	public void SetTags(Map<GAME_TAG, int> tagMap)
	{
		foreach (KeyValuePair<GAME_TAG, int> keyValuePair in tagMap)
		{
			this.SetTag(keyValuePair.Key, keyValuePair.Value);
		}
	}

	// Token: 0x06002BBA RID: 11194 RVA: 0x000D9780 File Offset: 0x000D7980
	public void SetTags(List<Network.Entity.Tag> tags)
	{
		foreach (Network.Entity.Tag tag in tags)
		{
			this.SetTag(tag.Name, tag.Value);
		}
	}

	// Token: 0x06002BBB RID: 11195 RVA: 0x000D97E0 File Offset: 0x000D79E0
	public Map<int, int> GetMap()
	{
		return this.m_values;
	}

	// Token: 0x06002BBC RID: 11196 RVA: 0x000D97E8 File Offset: 0x000D79E8
	public int GetTag(int tag)
	{
		int result = 0;
		this.m_values.TryGetValue(tag, out result);
		return result;
	}

	// Token: 0x06002BBD RID: 11197 RVA: 0x000D9808 File Offset: 0x000D7A08
	public TagEnum GetTag<TagEnum>(GAME_TAG enumTag)
	{
		int tag = Convert.ToInt32(enumTag);
		int tag2 = this.GetTag(tag);
		return (TagEnum)((object)Enum.ToObject(typeof(TagEnum), tag2));
	}

	// Token: 0x06002BBE RID: 11198 RVA: 0x000D9840 File Offset: 0x000D7A40
	public int GetTag(GAME_TAG enumTag)
	{
		int tag = Convert.ToInt32(enumTag);
		return this.GetTag(tag);
	}

	// Token: 0x06002BBF RID: 11199 RVA: 0x000D9860 File Offset: 0x000D7A60
	public bool HasTag(int tag)
	{
		int num = 0;
		return this.m_values.TryGetValue(tag, out num) && num > 0;
	}

	// Token: 0x06002BC0 RID: 11200 RVA: 0x000D9888 File Offset: 0x000D7A88
	public bool HasTag<TagEnum>(GAME_TAG tag)
	{
		TagEnum tag2 = this.GetTag<TagEnum>(tag);
		uint num = Convert.ToUInt32(tag2);
		return num > 0U;
	}

	// Token: 0x06002BC1 RID: 11201 RVA: 0x000D98AD File Offset: 0x000D7AAD
	public void Replace(TagSet sourceSet)
	{
		this.Clear();
		this.SetTags(sourceSet.m_values);
	}

	// Token: 0x06002BC2 RID: 11202 RVA: 0x000D98C1 File Offset: 0x000D7AC1
	public void Replace(List<Network.Entity.Tag> tags)
	{
		this.Clear();
		this.SetTags(tags);
	}

	// Token: 0x06002BC3 RID: 11203 RVA: 0x000D98D0 File Offset: 0x000D7AD0
	public void Clear()
	{
		this.m_values = new Map<int, int>();
	}

	// Token: 0x06002BC4 RID: 11204 RVA: 0x000D98E0 File Offset: 0x000D7AE0
	public TagDeltaSet CreateDeltas(TagSet comp)
	{
		TagDeltaSet tagDeltaSet = new TagDeltaSet();
		for (int i = 1; i < 512; i++)
		{
			if (this.m_values[i] != comp.m_values[i])
			{
				tagDeltaSet.Add(i, this.m_values[i], comp.m_values[i]);
			}
		}
		return tagDeltaSet;
	}

	// Token: 0x06002BC5 RID: 11205 RVA: 0x000D9948 File Offset: 0x000D7B48
	public TagDeltaSet CreateDeltas(List<Network.Entity.Tag> comp)
	{
		TagDeltaSet tagDeltaSet = new TagDeltaSet();
		foreach (Network.Entity.Tag tag in comp)
		{
			int name = tag.Name;
			int num = 0;
			this.m_values.TryGetValue(name, out num);
			int value = tag.Value;
			if (num != value)
			{
				tagDeltaSet.Add(name, num, value);
			}
		}
		return tagDeltaSet;
	}

	// Token: 0x06002BC6 RID: 11206 RVA: 0x000D99D4 File Offset: 0x000D7BD4
	public TagDeltaSet CreateDeltas(Map<GAME_TAG, int> comp)
	{
		TagDeltaSet tagDeltaSet = new TagDeltaSet();
		foreach (KeyValuePair<GAME_TAG, int> keyValuePair in comp)
		{
			int key = (int)keyValuePair.Key;
			int num = 0;
			this.m_values.TryGetValue(key, out num);
			int value = keyValuePair.Value;
			if (num != value)
			{
				tagDeltaSet.Add(key, num, value);
			}
		}
		return tagDeltaSet;
	}

	// Token: 0x06002BC7 RID: 11207 RVA: 0x000D9A60 File Offset: 0x000D7C60
	public bool TryGetValue(int tag, out int value)
	{
		return this.m_values.TryGetValue(tag, out value);
	}

	// Token: 0x04001A75 RID: 6773
	private Map<int, int> m_values = new Map<int, int>();
}
