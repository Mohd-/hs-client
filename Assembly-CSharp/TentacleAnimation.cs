using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F4B RID: 3915
[ExecuteInEditMode]
public class TentacleAnimation : MonoBehaviour
{
	// Token: 0x06007482 RID: 29826 RVA: 0x00224F96 File Offset: 0x00223196
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06007483 RID: 29827 RVA: 0x00224FA0 File Offset: 0x002231A0
	private void Update()
	{
		if (this.m_Bones == null)
		{
			return;
		}
		this.CalculateBoneAngles();
		if (this.m_ControlBones.Count > 0)
		{
			for (int i = 0; i < this.m_Bones.Count; i++)
			{
				this.m_Bones[i].localRotation = this.m_ControlBones[i].localRotation;
				this.m_Bones[i].localPosition = this.m_ControlBones[i].localPosition;
				this.m_Bones[i].localScale = this.m_ControlBones[i].localScale;
				this.m_Bones[i].Rotate(this.m_angleX[i], this.m_angleY[i], this.m_angleZ[i]);
			}
		}
		else
		{
			for (int j = 0; j < this.m_Bones.Count; j++)
			{
				this.m_Bones[j].rotation = this.m_orgRotation[j];
				this.m_Bones[j].Rotate(this.m_angleX[j], this.m_angleY[j], this.m_angleZ[j]);
			}
		}
		this.m_secondaryAnim = this.m_Secondary * 0.01f;
		this.m_smoothing = this.m_Smooth * 0.1f;
	}

	// Token: 0x06007484 RID: 29828 RVA: 0x0022510C File Offset: 0x0022330C
	private void Init()
	{
		if (this.m_Bones == null)
		{
			return;
		}
		if (this.m_IntensityCurve == null || this.m_IntensityCurve.length < 1)
		{
			this.m_IntensityCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
		}
		this.m_randomNumbers = new int[513];
		for (int i = 0; i < 5; i++)
		{
			this.m_randomNumbers[i] = Random.Range(0, 255);
		}
		this.m_randomCount = 4;
		this.m_secondaryAnim = this.m_Secondary * 0.01f;
		this.m_smoothing = 0f;
		this.m_jointStep = 1f / (float)this.m_Bones.Count;
		this.m_timeSeed = (float)Random.Range(1, 100);
		this.m_seedX = (float)Random.Range(1, 10);
		this.m_seedY = (float)Random.Range(1, 10);
		this.m_seedZ = (float)Random.Range(1, 10);
		this.m_intensityValues = new float[this.m_Bones.Count];
		this.m_angleX = new float[this.m_Bones.Count];
		this.m_angleY = new float[this.m_Bones.Count];
		this.m_angleZ = new float[this.m_Bones.Count];
		this.m_velocityX = new float[this.m_Bones.Count];
		this.m_velocityY = new float[this.m_Bones.Count];
		this.m_velocityZ = new float[this.m_Bones.Count];
		this.m_lastX = new float[this.m_Bones.Count];
		this.m_lastY = new float[this.m_Bones.Count];
		this.m_lastZ = new float[this.m_Bones.Count];
		this.InitBones();
		this.isInit = true;
	}

	// Token: 0x06007485 RID: 29829 RVA: 0x002252F8 File Offset: 0x002234F8
	private void InitBones()
	{
		if (this.m_ControlBones.Count < this.m_Bones.Count)
		{
			this.m_orgRotation = new Quaternion[this.m_Bones.Count];
			for (int i = 0; i < this.m_Bones.Count; i++)
			{
				this.m_orgRotation[i] = this.m_Bones[i].rotation;
			}
		}
		else
		{
			for (int j = 0; j < this.m_Bones.Count; j++)
			{
				this.m_Bones[j].rotation = this.m_ControlBones[j].rotation;
			}
		}
		for (int k = 0; k < this.m_Bones.Count; k++)
		{
			this.m_lastX[k] = this.m_Bones[k].eulerAngles.x;
			this.m_lastY[k] = this.m_Bones[k].eulerAngles.y;
			this.m_lastZ[k] = this.m_Bones[k].eulerAngles.z;
			this.m_velocityX[k] = 0f;
			this.m_velocityY[k] = 0f;
			this.m_velocityZ[k] = 0f;
			this.m_intensityValues[k] = this.m_IntensityCurve.Evaluate((float)k * this.m_jointStep);
		}
	}

	// Token: 0x06007486 RID: 29830 RVA: 0x0022547C File Offset: 0x0022367C
	private void CalculateBoneAngles()
	{
		for (int i = 0; i < this.m_Bones.Count; i++)
		{
			this.m_angleX[i] = this.CalculateAngle(i, this.m_lastX, this.m_velocityX, this.m_seedX) * this.m_X_Intensity;
			this.m_angleY[i] = this.CalculateAngle(i, this.m_lastY, this.m_velocityY, this.m_seedY) * this.m_Y_Intensity;
			this.m_angleZ[i] = this.CalculateAngle(i, this.m_lastZ, this.m_velocityZ, this.m_seedZ) * this.m_Z_Intensity;
		}
	}

	// Token: 0x06007487 RID: 29831 RVA: 0x00225520 File Offset: 0x00223720
	private float CalculateAngle(int index, float[] last, float[] velocity, float offset)
	{
		float num = Time.timeSinceLevelLoad * this.m_AnimSpeed + this.m_timeSeed - (float)index * this.m_secondaryAnim;
		float num2 = this.Simplex1D(num + offset);
		float num3 = num2 * this.m_intensityValues[index];
		float num4 = num3 * this.m_MaxAngle;
		float num5 = velocity[index];
		num4 = Mathf.SmoothDamp(last[index], num4, ref num5, this.m_smoothing);
		velocity[index] = num5;
		last[index] = num4;
		return num4;
	}

	// Token: 0x06007488 RID: 29832 RVA: 0x0022558C File Offset: 0x0022378C
	private float Simplex1D(float x)
	{
		int num = (int)Mathf.Floor(x);
		int num2 = num + 1;
		float num3 = x - (float)num;
		float num4 = num3 - 1f;
		float num5 = 1f - num3 * num3;
		float num6 = Mathf.Pow(num5, 4f) * this.Interpolate(this.GetRandomNumber(num & 255), num3);
		float num7 = 1f - num4 * num4;
		float num8 = Mathf.Pow(num7, 4f) * this.Interpolate(this.GetRandomNumber(num2 & 255), num4);
		return (num6 + num8) * 0.395f;
	}

	// Token: 0x06007489 RID: 29833 RVA: 0x0022561C File Offset: 0x0022381C
	private float Interpolate(int h, float x)
	{
		h &= 15;
		float num = 1f + (float)(h & 7);
		if ((h & 8) != 0)
		{
			return -num * x;
		}
		return num * x;
	}

	// Token: 0x0600748A RID: 29834 RVA: 0x0022564C File Offset: 0x0022384C
	private int GetRandomNumber(int index)
	{
		if (index > this.m_randomCount)
		{
			for (int i = this.m_randomCount + 1; i <= index + 1; i++)
			{
				this.m_randomNumbers[i] = Random.Range(0, 255);
			}
			this.m_randomCount = index + 1;
		}
		return this.m_randomNumbers[index];
	}

	// Token: 0x04005ED2 RID: 24274
	private const int RANDOM_INIT_COUNT = 5;

	// Token: 0x04005ED3 RID: 24275
	public float m_MaxAngle = 45f;

	// Token: 0x04005ED4 RID: 24276
	public float m_AnimSpeed = 0.5f;

	// Token: 0x04005ED5 RID: 24277
	public float m_Secondary = 10f;

	// Token: 0x04005ED6 RID: 24278
	public float m_Smooth = 3f;

	// Token: 0x04005ED7 RID: 24279
	[Range(0f, 1f)]
	public float m_X_Intensity = 1f;

	// Token: 0x04005ED8 RID: 24280
	[Range(0f, 1f)]
	public float m_Y_Intensity = 1f;

	// Token: 0x04005ED9 RID: 24281
	[Range(0f, 1f)]
	public float m_Z_Intensity = 1f;

	// Token: 0x04005EDA RID: 24282
	public AnimationCurve m_IntensityCurve;

	// Token: 0x04005EDB RID: 24283
	public List<Transform> m_Bones;

	// Token: 0x04005EDC RID: 24284
	public List<Transform> m_ControlBones;

	// Token: 0x04005EDD RID: 24285
	private float[] m_intensityValues;

	// Token: 0x04005EDE RID: 24286
	private float[] m_angleX;

	// Token: 0x04005EDF RID: 24287
	private float[] m_angleY;

	// Token: 0x04005EE0 RID: 24288
	private float[] m_angleZ;

	// Token: 0x04005EE1 RID: 24289
	private float[] m_velocityX;

	// Token: 0x04005EE2 RID: 24290
	private float[] m_velocityY;

	// Token: 0x04005EE3 RID: 24291
	private float[] m_velocityZ;

	// Token: 0x04005EE4 RID: 24292
	private float[] m_lastX;

	// Token: 0x04005EE5 RID: 24293
	private float[] m_lastY;

	// Token: 0x04005EE6 RID: 24294
	private float[] m_lastZ;

	// Token: 0x04005EE7 RID: 24295
	private Quaternion[] m_orgRotation;

	// Token: 0x04005EE8 RID: 24296
	private float m_jointStep;

	// Token: 0x04005EE9 RID: 24297
	private float m_secondaryAnim = 0.05f;

	// Token: 0x04005EEA RID: 24298
	private float m_smoothing = 0.3f;

	// Token: 0x04005EEB RID: 24299
	private float m_seedX;

	// Token: 0x04005EEC RID: 24300
	private float m_seedY;

	// Token: 0x04005EED RID: 24301
	private float m_seedZ;

	// Token: 0x04005EEE RID: 24302
	private float m_timeSeed;

	// Token: 0x04005EEF RID: 24303
	private int[] m_randomNumbers;

	// Token: 0x04005EF0 RID: 24304
	private int m_randomCount;

	// Token: 0x04005EF1 RID: 24305
	private bool isInit;
}
