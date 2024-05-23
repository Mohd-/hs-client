using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BFD RID: 3069
	[Tooltip("Gets info on the last event that caused a state change. See also Set Event Data action.")]
	[ActionCategory(12)]
	public class GetEventInfo : FsmStateAction
	{
		// Token: 0x06006546 RID: 25926 RVA: 0x001E11F8 File Offset: 0x001DF3F8
		public override void Reset()
		{
			this.sentByGameObject = null;
			this.fsmName = null;
			this.getBoolData = null;
			this.getIntData = null;
			this.getFloatData = null;
			this.getVector2Data = null;
			this.getVector3Data = null;
			this.getStringData = null;
			this.getGameObjectData = null;
			this.getRectData = null;
			this.getQuaternionData = null;
			this.getMaterialData = null;
			this.getTextureData = null;
			this.getColorData = null;
			this.getObjectData = null;
		}

		// Token: 0x06006547 RID: 25927 RVA: 0x001E1270 File Offset: 0x001DF470
		public override void OnEnter()
		{
			if (Fsm.EventData.SentByFsm != null)
			{
				this.sentByGameObject.Value = Fsm.EventData.SentByFsm.GameObject;
				this.fsmName.Value = Fsm.EventData.SentByFsm.Name;
			}
			else
			{
				this.sentByGameObject.Value = null;
				this.fsmName.Value = string.Empty;
			}
			this.getBoolData.Value = Fsm.EventData.BoolData;
			this.getIntData.Value = Fsm.EventData.IntData;
			this.getFloatData.Value = Fsm.EventData.FloatData;
			this.getVector2Data.Value = Fsm.EventData.Vector2Data;
			this.getVector3Data.Value = Fsm.EventData.Vector3Data;
			this.getStringData.Value = Fsm.EventData.StringData;
			this.getGameObjectData.Value = Fsm.EventData.GameObjectData;
			this.getRectData.Value = Fsm.EventData.RectData;
			this.getQuaternionData.Value = Fsm.EventData.QuaternionData;
			this.getMaterialData.Value = Fsm.EventData.MaterialData;
			this.getTextureData.Value = Fsm.EventData.TextureData;
			this.getColorData.Value = Fsm.EventData.ColorData;
			this.getObjectData.Value = Fsm.EventData.ObjectData;
			base.Finish();
		}

		// Token: 0x04004CEA RID: 19690
		[UIHint(10)]
		public FsmGameObject sentByGameObject;

		// Token: 0x04004CEB RID: 19691
		[UIHint(10)]
		public FsmString fsmName;

		// Token: 0x04004CEC RID: 19692
		[UIHint(10)]
		public FsmBool getBoolData;

		// Token: 0x04004CED RID: 19693
		[UIHint(10)]
		public FsmInt getIntData;

		// Token: 0x04004CEE RID: 19694
		[UIHint(10)]
		public FsmFloat getFloatData;

		// Token: 0x04004CEF RID: 19695
		[UIHint(10)]
		public FsmVector2 getVector2Data;

		// Token: 0x04004CF0 RID: 19696
		[UIHint(10)]
		public FsmVector3 getVector3Data;

		// Token: 0x04004CF1 RID: 19697
		[UIHint(10)]
		public FsmString getStringData;

		// Token: 0x04004CF2 RID: 19698
		[UIHint(10)]
		public FsmGameObject getGameObjectData;

		// Token: 0x04004CF3 RID: 19699
		[UIHint(10)]
		public FsmRect getRectData;

		// Token: 0x04004CF4 RID: 19700
		[UIHint(10)]
		public FsmQuaternion getQuaternionData;

		// Token: 0x04004CF5 RID: 19701
		[UIHint(10)]
		public FsmMaterial getMaterialData;

		// Token: 0x04004CF6 RID: 19702
		[UIHint(10)]
		public FsmTexture getTextureData;

		// Token: 0x04004CF7 RID: 19703
		[UIHint(10)]
		public FsmColor getColorData;

		// Token: 0x04004CF8 RID: 19704
		[UIHint(10)]
		public FsmObject getObjectData;
	}
}
