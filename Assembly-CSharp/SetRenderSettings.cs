using System;
using UnityEngine;

// Token: 0x02000DD6 RID: 3542
public class SetRenderSettings : MonoBehaviour
{
	// Token: 0x06006D71 RID: 28017 RVA: 0x00202B50 File Offset: 0x00200D50
	private void enableAmbientUpdates()
	{
		this.m_ambient_shouldUpdate = true;
		if (LoadingScreen.Get() != null && LoadingScreen.Get().IsPreviousSceneActive())
		{
			this.m_lastSavedAmbient = this.m_ambient;
			LoadingScreen.Get().RegisterPreviousSceneDestroyedListener(new LoadingScreen.PreviousSceneDestroyedCallback(this.OnPreviousSceneDestroyed));
		}
		else
		{
			RenderSettings.ambientLight = this.m_ambient;
		}
	}

	// Token: 0x06006D72 RID: 28018 RVA: 0x00202BB6 File Offset: 0x00200DB6
	private void disableAmbientUpdates()
	{
		this.m_ambient_shouldUpdate = false;
	}

	// Token: 0x06006D73 RID: 28019 RVA: 0x00202BC0 File Offset: 0x00200DC0
	private void Update()
	{
		if (this.m_ambient_shouldUpdate)
		{
			this.m_lastSavedAmbient = this.m_ambient;
			if (LoadingScreen.Get() == null || !LoadingScreen.Get().IsPreviousSceneActive())
			{
				RenderSettings.ambientLight = this.m_ambient;
			}
		}
	}

	// Token: 0x06006D74 RID: 28020 RVA: 0x00202C0E File Offset: 0x00200E0E
	public void SetColor(Color newColor)
	{
		this.m_ambient = newColor;
		this.m_lastSavedAmbient = newColor;
	}

	// Token: 0x06006D75 RID: 28021 RVA: 0x00202C20 File Offset: 0x00200E20
	private void OnPreviousSceneDestroyed(object userData)
	{
		LoadingScreen.Get().UnregisterPreviousSceneDestroyedListener(new LoadingScreen.PreviousSceneDestroyedCallback(this.OnPreviousSceneDestroyed));
		RenderSettings.ambientLight = this.m_lastSavedAmbient;
	}

	// Token: 0x0400561D RID: 22045
	public Color m_ambient;

	// Token: 0x0400561E RID: 22046
	private bool m_ambient_shouldUpdate;

	// Token: 0x0400561F RID: 22047
	private Color m_lastSavedAmbient;
}
