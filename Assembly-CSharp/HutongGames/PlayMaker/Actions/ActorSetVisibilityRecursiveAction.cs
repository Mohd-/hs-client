using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D98 RID: 3480
	[ActionCategory("Pegasus")]
	[Tooltip("Sets the visibility on a game object and its children. Will properly Show/Hide Actors in the hierarchy.")]
	public class ActorSetVisibilityRecursiveAction : FsmStateAction
	{
		// Token: 0x06006C78 RID: 27768 RVA: 0x001FE7E4 File Offset: 0x001FC9E4
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_Visible = false;
			this.m_IgnoreSpells = false;
			this.m_ResetOnExit = false;
			this.m_IncludeChildren = true;
			this.m_initialVisibility.Clear();
		}

		// Token: 0x06006C79 RID: 27769 RVA: 0x001FE82C File Offset: 0x001FCA2C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget != null)
			{
				if (this.m_ResetOnExit)
				{
					this.SaveInitialVisibility(ownerDefaultTarget);
				}
				this.SetVisibility(ownerDefaultTarget);
			}
			base.Finish();
		}

		// Token: 0x06006C7A RID: 27770 RVA: 0x001FE876 File Offset: 0x001FCA76
		public override void OnExit()
		{
			if (this.m_ResetOnExit)
			{
				this.RestoreInitialVisibility();
			}
		}

		// Token: 0x06006C7B RID: 27771 RVA: 0x001FE88C File Offset: 0x001FCA8C
		private void SaveInitialVisibility(GameObject go)
		{
			Actor component = go.GetComponent<Actor>();
			if (component != null)
			{
				this.m_initialVisibility[go] = component.IsShown();
				return;
			}
			Renderer component2 = go.GetComponent<Renderer>();
			if (component2 != null)
			{
				this.m_initialVisibility[go] = component2.enabled;
			}
			if (this.m_IncludeChildren)
			{
				foreach (object obj in go.transform)
				{
					Transform transform = (Transform)obj;
					this.SaveInitialVisibility(transform.gameObject);
				}
			}
		}

		// Token: 0x06006C7C RID: 27772 RVA: 0x001FE950 File Offset: 0x001FCB50
		private void RestoreInitialVisibility()
		{
			foreach (KeyValuePair<GameObject, bool> keyValuePair in this.m_initialVisibility)
			{
				GameObject key = keyValuePair.Key;
				bool value = keyValuePair.Value;
				Actor component = key.GetComponent<Actor>();
				if (component != null)
				{
					if (value)
					{
						this.ShowActor(component);
					}
					else
					{
						this.HideActor(component);
					}
				}
				else
				{
					Renderer component2 = key.GetComponent<Renderer>();
					component2.enabled = value;
				}
			}
		}

		// Token: 0x06006C7D RID: 27773 RVA: 0x001FE9F8 File Offset: 0x001FCBF8
		private void SetVisibility(GameObject go)
		{
			Actor component = go.GetComponent<Actor>();
			if (component != null)
			{
				if (this.m_Visible.Value)
				{
					this.ShowActor(component);
				}
				else
				{
					this.HideActor(component);
				}
				return;
			}
			Renderer component2 = go.GetComponent<Renderer>();
			if (component2 != null)
			{
				component2.enabled = this.m_Visible.Value;
			}
			if (this.m_IncludeChildren)
			{
				foreach (object obj in go.transform)
				{
					Transform transform = (Transform)obj;
					this.SetVisibility(transform.gameObject);
				}
			}
		}

		// Token: 0x06006C7E RID: 27774 RVA: 0x001FEACC File Offset: 0x001FCCCC
		private void ShowActor(Actor actor)
		{
			actor.Show(this.m_IgnoreSpells.Value);
		}

		// Token: 0x06006C7F RID: 27775 RVA: 0x001FEADF File Offset: 0x001FCCDF
		private void HideActor(Actor actor)
		{
			actor.Hide(this.m_IgnoreSpells.Value);
		}

		// Token: 0x04005511 RID: 21777
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005512 RID: 21778
		[Tooltip("Should objects be set to visible or invisible?")]
		public FsmBool m_Visible;

		// Token: 0x04005513 RID: 21779
		[Tooltip("Don't touch the Actor's SpellTable when setting visibility")]
		public FsmBool m_IgnoreSpells;

		// Token: 0x04005514 RID: 21780
		[Tooltip("Resets to the initial visibility once it leaves the state")]
		public bool m_ResetOnExit;

		// Token: 0x04005515 RID: 21781
		[Tooltip("Should children of the Game Object be affected?")]
		public bool m_IncludeChildren;

		// Token: 0x04005516 RID: 21782
		private Map<GameObject, bool> m_initialVisibility = new Map<GameObject, bool>();
	}
}
