using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CFD RID: 3325
	[Tooltip("Sends an event when a swipe is detected.")]
	[ActionCategory(33)]
	public class SwipeGestureEvent : FsmStateAction
	{
		// Token: 0x060069AB RID: 27051 RVA: 0x001EF30C File Offset: 0x001ED50C
		public override void Reset()
		{
			this.minSwipeDistance = 0.1f;
			this.swipeLeftEvent = null;
			this.swipeRightEvent = null;
			this.swipeUpEvent = null;
			this.swipeDownEvent = null;
		}

		// Token: 0x060069AC RID: 27052 RVA: 0x001EF348 File Offset: 0x001ED548
		public override void OnEnter()
		{
			this.screenDiagonalSize = Mathf.Sqrt((float)(Screen.width * Screen.width + Screen.height * Screen.height));
			this.minSwipeDistancePixels = this.minSwipeDistance.Value * this.screenDiagonalSize;
		}

		// Token: 0x060069AD RID: 27053 RVA: 0x001EF390 File Offset: 0x001ED590
		public override void OnUpdate()
		{
			if (Input.touchCount > 0)
			{
				Touch touch = Input.touches[0];
				switch (touch.phase)
				{
				case 0:
					this.touchStarted = true;
					this.touchStartPos = touch.position;
					break;
				case 3:
					if (this.touchStarted)
					{
						this.TestForSwipeGesture(touch);
						this.touchStarted = false;
					}
					break;
				case 4:
					this.touchStarted = false;
					break;
				}
			}
		}

		// Token: 0x060069AE RID: 27054 RVA: 0x001EF430 File Offset: 0x001ED630
		private void TestForSwipeGesture(Touch touch)
		{
			Vector2 position = touch.position;
			float num = Vector2.Distance(position, this.touchStartPos);
			if (num > this.minSwipeDistancePixels)
			{
				float num2 = position.y - this.touchStartPos.y;
				float num3 = position.x - this.touchStartPos.x;
				float num4 = 57.29578f * Mathf.Atan2(num3, num2);
				num4 = (360f + num4 - 45f) % 360f;
				Debug.Log(num4);
				if (num4 < 90f)
				{
					base.Fsm.Event(this.swipeRightEvent);
				}
				else if (num4 < 180f)
				{
					base.Fsm.Event(this.swipeDownEvent);
				}
				else if (num4 < 270f)
				{
					base.Fsm.Event(this.swipeLeftEvent);
				}
				else
				{
					base.Fsm.Event(this.swipeUpEvent);
				}
			}
		}

		// Token: 0x04005164 RID: 20836
		[Tooltip("How far a touch has to travel to be considered a swipe. Uses normalized distance (e.g. 1 = 1 screen diagonal distance). Should generally be a very small number.")]
		public FsmFloat minSwipeDistance;

		// Token: 0x04005165 RID: 20837
		[Tooltip("Event to send when swipe left detected.")]
		public FsmEvent swipeLeftEvent;

		// Token: 0x04005166 RID: 20838
		[Tooltip("Event to send when swipe right detected.")]
		public FsmEvent swipeRightEvent;

		// Token: 0x04005167 RID: 20839
		[Tooltip("Event to send when swipe up detected.")]
		public FsmEvent swipeUpEvent;

		// Token: 0x04005168 RID: 20840
		[Tooltip("Event to send when swipe down detected.")]
		public FsmEvent swipeDownEvent;

		// Token: 0x04005169 RID: 20841
		private float screenDiagonalSize;

		// Token: 0x0400516A RID: 20842
		private float minSwipeDistancePixels;

		// Token: 0x0400516B RID: 20843
		private bool touchStarted;

		// Token: 0x0400516C RID: 20844
		private Vector2 touchStartPos;
	}
}
