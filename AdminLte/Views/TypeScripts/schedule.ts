class Schedule {
    private urlGetData = "/schedule/table-data-view";
    private urlGetPaging = "/schedule/table-paging-view";
    private urlGetForm = "/schedule/form-view";
    private urlSave = '/schedule/save';
    private urlDelete = '/schedule/delete';
    private urlSubPeriode = '/sub-period/select-option';
    private urlPeriodeDetail = '/period/detail';
    private urlSubPeriodeDetail = '/sub-period/detail';

    private currentPage = 1;

    constructor() {
        this.init();
    }
    private init() {
        try {
            this.initTable(this.currentPage);
            $('#add').click(() => {
                this.add();
            });
            $(document).on("click", ".page-link", (e) => {
                const idx = $(e.currentTarget).data('dt-idx');
                this.currentPage = idx;
                this.initTable(idx);
            });
            $(document).on("click", ".btn-delete", (e) => {
                const id = $(e.currentTarget).data('id');
                const data = { id: id };
                this.delete(data);
            });
            $(document).on("click", ".btn-edit", (e) => {
                const id = $(e.currentTarget).data('id');
                const data = { id: id };
                this.edit(data);
            });
            $(document).on("change", "#Period", (e) => {
                const id = $(e.currentTarget).val();
                Util.request(this.urlSubPeriode + "?periodID=" + id, 'GET', 'html', (response) => {
                    $('#SubPeriod').empty();
                    $('#SubPeriod').append(response);
                }, function () {
                    console.error('Failed to get data. Please try again');
                    Util.error('Failed to get data. Please try again');
                });
                if (id == "-1") {
                    (<any>$('#Date')).daterangepicker({
                        locale: {
                            format: 'YYYY-MM-DD',
                            separator: ' s/d '
                        }
                    });
                    return;
                }
                Util.request(this.urlPeriodeDetail + "?periodID=" + id, 'GET', 'html', (response) => {
                    var json = JSON.parse(response);
                    console.log(json);
                    if (json.success) {
                        (<any>$('#Date')).daterangepicker({
                            locale: {
                                format: 'YYYY-MM-DD',
                                separator: ' s/d '
                            },
                            minDate: json.data.start.substring(0, 10),
                            maxDate: json.data.end.substring(0, 10),
                        });
                    } else {
                        Util.error(json.message);
                    }
                }, function () {
                    console.error('Failed to get data. Please try again');
                    Util.error('Failed to get data. Please try again');
                });
            });
            $(document).on("change", "#SubPeriod", (e) => {
                const id = $(e.currentTarget).val();
                if (id == "-1") {
                    $('#Period').trigger('change');
                    return;
                }
                Util.request(this.urlSubPeriodeDetail + "?subPeriodID=" + id, 'GET', 'html', (response) => {
                    var json = JSON.parse(response);
                    console.log(json);
                    if (json.success) {
                        (<any>$('#Date')).daterangepicker({
                            locale: {
                                format: 'YYYY-MM-DD',
                                separator: ' s/d '
                            },
                            minDate: json.data.start.substring(0, 10),
                            maxDate: json.data.end.substring(0, 10),
                        });
                    } else {
                        Util.error(json.message);
                    }
                }, function () {
                    console.error('Failed to get data. Please try again');
                    Util.error('Failed to get data. Please try again');
                });
            });

            this.initForm();

        } catch (e) {
            console.error(e);
            Util.error(e);
        }
    }
    private initTable(page) {
        try {
            Util.request(this.urlGetData + "?page=" + page, 'GET', 'html', (response) => {
                $('#table_list tbody').empty();
                $('#table_list tbody').append(response);
            }, function () {
                console.error('Failed to get data. Please try again');
                Util.error('Failed to get data. Please try again');
            });
            Util.request(this.urlGetPaging + "?page=" + page, 'GET', 'html', (response) => {
                $('#table_paging').empty();
                $('#table_paging').append(response);
            }, function () {
                console.error('Failed to get data. Please try again');
                Util.error('Failed to get data. Please try again');
            });
        } catch (e) {
            console.error(e);
            Util.error(e);
        }
    }
    private add() {
        try {
            Util.request(this.urlGetForm, 'GET', 'html', (response) => {
                $('#modal-default .modal-title').html("Tambah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                (<any>$("#modal-default")).modal("show");
                (<any>$('#Date')).daterangepicker({
                    locale: {
                        format: 'YYYY-MM-DD',
                        separator: ' s/d '
                    }
                });
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        } catch (e) {
            console.error(e);
            Util.error(e);
        }
    }
    private initForm() {
        try {
            $('#save_form').click(() => {
                this.save();
            });
            $('#close_form').click(() => {
                (<any>$("#modal-default")).modal("hide")
                this.initTable(this.currentPage)
            });
        } catch (e) {
            console.error(e);
            Util.error(e);
        }
    }
    private save() {
        try {
            if (!Util.formCheck()) {
                return;
            }
            const data = this.create();
            Util.request(this.urlSave, 'post', 'json', (response) => {
                if (response != null) {
                    if (response.success) {
                        Util.success(response.message);
                        (<any>$("#modal-default")).modal("hide")
                        this.initTable(this.currentPage)
                    } else {
                        Util.error(response.message);
                    }
                } else {
                    Util.error('Failed to get data #T7G985. Please try again.');
                    console.error('Failed to get data #T7G985. Please try again.');
                }
            }, () => {
            }, data);
        } catch (e) {
            console.error(e);
            Util.error(e);
        }
    }
    private create() {
        try {
            var Date = $('#Date').val().toString();
            var Dates = Date.split(" s/d ");
            const data = {
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
        } catch (e) {
            console.error(e);
            Util.error(e);
        }
    }
    private delete(data) {
        try {
            if (confirm("Apa anda yaking menghapus data ini ?") == true) {
                Util.request(this.urlDelete, 'post', 'json', (response) => {
                    if (response.success) {
                        Util.success(response.message);
                        this.initTable(this.currentPage)
                    } else {
                        Util.error(response.message);
                    }
                }, () => {
                    Util.error(`Failed to delete . Please try again`);
                }, data);
            }
        } catch (e) {
            console.error(e);
            Util.error(e);
        }
    }
    private edit(data) {
        try {
            Util.request(this.urlGetForm + "?id=" + data.id + "&scheduleID=" + $("#Schedule").val(), 'GET', 'html', (response) => {
                $('#modal-default .modal-title').html("Ubah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                (<any>$("#modal-default")).modal("show");
                (<any>$('#Date')).daterangepicker({
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
        } catch (e) {
            console.error(e);
        }
    }
}

$(document).ready(function () {
    new Schedule();
});