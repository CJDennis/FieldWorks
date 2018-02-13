// Copyright (c) 2015-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using SIL.LCModel;
using LanguageExplorer.Controls.DetailControls;
using System.Collections.Generic;

namespace LanguageExplorer.Areas.Lexicon
{
	/// <summary>
	/// Summary description for LexReferenceUnidirectionalView.
	/// </summary>
	internal class LexReferenceUnidirectionalView : VectorReferenceView
	{
		public LexReferenceUnidirectionalView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		protected override VectorReferenceVc CreateVectorReferenceVc()
		{
			return new LexReferenceUnidirectionalVc(m_cache, m_rootFlid, m_displayNameProperty, m_displayWs);
		}

		protected override void Delete()
		{
			Delete(LanguageExplorerResources.ksUndoDeleteRef, LanguageExplorerResources.ksRedoDeleteRef);
		}

		protected override void UpdateTimeStampsIfNeeded(int[] hvos)
		{
#if WANTPORTMULTI
			for (int i = 0; i < hvos.Length; ++i)
			{
				ICmObject cmo = ICmObject.CreateFromDBObject(m_cache, hvos[i]);
				(cmo as ICmObject).UpdateTimestampForVirtualChange();
			}
#endif
		}

		/// <summary>
		/// Put the hidden item back into the list of visible items to make the list that should be stored in the property.
		/// </summary>
		/// <param name="items"></param>
		protected override void AddHiddenItems(List<ICmObject> items)
		{
			var allItems = base.GetVisibleItemList();
			if (allItems.Count != 0)
			{
				items.Insert(0, allItems[0]);
			}
		}

		/// <summary>
		/// In a unidirectional view the FIRST item is hidden.
		/// </summary>
		protected override List<ICmObject> GetVisibleItemList()
		{
			var result = base.GetVisibleItemList();
			result.RemoveAt(0);
			return result;
		}

		#region Component Designer generated code
		/// <summary>
		/// The Superclass handles everything except our Name property.
		/// </summary>
		private void InitializeComponent()
		{
			this.Name = "LexReferenceUnidirectionalView";
		}
		#endregion
	}
}