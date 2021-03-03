var SurveyParticipantDetail = /** @class */ (function () {
    function SurveyParticipantDetail() {
        this.urlSave = '/survey-participant/save';
        this.init();
    }
    SurveyParticipantDetail.prototype.init = function () {
        var _this = this;
        try {
            $(document).on("click", ".btn-edit", function (e) {
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
            })

            $(document).on("change", ".numerical-box", function (e) {
                var elem = $(this);
                var col = elem.data("col");
                var row = elem.data("row");
                var val = elem.val();

                var total = 0;
                $(".numerical-box-" + col).each(function () {
                    var subElem = $(this);
                    total += parseInt(subElem.val());
                })

                $("#numeric_total").html(total);
            })

            $(document).on("click", "#submit-answer", function (e) {

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
                $(".numerical-box").each(function () {
                    var subElem = $(this);
                    if (subElem.val() == "") {
                        error += "Nilai baris ke " + subElem.data("sequence") + " belum diisi<br/>";
                    }
                    values.push(subElem.val());
                })

                if (error != "") {
                    Util.error(error)
                    return false;
                }

                var total = 0;
                $(".numerical-box").each(function () {
                    var subElem = $(this);
                    total += parseInt(subElem.val());
                })

                //if (total != 25) {
                //    Util.error("Total nilai harus 25")
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
                    return false;
                }
                _this.save();
            })
        }
        catch (e) {
            console.error(e);
            Util.error(e);
        }
    };
    SurveyParticipantDetail.prototype.save = function () {
        var _this = this;
        try {
            var data = this.create();
            console.log(data);
            Util.request(this.urlSave, 'post', 'json', function (response) {
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
                    }
                }
                else {
                    Util.error('Failed to get data #T7G985. Please try again.');
                    console.error('Failed to get data #T7G985. Please try again.');
                }
            }, function () {
            }, JSON.stringify(data));
        }
        catch (e) {
            console.error(e);
            Util.error(e);
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