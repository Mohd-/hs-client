using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C8F RID: 3215
	[Tooltip("Sends a Message to a Game Object. See Unity docs for SendMessage.")]
	[ActionCategory(11)]
	public class SendMessage : FsmStateAction
	{
		// Token: 0x060067C6 RID: 26566 RVA: 0x001E9311 File Offset: 0x001E7511
		public override void Reset()
		{
			this.gameObject = null;
			this.delivery = SendMessage.MessageType.SendMessage;
			this.options = 1;
			this.functionCall = null;
		}

		// Token: 0x060067C7 RID: 26567 RVA: 0x001E932F File Offset: 0x001E752F
		public override void OnEnter()
		{
			this.DoSendMessage();
			base.Finish();
		}

		// Token: 0x060067C8 RID: 26568 RVA: 0x001E9340 File Offset: 0x001E7540
		private void DoSendMessage()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			object obj = null;
			string parameterType = this.functionCall.ParameterType;
			if (parameterType != null)
			{
				if (SendMessage.<>f__switch$map7A == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(14);
					dictionary.Add("None", 0);
					dictionary.Add("bool", 1);
					dictionary.Add("int", 2);
					dictionary.Add("float", 3);
					dictionary.Add("string", 4);
					dictionary.Add("Vector2", 5);
					dictionary.Add("Vector3", 6);
					dictionary.Add("Rect", 7);
					dictionary.Add("GameObject", 8);
					dictionary.Add("Material", 9);
					dictionary.Add("Texture", 10);
					dictionary.Add("Color", 11);
					dictionary.Add("Quaternion", 12);
					dictionary.Add("Object", 13);
					SendMessage.<>f__switch$map7A = dictionary;
				}
				int num;
				if (SendMessage.<>f__switch$map7A.TryGetValue(parameterType, ref num))
				{
					switch (num)
					{
					case 1:
						obj = this.functionCall.BoolParameter.Value;
						break;
					case 2:
						obj = this.functionCall.IntParameter.Value;
						break;
					case 3:
						obj = this.functionCall.FloatParameter.Value;
						break;
					case 4:
						obj = this.functionCall.StringParameter.Value;
						break;
					case 5:
						obj = this.functionCall.Vector2Parameter.Value;
						break;
					case 6:
						obj = this.functionCall.Vector3Parameter.Value;
						break;
					case 7:
						obj = this.functionCall.RectParamater.Value;
						break;
					case 8:
						obj = this.functionCall.GameObjectParameter.Value;
						break;
					case 9:
						obj = this.functionCall.MaterialParameter.Value;
						break;
					case 10:
						obj = this.functionCall.TextureParameter.Value;
						break;
					case 11:
						obj = this.functionCall.ColorParameter.Value;
						break;
					case 12:
						obj = this.functionCall.QuaternionParameter.Value;
						break;
					case 13:
						obj = this.functionCall.ObjectParameter.Value;
						break;
					}
				}
			}
			switch (this.delivery)
			{
			case SendMessage.MessageType.SendMessage:
				ownerDefaultTarget.SendMessage(this.functionCall.FunctionName, obj, this.options);
				return;
			case SendMessage.MessageType.SendMessageUpwards:
				ownerDefaultTarget.SendMessageUpwards(this.functionCall.FunctionName, obj, this.options);
				return;
			case SendMessage.MessageType.BroadcastMessage:
				ownerDefaultTarget.BroadcastMessage(this.functionCall.FunctionName, obj, this.options);
				return;
			default:
				return;
			}
		}

		// Token: 0x04004F9D RID: 20381
		[Tooltip("GameObject that sends the message.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004F9E RID: 20382
		[Tooltip("Where to send the message.\nSee Unity docs.")]
		public SendMessage.MessageType delivery;

		// Token: 0x04004F9F RID: 20383
		[Tooltip("Send options.\nSee Unity docs.")]
		public SendMessageOptions options;

		// Token: 0x04004FA0 RID: 20384
		[RequiredField]
		public FunctionCall functionCall;

		// Token: 0x02000C90 RID: 3216
		public enum MessageType
		{
			// Token: 0x04004FA3 RID: 20387
			SendMessage,
			// Token: 0x04004FA4 RID: 20388
			SendMessageUpwards,
			// Token: 0x04004FA5 RID: 20389
			BroadcastMessage
		}
	}
}
