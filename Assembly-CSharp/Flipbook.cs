using System;
using UnityEngine;

// Token: 0x020004E0 RID: 1248
public class Flipbook : MonoBehaviour
{
	// Token: 0x06003ABB RID: 15035 RVA: 0x0011BEE4 File Offset: 0x0011A0E4
	private void Start()
	{
		if (this.m_RandomRateRange)
		{
			this.m_flipbookRate = Random.Range(this.m_RandomRateMin, this.m_RandomRateMax);
		}
	}

	// Token: 0x06003ABC RID: 15036 RVA: 0x0011BF14 File Offset: 0x0011A114
	private void Update()
	{
		float num = this.m_flipbookRate;
		if (num == 0f)
		{
			return;
		}
		bool flag = false;
		if (num < 0f)
		{
			num *= -1f;
			flag = true;
		}
		if (this.m_animate)
		{
			if (this.m_flipbookFrame > num)
			{
				int num3;
				if (this.m_flipbookRandom)
				{
					int num2 = 0;
					do
					{
						num3 = Random.Range(0, this.m_flipbookOffsets.Length);
						num2++;
					}
					while (num3 == this.m_flipbookLastOffset && num2 < 100);
					this.m_flipbookLastOffset = num3;
				}
				else
				{
					if (flag)
					{
						this.m_flipbookLastOffset -= Mathf.FloorToInt(this.m_flipbookFrame / num);
						if (this.m_flipbookLastOffset < 0)
						{
							this.m_flipbookLastOffset = Mathf.FloorToInt((float)(this.m_flipbookOffsets.Length - Mathf.Abs(this.m_flipbookLastOffset)));
							if (this.m_flipbookLastOffset < 0)
							{
								this.m_flipbookLastOffset = this.m_flipbookOffsets.Length - 1;
							}
						}
					}
					else if (!this.m_flipbookReverse)
					{
						if (this.m_reverse)
						{
							if (this.m_flipbookLastOffset >= this.m_flipbookOffsets.Length - 1)
							{
								this.m_flipbookLastOffset = this.m_flipbookOffsets.Length - 1;
								this.m_flipbookReverse = true;
							}
							else
							{
								this.m_flipbookLastOffset++;
							}
						}
						else
						{
							this.m_flipbookLastOffset += Mathf.FloorToInt(this.m_flipbookFrame / num);
							if (this.m_flipbookLastOffset >= this.m_flipbookOffsets.Length)
							{
								this.m_flipbookLastOffset = Mathf.FloorToInt((float)(this.m_flipbookLastOffset - this.m_flipbookOffsets.Length));
								if (this.m_flipbookLastOffset >= this.m_flipbookOffsets.Length)
								{
									this.m_flipbookLastOffset = 0;
								}
							}
						}
					}
					else if (this.m_flipbookLastOffset <= 0)
					{
						this.m_flipbookLastOffset = 1;
						this.m_flipbookReverse = false;
					}
					else
					{
						this.m_flipbookLastOffset -= Mathf.FloorToInt(this.m_flipbookFrame / num);
						if (this.m_flipbookLastOffset < 0)
						{
							this.m_flipbookLastOffset = Mathf.FloorToInt((float)(this.m_flipbookOffsets.Length - Mathf.Abs(this.m_flipbookLastOffset)));
						}
						if (this.m_flipbookLastOffset < 0)
						{
							this.m_flipbookLastOffset = this.m_flipbookOffsets.Length - 1;
						}
					}
					num3 = this.m_flipbookLastOffset;
				}
				this.m_flipbookFrame = 0f;
				this.SetIndex(num3);
			}
			this.m_flipbookFrame += Time.deltaTime * 60f;
		}
	}

	// Token: 0x06003ABD RID: 15037 RVA: 0x0011C184 File Offset: 0x0011A384
	public void SetIndex(int i)
	{
		if (i < 0 || i >= this.m_flipbookOffsets.Length)
		{
			if (i < 0)
			{
				this.m_flipbookLastOffset = 0;
			}
			else
			{
				this.m_flipbookLastOffset = this.m_flipbookOffsets.Length;
			}
			Log.Kyle.PrintError("m_flipbookOffsets index out of range: {0}", new object[]
			{
				i
			});
			return;
		}
		base.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", this.m_flipbookOffsets[i]);
	}

	// Token: 0x0400256C RID: 9580
	public float m_flipbookRate = 15f;

	// Token: 0x0400256D RID: 9581
	public bool m_flipbookRandom;

	// Token: 0x0400256E RID: 9582
	public Vector2[] m_flipbookOffsets = new Vector2[]
	{
		new Vector2(0f, 0.5f),
		new Vector2(0.5f, 0.5f),
		new Vector2(0f, 0f),
		new Vector2(0.5f, 0f)
	};

	// Token: 0x0400256F RID: 9583
	public bool m_animate = true;

	// Token: 0x04002570 RID: 9584
	public bool m_reverse = true;

	// Token: 0x04002571 RID: 9585
	public bool m_RandomRateRange;

	// Token: 0x04002572 RID: 9586
	public float m_RandomRateMin;

	// Token: 0x04002573 RID: 9587
	public float m_RandomRateMax;

	// Token: 0x04002574 RID: 9588
	private float m_flipbookFrame;

	// Token: 0x04002575 RID: 9589
	private bool m_flipbookReverse;

	// Token: 0x04002576 RID: 9590
	private int m_flipbookLastOffset;
}
