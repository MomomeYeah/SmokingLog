function FlashClassSuccess(target) {
    target.addClass("update-success", 200, function () { target.removeClass("update-success", 200) });
}

function FlashUpdateFailure() {
    $("#UpdateFailure").show();
    $("#UpdateFailure").fadeOut(3000);
}

function SetRowDataRO(target) {
    target.find("span.view").removeClass("hidden")
    target.find("span.edit").addClass("hidden");
}

function AJAXUpdateSuccess(target, data) {
    if (data.Success) {
        var parentTR = target.parents("tr");
        var NumberOfCigarettes = data.Object.NumberOfCigarettes;
        parentTR.removeClass("danger");

        parentTR.find("span.view").text(NumberOfCigarettes);
        SetRowDataRO(parentTR);

        FlashClassSuccess(parentTR);

        $("#CigarettesToday").focus();
    } else {
        //alert(data.ErrorMessage);
        AJAXUpdateFailure(target);
    }
}

function AJAXUpdateFailure(target) {
    var parentTR = target.parents("tr");
    var OldNumberOfCigarettes = parentTR.find("span.view").text();

    parentTR.find("span.edit input").val(OldNumberOfCigarettes);
    SetRowDataRO(parentTR);

    FlashUpdateFailure();

    $("#CigarettesToday").focus();
}

$(document).ready(function () {
    $("#smokinglog-table tr td.smokinglog-data").click(function () {
        $("#smokinglog-table tr").removeClass("danger");
        $("#smokinglog-table tr td span.view").removeClass("hidden");
        $("#smokinglog-table tr td span.edit").addClass("hidden");

        $(this).find("span.view").addClass("hidden");
        $(this).find("span.edit").removeClass("hidden");
        $(this).find("span.edit input").select().focus();
    })

    $("span.edit input").blur(function () {
        var token = $('input[name="__RequestVerificationToken"]').val();
        var LogDate = $(this).parents("tr").find("td:first").text();
        var NumberOfCigarettes = $(this).val();
        var target = $(this);

        var data = {
            LogDate: LogDate,
            NumberOfCigarettes: NumberOfCigarettes,
            __RequestVerificationToken: token
        };

        $.ajax({
            type: "POST",
            url: '/Log/EditAjax',
            data: data,
            success: function (data) { AJAXUpdateSuccess(target, data) },
            error: function () { AJAXUpdateFailure(target) }
        });
    })
});