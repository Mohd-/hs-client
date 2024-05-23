using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F2F RID: 3887
[ExecuteInEditMode]
[CustomEditClass]
public class ReplaceMaterials : MonoBehaviour
{
	// Token: 0x060073B5 RID: 29621 RVA: 0x002213B4 File Offset: 0x0021F5B4
	private void Start()
	{
		foreach (ReplaceMaterials.MaterialData materialData in this.m_Materials)
		{
			GameObject gameObject = this.FindGameObject(materialData.GameObjectName);
			if (gameObject == null && !materialData.ReplaceChildMaterials)
			{
				Log.Kyle.Print("ReplaceMaterials failed to locate object: {0}", new object[]
				{
					materialData.GameObjectName
				});
			}
			else if (materialData.ReplaceChildMaterials)
			{
				foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
				{
					if (!(renderer == null))
					{
						RenderUtils.SetMaterial(renderer, materialData.MaterialIndex, materialData.NewMaterial);
					}
				}
			}
			else
			{
				Renderer component = gameObject.GetComponent<Renderer>();
				if (gameObject == null)
				{
					Log.Kyle.Print("ReplaceMaterials failed to get Renderer: {0}", new object[]
					{
						materialData.GameObjectName
					});
				}
				else
				{
					RenderUtils.SetMaterial(component, materialData.MaterialIndex, materialData.NewMaterial);
				}
			}
		}
	}

	// Token: 0x060073B6 RID: 29622 RVA: 0x00221500 File Offset: 0x0021F700
	private GameObject FindGameObject(string gameObjName)
	{
		if (gameObjName.get_Chars(0) != '/')
		{
			return GameObject.Find(gameObjName);
		}
		string[] array = gameObjName.Split(new char[]
		{
			'/'
		});
		return GameObject.Find(array[array.Length - 1]);
	}

	// Token: 0x04005E49 RID: 24137
	public List<ReplaceMaterials.MaterialData> m_Materials;

	// Token: 0x02000F30 RID: 3888
	[Serializable]
	public class MaterialData
	{
		// Token: 0x04005E4A RID: 24138
		[CustomEditField(T = EditType.SCENE_OBJECT)]
		public string GameObjectName;

		// Token: 0x04005E4B RID: 24139
		public int MaterialIndex;

		// Token: 0x04005E4C RID: 24140
		public Material NewMaterial;

		// Token: 0x04005E4D RID: 24141
		public bool ReplaceChildMaterials;

		// Token: 0x04005E4E RID: 24142
		public GameObject DisplayGameObject;
	}
}
