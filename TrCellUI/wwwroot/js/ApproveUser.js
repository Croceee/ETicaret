$().ready(function () {
    var user_ID = getQueryStringByName('UI');
    var password = getQueryStringByNameNotDecode('Ph');
    console.log(password);

    if (user_ID != null && user_ID != password) {
        var url = "../Home/ApproveUserFromMail";
        GetAjaxCall(url, { passwordHash: password, user_id: user_ID.split("?")[0] }, 'function', ApproveUserResult, false, null);
    }
    
});


function ApproveUserResult(data) {
    console.log(data);
    if (data.result) {
        window.location.href = "/Home/Login?ApproveUser";
        return;
    } else {
        showToaster("Uyarı !", "E-Posta sisteme kayıtlı değil", "warning");
        return;
    }
}

