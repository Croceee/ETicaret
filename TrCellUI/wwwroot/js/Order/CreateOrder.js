var orderList = [];
var exp = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.";
$().ready(function () {
    //SetProducts(products_);
    //if (__userOrderGuid != null && __userOrderGuid.length>0) {
    //    GetAjaxCall("../Order/GetOrderByOrderGuid", { orderGuid=__userOrderGuid}, 'function', TempOrderList, false, null);

    //}
    GetAjaxCall("../Order/GetTempOrder", { userid: __userid }, 'function', TempOrderList, false, null);

});
function TempOrderList(data) {
    
    order = data.entity;
    orderList = order;
    console.log(order);
    orderDetailList = order.orderDetails;

    var div = "";
    var img = "";
    var product;
    var orderDetail;
    var totalPrice = 0;
    for (var i = 0; i < orderDetailList.length; i++) {
        orderDetail = orderDetailList[i]
        product = orderDetail.product;
        console.log(product.imageUrl);

        if (product == null || product.imageUrl == null || product.imageUrl.length < 1) {
            img = "photonotfound.png";
        } else {
            img = product.imageUrl
        }

        div += '<div class="card mb-3" style="width: 75vw; max-width: 1000px">' +
            '<div class="row g-0">   <div class="col-md-4"> <img src="../images/' + img + '" class="img-fluid rounded-start" alt="../images/' + img + '"/>' +
            ' </div> <div class="col-md-8"> <div class="card-body">  <h5 class="card-title">' + product.name + '</h5>' +
            ' <p class="card-text">' + exp + '</p> <div class="price-wrapper">  <span class="price">' + parseFloat(product.price * orderDetail.count).toFixed(2)  + '</span></div>' +
            '<div class="d-flex justify-content-end gap-2"><button  class="btn btn-outline-success px-3" style="width: 50px"><span class="fs-3">+</span> </button>' +
            ' <div class="d-flex align-items-center"> <span class="fs-3 px-2">' + orderDetail.count + '</span></div>' +
            '<button class="btn btn-outline-danger px-3" style="width: 50px"> <span class="fs-3">-</span></button><button class="btn btn-outline-danger px-3">Kaldır</button>' +
            ' </div> </div> </div> </div> </div>';
        totalPrice += product.price * orderDetail.count;
    }
    $('#divTempOrders').append(div);
    $('#divPrice').html(parseFloat(totalPrice).toFixed(2) + " ₺");

}
function ComplateOrder() {
    ClearInputs()
    $('#modalDetail').modal({ backdrop: 'static', keyboard: false });

}
function ClearInputs() {
    $('#txt_Address').val("");
    $('#txt_CreditCard').val("");

}
function onlyNumber(evt) {

    evt = (evt) ? evt : window.event
    var charCode = (evt.which) ? evt.which : evt.keyCode
    if (charCode > 31 && (charCode < 45 || charCode > 57)) {
        return false
    }
    return true
}
function SaveDetail() {
    var adress = $('#txt_Address').val();
    var creditCard = $('#txt_CreditCard').val();

    if (adress.length<1) {
        showToaster("Uyarı !", "Adres girilmelidir!", "warning");
        return;
    }

    if (creditCard.length < 1) {
        showToaster("Uyarı !", "Kredi Kartı Bilgisi girilmelidir", "warning");
        return;
    }

    var orders = [];
    var orderDetail = [];

    //console.log(orderList),
    //    console.log(order),
    //    console.log(orderDetailList);
    //return;
    orders.push({
        state: 1,
        address: adress,
        orderGuid: __userOrderGuid,
        userid: __userid,
        orderDetails: orderDetailList
    })

    GetAjaxCall("../Order/AddOrder", { order: JSON.stringify(orders[0]) }, 'function', SendOrderResult, false, null);

}
function SendOrderResult(data) {
    console.log(data);
    if (data.result) {
        window.location.href = "/Home";

    } else {
        showToaster("Uyarı !", data.errorMessage, "error");

    }
}
