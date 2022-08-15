$().ready(function () {

    //setTimeout(Test, 5000);
    Test();

});
function Test()
{
    GetAjaxCall("../Order/GetTempOrder", { userid: __userid }, 'function', TempOrderListForBucket, false, null);
}



function TempOrderListForBucket(data) {
    
    order = data.entity;
    if (order != null) {
        orderDetailList = order.orderDetails;
        var product;
        orderCount = 0;
        //return;
        for (var i = 0; i < orderDetailList.length; i++) {
            orderDetail = orderDetailList[i];
            product = orderDetailList[i]

            for (var j = 0; j < orderDetail.count; j++) {
                __tempOrderList.push(product);
            }
            orderCount += orderDetail.count;

        }

        console.log(__tempOrderList);
        $('#spnBucketCount').val(orderCount);
        $('#spnBucketCount').html(orderCount);
    }
   

}