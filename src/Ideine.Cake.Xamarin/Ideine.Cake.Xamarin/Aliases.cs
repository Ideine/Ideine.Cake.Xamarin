using System;
using System.IO;
using System.Xml.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Ideine.Cake.Xamarin
{
	[CakeAliasCategory("Ideine.Xamarin")]
    public static class Aliases
    {
		/// <summary>
		/// Androids the generate keystore.
		/// </summary>
		/// <returns>The generate keystore.</returns>
		/// <param name="context">Context.</param>
		/// <param name="releaseKeyStorePath">Release key store path.</param>
		/// <param name="name">Name.</param>
		/// <param name="aliasName">Alias name.</param>
		/// <param name="storepass">Storepass.</param>
		/// <param name="keypass">Keypass.</param>
		/// <param name="validityDays">Validity days.</param>
		[CakeMethodAlias]
		public static IProcess AndroidGenerateKeystore(this ICakeContext context, FilePath releaseKeyStorePath, string name, string aliasName, string storepass, string keypass, int validityDays)
		{
			var command = "keytool";
			var generatePrivateKeyArgument = "-genkey -v -keystore " + releaseKeyStorePath + " -dname " + name + " -alias " + aliasName + " -storepass " + storepass + " -keypass " + keypass + " -keyalg RSA -keysize 2048 -validity " + validityDays;

			ProcessSettings settings = new ProcessSettings { Arguments = generatePrivateKeyArgument };
			// Get the working directory.
			var workingDirectory = settings.WorkingDirectory ?? context.Environment.WorkingDirectory;
			settings.WorkingDirectory = workingDirectory.MakeAbsolute(context.Environment);

			var process = context.ProcessRunner.Start(command, settings);
			if (process == null)
			{
				throw new CakeException("Could not start process.");
			}

			process.WaitForExit();

			return process;
		}

		/// <summary>
		/// Androids the sign apk.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="releaseKeyStorePath">Release key store path.</param>
		/// <param name="apkFilePath">Apk file path.</param>
		/// <param name="aliasName">Alias name.</param>
		/// <param name="storepass">Storepass.</param>
		/// <param name="keypass">Keypass.</param>
		[CakeMethodAlias]
		public static IProcess AndroidSignApk(this ICakeContext context, FilePath releaseKeyStorePath, FilePath apkFilePath, string aliasName, string storepass, string keypass)
		{
			var command = "jarsigner";
			var signAppArgument = "-verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore " + releaseKeyStorePath.FullPath + " " + apkFilePath.FullPath + " " + aliasName + " -storepass " + storepass + " -keypass " + keypass;

			ProcessSettings settings = new ProcessSettings { Arguments = signAppArgument };
			// Get the working directory.
			var workingDirectory = settings.WorkingDirectory ?? context.Environment.WorkingDirectory;
			settings.WorkingDirectory = workingDirectory.MakeAbsolute(context.Environment);

			var process = context.ProcessRunner.Start(command, settings);
			if (process == null)
			{
				throw new CakeException("Could not start process.");
			}

			process.WaitForExit();

			return process;
		}

		/// <summary>
		/// Androids the sign apk verify.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="apkFilePath">Apk file path.</param>
		[CakeMethodAlias]
		public static IProcess AndroidSignApkVerify(this ICakeContext context, FilePath apkFilePath)
		{
			var command = "jarsigner ";
			var verifySignArgument = "-verify -verbose -certs " + apkFilePath.FullPath;


			ProcessSettings settings = new ProcessSettings { Arguments = verifySignArgument };
			// Get the working directory.
			var workingDirectory = settings.WorkingDirectory ?? context.Environment.WorkingDirectory;
			settings.WorkingDirectory = workingDirectory.MakeAbsolute(context.Environment);

			var process = context.ProcessRunner.Start(command, settings);
			if (process == null)
			{
				throw new CakeException("Could not start process.");
			}

			process.WaitForExit();

			return process;
		}

		/// <summary>
		/// Androids the zip align.
		/// </summary>
		/// <returns>The zip align.</returns>
		/// <param name="context">Context.</param>
		/// <param name="zipAlignCommandPath">Zip align command path.</param>
		/// <param name="alignment">Alignment.</param>
		/// <param name="inputApkPath">Input apk path.</param>
		/// <param name="outputApkPath">Output apk path.</param>
		[CakeMethodAlias]
		public static IProcess AndroidZipAlign(this ICakeContext context, string zipAlignCommandPath, int alignment, string inputApkPath, string outputApkPath)
		{
			var command = zipAlignCommandPath;
			var vzipAlignArgument = "-f -v " + alignment + " " + inputApkPath + " " + outputApkPath;


			ProcessSettings settings = new ProcessSettings { Arguments = vzipAlignArgument };
			// Get the working directory.
			var workingDirectory = settings.WorkingDirectory ?? context.Environment.WorkingDirectory;
			settings.WorkingDirectory = workingDirectory.MakeAbsolute(context.Environment);

			var process = context.ProcessRunner.Start(command, settings);

			if (process == null)
			{
				throw new CakeException("Could not start process.");
			}

			process.WaitForExit();

			return process;
		}

		/// <summary>
		/// Androids the manifest verion name and number.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="androidManifestPath">Android manifest path.</param>
		/// <param name="version">Version.</param>
		/// <param name="versionNumber">Version number.</param>
		[CakeMethodAlias]
		public static void AndroidManifestVersionNameAndNumber(this ICakeContext context, FilePath androidManifestPath, Version version, int versionNumber)
		{
			var path = androidManifestPath.FullPath;
			if (!File.Exists(path))
			{
				throw new CakeException("the AndroidManifest file provided must exist");
			}

			string androidNS = "http://schemas.android.com/apk/res/android";

			XName versionCodeAttributeName = XName.Get("versionCode", androidNS);
			XName versionNameAttributeName = XName.Get("versionName", androidNS);

			XDocument doc = XDocument.Load(path);

			doc.Root.SetAttributeValue(versionNameAttributeName, version);
			doc.Root.SetAttributeValue(versionCodeAttributeName, versionNumber);
			doc.Save(path);

			Console.WriteLine(doc);
		}

		/// <summary>
		/// Androids the name of the manifest verion.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="androidManifestPath">Android manifest path.</param>
		/// <param name="version">Version.</param>
		[CakeMethodAlias]
		public static void AndroidManifestVersionName(this ICakeContext context, FilePath androidManifestPath, Version version)
		{
			var path = androidManifestPath.FullPath;
			if (!File.Exists(path))
			{
				throw new CakeException("the AndroidManifest file provided must exist");
			}

			string androidNS = "http://schemas.android.com/apk/res/android";

			XName versionNameAttributeName = XName.Get("versionName", androidNS);

			XDocument doc = XDocument.Load(path);

			doc.Root.SetAttributeValue(versionNameAttributeName, version);
			doc.Save(path);
			Console.WriteLine(doc);
		}
		
		/// <summary>
		/// Androids the manifest verion number.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="androidManifestPath">Android manifest path.</param>
		/// <param name="versionNumber">Version number.</param>
		[CakeMethodAlias]
		public static void AndroidManifestVersionNumber(this ICakeContext context, FilePath androidManifestPath, int versionNumber)
		{
			var path = androidManifestPath.FullPath;
			if (!File.Exists(path))
			{
				throw new CakeException("the AndroidManifest file provided must exist");
			}

			string androidNS = "http://schemas.android.com/apk/res/android";

			XName versionCodeAttributeName = XName.Get("versionCode", androidNS);

			XDocument doc = XDocument.Load(path);

			doc.Root.SetAttributeValue(versionCodeAttributeName, versionNumber);
			doc.Save(path);

			Console.WriteLine(doc);
		}
	}
}
