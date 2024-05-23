using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F24 RID: 3876
[ExecuteInEditMode]
public class QuickBlendShape : MonoBehaviour
{
	// Token: 0x0600737E RID: 29566 RVA: 0x00220470 File Offset: 0x0021E670
	private void Awake()
	{
		this.m_MeshFilter = base.GetComponent<MeshFilter>();
		this.m_OrgMesh = this.m_MeshFilter.sharedMesh;
		if (this.m_DisableOnMobile && UniversalInputManager.Get().IsTouchMode())
		{
			MeshFilter component = base.GetComponent<MeshFilter>();
			if (component.sharedMesh == null && this.m_Meshes.Length > 0 && this.m_Meshes[0] != null)
			{
				component.sharedMesh = this.m_Meshes[0];
			}
			base.enabled = false;
			return;
		}
		this.CreateBlendMeshes();
	}

	// Token: 0x0600737F RID: 29567 RVA: 0x00220509 File Offset: 0x0021E709
	private void Update()
	{
		if (this.m_DisableOnMobile && UniversalInputManager.Get().IsTouchMode())
		{
			return;
		}
		if (!this.m_Play && this.m_AnimationType != QuickBlendShape.BLEND_SHAPE_ANIMATION_TYPE.Float)
		{
			return;
		}
		this.BlendShapeAnimate();
	}

	// Token: 0x06007380 RID: 29568 RVA: 0x00220544 File Offset: 0x0021E744
	private void OnEnable()
	{
		if (this.m_DisableOnMobile && UniversalInputManager.Get().IsTouchMode())
		{
			return;
		}
		if (this.m_PlayOnAwake)
		{
			this.PlayAnimation();
		}
	}

	// Token: 0x06007381 RID: 29569 RVA: 0x00220580 File Offset: 0x0021E780
	private void OnDisable()
	{
		if (this.m_DisableOnMobile && UniversalInputManager.Get().IsTouchMode())
		{
			this.m_MeshFilter.sharedMesh = this.m_OrgMesh;
			return;
		}
		this.m_animTime = 0f;
		if (this.m_BlendMaterials != null)
		{
			foreach (Material material in this.m_BlendMaterials)
			{
				material.SetFloat("_Blend", 0f);
			}
		}
		if (this.m_MeshFilter != null && this.m_OrgMesh != null)
		{
			this.m_MeshFilter.sharedMesh = this.m_OrgMesh;
		}
	}

	// Token: 0x06007382 RID: 29570 RVA: 0x00220634 File Offset: 0x0021E834
	public void PlayAnimation()
	{
		if (this.m_DisableOnMobile && UniversalInputManager.Get().IsTouchMode())
		{
			return;
		}
		if (this.m_MeshFilter == null)
		{
			return;
		}
		if (this.m_Meshes == null)
		{
			return;
		}
		if (this.m_BlendMeshes == null)
		{
			return;
		}
		this.m_animTime = 0f;
		this.m_Play = true;
	}

	// Token: 0x06007383 RID: 29571 RVA: 0x00220698 File Offset: 0x0021E898
	public void StopAnimation()
	{
		if (this.m_DisableOnMobile && UniversalInputManager.Get().IsTouchMode())
		{
			return;
		}
		this.m_Play = false;
	}

	// Token: 0x06007384 RID: 29572 RVA: 0x002206C8 File Offset: 0x0021E8C8
	private void BlendShapeAnimate()
	{
		if (this.m_BlendMaterials == null)
		{
			this.m_BlendMaterials = base.GetComponent<Renderer>().materials;
		}
		if (this.m_MeshFilter == null)
		{
			this.m_MeshFilter = base.GetComponent<MeshFilter>();
		}
		float time = this.m_BlendCurve.keys[this.m_BlendCurve.length - 1].time;
		this.m_animTime += Time.deltaTime;
		float num = this.m_BlendValue;
		if (num < 0f)
		{
			return;
		}
		if (this.m_AnimationType == QuickBlendShape.BLEND_SHAPE_ANIMATION_TYPE.Curve)
		{
			num = this.m_BlendCurve.Evaluate(this.m_animTime);
		}
		int num2 = Mathf.FloorToInt(num);
		if (num2 > this.m_BlendMeshes.Count - 1)
		{
			num2 -= this.m_BlendMeshes.Count - 1;
		}
		this.m_MeshFilter.mesh = this.m_BlendMeshes[num2];
		foreach (Material material in this.m_BlendMaterials)
		{
			material.SetFloat("_Blend", num - (float)Mathf.FloorToInt(num));
		}
		if (this.m_animTime > time)
		{
			if (this.m_Loop)
			{
				this.m_animTime = 0f;
			}
			else
			{
				this.m_Play = false;
			}
		}
	}

	// Token: 0x06007385 RID: 29573 RVA: 0x00220820 File Offset: 0x0021EA20
	private void CreateBlendMeshes()
	{
		this.m_BlendMeshes = new List<Mesh>();
		for (int i = 0; i < this.m_Meshes.Length; i++)
		{
			MeshFilter meshFilter = base.GetComponent<MeshFilter>();
			if (meshFilter == null)
			{
				meshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			Mesh mesh = Object.Instantiate<Mesh>(this.m_Meshes[i]);
			int num = this.m_Meshes[i].vertices.Length;
			int num2 = i + 1;
			if (num2 > this.m_Meshes.Length - 1)
			{
				num2 = 0;
			}
			Mesh mesh2 = this.m_Meshes[num2];
			Vector4[] array = new Vector4[num];
			for (int j = 0; j < num; j++)
			{
				if (j <= mesh2.vertices.Length - 1)
				{
					Vector3 vector = mesh2.vertices[j];
					array[j] = new Vector4(vector.x, vector.y, vector.z, 1f);
				}
			}
			mesh.vertices = this.m_Meshes[i].vertices;
			mesh.tangents = array;
			this.m_BlendMeshes.Add(mesh);
		}
	}

	// Token: 0x04005E09 RID: 24073
	public bool m_DisableOnMobile;

	// Token: 0x04005E0A RID: 24074
	public QuickBlendShape.BLEND_SHAPE_ANIMATION_TYPE m_AnimationType;

	// Token: 0x04005E0B RID: 24075
	public float m_BlendValue;

	// Token: 0x04005E0C RID: 24076
	public AnimationCurve m_BlendCurve;

	// Token: 0x04005E0D RID: 24077
	public bool m_Loop = true;

	// Token: 0x04005E0E RID: 24078
	public bool m_PlayOnAwake;

	// Token: 0x04005E0F RID: 24079
	public Mesh[] m_Meshes;

	// Token: 0x04005E10 RID: 24080
	private List<Mesh> m_BlendMeshes;

	// Token: 0x04005E11 RID: 24081
	private bool m_Play;

	// Token: 0x04005E12 RID: 24082
	private MeshFilter m_MeshFilter;

	// Token: 0x04005E13 RID: 24083
	private float m_animTime;

	// Token: 0x04005E14 RID: 24084
	private Material[] m_BlendMaterials;

	// Token: 0x04005E15 RID: 24085
	private Mesh m_OrgMesh;

	// Token: 0x02000F25 RID: 3877
	public enum BLEND_SHAPE_ANIMATION_TYPE
	{
		// Token: 0x04005E17 RID: 24087
		Curve,
		// Token: 0x04005E18 RID: 24088
		Float
	}
}
