using System;
using UnityEngine;

// Token: 0x020007FF RID: 2047
public class RewardCard : MonoBehaviour
{
	// Token: 0x06004F65 RID: 20325 RVA: 0x001796B4 File Offset: 0x001778B4
	private void OnDestroy()
	{
		this.m_Ready = false;
	}

	// Token: 0x06004F66 RID: 20326 RVA: 0x001796BD File Offset: 0x001778BD
	public bool IsReady()
	{
		return this.m_Ready;
	}

	// Token: 0x06004F67 RID: 20327 RVA: 0x001796C8 File Offset: 0x001778C8
	public void LoadCard(CardRewardData cardData, GameLayer layer = GameLayer.IgnoreFullScreenEffects)
	{
		this.m_layer = layer;
		this.m_CardID = cardData.CardID;
		this.m_premium = cardData.Premium;
		DefLoader.Get().LoadFullDef(this.m_CardID, new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
	}

	// Token: 0x06004F68 RID: 20328 RVA: 0x00179710 File Offset: 0x00177910
	public void Death()
	{
		this.m_actor.ActivateSpell(SpellType.DEATH);
	}

	// Token: 0x06004F69 RID: 20329 RVA: 0x00179720 File Offset: 0x00177920
	private void OnFullDefLoaded(string cardId, FullDef fullDef, object userData)
	{
		if (fullDef == null)
		{
			Debug.LogWarning(string.Format("RewardCard.OnFullDefLoaded() - FAILED to load \"{0}\"", cardId));
			return;
		}
		this.m_entityDef = fullDef.GetEntityDef();
		this.m_cardDef = fullDef.GetCardDef();
		string handActor = ActorNames.GetHandActor(this.m_entityDef, this.m_premium);
		AssetLoader.Get().LoadActor(handActor, new AssetLoader.GameObjectCallback(this.OnActorLoaded), null, false);
	}

	// Token: 0x06004F6A RID: 20330 RVA: 0x00179788 File Offset: 0x00177988
	private void OnActorLoaded(string name, GameObject actorObject, object userData)
	{
		if (actorObject == null)
		{
			Debug.LogWarning(string.Format("RewardCard.OnActorLoaded() - FAILED to load actor \"{0}\"", name));
			return;
		}
		Actor component = actorObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("RewardCard.OnActorLoaded() - ERROR actor \"{0}\" has no Actor component", name));
			return;
		}
		this.m_actor = component;
		this.m_actor.TurnOffCollider();
		SceneUtils.SetLayer(component.gameObject, this.m_layer);
		this.m_actor.SetEntityDef(this.m_entityDef);
		this.m_actor.SetCardDef(this.m_cardDef);
		this.m_actor.SetPremium(this.m_premium);
		this.m_actor.UpdateAllComponents();
		this.m_actor.transform.parent = base.transform;
		this.m_actor.transform.localPosition = Vector3.zero;
		this.m_actor.transform.localEulerAngles = new Vector3(270f, 0f, 0f);
		this.m_actor.transform.localScale = Vector3.one;
		this.m_actor.Show();
		this.m_Ready = true;
	}

	// Token: 0x04003642 RID: 13890
	public string m_CardID = string.Empty;

	// Token: 0x04003643 RID: 13891
	private bool m_Ready;

	// Token: 0x04003644 RID: 13892
	private TAG_PREMIUM m_premium;

	// Token: 0x04003645 RID: 13893
	private EntityDef m_entityDef;

	// Token: 0x04003646 RID: 13894
	private CardDef m_cardDef;

	// Token: 0x04003647 RID: 13895
	private Actor m_actor;

	// Token: 0x04003648 RID: 13896
	private GameLayer m_layer = GameLayer.IgnoreFullScreenEffects;
}
