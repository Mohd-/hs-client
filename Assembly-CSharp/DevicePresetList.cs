using System;
using System.Collections.Generic;

// Token: 0x02000EE4 RID: 3812
public class DevicePresetList : List<DevicePreset>
{
	// Token: 0x0600722A RID: 29226 RVA: 0x00218E20 File Offset: 0x00217020
	public string[] GetNames()
	{
		string[] array = new string[this.Count];
		for (int i = 0; i < this.Count; i++)
		{
			array[i] = this[i].name;
		}
		return array;
	}
}
