class Participant {
    private urlGetData = "participant/table-data-view";
    private urlGetPaging = "participant/table-paging-view";
    private urlGetForm = "participant/form-view";
    private urlSave = 'participant/save';
    private urlDelete = 'participant/delete';
    private urlDownload = 'participant/download';

    private currentPage = 1;
    private search = "";

    constructor() {
        this.init();
    }
    private init() {
        try {
            this.initTable(this.currentPage, this.search);
            $('#add').click(() => {
                this.add();
            });
            $('#search').click(() => {
                (<any>$("#modal-search")).modal("show")
            });
            $(document).on("click", ".page-link", (e) => {
                const idx = $(e.currentTarget).data('dt-idx');
                this.currentPage = idx;
                this.initTable(idx, this.search);
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
            $(document).on("change", "#IsCanRetake", (e) => {
                var isChecked = $("#IsCanRetake").is(":checked");
                if (isChecked) {
                    $("#div_MaxRetake").show();
                } else {
                    $("#div_MaxRetake").hide();
                }
            });
            $('#download').click(() => {
                window.open(this.urlDownload + "?scheduleID=" + $("#Schedule").val() + "&finish=" + $("#Finish").val() + "&search=" + this.search)
            });

            this.initForm();
            $('#search').show();
            $('#download').show();

        } catch (e) {
            console.error(e);
            Util.error(e);
        }
    }
    private initTable(page, search) {
        try {
            Util.request(this.urlGetData + "?page=" + page + "&scheduleID=" + $("#Schedule").val() + "&finish=" + $("#Finish").val() + "&search=" + search, 'GET', 'html', (response) => {
                $('#table_list tbody').empty();
                $('#table_list tbody').append(response);
            }, function () {
                console.error('Failed to get data. Please try again');
                Util.error('Failed to get data. Please try again');
            });
            Util.request(this.urlGetPaging + "?page=" + page + "&scheduleID=" + $("#Schedule").val() + "&finish=" + $("#Finish").val() + "&search=" + search, 'GET', 'html', (response) => {
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
            Util.request(this.urlGetForm + "?scheduleID=" + $("#Schedule").val(), 'GET', 'html', (response) => {
                $('#modal-default .modal-title').html("Tambah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                (<any>$("#modal-default")).modal("show");
                $("#div_MaxRetake").hide();
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
                this.initTable(this.currentPage, this.search)
            });
            $('#search_form').click(() => {
                (<any>$("#modal-search")).modal("hide")
                $("#divSearchResult").show();
                var search = $("#Search").val().toString();
                this.search = search;
                $("#span_Search").html("<b>\"" + search + "\"</b>");
                this.initTable(1, this.search)
            });
            $('#close_search_form').click(() => {
                (<any>$("#modal-search")).modal("hide")
                this.initTable(this.currentPage, this.search)
            });
            $('#clear_search').click(() => {
                $("#Search").val("");
                $("#divSearchResult").hide();
                this.search = "";
                this.initTable(1, this.search)
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
                        this.initTable(this.currentPage, this.search)
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
                        this.initTable(this.currentPage, this.search)
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
                $("#div_MaxRetake").hide();
                if ($("#IsCanRetake").is(":checked")) {
                    $("#div_MaxRetake").show();
                }
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        } catch (e) {
            console.error(e);
        }
    }
}

$(document).ready(function () {
    new Participant();
});