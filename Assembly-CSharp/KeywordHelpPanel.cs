using System;
using UnityEngine;

// Token: 0x0200039A RID: 922
public class KeywordHelpPanel : MonoBehaviour
{
	// Token: 0x06003066 RID: 12390 RVA: 0x000F3721 File Offset: 0x000F1921
	private void Awake()
	{
		SceneUtils.SetLayer(base.gameObject, GameLayer.Tooltip);
	}

	// Token: 0x06003067 RID: 12391 RVA: 0x000F3730 File Offset: 0x000F1930
	private void OnDestroy()
	{
		Object.Destroy(this.m_name);
		this.m_name = null;
		Object.Destroy(this.m_body);
		this.m_body = null;
		Object.Destroy(this.m_background);
		this.m_background = null;
	}

	// Token: 0x06003068 RID: 12392 RVA: 0x000F3774 File Offset: 0x000F1974
	public void Reset()
	{
		base.transform.localScale = Vector3.one;
		base.transform.eulerAngles = Vector3.zero;
	}

	// Token: 0x06003069 RID: 12393 RVA: 0x000F37A1 File Offset: 0x000F19A1
	public void SetScale(float newScale)
	{
		this.scaleToUse = newScale;
		base.transform.localScale = new Vector3(this.scaleToUse, this.scaleToUse, this.scaleToUse);
	}

	// Token: 0x0600306A RID: 12394 RVA: 0x000F37CC File Offset: 0x000F19CC
	public void Initialize(string keywordName, string keywordText)
	{
		this.SetName(keywordName);
		this.SetBodyText(keywordText);
		base.gameObject.SetActive(true);
		this.m_name.UpdateNow();
		this.m_body.UpdateNow();
		float height = this.m_name.Height;
		float num = this.m_body.GetTextBounds().size.y;
		if (keywordText == string.Empty)
		{
			num = 0f;
		}
		float num2 = 1f;
		if (this.m_initialBackgroundHeight == 0f || this.m_initialBackgroundScale == Vector3.zero)
		{
			this.m_initialBackgroundHeight = this.m_background.m_middle.GetComponent<Renderer>().bounds.size.z;
			this.m_initialBackgroundScale = this.m_background.m_middle.transform.localScale;
		}
		float num3 = (height + num) * num2;
		this.m_background.SetSize(new Vector3(this.m_initialBackgroundScale.x, this.m_initialBackgroundScale.y * num3 / this.m_initialBackgroundHeight, this.m_initialBackgroundScale.z));
	}

	// Token: 0x0600306B RID: 12395 RVA: 0x000F38FC File Offset: 0x000F1AFC
	public void SetName(string s)
	{
		this.m_name.Text = s;
	}

	// Token: 0x0600306C RID: 12396 RVA: 0x000F390A File Offset: 0x000F1B0A
	public void SetBodyText(string s)
	{
		this.m_body.Text = s;
	}

	// Token: 0x0600306D RID: 12397 RVA: 0x000F3918 File Offset: 0x000F1B18
	public float GetHeight()
	{
		return this.m_background.m_leftOrTop.GetComponent<Renderer>().bounds.size.z + this.m_background.m_middle.GetComponent<Renderer>().bounds.size.z + this.m_background.m_rightOrBottom.GetComponent<Renderer>().bounds.size.z;
	}

	// Token: 0x0600306E RID: 12398 RVA: 0x000F3998 File Offset: 0x000F1B98
	public float GetWidth()
	{
		return this.m_background.m_leftOrTop.GetComponent<Renderer>().bounds.size.x;
	}

	// Token: 0x0600306F RID: 12399 RVA: 0x000F39CA File Offset: 0x000F1BCA
	public bool IsTextRendered()
	{
		return this.m_name.IsDone() && this.m_body.IsDone();
	}

	// Token: 0x04001E25 RID: 7717
	public const float PACK_OPENING_SCALE = 2.75f;

	// Token: 0x04001E26 RID: 7718
	public const float UNOPENED_PACK_SCALE = 5f;

	// Token: 0x04001E27 RID: 7719
	public const float DECK_HELPER_SCALE = 3.75f;

	// Token: 0x04001E28 RID: 7720
	public const float GAMEPLAY_HERO_POWER_SCALE = 0.6f;

	// Token: 0x04001E29 RID: 7721
	public UberText m_name;

	// Token: 0x04001E2A RID: 7722
	public UberText m_body;

	// Token: 0x04001E2B RID: 7723
	public NewThreeSliceElement m_background;

	// Token: 0x04001E2C RID: 7724
	private float m_initialBackgroundHeight;

	// Token: 0x04001E2D RID: 7725
	private Vector3 m_initialBackgroundScale = Vector3.zero;

	// Token: 0x04001E2E RID: 7726
	public static readonly PlatformDependentValue<float> HAND_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 0.65f,
		Phone = 0.8f
	};

	// Token: 0x04001E2F RID: 7727
	public static readonly PlatformDependentValue<float> GAMEPLAY_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 0.75f,
		Phone = 1.4f
	};

	// Token: 0x04001E30 RID: 7728
	public static readonly PlatformDependentValue<float> GAMEPLAY_SCALE_LARGE = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 0.9f,
		Phone = 1.25f
	};

	// Token: 0x04001E31 RID: 7729
	public static PlatformDependentValue<float> BOX_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 8f,
		Phone = 4.5f
	};

	// Token: 0x04001E32 RID: 7730
	public static PlatformDependentValue<float> HISTORY_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 0.48f,
		Phone = 0.853f
	};

	// Token: 0x04001E33 RID: 7731
	public static PlatformDependentValue<float> MULLIGAN_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 0.65f,
		Phone = 0.4f
	};

	// Token: 0x04001E34 RID: 7732
	public static PlatformDependentValue<float> COLLECTION_MANAGER_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 4f,
		Phone = 8f
	};

	// Token: 0x04001E35 RID: 7733
	public static PlatformDependentValue<float> FORGE_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 4f,
		Phone = 8f
	};

	// Token: 0x04001E36 RID: 7734
	private float scaleToUse = KeywordHelpPanel.GAMEPLAY_SCALE;
}
