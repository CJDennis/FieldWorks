// Copyright (c) 2012-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using NUnit.Framework;

namespace SIL.FieldWorks.Common.FwUtils
{
	/// <summary>
	/// Beginnings of test for DynamicLoader. Most methods are not tested yet.
	/// </summary>
	[TestFixture]
	public class DynamicLoaderTests
	{
		/// <summary>
		/// This is a pretty minimal test of GetPlugins, but a more substantial test requires creating a whole new DLL.
		/// While a simpler bit of code could pass this test, it at least tries the main path through.
		/// </summary>
		[Test]
		public void GetPlugins_CreatesAnInstance()
		{
			Assert.That(typeof(ITest1).IsAssignableFrom(typeof(Test1)));
			var results = DynamicLoader.GetPlugins<ITest1>("FwUtils*.dll");
			Assert.That(results, Has.Count.EqualTo(1));
			Assert.That(results[0], Is.InstanceOf<Test1>());
		}

		/// <summary />
		private interface ITest1
		{
			/// <summary />
			string Dummy();
		}

		/// <summary />
		private sealed class Test1 : ITest1
		{
			/// <summary />
			public string Dummy()
			{
				throw new NotSupportedException();
			}
		}
	}
}