using System.Runtime.InteropServices;

namespace StayAwake;

internal static class ExecutionState
{
    [Flags]
    private enum ExecutionStateFlags : uint
    {
        Continuous = 0x80000000,
        SystemRequired = 0x00000001,
        DisplayRequired = 0x00000002
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern ExecutionStateFlags SetThreadExecutionState(ExecutionStateFlags flags);

    public static bool KeepAwake(bool keepDisplayOn)
    {
        var flags = ExecutionStateFlags.Continuous | ExecutionStateFlags.SystemRequired;
        if (keepDisplayOn) flags |= ExecutionStateFlags.DisplayRequired;
        return SetThreadExecutionState(flags) != 0;
    }

    public static void RestoreDefault() =>
        _ = SetThreadExecutionState(ExecutionStateFlags.Continuous);
}
