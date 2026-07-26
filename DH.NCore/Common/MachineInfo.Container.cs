namespace NewLife;

/// <summary>机器信息容器检测部分</summary>
public partial class MachineInfo
{
    #region 容器检测
    /// <summary>是否运行在容器中</summary>
    /// <remarks>
    /// 检测依据：
    /// 1. 环境变量 DOTNET_RUNNING_IN_CONTAINER
    /// 2. 存在 /.dockerenv 文件
    /// 3. /proc/1/cgroup 包含 docker/kubepods/containerd 等关键字
    /// </remarks>
    public static Boolean IsInContainer
    {
        get
        {
            // 环境变量检测
            var env = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
            if (env.EqualIgnoreCase("true", "1")) return true;

            if (!Runtime.Linux) return false;

            // Docker 环境文件检测
            if (File.Exists("/.dockerenv")) return true;

            // cgroup 检测
            var cgroupFile = "/proc/1/cgroup";
            if (File.Exists(cgroupFile))
            {
                try
                {
                    var content = File.ReadAllText(cgroupFile);
                    if (content.Contains("docker") ||
                        content.Contains("kubepods") ||
                        content.Contains("containerd") ||
                        content.Contains("lxc"))
                        return true;
                }
                catch { }
            }

            return false;
        }
    }

    /// <summary>获取容器资源限制</summary>
    /// <returns>元组：(内存限制字节数, CPU核数限制)。返回null表示无限制或获取失败</returns>
    /// <remarks>
    /// 读取 cgroup v1/v2 的资源限制配置：
    /// - 内存限制：/sys/fs/cgroup/memory/memory.limit_in_bytes (v1) 或 /sys/fs/cgroup/memory.max (v2)
    /// - CPU限制：/sys/fs/cgroup/cpu/cpu.cfs_quota_us 和 cpu.cfs_period_us (v1) 或 /sys/fs/cgroup/cpu.max (v2)
    /// </remarks>
    public static (Int64? MemoryLimit, Double? CpuLimit) GetContainerLimits()
    {
        Int64? memoryLimit = null;
        Double? cpuLimit = null;

        if (!Runtime.Linux) return (memoryLimit, cpuLimit);

        // 尝试读取内存限制
        // cgroup v2
        var memFile = "/sys/fs/cgroup/memory.max";
        if (File.Exists(memFile))
        {
            if (TryRead(memFile, out var value) && value != "max")
                memoryLimit = value.ToLong();
        }
        else
        {
            // cgroup v1
            memFile = "/sys/fs/cgroup/memory/memory.limit_in_bytes";
            if (File.Exists(memFile) && TryRead(memFile, out var value))
            {
                var limit = value.ToLong();
                // 如果接近系统最大值（通常是一个很大的数），则认为无限制
                if (limit > 0 && limit < 9_000_000_000_000_000_000L)
                    memoryLimit = limit;
            }
        }

        // 尝试读取 CPU 限制
        // cgroup v2: cpu.max 格式为 "quota period" 或 "max period"
        var cpuFile = "/sys/fs/cgroup/cpu.max";
        if (File.Exists(cpuFile))
        {
            if (TryRead(cpuFile, out var value))
            {
                var parts = value.Split(' ');
                if (parts.Length >= 2 && parts[0] != "max")
                {
                    var quota = parts[0].ToLong();
                    var period = parts[1].ToLong();
                    if (quota > 0 && period > 0)
                        cpuLimit = (Double)quota / period;
                }
            }
        }
        else
        {
            // cgroup v1
            var quotaFile = "/sys/fs/cgroup/cpu/cpu.cfs_quota_us";
            var periodFile = "/sys/fs/cgroup/cpu/cpu.cfs_period_us";
            if (File.Exists(quotaFile) && File.Exists(periodFile))
            {
                if (TryRead(quotaFile, out var quotaStr) && TryRead(periodFile, out var periodStr))
                {
                    var quota = quotaStr.ToLong();
                    var period = periodStr.ToLong();
                    // quota=-1 表示无限制
                    if (quota > 0 && period > 0)
                        cpuLimit = (Double)quota / period;
                }
            }
        }

        return (memoryLimit, cpuLimit);
    }

    /// <summary>获取容器内存使用量</summary>
    /// <returns>当前内存使用量（字节）。获取失败返回null</returns>
    public static Int64? GetContainerMemoryUsage()
    {
        if (!Runtime.Linux) return null;

        // cgroup v2
        var usageFile = "/sys/fs/cgroup/memory.current";
        if (File.Exists(usageFile) && TryRead(usageFile, out var value))
            return value.ToLong();

        // cgroup v1
        usageFile = "/sys/fs/cgroup/memory/memory.usage_in_bytes";
        if (File.Exists(usageFile) && TryRead(usageFile, out value))
            return value.ToLong();

        return null;
    }
    #endregion
}
