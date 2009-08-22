﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.SharpDevelop.Services;
using System;
using System.Collections.Generic;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// Object in the debugee, with lazy evaluated properties.
	/// </summary>
	public class ObjectValue
	{
		// Used to be able to expand items of IEnumerable
		// Now we rely on PermanentReference to be able to get member values on demand. With IList, PermanentReference could be replaced by Expression
		public Value PermanentReference { get; private set; }
		
		private Dictionary<string, ObjectProperty> properties = new Dictionary<string, ObjectProperty>();
		
		/// <summary> Used to quickly find MemberInfo by member name - DebugType.GetMember(name) uses a loop to search members </summary>
		private Dictionary<string, MemberInfo> memberForNameMap;
		
		internal ObjectValue(Dictionary<string, MemberInfo> memberFromNameMap)
		{
			this.memberForNameMap = memberFromNameMap;
		}

		/// <summary>
		/// Returns property with given name.
		/// </summary>
		public ObjectProperty this[string propertyName]
		{
			get
			{
				ObjectProperty property;
				// has property with name 'propertyName' already been evaluated?
				if(!this.properties.TryGetValue(propertyName, out property)) 
				{
					if (this.PermanentReference == null) {
						throw new DebuggerVisualizerException("Cannot get member of this ObjectValue - ObjectValue.PermanentReference is null");
					}
					MemberInfo memberInfo = this.memberForNameMap.GetValue(propertyName);
					if (memberInfo == null) {
						throw new DebuggerVisualizerException("Cannot get member value - no member found with name " + propertyName);
					}
					property = createPropertyFromValue(propertyName, this.PermanentReference.GetMemberValue(memberInfo));
					this.properties.Add(propertyName, property);
				}
				return property;
			}
			//set	{ properties[key] = value; }
		}
		
		public static ObjectValue Create(Debugger.Value value, Dictionary<string, MemberInfo> memberFromName)
		{
			ObjectValue result = new ObjectValue(memberFromName);
			
			// remember PermanentReference for expanding IEnumerable
			Value permanentReference = value.GetPermanentReference();
			result.PermanentReference = permanentReference;
			
			return result;
		}
		
		private static ObjectProperty createPropertyFromValue(string propertyName, Value value)
		{
			ObjectProperty property = new ObjectProperty();
			property.Name = propertyName;
			// property.Expression = ?.Age
			// - cannot use expression,
			// if the value is returned from an enumerator, it has no meaningful expression
			property.IsAtomic = value.Type.IsPrimitive;
			property.IsNull = value.IsNull;
			property.Value = value.IsNull ? "" : value.InvokeToString();
			return property;
		}
		
		/*public static ObjectValue Create(Expression expr, DebugType type, BindingFlags bindingFlags)
		{
			ObjectValue result = new ObjectValue();
			foreach(MemberInfo memberInfo in type.GetMembers(bindingFlags))
			{
				Expression memberExpression = expr.AppendMemberReference(memberInfo);
				Value memberValue = memberExpression.Evaluate(WindowsDebugger.CurrentProcess);
					
				ObjectProperty property = new ObjectProperty();
				property.Name = memberInfo.Name;
				property.Expression = memberExpression;
				property.IsAtomic = memberValue.Type.IsPrimitive;
				property.IsNull = memberValue.IsNull;
				property.Value = memberValue.IsNull ? "" : memberValue.InvokeToString();

				result.properties.Add(property.Name, property);
			}
			return result;
		}*/
	}
}