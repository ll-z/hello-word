using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data;

namespace DemonForWorldZhu.DB.Sqlite
{
    public class DataHelper : IDisposable
    {

        #region 字段
        /// <summary>
        /// 用于连数据库的保护对象
        /// </summary>
        protected SQLiteConnection conn;
        /// <summary>
        /// 数据库文件路径
        /// </summary>
        private string _filePath;
        #endregion

        #region 属性
        /// <summary>
        /// mdb文件路径
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        #endregion

        #region 方法

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">mdb文件路径</param>
        public DataHelper(string path)
        {
            this._filePath = path;
        }
        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <returns></returns>
        private bool connOpen()
        {

            string connstr = "Initial Catalog=sqlite;Data Source=" + _filePath + ";Integrated Security=True;";
            bool _ok = false;
            if (conn == null)
            {
                conn = new SQLiteConnection(connstr);
                conn.SetPassword("13117006");
                try
                {

                    conn.Open();

                    _ok = true;
                }
                catch (Exception ex)
                {
                    LogHelperEx.LoggerMain.Error("打开数据库时：", ex);
                    MessageBox.Show("打开数据库时：\n\n" + ex.Message,
                        "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            else if (conn.State == System.Data.ConnectionState.Closed)
            {
                try
                {
                    conn.Open();
                    _ok = true;
                }
                catch (Exception ex1)
                {
                    LogHelperEx.LoggerMain.Error("打开数据库时：", ex1);
                    MessageBox.Show("打开数据库时：\n\n" + ex1.Message,
                        "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            else if (conn.State == System.Data.ConnectionState.Open) { _ok = true; }
            return _ok;

        }
        /// <summary>
        /// 执行数据库命令
        /// </summary>
        /// <param name="cmdstr">sql语句</param>
        /// <returns>执行结果</returns>
        public bool ExCmd(string cmdstr, bool showerr = false)
        {
            bool runOK = false;
            if (!connOpen()) { return false; }
            using (SQLiteCommand cmd = new SQLiteCommand(cmdstr, conn))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                    runOK = true;
                }
                catch (Exception ex)
                {
                    if (showerr)
                        MessageBox.Show("执行数据库命令时：\n\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return runOK;
        }
        /// <summary>
        /// 执行图片数据命令
        /// </summary>
        /// <param name="cmdstr">命令</param>
        /// <param name="data">图片数据</param>
        /// <param name="showerr">是否显示错误</param>
        /// <returns></returns>
        public bool ExCmd(string cmdstr, byte[] data, bool showerr = false)
        {
            if (!connOpen()) { return false; }
            bool runOK = false;
            using (SQLiteCommand cmd = new SQLiteCommand(cmdstr, conn))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@pic", data);
                    cmd.ExecuteNonQuery();
                    runOK = true;
                }
                catch (Exception ex)
                {
                    if (showerr)
                        MessageBox.Show("执行数据库命令时：\n\n" + ex.Message,
                            "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return runOK;
        }
        /// <summary>
        /// 执行命令组
        /// </summary>
        /// <param name="cmdstrs">sql命令组</param>
        /// <returns>执行结果</returns>
        public bool ExCmd(string[] cmdstrs, bool showerr = false)
        {
            if (!connOpen()) { return false; }
            bool runOK = false;
            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteTransaction trans = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = trans;
            int count = 0;
            try
            {
                foreach (string str in cmdstrs)
                {
                    cmd.CommandText = str;
                    cmd.ExecuteNonQuery();
                    count++;
                }
                trans.Commit();
                runOK = true;
            }
            catch (Exception ex)
            {
                if (showerr)
                    MessageBox.Show("执行数据库命令组时：\n\n" + ex.Message,
                            "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                trans.Rollback();
            }
            finally
            {
                trans.Dispose(); cmd.Dispose();
            }
            return runOK;
        }
        /// <summary>
        /// 获取数据库表单
        /// </summary>
        /// <param name="cmdstr">sql命令</param>
        /// <param name="showerr">是否显示错误提示</param>
        /// <returns>得到的datatable</returns>
        public DataTable GetDataTable(string cmdstr, bool showerr = false)
        {
            DataTable dt = new DataTable();
            if (!connOpen()) { return null; }
            using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmdstr, conn))
            {
                try
                {
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    if (showerr)
                        MessageBox.Show("获取DATATABLE时：\n\n" + ex.Message,
                            "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dt = null;
                }
            }
            return dt;
        }
        /// <summary>
        /// 检查某一项是否存在
        /// </summary>
        /// <param name="cmdstr">sql命令</param>
        /// <param name="showerr">是否显示错误</param>
        /// <returns>返回结果</returns>
        public bool CheckExist(string cmdstr, bool showerr = false)
        {
            if (!connOpen()) { return false; }
            bool exist = false;
            using (SQLiteCommand cmd = new SQLiteCommand(cmdstr, conn))
            {
                try
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    exist = reader.Read() ? true : false;
                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    if (showerr)
                        MessageBox.Show("检查数据值存在时：\n\n" + ex.Message,
                            "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return exist;
        }
        /// <summary>
        /// 得到某一项的值
        /// </summary>
        /// <param name="cmdstr">sql命令</param>
        /// <param name="showerr">是否显示错误</param>
        /// <returns>返回结果</returns>
        public object GetValueObject(string cmdstr, bool showerr = false)
        {
            if (!connOpen()) { return null; }
            object fvalue = null;
            using (SQLiteCommand cmd = new SQLiteCommand(cmdstr, conn))
            {
                try
                {
                    fvalue = cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    if (showerr)
                        MessageBox.Show("获取数据库值时：\n\n" + ex.Message,
                            "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return fvalue;
        }
        /// <summary>
        /// 保存dataGridView控件数据到数据库
        /// </summary>
        /// <param name="cmdstr">sql语句</param>
        /// <param name="dgv">数据控件</param>
        /// <param name="IsSahow">显示成功对话框</param>
        public void SaveDataGridViewToData(string cmdstr, DataGridView dgv, bool IsSahow = true)
        {
            if (!connOpen()) { return; }
            dgv.CurrentCell = null;
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmdstr, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
            try
            {
                da.Update((DataTable)dgv.DataSource);
                if (IsSahow)
                {
                    MessageBox.Show("保存数据成功！", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                dgv.DataSource = GetDataTable(cmdstr);
            }
            catch (Exception ex)
            {

                MessageBox.Show("保存数据失败！\n\n" + ex.Message, "错误",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cb.Dispose();
                ds.Dispose();
                da.Dispose();
            }
        }
        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
            if (conn != null)
            {
                conn.Close(); conn.Dispose(); conn = null;
            }
        }
        /// <summary>
        /// 从数据库获取数据到List
        /// </summary>
        /// <param name="cmdstr">sql语句</param>
        public List<string> GetDataToList(string cmdstr)
        {
            List<string> resultLists = new List<string>();
            if (!connOpen()) { return resultLists; }
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmdstr, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    resultLists.Add(dt.Rows[i][0].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取数据到combox失败！\n\n" + ex.Message, "错误",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dt.Dispose();
                da.Dispose();
            }
            return resultLists;
        }
        /// <summary>
        /// 从数据库获取数据到List
        /// </summary>
        /// <param name="tableName">表单名名称</param>
        /// <param name="filedName">字段名称</param>
        /// <param name="whereStr">条件字符</param>
        /// <param name="repair">是否过滤重复</param>
        /// <returns>数据列表</returns>
        public List<string> GetDataToList(string tableName, string filedName, string whereStr = "", bool repair = false)
        {
            List<string> resultLists = new List<string>();
            if (!connOpen()) { return resultLists; }
            string cmdstr = string.Format("select {0} from {1}", filedName, tableName);
            if (repair)
            {
                cmdstr = string.Format("select DISTINCT {0} from {1}", filedName, tableName);
            }
            if (whereStr != "")
            {
                cmdstr += string.Format(" where {0}", whereStr);
            }
            resultLists = GetDataLists(cmdstr);
            return resultLists;
        }

        public List<string> GetDataLists(string cmdstr)
        {
            List<string> datalists = new List<string>();
            if (!connOpen()) { return null; }
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmdstr, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    datalists.Add(dt.Rows[i][0].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取数据到List失败！\n\n" + ex.Message, "错误",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dt.Dispose();
                da.Dispose();
            }
            return datalists;
        }

        /// <summary>
        /// 创建表单
        /// </summary>
        /// <param name="tableName">创建表单名称</param>
        /// <param name="filedName">创建表单字段名</param>
        /// <returns></returns>
        public bool CreateNewTables(string tableName, string[] filedName)
        {
            if (tableName == "" || filedName.Length == 0) { return false; }
            //ClearCurTables();//清除当前tables
            //创建新Table
            string cmdstr = string.Format("create table {0}", tableName);
            if (ExCmd(cmdstr))
            {
                //创建字段
                List<string> cmds = new List<string>();
                for (int i = 0; i < filedName.Length; i++)
                {
                    if (filedName[i] == "SN")
                    {
                        cmdstr = string.Format("alter table {0} add column {1} Memo primary key",
                        tableName, filedName[i]);
                    }
                    else
                    {
                        cmdstr = string.Format("alter table {0} add column {1} Memo",
                       tableName, filedName[i]);
                    }

                    cmds.Add(cmdstr);
                }
                if (ExCmd(cmds.ToArray()))
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                MessageBox.Show("创建表单失败-" + tableName, "错误",
               MessageBoxButtons.OK, MessageBoxIcon.Error); return false;
            }
        }

        /// <summary>
        /// 取指定表所有字段名称
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public List<string> GetTableFieldNameList(string TableName)
        {
            List<string> list = new List<string>();
            try
            {
                if (!connOpen()) { return null; }

                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.CommandText = string.Format("SELECT TOP 1 * FROM {0}", TableName);
                    cmd.Connection = conn;
                    SQLiteDataReader dr = cmd.ExecuteReader();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        list.Add(dr.GetName(i));
                    }
                }
                return list;
            }
            catch
            { return null; }
        }
        public void GetDataToCombox(string cmdstr, ComboBox cmb)
        {
            if (!connOpen()) { return; }
            cmb.Items.Clear();
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmdstr, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cmb.Items.Add(dt.Rows[i][0].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取数据到combox失败！\n\n" + ex.Message, "错误",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dt.Dispose();
                da.Dispose();
            }
        }
        #endregion
    }
}
