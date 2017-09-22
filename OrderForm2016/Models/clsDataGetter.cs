using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace OrderForm2016.Models
{
	public class clsDataGetter:IDisposable
	{
		public SqlConnection conn;
		string cnStr;

		public clsDataGetter(string connStr)
		{
			conn = new System.Data.SqlClient.SqlConnection(connStr);
			cnStr = connStr;
		}

		public System.Data.SqlClient.SqlDataReader GetDataReader(string sql, SqlConnection newConn = null)
		{
			System.Data.SqlClient.SqlDataReader dr = null;
			if (newConn == null)
			{
				newConn = conn;
			}

			if (newConn.State != ConnectionState.Open && newConn.State != ConnectionState.Connecting)
			{
				newConn.Open();
			}

			System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, newConn);
			cmd.CommandTimeout = 3600;
			try
			{
				dr = cmd.ExecuteReader();
			}
			catch (Exception ex)
			{
				newConn.Close();
			}
			return dr;
		}

        public System.Data.SqlClient.SqlDataReader GetDataReaderSP(string sql, List<SqlParameter> parameters, SqlConnection newConn = null)
        {
            System.Data.SqlClient.SqlDataReader dr = null;
            if (newConn == null)
            {
                newConn = conn;
            }

            if (newConn.State != ConnectionState.Open && newConn.State != ConnectionState.Connecting)
            {
                newConn.Open();
            }

            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, newConn);
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (var parm in parameters)
            {
                cmd.Parameters.Add(parm);
            }
            cmd.CommandTimeout = 3600;
            try
            {
                dr = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                newConn.Close();
            }
            return dr;
        }


        public void KillReader(SqlDataReader dr)
		{
			dr.Close();
			dr.Dispose();
		}
		public DataSet GetDataSet(string sql)
		{
			System.Data.SqlClient.SqlConnection conn3 = new System.Data.SqlClient.SqlConnection(cnStr);
			System.Data.SqlClient.SqlDataAdapter adapt = new System.Data.SqlClient.SqlDataAdapter(sql, conn3);
			DataSet ds = new DataSet();
			adapt.Fill(ds);
			conn3.Close();
			conn3.Dispose();
			conn3 = null;
			return ds;
		}

		public int GetScalarInteger(string sql)
		{
			int x = -1;
			System.Data.SqlClient.SqlConnection conn3 = new System.Data.SqlClient.SqlConnection(cnStr);
			conn3.Open();
			SqlCommand cmd = new SqlCommand(sql, conn3);
			//x = (int)cmd.ExecuteScalar();
			x = Convert.ToInt32(cmd.ExecuteScalar());
			conn3.Close();
			conn3.Dispose();
			conn3 = null;
			return x;
		}

		public bool GetScalarBoolean(string sql)
		{
			bool x;
			System.Data.SqlClient.SqlConnection conn3 = new System.Data.SqlClient.SqlConnection(cnStr);
			conn3.Open();
			SqlCommand cmd = new SqlCommand(sql, conn3);
			x = (bool)cmd.ExecuteScalar();
			conn3.Close();
			conn3.Dispose();
			conn3 = null;
			return x;
		}
		public decimal GetScalarDecimal(string sql)
		{
			decimal x = 0.0M;
			System.Data.SqlClient.SqlConnection conn3 = new System.Data.SqlClient.SqlConnection(cnStr);
			conn3.Open();
			SqlCommand cmd = new SqlCommand(sql, conn3);
			object result = cmd.ExecuteScalar();
			if (result == null)
			{
				return -1.0M;
			}
			x = (decimal)result;
			conn3.Close();
			conn3.Dispose();
			conn3 = null;
			return x;
		}

		public int GetDateDiff(DateTime start, DateTime end, string type = "day")
		{
			string sql;
			sql = "SELECT DATEDIFF(" + type + ",'" + start.ToShortDateString() + "','" + end.ToShortDateString() + "')";
			return GetScalarInteger(sql);
		}
		public string GetScalarString(string sql)
		{
			string x = "";
			System.Data.SqlClient.SqlConnection conn3 = new System.Data.SqlClient.SqlConnection(cnStr);
			conn3.Open();
			SqlCommand cmd = new SqlCommand(sql, conn3);
			object result = cmd.ExecuteScalar();
			if (result == null)
			{
				return "";
			}
			if (result.ToString() == "")
			{
				x = "";
			}
			else
			{
				x = (string)result;
			}
			conn3.Close();
			conn3.Dispose();
			conn3 = null;
			return x;
		}

		public decimal GetScalarDouble(string sql)
		{
			decimal x = 0.0M;
			System.Data.SqlClient.SqlConnection conn3 = new System.Data.SqlClient.SqlConnection(cnStr);
			conn3.Open();
			SqlCommand cmd = new SqlCommand(sql, conn3);
			decimal result = (decimal)(double)cmd.ExecuteScalar();
			if (result == 0)
			{
				x = 0.0M;
			}
			else
			{
				x = result;
			}
			conn3.Close();
			conn3.Dispose();
			conn3 = null;
			return x;
		}

		public DateTime GetScalarDate(string sql)
		{
			DateTime x;
			System.Data.SqlClient.SqlConnection conn3 = new System.Data.SqlClient.SqlConnection(cnStr);
			conn3.Open();
			SqlCommand cmd = new SqlCommand(sql, conn3);
			object result = cmd.ExecuteScalar();
			if (result == DBNull.Value)
			{
				return DateTime.Parse("1/1/1900");
			}
			else
			{
				x = (DateTime)result;
			}
			conn3.Close();
			conn3.Dispose();
			conn3 = null;
			return x;
		}
		public string GetScalarXML(string sql, int enrollID, string transType)
		{
			string x = "";
			System.Data.SqlClient.SqlConnection conn3 = new System.Data.SqlClient.SqlConnection(cnStr);
			conn3.Open();
			SqlCommand cmd = new SqlCommand();
			cmd.Connection = conn3;
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = sql;
			cmd.Parameters.AddWithValue("@enrollment_id", enrollID);
			cmd.Parameters.AddWithValue("@transType", transType);

			string result = "";

			SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);

			while (reader.Read())
			{
				result = result + reader.GetString(0);
			}

			if (result == null)
			{
				x = "";
				conn3.Close();
				conn3.Dispose();
				conn3 = null;
				return x;
			}
			if (result.ToString() == "")
			{
				x = "";
			}
			else
			{
				x = result.ToString();
			}
			conn3.Close();
			conn3.Dispose();
			conn3 = null;
			return x;
		}

		public bool HasData(string sql, SqlConnection newConn = null)
		{
			System.Data.SqlClient.SqlDataReader dr;
			if (newConn == null)
			{
				newConn = conn;
			}

			if (newConn.State != System.Data.ConnectionState.Open)
			{
				newConn.Open();
			}

			SqlCommand cmd = new SqlCommand(sql, newConn);
			cmd.CommandTimeout = 3600;
			try
			{
				dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					dr.Close();
					newConn.Close();
					return true;
				}
				else
				{
					dr.Close();
					newConn.Close();
					return false;
				}
			}
			catch (Exception ex)
			{
				newConn.Close();
				return false;
			}
		}

		public string RunCommand(string sql)
		{
			if (conn.State != System.Data.ConnectionState.Open)
			{
				conn.Open();
			}

			SqlCommand cmd = new SqlCommand(sql, conn);
			cmd.CommandTimeout = 6000000;
			try
			{
				cmd.ExecuteNonQuery();
				return "";
			}
			catch (Exception e)
			{
                throw (e);
			}
		}

        public void RunCommandWithEx(string sql)
        {
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandTimeout = 6000000;
            try
            {
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
                throw ex;
            }
        }

        public void RunCommandWithParams(string sql, List<SqlParameter> parameters)
        {
            SqlConnection newConn = conn;

            if (newConn.State != System.Data.ConnectionState.Open)
            {
                newConn.Open();
            }

            SqlCommand cmd = new SqlCommand(sql, newConn);
            cmd.CommandTimeout = 6000000;
            foreach (var parm in parameters)
            {
                cmd.Parameters.Add(parm);
            }
            try
            {
                cmd.ExecuteNonQuery();
                newConn.Close();
            }
            catch (Exception ex)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
                throw ex;
            }
        }

        internal string FSQ(string v)
		{
			if (v != null)
				return v.Replace("'", "''");
			else
				return "";
		}
		public string MapBoolean(bool val)
		{
			if (val)
				return "1";
			return "0";
		}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn = null;
            }
        }

    }
}



