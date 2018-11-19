// Copyright (c) 2006-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Xml.Linq;
using LanguageExplorer.Filters;
using SIL.LCModel;
using SIL.Xml;

namespace LanguageExplorer.Controls.XMLViews
{
	/// <summary>
	/// Subclass that specifies a flid of the main object to match on.
	/// </summary>
	internal abstract class FlidChoiceFilter : ListChoiceFilter
	{
		protected FlidChoiceFilter(LcmCache cache, ListMatchOptions mode, int flid, int[] targets)
			: base(cache, mode, targets)
		{
			Flid = flid;
		}
		internal FlidChoiceFilter() { } // default for persistence.

		internal int Flid { get; private set; }

		/// <summary>
		/// Checks if we are sorted by column of the given flid, and if so returns the hvo for that item.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if [is sorted by field] [the specified flid]; otherwise, <c>false</c>.
		/// </returns>
		internal static bool IsSortedByField(int flid, int iPathFlid, IManyOnePathSortItem item, out int hvo)
		{
			hvo = 0;
			if (item.PathLength > iPathFlid && item.PathFlid(iPathFlid) == flid)
			{
				hvo = item.PathLength > 1 && item.PathLength != iPathFlid + 1 ? item.PathObject(1) : item.KeyObject;
				return true;
			}
			return false;
		}

		public override void PersistAsXml(XElement element)
		{
			base.PersistAsXml(element);
			XmlUtils.SetAttribute(element, "flid", Flid.ToString());
		}

		public override void InitXml(XElement element)
		{
			base.InitXml(element);
			Flid = XmlUtils.GetMandatoryIntegerAttributeValue(element, "flid");
		}

		public override bool CompatibleFilter(XElement colSpec)
		{
			return base.CompatibleFilter(colSpec) && Flid == BulkEditBar.GetFlidFromClassDotName(m_cache, colSpec, "field");
		}
	}
}