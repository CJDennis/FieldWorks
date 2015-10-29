﻿// Copyright (c) 2015 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using SIL.Machine.Annotations;
using SIL.Machine.FeatureModel;
using SIL.Machine.Matching;

namespace SIL.FieldWorks.IText
{
	public class ComplexConcWordBdryNode : ComplexConcLeafNode
	{
		public override PatternNode<ComplexConcParagraphData, ShapeNode> GeneratePattern(FeatureSystem featSys)
		{
			return new Constraint<ComplexConcParagraphData, ShapeNode>(FeatureStruct.New(featSys).Symbol("bdry").Symbol("wordBdry").Value);
		}
	}
}
