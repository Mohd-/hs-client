using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD7 RID: 3543
	[ActionCategory("Pegasus")]
	[Tooltip("Show or Hide an Actor without messing up the game.")]
	public class SetActorLightBlend : FsmStateAction
	{
		// Token: 0x06006D77 RID: 28023 RVA: 0x00202C57 File Offset: 0x00200E57
		public override void Reset()
		{
			this.m_ActorObject = null;
			this.m_BlendValue = 1f;
			this.m_EveryFrame = false;
			this.m_actor = null;
		}

		// Token: 0x06006D78 RID: 28024 RVA: 0x00202C80 File Offset: 0x00200E80
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.m_actor == null)
			{
				this.FindActor();
			}
			this.m_actor.SetLightBlend(this.m_BlendValue.Value);
			if (!this.m_EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D79 RID: 28025 RVA: 0x00202CD4 File Offset: 0x00200ED4
		public override void OnUpdate()
		{
			if (this.m_actor == null)
			{
				this.FindActor();
			}
			if (this.m_actor != null)
			{
				this.m_actor.SetLightBlend(this.m_BlendValue.Value);
			}
		}

		// Token: 0x06006D7A RID: 28026 RVA: 0x00202D20 File Offset: 0x00200F20
		private void FindActor()
		{
			if (!this.m_ActorObject.IsNone)
			{
				GameObject value = this.m_ActorObject.Value;
				if (value != null)
				{
					this.m_actor = value.GetComponentInChildren<Actor>();
					if (this.m_actor == null)
					{
						this.m_actor = SceneUtils.FindComponentInThisOrParents<Actor>(value);
					}
				}
			}
		}

		// Token: 0x04005620 RID: 22048
		public FsmGameObject m_ActorObject;

		// Token: 0x04005621 RID: 22049
		[Tooltip("Light Blend Value")]
		public FsmFloat m_BlendValue;

		// Token: 0x04005622 RID: 22050
		[Tooltip("Update Every Frame")]
		public bool m_EveryFrame;

		// Token: 0x04005623 RID: 22051
		protected float m_initialLightBlendValue;

		// Token: 0x04005624 RID: 22052
		protected Actor m_actor;
	}
}
