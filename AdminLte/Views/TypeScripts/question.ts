class Question {
    private urlGetData = "question/table-data-view";
    private urlGetPaging = "question/table-paging-view";
    private urlGetForm = "question/form-view";
    private urlSave = 'question/save';
    private urlDelete = 'question/delete';
    private urlUploadImage = 'question/upload-image';

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
            $('#table_list tbody').empty();
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
                (<any>$('#Description')).summernote({
                    height: "150"
                });
                $(document).on("change", "#Attachment", (e) => {
                    const file = $(e.currentTarget).prop('files')[0];
                    this.uploadImage($(e.currentTarget).attr("id"), file);
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
            const data = {
                ID: $('#ID').val(),
                Section: {
                    ID: $('#Section').val()
                },
                Sequence: $('#Sequence').val(),
                QuestionType: $('#QuestionType').val(),
                MatrixSubtype: $('#MatrixSubtype').val(),
                IsMandatory: $('#IsMandatory').is(":checked"),
                IsRandomAnswer: $('#IsRandomAnswer').is(":checked"),
                Title: $('#Title').val(),
                Description: (<any>$('#Description')).summernote('code'),
                Attachment: $("#Attachment").data("image")
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
                (<any>$("#modal-default")).modal("show");
                (<any>$('#Description')).summernote({
                    height: "150"
                });
                $(document).on("change", "#Attachment", (e) => {
                    const file = $(e.currentTarget).prop('files')[0];
                    this.uploadImage($(e.currentTarget).attr("id"), file);
                });
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        } catch (e) {
            console.error(e);
        }
    }

    private uploadImage(id, image) {
        var data = new FormData();
        data.append("image", image);
        $.ajax({
            url: this.urlUploadImage,
            cache: false,
            contentType: false,
            processData: false,
            data: data,
            type: "POST",
            success: function (result) {
                if (result.success) {
                    $("#img_" + id).attr("src", result.data.base64);
                    $("#" + id).data("image", result.data.filename);
                } else {
                    Util.alert(result.message);
                }
            },
            error: function (data) {
                console.log(data);
                Util.alert('Failed to get data. Please try again.');
            }
        });
    }
}

$(document).ready(function () {
    new Question();
});