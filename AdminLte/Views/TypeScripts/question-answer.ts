class QuestionAnswer {
    private urlGetData = "question-answer/table-data-view";
    private urlGetPaging = "question-answer/table-paging-view";
    private urlGetForm = "question-answer/form-view";
    private urlSave = 'question-answer/save';
    private urlDelete = 'question-answer/delete';
    private urlSubVerticalDimention = 'sub-vertical-dimention/select-option';

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
            $(document).on("change", "#Type", (e) => {
                const id = $(e.currentTarget).val();
                if (id == "1") {
                    $("#VerticalDimention").val("-1");
                    $("#SubVerticalDimention").val("-1");
                    $("#HorizontalDimention").val("-1");
                    $("#VerticalDimention").attr('disabled', 'disabled');
                    $("#SubVerticalDimention").attr('disabled', 'disabled');
                    $("#HorizontalDimention").attr('disabled', 'disabled');

                    if ($("#Construct").val() == "CULTURE") {
                        $("#MatrixValue").removeAttr('disabled');
                    } else if ($("#Construct").val() == "PERFORMANCE") {
                        $("#div_IsUnFavorable").hide();
                    }
                } else {
                    $("#VerticalDimention").removeAttr('disabled');
                    $("#SubVerticalDimention").removeAttr('disabled');
                    $("#HorizontalDimention").removeAttr('disabled');

                    if ($("#Construct").val() == "CULTURE") {
                        $("#MatrixValue").attr('disabled', 'disabled');
                        $("#MatrixValue").val("-1");
                    } else if ($("#Construct").val() == "PERFORMANCE") {
                        $("#div_IsUnFavorable").show();
                    }
                }
            });
            $(document).on("change", "#VerticalDimention", (e) => {
                const id = $(e.currentTarget).val();
                Util.request(this.urlSubVerticalDimention + "?verticalDimentionID=" + id, 'GET', 'html', (response) => {
                    $('#SubVerticalDimention').empty();
                    $('#SubVerticalDimention').append(response);
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
            $('#table_list tbody').empty();
            Util.request(this.urlGetData + "?page=" + page + "&questionID=" + $("#Question").val(), 'GET', 'html', (response) => {
                $('#table_list tbody').empty();
                $('#table_list tbody').append(response);
            }, function () {
                    console.error('Failed to get data. Please try again');
                    Util.error('Failed to get data. Please try again');
            });
            Util.request(this.urlGetPaging + "?page=" + page + "&questionID=" + $("#Question").val(), 'GET', 'html', (response) => {
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
            Util.request(this.urlGetForm + "?questionID=" + $("#Question").val(), 'GET', 'html', (response) => {
                $('#modal-default .modal-title').html("Tambah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                (<any>$("#modal-default")).modal("show");
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
            var type = $("#Type").val();
            var question = $("#Question").val();
            var construct = $("#Construct").val();
            const data = {
                ID: $('#ID').val(),
                Sequence: $('#Sequence').val(),
                Value: $('#Value').val(),
                Type: $('#Type').val(),
                Weight: $('#Weight').val(),
                AnswerScore: $('#AnswerScore').val(),
                Question: {
                    ID: type == "1" ? question : 0,
                    Section: {
                        Construct: construct
                    }
                },
                MatrixQuestion: {
                    ID: type == "2" ? question : 0
                },
                VerticalDimention: {
                    ID: $('#VerticalDimention').val()
                },
                SubVerticalDimention: {
                    ID: $('#SubVerticalDimention').val()
                },
                HorizontalDimention: {
                    ID: $('#HorizontalDimention').val()
                },
                MatrixValue: $("#MatrixValue").val(),
                IsUnFavorable: $('#IsUnFavorable').is(":checked"),
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
            Util.request(this.urlGetForm + "?id=" + data.id + "&questionID=" + $("#Question").val(), 'GET', 'html', (response) => {
                $('#modal-default .modal-title').html("Ubah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                (<any>$("#modal-default")).modal("show");

                if ($("#Type").val() == "1") {
                    $("#VerticalDimention").val("-1");
                    $("#SubVerticalDimention").val("-1");
                    $("#HorizontalDimention").val("-1");
                    $("#VerticalDimention").attr('disabled', 'disabled');
                    $("#SubVerticalDimention").attr('disabled', 'disabled');
                    $("#HorizontalDimention").attr('disabled', 'disabled');

                    if ($("#Construct").val() == "CULTURE") {
                        $("#MatrixValue").removeAttr('disabled');
                    } else if ($("#Construct").val() == "PERFORMANCE") {
                        $("#div_IsUnFavorable").hide();
                    }

                } else {
                    $("#VerticalDimention").removeAttr('disabled');
                    $("#SubVerticalDimention").removeAttr('disabled');
                    $("#HorizontalDimention").removeAttr('disabled');

                    if ($("#Construct").val() == "CULTURE") {
                        $("#MatrixValue").attr('disabled', 'disabled');
                        $("#MatrixValue").val("-1");
                    } else if ($("#Construct").val() == "PERFORMANCE") {
                        $("#div_IsUnFavorable").show();
                    }
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
    new QuestionAnswer();
});