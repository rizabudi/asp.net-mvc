var SurveyQuestion = /** @class */ (function () {
    function SurveyQuestion() {
        this.urlGetData = "/survey-question/table-data-view";
        this.urlGetPaging = "/survey-question/table-paging-view";
        this.urlGetForm = "/survey-question/form-view";
        this.urlSave = '/survey-question/save';
        this.urlDelete = '/survey-question/delete';
        this.currentPage = 1;
        this.init();
    }
    SurveyQuestion.prototype.init = function () {
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
    SurveyQuestion.prototype.initTable = function (page) {
        try {
            Util.request(this.urlGetData + "?page=" + page + "&surveyID=" + $("#QuestionPackage").val(), 'GET', 'html', function (response) {
                $('#table_list tbody').empty();
                $('#table_list tbody').append(response);
            }, function () {
                console.error('Failed to get data. Please try again');
                Util.error('Failed to get data. Please try again');
            });
            Util.request(this.urlGetPaging + "?page=" + page + "&surveyID=" + $("#QuestionPackage").val(), 'GET', 'html', function (response) {
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
    SurveyQuestion.prototype.add = function () {
        try {
            Util.request(this.urlGetForm + "?surveyID=" + $("#QuestionPackage").val(), 'GET', 'html', function (response) {
                $('#modal-default .modal-title').html("Tambah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                $("#modal-default").modal("show");
                $('#Date').daterangepicker({
                    locale: {
                        format: 'YYYY-MM-DD',
                        separator: ' s/d '
                    },
                    minDate: $("#PeriodStart").val(),
                    maxDate: $("#PeriodEnd").val(),
                });
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    SurveyQuestion.prototype.initForm = function () {
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
    SurveyQuestion.prototype.save = function () {
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
    SurveyQuestion.prototype.create = function () {
        try {
            var data = {
                ID: $('#ID').val(),
                Question: {
                    ID: $('#Question').val()
                },
                QuestionPackage: {
                    ID: $('#QuestionPackage').val()
                }
            };
            return data;
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    SurveyQuestion.prototype.delete = function (data) {
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
    SurveyQuestion.prototype.edit = function (data) {
        try {
            Util.request(this.urlGetForm + "?id=" + data.id, 'GET', 'html', function (response) {
                $('#modal-default .modal-title').html("Ubah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                $("#modal-default").modal("show");
                $('#Date').daterangepicker({
                    locale: {
                        format: 'YYYY-MM-DD',
                        separator: ' s/d '
                    },
                    minDate: $("#PeriodStart").val(),
                    maxDate: $("#PeriodEnd").val(),
                });
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        }
        catch (e) {
            console.error(e);
        }
    };
    return SurveyQuestion;
}());
$(document).ready(function () {
    new SurveyQuestion();
});
//# sourceMappingURL=survey-question.js.map