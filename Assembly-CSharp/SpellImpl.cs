using System;
using UnityEngine;

// Token: 0x02000E55 RID: 3669
public class SpellImpl : Spell
{
	// Token: 0x06006F5F RID: 28511 RVA: 0x0020B134 File Offset: 0x00209334
	protected void InitActorVariables()
	{
		this.m_actor = SpellUtils.GetParentActor(this);
		this.m_rootObject = SpellUtils.GetParentRootObject(this);
		this.m_rootObjectRenderer = SpellUtils.GetParentRootObjectMesh(this);
	}

	// Token: 0x06006F60 RID: 28512 RVA: 0x0020B168 File Offset: 0x00209368
	protected void SetActorVisibility(bool visible, bool ignoreSpells)
	{
		if (this.m_actor != null)
		{
			if (visible)
			{
				this.m_actor.Show(ignoreSpells);
			}
			else
			{
				this.m_actor.Hide(ignoreSpells);
			}
		}
	}

	// Token: 0x06006F61 RID: 28513 RVA: 0x0020B1A9 File Offset: 0x002093A9
	protected void SetVisibility(GameObject go, bool visible)
	{
		go.GetComponent<Renderer>().enabled = visible;
	}

	// Token: 0x06006F62 RID: 28514 RVA: 0x0020B1B8 File Offset: 0x002093B8
	protected void SetVisibilityRecursive(GameObject go, bool visible)
	{
		if (go == null)
		{
			return;
		}
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		if (componentsInChildren != null)
		{
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.enabled = visible;
			}
		}
	}

	// Token: 0x06006F63 RID: 28515 RVA: 0x0020B200 File Offset: 0x00209400
	protected void SetAnimationSpeed(GameObject go, string animName, float speed)
	{
		if (go == null)
		{
			return;
		}
		go.GetComponent<Animation>()[animName].speed = speed;
	}

	// Token: 0x06006F64 RID: 28516 RVA: 0x0020B221 File Offset: 0x00209421
	protected void SetAnimationTime(GameObject go, string animName, float time)
	{
		if (go == null)
		{
			return;
		}
		go.GetComponent<Animation>()[animName].time = time;
	}

	// Token: 0x06006F65 RID: 28517 RVA: 0x0020B244 File Offset: 0x00209444
	protected void PlayAnimation(GameObject go, string animName, PlayMode playMode, float crossFade = 0f)
	{
		if (go == null)
		{
			return;
		}
		if (crossFade <= Mathf.Epsilon)
		{
			go.GetComponent<Animation>().Play(animName, playMode);
		}
		else
		{
			go.GetComponent<Animation>().CrossFade(animName, crossFade, playMode);
		}
	}

	// Token: 0x06006F66 RID: 28518 RVA: 0x0020B28C File Offset: 0x0020948C
	protected void PlayParticles(GameObject go, bool includeChildren)
	{
		if (go == null)
		{
			return;
		}
		ParticleSystem component = go.GetComponent<ParticleSystem>();
		component.Play(includeChildren);
	}

	// Token: 0x06006F67 RID: 28519 RVA: 0x0020B2B4 File Offset: 0x002094B4
	protected GameObject GetActorObject(string name)
	{
		if (this.m_actor == null)
		{
			return null;
		}
		return SceneUtils.FindChildBySubstring(this.m_actor.gameObject, name);
	}

	// Token: 0x06006F68 RID: 28520 RVA: 0x0020B2E8 File Offset: 0x002094E8
	protected void SetMaterialColor(GameObject go, Material material, string colorName, Color color, int materialIndex = 0)
	{
		if (colorName == string.Empty)
		{
			colorName = "_Color";
		}
		if (material != null)
		{
			material.SetColor(colorName, color);
			return;
		}
		if (go == null)
		{
			return;
		}
		if (go.GetComponent<Renderer>() == null)
		{
			return;
		}
		if (go.GetComponent<Renderer>().material == null)
		{
			return;
		}
		if (materialIndex == 0)
		{
			go.GetComponent<Renderer>().material.SetColor(colorName, color);
		}
		else if (go.GetComponent<Renderer>().materials.Length > materialIndex)
		{
			Material[] materials = go.GetComponent<Renderer>().materials;
			materials[materialIndex].SetColor(colorName, color);
			go.GetComponent<Renderer>().materials = materials;
		}
	}

	// Token: 0x06006F69 RID: 28521 RVA: 0x0020B3B0 File Offset: 0x002095B0
	protected Material GetMaterial(GameObject go, Material material, bool getSharedMaterial = false, int materialIndex = 0)
	{
		if (go == null || go.GetComponent<Renderer>() == null)
		{
			return null;
		}
		if (materialIndex == 0 && !getSharedMaterial)
		{
			return go.GetComponent<Renderer>().material;
		}
		if (materialIndex == 0 && getSharedMaterial)
		{
			return go.GetComponent<Renderer>().sharedMaterial;
		}
		if (go.GetComponent<Renderer>().materials.Length > materialIndex && !getSharedMaterial)
		{
			Material[] materials = go.GetComponent<Renderer>().materials;
			Material result = materials[materialIndex];
			go.GetComponent<Renderer>().materials = materials;
			return result;
		}
		if (go.GetComponent<Renderer>().materials.Length > materialIndex && getSharedMaterial)
		{
			Material[] sharedMaterials = go.GetComponent<Renderer>().sharedMaterials;
			Material result2 = sharedMaterials[materialIndex];
			go.GetComponent<Renderer>().sharedMaterials = sharedMaterials;
			return result2;
		}
		return null;
	}

	// Token: 0x0400588C RID: 22668
	protected Actor m_actor;

	// Token: 0x0400588D RID: 22669
	protected GameObject m_rootObject;

	// Token: 0x0400588E RID: 22670
	protected MeshRenderer m_rootObjectRenderer;
}
