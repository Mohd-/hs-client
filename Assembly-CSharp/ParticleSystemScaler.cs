using System;
using UnityEngine;

// Token: 0x02000E7C RID: 3708
public class ParticleSystemScaler : MonoBehaviour
{
	// Token: 0x06007064 RID: 28772 RVA: 0x00210BFC File Offset: 0x0020EDFC
	private void Awake()
	{
		this.m_unitMagnitude = Vector3.one.magnitude;
	}

	// Token: 0x06007065 RID: 28773 RVA: 0x00210C1C File Offset: 0x0020EE1C
	private void Update()
	{
		if (this.ObjectToInherit != null)
		{
			this.ParticleSystemScale = this.ObjectToInherit.transform.lossyScale.magnitude / this.m_unitMagnitude;
		}
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			if (!this.m_initialValues.ContainsKey(particleSystem))
			{
				this.m_initialValues.Add(particleSystem, new ParticleSystemSizes());
				this.m_initialValues[particleSystem].startSpeed = particleSystem.startSpeed;
				this.m_initialValues[particleSystem].startSize = particleSystem.startSize;
				this.m_initialValues[particleSystem].gravityModifier = particleSystem.gravityModifier;
			}
			particleSystem.startSize = this.m_initialValues[particleSystem].startSize * this.ParticleSystemScale;
			particleSystem.startSpeed = this.m_initialValues[particleSystem].startSpeed * this.ParticleSystemScale;
			particleSystem.gravityModifier = this.m_initialValues[particleSystem].gravityModifier * this.ParticleSystemScale;
		}
	}

	// Token: 0x06007066 RID: 28774 RVA: 0x00210D44 File Offset: 0x0020EF44
	private void ScaleParticleSystems(float scaleFactor)
	{
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			particleSystem.startSpeed *= scaleFactor;
			particleSystem.startSize *= scaleFactor;
			particleSystem.gravityModifier *= scaleFactor;
		}
	}

	// Token: 0x0400597F RID: 22911
	public float ParticleSystemScale = 1f;

	// Token: 0x04005980 RID: 22912
	public GameObject ObjectToInherit;

	// Token: 0x04005981 RID: 22913
	private float m_unitMagnitude;

	// Token: 0x04005982 RID: 22914
	private Map<ParticleSystem, ParticleSystemSizes> m_initialValues = new Map<ParticleSystem, ParticleSystemSizes>();
}
