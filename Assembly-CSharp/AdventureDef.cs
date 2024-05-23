using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000216 RID: 534
[CustomEditClass]
public class AdventureDef : MonoBehaviour
{
	// Token: 0x060020B6 RID: 8374 RVA: 0x0009FF1C File Offset: 0x0009E11C
	public void Init(AdventureDbfRecord advRecord, List<AdventureDataDbfRecord> advDataRecords)
	{
		this.m_AdventureId = (AdventureDbId)advRecord.ID;
		this.m_AdventureName = advRecord.Name;
		this.m_SortOrder = advRecord.SortOrder;
		foreach (AdventureDataDbfRecord adventureDataDbfRecord in advDataRecords)
		{
			int adventureId = adventureDataDbfRecord.AdventureId;
			if (adventureId == (int)this.m_AdventureId)
			{
				string adventureSubDefPrefab = adventureDataDbfRecord.AdventureSubDefPrefab;
				if (!string.IsNullOrEmpty(adventureSubDefPrefab))
				{
					GameObject gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(adventureSubDefPrefab), true, false);
					if (!(gameObject == null))
					{
						AdventureSubDef component = gameObject.GetComponent<AdventureSubDef>();
						if (component == null)
						{
							Debug.LogError(string.Format("{0} object does not contain AdventureSubDef component.", adventureSubDefPrefab));
							Object.Destroy(gameObject);
						}
						else
						{
							component.Init(adventureDataDbfRecord);
							this.m_SubDefs.Add(component.GetAdventureModeId(), component);
						}
					}
				}
			}
		}
	}

	// Token: 0x060020B7 RID: 8375 RVA: 0x000A0038 File Offset: 0x0009E238
	public AdventureDbId GetAdventureId()
	{
		return this.m_AdventureId;
	}

	// Token: 0x060020B8 RID: 8376 RVA: 0x000A0040 File Offset: 0x0009E240
	public string GetAdventureName()
	{
		return this.m_AdventureName;
	}

	// Token: 0x060020B9 RID: 8377 RVA: 0x000A0048 File Offset: 0x0009E248
	public AdventureSubDef GetSubDef(AdventureModeDbId modeId)
	{
		AdventureSubDef result = null;
		this.m_SubDefs.TryGetValue(modeId, out result);
		return result;
	}

	// Token: 0x060020BA RID: 8378 RVA: 0x000A0068 File Offset: 0x0009E268
	public List<AdventureSubDef> GetSortedSubDefs()
	{
		List<AdventureSubDef> list = new List<AdventureSubDef>(this.m_SubDefs.Values);
		list.Sort((AdventureSubDef l, AdventureSubDef r) => l.GetSortOrder() - r.GetSortOrder());
		return list;
	}

	// Token: 0x060020BB RID: 8379 RVA: 0x000A00AA File Offset: 0x0009E2AA
	public int GetSortOrder()
	{
		return this.m_SortOrder;
	}

	// Token: 0x060020BC RID: 8380 RVA: 0x000A00B4 File Offset: 0x0009E2B4
	public bool IsActiveAndPlayable()
	{
		foreach (WingDbfRecord wingDbfRecord in GameDbf.Wing.GetRecords())
		{
			int adventureId = wingDbfRecord.AdventureId;
			if (adventureId == (int)this.GetAdventureId())
			{
				if (AdventureProgressMgr.Get().IsWingOpen(wingDbfRecord.ID))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x040011E8 RID: 4584
	[CustomEditField(Sections = "Reward Banners")]
	public AdventureDef.BannerRewardType m_BannerRewardType;

	// Token: 0x040011E9 RID: 4585
	[CustomEditField(Sections = "Reward Banners", T = EditType.GAME_OBJECT)]
	public string m_BannerRewardPrefab;

	// Token: 0x040011EA RID: 4586
	[CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
	public String_MobileOverride m_ProgressDisplayPrefab;

	// Token: 0x040011EB RID: 4587
	[CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
	public string m_WingBottomBorderPrefab;

	// Token: 0x040011EC RID: 4588
	[CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
	public string m_DefaultQuotePrefab;

	// Token: 0x040011ED RID: 4589
	[CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
	public string m_ChooserButtonPrefab;

	// Token: 0x040011EE RID: 4590
	[CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
	public string m_ChooserSubButtonPrefab;

	// Token: 0x040011EF RID: 4591
	[CustomEditField(Sections = "Chooser Button", T = EditType.TEXTURE)]
	public string m_Texture;

	// Token: 0x040011F0 RID: 4592
	[CustomEditField(Sections = "Chooser Button")]
	public Vector2 m_TextureTiling = Vector2.one;

	// Token: 0x040011F1 RID: 4593
	[CustomEditField(Sections = "Chooser Button")]
	public Vector2 m_TextureOffset = Vector2.zero;

	// Token: 0x040011F2 RID: 4594
	private AdventureDbId m_AdventureId;

	// Token: 0x040011F3 RID: 4595
	private string m_AdventureName;

	// Token: 0x040011F4 RID: 4596
	private Map<AdventureModeDbId, AdventureSubDef> m_SubDefs = new Map<AdventureModeDbId, AdventureSubDef>();

	// Token: 0x040011F5 RID: 4597
	private int m_SortOrder;

	// Token: 0x0200022B RID: 555
	public enum BannerRewardType
	{
		// Token: 0x0400126F RID: 4719
		AdventureCompleteReward,
		// Token: 0x04001270 RID: 4720
		BannerManagerPopup
	}
}
