class Employees {
    private urlGetData = "/adminlte/employee/table-data-view";
    private urlAddEmployee = '/adminlte/employee/add';
    private urlSaveEmployee = '/adminlte/employee/save';
    private urlDeleteEmployee = '/adminlte/employee/delete';
    private urlEditEmployee = '/adminlte/employee/edit';
    private urlSearchEmployee = '/adminlte/employee/search';

    constructor() {
        this.init();
    }
    private init() {
        try {
            this.initTable();
            $('#add_employee').click(() => {
                this.add();
            });
            $('#search_employee').click(() => {
                const keyword = $('#keyword').val();
                this.search(keyword);
            });
        } catch (e) {
            console.error(e);
        }
    }
    private initTable() {
        try {
            Util.request(this.urlGetData, 'GET', 'html', (response) => {
                $('#employees_list tbody').empty();
                $('#employees_list tbody').append(response);
                $(document).on("click", ".employee-delete", (e) => {
                    const id = $(e.currentTarget).data('id');
                    const data = { id: id };
                    this.delete(data);
                });
                $(document).on("click", ".employee-edit", (e) => {
                    const id = $(e.currentTarget).data('id');
                    const data = { id: id };
                    this.edit(data);
                });
            }, function () {
                console.error('Failed to get data. Please try again');
            });
        } catch (e) {
            console.error(e);
        }
    }
    private add() {
        try {
            Util.request(this.urlAddEmployee, 'get', 'html', (response) => {
                $('#employee_form').html(response)
                this.initForm();
            }), () => {
                console.error('Failed to get data. Please try again');
            }
        } catch (e) {
            console.error(e);
        }
    }
    private initForm() {
        try {
            $('#save_form').click(() => {
                this.save();
            });
            $('#close_form').click(() => {
                location.reload(); //Reloads the page to show table
            });
        } catch (e) {
            console.error(e);
        }
    }
    private save() {
        try {
            const employee = this.createEmployee();
            Util.request(this.urlSaveEmployee, 'post', 'json', (response) => {
                if (response != null) {
                    Util.alert(response.message);
                    location.reload();
                } else {
                    Util.alert(response.message);
                    console.error('Failed to get data #T7G985. Please try again.');
                }
            }, () => {
            }, employee);
        } catch (e) {
            console.error(e);
        }
    }
    private createEmployee() {
        try {
            const employee = {
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
        } catch (e) {
            console.error(e);
        }
    }
    private delete(data) {
        try {
            if (confirm("Are you sure you want to delete this employee?") == true) {
                Util.request(this.urlDeleteEmployee, 'post', 'json', () => {
                    Util.alert(`Employee deleted successfully.`);
                    location.reload();
                }, () => {
                    Util.alert(`Failed to delete Employee. Please try again`);
                }, data);
            }
        } catch (e) {
            console.error(e);
        }
    }
    private edit(data) {
        try {
            Util.request(this.urlEditEmployee, 'get', 'html', (response) => {
                $('#employee_form').html(response)
                this.initForm();
            }, () => {
                console.error('Failed to get data. Please try again');
            }, data);
        } catch (e) {
            console.error(e);
        }
    }
    private search(keyword) {
        try {
            const data = { keyword: keyword };
            Util.request(this.urlSearchEmployee, 'GET', 'html', (response) => {
                const currentKeyWord = $('#keyword').val();
                if (currentKeyWord === keyword) {
                    $('#employees_list tbody').empty();
                    $('#employees_list tbody').append(response);
                }
            }, function () {
                Util.alert('Failed to get data. Please try again.');
                console.error('Failed to get data #T09576. Please try again.');
            }, data);
        } catch (e) {
            console.error(e);
        }
    }
}

$(document).ready(function () {
    new Employees();
});