using System;
using UnityEngine;

// Token: 0x02000EC5 RID: 3781
public class LocalizedTexture : MonoBehaviour
{
	// Token: 0x06007199 RID: 29081 RVA: 0x00216822 File Offset: 0x00214A22
	private void Awake()
	{
		AssetLoader.Get().LoadTexture(this.m_textureName, new AssetLoader.ObjectCallback(this.OnTextureLoaded), null, false);
	}

	// Token: 0x0600719A RID: 29082 RVA: 0x00216844 File Offset: 0x00214A44
	private void OnTextureLoaded(string name, Object obj, object callbackData)
	{
		Texture texture = obj as Texture;
		if (texture == null)
		{
			Debug.LogError("Failed to load LocalizedTexture m_textureName!");
			return;
		}
		base.GetComponent<Renderer>().material.mainTexture = texture;
	}

	// Token: 0x04005BC7 RID: 23495
	public string m_textureName;
}
