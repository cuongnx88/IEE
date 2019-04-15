$(document).ready(function () {
    var height = $(window).height();
    $('#page-wrapper').css("min-height", height);

    //$('.i-checks').iCheck({
    //    checkboxClass: 'icheckbox_square-green',
    //    radioClass: 'iradio_square-green',
    //});

    $('input').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green'
    });

    $('#date-range .input-daterange').datepicker({
        keyboardNavigation: false,
        forceParse: false,
        autoclose: true
    });

    $('#data_date .input-group.date').datepicker({
        todayBtn: "linked",
        keyboardNavigation: false,
        forceParse: false,
        calendarWeeks: true,
        autoclose: true
    });


    $(".delete").on("click", function () {
        var url = $(this).attr('url');
        var id = $(this).attr('id');
        var obj = $(this).attr('object');
        var flag = confirm('Bạn có muốn xóa ' + obj + '?');
        if (flag) {
            $.ajax({
                url: url,
                type: 'POST',
                data: { id: id },
                dataType: 'json',
                success: function (result) {
                    $(this).closest('tr').remove();
                },
                error: function () { alert('Error!'); }

            });
        }
    });

    ModalPopup();

    RecordStepWinzad();
    StudentAutocomplate();
    StudentToClassAutocomplate();
    StaffAutocomplate();
    UniversityAutocomplate();
    TeacherToClassAutocomplate();
    Nestable_Doc();

    $(".document a.event").click(function () {
        if ($(this).closest('.doc-item').children('ol').css("display") === "none") {
            $(this).closest('.doc-item').children('ol').show();
            $(this).children("i.fa").removeClass("fa-minus");
            $(this).children("i.fa").addClass("fa-plus");
        }
        else {
            $(this).closest('.doc-item').children('ol').hide();
            $(this).children("i.fa").removeClass("fa-plus");
            $(this).children("i.fa").addClass("fa-minus");
        }

    });

});

function ModalPopup() {
    $.ajaxSetup({ cache: false });

    $("a[data-modal]").on("click", function (e) {
        $('#myModalContent').load(this.href, function () {
            $('#myModal').modal({
                keyboard: true
            }, 'show');
            bindForm(this);
        });
        return false;
    });
}
function Nestable_Doc() {

    $('#nestable_document').nestable({
        drop: false
    })
}

function bindForm(dialog) {
    $('form', dialog).submit(function () {
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    $('#myModal').modal('hide');
                    $('.replacetarget').load(result.url); //  Load data from the server and place the returned HTML into the matched element
                } else {
                    $('#myModalContent').html(result);
                    bindForm(dialog);
                }
            }
        });
        return false;
    });
}

function StudentAutocomplate() {
    $("#Student").autocomplete({
        source: function (request, response) {
            var student = new Array();
            $.ajax({
                async: false,
                cache: false,
                type: "POST",
                url: "/ttn_content/student/autocomplate",
                data: { Prefix: request.term },
                success: function (data) {
                    for (var i = 0; i < data.length; i++) {
                        student[i] = { label: data[i].Value, Id: data[i].Key };
                    }
                }
            });
            response(student);
        },
        select: function (event, ui) {
            $.ajax({
                cache: false,
                async: false,
                type: "POST",
                url: "/ttn_content/student/result",
                data: { "id": ui.item.Id },

                success: function (data) {
                    $("#UserId").val(data.Id)
                    action = data.Action;
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Failed to retrieve states.');
                }
            });
        }
    });
}

function StudentToClassAutocomplate() {
    $("#StudentToClass").autocomplete({
        source: function (request, response) {
            var student = new Array();
            $.ajax({
                async: false,
                cache: false,
                type: "POST",
                url: "/ttn_content/student/autocomplate",
                data: { Prefix: request.term },
                success: function (data) {
                    for (var i = 0; i < data.length; i++) {
                        student[i] = { label: data[i].Value, Id: data[i].Key };
                    }
                }
            });
            response(student);
        },
        select: function (event, ui) {
            $.ajax({
                cache: false,
                async: false,
                type: "POST",
                url: "/ttn_content/student/result",
                data: { "id": ui.item.Id },

                success: function (data) {
                    var rowCount = $('#StudenstInClass tr').length;
                    var index = rowCount + 1;
                    var newRow = '<tr>'
                        + '<td><label class="i-checks"><input type="checkbox" value="' + data.Id + '"  checked name="selectedStudents"></label> </td>'
                        + '<td>'
                        + data.Name
                        + '</td>'
                        + '<td>'
                        + data.Email
                        + '</td>'
                        + '<td>'
                        + data.Phone
                        + '</td>'
                        + '<td>'
                        + '</td>'
                        + '</tr>';

                    $("#StudenstInClass tbody").append(newRow);

                    $('.i-checks').iCheck({
                        checkboxClass: 'icheckbox_square-green',
                        radioClass: 'iradio_square-green',
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Failed to retrieve states.');
                }
            });
        }
    });
}

function TeacherToClassAutocomplate() {
    $("#TeacherToClass").autocomplete({
        source: function (request, response) {
            var student = new Array();
            $.ajax({
                async: false,
                cache: false,
                type: "POST",
                url: "/ttn_content/teacher/autocomplate",
                data: { Prefix: request.term },
                success: function (data) {
                    for (var i = 0; i < data.length; i++) {
                        student[i] = { label: data[i].Value, Id: data[i].Key };
                    }
                }
            });
            response(student);
        },
        select: function (event, ui) {
            $.ajax({
                cache: false,
                async: false,
                type: "POST",
                url: "/ttn_content/teacher/result",
                data: { "id": ui.item.Id },

                success: function (data) {
                    var rowCount = $('#TeacherstInClass tr').length;
                    var index = rowCount + 1;
                    var newRow = '<tr>'
                        + '<td><label class="i-checks"><input type="checkbox" value="' + data.Id + '"  checked name="selectedTeachers"></label> </td>'
                        + '<td>'
                        + data.Name
                        + '</td>'
                        + '<td>'
                        + data.Email
                        + '</td>'
                        + '<td>'
                        + data.Phone
                        + '</td>'
                        + '<td>'
                        + '</td>'
                        + '</tr>';

                    $("#TeacherstInClass tbody").append(newRow);

                    $('.i-checks').iCheck({
                        checkboxClass: 'icheckbox_square-green',
                        radioClass: 'iradio_square-green',
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Failed to retrieve states.');
                }
            });
        }
    });
}

function StaffAutocomplate() {
    $("#staff").autocomplete({
        source: function (request, response) {
            var student = new Array();
            $.ajax({
                async: false,
                cache: false,
                type: "POST",
                url: "/ttn_content/staff/autocomplate",
                data: { Prefix: request.term },
                success: function (data) {
                    for (var i = 0; i < data.length; i++) {
                        student[i] = { label: data[i].Value, Id: data[i].Key };
                    }
                }
            });
            response(student);
        },
        select: function (event, ui) {
            $.ajax({
                cache: false,
                async: false,
                type: "POST",
                url: "/ttn_content/staff/result",
                data: { "id": ui.item.Id },

                success: function (data) {

                    var rowCount = $('#recordstaff tr').length;
                    var index = rowCount + 1;

                    var newRow = '<tr><input type="hidden" name="users.Index" value="' + index + '" /><input type="hidden" name="users[' + index + '].UserId" value=' + data.Id + ' />'
                        + '<td>'
                        + data.Name
                        + '</td>'
                        + '<td>'
                        + data.Email
                        + '</td>'
                        + '<td style="text-align:center"><label class="i-checks" ><input type="radio" name="users[' + index + '].Type" value="0" ></label> </td>'
                        + '<td style="text-align:center"><label class="i-checks" ><input type="radio" name="users[' + index + '].Type" value="1" .></label> </td>'
                        + '<td style="text-align:center"><label class="i-checks" ><input type="radio" name="users[' + index + '].Type" value="2" /></label> </td>'
                        + '<td><a class="btn btn-outline btn-danger  dim staff-delete" title="Xóa dữ liệu"><i class="fa fa-trash-o"></i></a></td></tr>';

                    $("#recordstaff tbody").append(newRow);
                    action = data.Action;
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Failed to retrieve states.');
                }
            });
        }
    });
}

function UniversityAutocomplate() {
    $("#University").autocomplete({
        source: function (request, response) {
            var student = new Array();
            $.ajax({
                async: false,
                cache: false,
                type: "POST",
                url: "/ttn_content/university/autocomplate",
                data: { Prefix: request.term },
                success: function (data) {
                    for (var i = 0; i < data.length; i++) {
                        student[i] = { label: data[i].Value, Id: data[i].Key };
                    }
                }
            });
            response(student);
        },
        select: function (event, ui) {
            $.ajax({
                cache: false,
                async: false,
                type: "POST",
                url: "/ttn_content/university/result",
                data: { "id": ui.item.Id },

                success: function (data) {
                    var rowCount = $('#universities tr').length;
                    var index = rowCount + 1;
                    var newRow = '<tr>'
                        + '<td><label class="i-checks"><input type="checkbox" value="' + data.Id + '"  checked name="selectedUniversities"></label> </td>'
                        + '<td>'
                        + data.Name
                        + '</td>'
                        + '<td>'
                        + data.Email
                        + '</td>'
                        + '<td>'
                        + data.Address
                        + '</td>'
                        + '<td>'
                        + '<a href="#" url="/ttn_content/record/deleteuniversity/' + data.Id + '" object="trường đại học" id="3" class="btn btn-outline dim delete" title="Xóa dữ liệu"><i class="fa fa-trash-o"></i></a>'
                        + '</td>'
                        + '</tr>';

                    $("#universities tbody").append(newRow);

                    $('.i-checks').iCheck({
                        checkboxClass: 'icheckbox_square-green',
                        radioClass: 'iradio_square-green',
                    });

                    action = data.Action;
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Failed to retrieve states.');
                }
            });
        }
    });
}

function RecordStepWinzad() {
    $("#wizard").steps();
    $("#form").steps({
        bodyTag: "fieldset",
        onStepChanging: function (event, currentIndex, newIndex) {
            // Always allow going backward even if the current step contains invalid fields!
            if (currentIndex > newIndex) {
                return true;
            }
            //// Forbid suppressing "Warning" step if the user is to young
            //if (newIndex === 3 && Number($("#age").val()) < 18)
            //{
            //    return false;
            //}
            alert(currentIndex);
            var form = $(this);

            // Clean up if user went backward before
            if (currentIndex < newIndex) {
                // To remove error styles
                $(".body:eq(" + newIndex + ") label.error", form).remove();
                $(".body:eq(" + newIndex + ") .error", form).removeClass("error");
            }
            // Disable validation on fields that are disabled or hidden.
            form.validate().settings.ignore = ":disabled,:hidden";
            // Start validation; Prevent going forward if false
            return form.valid();
        },
        onStepChanged: function (event, currentIndex, priorIndex) {
            // Suppress (skip) "Warning" step if the user is old enough.
            if (currentIndex === 2 && Number($("#age").val()) >= 18) {
                $(this).steps("next");
            }

            // Suppress (skip) "Warning" step if the user is old enough and wants to the previous step.
            if (currentIndex === 2 && priorIndex === 3) {
                $(this).steps("previous");
            }
        },
        onFinishing: function (event, currentIndex) {
            var form = $(this);

            // Disable validation on fields that are disabled.
            // At this point it's recommended to do an overall check (mean ignoring only disabled fields)
            form.validate().settings.ignore = ":disabled";

            // Start validation; Prevent form submission if false
            return form.valid();
        },
        onFinished: function (event, currentIndex) {
            var form = $(this);

            // Submit form input
            form.submit();
        }
    }).validate({
        errorPlacement: function (error, element) {
            element.before(error);
        },
        rules: {
            confirm: {
                equalTo: "#password"
            }
        }
    });
}
window.preview = function (input) {
    if (input.files && input.files[0]) {
        // alert($("#Photo").val());
        $("img#Photo").remove();
        $(input.files).each(function () {
            var reader = new FileReader();
            reader.readAsDataURL(this);
            reader.onload = function (e) {

                $("#previewImg").append("<img class='thumb' src='" + e.target.result + "'>");
            }
        });
    }
}
function previewThumbnail(file, displayElement) {
    alert(displayElement);
    if (file.files && file.files[0]) {
        $(file.files).each(function () {
            var fileName = this.name;
            var reader = new FileReader();
            reader.readAsDataURL(this);
            reader.onload = function (e) {
                $("#" + displayElement).attr("src", "" + e.target.result);
            }
        });
    }
}
function previewAttachFiles(f) {
    if (f.files && f.files[0]) {
        $(f.files).each(function () {
            var fileName = this.name;
            //var reader = new FileReader();
            //reader.readAsDataURL(this);
            //reader.onload = function (e) {
            $("#fileName").append("<div>'" + fileName + "'</div>");
            //}
        });

    }
    $("#fileName").append("<a onclick='cancelFile()'>Hủy</a>")
}
function cancelFile() {
    $("#fileName").empty();
    $('#attachFiles').remove();
    $('#lblAttachFile').append('<input type="file" multiple="multiple" name="file" id="attachFiles" onchange="previewAttachFiles(this)" class="hide">');
}
$('#lblStatus').click(function () {

    alert(1);
    var $this = $('#Status');
    if ($('#lblStatus').hasClass("FalseStatus")) {
        $this.prop('checked', 'true');
        $('#lblStatus').css('background', 'url(/Content/img/ActiveStatus.png) no-repeat');
        $('#lblStatus').css('height', '31px');
        $('#lblStatus').css('width', '41px');
        $('#lblStatus').removeClass("FalseStatus");
        $('#lblStatus').addClass("TrueStatus");

    }
    else {

        $this.removeAttr('checked');
        $('#lblStatus').css('background', 'url(/Content/img/inActiveStatus.png) no-repeat');
        $('#lblStatus').css('height', '31px');
        $('#lblStatus').css('width', '41px');
        $('#lblStatus').removeClass("TrueStatus");
        $('#lblStatus').addClass("FalseStatus");
    }


});

$('#lblRightAnswer').click(function () {


    var $this = $('#IsRightAnswer');
    if ($('#lblRightAnswer').hasClass("FalseAnswer")) {
        $this.prop('checked', 'true');
        $('#lblRightAnswer').css('background', 'url(/Content/img/trueIcon.png) no-repeat');
        $('#lblRightAnswer').css('height', '31px');
        $('#lblRightAnswer').css('width', '41px');
        $('#lblRightAnswer').removeClass("FalseAnswer");
        $('#lblRightAnswer').addClass("TrueAnswer");

    }
    else {

        $this.removeAttr('checked');
        $('#lblRightAnswer').css('background', 'url(/Content/img/falseIcon.png) no-repeat');
        $('#lblRightAnswer').css('height', '31px');
        $('#lblRightAnswer').css('width', '41px');
        $('#lblRightAnswer').removeClass("TrueAnswer");
        $('#lblRightAnswer').addClass("FalseAnswer");
    }


});
$('#HasInputAnswer').click(function () {
    alert(1);
    if ($("#HasInputAnswer").prop('checked')) {
        $('[mark]').hide();
    }
    //if ($("#AnswerType").val() == "1") {

    //}
    else {
        $('[mark]').show();

    }
});