var Profile = /** @class */ (function () {
    function Profile() {
        this.urlGetForm = "/profile/form-view";
        this.urlSave = '/profile/save';
        this.init();
    }
    Profile.prototype.init = function () {
        var _this = this;
        try {
            $(document).on("click", "#edit", function (e) {
                _this.edit();
            });
            $(document).on("keydown", "#WorkDuration", function (e) {
                return e.keyCode !== 190;
            });
            this.initForm();
            if ($("#IsEdit").val() == "1") {
                this.edit();
            }
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Profile.prototype.initForm = function () {
        var _this = this;
        try {
            $('#save_form').click(function () {
                _this.save();
            });
            $('#close_form').click(function () {
                $("#modal-default").modal("hide");
            });
            $(document).on("change", "#Entity", function (e) {
                var entityID = $(e.currentTarget).val();
                Util.request("/profile/entity-select-view?entity=" + entityID, 'GET', 'html', function (response) {
                    $('#SubEntity').html(response);
                }, function () {
                    Util.error('Failed to get data. Please try again');
                });
            });
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Profile.prototype.save = function () {
        try {
            if (!Util.formCheck()) {
                return;
            }
            if ($('#Age').val() == "0") {
                Util.error("Umur harus lebih besar dari 0");
                return;
            }
            if ($('#WorkDuration').val() == "0") {
                Util.error("Durasi kerja harus lebih besar dari 0");
                return;
            }
            var data = this.create();
            Util.request(this.urlSave, 'post', 'json', function (response) {
                if (response != null) {
                    if (response.success) {
                        Util.success(response.message);
                        $("#modal-default").modal("hide");
                        setTimeout(function () {
                            window.location.href = "/profile";
                        }, 2000);
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
    Profile.prototype.create = function () {
        try {
            var data = {
                UserId: $('#UserId').val(),
                Name: $('#Name').val(),
                Email: $('#Email').val(),
                Phone: $('#Phone').val(),
                Sex: $('#Sex').val() == "1",
                //BirthDate: $('#BirthDate').find("input").val(),
                Age: $('#Age').val(),
                WorkDuration: $('#WorkDuration').val(),
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
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    Profile.prototype.edit = function () {
        try {
            Util.request(this.urlGetForm, 'GET', 'html', function (response) {
                $('#modal-default .modal-title').html("Lengkapi Identitas Anda");
                $('#modal-default .modal-body').empty();
                $('#modal-default .modal-body').append(response);
                $("#modal-default").modal("show");
                //var date = new Date();
                //date.setFullYear(date.getFullYear() - 17);
                //var isEmpty = $('#BirthDate').find("input").val().toString() == "";
                //(<any>$('#BirthDate')).datetimepicker({
                //    format: 'YYYY-MM-DD',
                //    maxDate: date,
                //    date: new Date($('#BirthDate').find("input").val().toString())
                //});
                //if (isEmpty) {
                //    $("div#BirthDate .datetimepicker-input").val("")
                //}
                $('#WorkDuration').attr("step", 1);
                $('#WorkDuration').attr("pattern", "[0-9]");
                $('#Age').attr("step", 1);
                $('#Age').attr("pattern", "[0-9]");
            }, function () {
                Util.error('Failed to get data. Please try again');
            });
        }
        catch (e) {
            console.error(e);
        }
    };
    return Profile;
}());
$(document).ready(function () {
    new Profile();
});
//# sourceMappingURL=profile.js.map