function loadData() {
    $.post("/Foods/GetData")
        .done(function (data) {
            result_int = data;
            if (result_int != 0) {
                $("#result_int").html(result_int);
            }
            else {
                document.getElementById('result_int').textContent = "";
            }
        });
}

function updatePrices(id, kolvo) {
    $.post("/Orders/UpdatePrices", { id: id, amount: kolvo })
        .done(function (data) {

            res = data;
            document.getElementById("Price " + id).textContent = res+" руб.";
        });
}


function getInfo(id) {

    $.post("/Foods/GetKorzina", { id: id })
        .done(function (data) {

            result_int = data;
            $("#result_int").html(result_int);
        });
}

$('#myModal').on('shown.bs.modal', function () {
    $('#myInput').trigger('focus')
})

function plus(id) {

    $.post("/Foods/PlusKorzina", { id: id })
    .done(function (data) {
        result_int = data;
        loadData();
        document.getElementById("korzinaCounter " + id).textContent = result_int;
        updatePrices(id, result_int);
        totalPrice();
        totalAmount();
    });
}

function minus(id) {

    $.post("/Foods/MinusKorzina", { id: id })
        .done(function (data) {
            result_int = data;
            loadData();
            updatePrices(id, result_int);
            document.getElementById("korzinaCounter " + id).textContent = result_int;
            if (result_int == 0) {
                document.getElementById("CartBox " + id).remove();
            }
                totalPrice();
                totalAmount();
        });
}

function totalPrice() {
    $.post("/Orders/UpdateTotalPrice")
        .done(function (data) {
            var price = data;
            document.getElementById("totalPrice").textContent = price + " руб.";
        });
};

function totalAmount() {
    $.post("/Orders/UpdateTotalAmount")
        .done(function (data) {
            var amount = data;
            document.getElementById("totalAmount").textContent = amount + " шт.";
        });
};

function ClearOrders() {
    $.post("/Orders/ClearOrders")
        .done(function () {
            totalPrice();
            totalAmount();
            loadData();
            document.getElementById("deleteAll").remove();
        })
}

function CreateOrder() {
    $.post("/Orders/CreateOrder")
        .done(function () {
            ClearOrders();
        });
}

