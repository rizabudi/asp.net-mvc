﻿class VerticalDimention {
    private urlGetData = "/vertical-dimention/table-data-view";
    private urlGetPaging = "/vertical-dimention/table-paging-view";
    private urlGetForm = "/vertical-dimention/form-view";
    private urlSave = '/vertical-dimention/save';
    private urlDelete = '/vertical-dimention/delete';
    private urlEdit = '/vertical-dimention/edit';
    private urlSearch = '/vertical-dimention/search';

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
            $('#search').click(() => {
                const keyword = $('#keyword').val();
                this.search(keyword);
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
                Section: {
                    ID: $('#Section').val()
                },
                Name: $('#Name').val(),
                Description: $('#Description').val(),
                Sequence: $('#Sequence').val(),
                ValueDriverDimention: $('#ValueDriverDimention').val(),
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
    private search(keyword) {
        try {
            const data = { keyword: keyword };
            Util.request(this.urlSearch, 'GET', 'html', (response) => {
                const currentKeyWord = $('#keyword').val();
                if (currentKeyWord === keyword) {
                    $('#table_list tbody').empty();
                    $('#table_list tbody').append(response);
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
    new VerticalDimention();
});