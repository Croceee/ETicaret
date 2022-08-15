var productList = [];
var selectedProductList = [];
var exp = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.";
var isOrderUpdated = true;
$().ready(function () {
    GetProducts();

});
function GetProducts() {
    GetAjaxCall("../Product/GetProducts", {}, 'function', ProductList, false, null);

}

function ProductList(data) {

    productList = data.entities;
    console.log(productList);

    var div = "";
    var product;
    var perc = 0;
    var img = "";
    for (var i = 0; i < productList.length; i++) {
        product = productList[i];
        perc = i % 3;
        img = product.imageUrl;
        if (img != null && img.length<1) {
            img ="photonotfound.png";
        }
        if (i == 0) {

            div += ' <div class="row row-gap">';
        }
        if (i > 0 && perc == 0) {

            div += '</div>';
            div += ' <div class="row row-gap">';
        }
        div += '<div class="col-md-4"><div class="card" style="width: 18rem; margin: auto;height:100%"> <img' +
            ' style="width:17.5rem !important;" src="../images/' + img + '" class="card-img-top" alt="../images/' + img + '"/>' +
            '<div class="card-body" style="display: flex;flex-direction:column;justify-content: flex-end"><h5 class="card-title">' + product.name + '</h5><p class="card-text">' + exp + '</p>' +
            '<div class="price-wrapper"><span class="price">' + product.price + ' ₺</span></div><a  class="btn btn-primary" onclick="AddBucket(' + product.productId + ')" ' +
            '> Sepete Ekle</a > </div></div></div> ';         

        if (i == product.length - 1) {

            div += '</div>';
        }
    }
    $('#divProductList').append(div);
}

function AddBucket(productId) {
    //if (!isOrderUpdated) {
    //    showToaster("Uyarı !", "Önceki siparişiniz işleniyor", "warning");

    //    return;
    //}
    isOrderUpdated = false;
    var orderCount = $('#spnBucketCount').val();
    if (orderCount == '') orderCount = 0;
    orderCount = parseInt(orderCount) + 1;

    $('#spnBucketCount').html(orderCount);
    $('#spnBucketCount').val(orderCount);

 //   selectedProductList.push(productList.filter(x => x.productId == productId)[0]);
    __tempOrderList.push(productList.filter(x => x.productId == productId)[0]);
    var orders = [];
    var orderDetail = [];

    for (var i = 0; i < __tempOrderList.length; i++) {
        orderDetail.push({
            productId: __tempOrderList[i].productId,
            count: 1
        });
    }
    orders.push({
        state: 0,
        address: "",
        orderGuid: __userOrderGuid,
        userid: __userid,
        orderDetails: orderDetail
    })

    GetAjaxCallAsync("../Order/AddOrder", { order: JSON.stringify(orders[0]) }, 'function', GoCreateOrderResult, false, null);
}
function GoCreateOrder() {
    window.location.href = "/Order/CreateOrder";

    //console.log(selectedProductList);

    //var products =[];
    //products.push({
    //    productId: 1,
    //    name: "name",
    //    code: "code",
    //    deleted: 0,
    //    productTypeId: 1

    //});

    //GetAjaxCall("../Order/SendCreateOrder", { products: JSON.stringify(products[0]) }, 'function', GoCreateOrderResult, false, null);

    //GetAjaxCall("../Order/CreateOrder", { }, 'function', ProductList, false, null) JSON.stringify(selectedProductList);


}
function GoCreateOrderResult(data) {
    //window.location.href = "/Order/CreateOrder";
    isOrderUpdated = true;

}