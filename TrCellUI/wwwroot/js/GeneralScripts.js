var gettimeout;

function getQueryStringByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function getQueryStringByNameNotDecode(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return results[2];
}

function GetAjaxCallAsync(url, data, responseType, obj, showloader, parameters, cache, settimeoutinterval) {



    var ajax = $.ajax({
        url: url,
        type: "POST",
        data: data,
        async: false,
        traditional: true,
        cache: !(typeof (cache) === 'undefined') ? cache : false,
        success: function (response, status, xhr) {

            if (responseType == "html") {
                obj.html(response, parameters);
            }
            else {
                response = response.response;
                var forbidden = false;
                if (!response.result) {

                    if (response.errorType == "exception") {
                        //showToaster(_('message/exception'), 'error');
                    }
                    else if (response.errorType == "unauthorized") {
                        // showToaster(_("message/unauthorized"), 'error');
                        window.setTimeout(function () {
                            window.location = __authorizationReturnURL;
                        }, 2000);
                    }
                    else {
                        // showToaster(response.errorMessage, 'warning');
                    }
                }

                obj(response, parameters);
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log("GetAjaxCall - " + errorThrown);
        },
        complete: function () {
            if (!(typeof (settimeoutinterval) === 'undefined') && settimeoutinterval > 0) {
                window.clearTimeout(gettimeout);
                gettimeout = window.setTimeout(function () {
                    GetAjaxCall(url, data, responseType, obj, showloader, parameters, cache, settimeoutinterval);
                }, settimeoutinterval);

            }

        }
    });
    return ajax;
}

function GetAjaxCall(url, data, responseType, obj, showloader, parameters, cache, settimeoutinterval) {



    var ajax = $.ajax({
        url: url,
        type: "POST",
        data: data,
        async: true,
        traditional: true,
        cache: !(typeof (cache) === 'undefined') ? cache : false,
        success: function (response, status, xhr) {

            if (responseType == "html") {
                obj.html(response, parameters);
            }
            else {
                response = response.response;
                var forbidden = false;
                if (!response.result) {

                    if (response.errorType == "exception") {
                        //showToaster(_('message/exception'), 'error');
                    }
                    else if (response.errorType == "unauthorized") {
                       // showToaster(_("message/unauthorized"), 'error');
                        window.setTimeout(function () {
                            window.location = __authorizationReturnURL;
                        }, 2000);
                    }
                    else {
                       // showToaster(response.errorMessage, 'warning');
                    }
                }

                obj(response, parameters);
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log("GetAjaxCall - " + errorThrown);
        },
        complete: function () {
            if (!(typeof (settimeoutinterval) === 'undefined') && settimeoutinterval > 0) {
                window.clearTimeout(gettimeout);
                gettimeout = window.setTimeout(function () {
                    GetAjaxCall(url, data, responseType, obj, showloader, parameters, cache, settimeoutinterval);
                }, settimeoutinterval);

            }

        }
    });
    return ajax;
}

function GetAjaxCall(url, data, responseType, obj, showloader, parameters, cache, settimeoutinterval, errorfunction) {

    var ajax = $.ajax({
        url: url,
        type: "POST",
        data: data,
        async: true,
        //timeout:240000,
        traditional: true,
        cache: !(typeof (cache) === 'undefined') ? cache : false,
        success: function (response, status, xhr) {
            if (responseType == "html") {
                obj.html(response, parameters);
            }
            else {
                response = response.response;
                var forbidden = false;
                if (!response.result) {
                    if (response.errorType == "exception") {
                        //showToaster(_('message/exception'), 'error');
                    }
                    else if (response.errorType == "unauthorized") {
                        //showToaster(_("message/unauthorized"), 'error');
                        window.setTimeout(function () {
                            window.location = __authorizationReturnURL;
                        }, 2000);
                    }
                    else {
                       // showToaster(response.errorMessage, 'warning');
                    }
                }

                obj(response, parameters);
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (typeof errorfunction === 'function') {
                errorfunction(XMLHttpRequest, textStatus, errorThrown);
            }

            console.log("GetAjaxCall - " + errorThrown);
        },
        complete: function () {
            if (!(typeof (settimeoutinterval) === 'undefined') && settimeoutinterval > 0) {
                window.clearTimeout(gettimeout);
                gettimeout = window.setTimeout(function () {
                    GetAjaxCall(url, data, responseType, obj, showloader, parameters, cache, settimeoutinterval);
                }, settimeoutinterval);

            }

        }
    });
    return ajax;
}


function GetAjaxCallWithTimeOut(url, data, responseType, obj, showloader, parameters, cache, settimeoutinterval, errorfunction, timeout) {
    var ajax = $.ajax({
        url: url,
        type: "POST",
        data: data,
        async: true,
        timeout: timeout,
        traditional: true,
        cache: !(typeof (cache) === 'undefined') ? cache : false,
        success: function (response, status, xhr) {

            if (responseType == "html") {
                obj.html(response, parameters);
            }
            else {
                response = response.response;
                var forbidden = false;
                if (!response.result) {
                    if (response.errorType == "exception") {
                        //showToaster(_('message/exception'), 'error');
                    }
                    else if (response.errorType == "unauthorized") {
                        //showToaster(_("message/unauthorized"), 'error');
                        window.setTimeout(function () {
                            window.location = __authorizationReturnURL;
                        }, 2000);
                    }
                    else {
                        //showToaster(response.errorMessage, 'warning');
                    }
                }

                obj(response, parameters);
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (typeof errorfunction === 'function') {
                errorfunction(XMLHttpRequest, textStatus, errorThrown);
            }

            console.log("GetAjaxCall - " + errorThrown);
        },
        complete: function () {
            if (!(typeof (settimeoutinterval) === 'undefined') && settimeoutinterval > 0) {
                window.clearTimeout(gettimeout);
                gettimeout = window.setTimeout(function () {
                    GetAjaxCall(url, data, responseType, obj, showloader, parameters, cache, settimeoutinterval);
                }, settimeoutinterval);

            }

        }
    });
    return ajax;
}


function GetAjaxCallForSP(url, data, responseType, obj, showloader, parameters, cache, settimeoutinterval, errorfunction) {
    var ajax = $.ajax({
        url: url,
        type: "POST",
        data: data,
        async: true,
        //timeout:240000,
        traditional: true,
        cache: !(typeof (cache) === 'undefined') ? cache : false,
        success: function (response, status, xhr) {
            if (responseType == "html") {
                obj.html(response, parameters);
            }
            else {
                //response = response.response;
                var forbidden = false;
                if (!response.result) {
                    if (response.errorType == "exception") {
                        //showToaster(_('message/exception'), 'error');
                    }
                    else if (response.errorType == "unauthorized") {
                       // showToaster(_("message/unauthorized"), 'error');
                        window.setTimeout(function () {
                            window.location = __authorizationReturnURL;
                        }, 2000);
                    }
                    else {
                        //showToaster(response.errorMessage, 'warning');
                    }
                }

                obj(response, parameters);
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (typeof errorfunction === 'function') {
                errorfunction(XMLHttpRequest, textStatus, errorThrown);
            }

            console.log("GetAjaxCall - " + errorThrown);
        },
        complete: function () {
            if (!(typeof (settimeoutinterval) === 'undefined') && settimeoutinterval > 0) {
                window.clearTimeout(gettimeout);
                gettimeout = window.setTimeout(function () {
                    GetAjaxCall(url, data, responseType, obj, showloader, parameters, cache, settimeoutinterval);
                }, settimeoutinterval);

            }

        }
    });
    return ajax;
}

function GetAjaxCallForSPWithTimeOut(url, data, responseType, obj, showloader, parameters, cache, settimeoutinterval, errorfunction, timeout) {
    var ajax = $.ajax({
        url: url,
        type: "POST",
        data: data,
        async: true,
        timeout: timeout,
        traditional: true,
        cache: !(typeof (cache) === 'undefined') ? cache : false,
        success: function (response, status, xhr) {
            if (responseType == "html") {
                obj.html(response, parameters);
            }
            else {
                //response = response.response;
                var forbidden = false;
                if (!response.result) {
                    if (response.errorType == "exception") {
                        //showToaster(_('message/exception'), 'error');
                    }
                    else if (response.errorType == "unauthorized") {
                        //showToaster(_("message/unauthorized"), 'error');
                        window.setTimeout(function () {
                            window.location = __authorizationReturnURL;
                        }, 2000);
                    }
                    else {
                        //showToaster(response.errorMessage, 'warning');
                    }
                }

                obj(response, parameters);
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (typeof errorfunction === 'function') {
                errorfunction(XMLHttpRequest, textStatus, errorThrown);
            }

            console.log("GetAjaxCall - " + errorThrown);
        },
        complete: function () {
            if (!(typeof (settimeoutinterval) === 'undefined') && settimeoutinterval > 0) {
                window.clearTimeout(gettimeout);
                gettimeout = window.setTimeout(function () {
                    GetAjaxCall(url, data, responseType, obj, showloader, parameters, cache, settimeoutinterval);
                }, settimeoutinterval);

            }

        }
    });
    return ajax;
}

function showBootstrapNotify(message, title, icon, state, progressbar, valign, align, animation) {
    var content = {};
    content.message = message;
    if (title != '')
        content.title = title;
    if (icon != '')
        content.icon = icon;

    var notify = $.notify(content, {
        type: state, //danger,warning...
        allow_dismiss: true,
        newest_on_top: true,
        mouse_over: false,
        showProgressbar: progressbar['showprogress'],
        spacing: 10,
        timer: 2000,
        placement: {
            from: valign,
            align: align
        },
        offset: {
            x: 30,
            y: 30
        },
        delay: 1000,
        z_index: 25000,
        animate: {
            enter: 'animated ' + animation, //swing,shake
            exit: 'animated ' + animation
        }
    });


    if (progressbar['showprogress']) {

        $.each(progressbar['message'], function (index, value) {
            setTimeout(function () {
                notify.update('message', value);
                notify.update('type', progressbar['type'][index]);
                notify.update('progress', progressbar['progress'][index]);
            }, progressbar['timeout'][index]);
        });
    }
}

function showToaster(title,message, state) {

    toastr.options = {
        "closeButton": true,
        "debug": true,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "showDuration": "300",
        "hideDuration": "1000000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    if (state == 'error')
        toastr["error"](message,title);
    else if (state == 'success')
        toastr["success"](message,title);
    else if (state == 'warning')
        toastr["warning"](message,title);
   // toastr["success"]("Thanks for your support", "Hello BBBootstrap");

    //toastr.options = {
    //    "closeButton": true,
    //    "debug": false,
    //    "newestOnTop": false,
    //    "progressBar": false,
    //    "positionClass": "toast-top-right",
    //    "preventDuplicates": false,
    //    "onclick": null,
    //    "showDuration": "300",
    //    "hideDuration": "1000",
    //    "timeOut": "7000",
    //    "extendedTimeOut": "1000",
    //    "showEasing": "swing",
    //    "hideEasing": "linear",
    //    "showMethod": "fadeIn",
    //    "hideMethod": "fadeOut"
    //};

    //if (state == 'error')
    //    toastr.error(message);
    //else if (state == 'success')
    //    toastr.success(message);
    //else if (state == 'warning')
    //    toastr.warning(message);
}

