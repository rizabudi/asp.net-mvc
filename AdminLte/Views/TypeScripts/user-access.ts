class UserAccess {
    private urlGetData = "/role/table-data-view";
    private urlGetPaging = "/role/table-paging-view";
    private urlGetForm = "/role/form-view";
    private urlSave = '/role/save';
    private urlDelete = '/role/delete';

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
                (<any>$("#modal-default")).modal("show")
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
            const data = {
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
            Util.request(this.urlGetForm + "?id=" + data.id, 'GET', 'html', (response) => {
                $('#modal-default .modal-title').html("Ubah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                (<any>$("#modal-default")).modal("show")
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        } catch (e) {
            console.error(e);
        }
    }
}

$(document).ready(function () {
    new UserAccess();
});