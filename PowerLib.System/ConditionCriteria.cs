using System;
using PowerLib.System.ComponentModel;

namespace PowerLib.System
{
	/// <summary>
	/// Group criteria
	/// </summary>
	public enum GroupCriteria
	{
    [DisplayStringResource(typeof(GroupCriteria), "PowerLib.System.ConditionCriteria", "GroupCriteria_And")]
    And,
    [DisplayStringResource(typeof(GroupCriteria), "PowerLib.System.ConditionCriteria", "GroupCriteria_Or")]
    Or
	}

	/// <summary>
	/// Quantify criteria
	/// </summary>
	public enum QuantifyCriteria
	{
    [DisplayStringResource(typeof(QuantifyCriteria), "PowerLib.System.ConditionCriteria", "QuantifyCriteria_Any")]
    Any,
    [DisplayStringResource(typeof(QuantifyCriteria), "PowerLib.System.ConditionCriteria", "QuantifyCriteria_All")]
    All
	}

	/// <summary>
	/// Between criteria
	/// </summary>
	public enum BetweenCriteria
	{
    [DisplayStringResource(typeof(BetweenCriteria), "PowerLib.System.ConditionCriteria", "BetweenCriteria_IncludeBoth")]
    IncludeBoth,
    [DisplayStringResource(typeof(BetweenCriteria), "PowerLib.System.ConditionCriteria", "BetweenCriteria_ExcludeLower")]
    ExcludeLower,
    [DisplayStringResource(typeof(BetweenCriteria), "PowerLib.System.ConditionCriteria", "BetweenCriteria_ExcludeUpper")]
    ExcludeUpper,
    [DisplayStringResource(typeof(BetweenCriteria), "PowerLib.System.ConditionCriteria", "BetweenCriteria_ExcludeBoth")]
    ExcludeBoth
	}

	/// <summary>
	/// Comparision criteria
	/// </summary>
	public enum ComparisonCriteria
	{
    [DisplayStringResource(typeof(ComparisonCriteria), "PowerLib.System.ConditionCriteria", "ComparisonCriteria_Equal")]
    Equal,
    [DisplayStringResource(typeof(ComparisonCriteria), "PowerLib.System.ConditionCriteria", "ComparisonCriteria_NotEqual")]
    NotEqual,
    [DisplayStringResource(typeof(ComparisonCriteria), "PowerLib.System.ConditionCriteria", "ComparisonCriteria_LessThan")]
    LessThan,
    [DisplayStringResource(typeof(ComparisonCriteria), "PowerLib.System.ConditionCriteria", "ComparisonCriteria_GreaterThan")]
    GreaterThan,
    [DisplayStringResource(typeof(ComparisonCriteria), "PowerLib.System.ConditionCriteria", "ComparisonCriteria_LessThanOrEqual")]
    LessThanOrEqual,
    [DisplayStringResource(typeof(ComparisonCriteria), "PowerLib.System.ConditionCriteria", "ComparisonCriteria_GreaterThanOrEqual")]
    GreaterThanOrEqual,
	}
}