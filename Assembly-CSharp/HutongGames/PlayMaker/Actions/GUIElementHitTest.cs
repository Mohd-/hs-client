using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BBE RID: 3006
	[ActionCategory(18)]
	[Tooltip("Performs a Hit Test on a Game Object with a GUITexture or GUIText component.")]
	public class GUIElementHitTest : FsmStateAction
	{
		// Token: 0x06006467 RID: 25703 RVA: 0x001DDD70 File Offset: 0x001DBF70
		public override void Reset()
		{
			this.gameObject = null;
			this.camera = null;
			this.screenPoint = new FsmVector3
			{
				UseVariable = true
			};
			this.screenX = new FsmFloat
			{
				UseVariable = true
			};
			this.screenY = new FsmFloat
			{
				UseVariable = true
			};
			this.normalized = true;
			this.hitEvent = null;
			this.everyFrame = true;
		}

		// Token: 0x06006468 RID: 25704 RVA: 0x001DDDE6 File Offset: 0x001DBFE6
		public override void OnEnter()
		{
			this.DoHitTest();
			if (!this.everyFrame.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x06006469 RID: 25705 RVA: 0x001DDE04 File Offset: 0x001DC004
		public override void OnUpdate()
		{
			this.DoHitTest();
		}

		// Token: 0x0600646A RID: 25706 RVA: 0x001DDE0C File Offset: 0x001DC00C
		private void DoHitTest()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.gameObjectCached)
			{
				this.guiElement = (ownerDefaultTarget.GetComponent<GUITexture>() ?? ownerDefaultTarget.GetComponent<GUIText>());
				this.gameObjectCached = ownerDefaultTarget;
			}
			if (this.guiElement == null)
			{
				base.Finish();
				return;
			}
			Vector3 vector = (!this.screenPoint.IsNone) ? this.screenPoint.Value : new Vector3(0f, 0f);
			if (!this.screenX.IsNone)
			{
				vector.x = this.screenX.Value;
			}
			if (!this.screenY.IsNone)
			{
				vector.y = this.screenY.Value;
			}
			if (this.normalized.Value)
			{
				vector.x *= (float)Screen.width;
				vector.y *= (float)Screen.height;
			}
			if (this.guiElement.HitTest(vector, this.camera))
			{
				this.storeResult.Value = true;
				base.Fsm.Event(this.hitEvent);
			}
			else
			{
				this.storeResult.Value = false;
			}
		}

		// Token: 0x04004BE6 RID: 19430
		[Tooltip("The GameObject that has a GUITexture or GUIText component.")]
		[RequiredField]
		[CheckForComponent(typeof(GUIElement))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004BE7 RID: 19431
		[Tooltip("Specify camera or use MainCamera as default.")]
		public Camera camera;

		// Token: 0x04004BE8 RID: 19432
		[Tooltip("A vector position on screen. Usually stored by actions like GetTouchInfo, or World To Screen Point.")]
		public FsmVector3 screenPoint;

		// Token: 0x04004BE9 RID: 19433
		[Tooltip("Specify screen X coordinate.")]
		public FsmFloat screenX;

		// Token: 0x04004BEA RID: 19434
		[Tooltip("Specify screen Y coordinate.")]
		public FsmFloat screenY;

		// Token: 0x04004BEB RID: 19435
		[Tooltip("Whether the specified screen coordinates are normalized (0-1).")]
		public FsmBool normalized;

		// Token: 0x04004BEC RID: 19436
		[Tooltip("Event to send if the Hit Test is true.")]
		public FsmEvent hitEvent;

		// Token: 0x04004BED RID: 19437
		[UIHint(10)]
		[Tooltip("Store the result of the Hit Test in a bool variable (true/false).")]
		public FsmBool storeResult;

		// Token: 0x04004BEE RID: 19438
		[Tooltip("Repeat every frame. Useful if you want to wait for the hit test to return true.")]
		public FsmBool everyFrame;

		// Token: 0x04004BEF RID: 19439
		private GUIElement guiElement;

		// Token: 0x04004BF0 RID: 19440
		private GameObject gameObjectCached;
	}
}
