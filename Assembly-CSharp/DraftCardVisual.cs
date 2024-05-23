using System;
using UnityEngine;

// Token: 0x0200080A RID: 2058
public class DraftCardVisual : PegUIElement
{
	// Token: 0x06004FA6 RID: 20390 RVA: 0x0017A50F File Offset: 0x0017870F
	public void SetActor(Actor actor)
	{
		this.m_actor = actor;
	}

	// Token: 0x06004FA7 RID: 20391 RVA: 0x0017A518 File Offset: 0x00178718
	public Actor GetActor()
	{
		return this.m_actor;
	}

	// Token: 0x06004FA8 RID: 20392 RVA: 0x0017A520 File Offset: 0x00178720
	public void SetChoiceNum(int num)
	{
		this.m_cardChoice = num;
	}

	// Token: 0x06004FA9 RID: 20393 RVA: 0x0017A529 File Offset: 0x00178729
	public int GetChoiceNum()
	{
		return this.m_cardChoice;
	}

	// Token: 0x06004FAA RID: 20394 RVA: 0x0017A534 File Offset: 0x00178734
	public void ChooseThisCard()
	{
		if (GameUtils.IsAnyTransitionActive())
		{
			return;
		}
		Log.Arena.Print(string.Format("Client chooses: {0} ({1})", this.m_actor.GetEntityDef().GetName(), this.m_actor.GetEntityDef().GetCardId()), new object[0]);
		if (this.m_actor.GetEntityDef().IsHero())
		{
			DraftDisplay.Get().OnHeroClicked(this.m_cardChoice);
		}
		else
		{
			this.m_chosen = true;
			DraftManager.Get().MakeChoice(this.m_cardChoice);
		}
	}

	// Token: 0x06004FAB RID: 20395 RVA: 0x0017A5C7 File Offset: 0x001787C7
	public bool IsChosen()
	{
		return this.m_chosen;
	}

	// Token: 0x06004FAC RID: 20396 RVA: 0x0017A5CF File Offset: 0x001787CF
	public void SetChosenFlag(bool bOn)
	{
		this.m_chosen = bOn;
	}

	// Token: 0x06004FAD RID: 20397 RVA: 0x0017A5D8 File Offset: 0x001787D8
	protected override void OnPress()
	{
		this.m_mouseOverTimer = Time.realtimeSinceStartup;
	}

	// Token: 0x06004FAE RID: 20398 RVA: 0x0017A5E8 File Offset: 0x001787E8
	protected override void OnRelease()
	{
		if (UniversalInputManager.Get().IsTouchMode())
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup - this.m_mouseOverTimer >= 0.4f)
			{
				return;
			}
		}
		this.ChooseThisCard();
	}

	// Token: 0x06004FAF RID: 20399 RVA: 0x0017A624 File Offset: 0x00178824
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		if (this.m_actor.GetEntityDef().IsHero())
		{
			SoundManager.Get().LoadAndPlay("collection_manager_hero_mouse_over");
		}
		else
		{
			SoundManager.Get().LoadAndPlay("collection_manager_card_mouse_over");
		}
		this.m_actor.SetActorState(ActorStateType.CARD_MOUSE_OVER);
		KeywordHelpPanelManager.Get().UpdateKeywordHelpForForge(this.m_actor.GetEntityDef(), this.m_actor, this.m_cardChoice);
	}

	// Token: 0x06004FB0 RID: 20400 RVA: 0x0017A696 File Offset: 0x00178896
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
		KeywordHelpPanelManager.Get().HideKeywordHelp();
	}

	// Token: 0x0400367B RID: 13947
	private const float MOUSE_OVER_DELAY = 0.4f;

	// Token: 0x0400367C RID: 13948
	private Actor m_actor;

	// Token: 0x0400367D RID: 13949
	private int m_cardChoice = -1;

	// Token: 0x0400367E RID: 13950
	private bool m_chosen;

	// Token: 0x0400367F RID: 13951
	private float m_mouseOverTimer;
}
