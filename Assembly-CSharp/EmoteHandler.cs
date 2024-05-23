using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000851 RID: 2129
public class EmoteHandler : MonoBehaviour
{
	// Token: 0x060051FC RID: 20988 RVA: 0x00187C61 File Offset: 0x00185E61
	private void Awake()
	{
		EmoteHandler.s_instance = this;
		base.GetComponent<Collider>().enabled = false;
	}

	// Token: 0x060051FD RID: 20989 RVA: 0x00187C75 File Offset: 0x00185E75
	private void Start()
	{
		GameState.Get().RegisterHeroChangedListener(new GameState.HeroChangedCallback(this.OnHeroChanged), null);
	}

	// Token: 0x060051FE RID: 20990 RVA: 0x00187C8F File Offset: 0x00185E8F
	private void OnDestroy()
	{
		EmoteHandler.s_instance = null;
	}

	// Token: 0x060051FF RID: 20991 RVA: 0x00187C97 File Offset: 0x00185E97
	private void Update()
	{
		this.m_timeSinceLastEmote += Time.unscaledDeltaTime;
	}

	// Token: 0x06005200 RID: 20992 RVA: 0x00187CAB File Offset: 0x00185EAB
	public static EmoteHandler Get()
	{
		return EmoteHandler.s_instance;
	}

	// Token: 0x06005201 RID: 20993 RVA: 0x00187CB2 File Offset: 0x00185EB2
	public void ResetTimeSinceLastEmote()
	{
		if (this.m_timeSinceLastEmote < 9f)
		{
			this.m_chainedEmotes++;
		}
		else
		{
			this.m_chainedEmotes = 0;
		}
		this.m_timeSinceLastEmote = 0f;
	}

	// Token: 0x06005202 RID: 20994 RVA: 0x00187CEC File Offset: 0x00185EEC
	public void ShowEmotes()
	{
		if (this.m_emotesShown)
		{
			return;
		}
		this.m_shownAtFrame = Time.frameCount;
		this.m_emotesShown = true;
		base.GetComponent<Collider>().enabled = true;
		foreach (EmoteOption emoteOption in this.m_Emotes)
		{
			emoteOption.Enable();
		}
	}

	// Token: 0x06005203 RID: 20995 RVA: 0x00187D70 File Offset: 0x00185F70
	public void HideEmotes()
	{
		if (!this.m_emotesShown)
		{
			return;
		}
		this.m_mousedOverEmote = null;
		this.m_emotesShown = false;
		base.GetComponent<Collider>().enabled = false;
		foreach (EmoteOption emoteOption in this.m_Emotes)
		{
			emoteOption.Disable();
		}
	}

	// Token: 0x06005204 RID: 20996 RVA: 0x00187DF0 File Offset: 0x00185FF0
	public bool AreEmotesActive()
	{
		return this.m_emotesShown;
	}

	// Token: 0x06005205 RID: 20997 RVA: 0x00187DF8 File Offset: 0x00185FF8
	public void HandleInput()
	{
		RaycastHit raycastHit;
		if (!this.HitTestEmotes(out raycastHit))
		{
			this.HideEmotes();
			return;
		}
		EmoteOption component = raycastHit.transform.gameObject.GetComponent<EmoteOption>();
		if (component == null)
		{
			if (this.m_mousedOverEmote != null)
			{
				this.m_mousedOverEmote.HandleMouseOut();
				this.m_mousedOverEmote = null;
			}
		}
		else if (this.m_mousedOverEmote == null)
		{
			this.m_mousedOverEmote = component;
			this.m_mousedOverEmote.HandleMouseOver();
		}
		else if (this.m_mousedOverEmote != component)
		{
			this.m_mousedOverEmote.HandleMouseOut();
			this.m_mousedOverEmote = component;
			component.HandleMouseOver();
		}
		if (UniversalInputManager.Get().GetMouseButtonUp(0))
		{
			if (this.m_mousedOverEmote != null)
			{
				if (!this.EmoteSpamBlocked())
				{
					this.m_totalEmotes++;
					this.m_mousedOverEmote.DoClick();
				}
			}
			else if (UniversalInputManager.Get().IsTouchMode() && Time.frameCount != this.m_shownAtFrame)
			{
				this.HideEmotes();
			}
		}
	}

	// Token: 0x06005206 RID: 20998 RVA: 0x00187F20 File Offset: 0x00186120
	public bool IsMouseOverEmoteOption()
	{
		RaycastHit raycastHit;
		if (UniversalInputManager.Get().GetInputHitInfo(GameLayer.Default.LayerBit(), out raycastHit))
		{
			EmoteOption component = raycastHit.transform.gameObject.GetComponent<EmoteOption>();
			if (component != null)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06005207 RID: 20999 RVA: 0x00187F6C File Offset: 0x0018616C
	public bool EmoteSpamBlocked()
	{
		if (GameMgr.Get().IsFriendly() || GameMgr.Get().IsAI())
		{
			return false;
		}
		if (this.m_totalEmotes >= 25)
		{
			return this.m_timeSinceLastEmote < 45f;
		}
		if (this.m_totalEmotes >= 20)
		{
			return this.m_timeSinceLastEmote < 15f;
		}
		if (this.m_chainedEmotes >= 2)
		{
			return this.m_timeSinceLastEmote < 15f;
		}
		return this.m_timeSinceLastEmote < 4f;
	}

	// Token: 0x06005208 RID: 21000 RVA: 0x00187FF8 File Offset: 0x001861F8
	private void OnHeroChanged(Player player, object userData)
	{
		if (!player.IsFriendlySide())
		{
			return;
		}
		foreach (EmoteOption emoteOption in this.m_Emotes)
		{
			emoteOption.UpdateEmoteType();
		}
	}

	// Token: 0x06005209 RID: 21001 RVA: 0x00188060 File Offset: 0x00186260
	private bool HitTestEmotes(out RaycastHit hitInfo)
	{
		return UniversalInputManager.Get().GetInputHitInfo(GameLayer.CardRaycast.LayerBit(), out hitInfo) && (this.IsMousedOverHero(hitInfo) || this.IsMousedOverSelf(hitInfo) || this.IsMousedOverEmote(hitInfo));
	}

	// Token: 0x0600520A RID: 21002 RVA: 0x001880C4 File Offset: 0x001862C4
	private bool IsMousedOverHero(RaycastHit cardHitInfo)
	{
		Actor actor = SceneUtils.FindComponentInParents<Actor>(cardHitInfo.transform);
		if (actor == null)
		{
			return false;
		}
		Card card = actor.GetCard();
		return !(card == null) && card.GetEntity().IsHero();
	}

	// Token: 0x0600520B RID: 21003 RVA: 0x00188114 File Offset: 0x00186314
	private bool IsMousedOverSelf(RaycastHit cardHitInfo)
	{
		return base.GetComponent<Collider>() == cardHitInfo.collider;
	}

	// Token: 0x0600520C RID: 21004 RVA: 0x00188128 File Offset: 0x00186328
	private bool IsMousedOverEmote(RaycastHit cardHitInfo)
	{
		foreach (EmoteOption emoteOption in this.m_Emotes)
		{
			if (cardHitInfo.transform == emoteOption.transform)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04003852 RID: 14418
	private const float MIN_TIME_BETWEEN_EMOTES = 4f;

	// Token: 0x04003853 RID: 14419
	private const float TIME_WINDOW_TO_BE_CONSIDERED_A_CHAIN = 5f;

	// Token: 0x04003854 RID: 14420
	private const float SPAMMER_MIN_TIME_BETWEEN_EMOTES = 15f;

	// Token: 0x04003855 RID: 14421
	private const float UBER_SPAMMER_MIN_TIME_BETWEEN_EMOTES = 45f;

	// Token: 0x04003856 RID: 14422
	private const int NUM_EMOTES_BEFORE_CONSIDERED_A_SPAMMER = 20;

	// Token: 0x04003857 RID: 14423
	private const int NUM_EMOTES_BEFORE_CONSIDERED_UBER_SPAMMER = 25;

	// Token: 0x04003858 RID: 14424
	private const int NUM_CHAIN_EMOTES_BEFORE_CONSIDERED_SPAM = 2;

	// Token: 0x04003859 RID: 14425
	public List<EmoteOption> m_Emotes;

	// Token: 0x0400385A RID: 14426
	private static EmoteHandler s_instance;

	// Token: 0x0400385B RID: 14427
	private bool m_emotesShown;

	// Token: 0x0400385C RID: 14428
	private int m_shownAtFrame;

	// Token: 0x0400385D RID: 14429
	private EmoteOption m_mousedOverEmote;

	// Token: 0x0400385E RID: 14430
	private float m_timeSinceLastEmote = 4f;

	// Token: 0x0400385F RID: 14431
	private int m_totalEmotes;

	// Token: 0x04003860 RID: 14432
	private int m_chainedEmotes;
}
