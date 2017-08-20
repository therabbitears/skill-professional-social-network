// Declare a proxy to reference the hub.
var chatHub = $.connection.chatHub;
var lastKey = new Date();
var keyPressed = false;
var groupName = '';
var page = 0;
var pageSize = 100;
var isDefault = true;
var isLoading = false;
var isEnd = false;
var tryingToReconnect = false;
var groupID;
var memberID;
var userID;
var currentUserName;

function notifyUserOfConnectionProblem() {
    var waiting = '<div class="join-notification warning">Conncetion Slow<small class="child-headings">Either your connection is slow or server not responding in timely manner</small></div>';
    $('#user-join-container').html(waiting);
}


function notifyUserOfTryingToReconnect() {
    var waiting = '<div class="join-notification warning">Server disconnected. Trying to Reconnect...</div>';
    $('#user-join-container').html(waiting);
}

function notifyUserOfReconnect() {
    $('#user-join-container').html('');
    var waiting = '<div class="join-notification success">✓ Server reconnected successfully.</div>';
    $('#user-join-container').html(waiting);
    $('.join-notification').fadeOut(15000);
}

function notifyUserOfDisconnect() {
    var waiting = '<div class="join-notification warning">Server disconnected. Trying to Reconnect...</div>';
    $('#user-join-container').html(waiting);
}

$(function () {
    registerClientMethods(chatHub);
    // Start Hub
    $.connection.hub.start().done(function () {
        chatHub.server.joinRoom(groupName);
        registerEvents(chatHub)
    });

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
        $('#divusers').load('/ChatGroup/ChatUsers?s=all&page=0&pagesize=20');
        
    });




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

function setStartTimeOut() {
    setInterval(function () {
        if (keyPressed == true) {
            var now = new Date();
            var diffrent = Math.abs(now - lastKey);
            if (diffrent > 2000) {
                keyPressed = false;
                var userName = $('#hdUserName').val();
                //          chatHub.server.sendTypingStatus(toUser, 0);
            }
        }

    }, 2000);
}
function registerEvents(chatHub) {
    $('#btnSendMsg').click(function () {
        var msg = $("#txtMessage").val();

        if (msg.length > 0) {
            chatHub.server.sendToRoom(groupName, msg, memberID, groupID, currentUserName);
            AddMessage('You', msg);
            keyPressed = false;
            lastKey = new Date().setHours(-1);
            $("#txtMessage").val('');
        }
    });

    $("#txtMessage").keypress(function (e) {
        if (e.which == 13 && $('#chkEnterToSend').is(':checked')) {
            $('#btnSendMsg').click();
            e.preventDefault();

        } //else {
        //    keyPressed = true;
        //    var now = new Date();
        //    var diffrent = Math.abs(now - lastKey);
        //    lastKey = new Date();
        //    if (diffrent > 2000) {
        //        keyPressed = true;
        //        var userName = $('#hdUserName').val();
        //        chatHub.server.sendTypingStatus(toUser, 1);
        //    }
        //}
    });
}



function registerClientMethods(chatHub) {
    chatHub.client.addChatMessage = function (message, userName) {
        AddMessage(userName, message);
        $('#userTypingStatus').hide();
    }



    chatHub.client.addUserToGroup = function (userName) {
        //if (userName != userID && $('.loginUser[data-val-name="' + userName + '"]').length == 0) {
        //    $.get('/account/partialprofile?username='+userName+'&viewName=_GroupJoinNotificationProfile',function(res){
        //        var rs='<div class="join-notification"><table width="100%"><tr>'
        //        if (res.ProfilePic!=null&& res.ProfilePic!='') {
        //            rs=rs+'<td width="70px;"><div class="thumbnail-notification-con"><img class="thumbnail-notification" src="/UserImages/'+res.ProfilePic+'" /></div></td>';
        //        }
        //        rs +='<td><div><span class="page-title  no-margin inline-title">'+res.FirstName+'</span><small class="child-headings">Just joined the group</small> </div><div><small class="child-headings">'+res.TotalConversations +' Conversations</small><small class="child-headings">.</small><small class="child-headings">'+res.TotalAnswers+' Answers</small><small class="child-headings">.</small><small class="child-headings">' +res.TotalQuestions+' Questions Asked</small></div></td></tr></table></div>';
        //        $('#user-join-container').html(rs);
        //        $('.join-notification').fadeOut(15000);


        //        var userAdded='<div class="loginUser" data-val-name="'+userName+'"><div class="thumbnail-notification-con">';
        //        if (res.ProfilePic!=null&& res.ProfilePic!='') {
        //            userAdded+='<img src="/userimages/'+res.ProfilePic+'" alt="'+ res.FirstName + ' ' + res.LastName+' Online" title="' +res.FirstName + ' ' + res.LastName+' is Online" />';
        //        }
        //        else
        //        {
        //            userAdded+='<img src="/content/images/no-image-sm.png" alt="'+ res.FirstName + ' ' + res.LastName+' Online" title="' +res.FirstName + ' ' + res.LastName+' is Online" />';
        //        }
        //        userAdded+='<span class="online"></span></div><span class="user-name"> '+res.FirstName + ' ' + res.LastName+'</span></div>';
        //        $('#divusers').append(userAdded);
        //    });
        //}

    }



    //chatHub.client.setTypingStatus = function (isTyping) {
    //    if (isTyping == true) {
    //        $('#userTypingStatus').show();
    //        return;
    //    }
    //    $('#userTypingStatus').hide();
    //}


}

function AddMessage(userName, message, messgaeType) {
    if (userName == 'You') {
        var message = '<div style="position:relative;" class="message-current-user"><span class="userName">' + currentUserName + '</span><div><span class="mesage-content">' + message + '</span><small class="child-headings right">Just now</small></div></div>';
        $('#divChatWindow').append(message);
    } else {
        var message = '<div style="position:relative;" class="message"><span class="userName">' + userName + '</span><div><span class="mesage-content">' + message + '</span><small class="child-headings right">Just now</small></div></div>';
        $('#divChatWindow').append(message);
    }
    var height = $('#divChatWindow')[0].scrollHeight;
    $('#divChatWindow').scrollTop(height);
}

