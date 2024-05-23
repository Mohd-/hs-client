using System;
using System.Runtime.InteropServices;

// Token: 0x020002DC RID: 732
public class DLLUtils
{
	// Token: 0x06002676 RID: 9846
	[DllImport("kernel32.dll")]
	public static extern IntPtr LoadLibrary(string filename);

	// Token: 0x06002677 RID: 9847
	[DllImport("kernel32.dll")]
	public static extern IntPtr GetProcAddress(IntPtr module, string funcName);

	// Token: 0x06002678 RID: 9848
	[DllImport("kernel32.dll")]
	public static extern bool FreeLibrary(IntPtr module);
}
