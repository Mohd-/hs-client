using System;
using UnityEngine;

// Token: 0x02000F2E RID: 3886
public class RandomWeather : MonoBehaviour
{
	// Token: 0x060073B0 RID: 29616 RVA: 0x00221224 File Offset: 0x0021F424
	private void Start()
	{
		this.m_particleSystems = base.GetComponentsInChildren<ParticleSystem>();
		this.m_startTime = Random.Range(Time.timeSinceLevelLoad + this.m_StartDelayMinMinutes * 60f, Time.timeSinceLevelLoad + this.m_StartDelayMaxMinutes * 60f);
	}

	// Token: 0x060073B1 RID: 29617 RVA: 0x0022126C File Offset: 0x0021F46C
	private void Update()
	{
		if (this.m_active)
		{
			if (Time.timeSinceLevelLoad > this.m_runEndTime)
			{
				this.StopWeather();
			}
		}
		else if (Time.timeSinceLevelLoad > this.m_startTime)
		{
			this.StartWeather();
			this.m_startTime = Random.Range(Time.timeSinceLevelLoad + this.m_StartDelayMinMinutes * 60f, Time.timeSinceLevelLoad + this.m_StartDelayMaxMinutes * 60f);
		}
	}

	// Token: 0x060073B2 RID: 29618 RVA: 0x002212E4 File Offset: 0x0021F4E4
	[ContextMenu("Start Weather")]
	private void StartWeather()
	{
		this.m_active = true;
		this.m_runEndTime = Random.Range(Time.timeSinceLevelLoad + this.m_WeatherMinMinutes * 60f, Time.timeSinceLevelLoad + this.m_WeatherMaxMinutes * 60f);
		foreach (ParticleSystem particleSystem in this.m_particleSystems)
		{
			if (!(particleSystem == null))
			{
				particleSystem.Play();
			}
		}
	}

	// Token: 0x060073B3 RID: 29619 RVA: 0x00221360 File Offset: 0x0021F560
	[ContextMenu("Stop Weather")]
	private void StopWeather()
	{
		this.m_active = false;
		foreach (ParticleSystem particleSystem in this.m_particleSystems)
		{
			if (!(particleSystem == null))
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x04005E41 RID: 24129
	public float m_StartDelayMinMinutes = 1f;

	// Token: 0x04005E42 RID: 24130
	public float m_StartDelayMaxMinutes = 10f;

	// Token: 0x04005E43 RID: 24131
	public float m_WeatherMinMinutes = 2f;

	// Token: 0x04005E44 RID: 24132
	public float m_WeatherMaxMinutes = 5f;

	// Token: 0x04005E45 RID: 24133
	private ParticleSystem[] m_particleSystems;

	// Token: 0x04005E46 RID: 24134
	private float m_startTime;

	// Token: 0x04005E47 RID: 24135
	private float m_runEndTime;

	// Token: 0x04005E48 RID: 24136
	private bool m_active;
}
