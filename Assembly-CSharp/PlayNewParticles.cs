using System;
using UnityEngine;

// Token: 0x02000F20 RID: 3872
public class PlayNewParticles : MonoBehaviour
{
	// Token: 0x0600736C RID: 29548 RVA: 0x0021FFE4 File Offset: 0x0021E1E4
	public void PlayNewParticles3()
	{
		if (this.m_Target == null)
		{
			return;
		}
		this.m_Target.GetComponent<ParticleSystem>().Play();
	}

	// Token: 0x0600736D RID: 29549 RVA: 0x00220014 File Offset: 0x0021E214
	public void StopNewParticles3()
	{
		if (this.m_Target == null)
		{
			return;
		}
		this.m_Target.GetComponent<ParticleSystem>().Stop();
	}

	// Token: 0x0600736E RID: 29550 RVA: 0x00220043 File Offset: 0x0021E243
	public void PlayNewParticles3andChilds()
	{
		if (this.m_Target2 == null)
		{
			return;
		}
		this.m_Target2.GetComponent<ParticleSystem>().Play(true);
	}

	// Token: 0x0600736F RID: 29551 RVA: 0x00220068 File Offset: 0x0021E268
	public void StopNewParticles3andChilds()
	{
		if (this.m_Target2 == null)
		{
			return;
		}
		this.m_Target2.GetComponent<ParticleSystem>().Stop(true);
	}

	// Token: 0x06007370 RID: 29552 RVA: 0x0022008D File Offset: 0x0021E28D
	public void PlayNewParticles3andChilds2()
	{
		if (this.m_Target3 == null)
		{
			return;
		}
		this.m_Target3.GetComponent<ParticleSystem>().Play(true);
	}

	// Token: 0x06007371 RID: 29553 RVA: 0x002200B2 File Offset: 0x0021E2B2
	public void StopNewParticles3andChilds2()
	{
		if (this.m_Target3 == null)
		{
			return;
		}
		this.m_Target3.GetComponent<ParticleSystem>().Stop(true);
	}

	// Token: 0x06007372 RID: 29554 RVA: 0x002200D7 File Offset: 0x0021E2D7
	public void PlayNewParticles3andChilds3()
	{
		if (this.m_Target4 == null)
		{
			return;
		}
		this.m_Target4.GetComponent<ParticleSystem>().Play(true);
	}

	// Token: 0x06007373 RID: 29555 RVA: 0x002200FC File Offset: 0x0021E2FC
	public void StopNewParticles3andChilds3()
	{
		if (this.m_Target4 == null)
		{
			return;
		}
		this.m_Target4.GetComponent<ParticleSystem>().Stop(true);
	}

	// Token: 0x04005DFA RID: 24058
	public GameObject m_Target;

	// Token: 0x04005DFB RID: 24059
	public GameObject m_Target2;

	// Token: 0x04005DFC RID: 24060
	public GameObject m_Target3;

	// Token: 0x04005DFD RID: 24061
	public GameObject m_Target4;
}
