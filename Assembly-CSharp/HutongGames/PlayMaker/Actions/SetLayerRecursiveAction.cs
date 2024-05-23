using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DDC RID: 3548
	[ActionCategory("Pegasus")]
	[Tooltip("Sets the layer on a game object and its children.")]
	public class SetLayerRecursiveAction : FsmStateAction
	{
		// Token: 0x06006D91 RID: 28049 RVA: 0x00203173 File Offset: 0x00201373
		public override void Reset()
		{
			this.gameObject = null;
			this.layer = GameLayer.Default;
			this.resetOnExit = true;
			this.includeChildren = false;
			this.m_initialLayer.Clear();
		}

		// Token: 0x06006D92 RID: 28050 RVA: 0x0020319C File Offset: 0x0020139C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			if (this.includeChildren)
			{
				Transform[] componentsInChildren = ownerDefaultTarget.GetComponentsInChildren<Transform>();
				if (componentsInChildren != null)
				{
					foreach (Transform transform in componentsInChildren)
					{
						this.m_initialLayer[transform.gameObject] = (GameLayer)transform.gameObject.layer;
						transform.gameObject.layer = (int)this.layer;
					}
				}
			}
			else
			{
				Transform component = ownerDefaultTarget.GetComponent<Transform>();
				if (component != null)
				{
					this.m_initialLayer[component.gameObject] = (GameLayer)component.gameObject.layer;
					component.gameObject.layer = (int)this.layer;
				}
			}
			base.Finish();
		}

		// Token: 0x06006D93 RID: 28051 RVA: 0x00203284 File Offset: 0x00201484
		public override void OnExit()
		{
			if (!this.resetOnExit)
			{
				return;
			}
			foreach (KeyValuePair<GameObject, GameLayer> keyValuePair in this.m_initialLayer)
			{
				GameObject key = keyValuePair.Key;
				if (!(key == null))
				{
					key.layer = (int)keyValuePair.Value;
				}
			}
		}

		// Token: 0x04005637 RID: 22071
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005638 RID: 22072
		[Tooltip("Layer number")]
		public GameLayer layer;

		// Token: 0x04005639 RID: 22073
		[Tooltip("Resets to the initial layer once\nit leaves the state")]
		public bool resetOnExit;

		// Token: 0x0400563A RID: 22074
		public bool includeChildren;

		// Token: 0x0400563B RID: 22075
		private Map<GameObject, GameLayer> m_initialLayer = new Map<GameObject, GameLayer>();
	}
}
