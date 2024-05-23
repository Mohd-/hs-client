using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E87 RID: 3719
[ExecuteInEditMode]
public class ParticleEffects : MonoBehaviour
{
	// Token: 0x06007093 RID: 28819 RVA: 0x002122FC File Offset: 0x002104FC
	private void Update()
	{
		if (this.m_ParticleSystems == null)
		{
			return;
		}
		if (this.m_ParticleSystems.Count == 0)
		{
			ParticleSystem component = base.GetComponent<ParticleSystem>();
			if (component == null)
			{
				base.enabled = false;
			}
			this.m_ParticleSystems.Add(component);
		}
		for (int i = 0; i < this.m_ParticleSystems.Count; i++)
		{
			ParticleSystem particleSystem = this.m_ParticleSystems[i];
			if (!(particleSystem == null))
			{
				int particleCount = particleSystem.particleCount;
				if (particleCount == 0)
				{
					return;
				}
				ParticleSystem.Particle[] array = new ParticleSystem.Particle[particleCount];
				particleSystem.GetParticles(array);
				if (this.m_ParticleAttractors != null)
				{
					this.ParticleAttractor(particleSystem, array, particleCount);
				}
				if (this.m_ParticleRepulsers != null)
				{
					this.ParticleRepulser(particleSystem, array, particleCount);
				}
				if (this.m_ParticleOrientation != null && this.m_ParticleOrientation.m_OrientToDirection)
				{
					this.OrientParticlesToDirection(particleSystem, array, particleCount);
				}
				particleSystem.SetParticles(array, particleCount);
			}
		}
	}

	// Token: 0x06007094 RID: 28820 RVA: 0x002123FC File Offset: 0x002105FC
	private void OnDrawGizmos()
	{
		if (this.m_ParticleAttractors != null)
		{
			foreach (ParticleEffectsAttractor particleEffectsAttractor in this.m_ParticleAttractors)
			{
				if (!(particleEffectsAttractor.m_Transform == null))
				{
					Gizmos.color = Color.green;
					float num = particleEffectsAttractor.m_Radius * ((particleEffectsAttractor.m_Transform.lossyScale.x + particleEffectsAttractor.m_Transform.lossyScale.y + particleEffectsAttractor.m_Transform.lossyScale.z) * 0.333f);
					Gizmos.DrawWireSphere(particleEffectsAttractor.m_Transform.position, num);
				}
			}
		}
		if (this.m_ParticleRepulsers != null)
		{
			foreach (ParticleEffectsRepulser particleEffectsRepulser in this.m_ParticleRepulsers)
			{
				if (!(particleEffectsRepulser.m_Transform == null))
				{
					Gizmos.color = Color.red;
					float num2 = particleEffectsRepulser.m_Radius * ((particleEffectsRepulser.m_Transform.lossyScale.x + particleEffectsRepulser.m_Transform.lossyScale.y + particleEffectsRepulser.m_Transform.lossyScale.z) * 0.333f);
					Gizmos.DrawWireSphere(particleEffectsRepulser.m_Transform.position, num2);
				}
			}
		}
	}

	// Token: 0x06007095 RID: 28821 RVA: 0x002125A8 File Offset: 0x002107A8
	private void OrientParticlesToDirection(ParticleSystem particleSystem, ParticleSystem.Particle[] particles, int particleCount)
	{
		for (int i = 0; i < particleCount; i++)
		{
			particles[i].angularVelocity = 0f;
			Vector3 targetVector = particles[i].velocity;
			if (!this.m_WorldSpace)
			{
				targetVector = particleSystem.transform.TransformDirection(particles[i].velocity);
			}
			if (this.m_ParticleOrientation.m_UpVector == ParticleEffectsOrientUpVectors.Horizontal)
			{
				particles[i].rotation = ParticleEffects.VectorAngle(Vector3.forward, targetVector, Vector3.up);
			}
			else if (this.m_ParticleOrientation.m_UpVector == ParticleEffectsOrientUpVectors.Vertical)
			{
				particles[i].rotation = ParticleEffects.VectorAngle(Vector3.up, targetVector, Vector3.forward);
			}
		}
	}

	// Token: 0x06007096 RID: 28822 RVA: 0x00212668 File Offset: 0x00210868
	private void ParticleAttractor(ParticleSystem particleSystem, ParticleSystem.Particle[] particles, int particleCount)
	{
		for (int i = 0; i < particleCount; i++)
		{
			foreach (ParticleEffectsAttractor particleEffectsAttractor in this.m_ParticleAttractors)
			{
				if (!(particleEffectsAttractor.m_Transform == null) && particleEffectsAttractor.m_Radius > 0f && particleEffectsAttractor.m_Power > 0f)
				{
					Vector3 vector = particles[i].position;
					if (!this.m_WorldSpace)
					{
						vector = particleSystem.transform.TransformPoint(particles[i].position);
					}
					Vector3 vector2 = particleEffectsAttractor.m_Transform.position - vector;
					float num = particleEffectsAttractor.m_Radius * ((particleEffectsAttractor.m_Transform.lossyScale.x + particleEffectsAttractor.m_Transform.lossyScale.y + particleEffectsAttractor.m_Transform.lossyScale.z) * 0.333f);
					float num2 = (1f - vector2.magnitude / num) * particleEffectsAttractor.m_Power;
					Vector3 vector3 = vector2 * particles[i].velocity.magnitude;
					if (!this.m_WorldSpace)
					{
						vector3 = particleSystem.transform.InverseTransformDirection(vector2 * particles[i].velocity.magnitude);
					}
					Vector3 velocity = Vector3.Lerp(particles[i].velocity, vector3, num2 * Time.deltaTime).normalized * particles[i].velocity.magnitude;
					particles[i].velocity = velocity;
				}
			}
		}
	}

	// Token: 0x06007097 RID: 28823 RVA: 0x00212858 File Offset: 0x00210A58
	private void ParticleRepulser(ParticleSystem particleSystem, ParticleSystem.Particle[] particles, int particleCount)
	{
		for (int i = 0; i < particleCount; i++)
		{
			foreach (ParticleEffectsRepulser particleEffectsRepulser in this.m_ParticleRepulsers)
			{
				if (!(particleEffectsRepulser.m_Transform == null) && particleEffectsRepulser.m_Radius > 0f && particleEffectsRepulser.m_Power > 0f)
				{
					Vector3 vector = particles[i].position;
					if (!this.m_WorldSpace)
					{
						vector = particleSystem.transform.TransformPoint(particles[i].position);
					}
					Vector3 vector2 = particleEffectsRepulser.m_Transform.position - vector;
					float num = particleEffectsRepulser.m_Radius * ((particleEffectsRepulser.m_Transform.lossyScale.x + particleEffectsRepulser.m_Transform.lossyScale.y + particleEffectsRepulser.m_Transform.lossyScale.z) * 0.333f);
					float num2 = (1f - vector2.magnitude / num) * particleEffectsRepulser.m_Power + particleEffectsRepulser.m_Power;
					Vector3 vector3 = -vector2 * particles[i].velocity.magnitude;
					if (!this.m_WorldSpace)
					{
						vector3 = particleSystem.transform.InverseTransformDirection(-vector2 * particles[i].velocity.magnitude);
					}
					Vector3 velocity = Vector3.Lerp(particles[i].velocity, vector3, num2 * Time.deltaTime).normalized * particles[i].velocity.magnitude;
					particles[i].velocity = velocity;
				}
			}
		}
	}

	// Token: 0x06007098 RID: 28824 RVA: 0x00212A5C File Offset: 0x00210C5C
	private static float VectorAngle(Vector3 forwardVector, Vector3 targetVector, Vector3 upVector)
	{
		float num = Vector3.Angle(forwardVector, targetVector);
		Vector3 vector = Vector3.Cross(forwardVector, targetVector);
		float num2 = Vector3.Dot(vector, upVector);
		if (num2 < 0f)
		{
			return 360f - num;
		}
		return num;
	}

	// Token: 0x040059DE RID: 23006
	public List<ParticleSystem> m_ParticleSystems;

	// Token: 0x040059DF RID: 23007
	public bool m_WorldSpace;

	// Token: 0x040059E0 RID: 23008
	public ParticleEffectsOrientation m_ParticleOrientation;

	// Token: 0x040059E1 RID: 23009
	public List<ParticleEffectsAttractor> m_ParticleAttractors;

	// Token: 0x040059E2 RID: 23010
	public List<ParticleEffectsRepulser> m_ParticleRepulsers;
}
