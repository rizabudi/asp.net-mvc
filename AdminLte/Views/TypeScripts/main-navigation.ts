class MainNavigation {
    private urlLogout = "Identity/Account/Logout?returnUrl=%2Fhome";

    constructor() {
        this.init();
    }
    private init() {
        try {
            $(document).on("click", "#logout", (e) => {
                $("#form-logout").trigger("submit")
            });

        } catch (e) {
            console.error(e);
            Util.error(e);
        }
    }
    private logOut() {
        try {
            if (!Util.formCheck()) {
                return;
            }
            Util.request(this.urlLogout, 'post', 'html', (response) => {
                if (response != null) {
                    if (response.success) {
                        Util.success(response.message);
                        (<any>$("#modal-default")).modal("hide")
                        setTimeout(function () {
                            var BASE_URL = $('#BASE_URL').attr("href");
                            window.location.href = BASE_URL + "profile";
                        }, 2000);
                    } else {
                        Util.error(response.message);
                    }
                } else {
                    Util.error('Failed to get data #T7G985. Please try again.');
                    console.error('Failed to get data #T7G985. Please try again.');
                }
            }, () => {
            }, {});
        } catch (e) {
            console.error(e);
            Util.error(e);
        }
    }
}

$(document).ready(function () {
    new MainNavigation();
});