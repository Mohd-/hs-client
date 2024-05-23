using System;
using bgs;
using UnityEngine;

// Token: 0x02000581 RID: 1409
public class PlayerPortrait : MonoBehaviour
{
	// Token: 0x0600400A RID: 16394 RVA: 0x00136921 File Offset: 0x00134B21
	public BnetProgramId GetProgramId()
	{
		return this.m_programId;
	}

	// Token: 0x0600400B RID: 16395 RVA: 0x00136929 File Offset: 0x00134B29
	public bool SetProgramId(BnetProgramId programId)
	{
		if (this.m_programId == programId)
		{
			return false;
		}
		this.m_programId = programId;
		this.UpdateIcon();
		return true;
	}

	// Token: 0x0600400C RID: 16396 RVA: 0x0013694C File Offset: 0x00134B4C
	public bool IsIconReady()
	{
		return this.m_loadingTextureName == null && this.m_currentTextureName != null;
	}

	// Token: 0x0600400D RID: 16397 RVA: 0x00136968 File Offset: 0x00134B68
	public bool IsIconLoading()
	{
		return this.m_loadingTextureName != null;
	}

	// Token: 0x0600400E RID: 16398 RVA: 0x00136978 File Offset: 0x00134B78
	private void UpdateIcon()
	{
		if (this.m_programId == null)
		{
			this.m_currentTextureName = null;
			this.m_loadingTextureName = null;
			base.GetComponent<Renderer>().material.mainTexture = null;
			return;
		}
		string textureName = BnetProgramId.GetTextureName(this.m_programId);
		if (this.m_currentTextureName == textureName)
		{
			return;
		}
		if (this.m_loadingTextureName == textureName)
		{
			return;
		}
		this.m_loadingTextureName = textureName;
		AssetLoader.Get().LoadTexture(this.m_loadingTextureName, new AssetLoader.ObjectCallback(this.OnTextureLoaded), null, false);
	}

	// Token: 0x0600400F RID: 16399 RVA: 0x00136A0C File Offset: 0x00134C0C
	private void OnTextureLoaded(string name, Object obj, object callbackData)
	{
		if (name != this.m_loadingTextureName)
		{
			return;
		}
		Texture texture = obj as Texture;
		if (texture == null)
		{
			Error.AddDevFatal("PlayerPortrait.OnTextureLoaded() - Failed to load {0}. ProgramId={1}", new object[]
			{
				name,
				this.m_programId
			});
			this.m_currentTextureName = null;
			this.m_loadingTextureName = null;
			return;
		}
		this.m_currentTextureName = this.m_loadingTextureName;
		this.m_loadingTextureName = null;
		base.GetComponent<Renderer>().material.mainTexture = texture;
	}

	// Token: 0x04002903 RID: 10499
	private BnetProgramId m_programId;

	// Token: 0x04002904 RID: 10500
	private string m_currentTextureName;

	// Token: 0x04002905 RID: 10501
	private string m_loadingTextureName;
}
