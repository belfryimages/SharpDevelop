﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using IQToolkit;
using System.Text;

namespace ICSharpCode.Profiler.Controller.Data.Linq
{
	/// <summary>
	/// Performs query optimizations.
	/// See the documentation on SQLiteQueryProvider for the list of optimizations being performed.
	/// </summary>
	sealed class OptimizeQueryExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
	{
		QueryNode Visit(QueryNode queryNode)
		{
			return (QueryNode)base.Visit(queryNode);
		}
		
		protected override Expression VisitExtension(Expression node)
		{
			Filter filter = node as Filter;
			if (filter != null)
				return VisitFilter(filter);
			else
				return base.VisitExtension(node);
		}
		
		QueryNode VisitFilter(Filter filter)
		{
			QueryNode result = OptimizeFilter(filter);
			filter = result as Filter;
			if (filter == null)
				return result;
			
			return ReorderFilter(filter);
		}
		
		/// <summary>
		/// Tries to combine nested filters;
		/// move 'MergeByName' nodes out of filter, if possible
		/// </summary>
		QueryNode ReorderFilter(Filter filter)
		{
			if (filter.Target is Filter) {
				// x.Filter(y).Filter(z) -> x.Filter(y && z)
				Filter innerFilter = (Filter)filter.Target;
				return ReorderFilter(new Filter(innerFilter.Target, innerFilter.Conditions.Concat(filter.Conditions).ToArray()));
			} else if (filter.Target is MergeByName) {
				// x.MergeByName().Filter(<criteria>) -> x.Filter(x, <criteria>).MergeByName() for some safe criterias
				QueryNode innerTarget = filter.Target.Target;
				var conditionsToMoveIntoFilter = filter.Conditions.Where(c => IsConditionSafeForMoveIntoMergeByName.Test(c)).ToArray();
				if (conditionsToMoveIntoFilter.Length != 0) {
					MergeByName newTarget = new MergeByName(ReorderFilter(new Filter(innerTarget, conditionsToMoveIntoFilter)));
					var conditionsKeptOutsideFilter = filter.Conditions.Except(conditionsToMoveIntoFilter).ToArray();
					if (conditionsKeptOutsideFilter.Length == 0)
						return newTarget;
					else
						return new Filter(newTarget, conditionsKeptOutsideFilter);
				} else {
					return filter;
				}
			} else {
				return filter;
			}
		}
		
		sealed class IsConditionSafeForMoveIntoMergeByName : System.Linq.Expressions.ExpressionVisitor
		{
			public static bool Test(Expression ex)
			{
				var visitor = new IsConditionSafeForMoveIntoMergeByName();
				return visitor.IsSafe;
			}
			
			static readonly MemberInfo[] SafeMembers = {
				KnownMembers.CallTreeNode_NameMapping
			};
			
			bool IsSafe = true;
			
			protected override Expression VisitMember(MemberExpression node)
			{
				if (node.Expression.NodeType == ExpressionType.Parameter && !SafeMembers.Contains(node.Member))
					IsSafe = false;
				return base.VisitMember(node);
			}
			
			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				if (node.Object.NodeType == ExpressionType.Parameter && !SafeMembers.Contains(node.Method))
					IsSafe = false;
				return base.VisitMethodCall(node);
			}
		}
		
		/// <summary>
		/// Optimizes the filter; but does not try to combine nested filter (etc.)
		/// </summary>
		QueryNode OptimizeFilter(Filter filter)
		{
			QueryNode target = Visit(filter.Target);
			
			List<LambdaExpression> newConditions = new List<LambdaExpression>();
			OptimizeQueryExpressionVisitor optimizer = new OptimizeQueryExpressionVisitor();
			foreach (LambdaExpression expr in filter.Conditions) {
				Expression optimizedExpr = optimizer.Visit(expr.Body);
				if (optimizedExpr.NodeType == ExpressionType.Constant && optimizedExpr.Type == typeof(bool)) {
					bool val = (bool)((ConstantExpression)optimizedExpr).Value;
					if (val)
						continue;
				}
				newConditions.Add(Expression.Lambda(optimizedExpr, expr.Parameters));
			}
			if (newConditions.Count == 0)
				return target;
			else
				return new Filter(target, newConditions.ToArray());
		}
		
		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Method == KnownMembers.ListOfInt_Contains && node.Object.NodeType == ExpressionType.Constant && node.Arguments[0].Type == typeof(int)) {
				List<int> list = (List<int>)((ConstantExpression)node.Object).Value;
				if (list.Count == 0)
					return Expression.Constant(false);
				else if (list.Count == 1)
					return Expression.Equal(Visit(node.Arguments[0]), Expression.Constant(list[0]));
			}
			return base.VisitMethodCall(node);
		}
	}
}