var SurveyParticipantDetail = /** @class */ (function () {
    function SurveyParticipantDetail() {
        this.urlSave = '/survey-participant/save';
        this.urlFinish = '/survey-participant/finish';
        this.init();
    }
    SurveyParticipantDetail.prototype.init = function () {
        var _this = this;
        try {

            $(document).on("click", "#finish-section", function (e) {
                $("#finish-section").attr("disabled", "disabled");
                _this.finish();
            });

            $(document).on("change", ".select-sequence", function (e) {
                var elem = $(this);
                var col = elem.data("col");
                var row = elem.data("row");
                var val = elem.val();

                $(".select-sequence-" + col).each(function () {
                    if (elem.val() == "") {
                        return false;
                    }

                    var subElem = $(this);
                    if (subElem.attr("id") != elem.attr("id")) {
                        if (subElem.val() == elem.val()) {
                            Util.error("Urutan " + subElem.val() + " sudah digunakan, silakan gunakan urutan lain");
                            elem.val("");
                            return false;
                        }
                    }
                })

                $(".numerical-row-" + row).val(6-val);
            })

            $(document).on("change", ".numerical-box", function (e) {
                //var elem = $(this);
                //var col = elem.data("col");
                //var row = elem.data("row");
                //var val = elem.val();

                //var total = 0;
                //$(".numerical-box-" + col).each(function () {
                //    var subElem = $(this);
                //    total += parseInt(subElem.val());
                //})

                //$("#numeric_total").html(total);
            })

            $(document).on("click", "#submit-answer", function (e) {
                $("#submit-answer").attr("disabled", "disabled");

                var error = "";
                var ranks = [];
                var values = [];
                $(".select-sequence").each(function () {
                    var subElem = $(this);
                    if (subElem.val() == "") {
                        error += "Urutan baris ke " + subElem.data("sequence") + " belum diisi<br/>";
                    }
                    ranks.push(subElem.val());
                })
                //$(".numerical-box").each(function () {
                //    var subElem = $(this);
                //    if (subElem.val() == "" || subElem.val() == "0") {
                //        error += "Nilai baris ke " + subElem.data("sequence") + " belum diisi<br/>";
                //    }
                //    values.push(subElem.val());
                //})
                $(".select-simple").each(function () {
                    var subElem = $(this);
                    if (subElem.val() == "") {
                        error += "Respon baris ke " + subElem.data("sequence") + " belum diisi<br/>";
                    }
                })

                if (error != "") {
                    Util.error(error)
                    return false;
                }

                //var total = 0;
                //$(".numerical-box").each(function () {
                //    var subElem = $(this);
                //    total += parseInt(subElem.val());
                //})

                //if (values.length > 0 && total != 25) {
                //    Util.error("Total nilai harus 25")
                //    $("#submit-answer").removeAttr("disabled");
                //    return false;
                //}

                error = "";
                for (var i = 0; i < ranks.length; i++) {
                    var rankI = parseInt(ranks[i]);
                    var valueI = parseInt(values[i]);
                    for (var j = 0; j < ranks.length; j++) {
                        var rankJ = parseInt(ranks[j]);
                        var valueJ = parseInt(values[j]);
                        if (rankI > rankJ) {
                            continue;
                        }

                        if (valueI < valueJ) {
                            error += "Nilai baris ke " + (i+1) + " harus lebih besar atau sama dengan baris ke " + (j+1) + "<br/>";
                            break;
                        }
                    }
                }

                if (error != "") {
                    Util.error(error)
                    $("#submit-answer").removeAttr("disabled");
                    return false;
                }
                _this.save();
            })
        }
        catch (e) {
            console.error(e);
            Util.error(e);
            $("#submit-answer").removeAttr("disabled");
        }
    };
    SurveyParticipantDetail.prototype.save = function () {
        var _this = this;
        try {
            var data = this.create();
            console.log(data);
            Util.requestJson(this.urlSave, 'post', 'json', function (response) {
                if (response != null) {
                    if (response.success) {
                        Util.success(response.message);
                        setTimeout(function () {
                            var next = $("#QuestionNext").val();
                            var participantID = $("#ParticipantID").val();
                            if (next != undefined && next != "") {
                                if (next != "0") {
                                    window.location.href = "/survey-participant/detail/" + participantID + "?mode=1&id=" + next;
                                } else {
                                    window.location.href = "/survey-participant/detail/" + participantID + "?mode=3";
                                }
                            }

                        }, 1000)
                    }
                    else {
                        Util.error(response.message);
                        $("#submit-answer").removeAttr("disabled");
                    }
                }
                else {
                    Util.error('Failed to get data #T7G985. Please try again.');
                    console.error('Failed to get data #T7G985. Please try again.');
                    $("#submit-answer").removeAttr("disabled");
                }
            }, function () {
            }, JSON.stringify(data));
        }
        catch (e) {
            console.error(e);
            Util.error(e);
            $("#submit-answer").removeAttr("disabled");
        }
    };
    SurveyParticipantDetail.prototype.finish = function () {
        var _this = this;
        try {
            var data = {
                ID: parseInt($('#ParticipantAnswerSheet').val())
            };
            console.log(data);
            Util.request(this.urlFinish, 'post', 'json', function (response) {
                if (response != null) {
                    if (response.success) {
                        Util.success(response.message);
                        setTimeout(function () {
                            window.location.href = "/survey-participant/detail/" + $('#ParticipantID').val() + "?mode=0";
                        }, 1000)
                    }
                    else {
                        Util.error(response.message);
                        $("#finish-section").removeAttr("disabled");
                    }
                }
                else {
                    Util.error('Failed to get data #T7G985. Please try again.');
                    console.error('Failed to get data #T7G985. Please try again.');
                    $("#finish-section").removeAttr("disabled");
                }
            }, function () {
            }, data);
        }
        catch (e) {
            console.error(e);
            Util.error(e);
            $("#finish-section").removeAttr("disabled");
        }
    };
    SurveyParticipantDetail.prototype.create = function () {
        try {
            var lines = [];
            $(".select-sequence").each(function () {
                var elem = $(this);
                var col = elem.data("col");
                var row = elem.data("row");
                var val = elem.val();
                lines.push({
                    ParticipantAnswerSheet: {
                        ID: parseInt($('#ParticipantAnswerSheet').val())
                    },
                    Question: {
                        ID: parseInt($('#Question').val())
                    },
                    AnswerType: 0,
                    QuestionSquence: col,
                    MatrixRowAnswerID: row,
                    NumericalBoxValue: parseFloat(val)
                });
            })
            $(".numerical-box").each(function () {
                var elem = $(this);
                var col = elem.data("col");
                var row = elem.data("row");
                var val = elem.val();
                lines.push({
                    ParticipantAnswerSheet: {
                        ID: parseInt($('#ParticipantAnswerSheet').val())
                    },
                    Question: {
                        ID: parseInt($('#Question').val())
                    },
                    AnswerType: 2,
                    QuestionSquence: col,
                    MatrixRowAnswerID: row,
                    NumericalBoxValue: parseFloat(val)
                });
            })
            $(".select-simple").each(function () {
                var elem = $(this);
                var row = elem.data("row");
                var val = elem.val();
                lines.push({
                    ParticipantAnswerSheet: {
                        ID: parseInt($('#ParticipantAnswerSheet').val())
                    },
                    Question: {
                        ID: parseInt($('#Question').val())
                    },
                    AnswerType: 1,
                    QuestionSquence: parseInt(val),
                    MatrixRowAnswerID: row,
                    SuggestedAnswerID: parseInt(val)
                });
            })
            var data = lines;
            return data;
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    return SurveyParticipantDetail;
}());
$(document).ready(function () {
    new SurveyParticipantDetail();
});
//# sourceMappingURL=survey-participant-detail.js.map