function Delete(controller, action, data) {
    var dialogResult = confirm("Are you sure that you want to perform this action?");
    if (dialogResult) {
        jQuery.post("/" + controller + "/" + action, data, function (data) {
            if (data == "true") {
                alert("Success");
                location.reload(true);
            } else {
                alert("Error");
            }
        }).fail(function () {
            alert("Error");
        });

    }
}

function RenewExternalLink(fileID) {
    var dialogResult = confirm("Are you sure that you want to perform this action?");
    if (dialogResult) {
        jQuery.post("/UserStorage/RenewExternalLink", "fileID=" + fileID, function (data) {
            if (data == "true") {
                alert("Success");
                location.reload(true);
            } else {
                alert("Error");
            }
        }).fail(function () {
            alert("Error");
        });
    }
}

function ChangeShareDropdown(elem, htmlID) {
    var target = document.getElementById(htmlID);
    if (elem.value == "L") {
        target.disabled = true;
    } else {
        target.disabled = false;
    }
}