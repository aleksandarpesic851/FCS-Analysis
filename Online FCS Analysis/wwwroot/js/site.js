$(document).ready(function () {
    if (errorMsg) {
        if (errorMsg.showDlg == "login") {
            $.magnificPopup.open({
                items: {
                    src: "#login-form"
                },
                type: 'inline'
            });
        } else if (errorMsg.showDlg == "register") {
            $.magnificPopup.open({
                items: {
                    src: "#register-form"
                },
                type: 'inline'
            });
        }
    }
});