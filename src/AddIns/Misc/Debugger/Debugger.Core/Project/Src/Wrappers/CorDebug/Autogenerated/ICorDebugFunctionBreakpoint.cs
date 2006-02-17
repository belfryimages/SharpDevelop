// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

// This file is automatically generated - any changes will be lost

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public partial class ICorDebugFunctionBreakpoint
	{
		
		private Debugger.Interop.CorDebug.ICorDebugFunctionBreakpoint wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugFunctionBreakpoint WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugFunctionBreakpoint(Debugger.Interop.CorDebug.ICorDebugFunctionBreakpoint wrappedObject)
		{
			this.wrappedObject = wrappedObject;
			ResourceManager.TrackCOMObject(wrappedObject, typeof(ICorDebugFunctionBreakpoint));
		}
		
		public static ICorDebugFunctionBreakpoint Wrap(Debugger.Interop.CorDebug.ICorDebugFunctionBreakpoint objectToWrap)
		{
			if ((objectToWrap != null))
			{
				return new ICorDebugFunctionBreakpoint(objectToWrap);
			} else
			{
				return null;
			}
		}
		
		~ICorDebugFunctionBreakpoint()
		{
			object o = wrappedObject;
			wrappedObject = null;
			ResourceManager.ReleaseCOMObject(o, typeof(ICorDebugFunctionBreakpoint));
		}
		
		public bool Is<T>() where T: class
		{
			try {
				CastTo<T>();
				return true;
			} catch {
				return false;
			}
		}
		
		public T As<T>() where T: class
		{
			try {
				return CastTo<T>();
			} catch {
				return null;
			}
		}
		
		public T CastTo<T>() where T: class
		{
			return (T)Activator.CreateInstance(typeof(T), this.WrappedObject);
		}
		
		public static bool operator ==(ICorDebugFunctionBreakpoint o1, ICorDebugFunctionBreakpoint o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugFunctionBreakpoint o1, ICorDebugFunctionBreakpoint o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugFunctionBreakpoint casted = o as ICorDebugFunctionBreakpoint;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void Activate(int bActive)
		{
			this.WrappedObject.Activate(bActive);
		}
		
		public int IsActive
		{
			get
			{
				int pbActive;
				this.WrappedObject.IsActive(out pbActive);
				return pbActive;
			}
		}
		
		public ICorDebugFunction Function
		{
			get
			{
				ICorDebugFunction ppFunction;
				Debugger.Interop.CorDebug.ICorDebugFunction out_ppFunction;
				this.WrappedObject.GetFunction(out out_ppFunction);
				ppFunction = ICorDebugFunction.Wrap(out_ppFunction);
				return ppFunction;
			}
		}
		
		public uint Offset
		{
			get
			{
				uint pnOffset;
				this.WrappedObject.GetOffset(out pnOffset);
				return pnOffset;
			}
		}
	}
}
