using System;
using UnityEngine;

// Token: 0x020003B2 RID: 946
[CustomEditClass]
public class AdventureRewardsChest : MonoBehaviour
{
	// Token: 0x060031EA RID: 12778 RVA: 0x000FB177 File Offset: 0x000F9377
	public void AddChestEventListener(UIEventType type, UIEvent.Handler handler)
	{
		this.m_ChestClickArea.AddEventListener(type, handler);
	}

	// Token: 0x060031EB RID: 12779 RVA: 0x000FB187 File Offset: 0x000F9387
	public void RemoveChestEventListener(UIEventType type, UIEvent.Handler handler)
	{
		this.m_ChestClickArea.RemoveEventListener(type, handler);
	}

	// Token: 0x060031EC RID: 12780 RVA: 0x000FB197 File Offset: 0x000F9397
	public void SlamInCheckmark()
	{
		this.ShowCheckmark();
		this.m_EventTable.TriggerState("SlamInCheckmark", true, null);
	}

	// Token: 0x060031ED RID: 12781 RVA: 0x000FB1B1 File Offset: 0x000F93B1
	public void ShowCheckmark()
	{
		this.m_CheckmarkContainer.SetActive(true);
		this.m_ChestContainer.SetActive(false);
	}

	// Token: 0x060031EE RID: 12782 RVA: 0x000FB1CB File Offset: 0x000F93CB
	public void BurstCheckmark()
	{
		this.ShowCheckmark();
		this.m_EventTable.TriggerState("BurstCheckmark", true, null);
	}

	// Token: 0x060031EF RID: 12783 RVA: 0x000FB1E5 File Offset: 0x000F93E5
	public void BlinkChest()
	{
		this.ShowCheckmark();
		this.m_EventTable.TriggerState("BlinkChest", true, null);
	}

	// Token: 0x060031F0 RID: 12784 RVA: 0x000FB1FF File Offset: 0x000F93FF
	public void ShowChest()
	{
		this.m_CheckmarkContainer.SetActive(false);
		this.m_ChestContainer.SetActive(true);
	}

	// Token: 0x060031F1 RID: 12785 RVA: 0x000FB219 File Offset: 0x000F9419
	public void HideAll()
	{
		this.m_CheckmarkContainer.SetActive(false);
		this.m_ChestContainer.SetActive(false);
	}

	// Token: 0x060031F2 RID: 12786 RVA: 0x000FB234 File Offset: 0x000F9434
	public void Enable(bool enable)
	{
		if (this.m_ChestClickArea != null)
		{
			this.m_ChestClickArea.gameObject.SetActive(enable);
		}
	}

	// Token: 0x04001F28 RID: 7976
	private const string s_EventBlinkChest = "BlinkChest";

	// Token: 0x04001F29 RID: 7977
	private const string s_EventOpenChest = "OpenChest";

	// Token: 0x04001F2A RID: 7978
	private const string s_EventSlamInCheckmark = "SlamInCheckmark";

	// Token: 0x04001F2B RID: 7979
	private const string s_EventBurstCheckmark = "BurstCheckmark";

	// Token: 0x04001F2C RID: 7980
	[CustomEditField(Sections = "Event Table")]
	public StateEventTable m_EventTable;

	// Token: 0x04001F2D RID: 7981
	[CustomEditField(Sections = "UI")]
	public PegUIElement m_ChestClickArea;

	// Token: 0x04001F2E RID: 7982
	[CustomEditField(Sections = "UI")]
	public GameObject m_CheckmarkContainer;

	// Token: 0x04001F2F RID: 7983
	[CustomEditField(Sections = "UI")]
	public GameObject m_ChestContainer;
}
