## 核心库 DH.NCore
核心组件，支撑其它所有组件。
主要功能包括：
+ **[日志]** 统一ILog接口，内置控制台、文本文件、WinForm控件和网络日志等实现
+ **[网络]** 单点最高400万长连接
+ **[RPC]** 单点最高处理能力2266万tps
+ **[缓存]** 统一ICache接口，内置MemoryCache、Redis实现
+ **[安全]** AES/DES/RC4/RSA/DSA/CRC
+ **[多线程]** 定时调度TimerX
+ **[APM]** 性能追踪，分布式应用必备
+ **[序列化]** Binary/Json/Xml

各项目默认支持net10.0/netstandard2.1/netstandard2.0/net4.62/net4.5，旧版（2024.0801）支持net4.0/net2.0  