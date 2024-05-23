using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F4A RID: 3914
public class TargetListAnimUtils2 : MonoBehaviour
{
	// Token: 0x06007471 RID: 29809 RVA: 0x00224738 File Offset: 0x00222938
	public void PlayParticlesList2()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				gameObject.GetComponent<ParticleEmitter>().emit = true;
			}
		}
	}

	// Token: 0x06007472 RID: 29810 RVA: 0x002247A8 File Offset: 0x002229A8
	public void StopParticlesList2()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				gameObject.GetComponent<ParticleEmitter>().emit = false;
			}
		}
	}

	// Token: 0x06007473 RID: 29811 RVA: 0x00224818 File Offset: 0x00222A18
	public void KillParticlesList2()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				gameObject.GetComponent<ParticleEmitter>().particles = new Particle[0];
			}
		}
	}

	// Token: 0x06007474 RID: 29812 RVA: 0x00224890 File Offset: 0x00222A90
	public void PlayParticlesListInChildren2()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				ParticleEmitter[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleEmitter>();
				foreach (ParticleEmitter particleEmitter in componentsInChildren)
				{
					particleEmitter.emit = true;
				}
			}
		}
	}

	// Token: 0x06007475 RID: 29813 RVA: 0x00224924 File Offset: 0x00222B24
	public void StopParticlesListInChildren2()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				ParticleEmitter[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleEmitter>();
				foreach (ParticleEmitter particleEmitter in componentsInChildren)
				{
					particleEmitter.emit = false;
				}
			}
		}
	}

	// Token: 0x06007476 RID: 29814 RVA: 0x002249B8 File Offset: 0x00222BB8
	public void KillParticlesListInChildren2()
	{
		Particle[] particles = new Particle[0];
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				ParticleEmitter[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleEmitter>();
				foreach (ParticleEmitter particleEmitter in componentsInChildren)
				{
					particleEmitter.emit = false;
					particleEmitter.particles = particles;
				}
			}
		}
	}

	// Token: 0x06007477 RID: 29815 RVA: 0x00224A60 File Offset: 0x00222C60
	public void PlayAnimationList2()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				gameObject.GetComponent<Animation>().Play();
			}
		}
	}

	// Token: 0x06007478 RID: 29816 RVA: 0x00224AD0 File Offset: 0x00222CD0
	public void StopAnimationList2()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				gameObject.GetComponent<Animation>().Stop();
			}
		}
	}

	// Token: 0x06007479 RID: 29817 RVA: 0x00224B40 File Offset: 0x00222D40
	public void PlayAnimationListInChildren2()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				Animation[] componentsInChildren = gameObject.GetComponentsInChildren<Animation>();
				foreach (Animation animation in componentsInChildren)
				{
					animation.Play();
				}
			}
		}
	}

	// Token: 0x0600747A RID: 29818 RVA: 0x00224BD4 File Offset: 0x00222DD4
	public void StopAnimationListInChildren2()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				Animation[] componentsInChildren = gameObject.GetComponentsInChildren<Animation>();
				foreach (Animation animation in componentsInChildren)
				{
					animation.Stop();
				}
			}
		}
	}

	// Token: 0x0600747B RID: 29819 RVA: 0x00224C68 File Offset: 0x00222E68
	public void ActivateHierarchyList2()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x0600747C RID: 29820 RVA: 0x00224CD4 File Offset: 0x00222ED4
	public void DeactivateHierarchyList2()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600747D RID: 29821 RVA: 0x00224D40 File Offset: 0x00222F40
	public void DestroyHierarchyList2()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			Object.Destroy(gameObject);
		}
	}

	// Token: 0x0600747E RID: 29822 RVA: 0x00224D9C File Offset: 0x00222F9C
	public void FadeInList2(float FadeSec)
	{
		foreach (GameObject target in this.m_TargetList)
		{
			iTween.FadeTo(target, 1f, FadeSec);
		}
	}

	// Token: 0x0600747F RID: 29823 RVA: 0x00224DFC File Offset: 0x00222FFC
	public void FadeOutList2(float FadeSec)
	{
		foreach (GameObject target in this.m_TargetList)
		{
			iTween.FadeTo(target, 0f, FadeSec);
		}
	}

	// Token: 0x06007480 RID: 29824 RVA: 0x00224E5C File Offset: 0x0022305C
	public void SetAlphaHierarchyList2(float alpha)
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
				{
					if (renderer.material.HasProperty("_Color"))
					{
						Color color = renderer.material.color;
						color.a = alpha;
						renderer.material.color = color;
					}
				}
			}
		}
	}

	// Token: 0x04005ED1 RID: 24273
	public List<GameObject> m_TargetList;
}
