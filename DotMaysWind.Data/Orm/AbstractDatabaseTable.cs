﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using DotMaysWind.Data.Command;
using DotMaysWind.Data.Command.Function;
using DotMaysWind.Data.Helper;

namespace DotMaysWind.Data.Orm
{
    /// <summary>
    /// 抽象数据库表类
    /// </summary>
    /// <typeparam name="T">数据表实体</typeparam>
    /// <example>
    /// <code lang="C#">
    /// <![CDATA[
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Data;
    /// 
    /// using DotMaysWind.Data;
    /// using DotMaysWind.Data.Orm;
    /// 
    /// public class User
    /// {
    ///     public Int32 UserID { get; set; }
    ///     public String UserName { get; set; }
    /// }
    /// 
    /// public class UserDataProvider : AbstractDatabaseTable<User>
    /// {
    ///     private const String UserIDColumn = "UserID";
    ///     private const String UserNameColumn = "UserName";
    /// 
    ///     public UserDataProvider()
    ///         : base(MainDatabase.Instance) { }
    /// 
    ///     public override String TableName
    ///     {
    ///         get { return "tbl_Users"; }
    ///     }
    /// 
    ///     protected override User CreateEntity(DataRow row, DataColumnCollection columns)
    ///     {
    ///         User entity = new User();
    /// 
    ///         entity.UserID = this.LoadInt32(row, columns, UserIDColumn);
    ///         entity.UserName = this.LoadString(row, columns, UserNameColumn);
    /// 
    ///         return entity;
    ///     }
    /// 
    ///     public Boolean InsertEntity(User user)
    ///     {
    ///         return this.Insert()
    ///             .Add(UserIDColumn, user.UserID)
    ///             .Add(UserNameColumn, user.UserName)
    ///             .Result() > 0;
    ///     }
    /// 
    ///     public Boolean UpdateEntity(User user)
    ///     {
    ///         return this.Update()
    ///             .Set(UserNameColumn, user.UserName)
    ///             .Where(c => c.Equal(UserIDColumn, user.UserID))
    ///             .Result() > 0;
    ///     }
    /// 
    ///     public Boolean DeleteEntity(Int32 userID)
    ///     {
    ///         return this.Delete()
    ///             .Where(c => c.Equal(UserIDColumn, userID))
    ///             .Result() > 0;
    ///     }
    /// 
    ///     public List<User> GetAllEntities()
    ///     {
    ///         return this.Select()
    ///             .Querys(UserIDColumn, UserNameColumn)
    ///             .ToEntityList<User>(this);
    ///     }
    /// }
    /// 
    /// internal static class MainDatabase
    /// {
    ///     private static IDatabase _database;
    /// 
    ///     internal static IDatabase Instance
    ///     {
    ///         get { return _database; }
    ///     }
    /// 
    ///     static MainDatabase()
    ///     {
    ///         _database = DatabaseFactory.CreateDatabase("MainDatabase");
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public abstract class AbstractDatabaseTable<T> : IDatabaseTable  where T : class
    {
        #region 字段
        private IDatabase _baseDatabase;
        #endregion

        #region 抽象属性方法
        /// <summary>
        /// 获取数据表名
        /// </summary>
        public abstract String TableName { get; }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="sender">请求SQL语句</param>
        /// <param name="args">相关参数</param>
        /// <returns>数据表实体</returns>
        protected abstract T CreateEntity(Object sender, EntityCreatingArgs args);
        #endregion

        #region 属性
        /// <summary>
        /// 获取数据表所在数据库
        /// </summary>
        protected IDatabase Database
        {
            get { return this._baseDatabase; }
        }

        /// <summary>
        /// 获取Sql数据库支持的函数
        /// </summary>
        public SqlFunctions Functions
        {
            get { return this._baseDatabase.Functions; }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 初始化新的抽象数据库表
        /// </summary>
        /// <param name="baseDatabase">数据表所在数据库</param>
        /// <exception cref="ArgumentNullException">数据库不能为空</exception>
        protected AbstractDatabaseTable(IDatabase baseDatabase)
        {
            if (baseDatabase == null)
            {
                throw new ArgumentNullException("baseDatabase");
            }

            this._baseDatabase = baseDatabase;
        }
        #endregion

        #region CreateSqlCommand
        /// <summary>
        /// 创建新的Sql插入语句类
        /// </summary>
        /// <returns>Sql插入语句</returns>
        protected virtual InsertCommand Insert()
        {
            return this._baseDatabase.CreateInsertCommand(this.TableName);
        }

        /// <summary>
        /// 创建新的Sql更新语句类
        /// </summary>
        /// <returns>Sql更新语句</returns>
        protected virtual UpdateCommand Update()
        {
            return this._baseDatabase.CreateUpdateCommand(this.TableName);
        }

        /// <summary>
        /// 创建新的Sql删除语句类
        /// </summary>
        /// <returns>Sql删除语句</returns>
        protected virtual DeleteCommand Delete()
        {
            return this._baseDatabase.CreateDeleteCommand(this.TableName);
        }

        /// <summary>
        /// 创建新的Sql选择语句类
        /// </summary>
        /// <returns>Sql选择语句</returns>
        protected virtual SelectCommand Select()
        {
            return this._baseDatabase.CreateSelectCommand(this.TableName);
        }

        /// <summary>
        /// 创建新的Sql选择语句类
        /// </summary>
        /// <param name="tableAliasesName">数据表别名</param>
        /// <returns>Sql选择语句</returns>
        protected virtual SelectCommand Select(String tableAliasesName)
        {
            return this._baseDatabase.CreateSelectCommand(this.TableName, tableAliasesName);
        }
        #endregion

        #region UsingConnection/Transaction
        /// <summary>
        /// 使用持续数据库连接执行操作
        /// </summary>
        /// <param name="action">使用持续连接的操作</param>
        protected void UsingConnection(Action<DbConnection> action)
        {
            this._baseDatabase.UsingConnection(action);
        }

        /// <summary>
        /// 使用持续数据库连接执行操作
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="function">使用持续连接的操作</param>
        /// <returns>内部返回内容</returns>
        protected TResult UsingConnection<TResult>(Func<DbConnection, TResult> function)
        {
            return this._baseDatabase.UsingConnection<TResult>(function);
        }

        /// <summary>
        /// 使用数据库事务执行操作
        /// </summary>
        /// <param name="action">使用事务的操作</param>
        protected void UsingTransaction(Action<DbTransaction> action)
        {
            this._baseDatabase.UsingTransaction(action);
        }

        /// <summary>
        /// 使用数据库事务执行操作
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="function">使用事务的操作</param>
        /// <returns>内部返回内容</returns>
        protected TResult UsingTransaction<TResult>(Func<DbTransaction, TResult> function)
        {
            return this._baseDatabase.UsingTransaction<TResult>(function);
        }
        #endregion

        #region UsingDataReader
        /// <summary>
        /// 使用数据库读取器执行操作
        /// </summary>
        /// <param name="command">指定Sql语句</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="action">使用数据库读取器的操作</param>
        protected void UsingDataReader(ISqlCommand command, DbTransaction transaction, Action<IDataReader> action)
        {
            this._baseDatabase.UsingDataReader(command, transaction, action);
        }

        /// <summary>
        /// 使用数据库读取器执行操作
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="command">指定Sql语句</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="function">使用数据库读取器的操作</param>
        /// <returns>返回的内容</returns>
        protected TResult UsingDataReader<TResult>(ISqlCommand command, DbTransaction transaction, Func<IDataReader, TResult> function)
        {
            return this._baseDatabase.UsingDataReader<TResult>(command, transaction, function);
        }

        /// <summary>
        /// 使用数据库读取器执行操作
        /// </summary>
        /// <param name="command">指定Sql语句</param>
        /// <param name="connection">数据库连接</param>
        /// <param name="action">使用数据库读取器的操作</param>
        protected void UsingDataReader(ISqlCommand command, DbConnection connection, Action<IDataReader> action)
        {
            this._baseDatabase.UsingDataReader(command, connection, action);
        }

        /// <summary>
        /// 使用数据库读取器执行操作
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="command">指定Sql语句</param>
        /// <param name="connection">数据库连接</param>
        /// <param name="function">使用数据库读取器的操作</param>
        /// <returns>返回的内容</returns>
        protected TResult UsingDataReader<TResult>(ISqlCommand command, DbConnection connection, Func<IDataReader, TResult> function)
        {
            return this._baseDatabase.UsingDataReader<TResult>(command, connection, function);
        }

        /// <summary>
        /// 使用数据库读取器执行操作
        /// </summary>
        /// <param name="command">指定Sql语句</param>
        /// <param name="action">使用数据库读取器的操作</param>
        protected void UsingDataReader(ISqlCommand command, Action<IDataReader> action)
        {
            this._baseDatabase.UsingDataReader(command, action);
        }

        /// <summary>
        /// 使用数据库读取器执行操作
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="command">指定Sql语句</param>
        /// <param name="function">使用数据库读取器的操作</param>
        /// <returns>返回的内容</returns>
        protected TResult UsingDataReader<TResult>(ISqlCommand command, Func<IDataReader, TResult> function)
        {
            return this._baseDatabase.UsingDataReader<TResult>(command, function);
        }
        #endregion

        #region LoadValue
        #region 不可空
        /// <summary>
        /// 读取布尔型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>布尔型结果</returns>
        protected Boolean LoadBoolean(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToBoolean(row[columnName]);
            }
            else
            {
                return default(Boolean);
            }
        }

        /// <summary>
        /// 读取字节型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>字节型结果</returns>
        protected Char LoadChar(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToChar(row[columnName]);
            }
            else
            {
                return default(Char);
            }
        }

        /// <summary>
        /// 读取字节型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>字节型结果</returns>
        protected Byte LoadByte(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToByte(row[columnName]);
            }
            else
            {
                return default(Byte);
            }
        }

        /// <summary>
        /// 读取有符号字节型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>有符号字节型结果</returns>
        protected SByte LoadSByte(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToSByte(row[columnName]);
            }
            else
            {
                return default(SByte);
            }
        }

        /// <summary>
        /// 读取2字节整型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>2字节整型结果</returns>
        protected Int16 LoadInt16(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToInt16(row[columnName]);
            }
            else
            {
                return default(Int16);
            }
        }

        /// <summary>
        /// 读取2字节无符号整型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>2字节无符号整型结果</returns>
        protected UInt16 LoadUInt16(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToUInt16(row[columnName]);
            }
            else
            {
                return default(UInt16);
            }
        }

        /// <summary>
        /// 读取4字节整型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>4字节整型结果</returns>
        protected Int32 LoadInt32(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToInt32(row[columnName]);
            }
            else
            {
                return default(Int32);
            }
        }

        /// <summary>
        /// 读取4字节无符号整型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>4字节无符号整型结果</returns>
        protected UInt32 LoadUInt32(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToUInt32(row[columnName]);
            }
            else
            {
                return default(UInt32);
            }
        }

        /// <summary>
        /// 读取8字节整型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>8字节整型结果</returns>
        protected Int64 LoadInt64(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToInt64(row[columnName]);
            }
            else
            {
                return default(Int64);
            }
        }

        /// <summary>
        /// 读取8字节无符号整型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>8字节无符号整型结果</returns>
        protected UInt64 LoadUInt64(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToUInt64(row[columnName]);
            }
            else
            {
                return default(UInt64);
            }
        }

        /// <summary>
        /// 读取单精度浮点值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>单精度浮点型结果</returns>
        protected Single LoadSingle(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToSingle(row[columnName]);
            }
            else
            {
                return default(Single);
            }
        }

        /// <summary>
        /// 读取双精度浮点值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>双精度浮点型结果</returns>
        protected Double LoadDouble(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToDouble(row[columnName]);
            }
            else
            {
                return default(Double);
            }
        }

        /// <summary>
        /// 读取十进制值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>十进制型结果</returns>
        protected Decimal LoadDecimal(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToDecimal(row[columnName]);
            }
            else
            {
                return default(Decimal);
            }
        }

        /// <summary>
        /// 读取日期型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>日期型结果</returns>
        protected DateTime LoadDateTime(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToDateTime(row[columnName]);
            }
            else
            {
                return default(DateTime);
            }
        }

        /// <summary>
        /// 读取日期型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>日期型结果</returns>
        protected DateTimeOffset LoadDateTimeOffset(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToDateTimeOffset(row[columnName]);
            }
            else
            {
                return default(DateTimeOffset);
            }
        }

        /// <summary>
        /// 读取Guid值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>Guid结果</returns>
        protected Guid LoadGuid(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToGuid(row[columnName]);
            }
            else
            {
                return default(Guid);
            }
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>字符串结果</returns>
        protected String LoadString(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToString(row[columnName]);
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 读取指定类型数据
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <param name="dbType">数据类型</param>
        /// <returns>指定类型数据</returns>
        protected Object LoadValue(DataRow row, DataColumnCollection columns, String columnName, DbType dbType)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToValue(row[columnName], dbType);
            }
            else
            {
                return DbConvert.GetDefaultValue(dbType);
            }
        }

        /// <summary>
        /// 读取指定类型数据
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <param name="dbType">数据类型</param>
        /// <typeparam name="TValue">指定类型</typeparam>
        /// <returns>指定类型数据</returns>
        protected TValue LoadValue<TValue>(DataRow row, DataColumnCollection columns, String columnName, DbType dbType)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return (TValue)DbConvert.ToValue(row[columnName], dbType);
            }
            else
            {
                return default(TValue);
            }
        }

        /// <summary>
        /// 读取指定类型数据
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <typeparam name="TValue">指定类型</typeparam>
        /// <returns>指定类型数据</returns>
        protected TValue LoadValue<TValue>(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                Object value = row[columnName];
                DbType dbType = DbTypeHelper.InternalGetDbType(value);

                return (TValue)DbConvert.ToValue(value, dbType);
            }
            else
            {
                return default(TValue);
            }
        }
        #endregion

        #region 可空
        /// <summary>
        /// 读取可空布尔型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>布尔型结果</returns>
        protected Boolean? LoadNullableBoolean(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToBoolean(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空字节型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>字节型结果</returns>
        protected Char? LoadNullableChar(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToChar(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空字节型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>字节型结果</returns>
        protected Byte? LoadNullableByte(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToByte(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空有符号字节型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>有符号字节型结果</returns>
        protected SByte? LoadNullableSByte(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToSByte(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空2字节整型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>2字节整型结果</returns>
        protected Int16? LoadNullableInt16(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToInt16(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空2字节无符号整型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>2字节无符号整型结果</returns>
        protected UInt16? LoadNullableUInt16(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToUInt16(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空4字节整型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>4字节整型结果</returns>
        protected Int32? LoadNullableInt32(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToInt32(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空4字节无符号整型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>4字节无符号整型结果</returns>
        protected UInt32? LoadNullableUInt32(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToUInt32(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空8字节整型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>8字节整型结果</returns>
        protected Int64? LoadNullableInt64(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToInt64(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空8字节无符号整型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>8字节无符号整型结果</returns>
        protected UInt64? LoadNullableUInt64(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToUInt64(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空单精度浮点值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>单精度浮点型结果</returns>
        protected Single? LoadNullableSingle(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToSingle(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空双精度浮点值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>双精度浮点型结果</returns>
        protected Double? LoadNullableDouble(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToDouble(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空十进制值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>十进制型结果</returns>
        protected Decimal? LoadNullableDecimal(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToDecimal(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空日期型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>日期型结果</returns>
        protected DateTime? LoadNullableDateTime(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToDateTime(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空日期型值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>日期型结果</returns>
        protected DateTimeOffset? LoadNullableDateTimeOffset(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToDateTimeOffset(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取可空Guid值
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>Guid结果</returns>
        protected Guid? LoadNullableGuid(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToGuid(row[columnName]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取字节数组
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <returns>字节数组结果</returns>
        protected Byte[] LoadNullableBytes(DataRow row, DataColumnCollection columns, String columnName)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return row[columnName] as Byte[];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取指定类型数据
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <param name="dbType">数据类型</param>
        /// <returns>指定类型数据</returns>
        protected Object LoadNullableValue(DataRow row, DataColumnCollection columns, String columnName, DbType dbType)
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return DbConvert.ToValue(row[columnName], dbType);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取指定类型数据
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="columns">列集合</param>
        /// <param name="columnName">列名称</param>
        /// <param name="dbType">数据类型</param>
        /// <typeparam name="TValue">指定类型</typeparam>
        /// <returns>指定类型数据</returns>
        protected Nullable<TValue> LoadNullableValue<TValue>(DataRow row, DataColumnCollection columns, String columnName, DbType dbType) where TValue: struct
        {
            if (columns.Contains(columnName) && !Convert.IsDBNull(row[columnName]))
            {
                return (TValue)DbConvert.ToValue(row[columnName], dbType);
            }
            else
            {
                return null;
            }
        }
        #endregion
        #endregion

        #region 内部方法
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="sender">请求SQL语句</param>
        /// <param name="table">数据表</param>
        /// <param name="extraArg">创建实体时的额外参数</param>
        /// <returns>数据表实体</returns>
        internal T GetEntityInternal(Object sender, DataTable table, Object extraArg)
        {
            if (!DbConvert.IsDataTableNullOrEmpty(table))
            {
                return this.GetEntityInternal(sender, table.Rows[0], extraArg);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="sender">请求SQL语句</param>
        /// <param name="row">数据行</param>
        /// <param name="extraArg">创建实体时的额外参数</param>
        /// <returns>数据表实体</returns>
        internal T GetEntityInternal(Object sender, DataRow row, Object extraArg)
        {
            if (row != null)
            {
                T entity = this.CreateEntity(sender, new EntityCreatingArgs(row, row.Table.Columns, extraArg));

                return entity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="sender">请求SQL语句</param>
        /// <param name="table">数据表</param>
        /// <param name="args">创建实体时的额外参数</param>
        /// <returns>实体列表</returns>
        internal List<T> GetEntityListInternal(Object sender, DataTable table, Object args)
        {
            List<T> list = null;

            if (!DbConvert.IsDataTableNull(table))
            {
                list = new List<T>();

                for (Int32 i = 0; i < table.Rows.Count; i++)
                {
                    T entity = this.GetEntityInternal(sender, table.Rows[i], args);

                    list.Add(entity);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="sender">请求SQL语句</param>
        /// <param name="table">数据表</param>
        /// <param name="keyColumnName">键列名称</param>
        /// <param name="args">创建实体时的额外参数</param>
        /// <returns>实体列表</returns>
        internal Dictionary<TKey, T> GetEntityDictionaryInternal<TKey>(Object sender, DataTable table, String keyColumnName, Object args)
        {
            Dictionary<TKey, T> dict = null;

            if (!DbConvert.IsDataTableNull(table))
            {
                dict = new Dictionary<TKey, T>();

                for (Int32 i = 0; i < table.Rows.Count; i++)
                {
                    TKey key = this.LoadValue<TKey>(table.Rows[i], table.Columns, keyColumnName);
                    T entity = this.GetEntityInternal(sender, table.Rows[i], args);

                    dict[key] = entity;
                }
            }

            return dict;
        }
        #endregion

        #region 重载方法
        /// <summary>
        /// 返回当前对象的信息
        /// </summary>
        /// <returns>当前对象的信息</returns>
        public override String ToString()
        {
            return String.Format("{0}, {1}", base.ToString(), this.TableName);
        }
        #endregion
    }
}