$('#messageForm').on('submit', function (e) {
    e.preventDefault();
    var text_messasge = $('#text_messasge').val();
    $.ajax({
        type: "POST",
        url: "/PostMessage",
        data: { text_messasge: text_messasge },
        success: function (respone) {
            if (respone.success) {
                $('#text_messasge').val(null);
            }
        }
    })
})

connection.on("listMess", (list, customerId) => {
    console.log(list)
    console.log(customerId)
    var box_chat = document.getElementById('box_chat');
    var span = document.createElement('span');
    var chatbox_messages = document.getElementById('chatbox_messages');
    console.log(chatbox_messages.offsetHeight)
    for (var mess of list) {
        if (mess.sender === customerId) {
            span.innerHTML = `<div class="messages__item messages__item--operator">
                            ${mess.messageChat}
                        </div>`
            box_chat.appendChild(span);
            chatbox_messages.scrollTo('0', chatbox_messages.offsetHeight);
        } else if (mess.receiver === customerId){
            span.innerHTML = `<div class="messages__item messages__item--visitor">
                            ${mess.messageChat}
                        </div>`
            box_chat.appendChild(span);
            chatbox_messages.scrollTo('0', chatbox_messages.offsetHeight);
        }
    }
});

