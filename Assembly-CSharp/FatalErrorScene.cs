using System;
using UnityEngine;

// Token: 0x02000893 RID: 2195
public class FatalErrorScene : Scene
{
	// Token: 0x060053AA RID: 21418 RVA: 0x0018F3FC File Offset: 0x0018D5FC
	protected override void Awake()
	{
		AssetLoader.Get().LoadGameObject("FatalErrorScreen", new AssetLoader.GameObjectCallback(this.OnFatalErrorScreenLoaded), null, false);
		base.Awake();
		Navigation.Clear();
		Network.AppAbort();
		UserAttentionManager.StartBlocking(UserAttentionBlocker.FATAL_ERROR_SCENE);
		if (DialogManager.Get() != null)
		{
			Log.Mike.Print("FatalErrorScene.Awake() - calling DialogManager.Get().ClearAllImmediately()", new object[0]);
			DialogManager.Get().ClearAllImmediately();
		}
		foreach (Camera camera in Camera.allCameras)
		{
			FullScreenEffects component = camera.GetComponent<FullScreenEffects>();
			if (!(component == null))
			{
				component.Disable();
			}
		}
	}

	// Token: 0x060053AB RID: 21419 RVA: 0x0018F4A8 File Offset: 0x0018D6A8
	private void Start()
	{
		SceneMgr.Get().NotifySceneLoaded();
	}

	// Token: 0x060053AC RID: 21420 RVA: 0x0018F4B4 File Offset: 0x0018D6B4
	public override void Unload()
	{
		UserAttentionManager.StopBlocking(UserAttentionBlocker.FATAL_ERROR_SCENE);
	}

	// Token: 0x060053AD RID: 21421 RVA: 0x0018F4BC File Offset: 0x0018D6BC
	private void OnFatalErrorScreenLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			base.gameObject.AddComponent<FatalErrorDialog>();
		}
	}
}
