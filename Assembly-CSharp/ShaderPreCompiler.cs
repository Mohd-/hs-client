using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000EF4 RID: 3828
public class ShaderPreCompiler : MonoBehaviour
{
	// Token: 0x06007276 RID: 29302 RVA: 0x00219F54 File Offset: 0x00218154
	public ShaderPreCompiler()
	{
		int[] array = new int[3];
		array[0] = 2;
		array[1] = 1;
		this.MESH_TRIANGLES = array;
		base..ctor();
	}

	// Token: 0x06007278 RID: 29304 RVA: 0x0021A120 File Offset: 0x00218320
	private void Start()
	{
		if (GraphicsManager.Get().isVeryLowQualityDevice())
		{
			Debug.Log("ShaderPreCompiler: Disabled, very low quality mode");
			return;
		}
		if (GraphicsManager.Get().RenderQualityLevel != GraphicsQuality.Low)
		{
			base.StartCoroutine(this.WarmupShaders(this.m_StartupCompileShaders));
		}
		SceneMgr.Get().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.WarmupSceneChangeShader));
		this.AddShader(this.m_GoldenUberShader.name, this.m_GoldenUberShader);
		foreach (Shader shader in this.m_StartupCompileShaders)
		{
			if (!(shader == null))
			{
				this.AddShader(shader.name, shader);
			}
		}
		foreach (Shader shader2 in this.m_SceneChangeCompileShaders)
		{
			if (!(shader2 == null))
			{
				this.AddShader(shader2.name, shader2);
			}
		}
	}

	// Token: 0x06007279 RID: 29305 RVA: 0x0021A218 File Offset: 0x00218418
	public static Shader GetShader(string shaderName)
	{
		Shader shader;
		if (ShaderPreCompiler.s_shaderCache.TryGetValue(shaderName, out shader))
		{
			return shader;
		}
		shader = Shader.Find(shaderName);
		if (shader != null)
		{
			ShaderPreCompiler.s_shaderCache.Add(shaderName, shader);
		}
		return shader;
	}

	// Token: 0x0600727A RID: 29306 RVA: 0x0021A259 File Offset: 0x00218459
	private void AddShader(string shaderName, Shader shader)
	{
		if (ShaderPreCompiler.s_shaderCache.ContainsKey(shaderName))
		{
			return;
		}
		ShaderPreCompiler.s_shaderCache.Add(shaderName, shader);
	}

	// Token: 0x0600727B RID: 29307 RVA: 0x0021A278 File Offset: 0x00218478
	private void WarmupSceneChangeShader(SceneMgr.Mode prevMode, Scene prevScene, object userData)
	{
		if ((SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY || SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER || SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL) && Network.ShouldBeConnectedToAurora())
		{
			base.StartCoroutine(this.WarmupGoldenUberShader());
			this.PremiumShadersCompiled = true;
		}
		if (prevMode != SceneMgr.Mode.HUB)
		{
			return;
		}
		if (this.SceneChangeShadersCompiled)
		{
			return;
		}
		this.SceneChangeShadersCompiled = true;
		if (GraphicsManager.Get().RenderQualityLevel != GraphicsQuality.Low)
		{
			base.StartCoroutine(this.WarmupShaders(this.m_SceneChangeCompileShaders));
		}
		if (this.SceneChangeShadersCompiled && this.PremiumShadersCompiled)
		{
			SceneMgr.Get().UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.WarmupSceneChangeShader));
		}
	}

	// Token: 0x0600727C RID: 29308 RVA: 0x0021A340 File Offset: 0x00218540
	private IEnumerator WarmupGoldenUberShader()
	{
		float totalTime = 0f;
		foreach (string kw in this.GOLDEN_UBER_KEYWORDS1)
		{
			foreach (string kw2 in this.GOLDEN_UBER_KEYWORDS2)
			{
				ShaderVariantCollection svc = new ShaderVariantCollection();
				ShaderVariantCollection.ShaderVariant sv = default(ShaderVariantCollection.ShaderVariant);
				sv.shader = this.m_GoldenUberShader;
				sv.keywords = new string[]
				{
					kw,
					kw2
				};
				svc.Add(sv);
				float start = Time.realtimeSinceStartup;
				svc.WarmUp();
				float end = Time.realtimeSinceStartup;
				totalTime += end - start;
				Log.Graphics.Print(string.Format("Golden Uber Shader Compile: {0} Keywords: {1}, {2} ({3}s)", new object[]
				{
					this.m_GoldenUberShader.name,
					kw,
					kw2,
					end - start
				}), new object[0]);
				yield return null;
			}
		}
		Log.Graphics.Print("Profiling Shader Warmup: " + totalTime, new object[0]);
		yield break;
	}

	// Token: 0x0600727D RID: 29309 RVA: 0x0021A35C File Offset: 0x0021855C
	private IEnumerator WarmupShaders(Shader[] shaders)
	{
		float totalTime = 0f;
		foreach (Shader shader in shaders)
		{
			if (!(shader == null))
			{
				ShaderVariantCollection svc = new ShaderVariantCollection();
				ShaderVariantCollection.ShaderVariant sv = default(ShaderVariantCollection.ShaderVariant);
				sv.shader = shader;
				svc.Add(sv);
				float start = Time.realtimeSinceStartup;
				svc.WarmUp();
				float end = Time.realtimeSinceStartup;
				totalTime += end - start;
				Log.Graphics.Print(string.Format("Shader Compile: {0} ({1}s)", shader.name, end - start), new object[0]);
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x0600727E RID: 29310 RVA: 0x0021A380 File Offset: 0x00218580
	private GameObject CreateMesh(string name)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = name;
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		Mesh mesh = new Mesh();
		mesh.vertices = this.MESH_VERTS;
		mesh.uv = this.MESH_UVS;
		mesh.normals = this.MESH_NORMALS;
		mesh.tangents = this.MESH_TANGENTS;
		mesh.triangles = this.MESH_TRIANGLES;
		gameObject.GetComponent<MeshFilter>().mesh = mesh;
		return gameObject;
	}

	// Token: 0x0600727F RID: 29311 RVA: 0x0021A440 File Offset: 0x00218640
	private Material CreateMaterial(string name, Shader shader)
	{
		return new Material(shader)
		{
			name = name
		};
	}

	// Token: 0x04005C7B RID: 23675
	private readonly string[] GOLDEN_UBER_KEYWORDS1 = new string[]
	{
		"FX3_ADDBLEND",
		"FX3_ALPHABLEND"
	};

	// Token: 0x04005C7C RID: 23676
	private readonly string[] GOLDEN_UBER_KEYWORDS2 = new string[]
	{
		"LAYER3",
		"FX3_FLOWMAP",
		"LAYER4"
	};

	// Token: 0x04005C7D RID: 23677
	private readonly Vector3[] MESH_VERTS = new Vector3[]
	{
		Vector3.zero,
		Vector3.zero,
		Vector3.zero
	};

	// Token: 0x04005C7E RID: 23678
	private readonly Vector2[] MESH_UVS = new Vector2[]
	{
		new Vector2(0f, 0f),
		new Vector2(1f, 0f),
		new Vector2(0f, 1f)
	};

	// Token: 0x04005C7F RID: 23679
	private readonly Vector3[] MESH_NORMALS = new Vector3[]
	{
		Vector3.up,
		Vector3.up,
		Vector3.up
	};

	// Token: 0x04005C80 RID: 23680
	private readonly Vector4[] MESH_TANGENTS = new Vector4[]
	{
		new Vector4(1f, 0f, 0f, 0f),
		new Vector4(1f, 0f, 0f, 0f),
		new Vector4(1f, 0f, 0f, 0f)
	};

	// Token: 0x04005C81 RID: 23681
	private readonly int[] MESH_TRIANGLES;

	// Token: 0x04005C82 RID: 23682
	public Shader m_GoldenUberShader;

	// Token: 0x04005C83 RID: 23683
	public Shader[] m_StartupCompileShaders;

	// Token: 0x04005C84 RID: 23684
	public Shader[] m_SceneChangeCompileShaders;

	// Token: 0x04005C85 RID: 23685
	protected static Map<string, Shader> s_shaderCache = new Map<string, Shader>();

	// Token: 0x04005C86 RID: 23686
	private bool SceneChangeShadersCompiled;

	// Token: 0x04005C87 RID: 23687
	private bool PremiumShadersCompiled;
}
