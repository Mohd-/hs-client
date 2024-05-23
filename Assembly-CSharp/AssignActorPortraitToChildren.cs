using System;
using UnityEngine;

// Token: 0x02000EF2 RID: 3826
public class AssignActorPortraitToChildren : MonoBehaviour
{
	// Token: 0x06007272 RID: 29298 RVA: 0x00219DD3 File Offset: 0x00217FD3
	private void Start()
	{
		this.m_Actor = SceneUtils.FindComponentInThisOrParents<Actor>(base.gameObject);
	}

	// Token: 0x06007273 RID: 29299 RVA: 0x00219DE8 File Offset: 0x00217FE8
	public void AssignPortraitToAllChildren()
	{
		if (!this.m_Actor)
		{
			return;
		}
		if (this.m_Actor.m_portraitMesh == null)
		{
			return;
		}
		Material[] materials = this.m_Actor.m_portraitMesh.GetComponent<Renderer>().materials;
		if (materials.Length == 0 || this.m_Actor.m_portraitMatIdx < 0)
		{
			return;
		}
		Texture mainTexture = materials[this.m_Actor.m_portraitMatIdx].mainTexture;
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			Material[] materials2 = renderer.materials;
			foreach (Material material in materials2)
			{
				if (material.name.Contains("portrait"))
				{
					material.mainTexture = mainTexture;
				}
			}
		}
	}

	// Token: 0x04005C79 RID: 23673
	private Actor m_Actor;
}
