using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSystemApp.Data_Access_Layer.Model
{
    class Employee
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }

        public Employee(string id, string name, int age, string userName, string password, string department, string designation)
        {
            ID = id;
            Name = name;
            Age = age;
            UserName = userName;
            Password = password;
            Department = department;
            Designation = designation;
        }
    }
}
