using System;
using System.Collections;
using UnityEngine;

// Token: 0x020006E5 RID: 1765
public class CraftingModeButton : UIBButton
{
	// Token: 0x060048D9 RID: 18649 RVA: 0x0015C4C1 File Offset: 0x0015A6C1
	public void ShowActiveGlow(bool show)
	{
		this.m_isGlowEnabled = show;
		this.m_activeGlow.SetActive(show);
	}

	// Token: 0x060048DA RID: 18650 RVA: 0x0015C4D6 File Offset: 0x0015A6D6
	public void ShowDustBottle(bool show)
	{
		this.m_showDustBottle = show;
		this.m_dustBottle.SetActive(show);
		if (show)
		{
			this.StartBottleJiggle();
		}
	}

	// Token: 0x060048DB RID: 18651 RVA: 0x0015C4F7 File Offset: 0x0015A6F7
	private void StartBottleJiggle()
	{
		base.StopCoroutine("Jiggle");
		iTween.Stop(this.m_dustBottle.gameObject);
		this.BottleJiggle();
	}

	// Token: 0x060048DC RID: 18652 RVA: 0x0015C51A File Offset: 0x0015A71A
	private void BottleJiggle()
	{
		base.StartCoroutine(this.Jiggle());
	}

	// Token: 0x060048DD RID: 18653 RVA: 0x0015C52C File Offset: 0x0015A72C
	private IEnumerator Jiggle()
	{
		yield return new WaitForSeconds(1f);
		this.m_dustShower.Play();
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			this.m_jarJiggleRotation,
			"time",
			0.5f,
			"oncomplete",
			"BottleJiggle",
			"oncompletetarget",
			base.gameObject
		});
		iTween.PunchRotation(this.m_dustBottle.gameObject, args);
		yield break;
	}

	// Token: 0x060048DE RID: 18654 RVA: 0x0015C548 File Offset: 0x0015A748
	public void Enable(bool enabled)
	{
		this.SetEnabled(enabled);
		this.m_activeGlow.SetActive(enabled && this.m_isGlowEnabled);
		this.m_textObject.SetActive(enabled);
		this.m_dustShower.gameObject.SetActive(enabled);
		if (enabled)
		{
			this.m_dustBottle.SetActive(this.m_showDustBottle);
		}
		else
		{
			this.m_dustBottle.SetActive(false);
		}
		this.m_mainMesh.sharedMaterial = ((!enabled) ? this.m_disabledMaterial : this.m_enabledMaterial);
	}

	// Token: 0x04002FF6 RID: 12278
	public GameObject m_dustBottle;

	// Token: 0x04002FF7 RID: 12279
	public GameObject m_activeGlow;

	// Token: 0x04002FF8 RID: 12280
	public ParticleSystem m_dustShower;

	// Token: 0x04002FF9 RID: 12281
	public Vector3 m_jarJiggleRotation = new Vector3(0f, 30f, 0f);

	// Token: 0x04002FFA RID: 12282
	public GameObject m_textObject;

	// Token: 0x04002FFB RID: 12283
	public MeshRenderer m_mainMesh;

	// Token: 0x04002FFC RID: 12284
	public Material m_enabledMaterial;

	// Token: 0x04002FFD RID: 12285
	public Material m_disabledMaterial;

	// Token: 0x04002FFE RID: 12286
	private bool m_isGlowEnabled;

	// Token: 0x04002FFF RID: 12287
	private bool m_showDustBottle;

	// Token: 0x04003000 RID: 12288
	private bool m_isJiggling;
}
