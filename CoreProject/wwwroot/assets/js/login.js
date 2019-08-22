function authenticate() {
    $.ajax(
        {
            type: "POST", //HTTP POST Method
            url: "Login/Login", // Controller/View
            data: { //Passing data
                username: $('#username').val(),
                password: $('#password').val()
            },
            success: function (data) {
                if (data) {
                    location.href = '/Product/Index';
                }
                else {
                    console.log(data); 
                }
            },
            error: function (error) {
                alert('Please try after sometime.');
                console.log(error);
            }
        })
}
