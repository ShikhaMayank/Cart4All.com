var isMobile = false; //initiate as false
$(function () {
    // device detection
    if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|ipad|iris|kindle|Android|Silk|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(navigator.userAgent)
        || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(navigator.userAgent.substr(0, 4))) {
        isMobile = true;
    }

    if (isMobile == true) {
        $('.productList').hide();
        $('.mobileMenu').show().on('click', function () {
            $('.accordion').removeClass('text-right');
            $('.productList').toggleClass('productListMobile');
            $('ul.accordion').show();
        });
        $('.cartList').parent().addClass('mobileCartList');
    }
    var productTemplate = $('#product-template').html();
    var productDiv = $('.showProduct');
    function loadProducts(product) {
        productDiv.append(Mustache.render(productTemplate, product));
    }
    var subDomainName = getSubDomain();
    //$.ajax({
    //    url: "/assets/json/fooditems/FoodItem_" + subDomainName + ".json",
    //    success: function (data) {
    //        //var products = JSON.parse(data);
    //        var products = data;
    //        sessionStorage.setItem("FoodItems", JSON.stringify(data));
    //        $.each(products, function (i, product) {
    //            loadProducts(product);
    //            /*var productImage = $('#product'+ product.id +' .productImage');
    //            if(product.imageUrl != ""){
    //                productImage.show();
    //            }*/
    //        });
    //    },
    //    error: function (e) {
    //        alert('some error in site, please contact Admin.');
    //        console.log(e);
    //    }
    //});
        $.ajax(
            {
                type: "POST", //HTTP POST Method
                url: "Home/GetFoodItemList", // Controller/View
                data: { //Passing data
                    MenuId: 1, //Reading text box values using Jquery
                },
                success: function (data) {
                    var products = JSON.parse(data);
                    if (products.length == 0) {
                        $('#resOFF').show();
                    }
                    else {
                        $('#resOFF').hide();
                        sessionStorage.setItem("FoodItems", data);
                        $.each(products, function (i, product) {
                            loadProducts(product);
                            /*var productImage = $('#product'+ product.id +' .productImage');
                            if(product.imageUrl != ""){
                                productImage.show();
                            }*/
                        });
                    }
                    
                },
                error: function () {
                    alert("No Data");
                }
            });
    $(document).ready(function () {
        AjaxDisplayString();
        //Add to cart button click

        var $showProduct = $('.showProduct');
        var $cartListItems = $('.cartListItems');
        var $accordion = $('.accordion');

        var cartItemTemplate = $('#cartListTemplate').html();
        var cartListDiv = $('.cartListItems');

        function addProducts(product) {
            cartListDiv.append(Mustache.render(cartItemTemplate, product));
        }

        $showProduct.delegate('.addToCart', 'click', function () {
            var subTotal = $('.subTotalPrice span').text();
            var $this = $(this);
            var productId = $this.parent().parent().parent().parent().attr('id');
            $this.hide();
            $('#' + productId + ' .manageCart').show();
            $('#' + productId + ' .itemCount').text(1);
            $('.emptyCart').hide();

            var product = {
                name: $("#" + productId + " .productName h4").text(),
                type: $("#" + productId + " .productName h4 i").attr('class'),
                price: $("#" + productId + " .productPrice span").text(),
                id: productId,
                totalItem: $("#" + productId + " .itemCount").text()
            };
            addProducts(product);
            $('.cartTotal').show();
            if (subTotal == "") {
                subTotal = 0;
            }
            subTotal = parseInt(subTotal) + parseInt(product.price);
            $('.subTotalPrice span').text(subTotal);

            if (subTotal > 200) {
                $('.deliveryCharges').text(00);
            }

            $('.mobileCartList').show();
            var addMarginBody = $('.mobileCartList').height();
            $('html').css({ marginBottom: addMarginBody })

        });
        $showProduct.delegate('.plusItem', 'click', function () {
            var $this = $(this);
            var productId = $this.parent().parent().parent().parent().parent().attr('id');
            var itemCount = $('#' + productId + ' .itemCount');
            var cartItemCount = $('#cart' + productId + ' .itemCountCart');
            var itemPrice = parseInt($('#' + productId + ' .productPrice span').text());
            var carSubTotal = parseInt($('.subTotalPrice span').text());
            var currentItem = parseInt(itemCount.text());
            itemCount.text(currentItem + 1);
            cartItemCount.text(currentItem + 1);
            $('.subTotalPrice span').text(itemPrice + carSubTotal);
            if (carSubTotal > 200) {
                $('.deliveryCharges').text(00);
            }
            else {
                $('.deliveryCharges').text();
            }
        });
        $showProduct.delegate('.minusItem', 'click', function () {
            var $this = $(this);
            var productId = $this.parent().parent().parent().parent().parent().attr('id');
            var itemCount = $('#' + productId + ' .itemCount');
            var cartItemCount = $('#cart' + productId + ' .itemCountCart');

            var itemPrice = parseInt($('#' + productId + ' .productPrice span').text());
            var carSubTotal = parseInt($('.subTotalPrice span').text());

            var currentItem = parseInt(itemCount.text());
            itemCount.text(currentItem - 1);
            cartItemCount.text(currentItem - 1);
            $('.subTotalPrice span').text(carSubTotal - itemPrice);
            if (itemCount.text() <= 0) {
                var cartItem = $('#cart' + productId);
                $(cartItem).remove();
                $this.parent().hide();
                $this.parent().siblings('.addToCart').show();
                if ($('.cartListItems').length <= 0) {
                    $('.subTotalPrice span').text(00);
                    $('.emptyCart').show();
                    $('.cartTotal').hide();
                }

            }
        });

        $('.onlyVegBtn').on('click', function () {
            var $this = this;
            $($this).toggleClass('active');
            $($this).children('i').toggleClass('icon-squerCheck');
            // check if other filters are checked or not
            $('a.btn.NonVegBtn').removeClass('active');
            $('.NonVegBtn i.icon-squer').removeClass('icon-squerCheck');
            $('a.btn.EggBtn').removeClass('active');
            $('.EggBtn .icon-squer').removeClass('icon-squerCheck');
            if ($('.onlyVegBtn')[0].classList.contains('active')) {
                for (var i = 0; i < $('.product').length; i++) {
                    if ($('.product')[i].querySelector('div.productNameArea').querySelector('div.productName').querySelector('h4').querySelector('i').classList.contains('Veg')) {
                        $('.product')[i].style.display = 'block';
                    }
                    else {
                        $('.product')[i].style.display = 'none';
                    }
                }
            }
            else {
                for (var i = 0; i < $('.product').length; i++) {
                    $('.product')[i].style.display = 'block';
                }
            }
        });
        $('.NonVegBtn').on('click', function () {
            var $this = this;
            $($this).toggleClass('active');
            $($this).children('i').toggleClass('icon-squerCheck');
            $('a.btn.onlyVegBtn').removeClass('active');
            $('.onlyVeg i.icon-squer').removeClass('icon-squerCheck');
            $('a.btn.EggBtn').removeClass('active');
            $('.EggBtn .icon-squer').removeClass('icon-squerCheck');
            if ($('.NonVegBtn')[0].classList.contains('active')) {
                for (var i = 0; i < $('.product').length; i++) {
                    if ($('.product')[i].querySelector('div.productNameArea').querySelector('div.productName').querySelector('h4').querySelector('i').classList.contains('NonVeg')) {
                        $('.product')[i].style.display = 'block';
                    }
                    else {
                        $('.product')[i].style.display = 'none';
                    }
                }
            }
            else {
                for (var i = 0; i < $('.product').length; i++) {
                    $('.product')[i].style.display = 'block';
                }
            }
        });
        $('.EggBtn').on('click', function () {
            var $this = this;
            $($this).toggleClass('active');
            $($this).children('i').toggleClass('icon-squerCheck');

            $('a.btn.onlyVegBtn').removeClass('active');
            $('.onlyVeg i.icon-squer').removeClass('icon-squerCheck');
            $('a.btn.NonVegBtn').removeClass('active');
            $('.NonVeg .icon-squer').removeClass('icon-squerCheck');

            if ($('.EggBtn')[0].classList.contains('active')) {
                for (var i = 0; i < $('.product').length; i++) {
                    if ($('.product')[i].querySelector('div.productNameArea').querySelector('div.productName').querySelector('h4').querySelector('i').classList.contains('Eggetarian')) {
                        $('.product')[i].style.display = 'block';
                    }
                    else {
                        $('.product')[i].style.display = 'none';
                    }
                }
            }
            else {
                for (var i = 0; i < $('.product').length; i++) {
                    $('.product')[i].style.display = 'block';
                }
            }
        });

        $('.paymentOptions div').on('click', function () {
            var $this = this;
            $($this).addClass('active');
            $($this).children('i').addClass("icon-checked");
            $($this).siblings('div').removeClass('active');
            $($this).siblings('div').find('i').removeClass("icon-checked");

        });

        $accordion.delegate('.toggleList', 'click', function (e) {
            e.preventDefault();

            var $this = $(this);

            if ($this.next().hasClass('show')) {
                $this.next().removeClass('show');
                $this.next().slideUp(350);
            } else {
                $this.parent().parent().find('li .innerList').removeClass('show');
                $this.parent().parent().find('li .toggleList').removeClass('active');
                $this.parent().parent().find('li .innerList').slideUp(350);
                $this.next().slideToggle(350);
                $this.next().addClass('show');
                $this.addClass('active');
            }
            return false;
        });
        $accordion.delegate('.innerItem', 'click', function (e) {
            var $this = $(this);
            $this.parent().siblings('li').find('a').removeClass('active');
            $this.parent().parent().parent().siblings('li').find('a').removeClass('active');
            $this.addClass('active');
        });

        $cartListItems.delegate('.plusItemCart', 'click', function () {


            var $this = $(this);
            var productId = $this.parent().parent().parent().attr('id');
            var itemCount = $('#' + productId.substring(4, productId.length) + ' .itemCount');
            var cartItemCount = $('#' + productId + ' .itemCountCart');
            var itemPrice = parseInt($('#' + productId + ' .productPrice span').text());
            var carSubTotal = parseInt($('.subTotalPrice span').text());
            var currentItem = parseInt(cartItemCount.text());
            itemCount.text(currentItem + 1);
            cartItemCount.text(currentItem + 1);
            $('.subTotalPrice span').text(itemPrice + carSubTotal);
            if (carSubTotal > 200) {
                $('.deliveryCharges').text(00);
            }
            else {
                $('.deliveryCharges').text();
            }

        });
        $cartListItems.delegate('.minusItemCart', 'click', function () {


            var $this = $(this);
            var productId = $this.parent().parent().parent().attr('id');
            var mainItemId = productId.substring(4, productId.length);
            var itemCount = $('#' + mainItemId + ' .itemCount');
            var cartItemCount = $('#' + productId + ' .itemCountCart');

            var itemPrice = parseInt($('#' + productId + ' .productPrice span').text());
            var carSubTotal = parseInt($('.subTotalPrice span').text());

            var currentItem = parseInt(cartItemCount.text());
            itemCount.text(currentItem - 1);
            cartItemCount.text(currentItem - 1);
            $('.subTotalPrice span').text(carSubTotal - itemPrice);
            if (cartItemCount.text() <= 0) {
                var cartItem = $('#' + productId);
                $(cartItem).remove();
                $this.parent().hide();
                $('#' + mainItemId + ' .manageCart').hide();
                $('#' + mainItemId + ' .addToCart').show();
                if ($('.cartListItems').children().length <= 0) {
                    $('.subTotalPrice span').text(00);
                    $('.emptyCart').show();
                    $('.cartTotal').hide();

                }

            }

        });
        /*$("body").mouseup(function (e) {
            var subject = $('.productListMobile');
            if (e.target.id != subject.attr('id') && !subject.has(e.target).length) {
                subject.fadeOut();
            }
        });*/
    });

    $(document).ready(function () {
        var _subdomain = getSubDomain();
        $("#aVerifyOTP").click(function () {
            sessionStorage.setItem('price', $('.subTotalPrice span').html());
            var _address = $('#cAddress').val()
            var userlocation = ', ' + $('#cArea').children("option:selected").val();
            sessionStorage.setItem("FullAddress", (_address + userlocation));
            sessionStorage.setItem('userPhone', $('#cPhoneNumber').val());
            sessionStorage.setItem('userName', $('#cName').val());
            var userInput = $('#1st').val() + $('#a').val() + $('#b').val() + $('#c').val() + $('#d').val() + $('#e').val();
            if (userInput.length < 1) {
                $('#codeError').val('Value cannot be blank and shud be equal to 6 digits.');
                return false;
            }
            else if (userInput.length != 6) {
                //alert('Value shud be equal to 6 digits.')
                $('#codeError').val('Value shud be equal to 6 digits.!');
                return false;
            }
            else {
                $.ajax(
                    {
                        type: "POST", //HTTP POST Method
                        url: "Home/VerifyOTP", // Controller/View
                        data: { //Passing data
                            OTP: userInput, //Reading text box values using Jquery
                            HashCode: $('#EncryptedOTP').val(),
                            OrderedItems: $('.cartListItems')[0].innerText,
                            username: $('#cName').val(),
                            userPhone: $('#cPhoneNumber').val(),
                            userEmail: $('#cEmail').val(),
                            address: $('#cAddress').val() + ' - ' + $('#cArea').val(),
                            restaurant: _subdomain
                        },
                        success: function (data) {
                            if (data == "Error") {
                                alert('Wrong OTP Entered!');
                                $('.pincode-input-text').val('');
                                $('#1st').focus();
                                return false;
                            }
                            else if (data == '00000000-0000-0000-0000-000000000000') {
                                alert('Some problem occured while creating your order. Please try after some time.');
                                return false;
                            }
                            else {
                                sessionStorage.setItem('myOrderId', data);
                                sessionStorage.setItem('COD_Items', $('.cartListItems')[0].innerText);
                                location.href = '/home/thanks';
                            }
                        },
                        error: function (error) {
                            alert('Some issue occured while verifying OTP. Please try after sometime.');
                            console.log(error);
                        }
                    });
            }
        });
    });

});
function getSubDomain() {
    var getHostname = window.location.hostname;
    var domainNameList = getHostname.split('.');
    var subDomainName = domainNameList[0];
    if (subDomainName == 'localhost') {
        subDomainName = 'cart4all';
    }
    return subDomainName;
}
function AjaxDisplayString() {
    var subDomainName = getSubDomain();
    $.ajax({
        url: "/assets/json/Menu/" + subDomainName + ".json",
        method: 'GET',
        success: function (itemList) {
            localStorage.setItem('me', JSON.stringify(itemList));
            var storedData = JSON.parse(localStorage.getItem("me"));
            var listDiv = $('.productList ul');
            for (var i = 0; i < itemList.length; i++) {
                listDiv.append('<li id="' + storedData[i].Id + '"><a class="toggleList" onClick="getFoodItem(' + storedData[i].Id + ')">' + storedData[i].Name + '</a></li>');
            }
        }
    });
    //$.ajax({
    //    url: "/Home/GetMenu",
    //    method: 'GET',
    //    success: function (itemList) {
    //        localStorage.setItem('me', JSON.stringify(itemList));
    //        var storedData = JSON.parse(localStorage.getItem("me"));
    //        var listDiv = $('.productList ul');
    //        for (var i = 0; i < itemList.length; i++) {
    //            listDiv.append('<li id="' + itemList[i].id + '"><a class="toggleList" onClick="getFoodItem(' + itemList[i].id + ')">' + itemList[i].name + '</a></li>');
    //        }
    //    }
    //});
    //$.ajax({
    //    url: "/Home/LoadJson",
    //    method: 'GET',
    //    success: function (data) {
    //        $('.addressArea span').html(data[0]);//address
    //        $('.offers span').html(data[1]);//offers
    //        $('.phoneNumber span').html(data[2]);//mobile
    //    }
    //});
    var getHostname = window.location.hostname;
    var domainNameList = getHostname.split('.');
    var subDomainName = domainNameList[0];
    if (subDomainName == 'localhost') {
        subDomainName = 'cart4all';
    }
    $.ajax(
        {
            type: "POST", //HTTP POST Method
            url: "Home/LoadJson", // Controller/View
            data: { //Passing data
                domain: subDomainName
            },
            success: function (data) {
                if (data == null) {
                    return false;
                }
                else {
                    var json = JSON.parse(data);
                    if (json.length > 0) {
                        $('.addressArea span').html(json[0].Address);//address
                        $('.phoneNumber span').html(json[0].Landline1);//mobile
                        $('.rLicNo').html(json[0].fssai);
                        $('.email a').attr("href", "mailto: " + json[0].email);
                        $('.email a').text(json[0].email);
                        sessionStorage.setItem('OwnerEmail', json[0].email);
                        sessionStorage.setItem('RestaurantDiscount', json[0].discount);
                    }
                    
                }
            }
        });
}
function closeOTPBox() {
    $('#OTP').hide();
}
function validateDetails() {
    var name = $('#cName').val();
    var email = $('#cEmail').val();
    var mobile = $('#cPhoneNumber').val();

    if (name.length < 1) {
        alert('Name cannot be empty');
        $('#cName').focus();
        return false;
    }
    if (email.length < 1 && mobile.length != 10) {
        alert('Fill atleast one between email and mobile.');
        $('#cEmail').focus();
        return false;
    }
    if (!IsEmail(email) && email.length > 0) {
        alert('Please enter valid email Id');
        $('#cEmail').focus();
        return false;
    }
    if (mobile.length != 10) {
        alert('Please enter valid mobile number.');
        $('#cPhoneNumber').focus();
        return false;
    }
    if (mobile.length != 10) {
        alert('Mobile Number should be of 10 digits');
        $('#cPhoneNumber').focus();
        return false;
    }
    if ($('#cAddress').val().length < 1) {
        alert('Address cannot be empty');
        $('#cAddress').focus();
        return false;
    }
    //if ($('.cod.active').length < 1 && $('.paytm.active').length < 1) {
    //    alert('please select atleast one payment option!');
    //    return false;
    //}
    else {
        return true;
    }
}
function IsEmail(email) {
    var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (!regex.test(email)) {
        return false;
    } else {
        return true;
    }
}
function Payment() {
    var _address = $('#cAddress').val()
    var userlocation = ', '+$('#cArea').children("option:selected").val();
    sessionStorage.setItem("FullAddress", (_address + userlocation));
    sessionStorage.setItem('email', $('#cEmail').val());
    sessionStorage.setItem('name', $('#cName').val());
    sessionStorage.setItem('mobile', $('#cPhoneNumber').val());
    var i = validateDetails();
    if (i == false) { }
    else {
        $.ajax({
            url: "/Home/GetOTP",
            method: 'POST',
            data: { //Passing data
                number: $('#cPhoneNumber').val()
            },
            success: function (data) {
                var xhttp = new XMLHttpRequest();
                xhttp.onreadystatechange = function () {
                    if (this.readyState == 4 && this.status == 200) {
                        alert(this.responseText);
                    }
                };
                var encrypted = data[0];
                $('#EncryptedOTP').val(encrypted);
                $('#OTP').show();
            }
        });
    }
}
// Code For 6 digit OTP
moveOnMax = function (field, nextFieldID) {
    if (field.value.length == 1) {
        document.getElementById(nextFieldID).focus();
    }
}
function RazorPay() {
    var name = $('#cName').val();
    var email = $('#cEmail').val();
    var mobile = $('#cPhoneNumber').val();
    var bln = validateRazorDetails();
    if (bln == true) {
        var Descr = [];
        for (var j = 0; j < $('.productInCart h5 span').length; j++) {
            Descr.push({
                Item: $('.productInCart h5 span')[j].innerHTML,
                Price: $('.productInCart .productPrice span')[j].innerHTML,
                Quantity: $('.productAddCart .manageCart .itemCountCart')[j].innerHTML
            });
        }
        sessionStorage.setItem('price', $('.subTotalPrice span').html());
        var _address = $('#cAddress').val()
        var userlocation = ', ' + $('#cArea').children("option:selected").val();
        sessionStorage.setItem("FullAddress", (_address + userlocation));
        sessionStorage.setItem('Descr', JSON.stringify(Descr));
        sessionStorage.setItem('email', $('#cEmail').val());
        sessionStorage.setItem('name', $('#cName').val());
        sessionStorage.setItem('mobile', $('#cPhoneNumber').val());
        location.href = '/payment/index';
    }
}

function validateRazorDetails() {
    var name = $('#cName').val();
    var email = $('#cEmail').val();
    var mobile = $('#cPhoneNumber').val();

    if (name.length < 1) {
        alert('Name cannot be empty');
        $('#cName').focus();
        return false;
    }
    if (email.length < 1 && mobile.length != 10) {
        alert('Fill atleast one between email and mobile.');
        $('#cEmail').focus();
        return false;
    }
    if (mobile.length != 10) {
        alert('Mobile Number should be of 10 digits');
        $('#cPhoneNumber').focus();
        return false;
    }
    if ($('#cAddress').val().length < 1) {
        alert('Address cannot be empty');
        $('#cAddress').focus();
        return false;
    }
    else {
        return true;
    }
}
function getFoodItem(catID) {
    //We do that to ensure to get a correct JSON
    var FoodItemList = sessionStorage.getItem("FoodItems");
    var filtered_json = find_in_object(JSON.parse(FoodItemList), { CatId: parseInt(catID) - 1 });
    var productTemplate = $('#product-template').html();
    var productDiv = $('.showProduct').html('<div class="showProduct"></div>');
    var products = []; products = filtered_json;
    $.each(products, function (i, product) {
        productDiv.append(Mustache.render(productTemplate, product));
        /*var productImage = $('#product'+ product.id +' .productImage');
        if(product.imageUrl != ""){
            productImage.show();
        }*/
    })
    if (isMobile == true) {
        $('ul.accordion').hide();
    }
}
function find_in_object(my_object, my_criteria) {

    return my_object.filter(function (obj) {
        return Object.keys(my_criteria).every(function (c) {
            return obj[c] == my_criteria[c];
        });
    });
}