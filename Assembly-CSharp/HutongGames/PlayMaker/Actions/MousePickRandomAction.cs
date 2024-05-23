using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC7 RID: 3527
	[ActionCategory("Pegasus")]
	[Tooltip("Raycast from camera and send events. Mouse Down can have a rendom chance to send event")]
	public class MousePickRandomAction : FsmStateAction
	{
		// Token: 0x06006D3B RID: 27963 RVA: 0x00201C44 File Offset: 0x001FFE44
		public override void Reset()
		{
			this.GameObject = null;
			this.additionalColliders = new FsmGameObject[0];
			this.RandomGateClicksMin = 0;
			this.RandomGateClicksMax = 0;
			this.ResetOnOpen = false;
			this.mouseDownGateOpen = null;
			this.mouseDownGateClosed = null;
			this.mouseOver = null;
			this.mouseUp = null;
			this.mouseOff = null;
			this.checkFirstFrame = true;
			this.everyFrame = true;
			this.oneShot = false;
			this.ClickCount = 0;
		}

		// Token: 0x06006D3C RID: 27964 RVA: 0x00201CCC File Offset: 0x001FFECC
		public override void OnEnter()
		{
			if (this.RandomGateClicksMin.Value > this.RandomGateClicksMax.Value)
			{
				this.RandomGateClicksMin = this.RandomGateClicksMax;
			}
			if (this.m_opened)
			{
				this.m_RandomValue = Random.Range(this.RandomGateClicksMin.Value, this.RandomGateClicksMax.Value);
				this.m_opened = false;
			}
			if (this.checkFirstFrame)
			{
				this.DoMousePickEvent();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D3D RID: 27965 RVA: 0x00201D55 File Offset: 0x001FFF55
		public override void OnUpdate()
		{
			this.DoMousePickEvent();
		}

		// Token: 0x06006D3E RID: 27966 RVA: 0x00201D60 File Offset: 0x001FFF60
		private void DoMousePickEvent()
		{
			bool flag = this.mouseOver != null || this.mouseOff != null;
			bool flag2 = this.mouseUp != null;
			bool flag3 = this.mouseDownGateOpen != null || this.mouseDownGateClosed != null;
			if (!flag && (!flag2 || !Input.GetMouseButtonUp(0)) && (!flag3 || !Input.GetMouseButtonDown(0)))
			{
				return;
			}
			GameObject gameObject = (this.GameObject.OwnerOption != null) ? this.GameObject.GameObject.Value : base.Owner;
			if (!InputUtil.IsPlayMakerMouseInputAllowed(gameObject))
			{
				return;
			}
			bool flag4 = UniversalInputManager.Get().InputIsOver(gameObject.gameObject);
			if (!flag4 && this.additionalColliders.Length > 0)
			{
				for (int i = 0; i < this.additionalColliders.Length; i++)
				{
					GameObject value = this.additionalColliders[i].Value;
					if (!(value == null))
					{
						flag4 = UniversalInputManager.Get().InputIsOver(value);
						if (flag4)
						{
							break;
						}
					}
				}
			}
			if (flag4)
			{
				if (UniversalInputManager.Get().GetMouseButtonDown(0))
				{
					this.ClickCount.Value = this.ClickCount.Value + 1;
					if (this.ClickCount.Value >= this.m_RandomValue)
					{
						this.m_opened = true;
						if (this.ResetOnOpen.Value)
						{
							this.ClickCount.Value = 0;
						}
						if (this.mouseDownGateOpen != null)
						{
							base.Fsm.Event(this.mouseDownGateOpen);
						}
					}
					else if (this.mouseDownGateClosed != null)
					{
						base.Fsm.Event(this.mouseDownGateClosed);
					}
					if (this.oneShot)
					{
						base.Finish();
					}
				}
				if (this.mouseOver != null)
				{
					base.Fsm.Event(this.mouseOver);
				}
				if (this.mouseUp != null && UniversalInputManager.Get().GetMouseButtonUp(0))
				{
					base.Fsm.Event(this.mouseUp);
					if (this.oneShot)
					{
						base.Finish();
					}
				}
			}
			else if (this.mouseOff != null)
			{
				base.Fsm.Event(this.mouseOff);
			}
		}

		// Token: 0x06006D3F RID: 27967 RVA: 0x00201FC4 File Offset: 0x002001C4
		public override string ErrorCheck()
		{
			return string.Empty;
		}

		// Token: 0x040055E1 RID: 21985
		[CheckForComponent(typeof(Collider))]
		public FsmOwnerDefault GameObject;

		// Token: 0x040055E2 RID: 21986
		[Tooltip("Additional Colliders for mouse pick")]
		public FsmGameObject[] additionalColliders;

		// Token: 0x040055E3 RID: 21987
		[Tooltip("Min number of clicks before random gate open")]
		public FsmInt RandomGateClicksMin = 0;

		// Token: 0x040055E4 RID: 21988
		[Tooltip("Max number of clicks before random gate open")]
		public FsmInt RandomGateClicksMax = 0;

		// Token: 0x040055E5 RID: 21989
		[Tooltip("Resets count to 0 once triggered")]
		public FsmBool ResetOnOpen = false;

		// Token: 0x040055E6 RID: 21990
		[Tooltip("Mouse Down event. Random Gate open (true)")]
		public FsmEvent mouseDownGateOpen;

		// Token: 0x040055E7 RID: 21991
		[Tooltip("Mouse Down event. Random Gate is closed (false)")]
		public FsmEvent mouseDownGateClosed;

		// Token: 0x040055E8 RID: 21992
		[Tooltip("Mouse Over event")]
		public FsmEvent mouseOver;

		// Token: 0x040055E9 RID: 21993
		[Tooltip("Mouse Up event")]
		public FsmEvent mouseUp;

		// Token: 0x040055EA RID: 21994
		[Tooltip("Mouse Off event")]
		public FsmEvent mouseOff;

		// Token: 0x040055EB RID: 21995
		[Tooltip("Check for clicks as soon as the state machine enters this state.")]
		public bool checkFirstFrame;

		// Token: 0x040055EC RID: 21996
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040055ED RID: 21997
		[Tooltip("Stop processing after an event is triggered.")]
		public bool oneShot;

		// Token: 0x040055EE RID: 21998
		[Tooltip("Click Count")]
		public FsmInt ClickCount = 0;

		// Token: 0x040055EF RID: 21999
		private int m_RandomValue;

		// Token: 0x040055F0 RID: 22000
		private bool m_opened = true;
	}
}
