using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F49 RID: 3913
public class TargetListAnimUtils : MonoBehaviour
{
	// Token: 0x0600745E RID: 29790 RVA: 0x00223E20 File Offset: 0x00222020
	public void PlayParticlesList()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				gameObject.GetComponent<ParticleEmitter>().emit = true;
			}
		}
	}

	// Token: 0x0600745F RID: 29791 RVA: 0x00223E90 File Offset: 0x00222090
	public void StopParticlesList()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				gameObject.GetComponent<ParticleEmitter>().emit = false;
			}
		}
	}

	// Token: 0x06007460 RID: 29792 RVA: 0x00223F00 File Offset: 0x00222100
	public void KillParticlesList()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				gameObject.GetComponent<ParticleEmitter>().particles = new Particle[0];
			}
		}
	}

	// Token: 0x06007461 RID: 29793 RVA: 0x00223F78 File Offset: 0x00222178
	public void PlayParticlesListInChildren()
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

	// Token: 0x06007462 RID: 29794 RVA: 0x0022400C File Offset: 0x0022220C
	public void PlayNewParticlesListInChildren()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				ParticleSystem[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					particleSystem.Play();
				}
			}
		}
	}

	// Token: 0x06007463 RID: 29795 RVA: 0x002240A0 File Offset: 0x002222A0
	public void StopNewParticlesListInChildren()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				ParticleSystem[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					particleSystem.Stop();
				}
			}
		}
	}

	// Token: 0x06007464 RID: 29796 RVA: 0x00224134 File Offset: 0x00222334
	public void StopParticlesListInChildren()
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

	// Token: 0x06007465 RID: 29797 RVA: 0x002241C8 File Offset: 0x002223C8
	public void KillParticlesListInChildren()
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

	// Token: 0x06007466 RID: 29798 RVA: 0x00224270 File Offset: 0x00222470
	public void PlayAnimationList()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				gameObject.GetComponent<Animation>().Play();
			}
		}
	}

	// Token: 0x06007467 RID: 29799 RVA: 0x002242E0 File Offset: 0x002224E0
	public void StopAnimationList()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				gameObject.GetComponent<Animation>().Stop();
			}
		}
	}

	// Token: 0x06007468 RID: 29800 RVA: 0x00224350 File Offset: 0x00222550
	public void PlayAnimationListInChildren()
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

	// Token: 0x06007469 RID: 29801 RVA: 0x002243E4 File Offset: 0x002225E4
	public void StopAnimationListInChildren()
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

	// Token: 0x0600746A RID: 29802 RVA: 0x00224478 File Offset: 0x00222678
	public void ActivateHierarchyList()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x0600746B RID: 29803 RVA: 0x002244E4 File Offset: 0x002226E4
	public void DeactivateHierarchyList()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600746C RID: 29804 RVA: 0x00224550 File Offset: 0x00222750
	public void DestroyHierarchyList()
	{
		foreach (GameObject gameObject in this.m_TargetList)
		{
			Object.Destroy(gameObject);
		}
	}

	// Token: 0x0600746D RID: 29805 RVA: 0x002245AC File Offset: 0x002227AC
	public void FadeInList(float FadeSec)
	{
		foreach (GameObject target in this.m_TargetList)
		{
			iTween.FadeTo(target, 1f, FadeSec);
		}
	}

	// Token: 0x0600746E RID: 29806 RVA: 0x0022460C File Offset: 0x0022280C
	public void FadeOutList(float FadeSec)
	{
		foreach (GameObject target in this.m_TargetList)
		{
			iTween.FadeTo(target, 0f, FadeSec);
		}
	}

	// Token: 0x0600746F RID: 29807 RVA: 0x0022466C File Offset: 0x0022286C
	public void SetAlphaHierarchyList(float alpha)
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

	// Token: 0x04005ED0 RID: 24272
	public List<GameObject> m_TargetList;
}
