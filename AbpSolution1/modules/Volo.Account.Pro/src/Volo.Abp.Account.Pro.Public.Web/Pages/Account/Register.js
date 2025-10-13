$(function () {

    var isRecaptchaEnabled = typeof grecaptcha !== 'undefined';
    if (isRecaptchaEnabled) {
        grecaptcha.ready(function () {
            $("#registerForm button").removeAttr("disabled");
        });
    } else {
        $("#registerForm button").removeAttr("disabled");
    }

    $("#registerForm").submit(function (e) {
        e.preventDefault();
        var form = $(this);
        if (form.valid() && isRecaptchaEnabled && abp.utils.isFunction(grecaptcha.reExecute)) {
            grecaptcha.reExecute(function (token) {
                form.find("input[type=hidden][data-captcha=true]").val(token);
                abp.ui.setBusy("#registerForm");
                form[0].submit();
            })
        } else {
            if (form.valid()){
                abp.ui.setBusy("#registerForm");
                form[0].submit();
            }
        }
    });

    $("#register").click(function (e) {
        var url = new URL(window.location.href);
        url.searchParams.delete('handler');
        $('#registerForm').attr('action', url.toString()).submit();
    });

    $("#reSendCode").click(function (e) {
        $("#Input_Code").val("");
        $("#registerForm").data("validator").settings.rules["Input.Code"].required = false;

        var url = new URL(window.location.href);
        url.searchParams.delete('handler');
        url.searchParams.set('handler', 'ResendCode');
        $('#registerForm').attr('action', url.toString()).submit();
    });

    let button = $(this);
    let passwordInput = $('#password-input');

    passwordInput.passwordComplexityIndicator({
        insideParent: true
    });

    $("#PasswordVisibilityButton").click(function (e) {
        if (!passwordInput) {
            return;
        }

        if (passwordInput.attr("type") === "password") {
            passwordInput.attr("type", "text");
        }
        else {
            passwordInput.attr("type", "password");
        }

        let icon = $("#PasswordVisibilityButton");
        if (icon) {
            icon.toggleClass("bi-eye-slash").toggleClass("bi-eye");
        }
    });

    // CAPS LOCK CONTROL
    const password = document.getElementById('password-input');
    const passwordMsg = document.getElementById('capslockicon');
    if (password && passwordMsg) {
        password.addEventListener('keyup', e => {
            passwordMsg.style = e.getModifierState('CapsLock') ? 'display: inline' : 'display: none';
        });
    }

    var email = $("#Input_EmailAddress").val();
    if (email) {
        $("#register").attr("disabled", "disabled");
        $("#reSendCode").attr("disabled", "disabled");
        volo.abp.account.account.getEmailConfirmationCodeLimit(email).done(function (data){
            if(data.nextTryTime) {
                disableButton($("#register"), data.nextTryTime);
            } else {
                $("#register").removeAttr("disabled");
            }

            if(data.nextSendTime) {
                disableButton($("#reSendCode"), data.nextSendTime);
            } else {
                $("#reSendCode").removeAttr("disabled");
            }
        })

        function disableButton(button, nexTime) {
            function updateCountdown(button, buttonText, targetTime) {
                var now = new Date();
                var timeDifference = targetTime - now;
                if (timeDifference <= 0) {
                    clearInterval(countdownInterval);
                    button.removeAttr("disabled");
                    button.text(buttonText);
                } else {
                    function pad(num) {
                        return ('00' + num).slice(-2);
                    }
                    var seconds = Math.floor(timeDifference / 1000);
                    var h = Math.floor(seconds / 3600);
                    var m = Math.floor((seconds % 3600) / 60);
                    var s = seconds % 60;
                    var countdownText = pad(h) + ':' + pad(m) + ':' + pad(s);
                    button.attr("disabled", "disabled").text(buttonText + ` (${countdownText})`);
                }
            }
            var buttonText = button.text();
            var targetTime = new Date(nexTime);
            var countdownInterval = setInterval(updateCountdown, 1000, button, buttonText, targetTime);
        }
    }
});
