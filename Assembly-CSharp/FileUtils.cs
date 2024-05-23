using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// Token: 0x020000A1 RID: 161
public class FileUtils
{
	// Token: 0x060007EF RID: 2031 RVA: 0x0001F500 File Offset: 0x0001D700
	public static string MakeSourceAssetPath(DirectoryInfo folder)
	{
		return FileUtils.MakeSourceAssetPath(folder.FullName);
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0001F50D File Offset: 0x0001D70D
	public static string MakeSourceAssetPath(FileInfo fileInfo)
	{
		return FileUtils.MakeSourceAssetPath(fileInfo.FullName);
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x0001F51C File Offset: 0x0001D71C
	public static string MakeSourceAssetPath(string path)
	{
		string text = path.Replace("\\", "/");
		int num = text.IndexOf("/Assets", 5);
		return text.Remove(0, num + 1);
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x0001F553 File Offset: 0x0001D753
	public static string MakeMetaPathFromSourcePath(string path)
	{
		return string.Format("{0}.meta", path);
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x0001F560 File Offset: 0x0001D760
	public static string MakeSourceAssetMetaPath(string path)
	{
		string path2 = FileUtils.MakeSourceAssetPath(path);
		return FileUtils.MakeMetaPathFromSourcePath(path2);
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x0001F57C File Offset: 0x0001D77C
	public static string MakeLocalizedPathFromSourcePath(Locale locale, string enUsPath)
	{
		string text = Path.GetDirectoryName(enUsPath);
		string fileName = Path.GetFileName(enUsPath);
		int num = text.LastIndexOf("/");
		if (num >= 0)
		{
			string text2 = text.Substring(num + 1);
			if (text2.Equals(Localization.DEFAULT_LOCALE_NAME))
			{
				text = text.Remove(num);
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			return string.Format("{0}/{1}", locale, fileName);
		}
		return string.Format("{0}/{1}/{2}", text, locale, fileName);
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0001F5FC File Offset: 0x0001D7FC
	public static Locale? GetLocaleFromSourcePath(string path)
	{
		string directoryName = Path.GetDirectoryName(path);
		int num = directoryName.LastIndexOf("/");
		if (num < 0)
		{
			return default(Locale?);
		}
		string str = directoryName.Substring(num + 1);
		Locale locale;
		try
		{
			locale = EnumUtils.Parse<Locale>(str);
		}
		catch (Exception)
		{
			return default(Locale?);
		}
		return new Locale?(locale);
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0001F674 File Offset: 0x0001D874
	public static Locale? GetForeignLocaleFromSourcePath(string path)
	{
		Locale? localeFromSourcePath = FileUtils.GetLocaleFromSourcePath(path);
		if (localeFromSourcePath == null)
		{
			return default(Locale?);
		}
		if (localeFromSourcePath.Value == Locale.enUS)
		{
			return default(Locale?);
		}
		return localeFromSourcePath;
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0001F6B8 File Offset: 0x0001D8B8
	public static bool IsForeignLocaleSourcePath(string path)
	{
		return FileUtils.GetForeignLocaleFromSourcePath(path) != null;
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0001F6D4 File Offset: 0x0001D8D4
	public static string StripLocaleFromPath(string path)
	{
		string directoryName = Path.GetDirectoryName(path);
		string fileName = Path.GetFileName(path);
		string fileName2 = Path.GetFileName(directoryName);
		if (Localization.IsValidLocaleName(fileName2))
		{
			return string.Format("{0}/{1}", Path.GetDirectoryName(directoryName), fileName);
		}
		return path;
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0001F714 File Offset: 0x0001D914
	public static string GameToSourceAssetPath(string path, string dotExtension = ".prefab")
	{
		return string.Format("{0}{1}", path, dotExtension);
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0001F722 File Offset: 0x0001D922
	public static string GameToSourceAssetName(string folder, string name, string dotExtension = ".prefab")
	{
		return string.Format("{0}/{1}{2}", folder, name, dotExtension);
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0001F734 File Offset: 0x0001D934
	public static string SourceToGameAssetPath(string path)
	{
		int num = path.LastIndexOf('.');
		if (num < 0)
		{
			return path;
		}
		return path.Substring(0, num);
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x0001F75C File Offset: 0x0001D95C
	public static string SourceToGameAssetName(string path)
	{
		int num = path.LastIndexOf('/');
		if (num < 0)
		{
			return path;
		}
		int num2 = path.LastIndexOf('.');
		if (num2 < 0)
		{
			return path;
		}
		return path.Substring(num + 1, num2);
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x0001F798 File Offset: 0x0001D998
	public static string GameAssetPathToName(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return path;
		}
		int num = path.LastIndexOf('/');
		if (num < 0)
		{
			return path;
		}
		return path.Substring(num + 1);
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0001F7CD File Offset: 0x0001D9CD
	public static string GetAssetPath(string fileName)
	{
		return fileName;
	}

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x060007FF RID: 2047 RVA: 0x0001F7D0 File Offset: 0x0001D9D0
	public static string BasePersistentDataPath
	{
		get
		{
			string text = Environment.GetFolderPath(28);
			text = text.Replace('\\', '/');
			return string.Format("{0}/Blizzard/Hearthstone", text);
		}
	}

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x06000800 RID: 2048 RVA: 0x0001F7FB File Offset: 0x0001D9FB
	public static string PublicPersistentDataPath
	{
		get
		{
			return FileUtils.BasePersistentDataPath;
		}
	}

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000801 RID: 2049 RVA: 0x0001F802 File Offset: 0x0001DA02
	public static string InternalPersistentDataPath
	{
		get
		{
			return string.Format("{0}/Dev", FileUtils.BasePersistentDataPath);
		}
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000802 RID: 2050 RVA: 0x0001F814 File Offset: 0x0001DA14
	public static string PersistentDataPath
	{
		get
		{
			string text = null;
			if (ApplicationMgr.IsInternal())
			{
				text = FileUtils.InternalPersistentDataPath;
			}
			else
			{
				text = FileUtils.PublicPersistentDataPath;
			}
			if (!Directory.Exists(text))
			{
				try
				{
					Directory.CreateDirectory(text);
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Format("FileUtils.PersistentDataPath - Error creating {0}. Exception={1}", text, ex.Message));
					Error.AddFatalLoc("GLOBAL_ERROR_ASSET_CREATE_PERSISTENT_DATA_PATH", new object[0]);
				}
			}
			return text;
		}
	}

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x06000803 RID: 2051 RVA: 0x0001F894 File Offset: 0x0001DA94
	public static string CachePath
	{
		get
		{
			string text = string.Format("{0}/Cache", FileUtils.PersistentDataPath);
			if (!Directory.Exists(text))
			{
				try
				{
					Directory.CreateDirectory(text);
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Format("FileUtils.CachePath - Error creating {0}. Exception={1}", text, ex.Message));
				}
			}
			return text;
		}
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x0001F8F8 File Offset: 0x0001DAF8
	public static IntPtr LoadPlugin(string fileName, bool handleError = true)
	{
		string text = string.Empty;
		text = "Hearthstone_Data/Plugins/{0}";
		IntPtr result;
		try
		{
			string text2 = string.Format(text, fileName);
			IntPtr intPtr = DLLUtils.LoadLibrary(text2);
			if (intPtr == IntPtr.Zero && handleError)
			{
				string text3 = Directory.GetCurrentDirectory().Replace("\\", "/");
				string text4 = string.Format("{0}/{1}", text3, text2);
				Error.AddDevFatal("Failed to load plugin from '{0}'", new object[]
				{
					text4
				});
				Error.AddFatalLoc("GLOBAL_ERROR_ASSET_LOAD_FAILED", new object[]
				{
					fileName
				});
			}
			result = intPtr;
		}
		catch (Exception ex)
		{
			Error.AddDevFatal("FileUtils.LoadPlugin() - Exception occurred. message={0} stackTrace={1}", new object[]
			{
				ex.Message,
				ex.StackTrace
			});
			result = IntPtr.Zero;
		}
		return result;
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x0001F9D8 File Offset: 0x0001DBD8
	public static string GetOnDiskCapitalizationForFile(string filePath)
	{
		FileInfo fileInfo = new FileInfo(filePath);
		return FileUtils.GetOnDiskCapitalizationForFile(fileInfo);
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x0001F9F4 File Offset: 0x0001DBF4
	public static string GetOnDiskCapitalizationForDir(string dirPath)
	{
		DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
		return FileUtils.GetOnDiskCapitalizationForDir(dirInfo);
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x0001FA10 File Offset: 0x0001DC10
	public static string GetOnDiskCapitalizationForFile(FileInfo fileInfo)
	{
		DirectoryInfo directory = fileInfo.Directory;
		string name = directory.GetFiles(fileInfo.Name)[0].Name;
		string onDiskCapitalizationForDir = FileUtils.GetOnDiskCapitalizationForDir(directory);
		return Path.Combine(onDiskCapitalizationForDir, name);
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x0001FA48 File Offset: 0x0001DC48
	public static string GetOnDiskCapitalizationForDir(DirectoryInfo dirInfo)
	{
		DirectoryInfo parent = dirInfo.Parent;
		if (parent == null)
		{
			return dirInfo.Name;
		}
		string name = parent.GetDirectories(dirInfo.Name)[0].Name;
		string onDiskCapitalizationForDir = FileUtils.GetOnDiskCapitalizationForDir(parent);
		return Path.Combine(onDiskCapitalizationForDir, name);
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x0001FA8C File Offset: 0x0001DC8C
	public static bool GetLastFolderAndFileFromPath(string path, out string folderName, out string fileName)
	{
		folderName = null;
		fileName = null;
		if (string.IsNullOrEmpty(path))
		{
			return false;
		}
		int num = path.LastIndexOfAny(FileUtils.FOLDER_SEPARATOR_CHARS);
		if (num > 0)
		{
			int num2 = path.LastIndexOfAny(FileUtils.FOLDER_SEPARATOR_CHARS, num - 1);
			int num3 = (num2 >= 0) ? (num2 + 1) : 0;
			int num4 = num - num3;
			folderName = path.Substring(num3, num4);
		}
		if (num < 0)
		{
			fileName = path;
		}
		else if (num < path.Length - 1)
		{
			fileName = path.Substring(num + 1);
		}
		return folderName != null || fileName != null;
	}

	// Token: 0x0600080A RID: 2058 RVA: 0x0001FB28 File Offset: 0x0001DD28
	public static bool SetFolderWritableFlag(string dirPath, bool writable)
	{
		foreach (string path in Directory.GetFiles(dirPath))
		{
			FileUtils.SetFileWritableFlag(path, writable);
		}
		foreach (string dirPath2 in Directory.GetDirectories(dirPath))
		{
			FileUtils.SetFolderWritableFlag(dirPath2, writable);
		}
		return true;
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x0001FB8C File Offset: 0x0001DD8C
	public static bool SetFileWritableFlag(string path, bool setWritable)
	{
		if (!File.Exists(path))
		{
			return false;
		}
		try
		{
			FileAttributes attributes = File.GetAttributes(path);
			FileAttributes fileAttributes = (!setWritable) ? (attributes | 1) : (attributes & -2);
			if (setWritable && Environment.OSVersion.Platform == 6)
			{
				fileAttributes |= 128;
			}
			if (fileAttributes == attributes)
			{
				return true;
			}
			File.SetAttributes(path, fileAttributes);
			FileAttributes attributes2 = File.GetAttributes(path);
			if (attributes2 != fileAttributes)
			{
				return false;
			}
			return true;
		}
		catch (DirectoryNotFoundException)
		{
		}
		catch (FileNotFoundException)
		{
		}
		catch (Exception)
		{
		}
		return false;
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0001FC54 File Offset: 0x0001DE54
	public static string GetMD5(string fileName)
	{
		if (!File.Exists(fileName))
		{
			return string.Empty;
		}
		string result;
		using (FileStream fileStream = File.OpenRead(fileName))
		{
			MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array = md5CryptoServiceProvider.ComputeHash(fileStream);
			result = BitConverter.ToString(array).Replace("-", string.Empty);
		}
		return result;
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x0001FCC0 File Offset: 0x0001DEC0
	public static string GetMD5FromString(string buf)
	{
		MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(buf));
		return BitConverter.ToString(array).Replace("-", string.Empty);
	}

	// Token: 0x0400042F RID: 1071
	public static readonly char[] FOLDER_SEPARATOR_CHARS = new char[]
	{
		'/',
		'\\'
	};
}
