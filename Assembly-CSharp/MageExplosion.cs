using System;
using UnityEngine;

// Token: 0x02000F6D RID: 3949
public class MageExplosion : MonoBehaviour
{
	// Token: 0x06007527 RID: 29991 RVA: 0x00229579 File Offset: 0x00227779
	private void Start()
	{
	}

	// Token: 0x06007528 RID: 29992 RVA: 0x0022957B File Offset: 0x0022777B
	private void Awake()
	{
	}

	// Token: 0x06007529 RID: 29993 RVA: 0x0022957D File Offset: 0x0022777D
	public void Play()
	{
	}

	// Token: 0x0600752A RID: 29994 RVA: 0x00229580 File Offset: 0x00227780
	private void KillExplosion()
	{
		ParticleEmitter[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleEmitter>();
		foreach (ParticleEmitter particleEmitter in componentsInChildren)
		{
			ParticleAnimator component = particleEmitter.transform.GetComponent<ParticleAnimator>();
			if (component != null)
			{
				component.autodestruct = true;
			}
			Particle[] particles = particleEmitter.particles;
			for (int j = 0; j < particles.Length; j++)
			{
				particles[j].energy = 0.1f;
			}
			particleEmitter.particles = particles;
		}
		Object.Destroy(this);
	}

	// Token: 0x04005FAD RID: 24493
	public GameObject m_ring;
}
