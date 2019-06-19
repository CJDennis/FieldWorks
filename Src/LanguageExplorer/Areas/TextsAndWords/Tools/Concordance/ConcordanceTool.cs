// Copyright (c) 2015-2019 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using LanguageExplorer.Areas.TextsAndWords.Interlinear;
using LanguageExplorer.Controls;
using SIL.Code;
using SIL.FieldWorks.Common.FwUtils;
using SIL.FieldWorks.Resources;
using SIL.LCModel;
using SIL.LCModel.Application;

namespace LanguageExplorer.Areas.TextsAndWords.Tools.Concordance
{
	/// <summary>
	/// ITool implementation for the "concordance" tool in the "textsWords" area.
	/// </summary>
	[Export(AreaServices.TextAndWordsAreaMachineName, typeof(ITool))]
	internal sealed class ConcordanceTool : ITool
	{
		internal const string OccurrencesOfSelectedUnit = "OccurrencesOfSelectedUnit";
		private ConcordanceToolMenuHelper _toolMenuHelper;
		private MultiPane _concordanceContainer;
		private ConcordanceControl _concordanceControl;
		private RecordBrowseView _recordBrowseView;
		private IRecordList _recordList;
		private InterlinMasterNoTitleBar _interlinMasterNoTitleBar;
		[Import(AreaServices.TextAndWordsAreaMachineName)]
		private IArea _area;

		#region Implementation of IMajorFlexComponent

		/// <summary>
		/// Deactivate the component.
		/// </summary>
		/// <remarks>
		/// This is called on the outgoing component, when the user switches to a component.
		/// </remarks>
		public void Deactivate(MajorFlexComponentParameters majorFlexComponentParameters)
		{
			// This will also remove any event handlers set up by the tool's UserControl instances that may have registered event handlers.
			majorFlexComponentParameters.UiWidgetController.RemoveToolHandlers();
			MultiPaneFactory.RemoveFromParentAndDispose(majorFlexComponentParameters.MainCollapsingSplitContainer, ref _concordanceContainer);

			// Dispose after the main UI stuff.
			_toolMenuHelper.Dispose();

			_concordanceControl = null;
			_recordBrowseView = null;
			_interlinMasterNoTitleBar = null;
			_toolMenuHelper = null;
		}

		/// <summary>
		/// Activate the component.
		/// </summary>
		/// <remarks>
		/// This is called on the component that is becoming active.
		/// </remarks>
		public void Activate(MajorFlexComponentParameters majorFlexComponentParameters)
		{
			if (_recordList == null)
			{
				_recordList = majorFlexComponentParameters.FlexComponentParameters.PropertyTable.GetValue<IRecordListRepositoryForTools>(LanguageExplorerConstants.RecordListRepository).GetRecordList(OccurrencesOfSelectedUnit, majorFlexComponentParameters.StatusBar, FactoryMethod);
			}
			_toolMenuHelper = new ConcordanceToolMenuHelper(majorFlexComponentParameters, this);
			var mainConcordanceContainerParameters = new MultiPaneParameters
			{
				Orientation = Orientation.Vertical,
				Area = _area,
				Id = "WordsAndOccurrencesMultiPane",
				ToolMachineName = MachineName,
				DefaultPrintPane = "wordOccurrenceList",
				SecondCollapseZone = 144000,
				FirstControlParameters = new SplitterChildControlParameters(), // Leave its Control as null. Will be a newly created MultiPane, the controls of which are in "nestedMultiPaneParameters"
				SecondControlParameters = new SplitterChildControlParameters() // Control (PaneBarContainer+InterlinMasterNoTitleBar) added below. Leave Label null.
			};

			var root = XDocument.Parse(TextAndWordsResources.ConcordanceToolParameters).Root;
			var columns = XElement.Parse(TextAndWordsResources.ConcordanceColumns).Element("columns");
			root.Element("wordOccurrenceList").Element("parameters").Element("includeCordanceColumns").ReplaceWith(columns);
			_interlinMasterNoTitleBar = new InterlinMasterNoTitleBar(root.Element("ITextControl").Element("parameters"), majorFlexComponentParameters, _recordList);
			mainConcordanceContainerParameters.SecondControlParameters.Control = PaneBarContainerFactory.Create(majorFlexComponentParameters.FlexComponentParameters, _interlinMasterNoTitleBar);

			// This will be the nested MultiPane that goes into mainConcordanceContainerParameters.FirstControlParameters.Control
			var nestedMultiPaneParameters = new MultiPaneParameters
			{
				Orientation = Orientation.Horizontal,
				Area = _area,
				Id = "LineAndTextMultiPane",
				ToolMachineName = MachineName,
				FirstCollapseZone = 110000,
				SecondCollapseZone = 180000,
				DefaultFixedPaneSizePoints = "200",
				FirstControlParameters = new SplitterChildControlParameters(), // Control (PaneBarContainer+ConcordanceControl) added below. Leave Label null.
				SecondControlParameters = new SplitterChildControlParameters() // Control (PaneBarContainer+RecordBrowseView) added below. Leave Label null.
			};
			_concordanceControl = new ConcordanceControl(majorFlexComponentParameters.SharedEventHandlers, (MatchingConcordanceItems)_recordList);
			nestedMultiPaneParameters.FirstControlParameters.Control = PaneBarContainerFactory.Create(majorFlexComponentParameters.FlexComponentParameters, _concordanceControl);
			_recordBrowseView = new RecordBrowseView(root.Element("wordOccurrenceList").Element("parameters"), majorFlexComponentParameters.LcmCache, _recordList, majorFlexComponentParameters.UiWidgetController);
			nestedMultiPaneParameters.SecondControlParameters.Control = PaneBarContainerFactory.Create(majorFlexComponentParameters.FlexComponentParameters, _recordBrowseView);
			// Nested MP is created by call to MultiPaneFactory.CreateConcordanceContainer
			_concordanceContainer = MultiPaneFactory.CreateConcordanceContainer(majorFlexComponentParameters.FlexComponentParameters, majorFlexComponentParameters.MainCollapsingSplitContainer, mainConcordanceContainerParameters, nestedMultiPaneParameters);

			_interlinMasterNoTitleBar.FinishInitialization();
		}

		/// <summary>
		/// Do whatever might be needed to get ready for a refresh.
		/// </summary>
		public void PrepareToRefresh()
		{
			_interlinMasterNoTitleBar.PrepareToRefresh();
			_recordBrowseView.BrowseViewer.BrowseView.PrepareToRefresh();
		}

		/// <summary>
		/// Finish the refresh.
		/// </summary>
		public void FinishRefresh()
		{
			_recordList.ReloadIfNeeded();
			((DomainDataByFlidDecoratorBase)_recordList.VirtualListPublisher).Refresh();
		}

		/// <summary>
		/// The properties are about to be saved, so make sure they are all current.
		/// Add new ones, as needed.
		/// </summary>
		public void EnsurePropertiesAreCurrent()
		{
		}

		#endregion

		#region Implementation of IMajorFlexUiComponent

		/// <summary>
		/// Get the internal name of the component.
		/// </summary>
		/// <remarks>NB: This is the machine friendly name, not the user friendly name.</remarks>
		public string MachineName => AreaServices.ConcordanceMachineName;

		/// <summary>
		/// User-visible localizable component name.
		/// </summary>
		public string UiName => "Concordance";
		#endregion

		#region Implementation of ITool

		/// <summary>
		/// Get the area for the tool.
		/// </summary>
		public IArea Area => _area;

		/// <summary>
		/// Get the image for the area.
		/// </summary>
		public Image Icon => Images.SideBySideView.SetBackgroundColor(Color.Magenta);


		#endregion

		private static IRecordList FactoryMethod(LcmCache cache, FlexComponentParameters flexComponentParameters, string recordListId, StatusBar statusBar)
		{
			Require.That(recordListId == OccurrencesOfSelectedUnit, $"I don't know how to create a record list with an ID of '{recordListId}', as I can only create on with an id of '{OccurrencesOfSelectedUnit}'.");
			/*
            <clerk id="OccurrencesOfSelectedUnit" allowDeletions="false">
              <dynamicloaderinfo assemblyPath="ITextDll.dll" class="SIL.FieldWorks.IText.OccurrencesOfSelectedUnit" />
              <recordList class="LangProject" field="ConcOccurrences">
                <dynamicloaderinfo assemblyPath="ITextDll.dll" class="SIL.FieldWorks.IText.MatchingConcordanceItems" />
                <decoratorClass assemblyPath="xWorks.dll" class="SIL.FieldWorks.XWorks.ConcDecorator" />
              </recordList>
              <sortMethods />
            </clerk>
			*/
			var concDecorator = new ConcDecorator(cache.ServiceLocator);
			concDecorator.InitializeFlexComponent(flexComponentParameters);
			return new MatchingConcordanceItems(recordListId, statusBar, concDecorator);
		}

		private sealed class ConcordanceToolMenuHelper : IDisposable
		{
			private MajorFlexComponentParameters _majorFlexComponentParameters;

			internal ConcordanceToolMenuHelper(MajorFlexComponentParameters majorFlexComponentParameters, ITool tool)
			{
				Guard.AgainstNull(majorFlexComponentParameters, nameof(majorFlexComponentParameters));
				Guard.AgainstNull(tool, nameof(tool));

				_majorFlexComponentParameters = majorFlexComponentParameters;
				// Tool must be added, even when it adds no tool specific handlers.
				_majorFlexComponentParameters.UiWidgetController.AddHandlers(new ToolUiWidgetParameterObject(tool));
#if RANDYTODO
				// TODO: See LexiconEditTool for how to set up all manner of menus and tool bars.
#endif
				// NB: No popup menu on the browse view.
			}

			#region Implementation of IDisposable
			private bool _isDisposed;

			~ConcordanceToolMenuHelper()
			{
				// The base class finalizer is called automatically.
				Dispose(false);
			}

			/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
			public void Dispose()
			{
				Dispose(true);
				// This object will be cleaned up by the Dispose method.
				// Therefore, you should call GC.SuppressFinalize to
				// take this object off the finalization queue
				// and prevent finalization code for this object
				// from executing a second time.
				GC.SuppressFinalize(this);
			}

			private void Dispose(bool disposing)
			{
				Debug.WriteLineIf(!disposing, "****** Missing Dispose() call for " + GetType().Name + ". ****** ");
				if (_isDisposed)
				{
					// No need to run it more than once.
					return;
				}

				if (disposing)
				{
				}
				_majorFlexComponentParameters = null;

				_isDisposed = true;
			}
			#endregion
		}
	}
}
