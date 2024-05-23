using System;
using bgs;
using UnityEngine;

// Token: 0x02000649 RID: 1609
public class FriendListGameIcon : MonoBehaviour
{
	// Token: 0x06004559 RID: 17753 RVA: 0x0014D178 File Offset: 0x0014B378
	public BnetProgramId GetProgramId()
	{
		return this.m_programId;
	}

	// Token: 0x0600455A RID: 17754 RVA: 0x0014D180 File Offset: 0x0014B380
	public void SetProgramId(BnetProgramId programId)
	{
		if (this.m_programId == programId)
		{
			return;
		}
		this.m_programId = programId;
		this.UpdateIcon();
	}

	// Token: 0x0600455B RID: 17755 RVA: 0x0014D1A1 File Offset: 0x0014B3A1
	public bool IsIconReady()
	{
		return this.m_loadingIcon == null && this.m_currentIcon != null;
	}

	// Token: 0x0600455C RID: 17756 RVA: 0x0014D1BD File Offset: 0x0014B3BD
	public bool IsIconLoading()
	{
		return this.m_loadingIcon != null;
	}

	// Token: 0x0600455D RID: 17757 RVA: 0x0014D1CC File Offset: 0x0014B3CC
	private void UpdateIcon()
	{
		string text = (!(this.m_programId == null)) ? BnetProgramId.GetTextureName(this.m_programId) : null;
		if (text == null)
		{
			this.m_currentIcon = null;
			this.m_loadingIcon = null;
			this.m_Icon.GetComponent<Renderer>().material.mainTexture = null;
			return;
		}
		if (this.m_currentIcon == text)
		{
			return;
		}
		if (this.m_loadingIcon == text)
		{
			return;
		}
		this.m_loadingIcon = text;
		AssetLoader.Get().LoadTexture(this.m_loadingIcon, new AssetLoader.ObjectCallback(this.OnIconLoaded), null, false);
	}

	// Token: 0x0600455E RID: 17758 RVA: 0x0014D274 File Offset: 0x0014B474
	private void OnIconLoaded(string name, Object obj, object callbackData)
	{
		if (name != this.m_loadingIcon)
		{
			return;
		}
		Texture texture = obj as Texture;
		if (texture == null)
		{
			Error.AddDevFatal("FriendListGameIcon.OnIconLoaded() - Failed to load {0}. ProgramId={1}", new object[]
			{
				name,
				this.m_programId
			});
			this.m_currentIcon = null;
			this.m_loadingIcon = null;
			return;
		}
		this.m_currentIcon = this.m_loadingIcon;
		this.m_loadingIcon = null;
		this.m_Icon.GetComponent<Renderer>().material.mainTexture = texture;
	}

	// Token: 0x04002C3F RID: 11327
	public GameObject m_Icon;

	// Token: 0x04002C40 RID: 11328
	private BnetProgramId m_programId;

	// Token: 0x04002C41 RID: 11329
	private string m_currentIcon;

	// Token: 0x04002C42 RID: 11330
	private string m_loadingIcon;
}
