using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DAE RID: 3502
	[ActionCategory("Pegasus")]
	[Tooltip("INTERNAL USE ONLY. Do not put this on your FSMs.")]
	public abstract class CameraAction : FsmStateAction
	{
		// Token: 0x06006CD4 RID: 27860 RVA: 0x002001AC File Offset: 0x001FE3AC
		protected Camera GetCamera(CameraAction.WhichCamera which, FsmGameObject specificCamera, FsmString namedCamera)
		{
			if (which != CameraAction.WhichCamera.SPECIFIC)
			{
				if (which == CameraAction.WhichCamera.NAMED)
				{
					string text = (!namedCamera.IsNone) ? namedCamera.Value : null;
					if (!string.IsNullOrEmpty(text))
					{
						if (this.m_namedCamera)
						{
							if (this.m_namedCamera.name == text)
							{
								return this.m_namedCamera;
							}
							this.m_namedCamera = null;
						}
						Camera camera = null;
						GameObject gameObject = GameObject.Find(text);
						if (gameObject)
						{
							camera = gameObject.GetComponent<Camera>();
						}
						if (camera)
						{
							this.m_namedCamera = camera;
							return this.m_namedCamera;
						}
					}
				}
			}
			else if (!specificCamera.IsNone)
			{
				return specificCamera.Value.GetComponent<Camera>();
			}
			return Camera.main;
		}

		// Token: 0x04005575 RID: 21877
		protected Camera m_namedCamera;

		// Token: 0x02000DAF RID: 3503
		public enum WhichCamera
		{
			// Token: 0x04005577 RID: 21879
			MAIN,
			// Token: 0x04005578 RID: 21880
			SPECIFIC,
			// Token: 0x04005579 RID: 21881
			NAMED
		}
	}
}
