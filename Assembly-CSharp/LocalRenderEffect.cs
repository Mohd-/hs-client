using System;
using UnityEngine;

// Token: 0x02000F15 RID: 3861
public class LocalRenderEffect : MonoBehaviour
{
	// Token: 0x170009FB RID: 2555
	// (get) Token: 0x0600732B RID: 29483 RVA: 0x0021EA2C File Offset: 0x0021CC2C
	protected Material BloomMaterial
	{
		get
		{
			if (this.m_BloomMaterial == null)
			{
				this.m_BloomMaterial = new Material(this.m_BloomShader);
				SceneUtils.SetHideFlags(this.m_BloomMaterial, 52);
			}
			return this.m_BloomMaterial;
		}
	}

	// Token: 0x170009FC RID: 2556
	// (get) Token: 0x0600732C RID: 29484 RVA: 0x0021EA63 File Offset: 0x0021CC63
	protected Material BlurMaterial
	{
		get
		{
			if (this.m_BlurMaterial == null)
			{
				this.m_BlurMaterial = new Material(this.m_BlurShader);
				SceneUtils.SetHideFlags(this.m_BlurMaterial, 52);
			}
			return this.m_BlurMaterial;
		}
	}

	// Token: 0x170009FD RID: 2557
	// (get) Token: 0x0600732D RID: 29485 RVA: 0x0021EA9A File Offset: 0x0021CC9A
	protected Material AdditiveMaterial
	{
		get
		{
			if (this.m_AdditiveMaterial == null)
			{
				this.m_AdditiveMaterial = new Material(this.m_AdditiveShader);
				SceneUtils.SetHideFlags(this.m_AdditiveMaterial, 52);
			}
			return this.m_AdditiveMaterial;
		}
	}

	// Token: 0x0600732E RID: 29486 RVA: 0x0021EAD4 File Offset: 0x0021CCD4
	protected void Start()
	{
		this.m_BloomShader = ShaderUtils.FindShader(this.BLOOM_SHADER_NAME);
		if (!this.m_BloomShader)
		{
			Debug.LogError("Failed to load Local Rendering Effect Shader: " + this.BLOOM_SHADER_NAME);
		}
		this.m_BlurShader = ShaderUtils.FindShader(this.BLUR_SHADER_NAME);
		if (!this.m_BlurShader)
		{
			Debug.LogError("Failed to load Local Rendering Effect Shader: " + this.BLUR_SHADER_NAME);
		}
		this.m_AdditiveShader = ShaderUtils.FindShader(this.ADDITIVE_SHADER_NAME);
		if (!this.m_AdditiveShader)
		{
			Debug.LogError("Failed to load Local Rendering Effect Shader: " + this.ADDITIVE_SHADER_NAME);
		}
		this.CreateTexture();
		this.CreateCamera();
		this.CreateRenderPlane();
	}

	// Token: 0x0600732F RID: 29487 RVA: 0x0021EB95 File Offset: 0x0021CD95
	protected void Update()
	{
		this.Render();
	}

	// Token: 0x06007330 RID: 29488 RVA: 0x0021EBA0 File Offset: 0x0021CDA0
	protected void OnDestroy()
	{
		if (this.m_Camera)
		{
			this.m_Camera.targetTexture = null;
			this.m_Camera.enabled = false;
			Object.Destroy(this.m_Camera);
			Object.Destroy(this.m_CameraGO);
		}
		RenderTexture.active = null;
		if (this.m_BloomMaterial == null)
		{
			Object.Destroy(this.m_BloomMaterial);
		}
		if (this.m_BlurMaterial == null)
		{
			Object.Destroy(this.m_BlurMaterial);
		}
		if (this.m_AdditiveMaterial == null)
		{
			Object.Destroy(this.m_AdditiveMaterial);
		}
		if (this.m_RenderTexture)
		{
			Object.Destroy(this.m_RenderTexture);
		}
	}

	// Token: 0x06007331 RID: 29489 RVA: 0x0021EC60 File Offset: 0x0021CE60
	private void OnDrawGizmos()
	{
		if (this.m_Depth < 0f)
		{
			this.m_Depth = 0f;
		}
		Vector3 vector;
		vector..ctor(base.transform.position.x, base.transform.position.y - this.m_Depth * 0.5f, base.transform.position.z);
		Gizmos.DrawWireCube(vector, new Vector3(this.m_Width, this.m_Depth, this.m_Height));
	}

	// Token: 0x06007332 RID: 29490 RVA: 0x0021ECF4 File Offset: 0x0021CEF4
	public void Render()
	{
		if (this.m_Effect == localRenderEffects.Bloom || this.m_Effect == localRenderEffects.Blur)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(this.m_RenderTexture.width, this.m_RenderTexture.height, this.m_RenderTexture.depth, this.m_RenderTexture.format);
			this.m_Camera.targetTexture = temporary;
			this.m_Camera.Render();
			this.Sample(temporary, this.m_RenderTexture, this.BlurMaterial, this.m_BlurAmount);
			RenderTexture.ReleaseTemporary(temporary);
			this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial = this.BloomMaterial;
			this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", this.m_Color);
			this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = this.m_RenderTexture;
		}
		else
		{
			this.m_Camera.targetTexture = this.m_RenderTexture;
			this.m_Camera.Render();
		}
	}

	// Token: 0x06007333 RID: 29491 RVA: 0x0021EDF4 File Offset: 0x0021CFF4
	private void CreateTexture()
	{
		if (this.m_RenderTexture != null)
		{
			return;
		}
		Vector2 vector = this.CalcTextureSize();
		this.m_RenderTexture = new RenderTexture((int)vector.x, (int)vector.y, 32, 0);
	}

	// Token: 0x06007334 RID: 29492 RVA: 0x0021EE38 File Offset: 0x0021D038
	private void CreateCamera()
	{
		if (this.m_Camera != null)
		{
			return;
		}
		this.m_CameraGO = new GameObject();
		this.m_Camera = this.m_CameraGO.AddComponent<Camera>();
		this.m_CameraGO.name = base.name + "_TextRenderCamera";
		SceneUtils.SetHideFlags(this.m_CameraGO, 61);
		this.m_Camera.orthographic = true;
		this.m_CameraGO.transform.parent = base.transform;
		this.m_CameraGO.transform.rotation = Quaternion.identity;
		this.m_CameraGO.transform.position = base.transform.position;
		this.m_CameraGO.transform.Rotate(90f, 0f, 0f);
		this.m_Camera.nearClipPlane = 0f;
		this.m_Camera.farClipPlane = this.m_Depth;
		this.m_Camera.depth = Camera.main.depth - 1f;
		this.m_Camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		this.m_Camera.clearFlags = 2;
		this.m_Camera.depthTextureMode = 0;
		this.m_Camera.renderingPath = 1;
		this.m_Camera.cullingMask = this.m_LayerMask;
		this.m_Camera.targetTexture = this.m_RenderTexture;
		this.m_Camera.enabled = false;
		this.m_Camera.orthographicSize = Mathf.Min(this.m_Width * 0.5f, this.m_Height * 0.5f);
	}

	// Token: 0x06007335 RID: 29493 RVA: 0x0021EFEC File Offset: 0x0021D1EC
	private Vector2 CalcTextureSize()
	{
		float num = this.m_Height / this.m_Width;
		Vector2 result;
		result..ctor((float)this.m_Resolution, (float)this.m_Resolution * num);
		return result;
	}

	// Token: 0x06007336 RID: 29494 RVA: 0x0021F020 File Offset: 0x0021D220
	private void CreateRenderPlane()
	{
		if (this.m_Width == this.m_PreviousWidth && this.m_Height == this.m_PreviousHeight)
		{
			return;
		}
		if (this.m_PlaneGameObject != null)
		{
			Object.Destroy(this.m_PlaneGameObject);
		}
		this.m_PlaneGameObject = base.gameObject;
		this.m_PlaneGameObject.AddComponent<MeshFilter>();
		this.m_PlaneGameObject.AddComponent<MeshRenderer>();
		Mesh mesh = new Mesh();
		mesh.name = "TextMeshPlane";
		float num = this.m_Width * 0.5f;
		float num2 = this.m_Height * 0.5f;
		mesh.vertices = new Vector3[]
		{
			new Vector3(-num, this.RENDER_PLANE_OFFSET, -num2),
			new Vector3(num, this.RENDER_PLANE_OFFSET, -num2),
			new Vector3(-num, this.RENDER_PLANE_OFFSET, num2),
			new Vector3(num, this.RENDER_PLANE_OFFSET, num2)
		};
		mesh.uv = this.PLANE_UVS;
		mesh.normals = this.PLANE_NORMALS;
		mesh.triangles = this.PLANE_TRIANGLES;
		Mesh mesh2 = mesh;
		this.m_PlaneGameObject.GetComponent<MeshFilter>().mesh = mesh2;
		this.m_PlaneMesh = mesh2;
		this.m_PlaneMesh.RecalculateBounds();
		this.BloomMaterial.mainTexture = this.m_RenderTexture;
		this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial = this.AdditiveMaterial;
		this.m_PreviousWidth = this.m_Width;
		this.m_PreviousHeight = this.m_Height;
	}

	// Token: 0x06007337 RID: 29495 RVA: 0x0021F1B8 File Offset: 0x0021D3B8
	private void Sample(RenderTexture source, RenderTexture dest, Material sampleMat, float offset)
	{
		dest.DiscardContents();
		Graphics.BlitMultiTap(source, dest, sampleMat, new Vector2[]
		{
			new Vector2(-offset, -offset),
			new Vector2(-offset, offset),
			new Vector2(offset, offset),
			new Vector2(offset, -offset)
		});
	}

	// Token: 0x04005D9C RID: 23964
	private readonly string ADDITIVE_SHADER_NAME = "Hero/Additive";

	// Token: 0x04005D9D RID: 23965
	private readonly string BLOOM_SHADER_NAME = "Hidden/LocalRenderBloom";

	// Token: 0x04005D9E RID: 23966
	private readonly string BLUR_SHADER_NAME = "Hidden/LocalRenderBlur";

	// Token: 0x04005D9F RID: 23967
	private readonly float RENDER_PLANE_OFFSET = 0.05f;

	// Token: 0x04005DA0 RID: 23968
	public localRenderEffects m_Effect;

	// Token: 0x04005DA1 RID: 23969
	public int m_Resolution = 128;

	// Token: 0x04005DA2 RID: 23970
	public float m_Width = 1f;

	// Token: 0x04005DA3 RID: 23971
	public float m_Height = 1f;

	// Token: 0x04005DA4 RID: 23972
	public float m_Depth = 5f;

	// Token: 0x04005DA5 RID: 23973
	public LayerMask m_LayerMask = -1;

	// Token: 0x04005DA6 RID: 23974
	public Color m_Color = Color.gray;

	// Token: 0x04005DA7 RID: 23975
	public float m_BlurAmount = 0.6f;

	// Token: 0x04005DA8 RID: 23976
	private GameObject m_CameraGO;

	// Token: 0x04005DA9 RID: 23977
	private Camera m_Camera;

	// Token: 0x04005DAA RID: 23978
	private RenderTexture m_RenderTexture;

	// Token: 0x04005DAB RID: 23979
	private Mesh m_PlaneMesh;

	// Token: 0x04005DAC RID: 23980
	private GameObject m_PlaneGameObject;

	// Token: 0x04005DAD RID: 23981
	private float m_PreviousWidth;

	// Token: 0x04005DAE RID: 23982
	private float m_PreviousHeight;

	// Token: 0x04005DAF RID: 23983
	private Shader m_BloomShader;

	// Token: 0x04005DB0 RID: 23984
	private Material m_BloomMaterial;

	// Token: 0x04005DB1 RID: 23985
	private Shader m_BlurShader;

	// Token: 0x04005DB2 RID: 23986
	private Material m_BlurMaterial;

	// Token: 0x04005DB3 RID: 23987
	private Shader m_AdditiveShader;

	// Token: 0x04005DB4 RID: 23988
	private Material m_AdditiveMaterial;

	// Token: 0x04005DB5 RID: 23989
	private readonly Vector2[] PLANE_UVS = new Vector2[]
	{
		new Vector2(0f, 0f),
		new Vector2(1f, 0f),
		new Vector2(0f, 1f),
		new Vector2(1f, 1f)
	};

	// Token: 0x04005DB6 RID: 23990
	private readonly Vector3[] PLANE_NORMALS = new Vector3[]
	{
		Vector3.up,
		Vector3.up,
		Vector3.up,
		Vector3.up
	};

	// Token: 0x04005DB7 RID: 23991
	private readonly int[] PLANE_TRIANGLES = new int[]
	{
		3,
		1,
		2,
		2,
		1,
		0
	};
}
