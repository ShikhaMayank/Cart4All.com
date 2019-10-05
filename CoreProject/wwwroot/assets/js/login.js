function authenticate() {
    $.ajax(
        {
            type: "POST", //HTTP POST Method
            url: "Login/GmailLogin", // Controller/View
            data: { //Passing data
                username: 'mayank.gpt1@gmail.com'
            },
            success: function (data) {
                console.log('login successful!');
                window.location.replace(data.newUrl);
            },
            error: function () {
                console.log('login failed!');
            }
        });
    //$.ajax(
    //    {
    //        type: "POST", //HTTP POST Method
    //        url: "Login/Login", // Controller/View
    //        data: { //Passing data
    //            username: $('#username').val(),
    //            password: $('#password').val()
    //        },
    //        success: function (data) {
    //            if (data) {
    //                location.href = '/Product/Index';
    //            }
    //            else {
    //                console.log(data); 
    //            }
    //        },
    //        error: function (error) {
    //            alert('Please try after sometime.');
    //            console.log(error);
    //        }
    //    })
}
function onSignIn(googleUser) {
    var profile = googleUser.getBasicProfile();
    console.log('ID: ' + profile.getId()); // Do not send to your backend! Use an ID token instead.
    console.log('Name: ' + profile.getName());
    console.log('Image URL: ' + profile.getImageUrl());
    console.log('Email: ' + profile.getEmail()); // This is null if the 'email' scope is not present.
    var id_token = googleUser.getAuthResponse().id_token;
    console.log(id_token);
    $.ajax(
        {
            type: "POST", //HTTP POST Method
            url: "Login/GmailLogin", // Controller/View
            // Always include an "X-Requested-With" header in every AJAX request,
            // to protect against CSRF attacks.
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            data: { //Passing data
                username: id_token
            },
            success: function (data) {
                console.log('login successful!');
                window.location.replace(data.newUrl);
            },
            error: function () {
                console.log('login failed!');
            }
        });
}
    function signOut() {
            var auth2 = gapi.auth2.getAuthInstance();
            auth2.signOut().then(function () {
        console.log('User signed out.');
    });
}
