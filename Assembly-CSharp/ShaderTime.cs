using System;
using UnityEngine;

// Token: 0x02000F3E RID: 3902
public class ShaderTime : MonoBehaviour
{
	// Token: 0x060073F6 RID: 29686 RVA: 0x0022243B File Offset: 0x0022063B
	private void Awake()
	{
	}

	// Token: 0x060073F7 RID: 29687 RVA: 0x0022243D File Offset: 0x0022063D
	private void Update()
	{
		this.UpdateShaderAnimationTime();
		this.UpdateGyro();
	}

	// Token: 0x060073F8 RID: 29688 RVA: 0x0022244B File Offset: 0x0022064B
	private void OnDestroy()
	{
		Shader.SetGlobalFloat("_ShaderTime", 0f);
	}

	// Token: 0x060073F9 RID: 29689 RVA: 0x0022245C File Offset: 0x0022065C
	private void UpdateShaderAnimationTime()
	{
		this.m_time += Time.deltaTime / 20f;
		if (this.m_time > this.m_maxTime)
		{
			this.m_time -= this.m_maxTime;
			if (this.m_time <= 0f)
			{
				this.m_time = 0.0001f;
			}
		}
		Shader.SetGlobalFloat("_ShaderTime", this.m_time);
	}

	// Token: 0x060073FA RID: 29690 RVA: 0x002224D0 File Offset: 0x002206D0
	private void UpdateGyro()
	{
		Vector4 vector = Input.gyro.gravity;
		Shader.SetGlobalVector("_Gyroscope", vector);
	}

	// Token: 0x04005E8B RID: 24203
	private float m_maxTime = 999f;

	// Token: 0x04005E8C RID: 24204
	private float m_time;
}
