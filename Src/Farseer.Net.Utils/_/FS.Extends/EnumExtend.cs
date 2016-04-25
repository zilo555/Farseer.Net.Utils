﻿using System;
using System.Collections.Generic;
using FS.Cache;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    /// <summary>
    ///     其它扩展，夫归类的扩展
    /// </summary>
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     获取枚举中文
        /// </summary>
        /// <param name="eum">枚举值</param>
        public static string GetName(this Enum eum)
        {
            return EnumNameCacheManger.Cache(eum);
        }

        /// <summary>
        ///     获取枚举列表
        /// </summary>
        public static Dictionary<int, string> ToDictionary(this Type enumType)
        {
            var dic = new Dictionary<int, string>();
            foreach (int value in Enum.GetValues(enumType)) { dic.Add(value, GetName((Enum) Enum.ToObject(enumType, value))); }
            return dic;
        }
    }
}