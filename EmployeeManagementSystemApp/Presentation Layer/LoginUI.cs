using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EmployeeManagementSystemApp
{
    public partial class LoginUI : Form
    {
        public LoginUI()
        {
            InitializeComponent();
            passwordTextBox.PasswordChar = '*';
            passwordTextBox.MaxLength = 3;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string userName = userNameTextBox.Text;
            string password = passwordTextBox.Text;

            if (adminRadioButton.Checked)
            {
                DatabaseConnection aConnection = new DatabaseConnection();
                aConnection.Open();

                string checkQuery = "SELECT * FROM admin_login_table WHERE UserName = '" + userName + "' and Password = '" + password + "'";
                SqlDataAdapter checkAdapter = new SqlDataAdapter(checkQuery, aConnection.ConnectionString);
                DataTable checkDataTable = new DataTable();
                checkAdapter.Fill(checkDataTable);

                if (checkDataTable.Rows.Count == 1)
                {
                    AdminUI anAdminWindow = new AdminUI();
                    anAdminWindow.Show();
                    aConnection.Close();
                    Close();
                }
                else
                {
                    MessageBox.Show("User name or password not found");
                }
            }
            else
            {
                DatabaseConnection aConnection = new DatabaseConnection();
                aConnection.Open();

                string checkQuery = "SELECT * FROM employee_table WHERE UserName = '" + userName + "' and Password = '" + password + "'";
                SqlDataAdapter checkAdapter = new SqlDataAdapter(checkQuery, aConnection.ConnectionString);
                DataTable checkDataTable = new DataTable();
                checkAdapter.Fill(checkDataTable);

                if (checkDataTable.Rows.Count == 1)
                {
                    EmployeeUI anEmployeeWindow = new EmployeeUI();
                    anEmployeeWindow.SetUserName(userName);
                    anEmployeeWindow.Show();
                    aConnection.Close();
                    Close();
                }
                else
                {
                    MessageBox.Show("User name or password not found");
                }

            }
        }
    }
}
