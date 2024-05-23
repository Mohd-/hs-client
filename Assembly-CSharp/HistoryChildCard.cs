using System;
using UnityEngine;

// Token: 0x0200033E RID: 830
public class HistoryChildCard : HistoryItem
{
	// Token: 0x06002B73 RID: 11123 RVA: 0x000D88BC File Offset: 0x000D6ABC
	public void SetCardInfo(Entity entity, Texture portraitTexture, Material portraitGoldenMaterial, int splatAmount, bool isDead)
	{
		this.m_entity = entity;
		this.m_portraitTexture = portraitTexture;
		this.m_portraitGoldenMaterial = portraitGoldenMaterial;
		this.m_splatAmount = splatAmount;
		this.m_dead = isDead;
	}

	// Token: 0x06002B74 RID: 11124 RVA: 0x000D88E4 File Offset: 0x000D6AE4
	public void LoadMainCardActor()
	{
		string historyActor = ActorNames.GetHistoryActor(this.m_entity);
		GameObject gameObject = AssetLoader.Get().LoadActor(historyActor, false, false);
		if (gameObject == null)
		{
			Debug.LogWarningFormat("HistoryChildCard.LoadActorCallback() - FAILED to load actor \"{0}\"", new object[]
			{
				historyActor
			});
			return;
		}
		Actor component = gameObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarningFormat("HistoryChildCard.LoadActorCallback() - ERROR actor \"{0}\" has no Actor component", new object[]
			{
				historyActor
			});
			return;
		}
		this.m_mainCardActor = component;
		this.m_mainCardActor.SetPremium(this.m_entity.GetPremiumType());
		this.m_mainCardActor.SetHistoryItem(this);
		this.m_mainCardActor.UpdateAllComponents();
		SceneUtils.SetLayer(this.m_mainCardActor.gameObject, GameLayer.Tooltip);
		this.m_mainCardActor.Hide();
	}
}
