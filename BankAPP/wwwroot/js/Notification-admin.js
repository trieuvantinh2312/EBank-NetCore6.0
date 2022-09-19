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

connection.on("sendContact", (list) => {
    console.log(list)
    var message_list = document.getElementById("message_list");
    var span = document.createElement('span');
    var countMessage = document.getElementById("countMessage");
    var count = 0;
    for (var contact of list) {
        if (contact.status == "waiting") {
            count++;
            span.innerHTML = `<a class="dropdown-item d-flex align-items-center" href="#">
                                <div class="font-weight-bold">
                                    <div class="text-truncate">
                                        ${contact.description}
                                    </div>
                                    <div class="small text-gray-500">${contact.email}</div>
                                    <div class="small text-gray-500">${contact.dateContact}</div>
                                </div>
                            </a>`
            message_list.prepend(span);
        }
    }
    console.log(count)
    if (count < 9) {
        countMessage.innerHTML = count;
    } else {
        countMessage.innerHTML = "9+";
    }
})

connection.on("listMess", (list, customerId) => {
    console.log(list)
    console.log(customerId)
    var li = document.createElement('li');
    var seenIcon = document.querySelector('.seen_' + customerId);
    var seenDiv = document.createElement('div');
    var parent = document.getElementById("contain_icon");
    var activeUser = document.getElementById("activeUser_" + customerId)
    parent.insertBefore(activeUser, parent.firstChild);
    if (activeUser.classList.contains("active")) {
    } else {
        seenDiv.setAttribute('id', 'seenAppend_' + customerId)
        seenDiv.style.width = "10px"
        seenDiv.style.height = "10px"
        seenDiv.style.borderRadius = "50%"
        seenDiv.style.backgroundColor = "red"
        seenDiv.style.marginLeft = "2%"
        seenIcon.appendChild(seenDiv)
    }
   
    li.classList.add('clearfix');
    var ul = document.getElementById(`list_chat_${customerId}`);
    for (var mess of list) {
        if (mess.sender == customerId) {
            li.innerHTML = `
                                    <div class="message-data">
                                        <span class="message-data-time">${mess.formatDate}</span>
                                       
                                    </div>
                                    <div class="message my-message"> ${mess.messageChat} </div>
                                `
           
            ul.appendChild(li);
            $('#chat_history').scrollTop($('#chat_history')[0].scrollHeight);

        } else {
            li.innerHTML = `    
                                        <div class="message-data text-right">
                                            <span class="message-data-time">${mess.formatDate}</span>
                                            <img src="https://bootdey.com/img/Content/avatar/avatar7.png" alt="avatar">
                                        </div>
                                        <div class="message other-message float-right">${mess.messageChat} </div>
                                   `
            
            ul.appendChild(li);
            $('#chat_history').scrollTop($('#chat_history')[0].scrollHeight);
        }

    }
});

//connection.on("sendMessToCus", (list, cusId) => {

//})