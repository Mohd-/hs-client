using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// Token: 0x02000508 RID: 1288
public class Blizzard
{
	// Token: 0x02000509 RID: 1289
	public class Log
	{
		// Token: 0x06003BD8 RID: 15320 RVA: 0x00121B88 File Offset: 0x0011FD88
		public static void SayToFile(StreamWriter logFile, string format, params object[] args)
		{
			try
			{
				string text = Blizzard.Time.Stamp() + ": " + string.Format(format, args);
				if (logFile != null)
				{
					logFile.WriteLine(text);
					logFile.Flush();
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06003BD9 RID: 15321 RVA: 0x00121BE0 File Offset: 0x0011FDE0
		public static void Write(string message)
		{
			try
			{
				Debug.Log(string.Format("{0}: {1}", Blizzard.Time.Stamp(), message));
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06003BDA RID: 15322 RVA: 0x00121C20 File Offset: 0x0011FE20
		public static void Write(string format, params object[] args)
		{
			Blizzard.Log.Write(string.Format(format, args));
		}

		// Token: 0x06003BDB RID: 15323 RVA: 0x00121C30 File Offset: 0x0011FE30
		public static void Warning(string message)
		{
			try
			{
				Debug.LogWarning(string.Format("{0}: Warning: {1}", Blizzard.Time.Stamp(), message));
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06003BDC RID: 15324 RVA: 0x00121C70 File Offset: 0x0011FE70
		public static void Warning(string format, params object[] args)
		{
			Blizzard.Log.Warning(string.Format(format, args));
		}

		// Token: 0x06003BDD RID: 15325 RVA: 0x00121C80 File Offset: 0x0011FE80
		public static void Error(string message)
		{
			try
			{
				Debug.LogError(string.Format("{0}: Error: {1}", Blizzard.Time.Stamp(), message));
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06003BDE RID: 15326 RVA: 0x00121CC0 File Offset: 0x0011FEC0
		public static void Error(string format, params object[] args)
		{
			Blizzard.Log.Error(string.Format(format, args));
		}
	}

	// Token: 0x02000910 RID: 2320
	public class Crypto
	{
		// Token: 0x02000911 RID: 2321
		public class SHA1
		{
			// Token: 0x06005658 RID: 22104 RVA: 0x0019ED0C File Offset: 0x0019CF0C
			public static string Calc(byte[] bytes, int start, int count)
			{
				System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
				byte[] array = sha.ComputeHash(bytes, start, count);
				StringBuilder stringBuilder = new StringBuilder();
				foreach (byte b in array)
				{
					stringBuilder.Append(b.ToString("x2"));
				}
				return stringBuilder.ToString();
			}

			// Token: 0x06005659 RID: 22105 RVA: 0x0019ED6A File Offset: 0x0019CF6A
			public static string Calc(byte[] bytes)
			{
				return Blizzard.Crypto.SHA1.Calc(bytes, 0, bytes.Length);
			}

			// Token: 0x0600565A RID: 22106 RVA: 0x0019ED78 File Offset: 0x0019CF78
			public static IEnumerator Calc(byte[] bytes, int inputCount, Action<string> hash)
			{
				System.Security.Cryptography.SHA1 hasher = System.Security.Cryptography.SHA1.Create();
				int offset = 0;
				while (bytes.Length - offset >= inputCount)
				{
					offset += hasher.TransformBlock(bytes, offset, inputCount, bytes, offset);
					yield return null;
				}
				hasher.TransformFinalBlock(bytes, offset, bytes.Length - offset);
				StringBuilder sb = new StringBuilder();
				byte[] hashBytes = hasher.Hash;
				foreach (byte b in hashBytes)
				{
					sb.Append(b.ToString("x2"));
				}
				hash.Invoke(sb.ToString());
				yield break;
			}

			// Token: 0x0600565B RID: 22107 RVA: 0x0019EDB8 File Offset: 0x0019CFB8
			public static string Calc(string message)
			{
				byte[] array = new byte[message.Length * 2];
				Buffer.BlockCopy(message.ToCharArray(), 0, array, 0, array.Length);
				return Blizzard.Crypto.SHA1.Calc(array);
			}

			// Token: 0x0600565C RID: 22108 RVA: 0x0019EDEA File Offset: 0x0019CFEA
			public static string Calc(FileInfo path)
			{
				return Blizzard.Crypto.SHA1.Calc(System.IO.File.ReadAllBytes(path.FullName));
			}

			// Token: 0x04003D03 RID: 15619
			public const int Length = 40;

			// Token: 0x04003D04 RID: 15620
			public const string Null = "0000000000000000000000000000000000000000";
		}
	}

	// Token: 0x02000E05 RID: 3589
	public class File
	{
		// Token: 0x06006E33 RID: 28211 RVA: 0x00205890 File Offset: 0x00203A90
		public static void SearchFoldersForExtension(string dirPath, string extension, ref List<FileInfo> results)
		{
			if (Directory.Exists(dirPath))
			{
				Blizzard.File.SearchFoldersForExtension(new DirectoryInfo(dirPath), extension, ref results);
			}
		}

		// Token: 0x06006E34 RID: 28212 RVA: 0x002058AA File Offset: 0x00203AAA
		public static void SearchFoldersForExtensions(string dirPath, string[] extensions, ref List<FileInfo> results)
		{
			if (Directory.Exists(dirPath))
			{
				Blizzard.File.SearchFoldersForExtensions(new DirectoryInfo(dirPath), extensions, ref results);
			}
		}

		// Token: 0x06006E35 RID: 28213 RVA: 0x002058C4 File Offset: 0x00203AC4
		public static void SearchFoldersForExtension(DirectoryInfo dir, string extension, ref List<FileInfo> results)
		{
			foreach (FileInfo fileInfo in dir.GetFiles(string.Format("*.{0}", extension), 1))
			{
				results.Add(fileInfo);
			}
		}

		// Token: 0x06006E36 RID: 28214 RVA: 0x00205904 File Offset: 0x00203B04
		public static void SearchFoldersForExtensions(DirectoryInfo dir, string[] extensions, ref List<FileInfo> results)
		{
			foreach (string extension in extensions)
			{
				Blizzard.File.SearchFoldersForExtension(dir, extension, ref results);
			}
		}

		// Token: 0x06006E37 RID: 28215 RVA: 0x00205934 File Offset: 0x00203B34
		public static bool createFolder(string folder)
		{
			if (string.IsNullOrEmpty(folder))
			{
				return false;
			}
			try
			{
				Directory.CreateDirectory(folder);
				return true;
			}
			catch (UnauthorizedAccessException ex)
			{
				Blizzard.Log.Error("*** UnauthorizedAccessException writing {0}\n*** Exception was: {1}", new object[]
				{
					folder,
					ex.Message
				});
			}
			catch (ArgumentNullException ex2)
			{
				Blizzard.Log.Error("*** ArgumentNullException writing {0}\n*** Exception was: {1}", new object[]
				{
					folder,
					ex2.Message
				});
			}
			catch (ArgumentException ex3)
			{
				Blizzard.Log.Error("*** ArgumentException writing {0}\n*** Exception was: {1}", new object[]
				{
					folder,
					ex3.Message
				});
			}
			catch (PathTooLongException ex4)
			{
				Blizzard.Log.Error("*** PathTooLongException writing {0}\n*** Exception was: {1}", new object[]
				{
					folder,
					ex4.Message
				});
			}
			catch (DirectoryNotFoundException ex5)
			{
				Blizzard.Log.Error("*** DirectoryNotFoundException writing {0}\n*** Exception was: {1}", new object[]
				{
					folder,
					ex5.Message
				});
			}
			catch (IOException ex6)
			{
				Blizzard.Log.Error("*** IOException creating folder '{0}'\n*** Exception was: {1}", new object[]
				{
					folder,
					ex6.Message
				});
				throw new ApplicationException(string.Format("Unable to create folder '{0}': {1}", folder, ex6.Message));
			}
			catch (NotSupportedException ex7)
			{
				Blizzard.Log.Error("*** NotSupportedException writing {0}\n*** Exception was: {1}", new object[]
				{
					folder,
					ex7.Message
				});
			}
			return false;
		}

		// Token: 0x02000E06 RID: 3590
		public class CopyTask
		{
			// Token: 0x06006E38 RID: 28216 RVA: 0x00205AD4 File Offset: 0x00203CD4
			private CopyTask()
			{
			}

			// Token: 0x06006E39 RID: 28217 RVA: 0x00205AE4 File Offset: 0x00203CE4
			public static Blizzard.File.CopyTask FileToFile(string s, string t)
			{
				return new Blizzard.File.CopyTask
				{
					sourcePath = s,
					targetPath = t
				};
			}

			// Token: 0x06006E3A RID: 28218 RVA: 0x00205B08 File Offset: 0x00203D08
			public static Blizzard.File.CopyTask FileToFolder(string s, string t)
			{
				return new Blizzard.File.CopyTask
				{
					sourcePath = s,
					targetFolder = t,
					targetFilename = System.IO.Path.GetFileName(s)
				};
			}

			// Token: 0x06006E3B RID: 28219 RVA: 0x00205B38 File Offset: 0x00203D38
			public static Blizzard.File.CopyTask FolderToFolder(string s, string t)
			{
				return new Blizzard.File.CopyTask
				{
					sourceFolder = s,
					targetFolder = t,
					targetIsFolder = true,
					sourceIsFolder = true
				};
			}

			// Token: 0x1700094D RID: 2381
			// (get) Token: 0x06006E3C RID: 28220 RVA: 0x00205B68 File Offset: 0x00203D68
			// (set) Token: 0x06006E3D RID: 28221 RVA: 0x00205B70 File Offset: 0x00203D70
			public string sourceFilename { get; private set; }

			// Token: 0x1700094E RID: 2382
			// (get) Token: 0x06006E3E RID: 28222 RVA: 0x00205B79 File Offset: 0x00203D79
			// (set) Token: 0x06006E3F RID: 28223 RVA: 0x00205B81 File Offset: 0x00203D81
			public string sourceFolder { get; set; }

			// Token: 0x1700094F RID: 2383
			// (get) Token: 0x06006E40 RID: 28224 RVA: 0x00205B8A File Offset: 0x00203D8A
			// (set) Token: 0x06006E41 RID: 28225 RVA: 0x00205BA9 File Offset: 0x00203DA9
			public string sourcePath
			{
				get
				{
					return System.IO.Path.Combine(this.sourceFolder, this.sourceFilename ?? string.Empty);
				}
				set
				{
					this.sourceFolder = System.IO.Path.GetDirectoryName(value);
					this.sourceFilename = System.IO.Path.GetFileName(value);
				}
			}

			// Token: 0x17000950 RID: 2384
			// (get) Token: 0x06006E42 RID: 28226 RVA: 0x00205BC4 File Offset: 0x00203DC4
			public FileSystemInfo sourceInfo
			{
				get
				{
					return (!this.targetIsFolder) ? new FileInfo(this.targetPath) : new DirectoryInfo(this.targetPath);
				}
			}

			// Token: 0x17000951 RID: 2385
			// (get) Token: 0x06006E43 RID: 28227 RVA: 0x00205BF7 File Offset: 0x00203DF7
			public bool sourceExists
			{
				get
				{
					return (!this.sourceIsFolder) ? System.IO.File.Exists(this.sourcePath) : Directory.Exists(this.sourceFolder);
				}
			}

			// Token: 0x17000952 RID: 2386
			// (get) Token: 0x06006E44 RID: 28228 RVA: 0x00205C1F File Offset: 0x00203E1F
			// (set) Token: 0x06006E45 RID: 28229 RVA: 0x00205C27 File Offset: 0x00203E27
			public string targetFilename { get; private set; }

			// Token: 0x17000953 RID: 2387
			// (get) Token: 0x06006E46 RID: 28230 RVA: 0x00205C30 File Offset: 0x00203E30
			// (set) Token: 0x06006E47 RID: 28231 RVA: 0x00205C38 File Offset: 0x00203E38
			public string targetFolder { get; set; }

			// Token: 0x17000954 RID: 2388
			// (get) Token: 0x06006E48 RID: 28232 RVA: 0x00205C44 File Offset: 0x00203E44
			// (set) Token: 0x06006E49 RID: 28233 RVA: 0x00205C95 File Offset: 0x00203E95
			public string targetPath
			{
				get
				{
					if (this.targetIsFolder)
					{
						return (!this.sourceIsFolder) ? System.IO.Path.Combine(this.targetFolder, this.sourceFilename) : this.targetFolder;
					}
					return System.IO.Path.Combine(this.targetFolder, this.targetFilename);
				}
				set
				{
					this.targetFolder = System.IO.Path.GetDirectoryName(value);
					this.targetFilename = System.IO.Path.GetFileName(value);
				}
			}

			// Token: 0x17000955 RID: 2389
			// (get) Token: 0x06006E4A RID: 28234 RVA: 0x00205CB0 File Offset: 0x00203EB0
			public FileSystemInfo targetInfo
			{
				get
				{
					return (!this.targetIsFolder) ? new FileInfo(this.targetPath) : new DirectoryInfo(this.targetPath);
				}
			}

			// Token: 0x17000956 RID: 2390
			// (get) Token: 0x06006E4B RID: 28235 RVA: 0x00205CE3 File Offset: 0x00203EE3
			public bool targetExists
			{
				get
				{
					return (!this.targetIsFolder) ? System.IO.File.Exists(this.targetPath) : Directory.Exists(this.targetFolder);
				}
			}

			// Token: 0x17000957 RID: 2391
			// (get) Token: 0x06006E4C RID: 28236 RVA: 0x00205D0B File Offset: 0x00203F0B
			public DateTime sourceTime
			{
				get
				{
					return Blizzard.File.CopyTask.writeTime(this.sourceIsFolder, this.sourcePath);
				}
			}

			// Token: 0x17000958 RID: 2392
			// (get) Token: 0x06006E4D RID: 28237 RVA: 0x00205D1E File Offset: 0x00203F1E
			public DateTime targetTime
			{
				get
				{
					return Blizzard.File.CopyTask.writeTime(this.targetIsFolder, this.targetPath);
				}
			}

			// Token: 0x06006E4E RID: 28238 RVA: 0x00205D31 File Offset: 0x00203F31
			private static DateTime writeTime(bool isFolder, string path)
			{
				return (!isFolder) ? System.IO.File.GetLastWriteTime(path) : Directory.GetLastWriteTime(path);
			}

			// Token: 0x06006E4F RID: 28239 RVA: 0x00205D4C File Offset: 0x00203F4C
			public Blizzard.File.CopyTask.Result copy()
			{
				if (!this.sourceExists)
				{
					return Blizzard.File.CopyTask.Result.MissingSource;
				}
				if (this.targetExists)
				{
					if (!this.overwrite)
					{
						return Blizzard.File.CopyTask.Result.CantOverwriteTarget;
					}
					if ((this.targetIsFolder && !FileUtils.SetFolderWritableFlag(this.targetPath, true)) || (!this.targetIsFolder && !FileUtils.SetFileWritableFlag(this.targetPath, true)))
					{
						return Blizzard.File.CopyTask.Result.CantOverwriteTarget;
					}
				}
				Blizzard.File.CopyTask.Result result;
				try
				{
					if (this.sourceIsFolder)
					{
						Blizzard.File.CopyTask.copyRecursive(new DirectoryInfo(this.sourceFolder), this.targetFolder, this.overwrite);
					}
					else
					{
						Blizzard.File.createFolder(this.targetFolder);
						System.IO.File.Copy(this.sourcePath, this.targetPath, this.overwrite);
					}
					result = Blizzard.File.CopyTask.Result.Success;
				}
				catch (UnauthorizedAccessException ex)
				{
					Blizzard.Log.Warning("*** Unauthorized access writing {0} to {1}\n*** Exception was: {2}", new object[]
					{
						this.sourcePath,
						this.targetPath,
						ex.Message
					});
					result = Blizzard.File.CopyTask.Result.UnauthorizedAccess;
				}
				catch (ArgumentNullException)
				{
					result = Blizzard.File.CopyTask.Result.ArgumentNull;
				}
				catch (ArgumentException)
				{
					result = Blizzard.File.CopyTask.Result.Argument;
				}
				catch (PathTooLongException)
				{
					result = Blizzard.File.CopyTask.Result.PathTooLong;
				}
				catch (DirectoryNotFoundException)
				{
					result = Blizzard.File.CopyTask.Result.DirectoryNotFound;
				}
				catch (FileNotFoundException)
				{
					result = Blizzard.File.CopyTask.Result.FileNotFound;
				}
				catch (IOException ex2)
				{
					Blizzard.Log.Warning("*** IO error writing {0} to {1}\n*** Exception was: {2}", new object[]
					{
						this.sourcePath,
						this.targetPath,
						ex2.Message
					});
					result = Blizzard.File.CopyTask.Result.IO;
				}
				catch (NotSupportedException)
				{
					result = Blizzard.File.CopyTask.Result.NotSupported;
				}
				catch (Exception ex3)
				{
					Blizzard.Log.Warning("*** Unknown error writing {0} to {1}: {2}", new object[]
					{
						this.sourcePath,
						this.targetPath,
						ex3.Message
					});
					result = Blizzard.File.CopyTask.Result.Unknown;
				}
				return result;
			}

			// Token: 0x06006E50 RID: 28240 RVA: 0x00205F80 File Offset: 0x00204180
			private static void copyRecursive(DirectoryInfo source, string target, bool overwrite)
			{
				if (!Blizzard.File.createFolder(target))
				{
					return;
				}
				foreach (FileInfo fileInfo in source.GetFiles())
				{
					System.IO.File.Copy(fileInfo.FullName, Blizzard.Path.combine(new object[]
					{
						target,
						fileInfo.Name
					}), overwrite);
				}
				foreach (DirectoryInfo directoryInfo in source.GetDirectories())
				{
					Blizzard.File.CopyTask.copyRecursive(directoryInfo, Blizzard.Path.combine(new object[]
					{
						target,
						directoryInfo.Name
					}) + "/", overwrite);
				}
			}

			// Token: 0x06006E51 RID: 28241 RVA: 0x0020602C File Offset: 0x0020422C
			public override string ToString()
			{
				return string.Format("{0} => {1}", this.sourcePath, this.targetPath);
			}

			// Token: 0x040056DB RID: 22235
			public bool overwrite = true;

			// Token: 0x040056DC RID: 22236
			public bool sourceIsFolder;

			// Token: 0x040056DD RID: 22237
			public bool targetIsFolder;

			// Token: 0x02000E07 RID: 3591
			public enum Result
			{
				// Token: 0x040056E3 RID: 22243
				Unknown,
				// Token: 0x040056E4 RID: 22244
				Success,
				// Token: 0x040056E5 RID: 22245
				MissingSource,
				// Token: 0x040056E6 RID: 22246
				CantOverwriteTarget,
				// Token: 0x040056E7 RID: 22247
				UnauthorizedAccess,
				// Token: 0x040056E8 RID: 22248
				ArgumentNull,
				// Token: 0x040056E9 RID: 22249
				Argument,
				// Token: 0x040056EA RID: 22250
				PathTooLong,
				// Token: 0x040056EB RID: 22251
				DirectoryNotFound,
				// Token: 0x040056EC RID: 22252
				FileNotFound,
				// Token: 0x040056ED RID: 22253
				IO,
				// Token: 0x040056EE RID: 22254
				NotSupported
			}
		}
	}

	// Token: 0x02000E08 RID: 3592
	public static class Path
	{
		// Token: 0x06006E52 RID: 28242 RVA: 0x00206044 File Offset: 0x00204244
		public static string combine(params object[] args)
		{
			string text = string.Empty;
			foreach (object obj in args)
			{
				if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
				{
					text = System.IO.Path.Combine(text, obj.ToString());
				}
			}
			return text;
		}
	}

	// Token: 0x02000E09 RID: 3593
	public class Time
	{
		// Token: 0x06006E54 RID: 28244 RVA: 0x0020609D File Offset: 0x0020429D
		public static string Stamp()
		{
			return Blizzard.Time.Stamp(DateTime.Now);
		}

		// Token: 0x06006E55 RID: 28245 RVA: 0x002060A9 File Offset: 0x002042A9
		public static string Stamp(DateTime then)
		{
			return then.ToString("yyyy-MM-dd HH:mm:ss.fff");
		}

		// Token: 0x06006E56 RID: 28246 RVA: 0x002060B8 File Offset: 0x002042B8
		public static string FormatElapsedTime(TimeSpan elapsedTime)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (elapsedTime.TotalHours >= 1.0)
			{
				stringBuilder.Append(string.Format("{0}h", Convert.ToInt32(elapsedTime.TotalHours)));
			}
			if ((double)elapsedTime.Minutes >= 1.0)
			{
				stringBuilder.Append(string.Format("{0}m", Convert.ToInt32(elapsedTime.Minutes)));
			}
			stringBuilder.Append(elapsedTime.Seconds);
			stringBuilder.Append(".");
			stringBuilder.Append(elapsedTime.Milliseconds);
			stringBuilder.Append("s");
			return stringBuilder.ToString();
		}

		// Token: 0x02000E0A RID: 3594
		public class ScopedTimer : IDisposable
		{
			// Token: 0x06006E57 RID: 28247 RVA: 0x00206174 File Offset: 0x00204374
			private ScopedTimer(string message)
			{
				this.start_ = DateTime.Now;
				this.message_ = message;
			}

			// Token: 0x06006E58 RID: 28248 RVA: 0x0020618E File Offset: 0x0020438E
			public static Blizzard.Time.ScopedTimer Create(string postMessage)
			{
				return new Blizzard.Time.ScopedTimer(postMessage);
			}

			// Token: 0x06006E59 RID: 28249 RVA: 0x00206196 File Offset: 0x00204396
			public static Blizzard.Time.ScopedTimer Create(string preMessage, string postMessage)
			{
				Blizzard.Log.Write(preMessage);
				return Blizzard.Time.ScopedTimer.Create(postMessage);
			}

			// Token: 0x06006E5A RID: 28250 RVA: 0x002061A4 File Offset: 0x002043A4
			public void Dispose()
			{
				Blizzard.Log.Write(string.Format("{0} ({1})", this.message_, Blizzard.Time.FormatElapsedTime(DateTime.Now.Subtract(this.start_))));
			}

			// Token: 0x040056EF RID: 22255
			private readonly string message_;

			// Token: 0x040056F0 RID: 22256
			private readonly DateTime start_;
		}
	}
}
