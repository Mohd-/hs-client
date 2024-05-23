using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000390 RID: 912
public class HeroSkinHeroPower : MonoBehaviour
{
	// Token: 0x06002FB0 RID: 12208 RVA: 0x000EFE4B File Offset: 0x000EE04B
	private void Start()
	{
		if (SceneMgr.Get().IsInGame())
		{
			base.StartCoroutine(this.HeroSkinCustomHeroPowerTextures());
		}
	}

	// Token: 0x06002FB1 RID: 12209 RVA: 0x000EFE69 File Offset: 0x000EE069
	public void RestoreOriginalTextures()
	{
		this.SetFrontTexture(this.m_OriginalFrontTexture);
		this.SetBackTexture(this.m_OriginalBackTexture);
	}

	// Token: 0x06002FB2 RID: 12210 RVA: 0x000EFE83 File Offset: 0x000EE083
	public void SetFrontTextureFromPath(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		AssetLoader.Get().LoadTexture(path, new AssetLoader.ObjectCallback(this.OnFrontTextureLoaded), null, false);
	}

	// Token: 0x06002FB3 RID: 12211 RVA: 0x000EFEAB File Offset: 0x000EE0AB
	public void SetBackTextureFromPath(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		AssetLoader.Get().LoadTexture(path, new AssetLoader.ObjectCallback(this.OnBackTextureLoaded), null, false);
	}

	// Token: 0x06002FB4 RID: 12212 RVA: 0x000EFED4 File Offset: 0x000EE0D4
	public Texture GetFrontTexture()
	{
		Renderer component = base.GetComponent<Renderer>();
		return component.materials[0].mainTexture;
	}

	// Token: 0x06002FB5 RID: 12213 RVA: 0x000EFEF8 File Offset: 0x000EE0F8
	public Texture GetBackTexture()
	{
		Renderer component = base.GetComponent<Renderer>();
		return component.materials[2].mainTexture;
	}

	// Token: 0x06002FB6 RID: 12214 RVA: 0x000EFF1C File Offset: 0x000EE11C
	public void SetFrontTexture(Texture tex)
	{
		Renderer component = base.GetComponent<Renderer>();
		Material[] materials = component.materials;
		materials[0].mainTexture = tex;
		component.materials = materials;
	}

	// Token: 0x06002FB7 RID: 12215 RVA: 0x000EFF48 File Offset: 0x000EE148
	public void SetBackTexture(Texture tex)
	{
		Renderer component = base.GetComponent<Renderer>();
		Material[] materials = component.materials;
		materials[1].SetTexture("_SecondTex", tex);
		materials[2].mainTexture = tex;
		component.materials = materials;
	}

	// Token: 0x06002FB8 RID: 12216 RVA: 0x000EFF84 File Offset: 0x000EE184
	private IEnumerator HeroSkinCustomHeroPowerTextures()
	{
		Card card = this.m_Actor.GetCard();
		while (card == null)
		{
			card = this.m_Actor.GetCard();
			yield return 0;
		}
		Card heroCard = card.GetHeroCard();
		while (heroCard == null)
		{
			heroCard = card.GetHeroCard();
			yield return 0;
		}
		CardDef heroCardDef = heroCard.GetCardDef();
		if (heroCardDef == null)
		{
			Debug.LogWarning("HeroSkinHeroPower: heroCardDef is null!");
			yield break;
		}
		yield break;
	}

	// Token: 0x06002FB9 RID: 12217 RVA: 0x000EFFA0 File Offset: 0x000EE1A0
	private void OnFrontTextureLoaded(string name, Object obj, object callbackData)
	{
		Texture frontTexture = obj as Texture2D;
		this.SetFrontTexture(frontTexture);
	}

	// Token: 0x06002FBA RID: 12218 RVA: 0x000EFFBC File Offset: 0x000EE1BC
	private void OnBackTextureLoaded(string name, Object obj, object callbackData)
	{
		Texture backTexture = obj as Texture2D;
		this.SetBackTexture(backTexture);
	}

	// Token: 0x04001D93 RID: 7571
	public Actor m_Actor;

	// Token: 0x04001D94 RID: 7572
	public Texture m_OriginalFrontTexture;

	// Token: 0x04001D95 RID: 7573
	public Texture m_OriginalBackTexture;
}
