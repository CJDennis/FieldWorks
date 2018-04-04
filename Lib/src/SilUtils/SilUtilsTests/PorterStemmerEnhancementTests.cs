// ---------------------------------------------------------------------------------------------
// Copyright (c) 2012-2015 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)
//
// File: PorterStemmerEnhancementTests.cs
// ---------------------------------------------------------------------------------------------
using System;
using NUnit.Framework;

namespace SIL.Utils
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// TsStringUtils tests.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class PorterStemmerEnhancementTests
	{
		private static PorterStemmer s_stemmer = new PorterStemmer();

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for word ending in "-ying"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void YingSuffix()
		{
			Assert.AreEqual("pray", s_stemmer.stemTerm("praying"));
			Assert.AreEqual("buy", s_stemmer.stemTerm("buying"));
			Assert.AreEqual("ly", s_stemmer.stemTerm("lying"));
			Assert.AreEqual("obey", s_stemmer.stemTerm("obeying"));
			Assert.AreEqual("repli", s_stemmer.stemTerm("replying"));
			Assert.AreEqual("studi", s_stemmer.stemTerm("studying"));
			Assert.AreEqual("tidi", s_stemmer.stemTerm("tidying"));
			Assert.AreEqual("ying", s_stemmer.stemTerm("ying"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for word ending in "-le"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void LeSuffix()
		{
			Assert.AreEqual("disciple", s_stemmer.stemTerm("disciples"));
			Assert.AreEqual("temple", s_stemmer.stemTerm("temples"));
			Assert.AreEqual("people", s_stemmer.stemTerm("people"));
			Assert.AreEqual("cripple", s_stemmer.stemTerm("cripple"));
			Assert.AreEqual("ample", s_stemmer.stemTerm("ample"));
			Assert.AreEqual("battle", s_stemmer.stemTerm("battles"));
		}
	}
}