using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C59 RID: 3161
	[ActionCategory(6)]
	[Tooltip("Sends Events based on mouse interactions with a Game Object: MouseOver, MouseDown, MouseUp, MouseOff. Use Ray Distance to set how close the camera must be to pick the object.")]
	public class MousePickEvent : FsmStateAction
	{
		// Token: 0x060066D6 RID: 26326 RVA: 0x001E5CF4 File Offset: 0x001E3EF4
		public override void Reset()
		{
			this.GameObject = null;
			this.rayDistance = 100f;
			this.mouseOver = null;
			this.mouseDown = null;
			this.mouseUp = null;
			this.mouseOff = null;
			this.layerMask = new FsmInt[0];
			this.invertMask = false;
			this.everyFrame = true;
		}

		// Token: 0x060066D7 RID: 26327 RVA: 0x001E5D53 File Offset: 0x001E3F53
		public override void OnEnter()
		{
			this.DoMousePickEvent();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066D8 RID: 26328 RVA: 0x001E5D6C File Offset: 0x001E3F6C
		public override void OnUpdate()
		{
			this.DoMousePickEvent();
		}

		// Token: 0x060066D9 RID: 26329 RVA: 0x001E5D74 File Offset: 0x001E3F74
		private void DoMousePickEvent()
		{
			bool flag = this.DoRaycast();
			base.Fsm.RaycastHitInfo = ActionHelpers.mousePickInfo;
			if (flag)
			{
				if (this.mouseDown != null && Input.GetMouseButtonDown(0))
				{
					base.Fsm.Event(this.mouseDown);
				}
				if (this.mouseOver != null)
				{
					base.Fsm.Event(this.mouseOver);
				}
				if (this.mouseUp != null && Input.GetMouseButtonUp(0))
				{
					base.Fsm.Event(this.mouseUp);
				}
			}
			else if (this.mouseOff != null)
			{
				base.Fsm.Event(this.mouseOff);
			}
		}

		// Token: 0x060066DA RID: 26330 RVA: 0x001E5E2C File Offset: 0x001E402C
		private bool DoRaycast()
		{
			GameObject gameObject = (this.GameObject.OwnerOption != null) ? this.GameObject.GameObject.Value : base.Owner;
			return ActionHelpers.IsMouseOver(gameObject, this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
		}

		// Token: 0x060066DB RID: 26331 RVA: 0x001E5E8C File Offset: 0x001E408C
		public override string ErrorCheck()
		{
			string text = string.Empty;
			text += ActionHelpers.CheckRayDistance(this.rayDistance.Value);
			return text + ActionHelpers.CheckPhysicsSetup(this.GameObject);
		}

		// Token: 0x04004EA6 RID: 20134
		[CheckForComponent(typeof(Collider))]
		public FsmOwnerDefault GameObject;

		// Token: 0x04004EA7 RID: 20135
		[Tooltip("Length of the ray to cast from the camera.")]
		public FsmFloat rayDistance = 100f;

		// Token: 0x04004EA8 RID: 20136
		[Tooltip("Event to send when the mouse is over the GameObject.")]
		public FsmEvent mouseOver;

		// Token: 0x04004EA9 RID: 20137
		[Tooltip("Event to send when the mouse is pressed while over the GameObject.")]
		public FsmEvent mouseDown;

		// Token: 0x04004EAA RID: 20138
		[Tooltip("Event to send when the mouse is released while over the GameObject.")]
		public FsmEvent mouseUp;

		// Token: 0x04004EAB RID: 20139
		[Tooltip("Event to send when the mouse moves off the GameObject.")]
		public FsmEvent mouseOff;

		// Token: 0x04004EAC RID: 20140
		[UIHint(8)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x04004EAD RID: 20141
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04004EAE RID: 20142
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
