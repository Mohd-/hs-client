using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000257 RID: 599
public class BoxLightMgr : MonoBehaviour
{
	// Token: 0x06002210 RID: 8720 RVA: 0x000A7678 File Offset: 0x000A5878
	private void Start()
	{
		this.UpdateState();
	}

	// Token: 0x06002211 RID: 8721 RVA: 0x000A7680 File Offset: 0x000A5880
	public BoxLightStateType GetActiveState()
	{
		return this.m_activeStateType;
	}

	// Token: 0x06002212 RID: 8722 RVA: 0x000A7688 File Offset: 0x000A5888
	public void ChangeState(BoxLightStateType stateType)
	{
		if (stateType == BoxLightStateType.INVALID)
		{
			return;
		}
		if (this.m_activeStateType == stateType)
		{
			return;
		}
		this.ChangeStateImpl(stateType);
	}

	// Token: 0x06002213 RID: 8723 RVA: 0x000A76A5 File Offset: 0x000A58A5
	public void SetState(BoxLightStateType stateType)
	{
		if (this.m_activeStateType == stateType)
		{
			return;
		}
		this.m_activeStateType = stateType;
		this.UpdateState();
	}

	// Token: 0x06002214 RID: 8724 RVA: 0x000A76C4 File Offset: 0x000A58C4
	public void UpdateState()
	{
		BoxLightState boxLightState = this.FindState(this.m_activeStateType);
		if (boxLightState == null)
		{
			return;
		}
		boxLightState.m_Spell.ActivateState(SpellStateType.ACTION);
		iTween.Stop(base.gameObject);
		RenderSettings.ambientLight = boxLightState.m_AmbientColor;
		if (boxLightState.m_LightInfos == null)
		{
			return;
		}
		foreach (BoxLightInfo boxLightInfo in boxLightState.m_LightInfos)
		{
			iTween.Stop(boxLightInfo.m_Light.gameObject);
			boxLightInfo.m_Light.color = boxLightInfo.m_Color;
			boxLightInfo.m_Light.intensity = boxLightInfo.m_Intensity;
			LightType type = boxLightInfo.m_Light.type;
			if (type == 2 || type == null)
			{
				boxLightInfo.m_Light.range = boxLightInfo.m_Range;
				if (type == null)
				{
					boxLightInfo.m_Light.spotAngle = boxLightInfo.m_SpotAngle;
				}
			}
		}
	}

	// Token: 0x06002215 RID: 8725 RVA: 0x000A77CC File Offset: 0x000A59CC
	private BoxLightState FindState(BoxLightStateType stateType)
	{
		foreach (BoxLightState boxLightState in this.m_States)
		{
			if (boxLightState.m_Type == stateType)
			{
				return boxLightState;
			}
		}
		return null;
	}

	// Token: 0x06002216 RID: 8726 RVA: 0x000A7838 File Offset: 0x000A5A38
	private void ChangeStateImpl(BoxLightStateType stateType)
	{
		this.m_activeStateType = stateType;
		BoxLightState boxLightState = this.FindState(stateType);
		if (boxLightState == null)
		{
			return;
		}
		iTween.Stop(base.gameObject);
		boxLightState.m_Spell.ActivateState(SpellStateType.BIRTH);
		this.ChangeAmbient(boxLightState);
		if (boxLightState.m_LightInfos != null)
		{
			foreach (BoxLightInfo lightInfo in boxLightState.m_LightInfos)
			{
				this.ChangeLight(boxLightState, lightInfo);
			}
		}
	}

	// Token: 0x06002217 RID: 8727 RVA: 0x000A78D4 File Offset: 0x000A5AD4
	private void ChangeAmbient(BoxLightState state)
	{
		Color prevAmbientColor = RenderSettings.ambientLight;
		Action<object> action = delegate(object amount)
		{
			RenderSettings.ambientLight = Color.Lerp(prevAmbientColor, state.m_AmbientColor, (float)amount);
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			0f,
			"to",
			1f,
			"delay",
			state.m_DelaySec,
			"time",
			state.m_TransitionSec,
			"easetype",
			state.m_TransitionEaseType,
			"onupdate",
			action
		});
		iTween.ValueTo(base.gameObject, args);
	}

	// Token: 0x06002218 RID: 8728 RVA: 0x000A79AC File Offset: 0x000A5BAC
	private void ChangeLight(BoxLightState state, BoxLightInfo lightInfo)
	{
		iTween.Stop(lightInfo.m_Light.gameObject);
		Hashtable args = iTween.Hash(new object[]
		{
			"color",
			lightInfo.m_Color,
			"delay",
			state.m_DelaySec,
			"time",
			state.m_TransitionSec,
			"easetype",
			state.m_TransitionEaseType
		});
		iTween.ColorTo(lightInfo.m_Light.gameObject, args);
		float intensity = lightInfo.m_Light.intensity;
		Action<object> action = delegate(object amount)
		{
			lightInfo.m_Light.intensity = (float)amount;
		};
		Hashtable args2 = iTween.Hash(new object[]
		{
			"from",
			intensity,
			"to",
			lightInfo.m_Intensity,
			"delay",
			state.m_DelaySec,
			"time",
			state.m_TransitionSec,
			"easetype",
			state.m_TransitionEaseType,
			"onupdate",
			action
		});
		iTween.ValueTo(lightInfo.m_Light.gameObject, args2);
		LightType type = lightInfo.m_Light.type;
		if (type == 2 || type == null)
		{
			float range = lightInfo.m_Light.range;
			Action<object> action2 = delegate(object amount)
			{
				lightInfo.m_Light.range = (float)amount;
			};
			Hashtable args3 = iTween.Hash(new object[]
			{
				"from",
				range,
				"to",
				lightInfo.m_Range,
				"delay",
				state.m_DelaySec,
				"time",
				state.m_TransitionSec,
				"easetype",
				state.m_TransitionEaseType,
				"onupdate",
				action2
			});
			iTween.ValueTo(lightInfo.m_Light.gameObject, args3);
			if (type == null)
			{
				float spotAngle = lightInfo.m_Light.spotAngle;
				Action<object> action3 = delegate(object amount)
				{
					lightInfo.m_Light.spotAngle = (float)amount;
				};
				Hashtable args4 = iTween.Hash(new object[]
				{
					"from",
					spotAngle,
					"to",
					lightInfo.m_SpotAngle,
					"delay",
					state.m_DelaySec,
					"time",
					state.m_TransitionSec,
					"easetype",
					state.m_TransitionEaseType,
					"onupdate",
					action3
				});
				iTween.ValueTo(lightInfo.m_Light.gameObject, args4);
			}
		}
	}

	// Token: 0x04001381 RID: 4993
	public List<BoxLightState> m_States;

	// Token: 0x04001382 RID: 4994
	private BoxLightStateType m_activeStateType = BoxLightStateType.DEFAULT;
}
