using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C51 RID: 3153
	[Tooltip("Loads a Level by Name. NOTE: Before you can load a level, you have to add it to the list of levels defined in File->Build Settings...")]
	[ActionCategory(25)]
	public class LoadLevel : FsmStateAction
	{
		// Token: 0x060066B2 RID: 26290 RVA: 0x001E5104 File Offset: 0x001E3304
		public override void Reset()
		{
			this.levelName = string.Empty;
			this.additive = false;
			this.async = false;
			this.loadedEvent = null;
			this.dontDestroyOnLoad = false;
		}

		// Token: 0x060066B3 RID: 26291 RVA: 0x001E5138 File Offset: 0x001E3338
		public override void OnEnter()
		{
			if (this.dontDestroyOnLoad.Value)
			{
				Transform root = base.Owner.transform.root;
				Object.DontDestroyOnLoad(root.gameObject);
			}
			if (this.additive)
			{
				if (this.async)
				{
					this.asyncOperation = Application.LoadLevelAdditiveAsync(this.levelName.Value);
					Debug.Log("LoadLevelAdditiveAsyc: " + this.levelName.Value);
					return;
				}
				Application.LoadLevelAdditive(this.levelName.Value);
				Debug.Log("LoadLevelAdditive: " + this.levelName.Value);
			}
			else
			{
				if (this.async)
				{
					this.asyncOperation = Application.LoadLevelAsync(this.levelName.Value);
					Debug.Log("LoadLevelAsync: " + this.levelName.Value);
					return;
				}
				Application.LoadLevel(this.levelName.Value);
				Debug.Log("LoadLevel: " + this.levelName.Value);
			}
			this.Log("LOAD COMPLETE");
			base.Fsm.Event(this.loadedEvent);
			base.Finish();
		}

		// Token: 0x060066B4 RID: 26292 RVA: 0x001E526F File Offset: 0x001E346F
		public override void OnUpdate()
		{
			if (this.asyncOperation.isDone)
			{
				base.Fsm.Event(this.loadedEvent);
				base.Finish();
			}
		}

		// Token: 0x04004E69 RID: 20073
		[RequiredField]
		[Tooltip("The name of the level to load. NOTE: Must be in the list of levels defined in File->Build Settings... ")]
		public FsmString levelName;

		// Token: 0x04004E6A RID: 20074
		[Tooltip("Load the level additively, keeping the current scene.")]
		public bool additive;

		// Token: 0x04004E6B RID: 20075
		[Tooltip("Load the level asynchronously in the background.")]
		public bool async;

		// Token: 0x04004E6C RID: 20076
		[Tooltip("Event to send when the level has loaded. NOTE: This only makes sense if the FSM is still in the scene!")]
		public FsmEvent loadedEvent;

		// Token: 0x04004E6D RID: 20077
		[Tooltip("Keep this GameObject in the new level. NOTE: The GameObject and components is disabled then enabled on load; uncheck Reset On Disable to keep the active state.")]
		public FsmBool dontDestroyOnLoad;

		// Token: 0x04004E6E RID: 20078
		private AsyncOperation asyncOperation;
	}
}
