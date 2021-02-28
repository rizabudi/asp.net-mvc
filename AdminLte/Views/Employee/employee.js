var Employees = /** @class */ (function () {
    function Employees() {
        this.urlGetData = "/adminlte/employee/table-data-view";
        this.urlAddEmployee = '/adminlte/employee/add';
        this.urlSaveEmployee = '/adminlte/employee/save';
        this.urlDeleteEmployee = '/adminlte/employee/delete';
        this.urlEditEmployee = '/adminlte/employee/edit';
        this.init();
    }
    Employees.prototype.init = function () {
        var _this = this;
        try {
            this.initTable();
            $('#add_employee').click(function () {
                _this.add();
            });
        }
        catch (e) {
            console.error(e);
        }
    };
    Employees.prototype.initTable = function () {
        var _this = this;
        try {
            Util.request(this.urlGetData, 'GET', 'html', function (response) {
                $('#employees_list tbody').empty();
                $('#employees_list tbody').append(response);
                $(document).on("click", ".employee-delete", function (e) {
                    var id = $(e.currentTarget).data('id');
                    var data = { id: id };
                    _this.delete(data);
                });
                $(document).on("click", ".employee-edit", function (e) {
                    var id = $(e.currentTarget).data('id');
                    var data = { id: id };
                    _this.edit(data);
                });
            }, function () {
                console.error('Failed to get data. Please try again');
            });
        }
        catch (e) {
            console.error(e);
        }
    };
    Employees.prototype.add = function () {
        var _this = this;
        try {
            Util.request(this.urlAddEmployee, 'get', 'html', function (response) {
                $('#employee_form').html(response);
                _this.initForm();
            }), function () {
                console.error('Failed to get data. Please try again');
            };
        }
        catch (e) {
            console.error(e);
        }
    };
    Employees.prototype.initForm = function () {
        var _this = this;
        try {
            $('#save_form').click(function () {
                _this.save();
            });
            $('#close_form').click(function () {
                location.reload(); //Reloads the page to show table
            });
        }
        catch (e) {
            console.error(e);
        }
    };
    Employees.prototype.save = function () {
        try {
            var employee = this.createEmployee();
            Util.request(this.urlSaveEmployee, 'post', 'json', function (response) {
                if (response != null) {
                    Util.alert(response.message);
                    location.reload();
                }
                else {
                    Util.alert(response.message);
                    console.error('Failed to get data #T7G985. Please try again.');
                }
            }, function () {
            }, employee);
        }
        catch (e) {
            console.error(e);
        }
    };
    Employees.prototype.createEmployee = function () {
        try {
            var employee = {
                EmployeeId: $('#employee_id').val(),
                Firstname: $('#first_name').val(),
                Lastname: $('#last_name').val(),
                Position: $('#position').val(),
                JobTitle: $('#job_title').val(),
                Department: $('#department').val(),
                Salary: $('#salary').val(),
                DateJoined: $('#date_joined').val(),
                LastUpdated: $('#last_changed').val()
            };
            return employee;
        }
        catch (e) {
            console.error(e);
        }
    };
    Employees.prototype.delete = function (data) {
        try {
            if (confirm("Are you sure you want to delete this employee?") == true) {
                Util.request(this.urlDeleteEmployee, 'post', 'json', function () {
                    Util.alert("Employee deleted successfully.");
                    location.reload();
                }, function () {
                    Util.alert("Failed to delete Employee. Please try again");
                }, data);
            }
        }
        catch (e) {
            console.error(e);
        }
    };
    Employees.prototype.edit = function (data) {
        var _this = this;
        try {
            Util.request(this.urlEditEmployee, 'get', 'html', function (response) {
                $('#employee_form').html(response);
                _this.initForm();
            }, function () {
                console.error('Failed to get data. Please try again');
            }, data);
        }
        catch (e) {
            console.error(e);
        }
    };
    return Employees;
}());
$(document).ready(function () {
    new Employees();
});
//# sourceMappingURL=employee.js.map