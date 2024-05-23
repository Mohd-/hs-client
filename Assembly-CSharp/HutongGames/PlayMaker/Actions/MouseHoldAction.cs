using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC6 RID: 3526
	[Tooltip("Fires events based on how long you hold the left mouse button over a GameObject.")]
	[ActionCategory("Pegasus")]
	public class MouseHoldAction : FsmStateAction
	{
		// Token: 0x06006D33 RID: 27955 RVA: 0x002019F8 File Offset: 0x001FFBF8
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_HoldTime = 0f;
			this.m_HoldEvent = null;
			this.m_CancelEvent = null;
			this.m_holdingSec = 0f;
			this.m_suppressHoldEvent = false;
		}

		// Token: 0x06006D34 RID: 27956 RVA: 0x00201A31 File Offset: 0x001FFC31
		public override void OnEnter()
		{
			this.m_holdingSec = 0f;
			this.m_suppressHoldEvent = false;
			this.CheckHold(false);
		}

		// Token: 0x06006D35 RID: 27957 RVA: 0x00201A4C File Offset: 0x001FFC4C
		public override void OnUpdate()
		{
			this.CheckHold(true);
		}

		// Token: 0x06006D36 RID: 27958 RVA: 0x00201A58 File Offset: 0x001FFC58
		public override string ErrorCheck()
		{
			if (this.m_UseHoldTime.Value && FsmEvent.IsNullOrEmpty(this.m_HoldEvent))
			{
				return "Use Hold Time is checked but there is no Hold Event";
			}
			if (this.m_HoldTime.Value < 0f)
			{
				return "Hold Time cannot be less than 0";
			}
			return string.Empty;
		}

		// Token: 0x06006D37 RID: 27959 RVA: 0x00201AAC File Offset: 0x001FFCAC
		private void CheckHold(bool updating)
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (!InputUtil.IsPlayMakerMouseInputAllowed(ownerDefaultTarget))
			{
				return;
			}
			bool flag = UniversalInputManager.Get().InputIsOver(ownerDefaultTarget);
			bool mouseButton = UniversalInputManager.Get().GetMouseButton(0);
			if (updating)
			{
				if (flag && mouseButton)
				{
					this.HandleHold();
				}
				else
				{
					this.HandleCancel();
				}
			}
			if (flag && mouseButton)
			{
				this.m_holdingSec += Time.deltaTime;
			}
		}

		// Token: 0x06006D38 RID: 27960 RVA: 0x00201B30 File Offset: 0x001FFD30
		private void HandleHold()
		{
			if (this.m_suppressHoldEvent)
			{
				return;
			}
			if (this.m_UseHoldTime.Value && this.m_holdingSec >= this.m_HoldTime.Value)
			{
				this.m_suppressHoldEvent = true;
				base.Fsm.Event(this.m_HoldEvent);
			}
		}

		// Token: 0x06006D39 RID: 27961 RVA: 0x00201B88 File Offset: 0x001FFD88
		private void HandleCancel()
		{
			float holdingSec = this.m_holdingSec;
			this.m_holdingSec = 0f;
			if (this.m_suppressHoldEvent)
			{
				this.m_suppressHoldEvent = false;
				return;
			}
			if (this.m_UseHoldTime.Value && holdingSec >= this.m_HoldTime.Value)
			{
				base.Fsm.Event(this.m_HoldEvent);
			}
			else
			{
				base.Fsm.Event(this.m_CancelEvent);
			}
		}

		// Token: 0x040055DA RID: 21978
		[RequiredField]
		[CheckForComponent(typeof(Collider))]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x040055DB RID: 21979
		[Tooltip("Whether or not to fire the Hold Event after the Hold Time.")]
		public FsmBool m_UseHoldTime;

		// Token: 0x040055DC RID: 21980
		[Tooltip("How many seconds to wait before firing the Hold Event.")]
		public FsmFloat m_HoldTime;

		// Token: 0x040055DD RID: 21981
		[Tooltip("Fired after the Hold Time passes if Use Hold Time is enabled.")]
		public FsmEvent m_HoldEvent;

		// Token: 0x040055DE RID: 21982
		[Tooltip("Fired if the player mouses off OR releases the left mouse button before the Hold Time.")]
		public FsmEvent m_CancelEvent;

		// Token: 0x040055DF RID: 21983
		private float m_holdingSec;

		// Token: 0x040055E0 RID: 21984
		private bool m_suppressHoldEvent;
	}
}
