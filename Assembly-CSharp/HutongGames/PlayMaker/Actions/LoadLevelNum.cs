using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C52 RID: 3154
	[ActionCategory(25)]
	[Tooltip("Loads a Level by Index number. Before you can load a level, you have to add it to the list of levels defined in File->Build Settings...")]
	public class LoadLevelNum : FsmStateAction
	{
		// Token: 0x060066B6 RID: 26294 RVA: 0x001E52A0 File Offset: 0x001E34A0
		public override void Reset()
		{
			this.levelIndex = null;
			this.additive = false;
			this.loadedEvent = null;
			this.dontDestroyOnLoad = false;
		}

		// Token: 0x060066B7 RID: 26295 RVA: 0x001E52C4 File Offset: 0x001E34C4
		public override void OnEnter()
		{
			if (this.dontDestroyOnLoad.Value)
			{
				Transform root = base.Owner.transform.root;
				Object.DontDestroyOnLoad(root.gameObject);
			}
			if (this.additive)
			{
				Application.LoadLevelAdditive(this.levelIndex.Value);
			}
			else
			{
				Application.LoadLevel(this.levelIndex.Value);
			}
			base.Fsm.Event(this.loadedEvent);
			base.Finish();
		}

		// Token: 0x04004E6F RID: 20079
		[RequiredField]
		[Tooltip("The level index in File->Build Settings")]
		public FsmInt levelIndex;

		// Token: 0x04004E70 RID: 20080
		[Tooltip("Load the level additively, keeping the current scene.")]
		public bool additive;

		// Token: 0x04004E71 RID: 20081
		[Tooltip("Event to send after the level is loaded.")]
		public FsmEvent loadedEvent;

		// Token: 0x04004E72 RID: 20082
		[Tooltip("Keep this GameObject in the new level. NOTE: The GameObject and components is disabled then enabled on load; uncheck Reset On Disable to keep the active state.")]
		public FsmBool dontDestroyOnLoad;
	}
}
