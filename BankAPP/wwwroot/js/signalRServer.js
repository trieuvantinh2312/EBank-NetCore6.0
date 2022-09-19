
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationServer")
    .withAutomaticReconnect()
    .build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
    }
};
connection.onclose(async () => {
    await start();
});

// Start the connection.

start();

connection.on("sendNotification", (list) => {
    var content = document.getElementById("cont");
    var customerId = document.getElementById("customerId").value;
    var parseCustomerId = parseInt(customerId);
    var notification = document.createElement('span')
    var countNoti = document.getElementById('number_noti')
    var count = 0;
    for (var noti of list) {
        if (noti.fromCustomerId === parseCustomerId) {
            count++;
            notification.innerHTML = `<div class="sec new">
                 <a href="/DetailTransaction?id=${noti.transactionId}">
                   <div class="txt">You have transfered from ${noti.fromAccountId} to ${noti.toAccountId} - ${noti.formatAmount}</div>
                   <div class="txt sub">${noti.formatDate}</div>
                 </a>
            </div>`
            content.prepend(notification)
        } else if (noti.toCustomerId === parseCustomerId) {
            count++;
            notification.innerHTML = `<div class="sec new">
                 <a href="/DetailTransaction?id=${noti.transactionId}">
                   <div class="txt">You have received from ${noti.fromAccountId} to ${noti.toAccountId} with description ${noti.transactions.description} + ${noti.formatAmount}</div>
                   <div class="txt sub">${noti.formatDate}</div>
                 </a>
            </div>`
            content.prepend(notification)
        } else {

        }
    }
    countNoti.innerHTML = count;
});

connection.on("reloadNews", (list,pager) => {
    if (list.length > 0) {
        let variable = '<div class="row">';
        for (var i of list) {
            variable += '<div class="col-lg-4 col-md-6">' +
                '           <div class="single-blog-item wow fadeInUp delay-0-2s">' +
                '               <a href="blog-details.html">' +
                '                   <img src="' + i.imageMain + '" alt="Image">' +
                '               </a>' +
                '' +
                '               <div class="blog-content">' +
                '                   <ul>' +
                '                       <li>' +
                '                           <i class="flaticon-calendar"></i>' +
                '                               ' + i.formatDate + '' +
                '                       </li>' +
                '                       <li>' +
                '                           <i class="flaticon-user"></i>' +
                '                               <a href="blog-details.html">By Admin</a>' +
                '                       </li>' +
                '                   </ul>' +
                '                   <h3>' +
                '                       <a href="NewsDetail?id=' + i.newId + '">' +
                '                           ' + i.title + '' +
                '                       </a>' +
                '                   </h3>' +
                '               </div>' +
                '           </div>' +
                '       </div>'
        }
        variable += '</div>'
        if (pager.totalPages > 0) {
            var variable2 = '<ul class="pagination justify-content-center">'
            if (pager.currentPage > 1) {
                variable2 += '<li class="pagination-area">' +
                    '<button class="page-numbers" onclick="getPage(1)"> <i class="bi bi-arrow-left-short"></i> </button>' +
                    '</li>'
            }
            for (var pge = pager.startPage; pge <= pager.endPage; pge++) {
                variable2 += '<li class="pagination-area">' +
                    '<button class="page-numbers  ' + (pge == pager.currentPage ? "bg-info" : "") + '" onclick="getPage(' + pge + ')">' + pge + '</button>' +
                    '</li>'
            }
            if (pager.currentPage < pager.totalPages) {
                variable2 += '<li class="pagination-area">' +
                    '<li class="pagination-area">' +
                    '<button class="page-numbers" onclick="getPage(' + pager.totalPages + ')"> <i class="bi bi-arrow-right-short"></i> </button>' +
                    '</li>'
            }
            variable2 += '</ul>'
        }
        $("#renderNews").html(variable + variable2);
    } else {
        $("#renderNews").html("");
    }
});

connection.on("sendAccountBalance", (account) => {
    $("#balanceAccount").text(account.formatAmount)
})


