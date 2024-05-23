using System;
using UnityEngine;

// Token: 0x02000688 RID: 1672
public class CameraFade : MonoBehaviour
{
	// Token: 0x060046E5 RID: 18149 RVA: 0x001544D0 File Offset: 0x001526D0
	private void Awake()
	{
		this.m_TempTexture = new Texture2D(1, 1);
		this.m_TempTexture.SetPixel(0, 0, Color.white);
		this.m_TempTexture.Apply();
		this.m_Camera = base.GetComponent<Camera>();
		if (this.m_Camera == null)
		{
			Debug.LogError("CameraFade faild to find camera component!");
			base.enabled = false;
		}
		this.m_CameraDepth = this.m_Camera.depth;
		this.SetupCamera();
	}

	// Token: 0x060046E6 RID: 18150 RVA: 0x0015454C File Offset: 0x0015274C
	private void Update()
	{
		if (this.m_Fade <= 0f)
		{
			if (base.GetComponent<Renderer>() != null && base.GetComponent<Renderer>().enabled)
			{
				base.GetComponent<Renderer>().enabled = false;
			}
			if (this.m_Camera.enabled)
			{
				this.m_Camera.enabled = false;
			}
			return;
		}
		if (base.GetComponent<Renderer>() == null)
		{
			this.CreateRenderPlane();
		}
		if (!base.GetComponent<Renderer>().enabled)
		{
			base.GetComponent<Renderer>().enabled = true;
		}
		if (!this.m_Camera.enabled)
		{
			this.m_Camera.enabled = true;
		}
		if (this.m_RenderOverAll)
		{
			if (this.m_Camera.depth < 100f)
			{
				this.m_Camera.depth = 100f;
			}
		}
		else if (this.m_Camera.depth != this.m_CameraDepth)
		{
			this.m_Camera.depth = this.m_CameraDepth;
		}
		Color color;
		color..ctor(this.m_Color.r, this.m_Color.g, this.m_Color.b, this.m_Fade);
		this.m_Material.color = color;
	}

	// Token: 0x060046E7 RID: 18151 RVA: 0x00154698 File Offset: 0x00152898
	private void CreateRenderPlane()
	{
		base.gameObject.AddComponent<MeshFilter>();
		base.gameObject.AddComponent<MeshRenderer>();
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[]
		{
			new Vector3(-10f, -10f, 0f),
			new Vector3(10f, -10f, 0f),
			new Vector3(-10f, 10f, 0f),
			new Vector3(10f, 10f, 0f)
		};
		mesh.colors = new Color[]
		{
			Color.white,
			Color.white,
			Color.white,
			Color.white
		};
		mesh.uv = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		mesh.normals = new Vector3[]
		{
			Vector3.up,
			Vector3.up,
			Vector3.up,
			Vector3.up
		};
		mesh.triangles = new int[]
		{
			3,
			1,
			2,
			2,
			1,
			0
		};
		base.GetComponent<Renderer>().GetComponent<MeshFilter>().mesh = mesh;
		this.m_Material = new Material(ShaderUtils.FindShader("Hidden/CameraFade"));
		base.GetComponent<Renderer>().sharedMaterial = this.m_Material;
	}

	// Token: 0x060046E8 RID: 18152 RVA: 0x001548B8 File Offset: 0x00152AB8
	private void SetupCamera()
	{
		this.m_Camera.farClipPlane = 1f;
		this.m_Camera.nearClipPlane = -1f;
		this.m_Camera.clearFlags = 4;
		this.m_Camera.orthographicSize = 0.5f;
	}

	// Token: 0x04002DF9 RID: 11769
	public Color m_Color = Color.black;

	// Token: 0x04002DFA RID: 11770
	public float m_Fade = 1f;

	// Token: 0x04002DFB RID: 11771
	public bool m_RenderOverAll;

	// Token: 0x04002DFC RID: 11772
	private Texture2D m_TempTexture;

	// Token: 0x04002DFD RID: 11773
	private GameObject m_PlaneGameObject;

	// Token: 0x04002DFE RID: 11774
	private Material m_Material;

	// Token: 0x04002DFF RID: 11775
	private Camera m_Camera;

	// Token: 0x04002E00 RID: 11776
	private float m_CameraDepth = 14f;
}
