using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace API
{

    class DBAgentSql
    {
        //private CIni CiniObj;
        //string m_strSettingPath;

        public DBAgentSql(IConfiguration configuration)
        {
             //CiniObj = new CIni(SettingFilePath);
            //string strSection = "ERPDB";
            IP =configuration["DB:IP"];
            PORT = configuration["DB:PORT"];
            UID = configuration["DB:UID"];
            PWD = configuration["DB:PWD"]; 
            TableUserName = configuration["DB:TableUserName"]; 
            //m_strSettingPath = SettingFilePath;
            //LoadDefaultSetting();
        }

        public string m_ConnectionString
        {
            get
            {
                return string.Format("Data Source ={0}; Initial Catalog = {3}; Persist Security Info = True; User ID = {1}; Password ={2}; ", IP, UID, PWD, TableUserName);
            }
        }

        internal Boolean ExecuteNonQueryNoCatch_Sql(string sql, int timeout = 300)
        {
            Boolean bSuccess = true;
            SqlConnection connection = null;
            SqlCommand command = null;
            Exception exp = null;

            try
            {
                connection = new SqlConnection(m_ConnectionString);
                connection.Open();
                command = new SqlCommand(sql, connection);
                command.CommandTimeout = timeout;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                bSuccess = false;
                exp = ex;
                //MessageBox.Show("QueryReturnDT Error!! strGlobalCon: " + strGlobalCon + "\nException:" + e.ToString());

                //ProgramLog.WriteLineLog(string.Format("ExecuteNonQueryNoCatch Exception!! SQL: {0}, Msg: {1}", sql, ex.Message));
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }

            return bSuccess;
        }

        internal DataTable QueryReturnDTNoCatch(String sql, int timeout = 300)
        {
            String strErrorMsg = String.Empty;
            DataTable res = new DataTable();
            SqlConnection connection = null;
            SqlDataAdapter adapter = null;
            Boolean bSuccess = true;
            Exception exp = null;

            try
            {
                connection = new SqlConnection(m_ConnectionString);
                adapter = new SqlDataAdapter(sql, connection);
                adapter.SelectCommand.CommandTimeout = timeout;
                adapter.Fill(res);
            }
            catch (Exception ex)
            {
                res.Clear();
                //MessageBox.Show("QueryReturnDT Error!! m_ConnectionString: " + m_ConnectionString + "\nException:" + ex.ToString());
                strErrorMsg = String.Empty;
                strErrorMsg = ex.ToString();
                bSuccess = false;
                exp = ex;

                if (!sql.Contains("1=0"))
                {
                    //ProgramLog.WriteLineLog(string.Format("QueryReturnDTNoCatch Exception!! SQL: {0}, Msg: {1}", sql, ex.Message));
                }
            }
            finally
            {
                if (adapter != null)
                {
                    adapter.Dispose();
                }

                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }

            if (!bSuccess)
            {
                throw exp;
            }

            return res;
        }

        private string SettingFilePath
        {
            get
            {
                return (string.IsNullOrEmpty(GetCurrentDllFloderPath()) ? "system.ini" : GetCurrentDllFloderPath() + @"\system.ini");
            }
        }

        public static string GetCurrentDllFloderPath()
        {
            var dllPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            var dllFloderPath = Path.GetDirectoryName(dllPath).Replace("file:\\", string.Empty);

            return dllFloderPath;
        }

        // public void LoadDefaultSetting()
        // {
        //     CiniObj = new CIni(SettingFilePath);
        //     string strSection = "ERPDB";
        //     IP = CiniObj.ReadValue(strSection, "IP");
        //     PORT = CiniObj.ReadValue(strSection, "PORT");
        //     UID = CiniObj.ReadValue(strSection, "UID");
        //     PWD = CiniObj.ReadValue(strSection, "PWD");
        //     TableUserName = CiniObj.ReadValue(strSection, "TableUserName");
        // }

        public string IP
        {
            get;
            set;
        }

        public string PORT
        {
            get;
            set;
        }

        public string UID
        {
            get;
            set;
        }

        public string PWD
        {
            get;
            set;
        }

        public string StartTime
        {
            get;
            set;
        }

        public string EndTime
        {
            get;
            set;
        }

        public string TableUserName
        {
            get;
            set;
        }
    }
}

