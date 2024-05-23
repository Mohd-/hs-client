using System;
using UnityEngine;

// Token: 0x02000A0D RID: 2573
public class Debug1v1Button : PegUIElement
{
	// Token: 0x17000813 RID: 2067
	// (get) Token: 0x06005B4C RID: 23372 RVA: 0x001B3ED4 File Offset: 0x001B20D4
	// (set) Token: 0x06005B4D RID: 23373 RVA: 0x001B3EDB File Offset: 0x001B20DB
	public static bool HasUsedDebugMenu { get; set; }

	// Token: 0x06005B4E RID: 23374 RVA: 0x001B3EE4 File Offset: 0x001B20E4
	private void Start()
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(this.m_missionId);
		if (record != null)
		{
			string text = record.ShortName;
			if (this.m_name != null && !string.IsNullOrEmpty(text))
			{
				this.m_name.Text = text;
			}
		}
	}

	// Token: 0x06005B4F RID: 23375 RVA: 0x001B3F3C File Offset: 0x001B213C
	private void OnCardDefLoaded(string cardID, CardDef cardDef, object userData)
	{
		this.m_heroImage.GetComponent<Renderer>().material.mainTexture = cardDef.GetPortraitTexture();
	}

	// Token: 0x06005B50 RID: 23376 RVA: 0x001B3F64 File Offset: 0x001B2164
	protected override void OnRelease()
	{
		base.OnRelease();
		long selectedDeckID = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
		Debug1v1Button.HasUsedDebugMenu = true;
		GameMgr.Get().FindGame(16, this.m_missionId, selectedDeckID, 0L);
		Object.Destroy(base.transform.parent.gameObject);
	}

	// Token: 0x040042D4 RID: 17108
	public int m_missionId;

	// Token: 0x040042D5 RID: 17109
	public GameObject m_heroImage;

	// Token: 0x040042D6 RID: 17110
	public UberText m_name;

	// Token: 0x040042D7 RID: 17111
	private GameObject m_heroPowerObject;
}
