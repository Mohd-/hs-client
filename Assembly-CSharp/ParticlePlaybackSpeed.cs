using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F1C RID: 3868
[ExecuteInEditMode]
public class ParticlePlaybackSpeed : MonoBehaviour
{
	// Token: 0x06007358 RID: 29528 RVA: 0x0021FC30 File Offset: 0x0021DE30
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06007359 RID: 29529 RVA: 0x0021FC38 File Offset: 0x0021DE38
	private void Update()
	{
		if (this.m_ParticlePlaybackSpeed == this.m_PreviousPlaybackSpeed)
		{
			return;
		}
		this.m_PreviousPlaybackSpeed = this.m_ParticlePlaybackSpeed;
		int i = 0;
		while (i < this.m_ParticleSystems.Count)
		{
			ParticleSystem particleSystem = this.m_ParticleSystems[i];
			if (particleSystem)
			{
				particleSystem.playbackSpeed = this.m_ParticlePlaybackSpeed;
				i++;
			}
			else
			{
				this.m_OrgPlaybackSpeed.Remove(particleSystem);
				this.m_ParticleSystems.RemoveAt(i);
			}
		}
	}

	// Token: 0x0600735A RID: 29530 RVA: 0x0021FCC4 File Offset: 0x0021DEC4
	private void OnDisable()
	{
		if (this.m_RestoreSpeedOnDisable)
		{
			foreach (KeyValuePair<ParticleSystem, float> keyValuePair in this.m_OrgPlaybackSpeed)
			{
				ParticleSystem key = keyValuePair.Key;
				float value = keyValuePair.Value;
				if (key)
				{
					key.playbackSpeed = value;
				}
			}
		}
		this.m_PreviousPlaybackSpeed = -10000000f;
		this.m_ParticleSystems.Clear();
		this.m_OrgPlaybackSpeed.Clear();
	}

	// Token: 0x0600735B RID: 29531 RVA: 0x0021FD68 File Offset: 0x0021DF68
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x0600735C RID: 29532 RVA: 0x0021FD70 File Offset: 0x0021DF70
	private void Init()
	{
		if (this.m_ParticleSystems == null)
		{
			this.m_ParticleSystems = new List<ParticleSystem>();
		}
		else
		{
			this.m_ParticleSystems.Clear();
		}
		if (this.m_OrgPlaybackSpeed == null)
		{
			this.m_OrgPlaybackSpeed = new Map<ParticleSystem, float>();
		}
		else
		{
			this.m_OrgPlaybackSpeed.Clear();
		}
		foreach (ParticleSystem particleSystem in base.gameObject.GetComponentsInChildren<ParticleSystem>())
		{
			this.m_OrgPlaybackSpeed.Add(particleSystem, particleSystem.playbackSpeed);
			this.m_ParticleSystems.Add(particleSystem);
		}
	}

	// Token: 0x04005DE9 RID: 24041
	public float m_ParticlePlaybackSpeed = 1f;

	// Token: 0x04005DEA RID: 24042
	public bool m_RestoreSpeedOnDisable = true;

	// Token: 0x04005DEB RID: 24043
	private float m_PreviousPlaybackSpeed = 1f;

	// Token: 0x04005DEC RID: 24044
	private Map<ParticleSystem, float> m_OrgPlaybackSpeed;

	// Token: 0x04005DED RID: 24045
	private List<ParticleSystem> m_ParticleSystems;
}
