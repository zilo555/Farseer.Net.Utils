using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FS.Utils.Component
{
    /// <summary>
    ///     测试效率的工具
    ///     用于做平均效率测试
    /// </summary>
    public class SpeedTest : IDisposable
    {
        /// <summary>
        ///     锁定
        /// </summary>
        private readonly object _objLock = new object();

        /// <summary>
        ///     保存测试的结果
        /// </summary>
        public List<SpeedResult> ListResult = new List<SpeedResult>();

        /// <summary>
        ///     保存测试的结果
        /// </summary>
        public SpeedResult Result => ListResult.Last();

        /// <summary>
        ///     使用完后，自动计算时间
        /// </summary>
        public void Dispose()
        {
            ListResult.Last(o => o.Timer.IsRunning).Timer.Stop();
        }

        /// <summary>
        ///     开始计数
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public SpeedTest Begin(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName)) { throw new Exception("必须设置keyName的值！"); }

            Create(keyName);
            var result = ListResult.FirstOrDefault(o => o.KeyName == keyName);
            result.Timer = new Stopwatch();
            result.Timer.Start();
            return this;
        }

        /// <summary>
        ///     开始计数
        /// </summary>
        public SpeedTest Begin()
        {
            var result = new SpeedResult { Timer = new Stopwatch() };
            result.Timer.Start();

            ListResult = new List<SpeedResult> { result };
            return this;
        }

        /// <summary>
        ///     停止工作
        /// </summary>
        public void Stop(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName)) { throw new Exception("必须设置keyName的值！"); }

            Create(keyName);
            ListResult.FirstOrDefault(o => o.KeyName == keyName)?.Timer.Stop();
        }

        /// <summary>
        ///     判断键位是否存在（不存在，自动创建）
        /// </summary>
        private void Create(string keyName)
        {
            if (ListResult.Count(o => o.KeyName == keyName) != 0) return;
            lock (_objLock) { if (ListResult.Count(o => o.KeyName == keyName) == 0) { ListResult.Add(new SpeedResult { KeyName = keyName, Timer = new Stopwatch() }); } }
        }

        /// <summary>
        ///     返回执行结果
        /// </summary>
        public class SpeedResult
        {
            /// <summary>
            ///     当前键码
            /// </summary>
            public string KeyName;

            /// <summary>
            ///     当前时间计数器
            /// </summary>
            public Stopwatch Timer;
        }
    }
}