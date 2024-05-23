using System;
using UnityEngine;

// Token: 0x02000A0E RID: 2574
public class DebugMissionButton : PegUIElement
{
	// Token: 0x06005B52 RID: 23378 RVA: 0x001B3FBC File Offset: 0x001B21BC
	private void Start()
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(this.m_missionId);
		if (record == null)
		{
			Error.AddDevWarning("Error", "scenario {0} does not exist in the DBF", new object[]
			{
				this.m_missionId
			});
			return;
		}
		if (this.m_name != null)
		{
			this.m_name.Text = record.ShortName;
		}
		int num = record.ClientPlayer2HeroCardId;
		if (num == 0)
		{
			num = record.Player2HeroCardId;
		}
		string text = GameUtils.TranslateDbIdToCardId(num);
		if (text == null)
		{
			return;
		}
		DefLoader.Get().LoadCardDef(text, new DefLoader.LoadDefCallback<CardDef>(this.OnCardDefLoaded), null, null);
	}

	// Token: 0x06005B53 RID: 23379 RVA: 0x001B4068 File Offset: 0x001B2268
	private void OnCardDefLoaded(string cardID, CardDef cardDef, object userData)
	{
		this.m_heroImage.GetComponent<Renderer>().material.mainTexture = cardDef.GetPortraitTexture();
	}

	// Token: 0x06005B54 RID: 23380 RVA: 0x001B4090 File Offset: 0x001B2290
	protected override void OnRelease()
	{
		if (!string.IsNullOrEmpty(this.m_introline))
		{
			if (string.IsNullOrEmpty(this.m_characterPrefabName))
			{
				NotificationManager.Get().CreateKTQuote(this.m_introline, this.m_introline, true);
			}
			else
			{
				NotificationManager.Get().CreateCharacterQuote(this.m_characterPrefabName, GameStrings.Get(this.m_introline), this.m_introline, true, 0f, CanvasAnchor.BOTTOM_LEFT);
			}
		}
		base.OnRelease();
		long selectedDeckID = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
		GameMgr.Get().FindGame(1, this.m_missionId, selectedDeckID, 0L);
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06005B55 RID: 23381 RVA: 0x001B4134 File Offset: 0x001B2334
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		this.m_mousedOver = true;
		base.OnOver(oldState);
		string missionHeroPowerCardId = GameUtils.GetMissionHeroPowerCardId(this.m_missionId);
		if (string.IsNullOrEmpty(missionHeroPowerCardId))
		{
			return;
		}
		DefLoader.Get().LoadFullDef(GameUtils.GetMissionHeroPowerCardId(this.m_missionId), new DefLoader.LoadDefCallback<FullDef>(this.OnHeroPowerDefLoaded));
	}

	// Token: 0x06005B56 RID: 23382 RVA: 0x001B4188 File Offset: 0x001B2388
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		this.m_mousedOver = false;
		base.OnOut(oldState);
		if (this.m_heroPowerActor)
		{
			Object.Destroy(this.m_heroPowerActor.gameObject);
		}
	}

	// Token: 0x06005B57 RID: 23383 RVA: 0x001B41C3 File Offset: 0x001B23C3
	private void OnHeroPowerDefLoaded(string cardID, FullDef def, object userData)
	{
		this.m_heroPowerDef = def;
		if (!this.m_mousedOver)
		{
			return;
		}
		AssetLoader.Get().LoadActor("History_HeroPower_Opponent", new AssetLoader.GameObjectCallback(this.OnHeroPowerActorLoaded), null, false);
	}

	// Token: 0x06005B58 RID: 23384 RVA: 0x001B41F8 File Offset: 0x001B23F8
	private void OnHeroPowerActorLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		if (!this.m_mousedOver)
		{
			Object.Destroy(actorObject);
		}
		if (this.m_heroPowerActor)
		{
			Object.Destroy(this.m_heroPowerActor.gameObject);
		}
		if (this == null || base.gameObject == null)
		{
			Object.Destroy(actorObject);
		}
		if (actorObject == null)
		{
			return;
		}
		this.m_heroPowerActor = actorObject.GetComponent<Actor>();
		actorObject.transform.parent = base.gameObject.transform;
		this.m_heroPowerActor.SetCardDef(this.m_heroPowerDef.GetCardDef());
		this.m_heroPowerActor.SetEntityDef(this.m_heroPowerDef.GetEntityDef());
		this.m_heroPowerActor.UpdateAllComponents();
		actorObject.transform.position = base.transform.position + new Vector3(15f, 0f, 0f);
		actorObject.transform.localScale = Vector3.one;
		iTween.ScaleTo(actorObject, new Vector3(7f, 7f, 7f), 0.5f);
		SceneUtils.SetLayer(actorObject, GameLayer.Tooltip);
	}

	// Token: 0x040042D9 RID: 17113
	public int m_missionId;

	// Token: 0x040042DA RID: 17114
	public GameObject m_heroImage;

	// Token: 0x040042DB RID: 17115
	public UberText m_name;

	// Token: 0x040042DC RID: 17116
	public string m_introline;

	// Token: 0x040042DD RID: 17117
	public string m_characterPrefabName;

	// Token: 0x040042DE RID: 17118
	private GameObject m_heroPowerObject;

	// Token: 0x040042DF RID: 17119
	private bool m_mousedOver;

	// Token: 0x040042E0 RID: 17120
	private FullDef m_heroPowerDef;

	// Token: 0x040042E1 RID: 17121
	private Actor m_heroPowerActor;
}
