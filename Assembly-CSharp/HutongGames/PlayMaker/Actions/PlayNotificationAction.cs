using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD1 RID: 3537
	[ActionCategory("Pegasus Audio")]
	[Tooltip("Plays a notification.")]
	public class PlayNotificationAction : FsmStateAction
	{
		// Token: 0x06006D61 RID: 28001 RVA: 0x002027AC File Offset: 0x002009AC
		public override void Reset()
		{
			this.m_NotificationPrefab = string.Empty;
			this.m_NotificationVO = string.Empty;
		}

		// Token: 0x06006D62 RID: 28002 RVA: 0x002027DC File Offset: 0x002009DC
		public override void OnEnter()
		{
			Vector3 position = NotificationManager.DEFAULT_CHARACTER_POS;
			if (!this.m_NotificationPosition.IsNone)
			{
				position = this.m_NotificationPosition.Value;
			}
			NotificationManager.Get().CreateCharacterQuote(FileUtils.GameAssetPathToName(this.m_NotificationPrefab.Value), position, GameStrings.Get(this.m_NotificationVO.Value), this.m_NotificationVO.Value, true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
		}

		// Token: 0x0400560D RID: 22029
		[Tooltip("Notification quote prefab to use.")]
		public FsmString m_NotificationPrefab;

		// Token: 0x0400560E RID: 22030
		[Tooltip("The VO line to play (+loc string).")]
		public FsmString m_NotificationVO;

		// Token: 0x0400560F RID: 22031
		[Tooltip("Notification popup position")]
		public FsmVector3 m_NotificationPosition;
	}
}
