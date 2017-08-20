// Declare a proxy to reference the hub.
var chatHub = $.connection.chatHub;
var lastKey = new Date();
var keyPressed = false;
var toUser = '@Model.ToUser';
var page = 0;
var pageSize = 100;
var isDefault = true;
var isLoading = false;
var isEnd = false;
var tryingToReconnect = false;
var groupID;
var thisMemberID;
var userID;
var currentUserName;
var currentUserID;
var messageStack = [];
var messageID = 1;
var receiverUserID;
$(function () {
    registerClientMethods(chatHub);

    $.connection.hub.connectionSlow(function () {
        notifyUserOfConnectionProblem(); // Your function to notify user.
    });

    $.connection.hub.reconnecting(function () {
        notifyUserOfTryingToReconnect(); // Your function to notify user.
        tryingToReconnect = true;
    });

    $.connection.hub.reconnected(function () {
        tryingToReconnect = false;
        notifyUserOfReconnect();
    });

    $.connection.hub.disconnected(function () {
        if (tryingToReconnect) {
            notifyUserOfDisconnect(); // Your function to notify user.
            setTimeout(function () {
                $.connection.hub.start().done(function () {
                    notifyUserOfReconnect();
                });
            }, 1000); // Restart connection after 5 seconds.
        }
    });

    // Start Hub
    $.connection.hub.start().done(function () {
        registerEvents(chatHub)
    });

    $(document).ready(function () { setStartTimeOut(); });

});

$(document).ready(function () {
    //    setStartTimeOut();
    if (groupID != undefined) {

        loadHistory(page, pageSize, isDefault);
        $('#divChatWindow').scroll(function () {
            var scrollTop = $(this).scrollTop();
            if (scrollTop < 100 && isLoading == false) {
                isDefault = false;
                loadHistory(page, pageSize, isDefault);
            }

        });
    }
    if (receiverUserID != undefined) {
        $('#divusers').load('/ChatGroup/ChatUsers?s=all&page=0&pagesize=20&connectionID=' + receiverUserID);
    } else {
        $('#divusers').load('/ChatGroup/ChatUsers?s=all&page=0&pagesize=20');
    }


});

function loadHistory(pageNumber, pageSize, isDefault) {
    if (!isLoading && !isEnd) {
        isLoading = true;
        $.ajax({
            type: 'GET',
            url: '/ChatGroup/ChatHistory?group=' + groupID.toString() + '&page=' + pageNumber.toString() + '&pageSize=' + pageSize.toString(),
            dataType: 'HTML',
            success: function (res) {
                if (res.trim() == '') {
                    if (isDefault) {
                        $('#divChatWindow').html(res);
                    }
                    isEnd = true;
                    return;

                }
                if (isDefault) {
                    $('#divChatWindow').html(res);
                    var height = $('#divChatWindow')[0].scrollHeight;
                    $('#divChatWindow').scrollTop(height);
                    isLoading = false;
                    page++;
                }
                else {
                    var oldTop = $('#divChatWindow')[0].scrollHeight;
                    $('#divChatWindow').prepend(res);
                    var newTop = $('#divChatWindow')[0].scrollHeight;
                    isLoading = false;
                    $('#divChatWindow').scrollTop(newTop - oldTop);
                    page++;
                }
            }
        });
    }
}

function notifyUserOfConnectionProblem() {
    var waiting = '<div class="join-notification warning">Conncetion Slow<small class="child-headings">Either your connection is slow or server not responding in timely manner</small></div>';
    $('#user-join-container').html(waiting);
}


function notifyUserOfTryingToReconnect() {
    var waiting = '<div class="join-notification warning">Server disconnected. Trying to Reconnect...</div>';
    $('#user-join-container').html(waiting);
}

function notifyUserOfReconnect() {
    ____trySendMessages();
    $('#user-join-container').html('');
    var waiting = '<div class="join-notification success">✓ Server reconnected successfully.</div>';
    $('#user-join-container').html(waiting);
    $('.join-notification').fadeOut(15000);
}

function notifyUserOfDisconnect() {
    var waiting = '<div class="join-notification warning">Server disconnected. Trying to Reconnect...</div>';
    $('#user-join-container').html(waiting);
}

function setStartTimeOut() {
    setInterval(function () {
        if (keyPressed == true) {
            var now = new Date();
            var diffrent = Math.abs(now - lastKey);
            if (diffrent > 2000) {
                keyPressed = false;
                var userName = $('#hdUserName').val();
                chatHub.server.sendTypingStatus(toUser, 0);
            }
        }

    }, 2000);
}

function registerEvents(chatHub) {

    //  chatHub.server.connect('@Model.CurrentUser');

    $('#btnSendMsg').click(function () {
        var msg = $("#txtMessage").val();
        msg = msg.replace(/</g, "&lt;").replace(/>/g, "&gt;");
        
        if (msg.length > 0) {
            var error = false;
            try {
                chatHub.server.sendToUser(toUser, $("#txtMessage").val(), 1, currentUserID, thisMemberID, currentUserName, groupID, messageID);
                var messageObject = { toUser: toUser, msg: msg, type: 1, currentUserID: currentUserID, thisMemberID: thisMemberID, currentUserName: currentUserName, groupID: groupID, messageID: messageID };
                messageStack.push(messageObject);
            } catch (e) {
                error = true;
            }
            AddMessage(currentUserName, msg, 1, messageID);
            $('.feed-comment[data-val-name="chat-face-' + receiverUserID + '"]').find('.message').html(msg)
            var tempHtml = $('.feed-comment[data-val-name="chat-face-' + receiverUserID + '"]').html();
            $('.feed-comment[data-val-name="chat-face-' + receiverUserID + '"]').remove();
            tempHtml = '<div class="feed-comment active" data-val-name="chat-face-' + receiverUserID + '">' + tempHtml + '<\div>';
            $('#divusers').prepend(tempHtml);
            $("#txtMessage").val('');
            keyPressed = false;
            lastKey = new Date().setHours(-1);
            messageID++;
        }
    });

    $("#txtMessage").keypress(function (e) {
        if (e.which == 13 && $('#chkEnterToSend').is(':checked')) {
            $('#btnSendMsg').click();
            return false;
        } else {
            keyPressed = true;
            var now = new Date();
            var diffrent = Math.abs(now - lastKey);
            lastKey = new Date();
            if (diffrent > 2000) {
                keyPressed = true;
                var userName = $('#hdUserName').val();
                chatHub.server.sendTypingStatus(toUser, 1);
            }
        }
    });
}


function ____trySendMessages() {
    if (messageStack.length > 0) {
        for (var i = 0; i < messageStack.length; i++) {
            chatHub.server.sendToUser(messageStack[i].toUser, messageStack[i].msg, 1, messageStack[i].currentUserID, messageStack[i].thisMemberID, messageStack[i].currentUserName, messageStack[i].groupID, messageStack[i].messageID);
        }
    }
}

function registerClientMethods(chatHub) {
    chatHub.client.____callack = function (type, messageID) {
        $('#msg-' + messageID.toString()).find('.time-tick').html('Just Now'); $('#msg-' + messageID.toString()).removeClass('pending');
        for (var i = 0; i < messageStack.length; i++) {
            if (messageStack[i].messageID == messageID) {
                messageStack.pop(i);
            }
        }
    }

    chatHub.client.messageReceived = function (userName, message, messageType, senderID, groupID) {
        var isCurrentWindow = false;
        message = message.replace(/</g, "&lt;").replace(/>/g, "&gt;");

        if ($('.chat-content').is(':visible')) {
            if ($('.chat-content').attr("data-hash-activated-for") == senderID) {
                isCurrentWindow = true;
                AddMessage(userName, message);
                chatHub.server.sendReadReceipt(groupID);
                $('#userTypingStatus').hide();
            }
        }
        var faceName = '.feed-comment[data-val-name="chat-face-' + senderID + '"]';
        if ($(faceName).html() != undefined) {
            $('.feed-comment[data-val-name="chat-face-' + senderID + '"]').find('.message').html(message)
            var tempHtml = $('.feed-comment[data-val-name="chat-face-' + senderID + '"]').html();
            $('.feed-comment[data-val-name="chat-face-' + senderID + '"]').remove();
            if (isCurrentWindow)
                tempHtml = '<div class="feed-comment" data-val-name="chat-face-' + senderID + '">' + tempHtml + '<\div>';
            else
                tempHtml = '<div class="feed-comment pending face" data-val-name="chat-face-' + senderID + '">' + tempHtml + '<\div>';
            $('#divusers').prepend(tempHtml);
        } else {
            $.ajax({
                type: 'GET',
                url: '/chatgroup/chatusers?gid=' + groupID.toString(),
                dataType: 'HTML',
                success: function (tempHtml) { $('#divusers').prepend(tempHtml); }
            });
        }
    }

    chatHub.client.setTypingStatus = function (isTyping) {
        if (isTyping == true) {
            $('#userTypingStatus').show();
            return;
        }
        $('#userTypingStatus').hide();
    }
}

function AddMessage(userName, message, messgaeType, messageID) {
    if (userName == currentUserName) {
        var message = '<div id="msg-' + messageID.toString() + '" style="position:relative;" class="message-current-user pending"><div class="message-header"><h5 class="userName">' + userName + '</h5><small class="child-headings right time-tick">...</small></div><div><span class="mesage-content">' + message + '</span></div></div>';
        $('#divChatWindow').append(message);
    } else {
        var message = '<div style="position:relative;" class="message"><div class="message-header"><h5 class="userName">' + userName + '</h5><small class="child-headings right">Just now</small></div><div><span class="mesage-content">' + message + '</span></div></div>';
        $('#divChatWindow').append(message);
    }
    var height = $('#divChatWindow')[0].scrollHeight;
    $('#divChatWindow').scrollTop(height);
}


function ____showNewConversationInput() {
    if ($('.users .conversation-new').length>0) {
        $('.conversation-new').show();
    }
    else {
        $(this).attr('disabled', 'disabled');
        $.get('/chatgroup/startnew', function (res) {
            $('.users').prepend(res);
        });
    }
}