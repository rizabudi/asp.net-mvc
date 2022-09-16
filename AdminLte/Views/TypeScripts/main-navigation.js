var MainNavigation = /** @class */ (function () {
    function MainNavigation() {
        this.urlLogout = "Identity/Account/Logout?returnUrl=%2Fhome";
        this.init();
    }
    MainNavigation.prototype.init = function () {
        try {
            $(document).on("click", "#logout", function (e) {
                $("#form-logout").trigger("submit");
            });
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    MainNavigation.prototype.logOut = function () {
        try {
            if (!Util.formCheck()) {
                return;
            }
            Util.request(this.urlLogout, 'post', 'html', function (response) {
                if (response != null) {
                    if (response.success) {
                        Util.success(response.message);
                        $("#modal-default").modal("hide");
                        setTimeout(function () {
                            var BASE_URL = $('#BASE_URL').attr("href");
                            window.location.href = BASE_URL + "profile";
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
            }, {});
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    return MainNavigation;
}());
$(document).ready(function () {
    new MainNavigation();
});
//# sourceMappingURL=main-navigation.js.map