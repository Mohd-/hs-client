using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E0D RID: 3597
public class CardSpecificMultiVoSpell : CardSoundSpell
{
	// Token: 0x06006E5E RID: 28254 RVA: 0x00206240 File Offset: 0x00204440
	protected override void Play()
	{
		if (!this.m_forceDefaultAudioSource)
		{
			this.m_SpecificCardFound = this.SearchForCard();
		}
		if (this.m_SpecificCardFound)
		{
			this.Stop();
			this.m_ActiveAudioIndex = 0;
			this.m_activeAudioSource = ((!this.m_forceDefaultAudioSource) ? this.DetermineBestAudioSource() : this.m_CardSoundData.m_AudioSource);
			if (this.m_activeAudioSource == null)
			{
				this.OnStateFinished();
				return;
			}
			base.StartCoroutine("DelayedPlayMulti");
		}
		else
		{
			base.Play();
		}
	}

	// Token: 0x06006E5F RID: 28255 RVA: 0x002062D2 File Offset: 0x002044D2
	protected virtual void PlayNowMulti()
	{
		SoundManager.Get().Play(this.m_activeAudioSource);
		base.StartCoroutine("WaitForSourceThenContinue");
	}

	// Token: 0x06006E60 RID: 28256 RVA: 0x002062F1 File Offset: 0x002044F1
	protected override void Stop()
	{
		base.StopCoroutine("WaitForSourceThenContinue");
		base.Stop();
	}

	// Token: 0x06006E61 RID: 28257 RVA: 0x00206304 File Offset: 0x00204504
	public override AudioSource DetermineBestAudioSource()
	{
		if (!this.m_SpecificCardFound)
		{
			return base.DetermineBestAudioSource();
		}
		if (this.m_ActiveAudioIndex < this.m_CardSpecificVoData.m_Lines.Length)
		{
			return this.m_CardSpecificVoData.m_Lines[this.m_ActiveAudioIndex].m_AudioSource;
		}
		return null;
	}

	// Token: 0x06006E62 RID: 28258 RVA: 0x00206354 File Offset: 0x00204554
	private bool SearchForCard()
	{
		if (string.IsNullOrEmpty(this.m_CardSpecificVoData.m_CardId))
		{
			return false;
		}
		foreach (SpellZoneTag zoneTag in this.m_CardSpecificVoData.m_ZonesToSearch)
		{
			List<Zone> zones = SpellUtils.FindZonesFromTag(this, zoneTag, this.m_CardSpecificVoData.m_SideToSearch);
			if (this.IsCardInZones(zones))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06006E63 RID: 28259 RVA: 0x002063EC File Offset: 0x002045EC
	private bool IsCardInZones(List<Zone> zones)
	{
		if (zones == null)
		{
			return false;
		}
		foreach (Zone zone in zones)
		{
			foreach (Card card in zone.GetCards())
			{
				Entity entity = card.GetEntity();
				if (entity.GetCardId() == this.m_CardSpecificVoData.m_CardId)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06006E64 RID: 28260 RVA: 0x002064B4 File Offset: 0x002046B4
	protected IEnumerator DelayedPlayMulti()
	{
		float delaySec = this.m_CardSpecificVoData.m_Lines[this.m_ActiveAudioIndex].m_DelaySec;
		if (delaySec > 0f)
		{
			yield return new WaitForSeconds(delaySec);
		}
		this.PlayNowMulti();
		yield break;
	}

	// Token: 0x06006E65 RID: 28261 RVA: 0x002064D0 File Offset: 0x002046D0
	protected IEnumerator WaitForSourceThenContinue()
	{
		while (SoundManager.Get().IsActive(this.m_activeAudioSource))
		{
			yield return 0;
		}
		this.m_ActiveAudioIndex++;
		this.m_activeAudioSource = this.DetermineBestAudioSource();
		if (this.m_activeAudioSource != null)
		{
			base.StartCoroutine("DelayedPlayMulti");
		}
		else
		{
			this.OnStateFinished();
		}
		yield break;
	}

	// Token: 0x040056F7 RID: 22263
	public CardSpecificMultiVoData m_CardSpecificVoData = new CardSpecificMultiVoData();

	// Token: 0x040056F8 RID: 22264
	private int m_ActiveAudioIndex;

	// Token: 0x040056F9 RID: 22265
	private bool m_SpecificCardFound;
}
