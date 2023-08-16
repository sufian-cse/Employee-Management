using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EmployeeManagementSystemApp
{
    public partial class EmployeeUI : Form
    {
        public EmployeeUI()
        {
            InitializeComponent();
        }

        private string userName;
        private string employeeID;
        private string name;
        private int age;
        private string department;
        private string designation;
        private int present;

        public void SetUserName(string userName)
        {
            this.userName = userName;
        }

        private void SetInfo()
        {
            DatabaseConnection aConnection = new DatabaseConnection();
            aConnection.Open();

            string getQuery = "SELECT * FROM employee_table WHERE UserName = '" + userName + "'";
            SqlDataAdapter aDataAdapter = new SqlDataAdapter(getQuery, aConnection.ConnectionString);
            DataTable dataTable = new DataTable();
            aDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count == 1)
            {
                DataRow row = dataTable.Rows[0];

                employeeID = row["EmployeeID"].ToString();
                name = row["Name"].ToString();
                age = Convert.ToInt32(row["Age"]);
                department = row["Department"].ToString();
                designation = row["Designation"].ToString();
            }
            else
            {
                MessageBox.Show("Multiple Accounts!");
            }

            getQuery = "SELECT * FROM payroll_table WHERE EmployeeID = '" + employeeID + "'";
            aDataAdapter = new SqlDataAdapter(getQuery, aConnection.ConnectionString);
            dataTable = new DataTable();
            aDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count == 1)
            {
                DataRow row = dataTable.Rows[0];
                present = Convert.ToInt32(row["Attendence"]);
            }
            else
            {
                MessageBox.Show("Multiple Accounts!");
            }

            aConnection.Close();
        }

        private void ShowInfo()
        {
            employeeIDTextBox.Text = employeeID;
            nameTextBox.Text = name;
            ageTextBox.Text = age.ToString();
            departmentTextBox.Text = department;
            designationTextBox.Text = designation;
            presentLabel.Text = present.ToString();
        }

        private void EmployeeUI_Load(object sender, EventArgs e)
        {
            SetInfo();
            ShowInfo();
        }
    }
}
