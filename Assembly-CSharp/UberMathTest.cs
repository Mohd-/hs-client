using System;
using UnityEngine;

// Token: 0x02000F73 RID: 3955
public class UberMathTest : MonoBehaviour
{
	// Token: 0x0600753F RID: 30015 RVA: 0x002298D0 File Offset: 0x00227AD0
	private void Start()
	{
		this.m_particleGameObject = new GameObject("UberMathParticles");
		this.m_particleGameObject.transform.position = Vector3.zero;
		this.m_particleSystem = this.m_particleGameObject.AddComponent<ParticleSystem>();
		this.m_particleSystem.loop = false;
		this.m_particleSystem.emissionRate = 0f;
		this.Simplex2DTest();
	}

	// Token: 0x06007540 RID: 30016 RVA: 0x00229938 File Offset: 0x00227B38
	private void Update()
	{
		this.m_time = Time.timeSinceLevelLoad * this.m_TimeScale;
		this.m_gridIncrement = this.m_GridSize / (float)(this.m_Resolution - 1);
		this.Update2Dparticles();
	}

	// Token: 0x06007541 RID: 30017 RVA: 0x00229973 File Offset: 0x00227B73
	private void Simplex2DTest()
	{
		this.CreateParticles(this.m_Resolution);
	}

	// Token: 0x06007542 RID: 30018 RVA: 0x00229984 File Offset: 0x00227B84
	private void CreateParticles(int res)
	{
		this.m_particles = new ParticleSystem.Particle[res * res];
		int num = 0;
		for (int i = 0; i < res; i++)
		{
			for (int j = 0; j < res; j++)
			{
				Vector3 position;
				position..ctor((float)i * this.m_gridIncrement, 0f, (float)j * this.m_gridIncrement);
				this.m_particles[num].position = position;
				this.m_particles[num].color = Color.white;
				this.m_particles[num].size = this.m_ParticleSize;
				num++;
			}
		}
		Debug.LogFormat("Particle count: {0}", new object[]
		{
			this.m_particles.Length
		});
		this.m_particleSystem.SetParticles(this.m_particles, this.m_particles.Length);
	}

	// Token: 0x06007543 RID: 30019 RVA: 0x00229A64 File Offset: 0x00227C64
	private void Update2Dparticles()
	{
		int num = 0;
		for (int i = 0; i < this.m_Resolution; i++)
		{
			for (int j = 0; j < this.m_Resolution; j++)
			{
				float num2 = UberMath.SimplexNoise((float)i * this.m_NoiseScale, this.m_time * this.m_NoiseScale, (float)j * this.m_NoiseScale);
				Vector3 position = this.m_particles[num].position;
				position.x = (float)i * this.m_gridIncrement;
				position.y = num2 * 0.1f;
				position.z = (float)j * this.m_gridIncrement;
				this.m_particles[num].position = position;
				Color white = Color.white;
				white.r = (white.g = (white.b = (num2 + 1f) * 0.5f));
				this.m_particles[num].color = white;
				num++;
			}
		}
		this.m_particleSystem.SetParticles(this.m_particles, this.m_particles.Length);
	}

	// Token: 0x04005FB7 RID: 24503
	public float m_NoiseScale = 0.01f;

	// Token: 0x04005FB8 RID: 24504
	public int m_Resolution = 256;

	// Token: 0x04005FB9 RID: 24505
	public float m_GridSize = 1f;

	// Token: 0x04005FBA RID: 24506
	public float m_ParticleSize = 0.1f;

	// Token: 0x04005FBB RID: 24507
	public float m_TimeScale;

	// Token: 0x04005FBC RID: 24508
	private GameObject m_particleGameObject;

	// Token: 0x04005FBD RID: 24509
	private ParticleSystem m_particleSystem;

	// Token: 0x04005FBE RID: 24510
	private ParticleSystem.Particle[] m_particles;

	// Token: 0x04005FBF RID: 24511
	private float m_gridIncrement;

	// Token: 0x04005FC0 RID: 24512
	private float m_time;
}
