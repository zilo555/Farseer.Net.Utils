using System;
using FS.Cache;
using FS.Utils;
using FS.Utils.Common;

#if IsWeb
using System.Web.UI;
#endif

namespace FS.Extends
{
    public static class EntityExtend
    {
        /// <summary>
        ///     生成测试数据
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <param name="info">任意对象</param>
        /// <param name="subCount">如果成员包含List类型时，要填充的数量</param>
        public static TEntity FillRandData<TEntity>(this TEntity info, int subCount = 10)
        {
            return FillRandData(info, 1, subCount);
        }

        /// <summary>
        ///     生成测试数据
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <param name="info">任意对象</param>
        /// <param name="subCount">如果成员包含List类型时，要填充的数量</param>
        /// <param name="level">防止无限层</param>
        private static TEntity FillRandData<TEntity>(this TEntity info, int level, int subCount)
        {
            var type = info.GetType();
            foreach (var item in type.GetProperties())
            {
                if (!item.CanWrite) { continue; }

                var argumType = item.PropertyType;
                // 去掉Nullable的类型
                if (argumType.IsGenericType && argumType.GetGenericTypeDefinition() == typeof (Nullable<>)) { argumType = argumType.GetGenericArguments()[0]; }

                try
                {
                    // 对   List 类型处理
                    if (item.PropertyType.IsGenericType)
                    {
                        // 动态构造List
                        var objLst = Activator.CreateInstance(item.PropertyType);
                        // List元素
                        var objItem = Activator.CreateInstance(argumType.GetGenericArguments()[0]);

                        // 防止无限层递归
                        if (level < 3) { for (var i = 0; i < subCount; i++) { item.PropertyType.GetMethod("Add").Invoke(objLst, new[] {FillRandData(objItem, level + 1, subCount)}); } }
                        //item.SetValue(info, objLst, null);
                        PropertySetCacheManger.Cache(item, info, objLst);

                        continue;
                    }
                    // 普通成员
                    //item.SetValue(info, DynamicOperate.FillRandData(argumType), null);
                    PropertySetCacheManger.Cache(item, info, DynamicOperate.FillRandData(argumType));
                }
                catch {
                }
            }
            return info;
        }

        /// <summary>
        ///     设置对象属性值
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="info">当前实体类</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="objValue">要填充的值</param>
        public static void SetValue<TEntity>(this TEntity info, string propertyName, object objValue) where TEntity : class
        {
            if (info == null) { return; }
            foreach (var property in info.GetType().GetProperties())
            {
                if (property.Name != propertyName) { continue; }
                if (!property.CanWrite) { return; }
                //property.SetValue(info, objValue.ConvertType(property.PropertyType), null);
                PropertySetCacheManger.Cache(property, info, objValue.ConvertType(property.PropertyType));
            }
        }

#if IsWeb
    /// <summary>
    ///     将实体类填充到控件中
    /// </summary>
    /// <param name="page">当前页</param>
    /// <param name="info">要填入数据的实体类</param>
    /// <param name="contentPlaceHolderID">母版页面版ID</param>
    /// <param name="prefix">控件前缀</param>
        public static void Fill<TEntity>(this TEntity info, System.Web.UI.Page page, string contentPlaceHolderID, string prefix = "hl") where TEntity : class
        {
            if (info == null)
            {
                return;
            }

            var masterControl = page.FindControl(contentPlaceHolderID);
            if (masterControl == null)
            {
                return;
            }

            Fill(masterControl.Controls, info, prefix);
        }

        /// <summary>
        ///     将实体类填充到控件中
        /// </summary>
        /// <param name="page">当前页</param>
        /// <param name="prefix">控件前缀</param>
        /// <param name="info">要填充的值</param>
        public static void Fill<TEntity>(this TEntity info, System.Web.UI.Page page, string prefix = "hl") where TEntity : class
        {
            if (info == null) { return; }
            Fill(page.Controls, info, prefix);
        }

        /// <summary>
        ///     将实体类填充到控件中
        /// </summary>
        /// <param name="controls">页面控件集合</param>
        /// <param name="infoValue">所属实体类的值</param>
        /// <param name="prefix">前缀</param>
        private static void Fill<TEntity>(ControlCollection controls, TEntity infoValue, string prefix = "hl") where TEntity : class
        {
            //if (infoValue == null || controls == null)
            //{
            //    return;
            //}
            //var map = TableMapCache.GetMap(infoValue);
            //foreach (var kic in map.ModelList)
            //{
            //    // 当前成员的值
            //    var value = kic.Key.GetValue(infoValue, null);
            //    if (value == null) { continue; }

            //    var type = value.GetType();

            //    // 当前成员，是一个类
            //    if (value is ModelInfo)
            //    {
            //        foreach (var item in type.GetProperties()) { Fill(controls, (ModelInfo)value, prefix); }
            //        continue;
            //    }

            //    foreach (Control item in controls)
            //    {
            //        var control = item.FindControl(prefix + kic.Key.Name);
            //        if (control == null) { continue; }

            //        if (control is HiddenField)
            //        {
            //            ((HiddenField)control).Value = value.ToString();
            //            break;
            //        }
            //        if (control is CheckBox) { ((CheckBox)control).Checked = value.ConvertType(false); break; }
            //        if (control is CheckBoxList)
            //        {
            //            // 数据保存的是数字以逗号分隔的数据，并且是ListControl的控件，则可以直接填充数据
            //            if (value is string)
            //            {
            //                var lstIDs = value.ToString().ToList(0);
            //                ((CheckBoxList)control).SetValue(lstIDs);
            //                break;
            //            }

            //            // 枚举为二进制时
            //            var types = kic.Key.PropertyType.GetGenericArguments();
            //            if (types != null && types.Length > 0 && types[0].IsEnum)
            //            {
            //                var att = types[0].GetCustomAttributes(typeof(FlagsAttribute), false);

            //                if (att != null && att.Length > 0)
            //                {
            //                    foreach (ListItem listItem in ((CheckBoxList)control).Items)
            //                    {
            //                        var itemValue = listItem.Value.ConvertType(0);
            //                        listItem.Selected = (value.ConvertType(0) & itemValue) == itemValue;
            //                    }
            //                    break;
            //                }
            //            }

            //        }
            //        if (control is ListControl)
            //        {
            //            ((ListControl)control).SelectedItems(value);
            //            break;

            //        }

            //        if (value is Enum) { value = ((Enum)value).GetName(); }
            //        if (value is IList) { value = ((IList)value).ToString(","); }
            //        if (value is bool) { value = ((bool)value) ? "是" : "否"; }

            //        if (control is TextBox) { ((TextBox)control).Text = value.ToString(); break; }
            //        if (control is Label) { ((Label)control).Text = value.ToString(); break; }
            //        if (control is Literal) { ((Literal)control).Text = value.ToString(); break; }
            //        if (control is Image) { ((Image)control).ImageUrl = value.ToString(); break; }
            //        if (control is HyperLink) { ((HyperLink)control).NavigateUrl = value.ToString(); break; }
            //    }
            //}
        }
#endif

        ///// <summary>
        /////     关联两个实体
        ///// </summary>
        ///// <typeparam name="T1">主实体</typeparam>
        ///// <typeparam name="T2">要附加关联的实体</typeparam>
        ///// <param name="info">主数据</param>
        ///// <param name="JoinModule">要关联的子实体</param>
        ///// <param name="JoinModuleSelect">要附加关联的子实体的字段筛选</param>
        ///// <param name="JoinModuleID">主表关系字段</param>
        ///// <param name="defJoinModule">为空时如何处理？</param>
        ///// <param name="db">事务</param>
        //public static T1 Join<T1, T2>(this T1 info, Expression<Func<T1, T2>> JoinModule, Func<T1, int?> JoinModuleID = null, Expression<Func<T2, object>> JoinModuleSelect = null, T2 defJoinModule = null, DbExecutor db = null)
        //    where T1 : class
        //    where T2 : class
        //{
        //    if (info == null)
        //    {
        //        return null;
        //    }
        //    if (JoinModuleID == null)
        //    {
        //        JoinModuleID = o => o.ID;
        //    }

        //    #region 获取实际类型

        //    var memberExpression = JoinModule.Body as MemberExpression;
        //    // 获取属性类型
        //    var propertyType = (PropertyInfo)memberExpression.Member;

        //    var lstPropery = new List<PropertyInfo>();
        //    while (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
        //    {
        //        memberExpression = memberExpression.Expression as MemberExpression;
        //        lstPropery.Add((PropertyInfo)memberExpression.Member);
        //    }
        //    lstPropery.Reverse();

        //    #endregion

        //    // 详细资料
        //    var subInfo = (new T2()) is BaseCacheModel<T2>
        //                     ? BaseCacheModel<T2>.Cache(db).Where(o => o.ID == JoinModuleID.Invoke(info)).ToEntity()
        //                     : BaseModel<T2>.Data.Where(o => o.ID == JoinModuleID.Invoke(info))
        //                                    .Select(JoinModuleSelect)
        //                                    .ToInfo(db) ?? defJoinModule;

        //    object value = info;
        //    foreach (var propery in lstPropery)
        //    {
        //        value = propery.GetValue(value, null);
        //    }

        //    propertyType.SetValue(value, subInfo, null);

        //    return info;
        //}

        ///// <summary>
        /////     插入数据
        ///// </summary>
        ///// <param name="db">可传入事务的db</param>
        //public static bool Insert<TEntity>(this TEntity info, DbExecutor db = null) where TEntity : class
        //{
        //    if (info is BaseCacheModel<TEntity>) { return BaseCacheModel<TEntity>.Data.Insert(info, db); }
        //    return BaseModel<TEntity>.Data.Insert(info, db);
        //}

        ///// <summary>
        /////     插入数据
        ///// </summary>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="identity">标识，刚插入的ID</param>
        //public static bool Insert<TEntity>(this TEntity info, out int identity, DbExecutor db = null) where TEntity : class
        //{
        //    if ((new TEntity()) is BaseCacheModel<TEntity>) { return BaseCacheModel<TEntity>.Data.Insert(info, out identity, db); }
        //    return BaseModel<TEntity>.Data.Insert(info, out identity, db);
        //}

        ///// <summary>
        /////     更改实体类
        ///// </summary>
        ///// <param name="db">可传入事务的db</param>
        //public static bool Update<TEntity>(this TEntity info, DbExecutor db = null) where TEntity : class
        //{
        //    if ((new TEntity()) is BaseCacheModel<TEntity>) { return BaseCacheModel<TEntity>.Data.Update(info, db); }
        //    return BaseModel<TEntity>.Data.Update(info, db);
        //}

        ///// <summary>
        /////     更新数据
        ///// </summary>
        ///// <param name="where">条件</param>
        ///// <param name="db">可传入事务的db</param>
        //public static bool Update<TEntity>(this TEntity info, Expression<Func<TEntity, bool>> where, DbExecutor db = null) where TEntity : class
        //{
        //    if ((new TEntity()) is BaseCacheModel<TEntity>) { return BaseCacheModel<TEntity>.Data.Where(where).Update(info, db); }
        //    return BaseModel<TEntity>.Data.Where(where).Update(info, db);
        //}

        ///// <summary>
        /////     更改实体类
        ///// </summary>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        //public static bool Update<TEntity>(this TEntity info, int? ID, DbExecutor db = null) where TEntity : class
        //{
        //    if ((new TEntity()) is BaseCacheModel<TEntity>) { return BaseCacheModel<TEntity>.Data.Where(o => o.ID == ID).Update(info, db); }
        //    return BaseModel<TEntity>.Data.Where(o => o.ID == ID).Update(info, db);
        //}

        ///// <summary>
        /////     更改实体类
        ///// </summary>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        //public static bool Update<TEntity>(this TEntity info, List<int> IDs, DbExecutor db = null) where TEntity : class
        //{
        //    if ((new TEntity()) is BaseCacheModel<TEntity>) { return BaseCacheModel<TEntity>.Data.Where(o => IDs.Contains(o.ID)).Update(info, db); }
        //    return BaseModel<TEntity>.Data.Where(o => IDs.Contains(o.ID)).Update(info, db);
        //}

        ///// <summary>
        /////     保存数据
        ///// </summary>
        ///// <param name="reqID">请求ID</param>
        ///// <param name="tip">提示</param>
        ///// <param name="actInsert">插入时的方法委托</param>
        ///// <param name="actUpdate">更新时的方法委托</param>
        ///// <param name="actSuccess">成功后的方法委托</param>
        //public static void Save<TEntity>(this TEntity info, int reqID, Action<string, string> tip = null, Action<TEntity, DbExecutor> actInsert = null, Action<TEntity, DbExecutor> actUpdate = null, Action<int, TEntity, DbExecutor> actSuccess = null) where TEntity : IVerification, IEntity
        //{
        //    if (!info.Check(tip)) { return; }

        //    using (DbExecutor db = typeof(TEntity))
        //    {
        //        if (reqID > 0) { if (actUpdate != null) { actUpdate(info, db); } info.Update(reqID, db); }
        //        else { if (actInsert != null) { actInsert(info, db); } info.Insert(out reqID, db); }
        //        if (actSuccess != null) { actSuccess(reqID, info, db); }
        //        db.Commit();
        //    }
        //}
    }
}