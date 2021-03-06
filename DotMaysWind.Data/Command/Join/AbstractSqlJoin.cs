﻿using System;

namespace DotMaysWind.Data.Command.Join
{
    /// <summary>
    /// Sql连接语句抽象类
    /// </summary>
    public abstract class AbstractSqlJoin : ISqlJoin
    {
        #region 字段
        /// <summary>
        /// 连接类型
        /// </summary>
        protected SqlJoinType _joinType;

        /// <summary>
        /// 当前表名
        /// </summary>
        protected String _currentTableName;

        /// <summary>
        /// 当前表格主键
        /// </summary>
        protected String _currentTableKeyField;

        /// <summary>
        /// 连接表格主键
        /// </summary>
        protected String _anotherTableKeyField;
        #endregion

        #region 属性
        /// <summary>
        /// 获取连接模式
        /// </summary>
        public abstract SqlJoinMode JoinMode { get; }

        /// <summary>
        /// 获取连接语句类型
        /// </summary>
        public SqlJoinType JoinType
        {
            get { return this._joinType; }
        }

        /// <summary>
        /// 获取当前表格名称
        /// </summary>
        public String CurrentTableName
        {
            get { return this._currentTableName; }
        }

        /// <summary>
        /// 获取或设置当前表格主键
        /// </summary>
        public String CurrentTableKeyField
        {
            get { return this._currentTableKeyField; }
            set { this._currentTableKeyField = value; }
        }

        /// <summary>
        /// 获取或设置另个表格主键
        /// </summary>
        public String AnotherTableKeyField
        {
            get { return this._anotherTableKeyField; }
            set { this._anotherTableKeyField = value; }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 初始化Sql连接语句抽象类
        /// </summary>
        /// <param name="baseCommand">源选择语句</param>
        /// <param name="joinType">连接模式</param>
        /// <param name="currentTableName">当前表格名称</param>
        /// <param name="currentTableField">当前表格主键</param>
        /// <param name="anotherTableField">另个表格主键</param>
        protected AbstractSqlJoin(SelectCommand baseCommand, SqlJoinType joinType, String currentTableName, String currentTableField, String anotherTableField)
        {
            this._joinType = joinType;
            this._currentTableName = currentTableName;
            this._currentTableKeyField = currentTableField;
            this._anotherTableKeyField = anotherTableField;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获取所有参数集合
        /// </summary>
        /// <returns>所有参数集合</returns>
        public virtual DataParameter[] GetAllParameters()
        {
            return null;
        }

        /// <summary>
        /// 获取连接语句内容
        /// </summary>
        /// <returns>连接语句内容</returns>
        public abstract String GetClauseText();
        #endregion

        #region 重载方法
        /// <summary>
        /// 返回当前对象的信息
        /// </summary>
        /// <returns>当前对象的信息</returns>
        public override String ToString()
        {
            return String.Format("{0}, {1}", base.ToString(), this.GetClauseText());
        }
        #endregion
    }
}