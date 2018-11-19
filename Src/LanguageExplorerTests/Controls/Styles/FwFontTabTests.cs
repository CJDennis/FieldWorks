// Copyright (c) 2007-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using LanguageExplorer.Controls.Styles;
using NUnit.Framework;
using SIL.FieldWorks.FwCoreDlgs.Controls;
using SIL.LCModel;
using SIL.LCModel.Core.KernelInterfaces;
using SIL.LCModel.Utils;

namespace LanguageExplorerTests.Controls.Styles
{
	/// <summary>
	/// Tests for the font control tab.
	/// </summary>
	[TestFixture]
	public class FwFontTabTests : MemoryOnlyBackendProviderRestoredForEachTestTestBase
	{
		/// <summary>Field Works font tab control for testing.</summary>
		FwFontTab m_fontTab;

		/// <summary>
		/// Initialize for tests.
		/// </summary>
		public override void TestSetup()
		{
			base.TestSetup();

			m_fontTab = new FwFontTab();
			m_fontTab.FillFontInfo(Cache);
		}

		/// <summary>
		/// Tear down after tests.
		/// </summary>
		public override void TestTearDown()
		{
			try
			{
				m_fontTab.Dispose();
				m_fontTab = null;
			}
			catch (Exception err)
			{
				throw new Exception($"Error in running {GetType().Name} TestTearDown method.", err);
			}
			finally
			{
				base.TestTearDown();
			}
		}

		/// <summary>
		/// Test selecting a user-defined character style when it is based on a style with an
		/// unspecified font and the user-defined character style specifies it.
		/// </summary>
		[Test]
		public void UserDefinedCharacterStyle_ExplicitFontName()
		{
			// Create a style with an unspecified font name.
			var charStyle = Cache.ServiceLocator.GetInstance<IStStyleFactory>().Create();
			Cache.LangProject.StylesOC.Add(charStyle);
			charStyle.Context = ContextValues.Text;
			charStyle.Function = FunctionValues.Prose;
			charStyle.Structure = StructureValues.Body;
			charStyle.Type = StyleType.kstCharacter;
			var basedOn = new StyleInfo(charStyle);

			// Create a user-defined character style inherited from the previously-created character
			// style, but this style has a font name specified.
			var charStyleInfo = new StyleInfo("New Char Style", basedOn, StyleType.kstCharacter, Cache);
			m_fontTab.UpdateForStyle(charStyleInfo);

			// Select a font name for the style (which will call the event handler
			var cboFontNames = ReflectionHelper.GetField(m_fontTab, "m_cboFontNames") as FwInheritablePropComboBox;
			Assert.IsNotNull(cboFontNames);
			cboFontNames.AdjustedSelectedIndex = 1;
			// Make sure we successfully set the font for this user-defined character style.
			Assert.IsTrue(charStyleInfo.FontInfoForWs(-1).m_fontName.IsExplicit);
			Assert.AreEqual("<default font>", charStyleInfo.FontInfoForWs(-1).m_fontName.Value, "The font should have been set to the default font.");
		}

		/// <summary>
		/// Make sure font names are alphabetically sorted in combobox.
		/// Related to FWNX-273: Fonts not in alphabetical order
		/// </summary>
		[Test]
		public void FillFontNames_IsAlphabeticallySorted()
		{
			const int firstActualFontNameInListLocation = 4;
			m_fontTab.FillFontNames(true);
			var fontNames = m_fontTab.FontNamesComboBox.Items;
			for (var i = firstActualFontNameInListLocation; i + 1 < fontNames.Count; i++)
			{
				// Check that each font in the list is alphabetically before the next font in the list
				Assert.LessOrEqual(fontNames[i] as string, fontNames[i + 1] as string, "Font names not alphabetically sorted.");
			}
		}
	}
}