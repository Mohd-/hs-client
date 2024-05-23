using System;
using UnityEngine;

// Token: 0x02000F41 RID: 3905
public class SpriteSheet : MonoBehaviour
{
	// Token: 0x06007404 RID: 29700 RVA: 0x0022279C File Offset: 0x0022099C
	private void Start()
	{
		this.m_NextFrame = Time.timeSinceLevelLoad + 1f / this.m_fps;
		if (base.GetComponent<Renderer>() == null)
		{
			Debug.LogError("SpriteSheet needs a Renderer on: " + base.gameObject.name);
			base.enabled = false;
		}
		this.m_Size = new Vector2(1f / (float)this._uvTieX, 1f / (float)this._uvTieY);
	}

	// Token: 0x06007405 RID: 29701 RVA: 0x00222818 File Offset: 0x00220A18
	private void Update()
	{
		if (this.m_Old_Mode)
		{
			int num = (int)(Time.time * this.m_fps % (float)(this._uvTieX * this._uvTieY));
			if (num != this.m_LastIdx)
			{
				base.GetComponent<Renderer>().material.mainTextureOffset = new Vector2((float)(num % this._uvTieX) * this.m_Size.x, 1f - this.m_Size.y - (float)(num / this._uvTieY) * this.m_Size.y);
				base.GetComponent<Renderer>().material.mainTextureScale = this.m_Size;
				this.m_LastIdx = num;
			}
		}
		else
		{
			if (Time.timeSinceLevelLoad < this.m_NextFrame)
			{
				return;
			}
			this.m_X++;
			if (this.m_X > this._uvTieX - 1)
			{
				this.m_Y++;
				this.m_X = 0;
			}
			if (this.m_Y > this._uvTieY - 1)
			{
				this.m_Y = 0;
			}
			base.GetComponent<Renderer>().material.mainTextureOffset = new Vector2((float)this.m_X * this.m_Size.x, 1f - (float)this.m_Y * this.m_Size.y);
			base.GetComponent<Renderer>().material.mainTextureScale = this.m_Size;
			this.m_NextFrame = Time.timeSinceLevelLoad + 1f / this.m_fps;
		}
	}

	// Token: 0x04005E97 RID: 24215
	public int _uvTieX = 1;

	// Token: 0x04005E98 RID: 24216
	public int _uvTieY = 1;

	// Token: 0x04005E99 RID: 24217
	public float m_fps = 30f;

	// Token: 0x04005E9A RID: 24218
	public bool m_Old_Mode;

	// Token: 0x04005E9B RID: 24219
	private int m_LastIdx = -1;

	// Token: 0x04005E9C RID: 24220
	private Vector2 m_Size = Vector2.one;

	// Token: 0x04005E9D RID: 24221
	private float m_NextFrame;

	// Token: 0x04005E9E RID: 24222
	private int m_X;

	// Token: 0x04005E9F RID: 24223
	private int m_Y;
}
