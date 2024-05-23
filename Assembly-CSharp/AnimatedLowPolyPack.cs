using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200078A RID: 1930
[CustomEditClass]
public class AnimatedLowPolyPack : MonoBehaviour
{
	// Token: 0x17000558 RID: 1368
	// (get) Token: 0x06004CA0 RID: 19616 RVA: 0x0016CB51 File Offset: 0x0016AD51
	// (set) Token: 0x06004CA1 RID: 19617 RVA: 0x0016CB59 File Offset: 0x0016AD59
	public int Column { get; private set; }

	// Token: 0x06004CA2 RID: 19618 RVA: 0x0016CB62 File Offset: 0x0016AD62
	public void Init(int column, Vector3 targetLocalPos, Vector3 offScreenOffset, bool ignoreFullscreenEffects = true, bool changeActivation = true)
	{
		this.m_targetLocalPos = targetLocalPos;
		this.m_targetOffScreenLocalPos = targetLocalPos + offScreenOffset;
		this.m_changeActivation = changeActivation;
		this.Column = column;
		if (ignoreFullscreenEffects)
		{
			SceneUtils.SetLayer(base.gameObject, GameLayer.IgnoreFullScreenEffects);
		}
		this.PositionOffScreen();
	}

	// Token: 0x06004CA3 RID: 19619 RVA: 0x0016CBA4 File Offset: 0x0016ADA4
	public void FlyInImmediate()
	{
		iTween.Stop(base.gameObject);
		base.transform.localEulerAngles = this.m_flyInLocalAngles;
		base.transform.localPosition = this.m_targetLocalPos;
		this.m_state = AnimatedLowPolyPack.State.FLOWN_IN;
		if (this.m_changeActivation)
		{
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x06004CA4 RID: 19620 RVA: 0x0016CBFC File Offset: 0x0016ADFC
	public bool FlyIn(float animTime, float delay)
	{
		if (this.m_state == AnimatedLowPolyPack.State.FLOWN_IN)
		{
			return false;
		}
		if (this.m_state == AnimatedLowPolyPack.State.FLYING_IN)
		{
			return false;
		}
		this.m_state = AnimatedLowPolyPack.State.FLYING_IN;
		if (this.m_changeActivation)
		{
			base.gameObject.SetActive(true);
		}
		base.transform.localEulerAngles = this.m_flyInLocalAngles;
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_targetLocalPos,
			"isLocal",
			true,
			"time",
			animTime,
			"delay",
			delay,
			"easetype",
			iTween.EaseType.easeInCubic,
			"oncomplete",
			"OnFlownIn",
			"oncompletetarget",
			base.gameObject
		});
		iTween.Stop(base.gameObject);
		iTween.MoveTo(base.gameObject, args);
		return true;
	}

	// Token: 0x06004CA5 RID: 19621 RVA: 0x0016CCFC File Offset: 0x0016AEFC
	public void FlyOutImmediate()
	{
		iTween.Stop(base.gameObject);
		base.transform.localEulerAngles = this.m_flyOutLocalAngles;
		base.transform.localPosition = this.m_targetOffScreenLocalPos;
		this.OnHidden();
	}

	// Token: 0x06004CA6 RID: 19622 RVA: 0x0016CD3C File Offset: 0x0016AF3C
	public bool FlyOut(float animTime, float delay)
	{
		if (this.m_state == AnimatedLowPolyPack.State.HIDDEN)
		{
			return false;
		}
		if (this.m_state == AnimatedLowPolyPack.State.FLYING_OUT)
		{
			return false;
		}
		this.m_state = AnimatedLowPolyPack.State.FLYING_OUT;
		base.transform.localEulerAngles = this.m_flyOutLocalAngles;
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_targetOffScreenLocalPos,
			"isLocal",
			true,
			"time",
			animTime,
			"delay",
			delay,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"OnHidden",
			"oncompletetarget",
			base.gameObject
		});
		iTween.Stop(base.gameObject);
		iTween.MoveTo(base.gameObject, args);
		if (!string.IsNullOrEmpty(this.m_FlyOutSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_FlyOutSound));
		}
		return true;
	}

	// Token: 0x06004CA7 RID: 19623 RVA: 0x0016CE48 File Offset: 0x0016B048
	public void SetFlyingLocalRotations(Vector3 flyInLocalAngles, Vector3 flyOutLocalAngles)
	{
		this.m_flyInLocalAngles = flyInLocalAngles;
		this.m_flyOutLocalAngles = flyOutLocalAngles;
	}

	// Token: 0x06004CA8 RID: 19624 RVA: 0x0016CE58 File Offset: 0x0016B058
	public AnimatedLowPolyPack.State GetState()
	{
		return this.m_state;
	}

	// Token: 0x06004CA9 RID: 19625 RVA: 0x0016CE60 File Offset: 0x0016B060
	public void Hide()
	{
		this.OnHidden();
	}

	// Token: 0x06004CAA RID: 19626 RVA: 0x0016CE68 File Offset: 0x0016B068
	private void OnHidden()
	{
		this.m_state = AnimatedLowPolyPack.State.HIDDEN;
		if (this.m_changeActivation)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06004CAB RID: 19627 RVA: 0x0016CE88 File Offset: 0x0016B088
	private void OnFlownIn()
	{
		this.m_DustParticle.Play();
		this.m_state = AnimatedLowPolyPack.State.FLOWN_IN;
		iTween.PunchPosition(base.gameObject, this.PUNCH_POSITION_AMOUNT, this.PUNCH_POSITION_TIME);
		if (!string.IsNullOrEmpty(this.m_FlyInSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_FlyInSound));
		}
	}

	// Token: 0x06004CAC RID: 19628 RVA: 0x0016CEE4 File Offset: 0x0016B0E4
	private void PositionOffScreen()
	{
		iTween.Stop(base.gameObject);
		base.transform.localPosition = this.m_targetOffScreenLocalPos;
		this.OnHidden();
	}

	// Token: 0x04003373 RID: 13171
	public Vector3 PUNCH_POSITION_AMOUNT = new Vector3(0f, 5f, 0f);

	// Token: 0x04003374 RID: 13172
	public float PUNCH_POSITION_TIME = 0.25f;

	// Token: 0x04003375 RID: 13173
	public ParticleSystem m_DustParticle;

	// Token: 0x04003376 RID: 13174
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_FlyOutSound = "Assets/Game/Sounds/Pack Purchasing/purchase_pack_lift_whoosh_1";

	// Token: 0x04003377 RID: 13175
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_FlyInSound = "Assets/Game/Sounds/Pack Purchasing/purchase_pack_drop_impact_1";

	// Token: 0x04003378 RID: 13176
	private Vector3 m_flyInLocalAngles = Vector3.zero;

	// Token: 0x04003379 RID: 13177
	private Vector3 m_flyOutLocalAngles = Vector3.zero;

	// Token: 0x0400337A RID: 13178
	private Vector3 m_targetOffScreenLocalPos = Vector3.zero;

	// Token: 0x0400337B RID: 13179
	private Vector3 m_targetLocalPos = Vector3.zero;

	// Token: 0x0400337C RID: 13180
	private AnimatedLowPolyPack.State m_state;

	// Token: 0x0400337D RID: 13181
	private bool m_changeActivation = true;

	// Token: 0x020007C5 RID: 1989
	public enum State
	{
		// Token: 0x040034F2 RID: 13554
		UNKNOWN,
		// Token: 0x040034F3 RID: 13555
		FLOWN_IN,
		// Token: 0x040034F4 RID: 13556
		FLYING_IN,
		// Token: 0x040034F5 RID: 13557
		FLYING_OUT,
		// Token: 0x040034F6 RID: 13558
		HIDDEN
	}
}
