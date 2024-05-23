using System;
using UnityEngine;

// Token: 0x02000F18 RID: 3864
public class MeshAnimation : MonoBehaviour
{
	// Token: 0x06007342 RID: 29506 RVA: 0x0021F4B7 File Offset: 0x0021D6B7
	private void Start()
	{
		this.m_Mesh = base.GetComponent<MeshFilter>();
	}

	// Token: 0x06007343 RID: 29507 RVA: 0x0021F4C8 File Offset: 0x0021D6C8
	private void Update()
	{
		if (!this.m_Playing)
		{
			return;
		}
		this.m_FrameTime += Time.deltaTime;
		if (this.m_FrameTime >= this.FrameDuration)
		{
			this.m_Index = (this.m_Index + 1) % this.Meshes.Length;
			this.m_FrameTime -= this.FrameDuration;
			if (!this.Loop && this.m_Index == 0)
			{
				this.m_Playing = false;
				base.enabled = false;
				return;
			}
			this.m_Mesh.mesh = this.Meshes[this.m_Index];
		}
	}

	// Token: 0x06007344 RID: 29508 RVA: 0x0021F56B File Offset: 0x0021D76B
	public void Play()
	{
		base.enabled = true;
		this.m_Playing = true;
	}

	// Token: 0x06007345 RID: 29509 RVA: 0x0021F57B File Offset: 0x0021D77B
	public void Stop()
	{
		this.m_Playing = false;
		base.enabled = false;
	}

	// Token: 0x06007346 RID: 29510 RVA: 0x0021F58B File Offset: 0x0021D78B
	public void Reset()
	{
		this.m_Mesh.mesh = this.Meshes[0];
		this.m_FrameTime = 0f;
		this.m_Index = 0;
	}

	// Token: 0x04005DC3 RID: 24003
	public Mesh[] Meshes;

	// Token: 0x04005DC4 RID: 24004
	public bool Loop;

	// Token: 0x04005DC5 RID: 24005
	public float FrameDuration;

	// Token: 0x04005DC6 RID: 24006
	private int m_Index;

	// Token: 0x04005DC7 RID: 24007
	private bool m_Playing;

	// Token: 0x04005DC8 RID: 24008
	private float m_FrameTime;

	// Token: 0x04005DC9 RID: 24009
	private MeshFilter m_Mesh;
}
