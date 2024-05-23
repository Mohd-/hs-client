using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D9E RID: 3486
	[Tooltip("Loads and Plays a Sound Prefab.")]
	[ActionCategory("Pegasus Audio")]
	public class AudioLoadAndPlayAction : FsmStateAction
	{
		// Token: 0x06006C97 RID: 27799 RVA: 0x001FF1C0 File Offset: 0x001FD3C0
		public override void Reset()
		{
			this.m_ParentObject = null;
			this.m_PrefabName = null;
			this.m_VolumeScale = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06006C98 RID: 27800 RVA: 0x001FF1F0 File Offset: 0x001FD3F0
		public override void OnEnter()
		{
			if (this.m_PrefabName == null)
			{
				base.Finish();
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_ParentObject);
			string value = this.m_PrefabName.Value;
			if (this.m_VolumeScale.IsNone)
			{
				SoundManager.Get().LoadAndPlay(value, ownerDefaultTarget);
			}
			else
			{
				SoundManager.Get().LoadAndPlay(value, ownerDefaultTarget, this.m_VolumeScale.Value);
			}
			base.Finish();
		}

		// Token: 0x04005533 RID: 21811
		[Tooltip("Optional. If specified, the generated Audio Source will be attached to this object.")]
		public FsmOwnerDefault m_ParentObject;

		// Token: 0x04005534 RID: 21812
		[RequiredField]
		public FsmString m_PrefabName;

		// Token: 0x04005535 RID: 21813
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Optional. Scales the volume of the loaded sound.")]
		public FsmFloat m_VolumeScale;
	}
}
