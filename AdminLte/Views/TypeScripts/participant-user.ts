class ParticipantUser {
    private urlGetData = "participant-user/table-data-view";
    private urlGetPaging = "participant-user/table-paging-view";
    private urlGetForm = "participant-user/form-view";
    private urlSave = 'participant-user/save';
    private urlDelete = 'participant-user/delete';

    private currentPage = 1;
    private search = "";
    private order = "";
    private sort = "";

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
                const id = $(e.currentTarget).data('id-strng');
                const data = { id: id };
                this.delete(data);
            });
            $(document).on("click", ".btn-edit", (e) => {
                const id = $(e.currentTarget).data('id-strng');
                const data = { id: id };
                this.edit(data);
            });
            $(document).on("click", ".sortTable", (e) => {
                this.sort = $(e.currentTarget).data("sort");
                this.order = $(e.currentTarget).data("order");

                $(".sortTableAsc").addClass("sortTableBoth");
                $(".sortTableAsc").removeClass("sortTableAsc");
                $(".sortTableDesc").addClass("sortTableBoth");
                $(".sortTableDesc").removeClass("sortTableDesc");

                if (this.order == "") {
                    $(e.currentTarget).removeClass("sortTableBoth")
                    $(e.currentTarget).addClass("sortTableAsc")
                    $(e.currentTarget).data("order", "asc");

                    this.order = "asc";
                } else if (this.order == "asc") {
                    $(e.currentTarget).removeClass("sortTableAsc")
                    $(e.currentTarget).addClass("sortTableDesc")
                    $(e.currentTarget).data("order", "desc");

                    this.order = "desc";
                } else if (this.order == "desc") {
                    $(e.currentTarget).removeClass("sortTableDesc")
                    $(e.currentTarget).addClass("sortTableAsc")
                    $(e.currentTarget).data("order", "asc");

                    this.order = "asc";
                }

                this.initTable(this.currentPage, this.search)
            });

            this.initForm();
            $('#search').show();

        } catch (e) {
            console.error(e);
            Util.error(e);
        }
    }
    private initTable(page, search) {
        try {
            Util.request(this.urlGetData + "?page=" + page + "&search=" + search + "&order=" + this.order + "&sort=" + this.sort, 'GET', 'html', (response) => {
                $('#table_list tbody').empty();
                $('#table_list tbody').append(response);
            }, function () {
                    console.error('Failed to get data. Please try again');
                    Util.error('Failed to get data. Please try again');
            });
            Util.request(this.urlGetPaging + "?page=" + page + "&search=" + search, 'GET', 'html', (response) => {
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
            $(document).on("change", "#Entity", (e) => {
                var entityID = $(e.currentTarget).val();
                Util.request("entity/select-view?entity=" + entityID, 'GET', 'html', (response) => {
                    $('#SubEntity').html(response);
                }, function () {
                    Util.error('Failed to get data. Please try again');
                });
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
    new ParticipantUser();
});