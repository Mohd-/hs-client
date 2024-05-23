using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000ECF RID: 3791
[Serializable]
public class SpellStateAudioSource
{
	// Token: 0x060071BE RID: 29118 RVA: 0x002173EE File Offset: 0x002155EE
	public void Init()
	{
		if (this.m_AudioSource == null)
		{
			return;
		}
		this.m_AudioSource.playOnAwake = false;
	}

	// Token: 0x060071BF RID: 29119 RVA: 0x00217410 File Offset: 0x00215610
	public void Play(SpellState parent)
	{
		if (!this.m_Enabled)
		{
			return;
		}
		if (object.Equals(this.m_StartDelaySec, 0f))
		{
			this.PlayNow();
		}
		else
		{
			parent.StartCoroutine(this.DelayedPlay());
		}
	}

	// Token: 0x060071C0 RID: 29120 RVA: 0x00217460 File Offset: 0x00215660
	public void Stop()
	{
		if (!this.m_Enabled)
		{
			return;
		}
		if (this.m_AudioSource == null)
		{
			return;
		}
		if (this.m_PlayGlobally)
		{
			return;
		}
		if (this.m_StopOnStateChange)
		{
			this.m_AudioSource.Stop();
		}
	}

	// Token: 0x060071C1 RID: 29121 RVA: 0x002174B0 File Offset: 0x002156B0
	private IEnumerator DelayedPlay()
	{
		yield return new WaitForSeconds(this.m_StartDelaySec);
		this.PlayNow();
		yield break;
	}

	// Token: 0x060071C2 RID: 29122 RVA: 0x002174CC File Offset: 0x002156CC
	private void PlayNow()
	{
		if (this.m_AudioSource == null)
		{
			return;
		}
		if (this.m_PlayGlobally)
		{
			SoundPlayClipArgs soundPlayClipArgs = new SoundPlayClipArgs();
			soundPlayClipArgs.m_clip = this.m_AudioSource.clip;
			soundPlayClipArgs.m_volume = new float?(this.m_AudioSource.volume);
			soundPlayClipArgs.m_pitch = new float?(this.m_AudioSource.pitch);
			soundPlayClipArgs.m_category = new SoundCategory?(SoundManager.Get().GetCategory(this.m_AudioSource));
			soundPlayClipArgs.m_parentObject = this.m_AudioSource.gameObject;
			SoundManager.Get().PlayClip(soundPlayClipArgs);
		}
		else
		{
			SoundManager.Get().Play(this.m_AudioSource);
		}
	}

	// Token: 0x04005BF9 RID: 23545
	public AudioSource m_AudioSource;

	// Token: 0x04005BFA RID: 23546
	public float m_StartDelaySec;

	// Token: 0x04005BFB RID: 23547
	public bool m_PlayGlobally;

	// Token: 0x04005BFC RID: 23548
	public bool m_StopOnStateChange;

	// Token: 0x04005BFD RID: 23549
	public string m_Comment;

	// Token: 0x04005BFE RID: 23550
	public bool m_Enabled = true;
}
