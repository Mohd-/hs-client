using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F11 RID: 3857
[CustomEditClass]
public class LightningAnimator : MonoBehaviour
{
	// Token: 0x0600731F RID: 29471 RVA: 0x0021E4BC File Offset: 0x0021C6BC
	private void Start()
	{
		this.m_material = base.GetComponent<Renderer>().material;
		if (this.m_material == null)
		{
			base.enabled = false;
		}
		if (this.m_SetAlphaToZeroOnStart)
		{
			Color color = this.m_material.color;
			color.a = 0f;
			this.m_material.color = color;
		}
		if (this.m_material.HasProperty("_GlowIntensity"))
		{
			this.m_matGlowIntensity = this.m_material.GetFloat("_GlowIntensity");
		}
	}

	// Token: 0x06007320 RID: 29472 RVA: 0x0021E54C File Offset: 0x0021C74C
	private void OnEnable()
	{
		if (this.m_StartOnEnable)
		{
			this.StartAnimation();
		}
	}

	// Token: 0x06007321 RID: 29473 RVA: 0x0021E55F File Offset: 0x0021C75F
	public void StartAnimation()
	{
		base.StartCoroutine(this.AnimateMaterial());
	}

	// Token: 0x06007322 RID: 29474 RVA: 0x0021E570 File Offset: 0x0021C770
	private IEnumerator AnimateMaterial()
	{
		this.RandomJointRotation();
		Color matColor = this.m_material.color;
		matColor.a = 0f;
		this.m_material.color = matColor;
		yield return new WaitForSeconds(Random.Range(this.m_StartDelayMin, this.m_StartDelayMax));
		matColor = this.m_material.color;
		matColor.a = 1f;
		this.m_material.color = matColor;
		if (this.m_material.HasProperty("_GlowIntensity"))
		{
			this.m_material.SetFloat("_GlowIntensity", this.m_matGlowIntensity);
		}
		foreach (int frame in this.m_FrameList)
		{
			this.m_material.SetFloat(this.m_MatFrameProperty, (float)frame);
			yield return new WaitForSeconds(this.m_FrameTime);
		}
		matColor.a = 0f;
		this.m_material.color = matColor;
		if (this.m_material.HasProperty("_GlowIntensity"))
		{
			this.m_material.SetFloat("_GlowIntensity", 0f);
		}
		yield break;
	}

	// Token: 0x06007323 RID: 29475 RVA: 0x0021E58C File Offset: 0x0021C78C
	private void RandomJointRotation()
	{
		if (this.m_SourceJount != null)
		{
			Vector3 vector = Vector3.Lerp(this.m_SourceMinRotation, this.m_SourceMaxRotation, Random.value);
			this.m_SourceJount.Rotate(vector);
		}
		if (this.m_TargetJoint != null)
		{
			Vector3 vector2 = Vector3.Lerp(this.m_TargetMinRotation, this.m_TargetMaxRotation, Random.value);
			this.m_TargetJoint.Rotate(vector2);
		}
	}

	// Token: 0x04005D80 RID: 23936
	public bool m_StartOnEnable;

	// Token: 0x04005D81 RID: 23937
	public bool m_SetAlphaToZeroOnStart = true;

	// Token: 0x04005D82 RID: 23938
	public float m_StartDelayMin;

	// Token: 0x04005D83 RID: 23939
	public float m_StartDelayMax;

	// Token: 0x04005D84 RID: 23940
	public string m_MatFrameProperty = "_Frame";

	// Token: 0x04005D85 RID: 23941
	public float m_FrameTime = 0.01f;

	// Token: 0x04005D86 RID: 23942
	public List<int> m_FrameList;

	// Token: 0x04005D87 RID: 23943
	public Transform m_SourceJount;

	// Token: 0x04005D88 RID: 23944
	public Vector3 m_SourceMinRotation = new Vector3(0f, -10f, 0f);

	// Token: 0x04005D89 RID: 23945
	public Vector3 m_SourceMaxRotation = new Vector3(0f, 10f, 0f);

	// Token: 0x04005D8A RID: 23946
	public Transform m_TargetJoint;

	// Token: 0x04005D8B RID: 23947
	public Vector3 m_TargetMinRotation = new Vector3(0f, -20f, 0f);

	// Token: 0x04005D8C RID: 23948
	public Vector3 m_TargetMaxRotation = new Vector3(0f, 20f, 0f);

	// Token: 0x04005D8D RID: 23949
	private Material m_material;

	// Token: 0x04005D8E RID: 23950
	private float m_matGlowIntensity;
}
