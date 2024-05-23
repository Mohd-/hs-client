using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000377 RID: 887
[ExecuteInEditMode]
[CustomEditClass]
public class NestedPrefab : MonoBehaviour
{
	// Token: 0x06002D5B RID: 11611 RVA: 0x000E31A4 File Offset: 0x000E13A4
	public GameObject PrefabGameObject()
	{
		if (this.m_PrefabGameObject == null && this.m_InstantiatePrefabOnAccess)
		{
			this.UpdateMesh();
		}
		return this.m_PrefabGameObject;
	}

	// Token: 0x06002D5C RID: 11612 RVA: 0x000E31D9 File Offset: 0x000E13D9
	private void OnEnable()
	{
		if (this.m_PrefabGameObject == null)
		{
			this.UpdateMesh();
		}
	}

	// Token: 0x06002D5D RID: 11613 RVA: 0x000E31F4 File Offset: 0x000E13F4
	private void UpdateMesh()
	{
		this.LoadPrefab();
		this.m_EditorMeshes.Clear();
		if (base.enabled && this.m_PrefabGameObject != null)
		{
			this.SetupEditorMesh(this.m_PrefabGameObject, Matrix4x4.identity);
		}
	}

	// Token: 0x06002D5E RID: 11614 RVA: 0x000E3240 File Offset: 0x000E1440
	private void SetupEditorMesh(GameObject go, Matrix4x4 goMtx)
	{
		if (!go)
		{
			return;
		}
		Vector3 vector = go.transform.position * -1f;
		Matrix4x4 matrix4x = goMtx * Matrix4x4.TRS(vector, Quaternion.identity, Vector3.one);
		foreach (Renderer renderer in go.GetComponentsInChildren(typeof(Renderer), true))
		{
			MeshFilter component = renderer.GetComponent<MeshFilter>();
			if (!(component == null))
			{
				if (renderer.sharedMaterials != null && renderer.sharedMaterials.Length != 0)
				{
					this.m_EditorMeshes.Add(new NestedPrefab.EditorMesh
					{
						mesh = component.sharedMesh,
						matrix = matrix4x * renderer.transform.localToWorldMatrix,
						materials = new List<Material>(renderer.sharedMaterials)
					});
				}
			}
		}
		foreach (NestedPrefab nestedPrefab in go.GetComponentsInChildren(typeof(NestedPrefab), true))
		{
			if (nestedPrefab.enabled && nestedPrefab.gameObject.activeSelf)
			{
				this.SetupEditorMesh(nestedPrefab.m_PrefabGameObject, matrix4x * nestedPrefab.transform.localToWorldMatrix);
			}
		}
	}

	// Token: 0x06002D5F RID: 11615 RVA: 0x000E33B4 File Offset: 0x000E15B4
	private void LoadPrefab()
	{
		string name = FileUtils.GameAssetPathToName(this.m_Prefab);
		this.m_PrefabGameObject = AssetLoader.Get().LoadGameObject(name, true, false);
		Quaternion localRotation = this.m_PrefabGameObject.transform.localRotation;
		Vector3 localScale = this.m_PrefabGameObject.transform.localScale;
		this.m_PrefabGameObject.transform.parent = base.transform;
		this.m_PrefabGameObject.transform.localPosition = Vector3.zero;
		this.m_PrefabGameObject.transform.localRotation = localRotation;
		this.m_PrefabGameObject.transform.localScale = localScale;
	}

	// Token: 0x04001C2E RID: 7214
	private readonly Color SELECTED_WIRE_COLOR = new Color(0.3f, 0.3f, 0.5f, 0.5f);

	// Token: 0x04001C2F RID: 7215
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_Prefab;

	// Token: 0x04001C30 RID: 7216
	public bool m_InstantiatePrefabOnAccess;

	// Token: 0x04001C31 RID: 7217
	private List<NestedPrefab.EditorMesh> m_EditorMeshes = new List<NestedPrefab.EditorMesh>();

	// Token: 0x04001C32 RID: 7218
	private string m_lastPrefab;

	// Token: 0x04001C33 RID: 7219
	private GameObject m_PrefabGameObject;

	// Token: 0x0200063C RID: 1596
	private struct EditorMesh
	{
		// Token: 0x04002C0C RID: 11276
		public Mesh mesh;

		// Token: 0x04002C0D RID: 11277
		public Matrix4x4 matrix;

		// Token: 0x04002C0E RID: 11278
		public List<Material> materials;
	}
}
