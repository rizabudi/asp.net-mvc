var ParticipantUser = /** @class */ (function () {
    function ParticipantUser() {
        this.urlGetData = "/participant-user/table-data-view";
        this.urlGetPaging = "/participant-user/table-paging-view";
        this.urlGetForm = "/participant-user/form-view";
        this.urlSave = '/participant-user/save';
        this.urlDelete = '/participant-user/delete';
        this.currentPage = 1;
        this.search = "";
        this.order = "";
        this.sort = "";
        this.init();
    }
    ParticipantUser.prototype.init = function () {
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
                var id = $(e.currentTarget).data('id-strng');
                var data = { id: id };
                _this.delete(data);
            });
            $(document).on("click", ".btn-edit", function (e) {
                var id = $(e.currentTarget).data('id-strng');
                var data = { id: id };
                _this.edit(data);
            });
            $(document).on("click", ".sortTable", function (e) {
                _this.sort = $(e.currentTarget).data("sort");
                _this.order = $(e.currentTarget).data("order");
                $(".sortTableAsc").addClass("sortTableBoth");
                $(".sortTableAsc").removeClass("sortTableAsc");
                $(".sortTableDesc").addClass("sortTableBoth");
                $(".sortTableDesc").removeClass("sortTableDesc");
                if (_this.order == "") {
                    $(e.currentTarget).removeClass("sortTableBoth");
                    $(e.currentTarget).addClass("sortTableAsc");
                    $(e.currentTarget).data("order", "asc");
                    _this.order = "asc";
                }
                else if (_this.order == "asc") {
                    $(e.currentTarget).removeClass("sortTableAsc");
                    $(e.currentTarget).addClass("sortTableDesc");
                    $(e.currentTarget).data("order", "desc");
                    _this.order = "desc";
                }
                else if (_this.order == "desc") {
                    $(e.currentTarget).removeClass("sortTableDesc");
                    $(e.currentTarget).addClass("sortTableAsc");
                    $(e.currentTarget).data("order", "asc");
                    _this.order = "asc";
                }
                _this.initTable(_this.currentPage, _this.search);
            });
            this.initForm();
            $('#search').show();
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    ParticipantUser.prototype.initTable = function (page, search) {
        try {
            Util.request(this.urlGetData + "?page=" + page + "&search=" + search + "&order=" + this.order + "&sort=" + this.sort, 'GET', 'html', function (response) {
                $('#table_list tbody').empty();
                $('#table_list tbody').append(response);
            }, function () {
                console.error('Failed to get data. Please try again');
                Util.error('Failed to get data. Please try again');
            });
            Util.request(this.urlGetPaging + "?page=" + page + "&search=" + search, 'GET', 'html', function (response) {
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
    ParticipantUser.prototype.add = function () {
        try {
            Util.request(this.urlGetForm, 'GET', 'html', function (response) {
                $('#modal-default .modal-title').html("Tambah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                $("#modal-default").modal("show");
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    ParticipantUser.prototype.initForm = function () {
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
            $(document).on("change", "#Entity", function (e) {
                var entityID = $(e.currentTarget).val();
                Util.request("/entity/select-view?entity=" + entityID, 'GET', 'html', function (response) {
                    $('#SubEntity').html(response);
                }, function () {
                    Util.error('Failed to get data. Please try again');
                });
            });
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    ParticipantUser.prototype.save = function () {
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
    ParticipantUser.prototype.create = function () {
        try {
            var data = {
                UserId: $('#UserId').val(),
                Name: $('#Name').val(),
                Email: $('#Email').val(),
                Phone: $('#Phone').val(),
                EmployeeNumber: $('#EmployeeNumber').val(),
                User: {
                    UserName: $("#UserName").val(),
                    PasswordHash: $("#Password").val(),
                },
                Entity: {
                    ID: $("#Entity").val()
                },
                SubEntity: {
                    ID: $("#SubEntity").val()
                },
                Position: {
                    ID: $("#Position").val()
                },
                CompanyFunction: {
                    ID: $("#CompanyFunction").val()
                },
                Divition: {
                    ID: $("#Divition").val()
                },
                Department: {
                    ID: $("#Department").val()
                },
                JobLevel: {
                    ID: $("#JobLevel").val()
                }
            };
            return data;
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    ParticipantUser.prototype.delete = function (data) {
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
    ParticipantUser.prototype.edit = function (data) {
        try {
            Util.request(this.urlGetForm + "?id=" + data.id, 'GET', 'html', function (response) {
                $('#modal-default .modal-title').html("Ubah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                $("#modal-default").modal("show");
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        }
        catch (e) {
            console.error(e);
        }
    };
    return ParticipantUser;
}());
$(document).ready(function () {
    new ParticipantUser();
});
//# sourceMappingURL=participant-user.js.map