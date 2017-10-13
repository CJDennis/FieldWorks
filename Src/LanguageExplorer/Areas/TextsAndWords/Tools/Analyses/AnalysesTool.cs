﻿// Copyright (c) 2015-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using L10NSharp.UI;
using LanguageExplorer.Controls;
using LanguageExplorer.Controls.DetailControls;
using LanguageExplorer.Controls.PaneBar;
using SIL.FieldWorks.Common.FwUtils;
using SIL.FieldWorks.Resources;
using LanguageExplorer.Works;
using SIL.LCModel.Application;
using SIL.LCModel.Utils;

namespace LanguageExplorer.Areas.TextsAndWords.Tools.Analyses
{
	/// <summary>
	/// ITool implementation for the "Analyses" tool in the "textsWords" area.
	/// </summary>
	internal sealed class AnalysesTool : ITool
	{
		private AreaWideMenuHelper _areaWideMenuHelper;
		private TextAndWordsAreaMenuHelper _textAndWordsAreaMenuHelper;
		private MultiPane _multiPane;
		private RecordBrowseView _recordBrowseView;
		private RecordClerk _recordClerk;

		#region Implementation of IPropertyTableProvider

		/// <summary>
		/// Placement in the IPropertyTableProvider interface lets FwApp call IPropertyTable.DoStuff.
		/// </summary>
		public IPropertyTable PropertyTable { get; private set; }

		#endregion

		#region Implementation of IPublisherProvider

		/// <summary>
		/// Get the IPublisher.
		/// </summary>
		public IPublisher Publisher { get; private set; }

		#endregion

		#region Implementation of ISubscriberProvider

		/// <summary>
		/// Get the ISubscriber.
		/// </summary>
		public ISubscriber Subscriber { get; private set; }

		#endregion

		#region Implementation of IFlexComponent

		/// <summary>
		/// Initialize a FLEx component with the basic interfaces.
		/// </summary>
		/// <param name="flexComponentParameters">Parameter object that contains the required three interfaces.</param>
		public void InitializeFlexComponent(FlexComponentParameters flexComponentParameters)
		{
			FlexComponentCheckingService.CheckInitializationValues(flexComponentParameters, new FlexComponentParameters(PropertyTable, Publisher, Subscriber));

			PropertyTable = flexComponentParameters.PropertyTable;
			Publisher = flexComponentParameters.Publisher;
			Subscriber = flexComponentParameters.Subscriber;
		}

		#endregion

		#region Implementation of IMajorFlexComponent

		/// <summary>
		/// Deactivate the component.
		/// </summary>
		/// <remarks>
		/// This is called on the outgoing component, when the user switches to a component.
		/// </remarks>
		public void Deactivate(MajorFlexComponentParameters majorFlexComponentParameters)
		{
			_areaWideMenuHelper.Dispose();
			_textAndWordsAreaMenuHelper.Dispose();
			MultiPaneFactory.RemoveFromParentAndDispose(majorFlexComponentParameters.MainCollapsingSplitContainer, ref _multiPane);
			_recordBrowseView = null;
			_areaWideMenuHelper = null;
			_textAndWordsAreaMenuHelper = null;
		}

		/// <summary>
		/// Activate the component.
		/// </summary>
		/// <remarks>
		/// This is called on the component that is becoming active.
		/// </remarks>
		public void Activate(MajorFlexComponentParameters majorFlexComponentParameters)
		{
			if (_recordClerk == null)
			{
				_recordClerk = majorFlexComponentParameters.RecordClerkRepositoryForTools.GetRecordClerk(TextAndWordsArea.ConcordanceWords, majorFlexComponentParameters.Statusbar, TextAndWordsArea.ConcordanceWordsFactoryMethod);
			}
			_areaWideMenuHelper = new AreaWideMenuHelper(majorFlexComponentParameters, _recordClerk);
			_areaWideMenuHelper.SetupFileExportMenu();
			_textAndWordsAreaMenuHelper = new TextAndWordsAreaMenuHelper(majorFlexComponentParameters);
			_textAndWordsAreaMenuHelper.AddMenusForAllButConcordanceTool();

			var root = XDocument.Parse(TextAndWordsResources.WordListParameters).Root;
			var columnsElement = XElement.Parse(TextAndWordsResources.WordListColumns);
			var overriddenColumnElement = columnsElement.Elements("column").First(column => column.Attribute("label").Value == "Form");
			overriddenColumnElement.Attribute("width").Value = "25%";
			overriddenColumnElement = columnsElement.Elements("column").First(column => column.Attribute("label").Value == "Word Glosses");
			overriddenColumnElement.Attribute("width").Value = "25%";
			// LT-8373.The point of these overrides: By default, enable User Analyses for "Word Analyses"
			overriddenColumnElement = columnsElement.Elements("column").First(column => column.Attribute("label").Value == "User Analyses");
			overriddenColumnElement.Attribute("visibility").Value = "always";
			overriddenColumnElement.Add(new XAttribute("width", "15%"));
			root.Add(columnsElement);
			_recordBrowseView = new RecordBrowseView(root, majorFlexComponentParameters.LcmCache, _recordClerk);
#if RANDYTODO
			// TODO: See LexiconEditTool for how to set up all manner of menus and toolbars.
#endif
			var dataTree = new DataTree();
			var recordEditView = new RecordEditView(XElement.Parse(TextAndWordsResources.AnalysesRecordEditViewParameters), XDocument.Parse(AreaResources.VisibilityFilter_All), majorFlexComponentParameters.LcmCache, _recordClerk, dataTree, MenuServices.GetFilePrintMenu(majorFlexComponentParameters.MenuStrip));
			_multiPane = MultiPaneFactory.CreateMultiPaneWithTwoPaneBarContainersInMainCollapsingSplitContainer(
				majorFlexComponentParameters.FlexComponentParameters,
				majorFlexComponentParameters.MainCollapsingSplitContainer,
				new MultiPaneParameters
				{
					Orientation = Orientation.Vertical,
					AreaMachineName = AreaMachineName,
					Id = "WordsAndAnalysesMultiPane",
					ToolMachineName = MachineName
				},
				_recordBrowseView, "WordList", new PaneBar(),
				recordEditView, "SingleWord", new PaneBar());

			using (var gr = _multiPane.CreateGraphics())
			{
				_multiPane.Panel2MinSize = Math.Max((int)(180000 * gr.DpiX) / MiscUtils.kdzmpInch,
						CollapsingSplitContainer.kCollapseZone);
			}

			// Too early before now.
			recordEditView.FinishInitialization();
			RecordClerkServices.SetClerk(majorFlexComponentParameters, _recordClerk);
		}

		/// <summary>
		/// Do whatever might be needed to get ready for a refresh.
		/// </summary>
		public void PrepareToRefresh()
		{
			_recordBrowseView.BrowseViewer.BrowseView.PrepareToRefresh();
		}

		/// <summary>
		/// Finish the refresh.
		/// </summary>
		public void FinishRefresh()
		{
			_recordClerk.ReloadIfNeeded();
			((DomainDataByFlidDecoratorBase)_recordClerk.VirtualListPublisher).Refresh();
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
		public string MachineName => "Analyses";

		/// <summary>
		/// User-visible localizable component name.
		/// </summary>
		public string UiName => "Word Analyses";
		#endregion

		#region Implementation of ITool

		/// <summary>
		/// Get the area machine name the tool is for.
		/// </summary>
		public string AreaMachineName => "textsWords";

		/// <summary>
		/// Get the image for the area.
		/// </summary>
		public Image Icon => Images.SideBySideView.SetBackgroundColor(Color.Magenta);

		#endregion
	}
}