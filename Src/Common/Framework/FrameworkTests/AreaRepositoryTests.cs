﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SIL.FieldWorks.Common.Framework.Impls;

namespace SIL.FieldWorks.Common.Framework
{
	/// <summary>
	/// Test the AreaRepository.
	/// </summary>
	[TestFixture]
	[Category("ByHand")] // Needs to be run after everything has been built, or the areas may not be built yet.
	public class AreaRepositoryTests
	{
		private AreaRepository m_areaRepository;

		/// <summary>
		/// Set up test fixture.
		/// </summary>
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			m_areaRepository = new AreaRepository();
		}

		/// <summary>
		/// Tear down the test fixture.
		/// </summary>
		[TestFixtureTearDown]
		public void FextureTeardown()
		{
			m_areaRepository = null;
		}

		/// <summary>
		/// Doesn't have some unknown area.
		/// </summary>
		[Test]
		public void UnknownAreaNotPresent()
		{
			Assert.IsNull(m_areaRepository.GetArea("bogusArea"));
		}

		/// <summary>
		/// Make sure the AreaRepository has the expected nubmer of areas.
		/// </summary>
		[Test]
		public void AreaRepositoryHasLexiconArea()
		{
			Assert.IsNotNull(m_areaRepository.GetArea("lexicon"));
		}
	}
}
