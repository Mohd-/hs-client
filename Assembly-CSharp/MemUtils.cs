using System;
using System.Runtime.InteropServices;
using System.Text;

// Token: 0x02000EDE RID: 3806
public static class MemUtils
{
	// Token: 0x0600720C RID: 29196 RVA: 0x00218A04 File Offset: 0x00216C04
	public static void FreePtr(IntPtr ptr)
	{
		if (ptr == IntPtr.Zero)
		{
			return;
		}
		Marshal.FreeHGlobal(ptr);
	}

	// Token: 0x0600720D RID: 29197 RVA: 0x00218A20 File Offset: 0x00216C20
	public static byte[] PtrToBytes(IntPtr ptr, int size)
	{
		if (ptr == IntPtr.Zero)
		{
			return null;
		}
		if (size == 0)
		{
			return null;
		}
		byte[] array = new byte[size];
		Marshal.Copy(ptr, array, 0, size);
		return array;
	}

	// Token: 0x0600720E RID: 29198 RVA: 0x00218A58 File Offset: 0x00216C58
	public static IntPtr PtrFromBytes(byte[] bytes)
	{
		return MemUtils.PtrFromBytes(bytes, 0);
	}

	// Token: 0x0600720F RID: 29199 RVA: 0x00218A64 File Offset: 0x00216C64
	public static IntPtr PtrFromBytes(byte[] bytes, int offset)
	{
		if (bytes == null)
		{
			return IntPtr.Zero;
		}
		int len = bytes.Length - offset;
		return MemUtils.PtrFromBytes(bytes, offset, len);
	}

	// Token: 0x06007210 RID: 29200 RVA: 0x00218A8C File Offset: 0x00216C8C
	public static IntPtr PtrFromBytes(byte[] bytes, int offset, int len)
	{
		if (bytes == null)
		{
			return IntPtr.Zero;
		}
		if (len <= 0)
		{
			return IntPtr.Zero;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(len);
		Marshal.Copy(bytes, offset, intPtr, len);
		return intPtr;
	}

	// Token: 0x06007211 RID: 29201 RVA: 0x00218AC4 File Offset: 0x00216CC4
	public static byte[] StructToBytes<T>(T t)
	{
		int num = Marshal.SizeOf(typeof(T));
		byte[] array = new byte[num];
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.StructureToPtr(t, intPtr, true);
		Marshal.Copy(intPtr, array, 0, num);
		Marshal.FreeHGlobal(intPtr);
		return array;
	}

	// Token: 0x06007212 RID: 29202 RVA: 0x00218B0C File Offset: 0x00216D0C
	public static T StructFromBytes<T>(byte[] bytes)
	{
		return MemUtils.StructFromBytes<T>(bytes, 0);
	}

	// Token: 0x06007213 RID: 29203 RVA: 0x00218B18 File Offset: 0x00216D18
	public static T StructFromBytes<T>(byte[] bytes, int offset)
	{
		Type typeFromHandle = typeof(T);
		int num = Marshal.SizeOf(typeFromHandle);
		if (bytes == null)
		{
			return default(T);
		}
		if (bytes.Length - offset < num)
		{
			return default(T);
		}
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.Copy(bytes, offset, intPtr, num);
		T result = (T)((object)Marshal.PtrToStructure(intPtr, typeFromHandle));
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	// Token: 0x06007214 RID: 29204 RVA: 0x00218B84 File Offset: 0x00216D84
	public static IntPtr Utf8PtrFromString(string managedString)
	{
		if (managedString == null)
		{
			return IntPtr.Zero;
		}
		int num = 1 + Encoding.UTF8.GetByteCount(managedString);
		byte[] array = new byte[num];
		Encoding.UTF8.GetBytes(managedString, 0, managedString.Length, array, 0);
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.Copy(array, 0, intPtr, num);
		return intPtr;
	}

	// Token: 0x06007215 RID: 29205 RVA: 0x00218BD8 File Offset: 0x00216DD8
	public static string StringFromUtf8Ptr(IntPtr ptr)
	{
		int num;
		return MemUtils.StringFromUtf8Ptr(ptr, out num);
	}

	// Token: 0x06007216 RID: 29206 RVA: 0x00218BF0 File Offset: 0x00216DF0
	public static string StringFromUtf8Ptr(IntPtr ptr, out int len)
	{
		len = 0;
		if (ptr == IntPtr.Zero)
		{
			return null;
		}
		len = MemUtils.StringPtrByteLen(ptr);
		if (len == 0)
		{
			return null;
		}
		byte[] array = new byte[len];
		Marshal.Copy(ptr, array, 0, len);
		return Encoding.UTF8.GetString(array);
	}

	// Token: 0x06007217 RID: 29207 RVA: 0x00218C40 File Offset: 0x00216E40
	public static int StringPtrByteLen(IntPtr ptr)
	{
		if (ptr == IntPtr.Zero)
		{
			return 0;
		}
		int num = 0;
		while (Marshal.ReadByte(ptr, num) != 0)
		{
			num++;
		}
		return num;
	}
}
