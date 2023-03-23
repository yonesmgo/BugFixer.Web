function OpenAvatarInput() {
    $("#UserAvatar").click();
}
function UploadUserAvatar(url) {
    var avatarinput = document.getElementById("UserAvatar");
    if (avatarinput.files.length) {
        var file = avatarinput.files[0];
        var formData = new FormData();
        formData.append("formFile", file);
        $.ajax({
            url: url,
            type: "post",
            data: formData,
            contentType: false,
            processData: false,
            beforeSend: function () {
                StartLouading();
            },
            success: function (response) {
                if (response.status === "Success") {
                    location.reload();
                }
                else {
                    swal({
                        title: "خطا",
                        text: "فرمت فایل ارسال شده درست نمیباشد ",
                        icon: "error",
                        button: "باشه"
                    })
                }
            },
            error: function () {
                swal({
                    title: "خطا",
                    text: "عملیات با خطا مواجه شد  لطفا دوباره تلاش بفرمایید ",
                    icon: "error",
                    button: "باشه"
                });
            }
        })
    }
}
function StartLouading(selector = 'body') {
    $(selector).waitMe({
        effect: 'bounce',
        text: 'لطفا منتظر باشید ',
        bg: "rgba(255, 255, 255, 0.7)",
        color: "#000"
    });
}
function endLouading(selector = 'body') {
    $(selector).waitMe('hide');
}
$('#CountryId').on("change", function () {
    var CountryId = $('#CountryId').val();
    var s = $("#CountryId").attr("data-url");
    if (CountryId != "" && CountryId.length) {
        $.ajax({
            url: s,
            type: "get",
            data: {
                CountryId: CountryId
            },
            beforeSend: function () {
                StartLouading();
            },
            success: function (response) {
                endLouading();
                console.log(response);
                $("#CityId option").remove();
                $("#CityId").prop("disabled", false);
                for (var city of response) {
                    $("#CityId").append(`<option value="${city.id}">${city.title}</option>`)
                }
            },
            error: function () {
            }
        })
    }
    else {
        $("#CityId option").remove();
        $("#CityId").append(`<option value="">لطفا شهر مورد نظر را انتخاب بفرمایید</option>`)
        $("#CityId").prop("disabled", true);
    }
})

var datepickers = document.querySelectorAll(".datepicker");
if (datepickers.length) {
    for (datepicker of datepickers) {
        var id = $(datepicker).attr("id");
        kamaDatepicker(id, {
            placeholder: 'مثال : 1400/01/01',
            twodigit: true,
            closeAfterSelect: false,
            forceFarsiDigits: true,
            markToday: true,
            markHolidays: true,
            highlightSelectedDay: true,
            sync: true,
            gotoToday: true
        });
    }
}
var editorPush = [];
var Editor = document.querySelectorAll(".editor");
if (Editor.length) {
    for (edit of Editor) {
        ClassicEditor
            .create(edit, {
                licenseKey: '',
                simpleUpload: {
                    uploadUrl: '/Home/UploadEditorImage'
                }
            })
            .then(editor => {
                window.editor = editor;
                editorPush.push(editor);
            })
            .catch(error => {
                console.log(error);
            });
    }
}
//var magicsuggests = document.querySelectorAll(".magicsuggest");
//if (magicsuggests.length) {
//    $('head').append($('<link rel="stylesheet" type="text/css" />').attr('href', '/common/magicsuggest/magicsuggest.css'));
//    $.getScript("/common/magicsuggest/magicsuggest.js",
//        function (data, textStatus, jqxhr) {
//            for (magic of magicsuggests) {

//            }
//        });
//}
$(function () {
    if ($('#CountryId').val === '') {
        $('#CityId').prop("disabled", true);
    }
})


function SubmitQuestionForm() {
    $('#Filter_Form').submit();
}
function SubmitFilterFormPagonation(pageid) {
    $('#CurrentPage').val(pageid);
    $('#Filter_Form').submit();
}

function SubmitTagForm() {
    $('#Filter_Form').submit();
}

function AnswerQuestionFormDone(response) {

    endLouading('#submit-comment');
    if (response.status == 'Success') {
        swal("اعلان", "پاسخ شما ثبت شد", "success");
    }
    else if (response.status == 'EmptyAnswer') {
        swal("هشدار", "متن پاسخ نمیتواند خالی باشد ", "warning")
    }
    else if (response.status == 'Error') {
        swal("اعلان", "خطایی رخ داده است  لطفا مجددا تلاش بفرمایید", "error")
    }
    for (var editor of editorPush) {
        editor.setData('');
    }
    $("#AnswersBox").load(location.href + " #AnswersBox");

    $('html, body').animate({
        scrollTop: $("#AnswersBox").offset().top
    }, 1000);
}
