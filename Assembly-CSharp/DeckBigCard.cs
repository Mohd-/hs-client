using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000722 RID: 1826
public class DeckBigCard : MonoBehaviour
{
	// Token: 0x06004AB2 RID: 19122 RVA: 0x00165C59 File Offset: 0x00163E59
	private void Awake()
	{
		this.m_firstShowFrame = 0;
	}

	// Token: 0x06004AB3 RID: 19123 RVA: 0x00165C64 File Offset: 0x00163E64
	public void Show(EntityDef entityDef, TAG_PREMIUM premium, CardDef cardDef, Vector3 sourcePosition, GhostCard.Type ghosted, float delay = 0f)
	{
		bool flag = false;
		if (flag)
		{
			int frameCount = Time.frameCount;
			if (this.m_firstShowFrame == 0)
			{
				this.m_firstShowFrame = frameCount;
			}
			else if (frameCount - this.m_firstShowFrame <= 1)
			{
				return;
			}
		}
		base.StopCoroutine("ShowWithDelayInternal");
		this.m_shown = true;
		this.m_entityDef = entityDef;
		this.m_premium = premium;
		this.m_cardDef = cardDef;
		this.m_ghosted = ghosted;
		if (delay > 0f)
		{
			KeyValuePair<float, Action> keyValuePair = new KeyValuePair<float, Action>(delay, delegate()
			{
				this.Show(entityDef, premium, cardDef, sourcePosition, ghosted, 0f);
			});
			base.StartCoroutine("ShowWithDelayInternal", keyValuePair);
			return;
		}
		if (!UniversalInputManager.UsePhoneUI)
		{
			float z = this.m_bottomPosition.transform.position.z;
			float z2 = this.m_topPosition.transform.position.z;
			float z3 = Mathf.Clamp(sourcePosition.z, z, z2);
			TransformUtil.SetPosZ(base.transform, z3);
		}
		if (!this.m_actorCacheInit)
		{
			this.m_actorCacheInit = true;
			this.m_actorCache.AddActorLoadedListener(new HandActorCache.ActorLoadedCallback(this.OnActorLoaded));
			this.m_actorCache.Initialize();
		}
		if (this.m_actorCache.IsInitializing())
		{
			return;
		}
		this.Show();
	}

	// Token: 0x06004AB4 RID: 19124 RVA: 0x00165E0C File Offset: 0x0016400C
	public void Hide(EntityDef entityDef, TAG_PREMIUM premium)
	{
		if (this.m_entityDef != entityDef)
		{
			return;
		}
		if (this.m_premium != premium)
		{
			return;
		}
		this.Hide();
	}

	// Token: 0x06004AB5 RID: 19125 RVA: 0x00165E39 File Offset: 0x00164039
	public void ForceHide()
	{
		this.Hide();
	}

	// Token: 0x06004AB6 RID: 19126 RVA: 0x00165E44 File Offset: 0x00164044
	private IEnumerator ShowWithDelayInternal(KeyValuePair<float, Action> args)
	{
		yield return new WaitForSeconds(args.Key);
		args.Value.Invoke();
		yield break;
	}

	// Token: 0x06004AB7 RID: 19127 RVA: 0x00165E68 File Offset: 0x00164068
	private void OnActorLoaded(string name, Actor actor, object callbackData)
	{
		if (actor == null)
		{
			Debug.LogWarning(string.Format("DeckBigCard.OnActorLoaded() - FAILED to load {0}", name));
			return;
		}
		actor.TurnOffCollider();
		actor.Hide();
		actor.transform.parent = base.transform;
		TransformUtil.Identity(actor.transform);
		SceneUtils.SetLayer(actor, base.gameObject.layer);
		if (!this.m_actorCache.IsInitializing() && this.m_shown)
		{
			this.Show();
		}
	}

	// Token: 0x06004AB8 RID: 19128 RVA: 0x00165EEC File Offset: 0x001640EC
	private void Show()
	{
		this.m_shownActor = this.m_actorCache.GetActor(this.m_entityDef, this.m_premium);
		if (this.m_shownActor == null)
		{
			return;
		}
		this.m_shownActor.SetEntityDef(this.m_entityDef);
		this.m_shownActor.SetPremium(this.m_premium);
		this.m_cardDef = DefLoader.Get().GetCardDef(this.m_entityDef.GetCardId(), new CardPortraitQuality(3, this.m_premium));
		this.m_shownActor.SetCardDef(this.m_cardDef);
		this.m_shownActor.GhostCardEffect(this.m_ghosted);
		if (this.m_shownActor.isGhostCard())
		{
			GhostCard component = this.m_shownActor.m_ghostCardGameObject.GetComponent<GhostCard>();
			component.SetRenderQueue(70);
			component.SetBigCard(true);
		}
		this.m_shownActor.UpdateAllComponents();
		if (this.m_missingCardMaterial != null)
		{
			this.m_shownActor.SetMissingCardMaterial(this.m_missingCardMaterial);
		}
		this.m_shownActor.Show();
	}

	// Token: 0x06004AB9 RID: 19129 RVA: 0x00165FFC File Offset: 0x001641FC
	private void Hide()
	{
		base.StopCoroutine("ShowWithDelayInternal");
		this.m_shown = false;
		if (this.m_shownActor == null)
		{
			return;
		}
		this.m_shownActor.Hide();
		this.m_shownActor = null;
	}

	// Token: 0x040031B4 RID: 12724
	public GameObject m_topPosition;

	// Token: 0x040031B5 RID: 12725
	public GameObject m_bottomPosition;

	// Token: 0x040031B6 RID: 12726
	public Material m_missingCardMaterial;

	// Token: 0x040031B7 RID: 12727
	public Material m_ghostCardMaterial;

	// Token: 0x040031B8 RID: 12728
	public Material m_invalidCardMaterial;

	// Token: 0x040031B9 RID: 12729
	private HandActorCache m_actorCache = new HandActorCache();

	// Token: 0x040031BA RID: 12730
	private bool m_actorCacheInit;

	// Token: 0x040031BB RID: 12731
	private bool m_shown;

	// Token: 0x040031BC RID: 12732
	private EntityDef m_entityDef;

	// Token: 0x040031BD RID: 12733
	private TAG_PREMIUM m_premium;

	// Token: 0x040031BE RID: 12734
	private CardDef m_cardDef;

	// Token: 0x040031BF RID: 12735
	private Actor m_shownActor;

	// Token: 0x040031C0 RID: 12736
	private GhostCard.Type m_ghosted;

	// Token: 0x040031C1 RID: 12737
	private int m_firstShowFrame;
}
