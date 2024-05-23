using System;
using UnityEngine;

// Token: 0x020001B4 RID: 436
public class PegCursor : MonoBehaviour
{
	// Token: 0x06001CE9 RID: 7401 RVA: 0x00087D89 File Offset: 0x00085F89
	private void Awake()
	{
		PegCursor.s_instance = this;
	}

	// Token: 0x06001CEA RID: 7402 RVA: 0x00087D91 File Offset: 0x00085F91
	private void OnDestroy()
	{
		PegCursor.s_instance = null;
	}

	// Token: 0x06001CEB RID: 7403 RVA: 0x00087D99 File Offset: 0x00085F99
	public static PegCursor Get()
	{
		return PegCursor.s_instance;
	}

	// Token: 0x06001CEC RID: 7404 RVA: 0x00087DA0 File Offset: 0x00085FA0
	public void Show()
	{
		Cursor.visible = true;
	}

	// Token: 0x06001CED RID: 7405 RVA: 0x00087DA8 File Offset: 0x00085FA8
	public void Hide()
	{
		Cursor.visible = false;
	}

	// Token: 0x06001CEE RID: 7406 RVA: 0x00087DB0 File Offset: 0x00085FB0
	public void SetMode(PegCursor.Mode mode)
	{
		bool flag = false;
		if (this.m_currentMode == PegCursor.Mode.WAITING && mode != PegCursor.Mode.STOPWAITING)
		{
			if (mode == PegCursor.Mode.DOWN)
			{
				if (flag)
				{
					Cursor.SetCursor(this.m_cursorWaitingDown64, this.m_cursorWaitingDownHotspot64, 0);
				}
				else
				{
					Cursor.SetCursor(this.m_cursorWaitingDown, this.m_cursorWaitingDownHotspot, 0);
				}
			}
			else if (mode == PegCursor.Mode.UP)
			{
				if (flag)
				{
					Cursor.SetCursor(this.m_cursorWaiting64, this.m_cursorWaitingHotspot64, 0);
				}
				else
				{
					Cursor.SetCursor(this.m_cursorWaiting, this.m_cursorWaitingHotspot, 0);
				}
			}
			return;
		}
		if (this.m_currentMode == PegCursor.Mode.DRAG && mode != PegCursor.Mode.STOPDRAG)
		{
			return;
		}
		this.m_currentMode = mode;
		if (flag)
		{
			switch (mode)
			{
			case PegCursor.Mode.UP:
				Cursor.SetCursor(this.m_cursorUp64, this.m_cursorUpHotspot64, 0);
				break;
			case PegCursor.Mode.DOWN:
				Cursor.SetCursor(this.m_cursorDown64, this.m_cursorDownHotspot64, 0);
				break;
			case PegCursor.Mode.OVER:
				Cursor.SetCursor(this.m_cursorUp64, this.m_cursorUpHotspot64, 0);
				break;
			case PegCursor.Mode.DRAG:
				Cursor.SetCursor(this.m_cursorDrag64, this.m_cursorDragHotspot64, 0);
				break;
			case PegCursor.Mode.STOPDRAG:
			case PegCursor.Mode.STOPWAITING:
				Cursor.SetCursor(this.m_cursorUp64, this.m_cursorUpHotspot64, 0);
				break;
			case PegCursor.Mode.WAITING:
				Cursor.SetCursor(this.m_cursorWaiting64, this.m_cursorWaitingHotspot64, 0);
				break;
			case PegCursor.Mode.LEFTARROW:
				Cursor.SetCursor(this.m_leftArrow64, this.m_leftArrowHotspot64, 0);
				break;
			case PegCursor.Mode.RIGHTARROW:
				Cursor.SetCursor(this.m_rightArrow64, this.m_rightArrowHotspot64, 0);
				break;
			}
		}
		else
		{
			switch (mode)
			{
			case PegCursor.Mode.UP:
				Cursor.SetCursor(this.m_cursorUp, this.m_cursorUpHotspot, 0);
				break;
			case PegCursor.Mode.DOWN:
				Cursor.SetCursor(this.m_cursorDown, this.m_cursorDownHotspot, 0);
				break;
			case PegCursor.Mode.OVER:
				Cursor.SetCursor(this.m_cursorUp, this.m_cursorUpHotspot, 0);
				break;
			case PegCursor.Mode.DRAG:
				Cursor.SetCursor(this.m_cursorDrag, this.m_cursorDragHotspot, 0);
				break;
			case PegCursor.Mode.STOPDRAG:
			case PegCursor.Mode.STOPWAITING:
				Cursor.SetCursor(this.m_cursorUp, this.m_cursorUpHotspot, 0);
				break;
			case PegCursor.Mode.WAITING:
				Cursor.SetCursor(this.m_cursorWaiting, this.m_cursorWaitingHotspot, 0);
				break;
			case PegCursor.Mode.LEFTARROW:
				Cursor.SetCursor(this.m_leftArrow, this.m_leftArrowHotspot, 0);
				break;
			case PegCursor.Mode.RIGHTARROW:
				Cursor.SetCursor(this.m_rightArrow, this.m_rightArrowHotspot, 0);
				break;
			}
		}
	}

	// Token: 0x06001CEF RID: 7407 RVA: 0x00088045 File Offset: 0x00086245
	public PegCursor.Mode GetMode()
	{
		return this.m_currentMode;
	}

	// Token: 0x06001CF0 RID: 7408 RVA: 0x0008804D File Offset: 0x0008624D
	public GameObject GetExplosionPrefab()
	{
		return this.m_explosionPrefab;
	}

	// Token: 0x04000EF0 RID: 3824
	public Texture2D m_cursorUp;

	// Token: 0x04000EF1 RID: 3825
	public Vector2 m_cursorUpHotspot = Vector2.zero;

	// Token: 0x04000EF2 RID: 3826
	public Texture2D m_cursorDown;

	// Token: 0x04000EF3 RID: 3827
	public Vector2 m_cursorDownHotspot = Vector2.zero;

	// Token: 0x04000EF4 RID: 3828
	public Texture2D m_cursorDrag;

	// Token: 0x04000EF5 RID: 3829
	public Vector2 m_cursorDragHotspot = Vector2.zero;

	// Token: 0x04000EF6 RID: 3830
	public Texture2D m_cursorOver;

	// Token: 0x04000EF7 RID: 3831
	public Vector2 m_cursorOverHotspot = Vector2.zero;

	// Token: 0x04000EF8 RID: 3832
	public Texture2D m_cursorWaiting;

	// Token: 0x04000EF9 RID: 3833
	public Vector2 m_cursorWaitingHotspot = Vector2.zero;

	// Token: 0x04000EFA RID: 3834
	public Texture2D m_cursorWaitingDown;

	// Token: 0x04000EFB RID: 3835
	public Vector2 m_cursorWaitingDownHotspot = Vector2.zero;

	// Token: 0x04000EFC RID: 3836
	public Texture2D m_cursorWaitingUp;

	// Token: 0x04000EFD RID: 3837
	public Vector2 m_cursorWaitingUpHotspot = Vector2.zero;

	// Token: 0x04000EFE RID: 3838
	public Texture2D m_leftArrow;

	// Token: 0x04000EFF RID: 3839
	public Vector2 m_leftArrowHotspot = Vector2.zero;

	// Token: 0x04000F00 RID: 3840
	public Texture2D m_rightArrow;

	// Token: 0x04000F01 RID: 3841
	public Vector2 m_rightArrowHotspot = Vector2.zero;

	// Token: 0x04000F02 RID: 3842
	public Texture2D m_cursorUp64;

	// Token: 0x04000F03 RID: 3843
	public Vector2 m_cursorUpHotspot64 = Vector2.zero;

	// Token: 0x04000F04 RID: 3844
	public Texture2D m_cursorDown64;

	// Token: 0x04000F05 RID: 3845
	public Vector2 m_cursorDownHotspot64 = Vector2.zero;

	// Token: 0x04000F06 RID: 3846
	public Texture2D m_cursorDrag64;

	// Token: 0x04000F07 RID: 3847
	public Vector2 m_cursorDragHotspot64 = Vector2.zero;

	// Token: 0x04000F08 RID: 3848
	public Texture2D m_cursorOver64;

	// Token: 0x04000F09 RID: 3849
	public Vector2 m_cursorOverHotspot64 = Vector2.zero;

	// Token: 0x04000F0A RID: 3850
	public Texture2D m_cursorWaiting64;

	// Token: 0x04000F0B RID: 3851
	public Vector2 m_cursorWaitingHotspot64 = Vector2.zero;

	// Token: 0x04000F0C RID: 3852
	public Texture2D m_cursorWaitingDown64;

	// Token: 0x04000F0D RID: 3853
	public Vector2 m_cursorWaitingDownHotspot64 = Vector2.zero;

	// Token: 0x04000F0E RID: 3854
	public Texture2D m_cursorWaitingUp64;

	// Token: 0x04000F0F RID: 3855
	public Vector2 m_cursorWaitingUpHotspot64 = Vector2.zero;

	// Token: 0x04000F10 RID: 3856
	public Texture2D m_leftArrow64;

	// Token: 0x04000F11 RID: 3857
	public Vector2 m_leftArrowHotspot64 = Vector2.zero;

	// Token: 0x04000F12 RID: 3858
	public Texture2D m_rightArrow64;

	// Token: 0x04000F13 RID: 3859
	public Vector2 m_rightArrowHotspot64 = Vector2.zero;

	// Token: 0x04000F14 RID: 3860
	public GameObject m_explosionPrefab;

	// Token: 0x04000F15 RID: 3861
	private Texture2D m_cursorTexture;

	// Token: 0x04000F16 RID: 3862
	private PegCursor.Mode m_currentMode;

	// Token: 0x04000F17 RID: 3863
	private static PegCursor s_instance;

	// Token: 0x020001B5 RID: 437
	public enum Mode
	{
		// Token: 0x04000F19 RID: 3865
		UP,
		// Token: 0x04000F1A RID: 3866
		DOWN,
		// Token: 0x04000F1B RID: 3867
		OVER,
		// Token: 0x04000F1C RID: 3868
		DRAG,
		// Token: 0x04000F1D RID: 3869
		STOPDRAG,
		// Token: 0x04000F1E RID: 3870
		WAITING,
		// Token: 0x04000F1F RID: 3871
		STOPWAITING,
		// Token: 0x04000F20 RID: 3872
		LEFTARROW,
		// Token: 0x04000F21 RID: 3873
		RIGHTARROW,
		// Token: 0x04000F22 RID: 3874
		NONE
	}
}
