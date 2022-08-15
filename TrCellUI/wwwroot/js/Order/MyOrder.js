var orderList = [];
var exp = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.";
$().ready(function () {
    //SetProducts(products_);
    //if (__userOrderGuid != null && __userOrderGuid.length>0) {
    //    GetAjaxCall("../Order/GetOrderByOrderGuid", { orderGuid=__userOrderGuid}, 'function', TempOrderList, false, null);

    //}
    GetAjaxCall("../Order/GetOrderByUserId", { userid: __userid }, 'function', TempOrderList, false, null);

});
function TempOrderList(data) {

    console.log(data);
    //return;
    orders = data.entities;
  
    //orderList = order;
    //console.log(order);
    //orderDetailList = order.orderDetails;

    var div = "";
    var img = "";
    var product;
    var orderDetail;
    var totalPrice = 0;
    var orderState = "";
    var productCount = 0;
    for (var i = 0; i < orders.length; i++) {
        totalPrice = 0;
        orderDetail = orders[i].orderDetails;
      
        img = orderDetail[0].product.imageUrl;

        console.log("***");
        for (var j = 0; j < orderDetail.length; j++) {
            console.log(orderDetail[j].product.price);
            console.log(orderDetail[j].count);
            totalPrice += orderDetail[j].count * orderDetail[j].product.price;
            productCount = productCount + 1;
        }

        switch (orders[i].state) {
            case 0: orderState = "Sepette";
                img ="shoppingbucket.jpg";
                break;
            case 1: orderState = "Sipariş Verildi"
                img = "kargo.jpg";
                break;
            case 2: orderState = "Teslim Edildi"
                img = "delivered.jpg";
                break;
            case -1: orderState = "İptal Edildi"
                img = "cancel.jpg";
                break;
            default:
        }

        div += '<div class="card mb-3" style="width: 75vw; max-width: 1000px">' +
            '<div class="row g-0">   <div class="col-md-4"> <img src="../images/' + img + '" class="img-fluid rounded-start" alt="../images/' + img + '"/>' +
            ' </div> <div class="col-md-8"> <div class="card-body">  <h5 class="card-title">' + orderState + '</h5>' +
            ' <p class="card-text">' + exp + '</p>  <p class="card-text">' + productCount + ' Adet Ürün </p><div class="price-wrapper">  <span class="price">' + parseFloat(totalPrice).toFixed(2) + ' ₺</span></div>' +
            ' </div> </div> </div> </div>';
        //totalPrice += product.price * orderDetail.count;
    }
    $('#divTempOrders').append(div);
    

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

    if (adress.length < 1) {
        showToaster("Uyarı !", "Adres girilmelidir!", "warning");
        return;
    }

    if (creditCard.length < 1) {
        showToaster("Uyarı !", "Kredi Kartı Bilgisi girilmelidir", "warning");
        return;
    }

    var orders = [];
    var orderDetail = [];

    orders.push({
        state: 1,
        address: "",
        orderGuid: __userOrderGuid,
        userid: __userid,
        orderDetails: orderList.orderDetail
    })

    GetAjaxCall("../Order/AddOrder", { order: JSON.stringify(orders[0]) }, 'function', SendOrderResult, false, null);

}
function SendOrderResult(data) {
    console.log(data);
    if (data.result) {
        window.location.href = "/Order/Order";

    } else {
        showToaster("Uyarı !", data.errorMessage, "error");

    }
}
