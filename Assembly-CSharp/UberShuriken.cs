using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F4F RID: 3919
[ExecuteInEditMode]
public class UberShuriken : MonoBehaviour
{
	// Token: 0x06007499 RID: 29849 RVA: 0x00226720 File Offset: 0x00224920
	private void Awake()
	{
		this.UpdateParticleSystemList();
	}

	// Token: 0x0600749A RID: 29850 RVA: 0x00226728 File Offset: 0x00224928
	private void Update()
	{
		this.m_time = Time.time;
		this.UpdateParticles();
	}

	// Token: 0x0600749B RID: 29851 RVA: 0x0022673C File Offset: 0x0022493C
	private void UpdateParticles()
	{
		foreach (ParticleSystem particleSystem in this.m_particleSystems)
		{
			if (!(particleSystem == null))
			{
				int particleCount = particleSystem.particleCount;
				if (particleCount == 0)
				{
					break;
				}
				ParticleSystem.Particle[] array = new ParticleSystem.Particle[particleCount];
				particleSystem.GetParticles(array);
				if (this.m_CurlNoise)
				{
					this.ParticleCurlNoise(particleSystem, array, particleCount);
				}
				if (this.m_Twinkle)
				{
					this.ParticleTwinkle(particleSystem, array, particleCount);
				}
				particleSystem.SetParticles(array, particleCount);
			}
		}
	}

	// Token: 0x0600749C RID: 29852 RVA: 0x002267F4 File Offset: 0x002249F4
	private void UpdateParticleSystemList()
	{
		this.m_particleSystems.Clear();
		if (this.m_IncludeChildren)
		{
			ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
			if (base.GetComponent<ParticleSystem>() == null || componentsInChildren.Length == 0)
			{
				Debug.LogError("Failed to find a ParticleSystem");
			}
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				this.m_particleSystems.Add(particleSystem);
			}
		}
		else
		{
			ParticleSystem component = base.GetComponent<ParticleSystem>();
			if (component == null)
			{
				Debug.LogError("Failed to find a ParticleSystem");
			}
			this.m_particleSystems.Add(component);
		}
	}

	// Token: 0x0600749D RID: 29853 RVA: 0x00226898 File Offset: 0x00224A98
	private void ParticleCurlNoise(ParticleSystem particleSystem, ParticleSystem.Particle[] particles, int particleCount)
	{
		float time = this.m_time;
		float num = this.m_CurlNoiseAnimation.x * time;
		float num2 = this.m_CurlNoiseAnimation.y * time;
		float num3 = this.m_CurlNoiseAnimation.z * time;
		for (int i = 0; i < particleCount; i++)
		{
			float num4 = 1f - particles[i].lifetime / particles[i].startLifetime;
			float num5 = this.m_CurlNoiseOverLifetime.Evaluate(num4) * this.m_CurlNoisePower;
			Vector3 velocity = particles[i].velocity;
			Vector3 vector = particles[i].position * this.m_CurlNoiseScale * 0.1f;
			velocity.x += UberMath.SimplexNoise(vector.x + num, vector.y + num2, vector.z + num3) * num5;
			velocity.y += UberMath.SimplexNoise(vector.y + num, vector.z + num2, vector.x + num3) * num5;
			velocity.z += UberMath.SimplexNoise(vector.z + num, vector.x + num2, vector.y + num3) * num5;
			velocity = velocity.normalized * particles[i].velocity.magnitude;
			particles[i].velocity = velocity;
		}
	}

	// Token: 0x0600749E RID: 29854 RVA: 0x00226A20 File Offset: 0x00224C20
	private void ParticleTwinkle(ParticleSystem particleSystem, ParticleSystem.Particle[] particles, int particleCount)
	{
		for (int i = 0; i < particleCount; i++)
		{
			float num = particles[i].lifetime / particles[i].startLifetime;
			Vector3 position = particles[i].position;
			Color color = particles[i].color;
			color.a = Mathf.Clamp01(UberMath.SimplexNoise((position.x + position.y + position.z - num - (float)i * 3.33f) * this.m_TwinkleRate, 0.5f) + this.m_TwinkleBias + num * this.m_TwinkleOverLifetime.Evaluate(num));
			particles[i].color = color;
		}
	}

	// Token: 0x04005F00 RID: 24320
	public bool m_IncludeChildren;

	// Token: 0x04005F01 RID: 24321
	public bool m_CurlNoise;

	// Token: 0x04005F02 RID: 24322
	public float m_CurlNoisePower = 1f;

	// Token: 0x04005F03 RID: 24323
	public AnimationCurve m_CurlNoiseOverLifetime = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04005F04 RID: 24324
	public float m_CurlNoiseScale = 1f;

	// Token: 0x04005F05 RID: 24325
	public Vector3 m_CurlNoiseAnimation = Vector3.zero;

	// Token: 0x04005F06 RID: 24326
	public bool m_Twinkle;

	// Token: 0x04005F07 RID: 24327
	public float m_TwinkleRate = 1f;

	// Token: 0x04005F08 RID: 24328
	[Range(-1f, 1f)]
	public float m_TwinkleBias;

	// Token: 0x04005F09 RID: 24329
	public AnimationCurve m_TwinkleOverLifetime = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04005F0A RID: 24330
	private List<ParticleSystem> m_particleSystems = new List<ParticleSystem>();

	// Token: 0x04005F0B RID: 24331
	private float m_time;
}
