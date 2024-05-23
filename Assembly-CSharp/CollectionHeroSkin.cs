using System;
using UnityEngine;

// Token: 0x0200076C RID: 1900
public class CollectionHeroSkin : MonoBehaviour
{
	// Token: 0x06004C16 RID: 19478 RVA: 0x0016AD2C File Offset: 0x00168F2C
	public void Awake()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			Actor component = base.gameObject.GetComponent<Actor>();
			if (component != null)
			{
				component.OverrideNameText(null);
			}
			this.m_nameShadow.SetActive(false);
		}
	}

	// Token: 0x06004C17 RID: 19479 RVA: 0x0016AD74 File Offset: 0x00168F74
	public void SetClass(TAG_CLASS classTag)
	{
		if (this.m_classIcon != null)
		{
			Vector2 vector = CollectionPageManager.s_classTextureOffsets[classTag];
			this.m_classIcon.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", vector);
		}
		if (this.m_favoriteBannerText != null)
		{
			this.m_favoriteBannerText.Text = GameStrings.Format("GLUE_COLLECTION_MANAGER_FAVORITE_DEFAULT_TEXT", new object[]
			{
				GameStrings.GetClassName(classTag)
			});
		}
	}

	// Token: 0x06004C18 RID: 19480 RVA: 0x0016ADEE File Offset: 0x00168FEE
	public void ShowShadow(bool show)
	{
		if (this.m_shadow == null)
		{
			return;
		}
		this.m_shadow.SetActive(show);
	}

	// Token: 0x06004C19 RID: 19481 RVA: 0x0016AE0E File Offset: 0x0016900E
	public void ShowFavoriteBanner(bool show)
	{
		if (this.m_favoriteBanner == null)
		{
			return;
		}
		this.m_favoriteBanner.SetActive(show);
	}

	// Token: 0x06004C1A RID: 19482 RVA: 0x0016AE30 File Offset: 0x00169030
	public void ShowSocketFX()
	{
		if (this.m_socketFX == null || !this.m_socketFX.gameObject.activeInHierarchy)
		{
			return;
		}
		this.m_socketFX.gameObject.SetActive(true);
		this.m_socketFX.Activate();
	}

	// Token: 0x06004C1B RID: 19483 RVA: 0x0016AE80 File Offset: 0x00169080
	public void ShowCollectionManagerText()
	{
		Actor component = base.gameObject.GetComponent<Actor>();
		if (component != null)
		{
			component.OverrideNameText(this.m_collectionManagerName);
		}
	}

	// Token: 0x040032F8 RID: 13048
	public MeshRenderer m_classIcon;

	// Token: 0x040032F9 RID: 13049
	public GameObject m_favoriteBanner;

	// Token: 0x040032FA RID: 13050
	public UberText m_favoriteBannerText;

	// Token: 0x040032FB RID: 13051
	public GameObject m_shadow;

	// Token: 0x040032FC RID: 13052
	public Spell m_socketFX;

	// Token: 0x040032FD RID: 13053
	public UberText m_name;

	// Token: 0x040032FE RID: 13054
	public GameObject m_nameShadow;

	// Token: 0x040032FF RID: 13055
	public UberText m_collectionManagerName;
}
