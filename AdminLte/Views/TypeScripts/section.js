var Section = /** @class */ (function () {
    function Section() {
        this.urlGetData = "/section/table-data-view";
        this.urlGetPaging = "/section/table-paging-view";
        this.urlGetForm = "/section/form-view";
        this.urlSave = '/section/save';
        this.urlDelete = '/section/delete';
        this.urlEdit = '/section/edit';
        this.urlSearch = '/section/search';
        this.currentPage = 1;
        this.init();
    }
    Section.prototype.init = function () {
        var _this = this;
        try {
            this.initTable(this.currentPage);
            $('#add').click(function () {
                _this.add();
            });
            $('#search').click(function () {
                var keyword = $('#keyword').val();
                _this.search(keyword);
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
    Section.prototype.initTable = function (page) {
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
    Section.prototype.add = function () {
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
    Section.prototype.initForm = function () {
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
    Section.prototype.save = function () {
        var _this = this;
        try {
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
    Section.prototype.create = function () {
        try {
            var data = {
                ID: $('#ID').val(),
                Assesment: {
                    ID: $('#Assesment').val()
                },
                Name: $('#Name').val(),
                Description: $('#Description').val(),
                Sequence: $('#Sequence').val(),
                IsRandom: $('#IsRandom').is(":checked"),
                Construct: $('#Construct').val(),
            };
            return data;
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Section.prototype.delete = function (data) {
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
    Section.prototype.edit = function (data) {
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
    Section.prototype.search = function (keyword) {
        try {
            var data = { keyword: keyword };
            Util.request(this.urlSearch, 'GET', 'html', function (response) {
                var currentKeyWord = $('#keyword').val();
                if (currentKeyWord === keyword) {
                    $('#table_list tbody').empty();
                    $('#table_list tbody').append(response);
                }
            }, function () {
                Util.alert('Failed to get data. Please try again.');
                console.error('Failed to get data #T09576. Please try again.');
            }, data);
        }
        catch (e) {
            console.error(e);
        }
    };
    return Section;
}());
$(document).ready(function () {
    new Section();
});
//# sourceMappingURL=section.js.map