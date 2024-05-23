using System;
using UnityEngine;

// Token: 0x02000677 RID: 1655
public class CameraShakeMgr : MonoBehaviour
{
	// Token: 0x06004689 RID: 18057 RVA: 0x0015319D File Offset: 0x0015139D
	private void Update()
	{
		if (this.m_progressSec >= this.m_durationSec && !this.IsHolding())
		{
			this.DestroyShake();
			return;
		}
		this.UpdateShake();
	}

	// Token: 0x0600468A RID: 18058 RVA: 0x001531C8 File Offset: 0x001513C8
	public static void Shake(Camera camera, Vector3 amount, AnimationCurve intensityCurve, float? holdAtTime = null)
	{
		if (!camera)
		{
			return;
		}
		CameraShakeMgr cameraShakeMgr = camera.GetComponent<CameraShakeMgr>();
		if (cameraShakeMgr)
		{
			if (CameraShakeMgr.DoesCurveHaveZeroTime(intensityCurve))
			{
				cameraShakeMgr.DestroyShake();
				return;
			}
		}
		else
		{
			if (CameraShakeMgr.DoesCurveHaveZeroTime(intensityCurve))
			{
				return;
			}
			cameraShakeMgr = camera.gameObject.AddComponent<CameraShakeMgr>();
		}
		cameraShakeMgr.StartShake(amount, intensityCurve, holdAtTime);
	}

	// Token: 0x0600468B RID: 18059 RVA: 0x0015322C File Offset: 0x0015142C
	public static void Shake(Camera camera, Vector3 amount, float time)
	{
		AnimationCurve intensityCurve = AnimationCurve.Linear(0f, 1f, time, 0f);
		CameraShakeMgr.Shake(camera, amount, intensityCurve, default(float?));
	}

	// Token: 0x0600468C RID: 18060 RVA: 0x00153260 File Offset: 0x00151460
	public static void Stop(Camera camera, float time = 0f)
	{
		if (!camera)
		{
			return;
		}
		CameraShakeMgr component = camera.GetComponent<CameraShakeMgr>();
		if (!component)
		{
			return;
		}
		if (time <= 0f)
		{
			component.DestroyShake();
			return;
		}
		float num = component.ComputeIntensity();
		AnimationCurve intensityCurve = AnimationCurve.Linear(0f, num, time, 0f);
		component.StartShake(component.m_amount, intensityCurve, default(float?));
	}

	// Token: 0x0600468D RID: 18061 RVA: 0x001532D0 File Offset: 0x001514D0
	public static bool IsShaking(Camera camera)
	{
		if (!camera)
		{
			return false;
		}
		CameraShakeMgr component = camera.GetComponent<CameraShakeMgr>();
		return component;
	}

	// Token: 0x0600468E RID: 18062 RVA: 0x00153300 File Offset: 0x00151500
	private static bool DoesCurveHaveZeroTime(AnimationCurve intensityCurve)
	{
		if (intensityCurve == null)
		{
			return true;
		}
		if (intensityCurve.length == 0)
		{
			return true;
		}
		Keyframe keyframe = intensityCurve.keys[intensityCurve.length - 1];
		return keyframe.time <= 0f;
	}

	// Token: 0x0600468F RID: 18063 RVA: 0x00153350 File Offset: 0x00151550
	private void StartShake(Vector3 amount, AnimationCurve intensityCurve, float? holdAtSec = null)
	{
		this.m_amount = amount;
		this.m_intensityCurve = intensityCurve;
		this.m_holdAtSec = holdAtSec;
		if (!this.m_started)
		{
			this.m_started = true;
			this.m_initialPos = base.transform.position;
		}
		this.m_progressSec = 0f;
		Keyframe keyframe = intensityCurve.keys[intensityCurve.length - 1];
		this.m_durationSec = keyframe.time;
	}

	// Token: 0x06004690 RID: 18064 RVA: 0x001533C6 File Offset: 0x001515C6
	private void DestroyShake()
	{
		base.transform.position = this.m_initialPos;
		Object.Destroy(this);
	}

	// Token: 0x06004691 RID: 18065 RVA: 0x001533E0 File Offset: 0x001515E0
	private void UpdateShake()
	{
		float num = this.ComputeIntensity();
		Vector3 vector = default(Vector3);
		vector.x = Random.Range(-this.m_amount.x * num, this.m_amount.x * num);
		vector.y = Random.Range(-this.m_amount.y * num, this.m_amount.y * num);
		vector.z = Random.Range(-this.m_amount.z * num, this.m_amount.z * num);
		base.transform.position = this.m_initialPos + vector;
		if (this.IsHolding())
		{
			return;
		}
		this.m_progressSec = Mathf.Min(this.m_progressSec + Time.deltaTime, this.m_durationSec);
	}

	// Token: 0x06004692 RID: 18066 RVA: 0x001534B1 File Offset: 0x001516B1
	private float ComputeIntensity()
	{
		return this.m_intensityCurve.Evaluate(this.m_progressSec);
	}

	// Token: 0x06004693 RID: 18067 RVA: 0x001534C4 File Offset: 0x001516C4
	private bool IsHolding()
	{
		float? holdAtSec = this.m_holdAtSec;
		if (holdAtSec == null)
		{
			return false;
		}
		float? holdAtSec2 = this.m_holdAtSec;
		return holdAtSec2 != null && this.m_progressSec >= holdAtSec2.Value;
	}

	// Token: 0x04002D9F RID: 11679
	private Vector3 m_amount;

	// Token: 0x04002DA0 RID: 11680
	private AnimationCurve m_intensityCurve;

	// Token: 0x04002DA1 RID: 11681
	private float? m_holdAtSec;

	// Token: 0x04002DA2 RID: 11682
	private bool m_started;

	// Token: 0x04002DA3 RID: 11683
	private Vector3 m_initialPos;

	// Token: 0x04002DA4 RID: 11684
	private float m_progressSec;

	// Token: 0x04002DA5 RID: 11685
	private float m_durationSec;
}
