var UserAccess = /** @class */ (function () {
    function UserAccess() {
        this.urlGetData = "/role/table-data-view";
        this.urlGetPaging = "/role/table-paging-view";
        this.urlGetForm = "/role/form-view";
        this.urlSave = '/role/save';
        this.urlDelete = '/role/delete';
        this.currentPage = 1;
        this.init();
    }
    UserAccess.prototype.init = function () {
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
            this.initForm();
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    UserAccess.prototype.initTable = function (page) {
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
    UserAccess.prototype.add = function () {
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
    UserAccess.prototype.initForm = function () {
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
    UserAccess.prototype.save = function () {
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
    UserAccess.prototype.create = function () {
        try {
            var data = {
                ID: $('#ID').val(),
                Name: $('#Name').val(),
                Access_MasterData_JenisSurvei: $("#Access_MasterData_JenisSurvei").is(":checked"),
                Access_MasterData_Konstruk: $("#Access_MasterData_Konstruk").is(":checked"),
                Access_MasterData_DimensiVertical: $("#Access_MasterData_DimensiVertical").is(":checked"),
                Access_MasterData_DimensiHorizontal: $("#Access_MasterData_DimensiHorizontal").is(":checked"),
                Access_MasterData_StrukturOrganisasi_Entitas: $("#Access_MasterData_StrukturOrganisasi_Entitas").is(":checked"),
                Access_MasterData_StrukturOrganisasi_LevelJabatan: $("#Access_MasterData_StrukturOrganisasi_LevelJabatan").is(":checked"),
                Access_MasterData_Periode: $("#Access_MasterData_Periode").is(":checked"),
                Access_MasterData_Pertanyaan: $("#Access_MasterData_Pertanyaan").is(":checked"),
                Access_MasterData_DaftarSurvei: $("#Access_MasterData_DaftarSurvei").is(":checked"),
                Access_Penjadwalan_PenjadwalanSurvei: $("#Access_Penjadwalan_PenjadwalanSurvei").is(":checked"),
                Access_Penjadwalan_PenjadwalanPeserta: $("#Access_Penjadwalan_PenjadwalanPeserta").is(":checked"),
                Access_PengaturanPengguna_HakAkses: $("#Access_PengaturanPengguna_HakAkses").is(":checked"),
                Access_PengaturanPengguna_PenggunaUmum: $("#Access_PengaturanPengguna_PenggunaUmum").is(":checked"),
                Access_PengaturanPengguna_PenggunaKhusus: $("#Access_PengaturanPengguna_PenggunaKhusus").is(":checked")
            };
            return data;
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    UserAccess.prototype.delete = function (data) {
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
    UserAccess.prototype.edit = function (data) {
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
    return UserAccess;
}());
$(document).ready(function () {
    new UserAccess();
});
//# sourceMappingURL=user-access.js.map