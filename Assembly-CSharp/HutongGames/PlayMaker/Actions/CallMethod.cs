using System;
using System.Reflection;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B68 RID: 2920
	[ActionCategory(11)]
	public class CallMethod : FsmStateAction
	{
		// Token: 0x06006306 RID: 25350 RVA: 0x001D95F8 File Offset: 0x001D77F8
		public override void OnEnter()
		{
			this.parametersArray = new object[this.parameters.Length];
			this.DoMethodCall();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006307 RID: 25351 RVA: 0x001D962F File Offset: 0x001D782F
		public override void OnUpdate()
		{
			this.DoMethodCall();
		}

		// Token: 0x06006308 RID: 25352 RVA: 0x001D9638 File Offset: 0x001D7838
		private void DoMethodCall()
		{
			if (this.behaviour.Value == null)
			{
				base.Finish();
				return;
			}
			if (this.cachedBehaviour != this.behaviour.Value)
			{
				this.errorString = string.Empty;
				if (!this.DoCache())
				{
					Debug.LogError(this.errorString);
					base.Finish();
					return;
				}
			}
			object value;
			if (this.cachedParameterInfo.Length == 0)
			{
				value = this.cachedMethodInfo.Invoke(this.cachedBehaviour, null);
			}
			else
			{
				for (int i = 0; i < this.parameters.Length; i++)
				{
					FsmVar fsmVar = this.parameters[i];
					fsmVar.UpdateValue();
					this.parametersArray[i] = fsmVar.GetValue();
				}
				value = this.cachedMethodInfo.Invoke(this.cachedBehaviour, this.parametersArray);
			}
			this.storeResult.SetValue(value);
		}

		// Token: 0x06006309 RID: 25353 RVA: 0x001D9728 File Offset: 0x001D7928
		private bool DoCache()
		{
			this.cachedBehaviour = (this.behaviour.Value as MonoBehaviour);
			if (this.cachedBehaviour == null)
			{
				this.errorString += "Behaviour is invalid!\n";
				base.Finish();
				return false;
			}
			this.cachedType = this.behaviour.Value.GetType();
			this.cachedMethodInfo = this.cachedType.GetMethod(this.methodName.Value);
			if (this.cachedMethodInfo == null)
			{
				this.errorString = this.errorString + "Method Name is invalid: " + this.methodName.Value + "\n";
				base.Finish();
				return false;
			}
			this.cachedParameterInfo = this.cachedMethodInfo.GetParameters();
			return true;
		}

		// Token: 0x0600630A RID: 25354 RVA: 0x001D97F8 File Offset: 0x001D79F8
		public override string ErrorCheck()
		{
			this.errorString = string.Empty;
			this.DoCache();
			if (!string.IsNullOrEmpty(this.errorString))
			{
				return this.errorString;
			}
			if (this.parameters.Length != this.cachedParameterInfo.Length)
			{
				return string.Concat(new object[]
				{
					"Parameter count does not match method.\nMethod has ",
					this.cachedParameterInfo.Length,
					" parameters.\nYou specified ",
					this.parameters.Length,
					" paramaters."
				});
			}
			for (int i = 0; i < this.parameters.Length; i++)
			{
				FsmVar fsmVar = this.parameters[i];
				Type realType = fsmVar.RealType;
				Type parameterType = this.cachedParameterInfo[i].ParameterType;
				if (!object.ReferenceEquals(realType, parameterType))
				{
					return string.Concat(new object[]
					{
						"Parameters do not match method signature.\nParameter ",
						i + 1,
						" (",
						realType,
						") should be of type: ",
						parameterType
					});
				}
			}
			if (object.ReferenceEquals(this.cachedMethodInfo.ReturnType, typeof(void)))
			{
				if (!string.IsNullOrEmpty(this.storeResult.variableName))
				{
					return "Method does not have return.\nSpecify 'none' in Store Result.";
				}
			}
			else if (!object.ReferenceEquals(this.cachedMethodInfo.ReturnType, this.storeResult.RealType))
			{
				return "Store Result is of the wrong type.\nIt should be of type: " + this.cachedMethodInfo.ReturnType;
			}
			return string.Empty;
		}

		// Token: 0x04004A7F RID: 19071
		[ObjectType(typeof(MonoBehaviour))]
		[Tooltip("Store the component in an Object variable.\nNOTE: Set theObject variable's Object Type to get a component of that type. E.g., set Object Type to UnityEngine.AudioListener to get the AudioListener component on the camera.")]
		public FsmObject behaviour;

		// Token: 0x04004A80 RID: 19072
		[Tooltip("Name of the method to call on the component")]
		public FsmString methodName;

		// Token: 0x04004A81 RID: 19073
		[Tooltip("Method paramters. NOTE: these must match the method's signature!")]
		public FsmVar[] parameters;

		// Token: 0x04004A82 RID: 19074
		[UIHint(10)]
		[ActionSection("Store Result")]
		[Tooltip("Store the result of the method call.")]
		public FsmVar storeResult;

		// Token: 0x04004A83 RID: 19075
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04004A84 RID: 19076
		private Object cachedBehaviour;

		// Token: 0x04004A85 RID: 19077
		private Type cachedType;

		// Token: 0x04004A86 RID: 19078
		private MethodInfo cachedMethodInfo;

		// Token: 0x04004A87 RID: 19079
		private ParameterInfo[] cachedParameterInfo;

		// Token: 0x04004A88 RID: 19080
		private object[] parametersArray;

		// Token: 0x04004A89 RID: 19081
		private string errorString;
	}
}
