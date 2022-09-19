/*==============================================================*/
// Raque Contact Form  JS
/*==============================================================*/
(function ($) {
    "use strict"; // Start of use strict
    $("#contactForm").validator().on("submit", function (event) {
        if (event.isDefaultPrevented()) {
            // handle the invalid form...
            formError();
            $("#errors").html("");
            //submitMSG(false, "Did you fill in the form properly?");
        } else {
            // everything looks good!
            event.preventDefault();
            submitForm();
        }
    });


    function submitForm() {
        // Initiate Variables With Form Content
        var name = $("#name").val();
        var email = $("#email").val();
        var phone_number = $("#phone_number").val();
        var message = $("#message").val();
        var checkBox = $("#flexCheckDefault-1")[0].checked;
        if (checkBox) {
            $.ajax({
                url: "/ContactUs",
                data: {
                    contact: {
                        UserContact: name,
                        UserPhone: phone_number,
                        Email: email,
                        Description: message
                    },
                },
                type: "POST",
                success: (data) => {
                    if (data.status == true) {
                        $("#contactForm")[0].reset();
                        $("#errors").html("<b style='color:green;'>" + data.msg + "</b>")
                        $("#termRead").html("");
                    } else {
                        $("#errors").html("<b style='color:red;'>" + data.msg + "</b>")
                    }
                }
            });
        } else {
            $("#flexCheckDefault-1").prop("checked", false);
            $("#termRead").html("<b style='color:red;'>Please accept our Term</b>");
            $("#errors").html("");
        }

    }

    function formSuccess() {
        $("#contactForm")[0].reset();
        submitMSG(true, "Message Submitted!")
    }

    function formError() {
        $("#contactForm").removeClass().addClass('shake animated').one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
            $(this).removeClass();
        });
    }

    function submitMSG(valid, msg) {
        if (valid) {
            var msgClasses = "h4 tada animated text-success";
        } else {
            var msgClasses = "h4 text-danger";
        }
        $("#msgSubmit").removeClass().addClass(msgClasses).text(msg);
    }
}(jQuery)); // End of use strict