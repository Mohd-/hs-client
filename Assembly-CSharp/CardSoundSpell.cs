using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000186 RID: 390
public class CardSoundSpell : Spell
{
	// Token: 0x06001665 RID: 5733 RVA: 0x0006A406 File Offset: 0x00068606
	protected override void OnBirth(SpellStateType prevStateType)
	{
		base.OnBirth(prevStateType);
		this.Play();
	}

	// Token: 0x06001666 RID: 5734 RVA: 0x0006A415 File Offset: 0x00068615
	protected override void OnNone(SpellStateType prevStateType)
	{
		base.OnNone(prevStateType);
		this.Stop();
	}

	// Token: 0x06001667 RID: 5735 RVA: 0x0006A424 File Offset: 0x00068624
	public AudioSource GetActiveAudioSource()
	{
		return this.m_activeAudioSource;
	}

	// Token: 0x06001668 RID: 5736 RVA: 0x0006A42C File Offset: 0x0006862C
	public void ForceDefaultAudioSource()
	{
		this.m_forceDefaultAudioSource = true;
	}

	// Token: 0x06001669 RID: 5737 RVA: 0x0006A435 File Offset: 0x00068635
	public bool HasActiveAudioSource()
	{
		return this.m_activeAudioSource != null;
	}

	// Token: 0x0600166A RID: 5738 RVA: 0x0006A443 File Offset: 0x00068643
	public virtual AudioSource DetermineBestAudioSource()
	{
		return this.m_CardSoundData.m_AudioSource;
	}

	// Token: 0x0600166B RID: 5739 RVA: 0x0006A450 File Offset: 0x00068650
	protected virtual void Play()
	{
		this.Stop();
		this.m_activeAudioSource = ((!this.m_forceDefaultAudioSource) ? this.DetermineBestAudioSource() : this.m_CardSoundData.m_AudioSource);
		if (this.m_activeAudioSource == null)
		{
			this.OnStateFinished();
			return;
		}
		base.StartCoroutine("DelayedPlay");
	}

	// Token: 0x0600166C RID: 5740 RVA: 0x0006A4AE File Offset: 0x000686AE
	protected virtual void PlayNow()
	{
		SoundManager.Get().Play(this.m_activeAudioSource);
		base.StartCoroutine("WaitForSourceThenFinishState");
	}

	// Token: 0x0600166D RID: 5741 RVA: 0x0006A4CD File Offset: 0x000686CD
	protected virtual void Stop()
	{
		base.StopCoroutine("DelayedPlay");
		base.StopCoroutine("WaitForSourceThenFinishState");
		SoundManager.Get().Stop(this.m_activeAudioSource);
	}

	// Token: 0x0600166E RID: 5742 RVA: 0x0006A4F8 File Offset: 0x000686F8
	protected IEnumerator DelayedPlay()
	{
		if (this.m_CardSoundData.m_DelaySec > 0f)
		{
			yield return new WaitForSeconds(this.m_CardSoundData.m_DelaySec);
		}
		this.PlayNow();
		yield break;
	}

	// Token: 0x0600166F RID: 5743 RVA: 0x0006A514 File Offset: 0x00068714
	protected IEnumerator WaitForSourceThenFinishState()
	{
		while (SoundManager.Get().IsActive(this.m_activeAudioSource))
		{
			yield return 0;
		}
		this.OnStateFinished();
		yield break;
	}

	// Token: 0x04000B34 RID: 2868
	public CardSoundData m_CardSoundData = new CardSoundData();

	// Token: 0x04000B35 RID: 2869
	protected AudioSource m_activeAudioSource;

	// Token: 0x04000B36 RID: 2870
	protected bool m_forceDefaultAudioSource;
}
