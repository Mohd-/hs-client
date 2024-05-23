using System;
using UnityEngine;

// Token: 0x02000F47 RID: 3911
public class TargetAnimUtils : MonoBehaviour
{
	// Token: 0x0600742F RID: 29743 RVA: 0x002234AA File Offset: 0x002216AA
	private void Awake()
	{
		if (this.m_Target == null)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06007430 RID: 29744 RVA: 0x002234C4 File Offset: 0x002216C4
	public void PrintLog(string message)
	{
		Debug.Log(message);
	}

	// Token: 0x06007431 RID: 29745 RVA: 0x002234CC File Offset: 0x002216CC
	public void PrintLogWarning(string message)
	{
		Debug.LogWarning(message);
	}

	// Token: 0x06007432 RID: 29746 RVA: 0x002234D4 File Offset: 0x002216D4
	public void PrintLogError(string message)
	{
		Debug.LogError(message);
	}

	// Token: 0x06007433 RID: 29747 RVA: 0x002234DC File Offset: 0x002216DC
	public void PlayParticles()
	{
		if (this.m_Target == null)
		{
			return;
		}
		if (this.m_Target.GetComponent<ParticleEmitter>() != null)
		{
			this.m_Target.GetComponent<ParticleEmitter>().emit = true;
		}
	}

	// Token: 0x06007434 RID: 29748 RVA: 0x00223522 File Offset: 0x00221722
	public void PlayNewParticles()
	{
		this.m_Target.GetComponent<ParticleSystem>().Play();
	}

	// Token: 0x06007435 RID: 29749 RVA: 0x00223534 File Offset: 0x00221734
	public void StopNewParticles()
	{
		if (this.m_Target == null)
		{
			return;
		}
		this.m_Target.GetComponent<ParticleSystem>().Stop();
	}

	// Token: 0x06007436 RID: 29750 RVA: 0x00223564 File Offset: 0x00221764
	public void StopParticles()
	{
		if (this.m_Target == null)
		{
			return;
		}
		if (this.m_Target.GetComponent<ParticleEmitter>() != null)
		{
			this.m_Target.GetComponent<ParticleEmitter>().emit = false;
		}
	}

	// Token: 0x06007437 RID: 29751 RVA: 0x002235AC File Offset: 0x002217AC
	public void PlayParticlesInChildren()
	{
		if (this.m_Target == null)
		{
			return;
		}
		ParticleEmitter[] componentsInChildren = this.m_Target.GetComponentsInChildren<ParticleEmitter>();
		foreach (ParticleEmitter particleEmitter in componentsInChildren)
		{
			particleEmitter.emit = true;
		}
	}

	// Token: 0x06007438 RID: 29752 RVA: 0x002235F8 File Offset: 0x002217F8
	public void StopParticlesInChildren()
	{
		if (this.m_Target == null)
		{
			return;
		}
		ParticleEmitter[] componentsInChildren = this.m_Target.GetComponentsInChildren<ParticleEmitter>();
		foreach (ParticleEmitter particleEmitter in componentsInChildren)
		{
			particleEmitter.emit = false;
		}
	}

	// Token: 0x06007439 RID: 29753 RVA: 0x00223644 File Offset: 0x00221844
	public void KillParticlesInChildren()
	{
		if (this.m_Target == null)
		{
			return;
		}
		Particle[] particles = new Particle[0];
		ParticleEmitter[] componentsInChildren = this.m_Target.GetComponentsInChildren<ParticleEmitter>();
		foreach (ParticleEmitter particleEmitter in componentsInChildren)
		{
			particleEmitter.emit = false;
			particleEmitter.particles = particles;
		}
	}

	// Token: 0x0600743A RID: 29754 RVA: 0x002236A4 File Offset: 0x002218A4
	public void PlayAnimation()
	{
		if (this.m_Target == null)
		{
			return;
		}
		if (this.m_Target.GetComponent<Animation>() != null)
		{
			this.m_Target.GetComponent<Animation>().Play();
		}
	}

	// Token: 0x0600743B RID: 29755 RVA: 0x002236EA File Offset: 0x002218EA
	public void StopAnimation()
	{
		if (this.m_Target == null)
		{
			return;
		}
		if (this.m_Target.GetComponent<Animation>() != null)
		{
			this.m_Target.GetComponent<Animation>().Stop();
		}
	}

	// Token: 0x0600743C RID: 29756 RVA: 0x00223724 File Offset: 0x00221924
	public void PlayAnimationsInChildren()
	{
		if (this.m_Target == null)
		{
			return;
		}
		Animation[] componentsInChildren = this.m_Target.GetComponentsInChildren<Animation>();
		foreach (Animation animation in componentsInChildren)
		{
			animation.Play();
		}
	}

	// Token: 0x0600743D RID: 29757 RVA: 0x00223770 File Offset: 0x00221970
	public void StopAnimationsInChildren()
	{
		if (this.m_Target == null)
		{
			return;
		}
		Animation[] componentsInChildren = this.m_Target.GetComponentsInChildren<Animation>();
		foreach (Animation animation in componentsInChildren)
		{
			animation.Stop();
		}
	}

	// Token: 0x0600743E RID: 29758 RVA: 0x002237BB File Offset: 0x002219BB
	public void ActivateHierarchy()
	{
		this.m_Target.SetActive(true);
	}

	// Token: 0x0600743F RID: 29759 RVA: 0x002237C9 File Offset: 0x002219C9
	public void DeactivateHierarchy()
	{
		if (this.m_Target == null)
		{
			return;
		}
		this.m_Target.SetActive(false);
	}

	// Token: 0x06007440 RID: 29760 RVA: 0x002237E9 File Offset: 0x002219E9
	public void DestroyHierarchy()
	{
		if (this.m_Target == null)
		{
			return;
		}
		Object.Destroy(this.m_Target);
	}

	// Token: 0x06007441 RID: 29761 RVA: 0x00223808 File Offset: 0x00221A08
	public void FadeIn(float FadeSec)
	{
		if (this.m_Target == null)
		{
			return;
		}
		iTween.FadeTo(this.m_Target, 1f, FadeSec);
	}

	// Token: 0x06007442 RID: 29762 RVA: 0x0022382D File Offset: 0x00221A2D
	public void FadeOut(float FadeSec)
	{
		if (this.m_Target == null)
		{
			return;
		}
		iTween.FadeTo(this.m_Target, 0f, FadeSec);
	}

	// Token: 0x06007443 RID: 29763 RVA: 0x00223854 File Offset: 0x00221A54
	public void SetAlphaHierarchy(float alpha)
	{
		if (this.m_Target == null)
		{
			return;
		}
		foreach (Renderer renderer in this.m_Target.GetComponentsInChildren<Renderer>())
		{
			if (renderer.material.HasProperty("_Color"))
			{
				Color color = renderer.material.color;
				color.a = alpha;
				renderer.material.color = color;
			}
		}
	}

	// Token: 0x06007444 RID: 29764 RVA: 0x002238D4 File Offset: 0x00221AD4
	public void PlayDefaultSound()
	{
		if (this.m_Target == null)
		{
			return;
		}
		if (this.m_Target.GetComponent<AudioSource>() == null)
		{
			string text = string.Format("TargetAnimUtils.PlayDefaultSound() - Tried to play the AudioSource on {0} but it has no AudioSource. You need an AudioSource to use this function.", this.m_Target);
			Debug.LogError(text);
			return;
		}
		if (SoundManager.Get() == null)
		{
			this.m_Target.GetComponent<AudioSource>().Play();
		}
		else
		{
			SoundManager.Get().Play(this.m_Target.GetComponent<AudioSource>());
		}
	}

	// Token: 0x06007445 RID: 29765 RVA: 0x0022395C File Offset: 0x00221B5C
	public void PlaySound(AudioClip clip)
	{
		if (this.m_Target == null)
		{
			return;
		}
		if (clip == null)
		{
			string text = string.Format("TargetAnimUtils.PlayDefaultSound() - No clip was given when trying to play the AudioSource on {0}. You need a clip to use this function.", this.m_Target);
			Debug.LogError(text);
			return;
		}
		if (this.m_Target.GetComponent<AudioSource>() == null)
		{
			string text2 = string.Format("TargetAnimUtils.PlayDefaultSound() - Tried to play clip {0} on {1} but it has no AudioSource. You need an AudioSource to use this function.", clip, this.m_Target);
			Debug.LogError(text2);
			return;
		}
		if (SoundManager.Get() == null)
		{
			this.m_Target.GetComponent<AudioSource>().PlayOneShot(clip);
		}
		else
		{
			SoundManager.Get().PlayOneShot(this.m_Target.GetComponent<AudioSource>(), clip, 1f);
		}
	}

	// Token: 0x04005ECE RID: 24270
	public GameObject m_Target;
}
