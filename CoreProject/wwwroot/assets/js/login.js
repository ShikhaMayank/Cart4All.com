function authenticate() {
    $.ajax(
        {
            type: "POST", //HTTP POST Method
            url: "Login/login", // Controller/View
            data: { //Passing data
                username: $('#username').val(),
                password: $('#password').val()
            },
            success: function (data) {
                var isValid = []; isValid = JSON.parse(data);
                console.log(isValid[0].UserExists);
                if (isValid[0].UserExists) {
                    localStorage.setItem('userSession', $('#username').val());
                    location.href = '/Product/Index';
                }
            },
            error: function (error) {
                alert('Please try after sometime.');
                console.log(error);
            }
        })
}