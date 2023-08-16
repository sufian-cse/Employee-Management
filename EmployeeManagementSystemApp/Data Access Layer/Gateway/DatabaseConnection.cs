using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagementSystemApp
{
    class DatabaseConnection
    {
        public string ConnectionString { get; } = @"Data Source=DESKTOP-DR6TIO7\STSQLSERVER;Initial Catalog=EmployeeManagementSystemDB;Integrated Security=True;MultipleActiveResultSets=True";
        public SqlConnection connection { set; get; }

        public void Open()
        {
            connection = new SqlConnection(ConnectionString);
            connection.Open();
        }


        public void Close()
        {
            connection.Close();
        }


        public int ExecuteQuery(string query)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            int n = cmd.ExecuteNonQuery();
            return n;
        }


        public SqlDataReader DataReader(string query)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader dr = cmd.ExecuteReader();
            return dr;
        }


        public object ShowDataInGridView(string query)
        {
            SqlDataAdapter dr = new SqlDataAdapter(query, ConnectionString);
            DataSet ds = new DataSet();
            dr.Fill(ds);
            object dataum = ds.Tables[0];
            return dataum;
        }
    }
}
