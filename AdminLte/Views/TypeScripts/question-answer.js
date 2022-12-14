var QuestionAnswer = /** @class */ (function () {
    function QuestionAnswer() {
        this.urlGetData = "question-answer/table-data-view";
        this.urlGetPaging = "question-answer/table-paging-view";
        this.urlGetForm = "question-answer/form-view";
        this.urlSave = 'question-answer/save';
        this.urlDelete = 'question-answer/delete';
        this.urlSubVerticalDimention = 'sub-vertical-dimention/select-option';
        this.currentPage = 1;
        this.init();
    }
    QuestionAnswer.prototype.init = function () {
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
            $(document).on("change", "#Type", function (e) {
                var id = $(e.currentTarget).val();
                if (id == "1") {
                    $("#VerticalDimention").val("-1");
                    $("#SubVerticalDimention").val("-1");
                    $("#HorizontalDimention").val("-1");
                    $("#VerticalDimention").attr('disabled', 'disabled');
                    $("#SubVerticalDimention").attr('disabled', 'disabled');
                    $("#HorizontalDimention").attr('disabled', 'disabled');
                    if ($("#Construct").val() == "CULTURE") {
                        $("#MatrixValue").removeAttr('disabled');
                    }
                    else if ($("#Construct").val() == "PERFORMANCE") {
                        $("#div_IsUnFavorable").hide();
                    }
                }
                else {
                    $("#VerticalDimention").removeAttr('disabled');
                    $("#SubVerticalDimention").removeAttr('disabled');
                    $("#HorizontalDimention").removeAttr('disabled');
                    if ($("#Construct").val() == "CULTURE") {
                        $("#MatrixValue").attr('disabled', 'disabled');
                        $("#MatrixValue").val("-1");
                    }
                    else if ($("#Construct").val() == "PERFORMANCE") {
                        $("#div_IsUnFavorable").show();
                    }
                }
            });
            $(document).on("change", "#VerticalDimention", function (e) {
                var id = $(e.currentTarget).val();
                Util.request(_this.urlSubVerticalDimention + "?verticalDimentionID=" + id, 'GET', 'html', function (response) {
                    $('#SubVerticalDimention').empty();
                    $('#SubVerticalDimention').append(response);
                }, function () {
                    console.error('Failed to get data. Please try again');
                    Util.error('Failed to get data. Please try again');
                });
            });
            this.initForm();
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    QuestionAnswer.prototype.initTable = function (page) {
        try {
            $('#table_list tbody').empty();
            Util.request(this.urlGetData + "?page=" + page + "&questionID=" + $("#Question").val(), 'GET', 'html', function (response) {
                $('#table_list tbody').empty();
                $('#table_list tbody').append(response);
            }, function () {
                console.error('Failed to get data. Please try again');
                Util.error('Failed to get data. Please try again');
            });
            Util.request(this.urlGetPaging + "?page=" + page + "&questionID=" + $("#Question").val(), 'GET', 'html', function (response) {
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
    QuestionAnswer.prototype.add = function () {
        try {
            Util.request(this.urlGetForm + "?questionID=" + $("#Question").val(), 'GET', 'html', function (response) {
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
    QuestionAnswer.prototype.initForm = function () {
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
    QuestionAnswer.prototype.save = function () {
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
    QuestionAnswer.prototype.create = function () {
        try {
            var type = $("#Type").val();
            var question = $("#Question").val();
            var construct = $("#Construct").val();
            var data = {
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
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    QuestionAnswer.prototype.delete = function (data) {
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
    QuestionAnswer.prototype.edit = function (data) {
        try {
            Util.request(this.urlGetForm + "?id=" + data.id + "&questionID=" + $("#Question").val(), 'GET', 'html', function (response) {
                $('#modal-default .modal-title').html("Ubah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                $("#modal-default").modal("show");
                if ($("#Type").val() == "1") {
                    $("#VerticalDimention").val("-1");
                    $("#SubVerticalDimention").val("-1");
                    $("#HorizontalDimention").val("-1");
                    $("#VerticalDimention").attr('disabled', 'disabled');
                    $("#SubVerticalDimention").attr('disabled', 'disabled');
                    $("#HorizontalDimention").attr('disabled', 'disabled');
                    if ($("#Construct").val() == "CULTURE") {
                        $("#MatrixValue").removeAttr('disabled');
                    }
                    else if ($("#Construct").val() == "PERFORMANCE") {
                        $("#div_IsUnFavorable").hide();
                    }
                }
                else {
                    $("#VerticalDimention").removeAttr('disabled');
                    $("#SubVerticalDimention").removeAttr('disabled');
                    $("#HorizontalDimention").removeAttr('disabled');
                    if ($("#Construct").val() == "CULTURE") {
                        $("#MatrixValue").attr('disabled', 'disabled');
                        $("#MatrixValue").val("-1");
                    }
                    else if ($("#Construct").val() == "PERFORMANCE") {
                        $("#div_IsUnFavorable").show();
                    }
                }
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        }
        catch (e) {
            console.error(e);
        }
    };
    return QuestionAnswer;
}());
$(document).ready(function () {
    new QuestionAnswer();
});
//# sourceMappingURL=question-answer.js.map