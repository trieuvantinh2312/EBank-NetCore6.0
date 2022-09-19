$(document).ready(function () {
	// function Leading 0
	const zeroPad = (num, places) => String(num).padStart(places, '0');
	//Get Today
    const timeElapsed = Date.now();
    const today = new Date(timeElapsed);

	//Random account No
    $("#randomAccountNo").click(function(){
        var temp = Math.floor((Math.random() * 999999999));
        var accountNo = "EBANK" + zeroPad(temp, 9);
        $("#create-account-accountNo").val(accountNo);

        $.ajax({
            type: "GET",
            url: "/isExitedAccount",
            data: {
                accountNo: accountNo
            },
            success: function (respone) {
                if (respone) {
                    $("#randomAccountNo").click();
                }
            }
        });
	});


    //Update Status Customer
    $("#status").click(function () {
        var customerId = $(".customerId").html();
        var customerStatus = $(".pro").html();
        customerStatus = customerStatus.toLowerCase();
        var showLock =""
        if (customerStatus == "normal") {
            showLock = "Lock"
        } else {
            showLock = "Unlock"
        }
        Swal.fire({
            title: 'Do you want to ' + showLock +' this customer?',
            showCancelButton: true,
            confirmButtonText: showLock,
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    type: "GET",
                    url: "/updateStatus",
                    data: {
                        id: customerId,
                        status: customerStatus
                    },
                    success: function (respone) {
                        if (respone.success) {
                            if (respone.status == "lock") {
                                $("#status").html("Unlock");
                                $("span[class='pro']").css("background-color", "Red").css("color", "black").html("LOCK");
                                Swal.fire('Lock Success!', '', 'success')
                            }
                            else {
                                $("#status").html("Lock");
                                $("span[class='pro']").css("background-color", "Green").css("color", "white").html("NORMAL");
                                Swal.fire('Unclock Success!', '', 'success')
                            }
                        }
                        else {
                            Swal.fire(
                                'Error!',
                                'Update status error, try again later !',
                                'error'
                            );
                        }
                    },
                });
            }
        })
    });

    //Update Status Account
    $(".disable-account").click(function () {
        var AccountNo = $(this).data("id");
        var status = $(this).prop("checked") ? true : false;
        $.ajax({
            type: "GET",
            url: "/disableAccount",
            data: {
                accountNo: AccountNo,
                status: status
            },
            success: function (respone) {
                if (respone.success) {
                    if (respone.status == 'lock') {
                        Swal.fire(
                            'success!',
                            respone.msg,
                            'success'
                        );
                    }
                    else {
                        Swal.fire(
                            'success!',
                            respone.msg,
                            'success'
                        );
                    }
                }
                else {
                    Swal.fire(
                        'Error!',
                        respone.msg,
                        'error'
                    );
                    window.location.reload();
                }
            },
        });
    });


    // Create account Balance format currency
    $("#create-account-balance").keyup(function () {
        var Balance = $("#create-account-balance").val();
        Balance = Balance.replace(/\D/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        $(this).val(Balance);
    });

    //Create new Account
    $("#createAccount").click(function() {
        //create parameter
        var CustonerId = $("#create-account-customerId").val();
        var AccountNo = $("#create-account-accountNo").val();
        var Balance = parseFloat($("#create-account-balance").val().replaceAll(",", ""));
        if (AccountNo == "" || Balance == "" || isNaN(Balance)) {
            Swal.fire(
                'Error!',
                'All field not blank',
                'error'
            );
            return;
        }
        Swal.fire({
            title: "Please wait...",
            showConfirmButton: false,
            didOpen: () => {
                Swal.showLoading()
            },
        })
		$.ajax({
			type: "GET",
			url: "/CreateAccount",
			data: {
				CustomerId: CustonerId,
				AccountNo: AccountNo,
				Balance: Balance
			},
			success: function (respone) {
                if (respone.status) {
                    Swal.fire({
                        title: 'Success',
                        text: "Create success",
                        icon: 'success',
                        confirmButtonText: 'Ok'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.reload()
                        }
                    });
                } else {
                    Swal.fire(
                        'Error!',
                        'All field not blank',
                        'error'
                    );
                }
			}
		});

    });

    //Create new Card
    //$("#createCard").click(function () {
    //    //Random 12 number width leading 0
    //    var temp = Math.floor((Math.random() * 999999999999));
    //    temp = zeroPad(temp, 12);

        

    //    //parametter send to ajax
    //    var CardNo = "4221" + temp;
    //    var AccountNo = $(this).data("account");
    //    var month = today.getMonth();
    //    var year = today.getFullYear();
    //    var cvv = zeroPad(Math.floor((Math.random() * 999)), 3);

    //    //Ajax
    //    $.ajax({
    //        type: "POST",
    //        url: "/CreateCard",
    //        data: {
    //            CardNo: CardNo,
    //            AccountNo: AccountNo,
    //            month: month,
    //            year: year,
    //            cvv: cvv
    //        },
    //        success: function (respone) {
    //            if (respone.success) {
    //                console.log("createCard");
    //            }
    //        }
    //    });

    //});

});