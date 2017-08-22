// Copyright (c) 2003-2015 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)
//
// THIS NEEDS TO BE REFACTORED!!
// <remarks>
// Implementation of:
//		XAmpleTrace - Deal with results of an XAmple trace
// </remarks>

using System.Xml.Linq;
using SIL.FieldWorks.Common.FwUtils;

namespace LanguageExplorer.Areas.TextsAndWords
{
	/// <summary>
	/// Summary description for XAmpleTrace.
	/// </summary>
	internal sealed class XAmpleTrace : IParserTrace
	{
		private static ParserTraceUITransform s_traceTransform;
		private static ParserTraceUITransform TraceTransform
		{
			get
			{
				if (s_traceTransform == null)
					s_traceTransform = new ParserTraceUITransform("FormatXAmpleTrace");
				return s_traceTransform;
			}
		}

		private static ParserTraceUITransform s_parseTransform;
		private static ParserTraceUITransform ParseTransform
		{
			get
			{
				if (s_parseTransform == null)
					s_parseTransform = new ParserTraceUITransform("FormatXAmpleParse");
				return s_parseTransform;
			}
		}

		/// <summary>
		/// Create an HTML page of the results
		/// </summary>
		/// <param name="propertyTable"></param>
		/// <param name="result">XML string of the XAmple trace output</param>
		/// <param name="isTrace"></param>
		/// <returns>URL of the resulting HTML page</returns>
		public string CreateResultPage(IPropertyTable propertyTable, XDocument result, bool isTrace)
		{
			ParserTraceUITransform transform;
			string baseName;
			if (isTrace)
			{
				transform = TraceTransform;
				baseName = "XAmpleTrace";
			}
			else
			{
				transform = ParseTransform;
				baseName = "XAmpleParse";
			}
			return transform.Transform(propertyTable, result, baseName);
		}
	}
}