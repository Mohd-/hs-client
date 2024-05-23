using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC3 RID: 3011
	[Tooltip("Begin a GUILayout area that follows the specified game object. Useful for overlays (e.g., playerName). NOTE: Block must end with a corresponding GUILayoutEndArea.")]
	[ActionCategory(26)]
	public class GUILayoutBeginAreaFollowObject : FsmStateAction
	{
		// Token: 0x06006477 RID: 25719 RVA: 0x001DE35C File Offset: 0x001DC55C
		public override void Reset()
		{
			this.gameObject = null;
			this.offsetLeft = 0f;
			this.offsetTop = 0f;
			this.width = 1f;
			this.height = 1f;
			this.normalized = true;
			this.style = string.Empty;
		}

		// Token: 0x06006478 RID: 25720 RVA: 0x001DE3CC File Offset: 0x001DC5CC
		public override void OnGUI()
		{
			GameObject value = this.gameObject.Value;
			if (value == null || Camera.main == null)
			{
				GUILayoutBeginAreaFollowObject.DummyBeginArea();
				return;
			}
			Vector3 position = value.transform.position;
			if (Camera.main.transform.InverseTransformPoint(position).z < 0f)
			{
				GUILayoutBeginAreaFollowObject.DummyBeginArea();
				return;
			}
			Vector2 vector = Camera.main.WorldToScreenPoint(position);
			float num = vector.x + ((!this.normalized.Value) ? this.offsetLeft.Value : (this.offsetLeft.Value * (float)Screen.width));
			float num2 = vector.y + ((!this.normalized.Value) ? this.offsetTop.Value : (this.offsetTop.Value * (float)Screen.width));
			Rect rect;
			rect..ctor(num, num2, this.width.Value, this.height.Value);
			if (this.normalized.Value)
			{
				rect.width *= (float)Screen.width;
				rect.height *= (float)Screen.height;
			}
			rect.y = (float)Screen.height - rect.y;
			GUILayout.BeginArea(rect, this.style.Value);
		}

		// Token: 0x06006479 RID: 25721 RVA: 0x001DE544 File Offset: 0x001DC744
		private static void DummyBeginArea()
		{
			GUILayout.BeginArea(default(Rect));
		}

		// Token: 0x04004C00 RID: 19456
		[RequiredField]
		[Tooltip("The GameObject to follow.")]
		public FsmGameObject gameObject;

		// Token: 0x04004C01 RID: 19457
		[RequiredField]
		public FsmFloat offsetLeft;

		// Token: 0x04004C02 RID: 19458
		[RequiredField]
		public FsmFloat offsetTop;

		// Token: 0x04004C03 RID: 19459
		[RequiredField]
		public FsmFloat width;

		// Token: 0x04004C04 RID: 19460
		[RequiredField]
		public FsmFloat height;

		// Token: 0x04004C05 RID: 19461
		[Tooltip("Use normalized screen coordinates (0-1).")]
		public FsmBool normalized;

		// Token: 0x04004C06 RID: 19462
		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;
	}
}
