using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020001BF RID: 447
public class HighlightState : MonoBehaviour
{
	// Token: 0x06001D0D RID: 7437 RVA: 0x00088494 File Offset: 0x00086694
	private void Awake()
	{
		if (this.m_RenderPlane == null)
		{
			if (!Application.isEditor)
			{
				Debug.LogError("m_RenderPlane is null!");
			}
			base.enabled = false;
		}
		else
		{
			this.m_RenderPlane.GetComponent<Renderer>().enabled = false;
			this.m_VisibilityState = false;
			this.m_FSM = this.m_RenderPlane.GetComponent<PlayMakerFSM>();
		}
		if (this.m_FSM != null)
		{
			this.m_FSM.enabled = true;
		}
		if (this.m_highlightType == HighlightStateType.NONE)
		{
			Transform parent = base.transform.parent;
			if (parent != null)
			{
				if (parent.GetComponent<ActorStateMgr>())
				{
					this.m_highlightType = HighlightStateType.CARD;
				}
				else
				{
					this.m_highlightType = HighlightStateType.HIGHLIGHT;
				}
			}
		}
		if (this.m_highlightType == HighlightStateType.NONE)
		{
			Debug.LogError("m_highlightType is not set!");
			base.enabled = false;
		}
		this.Setup();
	}

	// Token: 0x06001D0E RID: 7438 RVA: 0x00088580 File Offset: 0x00086780
	private void Update()
	{
		if (this.m_debugState != ActorStateType.NONE)
		{
			this.ChangeState(this.m_debugState);
			this.ForceUpdate();
		}
		if (!this.m_Hide)
		{
			if (this.m_isDirty)
			{
				if (this.m_RenderPlane == null)
				{
					return;
				}
				if (this.m_RenderPlane.GetComponent<Renderer>().enabled)
				{
					this.UpdateSilouette();
					this.m_isDirty = false;
				}
			}
			return;
		}
		if (this.m_RenderPlane == null)
		{
			return;
		}
		this.m_RenderPlane.GetComponent<Renderer>().enabled = false;
	}

	// Token: 0x06001D0F RID: 7439 RVA: 0x00088619 File Offset: 0x00086819
	private void OnApplicationFocus(bool state)
	{
		this.m_isDirty = true;
		this.m_forceRerender = true;
	}

	// Token: 0x06001D10 RID: 7440 RVA: 0x00088629 File Offset: 0x00086829
	protected void OnDestroy()
	{
		if (this.m_Material)
		{
			Object.Destroy(this.m_Material);
		}
	}

	// Token: 0x06001D11 RID: 7441 RVA: 0x00088646 File Offset: 0x00086846
	private void LateUpdate()
	{
	}

	// Token: 0x06001D12 RID: 7442 RVA: 0x00088648 File Offset: 0x00086848
	private void Setup()
	{
		this.m_seed = Random.value;
		this.m_CurrentState = ActorStateType.CARD_IDLE;
		this.m_RenderPlane.GetComponent<Renderer>().enabled = false;
		this.m_VisibilityState = false;
		if (this.m_Material == null)
		{
			Shader shader = ShaderUtils.FindShader(this.HIGHLIGHT_SHADER_NAME);
			if (!shader)
			{
				Debug.LogError("Failed to load Highlight Shader: " + this.HIGHLIGHT_SHADER_NAME);
				base.enabled = false;
			}
			this.m_Material = new Material(shader);
			this.m_RenderPlane.GetComponent<Renderer>().sharedMaterial = this.m_Material;
		}
		this.m_RenderPlane.GetComponent<Renderer>().sharedMaterial = this.m_Material;
	}

	// Token: 0x06001D13 RID: 7443 RVA: 0x000886FC File Offset: 0x000868FC
	public void Show()
	{
		this.m_Hide = false;
		if (this.m_RenderPlane == null)
		{
			return;
		}
		if (this.m_VisibilityState && !this.m_RenderPlane.GetComponent<Renderer>().enabled)
		{
			this.m_RenderPlane.GetComponent<Renderer>().enabled = true;
		}
	}

	// Token: 0x06001D14 RID: 7444 RVA: 0x00088754 File Offset: 0x00086954
	public void Hide()
	{
		this.m_Hide = true;
		if (this.m_RenderPlane == null)
		{
			return;
		}
		this.m_RenderPlane.GetComponent<Renderer>().enabled = false;
	}

	// Token: 0x06001D15 RID: 7445 RVA: 0x0008878B File Offset: 0x0008698B
	public void SetDirty()
	{
		this.m_isDirty = true;
	}

	// Token: 0x06001D16 RID: 7446 RVA: 0x00088794 File Offset: 0x00086994
	public void ForceUpdate()
	{
		this.m_isDirty = true;
		this.m_forceRerender = true;
	}

	// Token: 0x06001D17 RID: 7447 RVA: 0x000887A4 File Offset: 0x000869A4
	public void ContinuousUpdate(float updateTime)
	{
		base.StartCoroutine(this.ContinuousSilouetteRender(updateTime));
	}

	// Token: 0x06001D18 RID: 7448 RVA: 0x000887B4 File Offset: 0x000869B4
	public bool IsReady()
	{
		return this.m_Material != null;
	}

	// Token: 0x06001D19 RID: 7449 RVA: 0x000887C4 File Offset: 0x000869C4
	public bool ChangeState(ActorStateType stateType)
	{
		if (stateType == this.m_CurrentState)
		{
			return true;
		}
		this.m_PreviousState = this.m_CurrentState;
		this.m_CurrentState = stateType;
		if (stateType == ActorStateType.NONE)
		{
			this.m_RenderPlane.GetComponent<Renderer>().enabled = false;
			this.m_VisibilityState = false;
			return true;
		}
		if (stateType != ActorStateType.CARD_IDLE && stateType != ActorStateType.HIGHLIGHT_OFF)
		{
			foreach (HighlightRenderState highlightRenderState in this.m_HighlightStates)
			{
				if (highlightRenderState.m_StateType == stateType)
				{
					if (highlightRenderState.m_Material != null && this.m_Material != null)
					{
						this.m_Material.CopyPropertiesFromMaterial(highlightRenderState.m_Material);
						this.m_RenderPlane.GetComponent<Renderer>().sharedMaterial = this.m_Material;
						this.m_RenderPlane.GetComponent<Renderer>().sharedMaterial.SetFloat("_Seed", this.m_seed);
						bool result = this.RenderSilouette();
						if (stateType == ActorStateType.CARD_HISTORY)
						{
							base.transform.localPosition = this.m_HistoryTranslation;
						}
						else
						{
							base.transform.localPosition = highlightRenderState.m_Offset;
						}
						if (this.m_FSM == null)
						{
							if (!this.m_Hide)
							{
								this.m_RenderPlane.GetComponent<Renderer>().enabled = true;
							}
							this.m_VisibilityState = true;
						}
						else
						{
							this.m_BirthTransition = stateType.ToString();
							this.m_SecondBirthTransition = this.m_PreviousState.ToString();
							this.m_IdleTransition = this.m_BirthTransition;
							this.SendDataToPlaymaker();
							this.SendPlaymakerBirthEvent();
						}
						return result;
					}
					this.m_RenderPlane.GetComponent<Renderer>().enabled = false;
					this.m_VisibilityState = false;
					return true;
				}
			}
			if (this.m_highlightType == HighlightStateType.CARD)
			{
				this.m_CurrentState = ActorStateType.CARD_IDLE;
			}
			else if (this.m_highlightType == HighlightStateType.HIGHLIGHT)
			{
				this.m_CurrentState = ActorStateType.HIGHLIGHT_OFF;
			}
			this.m_DeathTransition = this.m_PreviousState.ToString();
			this.SendDataToPlaymaker();
			this.SendPlaymakerDeathEvent();
			this.m_RenderPlane.GetComponent<Renderer>().enabled = false;
			this.m_VisibilityState = false;
			return false;
		}
		if (this.m_FSM == null)
		{
			this.m_RenderPlane.GetComponent<Renderer>().enabled = false;
			this.m_VisibilityState = false;
			return true;
		}
		this.m_DeathTransition = this.m_PreviousState.ToString();
		this.SendDataToPlaymaker();
		this.SendPlaymakerDeathEvent();
		return true;
	}

	// Token: 0x06001D1A RID: 7450 RVA: 0x00088A74 File Offset: 0x00086C74
	protected void UpdateSilouette()
	{
		this.RenderSilouette();
	}

	// Token: 0x06001D1B RID: 7451 RVA: 0x00088A80 File Offset: 0x00086C80
	private bool RenderSilouette()
	{
		this.m_isDirty = false;
		if (this.m_StaticSilouetteTexture != null)
		{
			if (this.m_StaticSilouetteTextureUnique != null)
			{
				Actor actor = SceneUtils.FindComponentInParents<Actor>(base.gameObject);
				if (actor != null && actor.IsElite())
				{
					this.m_RenderPlane.GetComponent<Renderer>().sharedMaterial.mainTexture = this.m_StaticSilouetteTextureUnique;
					this.m_RenderPlane.GetComponent<Renderer>().sharedMaterial.renderQueue = 3000 + this.m_RenderQueue;
					this.m_forceRerender = false;
					return true;
				}
			}
			this.m_RenderPlane.GetComponent<Renderer>().sharedMaterial.mainTexture = this.m_StaticSilouetteTexture;
			this.m_RenderPlane.GetComponent<Renderer>().sharedMaterial.renderQueue = 3000 + this.m_RenderQueue;
			this.m_forceRerender = false;
			return true;
		}
		HighlightRender component = this.m_RenderPlane.GetComponent<HighlightRender>();
		if (component == null)
		{
			Debug.LogError("Unable to find HighlightRender component on m_RenderPlane");
			return false;
		}
		if (component.enabled)
		{
			component.CreateSilhouetteTexture(this.m_forceRerender);
			this.m_RenderPlane.GetComponent<Renderer>().sharedMaterial.mainTexture = component.SilhouetteTexture;
			this.m_RenderPlane.GetComponent<Renderer>().sharedMaterial.renderQueue = 3000 + this.m_RenderQueue;
		}
		this.m_forceRerender = false;
		return true;
	}

	// Token: 0x06001D1C RID: 7452 RVA: 0x00088BE4 File Offset: 0x00086DE4
	private IEnumerator ContinuousSilouetteRender(float renderTime)
	{
		if (GraphicsManager.Get().RenderQualityLevel != GraphicsQuality.Low)
		{
			float endTime = Time.realtimeSinceStartup + renderTime;
			while (Time.realtimeSinceStartup < endTime)
			{
				if (!this.m_RenderPlane.GetComponent<Renderer>().enabled)
				{
					yield break;
				}
				this.m_isDirty = true;
				this.m_forceRerender = true;
				this.RenderSilouette();
				yield return null;
			}
			yield break;
		}
		yield return new WaitForSeconds(renderTime);
		if (!this.m_RenderPlane.GetComponent<Renderer>().enabled)
		{
			yield break;
		}
		this.m_isDirty = true;
		this.m_forceRerender = true;
		this.RenderSilouette();
		yield break;
	}

	// Token: 0x06001D1D RID: 7453 RVA: 0x00088C10 File Offset: 0x00086E10
	private void SendDataToPlaymaker()
	{
		if (this.m_FSM == null)
		{
			return;
		}
		FsmMaterial fsmMaterial = this.m_FSM.FsmVariables.GetFsmMaterial("HighlightMaterial");
		if (fsmMaterial != null)
		{
			fsmMaterial.Value = this.m_RenderPlane.GetComponent<Renderer>().sharedMaterial;
		}
		FsmString fsmString = this.m_FSM.FsmVariables.GetFsmString("CurrentState");
		if (fsmString != null)
		{
			fsmString.Value = this.m_CurrentState.ToString();
		}
		FsmString fsmString2 = this.m_FSM.FsmVariables.GetFsmString("PreviousState");
		if (fsmString2 != null)
		{
			fsmString2.Value = this.m_PreviousState.ToString();
		}
	}

	// Token: 0x06001D1E RID: 7454 RVA: 0x00088CC8 File Offset: 0x00086EC8
	private void SendPlaymakerDeathEvent()
	{
		if (this.m_FSM == null)
		{
			return;
		}
		FsmString fsmString = this.m_FSM.FsmVariables.GetFsmString("DeathTransition");
		if (fsmString != null)
		{
			fsmString.Value = this.m_DeathTransition;
		}
		this.m_FSM.SendEvent("Death");
	}

	// Token: 0x06001D1F RID: 7455 RVA: 0x00088D20 File Offset: 0x00086F20
	private void SendPlaymakerBirthEvent()
	{
		if (this.m_FSM == null)
		{
			return;
		}
		FsmString fsmString = this.m_FSM.FsmVariables.GetFsmString("BirthTransition");
		if (fsmString != null)
		{
			fsmString.Value = this.m_BirthTransition;
		}
		FsmString fsmString2 = this.m_FSM.FsmVariables.GetFsmString("SecondBirthTransition");
		if (fsmString2 != null)
		{
			fsmString2.Value = this.m_SecondBirthTransition;
		}
		FsmString fsmString3 = this.m_FSM.FsmVariables.GetFsmString("IdleTransition");
		if (fsmString3 != null)
		{
			fsmString3.Value = this.m_IdleTransition;
		}
		this.m_FSM.SendEvent("Birth");
	}

	// Token: 0x06001D20 RID: 7456 RVA: 0x00088DC7 File Offset: 0x00086FC7
	public void OnActionFinished()
	{
	}

	// Token: 0x04000F39 RID: 3897
	private const string FSM_BIRTH_STATE = "Birth";

	// Token: 0x04000F3A RID: 3898
	private const string FSM_IDLE_STATE = "Idle";

	// Token: 0x04000F3B RID: 3899
	private const string FSM_DEATH_STATE = "Death";

	// Token: 0x04000F3C RID: 3900
	private const string FSM_BIRTHTRANSITION_STATE = "BirthTransition";

	// Token: 0x04000F3D RID: 3901
	private const string FSM_IDLETRANSITION_STATE = "IdleTransition";

	// Token: 0x04000F3E RID: 3902
	private const string FSM_DEATHTRANSITION_STATE = "DeathTransition";

	// Token: 0x04000F3F RID: 3903
	private readonly string HIGHLIGHT_SHADER_NAME = "Custom/Selection/Highlight";

	// Token: 0x04000F40 RID: 3904
	public GameObject m_RenderPlane;

	// Token: 0x04000F41 RID: 3905
	public HighlightStateType m_highlightType;

	// Token: 0x04000F42 RID: 3906
	public Texture2D m_StaticSilouetteTexture;

	// Token: 0x04000F43 RID: 3907
	public Texture2D m_StaticSilouetteTextureUnique;

	// Token: 0x04000F44 RID: 3908
	public Vector3 m_HistoryTranslation = new Vector3(0f, -0.1f, 0f);

	// Token: 0x04000F45 RID: 3909
	public int m_RenderQueue;

	// Token: 0x04000F46 RID: 3910
	public List<HighlightRenderState> m_HighlightStates;

	// Token: 0x04000F47 RID: 3911
	public ActorStateType m_debugState;

	// Token: 0x04000F48 RID: 3912
	protected ActorStateType m_PreviousState;

	// Token: 0x04000F49 RID: 3913
	protected ActorStateType m_CurrentState;

	// Token: 0x04000F4A RID: 3914
	protected PlayMakerFSM m_FSM;

	// Token: 0x04000F4B RID: 3915
	private string m_sendEvent;

	// Token: 0x04000F4C RID: 3916
	private bool m_isDirty;

	// Token: 0x04000F4D RID: 3917
	private bool m_forceRerender;

	// Token: 0x04000F4E RID: 3918
	private string m_BirthTransition = "None";

	// Token: 0x04000F4F RID: 3919
	private string m_SecondBirthTransition = "None";

	// Token: 0x04000F50 RID: 3920
	private string m_IdleTransition = "None";

	// Token: 0x04000F51 RID: 3921
	private string m_DeathTransition = "None";

	// Token: 0x04000F52 RID: 3922
	private bool m_Hide;

	// Token: 0x04000F53 RID: 3923
	private bool m_VisibilityState;

	// Token: 0x04000F54 RID: 3924
	private float m_seed;

	// Token: 0x04000F55 RID: 3925
	private Material m_Material;
}
