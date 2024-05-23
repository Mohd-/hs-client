using System;
using UnityEngine;

// Token: 0x02000F38 RID: 3896
public class SelfAnimUtils : MonoBehaviour
{
	// Token: 0x060073CF RID: 29647 RVA: 0x00221AF6 File Offset: 0x0021FCF6
	public void PrintLog(string message)
	{
		Debug.Log(message);
	}

	// Token: 0x060073D0 RID: 29648 RVA: 0x00221AFE File Offset: 0x0021FCFE
	public void PrintLogWarning(string message)
	{
		Debug.LogWarning(message);
	}

	// Token: 0x060073D1 RID: 29649 RVA: 0x00221B06 File Offset: 0x0021FD06
	public void PrintLogError(string message)
	{
		Debug.LogError(message);
	}

	// Token: 0x060073D2 RID: 29650 RVA: 0x00221B0E File Offset: 0x0021FD0E
	public void PlayParticles()
	{
		if (base.GetComponent<ParticleEmitter>() != null)
		{
			base.GetComponent<ParticleEmitter>().emit = true;
		}
	}

	// Token: 0x060073D3 RID: 29651 RVA: 0x00221B2D File Offset: 0x0021FD2D
	public void StopParticles()
	{
		if (base.GetComponent<ParticleEmitter>() != null)
		{
			base.GetComponent<ParticleEmitter>().emit = false;
		}
	}

	// Token: 0x060073D4 RID: 29652 RVA: 0x00221B4C File Offset: 0x0021FD4C
	public void KillParticles()
	{
		if (base.GetComponent<ParticleEmitter>() == null)
		{
			return;
		}
		base.GetComponent<ParticleEmitter>().emit = false;
		base.GetComponent<ParticleEmitter>().particles = new Particle[0];
	}

	// Token: 0x060073D5 RID: 29653 RVA: 0x00221B88 File Offset: 0x0021FD88
	public void PlayAnimation()
	{
		if (base.GetComponent<Animation>() != null)
		{
			base.GetComponent<Animation>().Play();
		}
	}

	// Token: 0x060073D6 RID: 29654 RVA: 0x00221BA7 File Offset: 0x0021FDA7
	public void StopAnimation()
	{
		if (base.GetComponent<Animation>() != null)
		{
			base.GetComponent<Animation>().Stop();
		}
	}

	// Token: 0x060073D7 RID: 29655 RVA: 0x00221BC5 File Offset: 0x0021FDC5
	public void ActivateHierarchy()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x060073D8 RID: 29656 RVA: 0x00221BD3 File Offset: 0x0021FDD3
	public void DeactivateHierarchy()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x060073D9 RID: 29657 RVA: 0x00221BE1 File Offset: 0x0021FDE1
	public void DestroyHierarchy()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060073DA RID: 29658 RVA: 0x00221BEE File Offset: 0x0021FDEE
	public void FadeIn(float FadeSec)
	{
		iTween.FadeTo(base.gameObject, 1f, FadeSec);
	}

	// Token: 0x060073DB RID: 29659 RVA: 0x00221C01 File Offset: 0x0021FE01
	public void FadeOut(float FadeSec)
	{
		iTween.FadeTo(base.gameObject, 0f, FadeSec);
	}

	// Token: 0x060073DC RID: 29660 RVA: 0x00221C14 File Offset: 0x0021FE14
	public void SetAlphaHierarchy(float alpha)
	{
		foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>())
		{
			if (renderer.material.HasProperty("_Color"))
			{
				Color color = renderer.material.color;
				color.a = alpha;
				renderer.material.color = color;
			}
		}
	}

	// Token: 0x060073DD RID: 29661 RVA: 0x00221C7C File Offset: 0x0021FE7C
	public void PlayDefaultSound()
	{
		if (base.GetComponent<AudioSource>() == null)
		{
			string text = string.Format("SelfAnimUtils.PlayDefaultSound() - Tried to play the AudioSource on {0} but it has no AudioSource. You need an AudioSource to use this function.", base.gameObject);
			Debug.LogError(text);
			return;
		}
		if (SoundManager.Get() == null)
		{
			base.GetComponent<AudioSource>().Play();
		}
		else
		{
			SoundManager.Get().Play(base.GetComponent<AudioSource>());
		}
	}

	// Token: 0x060073DE RID: 29662 RVA: 0x00221CE4 File Offset: 0x0021FEE4
	public void PlaySound(AudioClip clip)
	{
		if (clip == null)
		{
			string text = string.Format("SelfAnimUtils.PlayDefaultSound() - No clip was given when trying to play the AudioSource on {0}. You need a clip to use this function.", base.gameObject);
			Debug.LogError(text);
			return;
		}
		if (base.GetComponent<AudioSource>() == null)
		{
			string text2 = string.Format("SelfAnimUtils.PlayDefaultSound() - Tried to play clip {0} on {1} but it has no AudioSource. You need an AudioSource to use this function.", clip, base.gameObject);
			Debug.LogError(text2);
			return;
		}
		if (SoundManager.Get() == null)
		{
			base.GetComponent<AudioSource>().PlayOneShot(clip);
		}
		else
		{
			SoundManager.Get().PlayOneShot(base.GetComponent<AudioSource>(), clip, 1f);
		}
	}

	// Token: 0x060073DF RID: 29663 RVA: 0x00221D77 File Offset: 0x0021FF77
	public void RandomRotationX()
	{
		TransformUtil.SetEulerAngleX(base.gameObject, Random.Range(0f, 360f));
	}

	// Token: 0x060073E0 RID: 29664 RVA: 0x00221D93 File Offset: 0x0021FF93
	public void RandomRotationY()
	{
		TransformUtil.SetEulerAngleY(base.gameObject, Random.Range(0f, 360f));
	}

	// Token: 0x060073E1 RID: 29665 RVA: 0x00221DAF File Offset: 0x0021FFAF
	public void RandomRotationZ()
	{
		TransformUtil.SetEulerAngleZ(base.gameObject, Random.Range(0f, 360f));
	}
}
