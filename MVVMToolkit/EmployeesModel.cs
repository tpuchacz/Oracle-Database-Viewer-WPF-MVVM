using CommunityToolkit.Mvvm.ComponentModel;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMToolkit;

public partial class EmployeesModel : ObservableObject
{
    /*public EmployeesModel(int employeeId,string firstName, string lastName,
                         string email, string phoneNumber, DateTime hireDate, string jobId,
                         double salary, double commissionPct, int managerId, int departmentId)
    {
        EmployeeId = employeeId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        HireDate = hireDate;
        JobId = jobId;
        Salary = salary;
        CommissionPct = commissionPct;
        ManagerId = managerId;
        DepartmentId = departmentId;
    }*/

    [ObservableProperty]
    private int _employeeId;

    [ObservableProperty]
    private string? _firstName;

    [ObservableProperty]
    private string _lastName;

    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string? _phoneNumber;

    [ObservableProperty]
    private DateTime _hireDate;

    [ObservableProperty]
    private string _jobId;

    [ObservableProperty]
    private double? _salary;

    [ObservableProperty]
    private double? _commissionPct;

    [ObservableProperty]
    private int? _managerId;

    [ObservableProperty]
    private int? _departmentId;
}
