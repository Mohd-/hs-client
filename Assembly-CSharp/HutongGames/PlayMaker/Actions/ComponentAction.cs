using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B3A RID: 2874
	public abstract class ComponentAction<T> : FsmStateAction where T : Component
	{
		// Token: 0x17000936 RID: 2358
		// (get) Token: 0x06006227 RID: 25127 RVA: 0x001D3172 File Offset: 0x001D1372
		protected Rigidbody rigidbody
		{
			get
			{
				return this.component as Rigidbody;
			}
		}

		// Token: 0x17000937 RID: 2359
		// (get) Token: 0x06006228 RID: 25128 RVA: 0x001D3184 File Offset: 0x001D1384
		protected Renderer renderer
		{
			get
			{
				return this.component as Renderer;
			}
		}

		// Token: 0x17000938 RID: 2360
		// (get) Token: 0x06006229 RID: 25129 RVA: 0x001D3196 File Offset: 0x001D1396
		protected Animation animation
		{
			get
			{
				return this.component as Animation;
			}
		}

		// Token: 0x17000939 RID: 2361
		// (get) Token: 0x0600622A RID: 25130 RVA: 0x001D31A8 File Offset: 0x001D13A8
		protected AudioSource audio
		{
			get
			{
				return this.component as AudioSource;
			}
		}

		// Token: 0x1700093A RID: 2362
		// (get) Token: 0x0600622B RID: 25131 RVA: 0x001D31BA File Offset: 0x001D13BA
		protected Camera camera
		{
			get
			{
				return this.component as Camera;
			}
		}

		// Token: 0x1700093B RID: 2363
		// (get) Token: 0x0600622C RID: 25132 RVA: 0x001D31CC File Offset: 0x001D13CC
		protected GUIText guiText
		{
			get
			{
				return this.component as GUIText;
			}
		}

		// Token: 0x1700093C RID: 2364
		// (get) Token: 0x0600622D RID: 25133 RVA: 0x001D31DE File Offset: 0x001D13DE
		protected GUITexture guiTexture
		{
			get
			{
				return this.component as GUITexture;
			}
		}

		// Token: 0x1700093D RID: 2365
		// (get) Token: 0x0600622E RID: 25134 RVA: 0x001D31F0 File Offset: 0x001D13F0
		protected Light light
		{
			get
			{
				return this.component as Light;
			}
		}

		// Token: 0x1700093E RID: 2366
		// (get) Token: 0x0600622F RID: 25135 RVA: 0x001D3202 File Offset: 0x001D1402
		protected NetworkView networkView
		{
			get
			{
				return this.component as NetworkView;
			}
		}

		// Token: 0x06006230 RID: 25136 RVA: 0x001D3214 File Offset: 0x001D1414
		protected bool UpdateCache(GameObject go)
		{
			if (go == null)
			{
				return false;
			}
			if (this.component == null || this.cachedGameObject != go)
			{
				this.component = go.GetComponent<T>();
				this.cachedGameObject = go;
				if (this.component == null)
				{
					this.LogWarning("Missing component: " + typeof(T).FullName + " on: " + go.name);
				}
			}
			return this.component != null;
		}

		// Token: 0x0400493B RID: 18747
		private GameObject cachedGameObject;

		// Token: 0x0400493C RID: 18748
		private T component;
	}
}
