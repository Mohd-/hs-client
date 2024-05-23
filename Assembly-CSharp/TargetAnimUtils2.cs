using System;
using UnityEngine;

// Token: 0x02000F48 RID: 3912
public class TargetAnimUtils2 : MonoBehaviour
{
	// Token: 0x06007447 RID: 29767 RVA: 0x00223A18 File Offset: 0x00221C18
	public void PrintLog2(string message)
	{
		Debug.Log(message);
	}

	// Token: 0x06007448 RID: 29768 RVA: 0x00223A20 File Offset: 0x00221C20
	public void PrintLogWarning2(string message)
	{
		Debug.LogWarning(message);
	}

	// Token: 0x06007449 RID: 29769 RVA: 0x00223A28 File Offset: 0x00221C28
	public void PrintLogError2(string message)
	{
		Debug.LogError(message);
	}

	// Token: 0x0600744A RID: 29770 RVA: 0x00223A30 File Offset: 0x00221C30
	public void PlayNewParticles2()
	{
		this.m_Target.GetComponent<ParticleSystem>().Play();
	}

	// Token: 0x0600744B RID: 29771 RVA: 0x00223A42 File Offset: 0x00221C42
	public void StopNewParticles2()
	{
		this.m_Target.GetComponent<ParticleSystem>().Stop();
	}

	// Token: 0x0600744C RID: 29772 RVA: 0x00223A54 File Offset: 0x00221C54
	public void PlayParticles2()
	{
		if (this.m_Target.GetComponent<ParticleEmitter>() != null)
		{
			this.m_Target.GetComponent<ParticleEmitter>().emit = true;
		}
	}

	// Token: 0x0600744D RID: 29773 RVA: 0x00223A7D File Offset: 0x00221C7D
	public void StopParticles2()
	{
		if (this.m_Target.GetComponent<ParticleEmitter>() != null)
		{
			this.m_Target.GetComponent<ParticleEmitter>().emit = false;
		}
	}

	// Token: 0x0600744E RID: 29774 RVA: 0x00223AA8 File Offset: 0x00221CA8
	public void PlayParticlesInChildren2()
	{
		ParticleEmitter[] componentsInChildren = this.m_Target.GetComponentsInChildren<ParticleEmitter>();
		foreach (ParticleEmitter particleEmitter in componentsInChildren)
		{
			particleEmitter.emit = true;
		}
	}

	// Token: 0x0600744F RID: 29775 RVA: 0x00223AE4 File Offset: 0x00221CE4
	public void StopParticlesInChildren2()
	{
		ParticleEmitter[] componentsInChildren = this.m_Target.GetComponentsInChildren<ParticleEmitter>();
		foreach (ParticleEmitter particleEmitter in componentsInChildren)
		{
			particleEmitter.emit = false;
		}
	}

	// Token: 0x06007450 RID: 29776 RVA: 0x00223B20 File Offset: 0x00221D20
	public void KillParticlesInChildren2()
	{
		Particle[] particles = new Particle[0];
		ParticleEmitter[] componentsInChildren = this.m_Target.GetComponentsInChildren<ParticleEmitter>();
		foreach (ParticleEmitter particleEmitter in componentsInChildren)
		{
			particleEmitter.emit = false;
			particleEmitter.particles = particles;
		}
	}

	// Token: 0x06007451 RID: 29777 RVA: 0x00223B6D File Offset: 0x00221D6D
	public void PlayAnimation2()
	{
		if (this.m_Target.GetComponent<Animation>() != null)
		{
			this.m_Target.GetComponent<Animation>().Play();
		}
	}

	// Token: 0x06007452 RID: 29778 RVA: 0x00223B98 File Offset: 0x00221D98
	public void StopAnimation2()
	{
		if (this.m_Target.GetComponent<Animation>() != null)
		{
			this.m_Target.GetComponent<Animation>().Stop();
		}
	}

	// Token: 0x06007453 RID: 29779 RVA: 0x00223BCC File Offset: 0x00221DCC
	public void PlayAnimationsInChildren2()
	{
		Animation[] componentsInChildren = this.m_Target.GetComponentsInChildren<Animation>();
		foreach (Animation animation in componentsInChildren)
		{
			animation.Play();
		}
	}

	// Token: 0x06007454 RID: 29780 RVA: 0x00223C08 File Offset: 0x00221E08
	public void StopAnimationsInChildren2()
	{
		Animation[] componentsInChildren = this.m_Target.GetComponentsInChildren<Animation>();
		foreach (Animation animation in componentsInChildren)
		{
			animation.Stop();
		}
	}

	// Token: 0x06007455 RID: 29781 RVA: 0x00223C41 File Offset: 0x00221E41
	public void ActivateHierarchy2()
	{
		this.m_Target.SetActive(true);
	}

	// Token: 0x06007456 RID: 29782 RVA: 0x00223C4F File Offset: 0x00221E4F
	public void DeactivateHierarchy2()
	{
		this.m_Target.SetActive(false);
	}

	// Token: 0x06007457 RID: 29783 RVA: 0x00223C5D File Offset: 0x00221E5D
	public void DestroyHierarchy2()
	{
		Object.Destroy(this.m_Target);
	}

	// Token: 0x06007458 RID: 29784 RVA: 0x00223C6A File Offset: 0x00221E6A
	public void FadeIn2(float FadeSec)
	{
		iTween.FadeTo(this.m_Target, 1f, FadeSec);
	}

	// Token: 0x06007459 RID: 29785 RVA: 0x00223C7D File Offset: 0x00221E7D
	public void FadeOut2(float FadeSec)
	{
		iTween.FadeTo(this.m_Target, 0f, FadeSec);
	}

	// Token: 0x0600745A RID: 29786 RVA: 0x00223C90 File Offset: 0x00221E90
	public void SetAlphaHierarchy2(float alpha)
	{
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

	// Token: 0x0600745B RID: 29787 RVA: 0x00223CFC File Offset: 0x00221EFC
	public void PlayDefaultSound2()
	{
		if (this.m_Target.GetComponent<AudioSource>() == null)
		{
			string text = string.Format("TargetAnimUtils2.PlayDefaultSound() - Tried to play the AudioSource on {0} but it has no AudioSource. You need an AudioSource to use this function.", this.m_Target);
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

	// Token: 0x0600745C RID: 29788 RVA: 0x00223D74 File Offset: 0x00221F74
	public void PlaySound2(AudioClip clip)
	{
		if (clip == null)
		{
			string text = string.Format("TargetAnimUtils2.PlayDefaultSound() - No clip was given when trying to play the AudioSource on {0}. You need a clip to use this function.", this.m_Target);
			Debug.LogError(text);
			return;
		}
		if (this.m_Target.GetComponent<AudioSource>() == null)
		{
			string text2 = string.Format("TargetAnimUtils2.PlayDefaultSound() - Tried to play clip {0} on {1} but it has no AudioSource. You need an AudioSource to use this function.", clip, this.m_Target);
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

	// Token: 0x04005ECF RID: 24271
	public GameObject m_Target;
}
