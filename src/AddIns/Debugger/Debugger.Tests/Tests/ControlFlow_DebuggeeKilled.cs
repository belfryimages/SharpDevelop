﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests
{
	public class ControlFlow_DebuggeeKilled
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using NUnit.Framework;

	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void ControlFlow_DebuggeeKilled()
		{
			StartTest();
			
			foreach(System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses()) {
				if (p.ProcessName.StartsWith("ControlFlow_DebuggeeKilled"))
					p.Kill();
			}
			process.WaitForExit();
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="ControlFlow_DebuggeeKilled.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ControlFlow_DebuggeeKilled.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break ControlFlow_DebuggeeKilled.cs:16,4-16,40</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT