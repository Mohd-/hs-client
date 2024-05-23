using System;
using UnityEngine;

// Token: 0x0200021B RID: 539
[Serializable]
public class MobileOverrideValue<T>
{
	// Token: 0x060020F4 RID: 8436 RVA: 0x000A1218 File Offset: 0x0009F418
	public MobileOverrideValue()
	{
		this.screens = new ScreenCategory[1];
		this.screens[0] = ScreenCategory.PC;
		this.values = new T[1];
		this.values[0] = default(T);
	}

	// Token: 0x060020F5 RID: 8437 RVA: 0x000A1264 File Offset: 0x0009F464
	public MobileOverrideValue(T defaultValue)
	{
		this.screens = new ScreenCategory[]
		{
			ScreenCategory.PC
		};
		this.values = new T[]
		{
			defaultValue
		};
	}

	// Token: 0x060020F6 RID: 8438 RVA: 0x000A129B File Offset: 0x0009F49B
	public T[] GetValues()
	{
		return this.values;
	}

	// Token: 0x060020F7 RID: 8439 RVA: 0x000A12A4 File Offset: 0x0009F4A4
	public static implicit operator T(MobileOverrideValue<T> val)
	{
		if (val == null)
		{
			return default(T);
		}
		ScreenCategory[] array = val.screens;
		T[] array2 = val.values;
		if (array.Length < 1)
		{
			Debug.LogError("MobileOverrideValue should always have at least one value!");
			return default(T);
		}
		T result = array2[0];
		ScreenCategory screen = PlatformSettings.Screen;
		for (int i = 1; i < array.Length; i++)
		{
			if (screen == array[i])
			{
				result = array2[i];
			}
		}
		return result;
	}

	// Token: 0x04001211 RID: 4625
	public ScreenCategory[] screens;

	// Token: 0x04001212 RID: 4626
	public T[] values;
}
