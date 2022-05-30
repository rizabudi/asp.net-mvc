var Schedule = /** @class */ (function () {
    function Schedule() {
        this.urlGetData = "schedule/table-data-view";
        this.urlGetPaging = "schedule/table-paging-view";
        this.urlGetForm = "schedule/form-view";
        this.urlSave = 'schedule/save';
        this.urlDelete = 'schedule/delete';
        this.urlSubPeriode = 'sub-period/select-option';
        this.urlPeriodeDetail = 'period/detail';
        this.urlSubPeriodeDetail = 'sub-period/detail';
        this.currentPage = 1;
        this.init();
    }
    Schedule.prototype.init = function () {
        var _this = this;
        try {
            this.initTable(this.currentPage);
            $('#add').click(function () {
                _this.add();
            });
            $(document).on("click", ".page-link", function (e) {
                var idx = $(e.currentTarget).data('dt-idx');
                _this.currentPage = idx;
                _this.initTable(idx);
            });
            $(document).on("click", ".btn-delete", function (e) {
                var id = $(e.currentTarget).data('id');
                var data = { id: id };
                _this.delete(data);
            });
            $(document).on("click", ".btn-edit", function (e) {
                var id = $(e.currentTarget).data('id');
                var data = { id: id };
                _this.edit(data);
            });
            $(document).on("change", "#Period", function (e) {
                var id = $(e.currentTarget).val();
                Util.request(_this.urlSubPeriode + "?periodID=" + id, 'GET', 'html', function (response) {
                    $('#SubPeriod').empty();
                    $('#SubPeriod').append(response);
                }, function () {
                    console.error('Failed to get data. Please try again');
                    Util.error('Failed to get data. Please try again');
                });
                if (id == "-1") {
                    $('#Date').daterangepicker({
                        locale: {
                            format: 'YYYY-MM-DD',
                            separator: ' s/d '
                        }
                    });
                    return;
                }
                Util.request(_this.urlPeriodeDetail + "?periodID=" + id, 'GET', 'html', function (response) {
                    var json = JSON.parse(response);
                    console.log(json);
                    if (json.success) {
                        $('#Date').daterangepicker({
                            locale: {
                                format: 'YYYY-MM-DD',
                                separator: ' s/d '
                            },
                            minDate: json.data.start.substring(0, 10),
                            maxDate: json.data.end.substring(0, 10),
                        });
                    }
                    else {
                        Util.error(json.message);
                    }
                }, function () {
                    console.error('Failed to get data. Please try again');
                    Util.error('Failed to get data. Please try again');
                });
            });
            $(document).on("change", "#SubPeriod", function (e) {
                var id = $(e.currentTarget).val();
                if (id == "-1") {
                    $('#Period').trigger('change');
                    return;
                }
                Util.request(_this.urlSubPeriodeDetail + "?subPeriodID=" + id, 'GET', 'html', function (response) {
                    var json = JSON.parse(response);
                    console.log(json);
                    if (json.success) {
                        $('#Date').daterangepicker({
                            locale: {
                                format: 'YYYY-MM-DD',
                                separator: ' s/d '
                            },
                            minDate: json.data.start.substring(0, 10),
                            maxDate: json.data.end.substring(0, 10),
                        });
                    }
                    else {
                        Util.error(json.message);
                    }
                }, function () {
                    console.error('Failed to get data. Please try again');
                    Util.error('Failed to get data. Please try again');
                });
            });
            this.initForm();
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Schedule.prototype.initTable = function (page) {
        try {
            Util.request(this.urlGetData + "?page=" + page, 'GET', 'html', function (response) {
                $('#table_list tbody').empty();
                $('#table_list tbody').append(response);
            }, function () {
                console.error('Failed to get data. Please try again');
                Util.error('Failed to get data. Please try again');
            });
            Util.request(this.urlGetPaging + "?page=" + page, 'GET', 'html', function (response) {
                $('#table_paging').empty();
                $('#table_paging').append(response);
            }, function () {
                console.error('Failed to get data. Please try again');
                Util.error('Failed to get data. Please try again');
            });
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Schedule.prototype.add = function () {
        try {
            Util.request(this.urlGetForm, 'GET', 'html', function (response) {
                $('#modal-default .modal-title').html("Tambah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                $("#modal-default").modal("show");
                $('#Date').daterangepicker({
                    locale: {
                        format: 'YYYY-MM-DD',
                        separator: ' s/d '
                    }
                });
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Schedule.prototype.initForm = function () {
        var _this = this;
        try {
            $('#save_form').click(function () {
                _this.save();
            });
            $('#close_form').click(function () {
                $("#modal-default").modal("hide");
                _this.initTable(_this.currentPage);
            });
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Schedule.prototype.save = function () {
        var _this = this;
        try {
            if (!Util.formCheck()) {
                return;
            }
            var data = this.create();
            Util.request(this.urlSave, 'post', 'json', function (response) {
                if (response != null) {
                    if (response.success) {
                        Util.success(response.message);
                        $("#modal-default").modal("hide");
                        _this.initTable(_this.currentPage);
                    }
                    else {
                        Util.error(response.message);
                    }
                }
                else {
                    Util.error('Failed to get data #T7G985. Please try again.');
                    console.error('Failed to get data #T7G985. Please try again.');
                }
            }, function () {
            }, data);
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Schedule.prototype.create = function () {
        try {
            var Date = $('#Date').val().toString();
            var Dates = Date.split(" s/d ");
            var data = {
                ID: $('#ID').val(),
                Assesment: {
                    ID: $('#Assesment').val(),
                },
                Entity: {
                    ID: $('#Entity').val(),
                },
                Period: {
                    ID: $('#Period').val(),
                },
                SubPeriod: {
                    ID: $('#SubPeriod').val(),
                },
                Name: $('#Name').val(),
                Start: Dates[0],
                End: Dates[1],
            };
            return data;
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Schedule.prototype.delete = function (data) {
        var _this = this;
        try {
            if (confirm("Apa anda yaking menghapus data ini ?") == true) {
                Util.request(this.urlDelete, 'post', 'json', function (response) {
                    if (response.success) {
                        Util.success(response.message);
                        _this.initTable(_this.currentPage);
                    }
                    else {
                        Util.error(response.message);
                    }
                }, function () {
                    Util.error("Failed to delete . Please try again");
                }, data);
            }
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Schedule.prototype.edit = function (data) {
        try {
            Util.request(this.urlGetForm + "?id=" + data.id + "&scheduleID=" + $("#Schedule").val(), 'GET', 'html', function (response) {
                $('#modal-default .modal-title').html("Ubah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                $("#modal-default").modal("show");
                $('#Date').daterangepicker({
                    locale: {
                        format: 'YYYY-MM-DD',
                        separator: ' s/d '
                    },
                    minDate: $("#MinDate").val(),
                    maxDate: $("#MaxDate").val(),
                });
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        }
        catch (e) {
            console.error(e);
        }
    };
    return Schedule;
}());
$(document).ready(function () {
    new Schedule();
});
//# sourceMappingURL=schedule.js.map