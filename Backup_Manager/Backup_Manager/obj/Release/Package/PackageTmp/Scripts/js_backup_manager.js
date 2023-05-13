var chk_ids = [];
var tasks_count = 0;

function Get_Selected_Row(DataTableID, rowData) {
    var row = $("#" + DataTableID).find(".dtactive");
    if (row.length > 0)
        return $("#" + DataTableID).DataTable().row(row[0]).data()[rowData];
    else
        return 0;
}

Load_Backup_Jobs();
function Load_Backup_Jobs() {
    var dtBackups = $('#tbl_backup_jobs').DataTable({
        destroy: true,
        responsive: true,
        processing: false,
        search: true,
        stateSave: true,
        info: true,
        searching: false,
        paging: false,
        info: false,
        order: [[1, "asc"], [2, "asc"]],
        lengthMenu: [[5, 10, 20, -1], [5, 10, 20, "All"]],
        ajax: {
            "url": "/Home/LoadSettings",
            "type": "GET"
        },
        columns:
            [
                {
                    data: "obj1", title: "", render: function (data) {
                        //return '<div class="form-check"><input class="form-check-input" name="task' + data + '" type="checkbox" id="' + data + '"" /></div>'
                        if (data.selected == "true") {
                            return '<div class="form-check"><input class="form-check-input" name="task" type="checkbox" id="' + data.rowID + '" onclick="select_task(' + data.rowID + ')" checked="checked" /></div>'
                        } else {
                            return '<div class="form-check"><input class="form-check-input" name="task" type="checkbox" id="' + data.rowID + '" onclick="select_task(' + data.rowID + ')" /></div>'
                        }
                    }
                },
                { data: "output_filename", title: "Output Filename", sClass: "dt-body-left", orderable: false },
                { data: "source_file", title: "Backup Source", sClass: "dt-body-left", orderable: false },
                { data: "destination", title: "Backup Destination", sClass: "dt-body-left", orderable: false },
                //{ data: "row_id", title: "row id", sClass: "dt-body-left", orderable: false },
                {
                    data: "row_id", title: "Action", render: function (data) {
                        return "<div class='row justify-content-center'>" +
                            "<div class='dropdown dropleft'><button type='button' class='btn btn-success btn-sm' data-toggle='dropdown'>" +
                            "<i class='fa fa-bars'></i></button> <div class='dropdown-menu'><div class='container fluid'> " +
                            "<a id='btn_update' onclick='update_task(" + data + ")' class='btn btn-warning fa fa-edit col-sm-12' style='margin-bottom: 3px; margin-top: 3px; color: white'>&nbspUpdate</a><br />" +
                            "<a id='btn_delete' onclick='delete_task(" + data + ")' class='btn btn-danger fa fa-trash col-sm-12' style='margin-bottom: 3px; margin-top: 3px; color: white;'>&nbspDelete</a><br />" +
                            "</div></div></div ></div>";
                    }
                },
                {
                    data: "destination", visible: false, render: function (data) {
                        return $('[name="destination"]').val(data);
                    }
                },
                {
                    data: "IsDatedFolder", visible: false, render: function (data) {
                        return $('[name="IsDatedFolder"]').prop("checked", data);
                    }
                },
                {
                    data: "IsDatedSuffix", visible: false, render: function (data) {
                        return $('[name="IsDatedSuffix"]').prop("checked", data);
                    }
                },
                {
                    data: "IsDefaultNetwork", visible: false, render: function (data) {
                        return $('[name="IsDefaultNetwork"]').prop("checked", data);
                    }
                },
                {
                    data: "IsScheduled", visible: false, render: function (data) {
                        return $('[name="IsScheduled"]').prop("checked", data);
                    }
                },
                {
                    data: "IsDaily", visible: false, render: function (data) {
                        return $('[name="IsDaily"]').prop("checked", data);
                    }
                },
            ]
    });

    $("#tbl_backup_jobs").find("tbody").off().on('click', 'tr', function (e) {
        e.preventDefault();

    });
}

function clear_textfields() {
    //$('[name="destination"]').val("");
    $('[name="output_filename"]').val("");
    $('[name="source_file"]').val("");
    $("#row_id").val("");
}

function get_settings_details() {
    var IsDatedFolder = $('[name="IsDatedFolder"]').is(":checked");
    var IsDatedSuffix = $('[name="IsDatedSuffix"]').is(":checked");
    var IsDefaultNetwork = $('[name="IsDefaultNetwork"]').is(":checked");
    var IsScheduled = $('[name="IsScheduled"]').is(":checked");
    var IsDaily = $('[name="IsDaily"]').is(":checked");

    var obj = {
        IsDatedFolder: IsDatedFolder,
        IsDatedSuffix: IsDatedSuffix,
        IsDefaultNetwork: IsDefaultNetwork,
        IsScheduled: IsScheduled,
        IsDaily: IsDaily
    };
    return obj;
}

function get_task_details() {
    var row_id = $("#row_id").val();
    var output_filename = $('[name="output_filename"]').val();
    var source_file = $('[name="source_file"]').val();
    var destination = $('[name="destination"]').val();

    var obj = {
        rowID: row_id,
        filename: output_filename,
        source: source_file,
        destination: destination
    };
    return obj;
}

function save_settings(trans) {
    var form = $("#form_settings");
    $.ajax({
        url: '/Home/Save_Settings',
        data: { setting: get_settings_details(), task: get_task_details() },
        type: 'POST',
        success: function (res) {
            if (trans == "update") {
                if (res.success == true) {
                    Swal.fire('Success!', 'Successfully saved.', 'success');
                }
                else {
                    Swal.fire('Error!', res.message, 'error');
                }
                $("#mod-settings").modal("hide");
                $("#mod-add-task").modal("hide");
                clear_textfields();
                Load_Backup_Jobs();
            }
        },
    });
}

function save_task() {
    save_settings("update");
}

function update_task(id) {
    $("#mod-add-task").modal("show");
    $.ajax({
        url: '/Home/Update_Task',
        data: { task_id: id },
        type: 'GET',
        success: function (res) {
            if (res.data != null) {
                $('[name="output_filename"]').val(res.data.filename);
                $('[name="source_file"]').val(res.data.source);
                $('[name="destination"]').val(res.data.destination);
                $("#row_id").val(res.data.rowID);
            }
        },
    });
}

function delete_task(id) {
    Swal.fire({
        text: "Do you want to delete this record?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#dc3545',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Home/Delete_Task',
                data: { task_id: id },
                type: 'GET',
                success: function (res) {
                    if (res.success == true) {
                        if (res.success == true) {
                            Swal.fire('Success!', 'Successfully deleted.', 'success');
                        }
                        else {
                            Swal.fire('Error!', res.message, 'error');
                        }
                        Load_Backup_Jobs();
                    }
                },
            });
        }
    });
}

function select_task(id) {
    $.ajax({
        url: '/Home/Select_Task',
        data: { task_id: id },
        type: 'POST',
        success: function (res) {
            if (res.success == true) {
                Load_Backup_Jobs();
            } else {
                Swal.fire('Error!', res.message, 'error');
            }
        },
    });
}

function select_all(is_true) {
    var chk_ids = [];
    //$("#tbl_backup_jobs input[type=checkbox]").prop("checked", true);
    //$("#tbl_backup_jobs input:checkbox[name='task']:checked").each(function () {
    $("#tbl_backup_jobs input[type=checkbox]").each(function () {
        chk_ids.push($(this).attr("id"));
    });
    //alert(chk_ids);
    $.ajax({
        url: '/Home/Select_All_Task',
        data: { task_ids: chk_ids, is_true: is_true },
        type: 'POST',
        success: function (res) {
            if (res.success == true) {
                Load_Backup_Jobs();
            } else {
                Swal.fire('Error!', res.message, 'error');
            }
        },
    });
}

function start_backup() {
    var chk_ids = [];
    $("#tbl_backup_jobs input:checkbox[name='task']:checked").each(function () {
        chk_ids.push($(this).attr("id"));
    });
    if (chk_ids.length != 0) {
        $("#mod-preparing").modal("show");
        $.ajax({
            url: '/Home/Start_Backup',
            data: { task_ids: chk_ids },
            type: 'POST',
            success: function (res) {
                if (res.success == true) {
                    //Swal.fire('Success!', res.message, 'success');
                } else {
                    Swal.fire('Error!', res.message, 'error');
                }
            },
        });
    }
    else {
        Swal.fire('Warning!', 'Select atleast one(1) Backup jobs.', 'warning');
    }
}


$(function () {
    // Reference the auto-generated proxy for the hub.
    var progress = $.connection.progressHub;
    console.log(progress);

    // Create a function that the hub can call back to display messages.
    progress.client.AddProgress = function (message, percentage, successMessage) {
        ProgressBarModal("show", message + " " + percentage);
        $('#ProgressMessage').width(percentage);
        if (percentage == "100%") {
            ProgressBarModal("", successMessage);
        }
    };

    $.connection.hub.start().done(function () {
        var connectionId = $.connection.hub.id;
        console.log(connectionId);
    });
});

