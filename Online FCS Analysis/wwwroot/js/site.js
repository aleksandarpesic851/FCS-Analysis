$(document).ready(function () {
    if (errorMsg) {
        if (errorMsg.errorIn == "login") {
            $.magnificPopup.open({
                items: {
                    src: "#login-form"
                },
                type: 'inline'
            });
        } else if (errorMsg.errorIn == "register") {
            $.magnificPopup.open({
                items: {
                    src: "#register-form"
                },
                type: 'inline'
            });
        }
    }
});