using System.Windows;
using System.Windows.Input;

namespace AbstractDevelop
{

    public static class CustomCommands
    {
        public static readonly RoutedUICommand
            FileClose = new RoutedUICommand(nameof(FileClose), nameof(FileClose), typeof(CustomCommands), null),
            ClearDebug = new RoutedUICommand(nameof(ClearDebug), nameof(ClearDebug), typeof(CustomCommands), null),
            ClearAll = new RoutedUICommand(nameof(ClearAll), nameof(ClearAll), typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.F2) }),
            SelectFormat = new RoutedUICommand(nameof(SelectFormat), nameof(SelectFormat), typeof(CustomCommands), null),
            CustomizeLayout = new RoutedUICommand(nameof(CustomizeLayout), nameof(CustomizeLayout), typeof(CustomCommands), null),
            DebugStartStop = new RoutedUICommand(nameof(DebugStartStop), nameof(DebugStartStop), typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.F5) }),
            DebugStep = new RoutedUICommand(nameof(DebugStep), nameof(DebugStep), typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.F6) }),
            DebugPause = new RoutedUICommand(nameof(DebugPause), nameof(DebugPause), typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.F7) }),
            DebugBreakpoint = new RoutedUICommand(nameof(DebugBreakpoint), nameof(DebugBreakpoint), typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.F9) });
    }

}
