using System;
using UnityEngine;

// Token: 0x020001D4 RID: 468
[CustomEditClass]
public class AdventureSubDef : MonoBehaviour
{
	// Token: 0x06001D78 RID: 7544 RVA: 0x00089F00 File Offset: 0x00088100
	public void Init(AdventureDataDbfRecord advDataRecord)
	{
		this.m_AdventureModeId = (AdventureModeDbId)advDataRecord.ModeId;
		this.m_SortOrder = advDataRecord.SortOrder;
		this.m_ShortName = advDataRecord.ShortName;
		this.m_Description = ((!UniversalInputManager.UsePhoneUI) ? advDataRecord.Description : advDataRecord.ShortDescription);
		this.m_RequirementsDescription = advDataRecord.RequirementsDescription;
		this.m_CompleteBannerText = advDataRecord.CompleteBannerText;
	}

	// Token: 0x06001D79 RID: 7545 RVA: 0x00089F83 File Offset: 0x00088183
	public AdventureModeDbId GetAdventureModeId()
	{
		return this.m_AdventureModeId;
	}

	// Token: 0x06001D7A RID: 7546 RVA: 0x00089F8B File Offset: 0x0008818B
	public int GetSortOrder()
	{
		return this.m_SortOrder;
	}

	// Token: 0x06001D7B RID: 7547 RVA: 0x00089F93 File Offset: 0x00088193
	public string GetShortName()
	{
		return this.m_ShortName;
	}

	// Token: 0x06001D7C RID: 7548 RVA: 0x00089F9B File Offset: 0x0008819B
	public string GetDescription()
	{
		return this.m_Description;
	}

	// Token: 0x06001D7D RID: 7549 RVA: 0x00089FA3 File Offset: 0x000881A3
	public string GetRequirementsDescription()
	{
		return this.m_RequirementsDescription;
	}

	// Token: 0x06001D7E RID: 7550 RVA: 0x00089FAB File Offset: 0x000881AB
	public string GetCompleteBannerText()
	{
		return this.m_CompleteBannerText;
	}

	// Token: 0x04001032 RID: 4146
	[CustomEditField(Sections = "Mission Display", T = EditType.TEXTURE)]
	public string m_WatermarkTexture;

	// Token: 0x04001033 RID: 4147
	[CustomEditField(Sections = "Chooser", T = EditType.GAME_OBJECT)]
	public String_MobileOverride m_ChooserDescriptionPrefab;

	// Token: 0x04001034 RID: 4148
	[CustomEditField(Sections = "Chooser", T = EditType.TEXTURE)]
	public string m_Texture;

	// Token: 0x04001035 RID: 4149
	[CustomEditField(Sections = "Chooser")]
	public Vector2 m_TextureTiling = Vector2.one;

	// Token: 0x04001036 RID: 4150
	[CustomEditField(Sections = "Chooser")]
	public Vector2 m_TextureOffset = Vector2.zero;

	// Token: 0x04001037 RID: 4151
	private AdventureModeDbId m_AdventureModeId;

	// Token: 0x04001038 RID: 4152
	private int m_SortOrder;

	// Token: 0x04001039 RID: 4153
	private string m_ShortName;

	// Token: 0x0400103A RID: 4154
	private string m_Description;

	// Token: 0x0400103B RID: 4155
	private string m_RequirementsDescription;

	// Token: 0x0400103C RID: 4156
	private string m_CompleteBannerText;
}
