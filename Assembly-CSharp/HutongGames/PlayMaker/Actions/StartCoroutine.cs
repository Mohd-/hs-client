using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF3 RID: 3315
	[ActionCategory(11)]
	[Tooltip("Start a Coroutine in a Behaviour on a Game Object. See Unity StartCoroutine docs.")]
	public class StartCoroutine : FsmStateAction
	{
		// Token: 0x06006980 RID: 27008 RVA: 0x001EE977 File Offset: 0x001ECB77
		public override void Reset()
		{
			this.gameObject = null;
			this.behaviour = null;
			this.functionCall = null;
			this.stopOnExit = false;
		}

		// Token: 0x06006981 RID: 27009 RVA: 0x001EE995 File Offset: 0x001ECB95
		public override void OnEnter()
		{
			this.DoStartCoroutine();
			base.Finish();
		}

		// Token: 0x06006982 RID: 27010 RVA: 0x001EE9A4 File Offset: 0x001ECBA4
		private void DoStartCoroutine()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.component = (ownerDefaultTarget.GetComponent(this.behaviour.Value) as MonoBehaviour);
			if (this.component == null)
			{
				this.LogWarning("StartCoroutine: " + ownerDefaultTarget.name + " missing behaviour: " + this.behaviour.Value);
				return;
			}
			string parameterType = this.functionCall.ParameterType;
			if (parameterType != null)
			{
				if (StartCoroutine.<>f__switch$map7B == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
					dictionary.Add("None", 0);
					dictionary.Add("int", 1);
					dictionary.Add("float", 2);
					dictionary.Add("string", 3);
					dictionary.Add("bool", 4);
					dictionary.Add("Vector2", 5);
					dictionary.Add("Vector3", 6);
					dictionary.Add("Rect", 7);
					dictionary.Add("GameObject", 8);
					dictionary.Add("Material", 9);
					dictionary.Add("Texture", 10);
					dictionary.Add("Quaternion", 11);
					dictionary.Add("Object", 12);
					StartCoroutine.<>f__switch$map7B = dictionary;
				}
				int num;
				if (StartCoroutine.<>f__switch$map7B.TryGetValue(parameterType, ref num))
				{
					switch (num)
					{
					case 0:
						this.component.StartCoroutine(this.functionCall.FunctionName);
						return;
					case 1:
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.IntParameter.Value);
						return;
					case 2:
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.FloatParameter.Value);
						return;
					case 3:
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.StringParameter.Value);
						return;
					case 4:
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.BoolParameter.Value);
						return;
					case 5:
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.Vector2Parameter.Value);
						return;
					case 6:
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.Vector3Parameter.Value);
						return;
					case 7:
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.RectParamater.Value);
						return;
					case 8:
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.GameObjectParameter.Value);
						return;
					case 9:
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.MaterialParameter.Value);
						break;
					case 10:
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.TextureParameter.Value);
						break;
					case 11:
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.QuaternionParameter.Value);
						break;
					case 12:
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.ObjectParameter.Value);
						return;
					}
				}
			}
		}

		// Token: 0x06006983 RID: 27011 RVA: 0x001EED66 File Offset: 0x001ECF66
		public override void OnExit()
		{
			if (this.component == null)
			{
				return;
			}
			if (this.stopOnExit)
			{
				this.component.StopCoroutine(this.functionCall.FunctionName);
			}
		}

		// Token: 0x0400513D RID: 20797
		[RequiredField]
		[Tooltip("The game object that owns the Behaviour.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400513E RID: 20798
		[Tooltip("The Behaviour that contains the method to start as a coroutine.")]
		[RequiredField]
		[UIHint(2)]
		public FsmString behaviour;

		// Token: 0x0400513F RID: 20799
		[UIHint(5)]
		[Tooltip("The name of the coroutine method.")]
		[RequiredField]
		public FunctionCall functionCall;

		// Token: 0x04005140 RID: 20800
		[Tooltip("Stop the coroutine when the state is exited.")]
		public bool stopOnExit;

		// Token: 0x04005141 RID: 20801
		private MonoBehaviour component;
	}
}
