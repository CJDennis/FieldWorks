// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2010, SIL International. All Rights Reserved.
// <copyright from='2010' to='2010' company='SIL International'>
//		Copyright (c) 2010, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: FieldWorksTests.cs
// Responsibility: FW Team
// ---------------------------------------------------------------------------------------------
using System;
using NUnit.Framework;
using SIL.FieldWorks.FDO;
using SIL.FieldWorks.Test.TestUtils;
using SIL.Utils;

namespace SIL.FieldWorks
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class FieldWorksTests : BaseTest
	{
		#region GetProjectMatchStatus tests
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetProjectMatchStatus method on FieldWorks with a matching project
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetProjectMatchStatus_Match()
		{
			ReflectionHelper.SetField(typeof(FieldWorks), "s_fSingleProcessMode", false);
			ReflectionHelper.SetField(typeof(FieldWorks), "s_fWaitingForUserOrOtherFw", false);
			ReflectionHelper.SetField(typeof(FieldWorks), "s_projectId",
				new ProjectId(FDOBackendProviderType.kXML, "monkey", null));

			Assert.AreEqual(ProjectMatch.ItsMyProject, ReflectionHelper.GetResult(
				typeof(FieldWorks), "GetProjectMatchStatus",
				new ProjectId(FDOBackendProviderType.kXML, "monkey", null)));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetProjectMatchStatus method on FieldWorks with a different project
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetProjectMatchStatus_NotMatch()
		{
			ReflectionHelper.SetField(typeof(FieldWorks), "s_fSingleProcessMode", false);
			ReflectionHelper.SetField(typeof(FieldWorks), "s_fWaitingForUserOrOtherFw", false);
			ReflectionHelper.SetField(typeof(FieldWorks), "s_projectId",
				new ProjectId(FDOBackendProviderType.kXML, "primate", null));

			Assert.AreEqual(ProjectMatch.ItsNotMyProject, ReflectionHelper.GetResult(
				typeof(FieldWorks), "GetProjectMatchStatus",
				new ProjectId(FDOBackendProviderType.kXML, "monkey", null)));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetProjectMatchStatus method on FieldWorks when the project has yet to
		/// be determined
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetProjectMatchStatus_DontKnow()
		{
			ReflectionHelper.SetField(typeof(FieldWorks), "s_fSingleProcessMode", false);
			ReflectionHelper.SetField(typeof(FieldWorks), "s_fWaitingForUserOrOtherFw", false);
			ReflectionHelper.SetField(typeof(FieldWorks), "s_projectId", null);

			Assert.AreEqual(ProjectMatch.DontKnowYet, ReflectionHelper.GetResult(
				typeof(FieldWorks), "GetProjectMatchStatus",
				new ProjectId(FDOBackendProviderType.kXML, "monkey", null)));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetProjectMatchStatus method on FieldWorks when waiting on another
		/// FieldWorks process
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetProjectMatchStatus_WaitingForFw()
		{
			ReflectionHelper.SetField(typeof(FieldWorks), "s_fSingleProcessMode", false);
			ReflectionHelper.SetField(typeof(FieldWorks), "s_fWaitingForUserOrOtherFw", true);
			ReflectionHelper.SetField(typeof(FieldWorks), "s_projectId",
				new ProjectId(FDOBackendProviderType.kXML, "monkey", null));

			Assert.AreEqual(ProjectMatch.WaitingForUserOrOtherFw, ReflectionHelper.GetResult(
				typeof(FieldWorks), "GetProjectMatchStatus",
				new ProjectId(FDOBackendProviderType.kXML, "monkey", null)));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetProjectMatchStatus method on FieldWorks when in "single process mode"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetProjectMatchStatus_SingleProcessMode()
		{
			ReflectionHelper.SetField(typeof(FieldWorks), "s_fSingleProcessMode", true);
			ReflectionHelper.SetField(typeof(FieldWorks), "s_fWaitingForUserOrOtherFw", true);
			ReflectionHelper.SetField(typeof(FieldWorks), "s_projectId",
				new ProjectId(FDOBackendProviderType.kXML, "monkey", null));

			Assert.AreEqual(ProjectMatch.SingleProcessMode, ReflectionHelper.GetResult(
				typeof(FieldWorks), "GetProjectMatchStatus",
				new ProjectId(FDOBackendProviderType.kXML, "monkey", null)));
		}

		#endregion

		/// <summary/>
		[Test]
		public void EnsureValidLinkedFilesFolderCore_IfUsingDefaultDir_CreatesDirIfNotExist()
		{
			EnsureValidLinkedFilesFolderCore_TestHelper(defaultFolder => {
				var configuredFolder = defaultFolder;
				Assert.That(FileUtils.DirectoryExists(configuredFolder), Is.False, "Unit test not testing what it's supposed to");
				FieldWorks.EnsureValidLinkedFilesFolderCore(configuredFolder, defaultFolder);
				Assert.That(FileUtils.DirectoryExists(configuredFolder), Is.True, "Should have created directory");
			});
		}

		/// <summary/>
		[Test]
		public void EnsureValidLinkedFilesFolderCore_IfUsingDefaultDirAndItExists_DoesntCrashOrAnything()
		{
			EnsureValidLinkedFilesFolderCore_TestHelper(defaultFolder => {
				var configuredFolder = defaultFolder;

				// Make default linked files directory already exist
				FileUtils.EnsureDirectoryExists(defaultFolder);

				Assert.That(FileUtils.DirectoryExists(configuredFolder), Is.True, "Unit test not testing what it's supposed to");
				// Not crash or anything
				FieldWorks.EnsureValidLinkedFilesFolderCore(configuredFolder, defaultFolder);
			});
		}

		/// <summary/>
		[Test]
		public void EnsureValidLinkedFilesFolderCore_NonDefaultLocation_NotCreateNonExistentDir()
		{
			EnsureValidLinkedFilesFolderCore_TestHelper(defaultFolder => {
				var configuredFolder = FileUtils.ChangePathToPlatform("/nondefaultAndNonexistentPath");

				Assert.That(defaultFolder, Is.Not.EqualTo(configuredFolder), "Unit test not set up right");
				Assert.That(FileUtils.DirectoryExists(configuredFolder), Is.False, "Unit test not testing what it's supposed to");

				FieldWorks.EnsureValidLinkedFilesFolderCore(configuredFolder, defaultFolder);
				Assert.That(FileUtils.DirectoryExists(configuredFolder), Is.False, "Shouldn't have just made the nondefault directory");
			});
		}

		/// <summary/>
		[Test]
		public void EnsureValidLinkedFilesFolderCore_NonDefaultLocationAndExists_DoesntCrashOrAnything()
		{
			EnsureValidLinkedFilesFolderCore_TestHelper(defaultFolder => {
				var configuredFolder = FileUtils.ChangePathToPlatform("/nondefaultPath");

				// Make linked files directory already exist
				FileUtils.EnsureDirectoryExists(configuredFolder);

				Assert.That(defaultFolder, Is.Not.EqualTo(configuredFolder), "Unit test not set up right");
				Assert.That(FileUtils.DirectoryExists(configuredFolder), Is.True, "Unit test not testing what it's supposed to");

				// Not crash or anything
				FieldWorks.EnsureValidLinkedFilesFolderCore(configuredFolder, defaultFolder);
			});
		}

		/// <summary>
		/// Unit test helper to set up environment in which to test EnsureValidLinkedFilesFolderCore.
		/// testToExecute takes (string defaultFolder, FdoCache cache).
		/// </summary>
		public void EnsureValidLinkedFilesFolderCore_TestHelper(Action<string> testToExecute)
		{
			var defaultFolder = FileUtils.ChangePathToPlatform("/ProjectDir/LinkedFiles");

			var fileOs = new MockFileOS();
			try
			{
				FileUtils.Manager.SetFileAdapter(fileOs);

				testToExecute(defaultFolder);
			}
			finally
			{
				FileUtils.Manager.Reset();
			}
		}
	}
}
