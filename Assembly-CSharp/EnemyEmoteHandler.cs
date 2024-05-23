using System;
using UnityEngine;

// Token: 0x02000852 RID: 2130
public class EnemyEmoteHandler : MonoBehaviour
{
	// Token: 0x0600520E RID: 21006 RVA: 0x001881A4 File Offset: 0x001863A4
	private void Awake()
	{
		EnemyEmoteHandler.s_instance = this;
		base.GetComponent<Collider>().enabled = false;
		this.m_squelchEmoteStartingScale = this.m_SquelchEmote.transform.localScale;
		this.m_SquelchEmoteText.gameObject.SetActive(false);
		this.m_SquelchEmoteBackplate.enabled = false;
		this.m_SquelchEmote.transform.localScale = Vector3.zero;
	}

	// Token: 0x0600520F RID: 21007 RVA: 0x0018820B File Offset: 0x0018640B
	private void OnDestroy()
	{
		EnemyEmoteHandler.s_instance = null;
	}

	// Token: 0x06005210 RID: 21008 RVA: 0x00188213 File Offset: 0x00186413
	public static EnemyEmoteHandler Get()
	{
		return EnemyEmoteHandler.s_instance;
	}

	// Token: 0x06005211 RID: 21009 RVA: 0x0018821A File Offset: 0x0018641A
	public bool AreEmotesActive()
	{
		return this.m_emotesShown;
	}

	// Token: 0x06005212 RID: 21010 RVA: 0x00188222 File Offset: 0x00186422
	public bool IsSquelched()
	{
		return this.m_squelched;
	}

	// Token: 0x06005213 RID: 21011 RVA: 0x0018822C File Offset: 0x0018642C
	public void ShowEmotes()
	{
		if (this.m_emotesShown)
		{
			return;
		}
		this.m_emotesShown = true;
		base.GetComponent<Collider>().enabled = true;
		this.m_shownAtFrame = Time.frameCount;
		if (this.m_squelched)
		{
			this.m_SquelchEmoteText.Text = GameStrings.Get(this.m_UnsquelchStringTag);
		}
		else
		{
			this.m_SquelchEmoteText.Text = GameStrings.Get(this.m_SquelchStringTag);
		}
		this.m_SquelchEmoteBackplate.enabled = true;
		this.m_SquelchEmoteText.gameObject.SetActive(true);
		this.m_SquelchEmote.GetComponent<Collider>().enabled = true;
		iTween.Stop(this.m_SquelchEmote);
		iTween.ScaleTo(this.m_SquelchEmote, iTween.Hash(new object[]
		{
			"scale",
			this.m_squelchEmoteStartingScale,
			"time",
			0.5f,
			"ignoretimescale",
			true,
			"easetype",
			iTween.EaseType.easeOutElastic
		}));
	}

	// Token: 0x06005214 RID: 21012 RVA: 0x0018833C File Offset: 0x0018653C
	public void HideEmotes()
	{
		if (!this.m_emotesShown)
		{
			return;
		}
		this.m_emotesShown = false;
		base.GetComponent<Collider>().enabled = false;
		this.m_SquelchEmote.GetComponent<Collider>().enabled = false;
		iTween.Stop(this.m_SquelchEmote);
		iTween.ScaleTo(this.m_SquelchEmote, iTween.Hash(new object[]
		{
			"scale",
			Vector3.zero,
			"time",
			0.1f,
			"ignoretimescale",
			true,
			"easetype",
			iTween.EaseType.linear,
			"oncompletetarget",
			base.gameObject,
			"oncomplete",
			"FinishDisable"
		}));
	}

	// Token: 0x06005215 RID: 21013 RVA: 0x0018840C File Offset: 0x0018660C
	public void HandleInput()
	{
		RaycastHit raycastHit;
		if (!this.HitTestEmotes(out raycastHit))
		{
			this.HideEmotes();
			return;
		}
		GameObject gameObject = raycastHit.transform.gameObject;
		if (gameObject != this.m_SquelchEmote)
		{
			if (this.m_squelchMousedOver)
			{
				this.MouseOutSquelch();
				this.m_squelchMousedOver = false;
			}
		}
		else if (!this.m_squelchMousedOver)
		{
			this.m_squelchMousedOver = true;
			this.MouseOverSquelch();
		}
		if (UniversalInputManager.Get().GetMouseButtonUp(0))
		{
			if (this.m_squelchMousedOver)
			{
				this.DoSquelchClick();
			}
			else if (UniversalInputManager.Get().IsTouchMode() && Time.frameCount != this.m_shownAtFrame)
			{
				this.HideEmotes();
			}
		}
	}

	// Token: 0x06005216 RID: 21014 RVA: 0x001884CC File Offset: 0x001866CC
	public bool IsMouseOverEmoteOption()
	{
		RaycastHit raycastHit;
		return UniversalInputManager.Get().GetInputHitInfo(GameLayer.Default.LayerBit(), out raycastHit) && raycastHit.transform.gameObject == this.m_SquelchEmote;
	}

	// Token: 0x06005217 RID: 21015 RVA: 0x00188514 File Offset: 0x00186714
	private void MouseOverSquelch()
	{
		iTween.ScaleTo(this.m_SquelchEmote, iTween.Hash(new object[]
		{
			"scale",
			this.m_squelchEmoteStartingScale * 1.1f,
			"time",
			0.2f,
			"ignoretimescale",
			true
		}));
	}

	// Token: 0x06005218 RID: 21016 RVA: 0x00188580 File Offset: 0x00186780
	private void MouseOutSquelch()
	{
		iTween.ScaleTo(this.m_SquelchEmote, iTween.Hash(new object[]
		{
			"scale",
			this.m_squelchEmoteStartingScale,
			"time",
			0.2f,
			"ignoretimescale",
			true
		}));
	}

	// Token: 0x06005219 RID: 21017 RVA: 0x001885DF File Offset: 0x001867DF
	private void DoSquelchClick()
	{
		this.m_squelched = !this.m_squelched;
		this.HideEmotes();
	}

	// Token: 0x0600521A RID: 21018 RVA: 0x001885F8 File Offset: 0x001867F8
	private bool HitTestEmotes(out RaycastHit hitInfo)
	{
		return UniversalInputManager.Get().GetInputHitInfo(GameLayer.CardRaycast.LayerBit(), out hitInfo) && (this.IsMousedOverHero(hitInfo) || this.IsMousedOverSelf(hitInfo) || this.IsMousedOverEmote(hitInfo));
	}

	// Token: 0x0600521B RID: 21019 RVA: 0x0018865C File Offset: 0x0018685C
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

	// Token: 0x0600521C RID: 21020 RVA: 0x001886AC File Offset: 0x001868AC
	private bool IsMousedOverSelf(RaycastHit cardHitInfo)
	{
		return base.GetComponent<Collider>() == cardHitInfo.collider;
	}

	// Token: 0x0600521D RID: 21021 RVA: 0x001886C0 File Offset: 0x001868C0
	private bool IsMousedOverEmote(RaycastHit cardHitInfo)
	{
		return cardHitInfo.transform == this.m_SquelchEmote.transform;
	}

	// Token: 0x04003861 RID: 14433
	public GameObject m_SquelchEmote;

	// Token: 0x04003862 RID: 14434
	public MeshRenderer m_SquelchEmoteBackplate;

	// Token: 0x04003863 RID: 14435
	public UberText m_SquelchEmoteText;

	// Token: 0x04003864 RID: 14436
	public string m_SquelchStringTag;

	// Token: 0x04003865 RID: 14437
	public string m_UnsquelchStringTag;

	// Token: 0x04003866 RID: 14438
	private static EnemyEmoteHandler s_instance;

	// Token: 0x04003867 RID: 14439
	private Vector3 m_squelchEmoteStartingScale;

	// Token: 0x04003868 RID: 14440
	private bool m_emotesShown;

	// Token: 0x04003869 RID: 14441
	private int m_shownAtFrame;

	// Token: 0x0400386A RID: 14442
	private bool m_squelchMousedOver;

	// Token: 0x0400386B RID: 14443
	private bool m_squelched;
}
