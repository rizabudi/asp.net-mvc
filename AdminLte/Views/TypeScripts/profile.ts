class Profile {
    private urlGetForm = "/profile/form-view";
    private urlSave = '/profile/save';

    constructor() {
        this.init();
    }
    private init() {
        try {
            $(document).on("click", "#edit", (e) => {
                this.edit();
            });
            this.initForm();

            if ($("#IsEdit").val() == "1")
            {
                this.edit();
            }

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
                        setTimeout(function () {
                            window.location.href = "/profile";
                        }, 2000);
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
                Entity: {
                    ID: $("#Entity").val()
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
                }
            };
            return data;
        } catch (e) {
            console.error(e);
            Util.error(e);
        }
    }
    private edit() {
        try {
            Util.request(this.urlGetForm, 'GET', 'html', (response) => {
                $('#modal-default .modal-title').html("Lengkapi Identitas Anda");
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
    new Profile();
});