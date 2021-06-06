var Participant = /** @class */ (function () {
    function Participant() {
        this.urlGetData = "/participant/table-data-view";
        this.urlGetPaging = "/participant/table-paging-view";
        this.urlGetForm = "/participant/form-view";
        this.urlSave = '/participant/save';
        this.urlDelete = '/participant/delete';
        this.currentPage = 1;
        this.search = "";
        this.init();
    }
    Participant.prototype.init = function () {
        var _this = this;
        try {
            this.initTable(this.currentPage, this.search);
            $('#add').click(function () {
                _this.add();
            });
            $('#search').click(function () {
                $("#modal-search").modal("show");
            });
            $(document).on("click", ".page-link", function (e) {
                var idx = $(e.currentTarget).data('dt-idx');
                _this.currentPage = idx;
                _this.initTable(idx, _this.search);
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
            $(document).on("change", "#IsCanRetake", function (e) {
                var isChecked = $("#IsCanRetake").is(":checked");
                if (isChecked) {
                    $("#div_MaxRetake").show();
                }
                else {
                    $("#div_MaxRetake").hide();
                }
            });
            this.initForm();
            $('#search').show();
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Participant.prototype.initTable = function (page, search) {
        try {
            Util.request(this.urlGetData + "?page=" + page + "&scheduleID=" + $("#Schedule").val() + "&finish=" + $("#Finish").val() + "&search=" + search, 'GET', 'html', function (response) {
                $('#table_list tbody').empty();
                $('#table_list tbody').append(response);
            }, function () {
                console.error('Failed to get data. Please try again');
                Util.error('Failed to get data. Please try again');
            });
            Util.request(this.urlGetPaging + "?page=" + page + "&scheduleID=" + $("#Schedule").val() + "&finish=" + $("#Finish").val() + "&search=" + search, 'GET', 'html', function (response) {
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
    Participant.prototype.add = function () {
        try {
            Util.request(this.urlGetForm + "?scheduleID=" + $("#Schedule").val(), 'GET', 'html', function (response) {
                $('#modal-default .modal-title').html("Tambah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                $("#modal-default").modal("show");
                $("#div_MaxRetake").hide();
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Participant.prototype.initForm = function () {
        var _this = this;
        try {
            $('#save_form').click(function () {
                _this.save();
            });
            $('#close_form').click(function () {
                $("#modal-default").modal("hide");
                _this.initTable(_this.currentPage, _this.search);
            });
            $('#search_form').click(function () {
                $("#modal-search").modal("hide");
                $("#divSearchResult").show();
                var search = $("#Search").val().toString();
                _this.search = search;
                $("#span_Search").html("<b>\"" + search + "\"</b>");
                _this.initTable(1, _this.search);
            });
            $('#close_search_form').click(function () {
                $("#modal-search").modal("hide");
                _this.initTable(_this.currentPage, _this.search);
            });
            $('#clear_search').click(function () {
                $("#Search").val("");
                $("#divSearchResult").hide();
                _this.search = "";
                _this.initTable(1, _this.search);
            });
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Participant.prototype.save = function () {
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
                        _this.initTable(_this.currentPage, _this.search);
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
    Participant.prototype.create = function () {
        try {
            var data = {
                ID: $('#ID').val(),
                QuestionPackage: {
                    ID: $('#QuestionPackage').val(),
                },
                ParticipantUser: {
                    UserId: $('#ParticipantUser').val(),
                },
                Schedule: {
                    ID: $('#Schedule').val(),
                },
                IsCanRetake: $('#IsCanRetake').is(":checked"),
                MaxRetake: $('#MaxRetake').val()
            };
            return data;
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Participant.prototype.delete = function (data) {
        var _this = this;
        try {
            if (confirm("Apa anda yaking menghapus data ini ?") == true) {
                Util.request(this.urlDelete, 'post', 'json', function (response) {
                    if (response.success) {
                        Util.success(response.message);
                        _this.initTable(_this.currentPage, _this.search);
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
    Participant.prototype.edit = function (data) {
        try {
            Util.request(this.urlGetForm + "?id=" + data.id + "&scheduleID=" + $("#Schedule").val(), 'GET', 'html', function (response) {
                $('#modal-default .modal-title').html("Ubah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                $("#modal-default").modal("show");
                $("#div_MaxRetake").hide();
                if ($("#IsCanRetake").is(":checked")) {
                    $("#div_MaxRetake").show();
                }
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        }
        catch (e) {
            console.error(e);
        }
    };
    return Participant;
}());
$(document).ready(function () {
    new Participant();
});
//# sourceMappingURL=participant.js.map