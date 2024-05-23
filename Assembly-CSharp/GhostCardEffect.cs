using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E67 RID: 3687
public class GhostCardEffect : Spell
{
	// Token: 0x06006FCC RID: 28620 RVA: 0x0020D45C File Offset: 0x0020B65C
	protected override void OnBirth(SpellStateType prevStateType)
	{
		if (this.m_Glow != null)
		{
			this.m_Glow.GetComponent<Renderer>().enabled = false;
		}
		if (this.m_GlowUnique != null)
		{
			this.m_GlowUnique.GetComponent<Renderer>().enabled = false;
		}
		base.StartCoroutine(this.GhostEffect(prevStateType));
	}

	// Token: 0x06006FCD RID: 28621 RVA: 0x0020D4BC File Offset: 0x0020B6BC
	protected override void OnDeath(SpellStateType prevStateType)
	{
		if (this.m_Glow != null)
		{
			this.m_Glow.GetComponent<Renderer>().enabled = false;
		}
		if (this.m_GlowUnique != null)
		{
			this.m_GlowUnique.GetComponent<Renderer>().enabled = false;
		}
		base.OnDeath(prevStateType);
		this.OnSpellFinished();
	}

	// Token: 0x06006FCE RID: 28622 RVA: 0x0020D51C File Offset: 0x0020B71C
	private IEnumerator GhostEffect(SpellStateType prevStateType)
	{
		Actor actor = SceneUtils.FindComponentInParents<Actor>(base.gameObject);
		if (actor == null)
		{
			Debug.LogWarning("GhostCardEffect actor is null");
			yield break;
		}
		GhostCard ghostCard = base.gameObject.GetComponentInChildren<GhostCard>();
		if (ghostCard == null)
		{
			Debug.LogWarning("GhostCardEffect GhostCard is null");
			yield break;
		}
		if (this.m_Glow != null)
		{
			GameObject glow = this.m_Glow;
			if (actor.IsElite() && this.m_GlowUnique != null)
			{
				glow = this.m_GlowUnique;
			}
			glow.GetComponent<Renderer>().enabled = true;
		}
		KeywordHelpPanelManager.Get().HideKeywordHelp();
		ghostCard.RenderGhostCard();
		yield return new WaitForEndOfFrame();
		RenderToTexture r2t = base.gameObject.GetComponentInChildren<RenderToTexture>();
		if (r2t)
		{
			if (GraphicsManager.Get().RenderQualityLevel == GraphicsQuality.High && actor.GetPremium() == TAG_PREMIUM.GOLDEN)
			{
				r2t.m_RealtimeRender = true;
			}
			else
			{
				r2t.m_RealtimeRender = false;
			}
			r2t.m_LateUpdate = true;
		}
		ghostCard.RenderGhostCard(true);
		actor.Show();
		KeywordHelpPanelManager.Get().HideKeywordHelp();
		r2t.Render();
		base.OnBirth(prevStateType);
		this.OnSpellFinished();
		yield break;
	}

	// Token: 0x040058F8 RID: 22776
	public GameObject m_Glow;

	// Token: 0x040058F9 RID: 22777
	public GameObject m_GlowUnique;
}
