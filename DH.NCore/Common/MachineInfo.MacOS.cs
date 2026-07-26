using System.Runtime.Versioning;

namespace NewLife;

/// <summary>机器信息 macOS 部分</summary>
public partial class MachineInfo
{
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("macos")]
#endif
    private void LoadMacInfo()
    {
        var dic = ReadCommand("sw_vers");
        if (dic != null)
        {
            if (dic.TryGetValue("ProductName", out var str)) OSName = str;
            if (dic.TryGetValue("ProductVersion", out str)) OSVersion = str;
        }

        dic = ReadCommand("system_profiler", "SPHardwareDataType");
        if (dic != null)
        {
            //if (dic2.TryGetValue("Model Name", out str)) Product = str;
            if (dic.TryGetValue("Model Identifier", out var str)) Product = str;
            if (dic.TryGetValue("Processor Name", out str)) Processor = str;
            if (dic.TryGetValue("Memory", out str)) Memory = (UInt64)str.TrimSuffix("GB").Trim().ToLong() * 1024 * 1024 * 1024;
            if (dic.TryGetValue("Serial Number (system)", out str)) Serial = str;
            if (dic.TryGetValue("Hardware UUID", out str)) UUID = str;
        }

        if (Vendor.IsNullOrEmpty()) Vendor = "Apple";

        dic = ReadCommand("diskutil", "info disk1");
        if (dic != null)
        {
            if (dic.TryGetValue("Disk / Partition UUID", out var str)) DiskID = str;
        }
    }

    #region macOS辅助
    private static IDictionary<String, String>? ReadCommand(String cmd, String? arguments = null)
    {
        var str = cmd.Execute(arguments, 0, false);
        if (str.IsNullOrEmpty()) return null;

        return str.SplitAsDictionary(":", "\n", true);
    }
    #endregion
}
