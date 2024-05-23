using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D00 RID: 3328
	[ActionCategory(33)]
	[Tooltip("Sends events when a GUI Texture or GUI Text is touched. Optionally filter by a fingerID.")]
	public class TouchGUIEvent : FsmStateAction
	{
		// Token: 0x060069B6 RID: 27062 RVA: 0x001EF6DC File Offset: 0x001ED8DC
		public override void Reset()
		{
			this.gameObject = null;
			this.fingerId = new FsmInt
			{
				UseVariable = true
			};
			this.touchBegan = null;
			this.touchMoved = null;
			this.touchStationary = null;
			this.touchEnded = null;
			this.touchCanceled = null;
			this.storeFingerId = null;
			this.storeHitPoint = null;
			this.normalizeHitPoint = false;
			this.storeOffset = null;
			this.relativeTo = TouchGUIEvent.OffsetOptions.Center;
			this.normalizeOffset = true;
			this.everyFrame = true;
		}

		// Token: 0x060069B7 RID: 27063 RVA: 0x001EF762 File Offset: 0x001ED962
		public override void OnEnter()
		{
			this.DoTouchGUIEvent();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069B8 RID: 27064 RVA: 0x001EF77B File Offset: 0x001ED97B
		public override void OnUpdate()
		{
			this.DoTouchGUIEvent();
		}

		// Token: 0x060069B9 RID: 27065 RVA: 0x001EF784 File Offset: 0x001ED984
		private void DoTouchGUIEvent()
		{
			if (Input.touchCount > 0)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				this.guiElement = (ownerDefaultTarget.GetComponent<GUITexture>() ?? ownerDefaultTarget.GetComponent<GUIText>());
				if (this.guiElement == null)
				{
					return;
				}
				foreach (Touch touch in Input.touches)
				{
					this.DoTouch(touch);
				}
			}
		}

		// Token: 0x060069BA RID: 27066 RVA: 0x001EF814 File Offset: 0x001EDA14
		private void DoTouch(Touch touch)
		{
			if (this.fingerId.IsNone || touch.fingerId == this.fingerId.Value)
			{
				Vector3 vector = touch.position;
				if (this.guiElement.HitTest(vector))
				{
					if (touch.phase == null)
					{
						this.touchStartPos = vector;
					}
					this.storeFingerId.Value = touch.fingerId;
					if (this.normalizeHitPoint.Value)
					{
						vector.x /= (float)Screen.width;
						vector.y /= (float)Screen.height;
					}
					this.storeHitPoint.Value = vector;
					this.DoTouchOffset(vector);
					switch (touch.phase)
					{
					case 0:
						base.Fsm.Event(this.touchBegan);
						return;
					case 1:
						base.Fsm.Event(this.touchMoved);
						return;
					case 2:
						base.Fsm.Event(this.touchStationary);
						return;
					case 3:
						base.Fsm.Event(this.touchEnded);
						return;
					case 4:
						base.Fsm.Event(this.touchCanceled);
						return;
					}
				}
				else
				{
					base.Fsm.Event(this.notTouching);
				}
			}
		}

		// Token: 0x060069BB RID: 27067 RVA: 0x001EF970 File Offset: 0x001EDB70
		private void DoTouchOffset(Vector3 touchPos)
		{
			if (this.storeOffset.IsNone)
			{
				return;
			}
			Rect screenRect = this.guiElement.GetScreenRect();
			Vector3 value = default(Vector3);
			switch (this.relativeTo)
			{
			case TouchGUIEvent.OffsetOptions.TopLeft:
				value.x = touchPos.x - screenRect.x;
				value.y = touchPos.y - screenRect.y;
				break;
			case TouchGUIEvent.OffsetOptions.Center:
			{
				Vector3 vector;
				vector..ctor(screenRect.x + screenRect.width * 0.5f, screenRect.y + screenRect.height * 0.5f, 0f);
				value = touchPos - vector;
				break;
			}
			case TouchGUIEvent.OffsetOptions.TouchStart:
				value = touchPos - this.touchStartPos;
				break;
			}
			if (this.normalizeOffset.Value)
			{
				value.x /= screenRect.width;
				value.y /= screenRect.height;
			}
			this.storeOffset.Value = value;
		}

		// Token: 0x04005174 RID: 20852
		[RequiredField]
		[CheckForComponent(typeof(GUIElement))]
		[Tooltip("The Game Object that owns the GUI Texture or GUI Text.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005175 RID: 20853
		[Tooltip("Only detect touches that match this fingerID, or set to None.")]
		public FsmInt fingerId;

		// Token: 0x04005176 RID: 20854
		[ActionSection("Events")]
		[Tooltip("Event to send on touch began.")]
		public FsmEvent touchBegan;

		// Token: 0x04005177 RID: 20855
		[Tooltip("Event to send on touch moved.")]
		public FsmEvent touchMoved;

		// Token: 0x04005178 RID: 20856
		[Tooltip("Event to send on stationary touch.")]
		public FsmEvent touchStationary;

		// Token: 0x04005179 RID: 20857
		[Tooltip("Event to send on touch ended.")]
		public FsmEvent touchEnded;

		// Token: 0x0400517A RID: 20858
		[Tooltip("Event to send on touch cancel.")]
		public FsmEvent touchCanceled;

		// Token: 0x0400517B RID: 20859
		[Tooltip("Event to send if not touching (finger down but not over the GUI element)")]
		public FsmEvent notTouching;

		// Token: 0x0400517C RID: 20860
		[UIHint(10)]
		[Tooltip("Store the fingerId of the touch.")]
		[ActionSection("Store Results")]
		public FsmInt storeFingerId;

		// Token: 0x0400517D RID: 20861
		[UIHint(10)]
		[Tooltip("Store the screen position where the GUI element was touched.")]
		public FsmVector3 storeHitPoint;

		// Token: 0x0400517E RID: 20862
		[Tooltip("Normalize the hit point screen coordinates (0-1).")]
		public FsmBool normalizeHitPoint;

		// Token: 0x0400517F RID: 20863
		[UIHint(10)]
		[Tooltip("Store the offset position of the hit.")]
		public FsmVector3 storeOffset;

		// Token: 0x04005180 RID: 20864
		[Tooltip("How to measure the offset.")]
		public TouchGUIEvent.OffsetOptions relativeTo;

		// Token: 0x04005181 RID: 20865
		[Tooltip("Normalize the offset.")]
		public FsmBool normalizeOffset;

		// Token: 0x04005182 RID: 20866
		[ActionSection("")]
		[Tooltip("Repeate every frame.")]
		public bool everyFrame;

		// Token: 0x04005183 RID: 20867
		private Vector3 touchStartPos;

		// Token: 0x04005184 RID: 20868
		private GUIElement guiElement;

		// Token: 0x02000D01 RID: 3329
		public enum OffsetOptions
		{
			// Token: 0x04005186 RID: 20870
			TopLeft,
			// Token: 0x04005187 RID: 20871
			Center,
			// Token: 0x04005188 RID: 20872
			TouchStart
		}
	}
}
