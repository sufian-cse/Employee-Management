using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using EmployeeManagementSystemApp.Data_Access_Layer.Model;

namespace EmployeeManagementSystemApp
{
    public partial class AdminUI : Form
    {
        public AdminUI()
        {
            InitializeComponent();
            adminTabControl.SelectedIndexChanged += new EventHandler(Tabs_SelectedIndexChanged);
            passwordTextBox.PasswordChar = '*';
            passwordTextBox.MaxLength = 3;
        }

        private void FillDepartmentCombobox()
        {
            DatabaseConnection aConnection = new DatabaseConnection();
            DataSet ds = new DataSet();
            try
            {
                aConnection.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT DepartmentName FROM department_table", aConnection.connection);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
                departmentComboBox.DisplayMember = "DepartmentName";
                departmentComboBox.ValueMember = "DepartmentID";
                departmentComboBox.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {
                //Exception Message
            }
            finally
            {
                aConnection.Close();
            }
        }

        private void FillDesignationCombobox()
        {
            DatabaseConnection aConnection = new DatabaseConnection();
            DataSet ds = new DataSet();
            try
            {
                aConnection.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT Designation FROM department_table", aConnection.connection);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
                designationComboBox.DisplayMember = "Designation";
                designationComboBox.ValueMember = "DepartmentID";
                designationComboBox.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {
                //Exception Message
            }
            finally
            {
                aConnection.Close();
            }
        }

        private string GenerateAutoID()
        {
            string ID = "EMP-1901";
            DatabaseConnection aConnection = new DatabaseConnection();
            aConnection.Open();
            SqlCommand cmd = new SqlCommand("SELECT COUNT(EmployeeID) FROM employee_table", aConnection.connection);
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            aConnection.Close();
            i++;
            ID += i.ToString();
            return ID;
        }



        private void Tabs_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (adminTabControl.SelectedTab == employeeListTabPage)
            {
                DatabaseConnection aConnection = new DatabaseConnection();
                aConnection.Open();

                employeeListDataGridView.DataSource =
                    aConnection.ShowDataInGridView("SELECT EmployeeID, Name, Age FROM employee_table");

                aConnection.Close();
            }
            else if (adminTabControl.SelectedTab == addEmployeeTabPage)
            {
                
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            DatabaseConnection connection = new DatabaseConnection();
            connection.Open();

            string id = GenerateAutoID();
            string name = nameTextBox.Text;
            int age = Convert.ToInt32(ageTextBox.Text);
            string userName = userNameTextBox.Text;
            string password = passwordTextBox.Text;
            string department = departmentComboBox.Text;
            string designation = designationComboBox.Text;

            //Employee anEmployee = new Employee(id, name, age, userName, password, department, desi);

            string checkQuery = "SELECT * FROM employee_table WHERE UserName = '" + userName + "'";
            SqlDataAdapter aDataAdapter = new SqlDataAdapter(checkQuery, connection.ConnectionString);
            DataTable dataTable = new DataTable();
            aDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 1)
            {
                MessageBox.Show("User name already exists!");
            }
            else
            {
                SqlCommand cmd = new SqlCommand("SELECT Salary FROM department_table WHERE DepartmentName = '" + department + "' and Designation = '" + designation + "'", connection.connection);
                int salary = Convert.ToInt32(cmd.ExecuteScalar());
                cmd = new SqlCommand("SELECT Allowance FROM department_table WHERE DepartmentName = '" + department + "' and Designation = '" + designation + "'", connection.connection);
                int allowance = Convert.ToInt32(cmd.ExecuteScalar());

                salary += (allowance / 100) * salary;

                string insertQuery = "INSERT INTO employee_table (EmployeeID, Name, Age, UserName, Password, Department, Designation) VALUES ('" + id + "','" + name +
                                     "', " + age + ", '" + userName + "', '" + password + "', '" + department + "', '" + designation + "')";
                int n = connection.ExecuteQuery(insertQuery);

                if (n > 0)
                {
                    MessageBox.Show("Employee ID: " + id + '\n' + "Password: " + password);
                    nameTextBox.Clear();
                    ageTextBox.Clear();
                    userNameTextBox.Clear();
                    passwordTextBox.Clear();
                }
                else
                {
                    MessageBox.Show("Could not save information");
                }

                insertQuery =
                    "INSERT INTO payroll_table (EmployeeID, Name, Salary, Attendence, WorkingDays, LeaveCatagory) VALUES (@EmployeeID, @Name, @Salary, @Attendence, @WorkingDays, @LeaveCatagory)";

                cmd = new SqlCommand(insertQuery, connection.connection);
                cmd.Parameters.AddWithValue("@EmployeeID", id);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Salary", salary);
                cmd.Parameters.AddWithValue("@Attendence", 0);
                cmd.Parameters.AddWithValue("@WorkingDays", 24);
                cmd.Parameters.AddWithValue("@LeaveCatagory", 0);

                cmd.ExecuteNonQuery();

                connection.Close();
            }

            
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            DatabaseConnection aConnection = new DatabaseConnection();
            aConnection.Open();

            

            string checkQuery = "SELECT * FROM attendence_record_table WHERE AttendenceDate = '" +
                                attendenceDateTimePicker.Text + "'";
            SqlDataAdapter checkAdapter = new SqlDataAdapter(checkQuery, aConnection.ConnectionString);
            DataTable checkDataTable = new DataTable();
            checkAdapter.Fill(checkDataTable);

            if (checkDataTable.Rows.Count > 0)
            {
                attendenceDataGridView.DataSource =
                    aConnection.ShowDataInGridView(
                        "SELECT EmployeeID, EmployeeName, AttendenceDate, Present FROM attendence_record_table WHERE AttendenceDate = '" +
                        attendenceDateTimePicker.Text + "'");

              
            }
            else
            {
                string monthString = attendenceDateTimePicker.Text;
                monthString = monthString.Substring(3, 2);

                int month = Convert.ToInt32(monthString);

                string getQuery = "SELECT * FROM employee_table";
                SqlDataAdapter getAdapter = new SqlDataAdapter(getQuery, aConnection.ConnectionString);
                DataTable getDataTable = new DataTable();
                getAdapter.Fill(getDataTable);

                foreach (DataRow row in getDataTable.Rows)
                {
                    aConnection.ExecuteQuery(
                        "INSERT INTO attendence_record_table (EmployeeID, EmployeeName, AttendenceDate, Present, Month) VALUES ('"+row[1]+"','" +
                        row[2] + "', '" + attendenceDateTimePicker.Text + "', 0, '" + month.ToString() + "')");
                }

                attendenceDataGridView.DataSource = aConnection.ShowDataInGridView(
                    "SELECT EmployeeID, EmployeeName, AttendenceDate, Present FROM attendence_record_table WHERE AttendenceDate = '" +
                    attendenceDateTimePicker.Text + "'");

               

                aConnection.Close();
            }
        }

        private void attendenceDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DatabaseConnection aConnection = new DatabaseConnection();
            aConnection.Open();

            if (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 2)
            {
                attendenceDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = true;
                
            }

            else if ((bool) attendenceDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == false)
            {

                aConnection.ExecuteQuery("UPDATE attendence_record_table SET Present = 1 WHERE EmployeeID = '" +
                                         attendenceDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString() + "' AND AttendenceDate = '"+attendenceDataGridView.Rows[e.RowIndex].Cells[2].Value.ToString()+"'");

                aConnection.ExecuteQuery("UPDATE payroll_table SET Attendence = Attendence + 1 WHERE EmployeeID = '" +
                                         attendenceDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString() + "'");

            }

            else
            {
                aConnection.ExecuteQuery("UPDATE attendence_record_table SET Present = 0 WHERE EmployeeID = '" +
                                         attendenceDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString() + "'  AND AttendenceDate = '" + attendenceDataGridView.Rows[e.RowIndex].Cells[2].Value.ToString() + "'");

                aConnection.ExecuteQuery("UPDATE payroll_table SET Attendence = Attendence - 1 WHERE EmployeeID = '" +
                                         attendenceDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString() + "'");
            }

            aConnection.Close();
        }

        private void monthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DatabaseConnection aConnection = new DatabaseConnection();
            aConnection.Open();

            string month = ((monthComboBox.SelectedIndex) + 1).ToString();

            showAttendenceDataGridView.DataSource =
                aConnection.ShowDataInGridView(
                    "SELECT EmployeeID, EmployeeName, AttendenceDate, Present FROM attendence_record_table WHERE Month = '" +
                    month + "' ");

            aConnection.Close();
        }

        private void saveDepartmentButton_Click(object sender, EventArgs e)
        {
            DatabaseConnection aConnection = new DatabaseConnection();
            aConnection.Open();

            string departmentName = departmentNameTextBox.Text;
            string designation = designationTextBox.Text;
            int salary = Convert.ToInt32(salaryTextBox.Text);
            int allowance = Convert.ToInt32(allowanceTextBox.Text);

            string insertQuery =
                "INSERT INTO department_table (DepartmentName, Designation, Salary, Allowance) VALUES (@DepartmentName, @Designation, @Salary, @Allowance)";

            SqlCommand cmd = new SqlCommand(insertQuery, aConnection.connection);
            cmd.Parameters.AddWithValue("@DepartmentName", departmentName);
            cmd.Parameters.AddWithValue("@Designation", designation);
            cmd.Parameters.AddWithValue("@Salary", salary);
            cmd.Parameters.AddWithValue("@Allowance", allowance);

            int rowAffected = cmd.ExecuteNonQuery();

            if (rowAffected > 0)
            {
                MessageBox.Show("Saved");
                departmentNameTextBox.Clear();
                designationTextBox.Clear();
                salaryTextBox.Clear();
                allowanceTextBox.Clear();
            }
                
            else
                MessageBox.Show("Can not save information");


                aConnection.Close();
        }

        private void AdminUI_Load(object sender, EventArgs e)
        {
            DateTime today = DateTime.Now;
            if (today.Day == 1)
            {
                DatabaseConnection aConnection = new DatabaseConnection();
                aConnection.Open();

                string getQuery = "SELECT * FROM payroll_table";
                SqlDataAdapter getAdapter = new SqlDataAdapter(getQuery, aConnection.ConnectionString);
                DataTable getDataTable = new DataTable();
                getAdapter.Fill(getDataTable);

                foreach (DataRow row in getDataTable.Rows)
                {
                    int attendence = Convert.ToInt32(row[4]);
                    int leave = Convert.ToInt32(row[6]);
                    int salary = Convert.ToInt32(row[3]);

                    int absent = 24 - (attendence + leave);

                    salary -= 200 * absent;

                    aConnection.ExecuteQuery("UPDATE payroll_table SET Salary = "+salary+", Attendence = 0, LeaveCatagory = 0 WHERE EmployeeID = '"+row[1]+"'");
                }

                aConnection.Close();
            }
            FillDepartmentCombobox();
            FillDesignationCombobox();
        }

        private void saveLeaveCatagoryButton_Click(object sender, EventArgs e)
        {
            DatabaseConnection aConnection = new DatabaseConnection();
            aConnection.Open();
            string id = employeeIDTextBox.Text;
            string noOfDays = noOfDaysTextBox.Text;

            string updateQuery = "UPDATE payroll_table SET LeaveCatagory = LeaveCatagory + @noOfDays WHERE EmployeeID = @id";
            SqlCommand cmd = new SqlCommand(updateQuery, aConnection.connection);
            cmd.Parameters.AddWithValue("@noOfDays", noOfDays);
            cmd.Parameters.AddWithValue("@id", id);

            int rowAffected = cmd.ExecuteNonQuery();

            if (rowAffected > 0)
            {
                MessageBox.Show("Successfully added");
            }
            else
            {
                MessageBox.Show("Can not be added");
            }

            aConnection.Close();
        }
    }
}