var Question = /** @class */ (function () {
    function Question() {
        this.urlGetData = "question/table-data-view";
        this.urlGetPaging = "question/table-paging-view";
        this.urlGetForm = "question/form-view";
        this.urlSave = 'question/save';
        this.urlDelete = 'question/delete';
        this.urlUploadImage = 'question/upload-image';
        this.currentPage = 1;
        this.init();
    }
    Question.prototype.init = function () {
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
    Question.prototype.initTable = function (page) {
        try {
            $('#table_list tbody').empty();
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
    Question.prototype.add = function () {
        var _this = this;
        try {
            Util.request(this.urlGetForm, 'GET', 'html', function (response) {
                $('#modal-default .modal-title').html("Tambah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                $("#modal-default").modal("show");
                $('#Description').summernote({
                    height: "150"
                });
                $(document).on("change", "#Attachment", function (e) {
                    var file = $(e.currentTarget).prop('files')[0];
                    _this.uploadImage($(e.currentTarget).attr("id"), file);
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
    Question.prototype.initForm = function () {
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
    Question.prototype.save = function () {
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
    Question.prototype.create = function () {
        try {
            var data = {
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
                Description: $('#Description').summernote('code'),
                Attachment: $("#Attachment").data("image")
            };
            return data;
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Question.prototype.delete = function (data) {
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
    Question.prototype.edit = function (data) {
        var _this = this;
        try {
            Util.request(this.urlGetForm + "?id=" + data.id, 'GET', 'html', function (response) {
                $('#modal-default .modal-title').html("Ubah Data");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                $("#modal-default").modal("show");
                $('#Description').summernote({
                    height: "150"
                });
                $(document).on("change", "#Attachment", function (e) {
                    var file = $(e.currentTarget).prop('files')[0];
                    _this.uploadImage($(e.currentTarget).attr("id"), file);
                });
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        }
        catch (e) {
            console.error(e);
        }
    };
    Question.prototype.uploadImage = function (id, image) {
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
                }
                else {
                    Util.alert(result.message);
                }
            },
            error: function (data) {
                console.log(data);
                Util.alert('Failed to get data. Please try again.');
            }
        });
    };
    return Question;
}());
$(document).ready(function () {
    new Question();
});
//# sourceMappingURL=question.js.map