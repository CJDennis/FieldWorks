﻿// Copyright (c) 2015 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Text;
using NUnit.Framework;
using SIL.FieldWorks.XWorks.Archiving;

namespace SIL.FieldWorks.XWorks
{
	[TestFixture]
	public class ArchivingTests
	{
		[Test]
		public void StringBuilder_AppendLineFormat()
		{
			var A = "A";
			var B = "B";
			var C = "C";
			var format = "{0}{1}{2}";
			var delimiter = ";;";
			var expected = "ABC;;CBA;;BCA";

			var sb = new StringBuilder();
			sb.AppendLineFormat(format, new object[] { A, B, C }, delimiter);
			sb.AppendLineFormat(format, new object[] { C, B, A }, delimiter);
			sb.AppendLineFormat(format, new object[] { B, C, A }, delimiter);

			Assert.AreEqual(expected, sb.ToString());
		}
	}
}