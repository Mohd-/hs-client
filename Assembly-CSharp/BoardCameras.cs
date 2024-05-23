using System;
using UnityEngine;

// Token: 0x020003EE RID: 1006
public class BoardCameras : MonoBehaviour
{
	// Token: 0x06003413 RID: 13331 RVA: 0x0010436C File Offset: 0x0010256C
	private void Awake()
	{
		BoardCameras.s_instance = this;
		if (LoadingScreen.Get() != null)
		{
			LoadingScreen.Get().NotifyMainSceneObjectAwoke(base.gameObject);
		}
	}

	// Token: 0x06003414 RID: 13332 RVA: 0x00104394 File Offset: 0x00102594
	private void OnDestroy()
	{
		BoardCameras.s_instance = null;
	}

	// Token: 0x06003415 RID: 13333 RVA: 0x0010439C File Offset: 0x0010259C
	public static BoardCameras Get()
	{
		return BoardCameras.s_instance;
	}

	// Token: 0x06003416 RID: 13334 RVA: 0x001043A3 File Offset: 0x001025A3
	public AudioListener GetAudioListener()
	{
		return this.m_AudioListener;
	}

	// Token: 0x04002033 RID: 8243
	public AudioListener m_AudioListener;

	// Token: 0x04002034 RID: 8244
	private static BoardCameras s_instance;
}
