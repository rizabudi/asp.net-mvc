var Util = (function (scope) {
    scope.request = function (url, type, dataType, successCallback, failureCallback, data) {
        try {
            if (data === undefined || data === null) {
                data = {};
            }
            $.ajax({
                type: type, //GET or POST
                dataType: dataType, //html or json
                processData: true,
                data: data,
                url: BASE_URL + url,
                statusCode: processResponse(successCallback, failureCallback),
                complete: function () {
                }
            });
        } catch (e) {
            console.error(e)
        }
    }

    scope.requestJson = function (url, type, dataType, successCallback, failureCallback, data) {
        try {
            if (data === undefined || data === null) {
                data = {};
            }
            $.ajax({
                type: type, //GET or POST
                dataType: dataType, //html or json
                processData: true,
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                data: data,
                url: BASE_URL + url,
                statusCode: processResponse(successCallback, failureCallback),
                complete: function () {
                }
            });
        } catch (e) {
            console.error(e)
        }
    }


    scope.alert = function (msg) {
        $.notify(msg);
    }

    scope.info = function (msg) {
        toastr.info(msg)
    }

    scope.error = function (msg) {
        toastr.error(msg)
    }

    scope.success = function (msg) {
        toastr.success(msg)
    }

    scope.formCheck = function () {
        var isValid = true;
        var msg = "";
        $(".text-required").each(function() {
            if ($(this).val() == "") {
                msg += $(this).data('label') + " harus diisi" + "<br/>";
            }
        })

        $(".number-required").each(function () {
            if ($(this).val() == "") {
                msg += $(this).data('label') + " harus diisi" + "<br/>";
            }
        })

        $(".select-required").each(function () {
            if ($(this).attr("disabled") == null && ($(this).val() == "" || $(this).val() == "-1")) {
                msg += $(this).data('label') + " harus diisi" + "<br/>";
            }
        })

        $("input[type='email']").each(function () {
            if ($(this).val() != "") {
                if (!validateEmail($(this).val())) {
                    msg += $(this).data('label') + " tidak valid" + "<br/>";
                }
            }
        })

        if (msg != "") {
            Util.error(msg)
        }

        return msg == ""
    }

    function validateEmail(email) {
        const re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    }

    function processResponse(successCallback, failureCallback) {
        return {
            200: successCallback,
            401: handleUnauthorizedError,
            403: handleForbiddenError,
            404: failureCallback,
            500: failureCallback
        };
    }

    function handleUnauthorizedError() {
        location.reload();
    }
    function handleForbiddenError() {
        toastr.error('No permission to access.');
    }

    return scope;
}) ({});