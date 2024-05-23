using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA3 RID: 3235
	[ActionCategory(12)]
	[Tooltip("Sets Event Data before sending an event. Get the Event Data, along with sender information, using Get Event Info action.")]
	public class SetEventData : FsmStateAction
	{
		// Token: 0x0600681C RID: 26652 RVA: 0x001EA2DC File Offset: 0x001E84DC
		public override void Reset()
		{
			this.setGameObjectData = new FsmGameObject
			{
				UseVariable = true
			};
			this.setIntData = new FsmInt
			{
				UseVariable = true
			};
			this.setFloatData = new FsmFloat
			{
				UseVariable = true
			};
			this.setStringData = new FsmString
			{
				UseVariable = true
			};
			this.setBoolData = new FsmBool
			{
				UseVariable = true
			};
			this.setVector2Data = new FsmVector2
			{
				UseVariable = true
			};
			this.setVector3Data = new FsmVector3
			{
				UseVariable = true
			};
			this.setRectData = new FsmRect
			{
				UseVariable = true
			};
			this.setQuaternionData = new FsmQuaternion
			{
				UseVariable = true
			};
			this.setColorData = new FsmColor
			{
				UseVariable = true
			};
			this.setMaterialData = new FsmMaterial
			{
				UseVariable = true
			};
			this.setTextureData = new FsmTexture
			{
				UseVariable = true
			};
			this.setObjectData = new FsmObject
			{
				UseVariable = true
			};
		}

		// Token: 0x0600681D RID: 26653 RVA: 0x001EA408 File Offset: 0x001E8608
		public override void OnEnter()
		{
			Fsm.EventData.BoolData = this.setBoolData.Value;
			Fsm.EventData.IntData = this.setIntData.Value;
			Fsm.EventData.FloatData = this.setFloatData.Value;
			Fsm.EventData.Vector2Data = this.setVector2Data.Value;
			Fsm.EventData.Vector3Data = this.setVector3Data.Value;
			Fsm.EventData.StringData = this.setStringData.Value;
			Fsm.EventData.GameObjectData = this.setGameObjectData.Value;
			Fsm.EventData.RectData = this.setRectData.Value;
			Fsm.EventData.QuaternionData = this.setQuaternionData.Value;
			Fsm.EventData.ColorData = this.setColorData.Value;
			Fsm.EventData.MaterialData = this.setMaterialData.Value;
			Fsm.EventData.TextureData = this.setTextureData.Value;
			Fsm.EventData.ObjectData = this.setObjectData.Value;
			base.Finish();
		}

		// Token: 0x04004FE1 RID: 20449
		public FsmGameObject setGameObjectData;

		// Token: 0x04004FE2 RID: 20450
		public FsmInt setIntData;

		// Token: 0x04004FE3 RID: 20451
		public FsmFloat setFloatData;

		// Token: 0x04004FE4 RID: 20452
		public FsmString setStringData;

		// Token: 0x04004FE5 RID: 20453
		public FsmBool setBoolData;

		// Token: 0x04004FE6 RID: 20454
		public FsmVector2 setVector2Data;

		// Token: 0x04004FE7 RID: 20455
		public FsmVector3 setVector3Data;

		// Token: 0x04004FE8 RID: 20456
		public FsmRect setRectData;

		// Token: 0x04004FE9 RID: 20457
		public FsmQuaternion setQuaternionData;

		// Token: 0x04004FEA RID: 20458
		public FsmColor setColorData;

		// Token: 0x04004FEB RID: 20459
		public FsmMaterial setMaterialData;

		// Token: 0x04004FEC RID: 20460
		public FsmTexture setTextureData;

		// Token: 0x04004FED RID: 20461
		public FsmObject setObjectData;
	}
}
