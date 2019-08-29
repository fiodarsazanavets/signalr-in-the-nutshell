const connection = new signalR.HubConnectionBuilder()
    .withUrl("/learningHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

var messageId = 1;

connection.on("ReceiveMessage", (message) => {
    $('#signalr-message-panel').prepend($('<div />').text(message));
});

connection.on("ReceiveObject", (object) => {
    $('#signalr-message-panel').prepend($('<div />').text(JSON.stringify(object)));
});

$('#btn-broadcast').click(function () {
    var message = $('#broadcast').val();
    connection.invoke("BroadcastMessage", message).catch(err => console.error(err.toString()));
});

$('#btn-broadcast-object').click(function () {
    var title = $('#broadcast-title').val();
    var message = $('#broadcast-message').val();

    var object = { "messageId": messageId, "title": title, "message": message };

    connection.invoke("BroadcastObject", object).catch(err => console.error(err.toString()));
    messageId++;
});

$('#btn-self-message').click(function () {
    var message = $('#self-message').val();
    connection.invoke("SendToCaller", message).catch(err => console.error(err.toString()));
});

$('#btn-self-object').click(function () {
    var title = $('#self-object-title').val();
    var message = $('#self-object-message').val();

    var object = { "messageId": messageId, "title": title, "message": message };

    connection.invoke("SendObjectToCaller", object).catch(err => console.error(err.toString()));
    messageId++;
});

$('#btn-others-message').click(function () {
    var message = $('#others-message').val();
    connection.invoke("SendToOthers", message).catch(err => console.error(err.toString()));
});

$('#btn-others-object').click(function () {
    var title = $('#others-object-title').val();
    var message = $('#others-object-message').val();

    var object = { "messageId": messageId, "title": title, "message": message };

    connection.invoke("SendObjectToOthers", object).catch(err => console.error(err.toString()));
    messageId++;
});

$('#btn-group-message').click(function () {
    var message = $('#group-message').val();
    var group = $('#group-for-message').val();
    connection.invoke("SendToGroup", group, message).catch(err => console.error(err.toString()));
});

$('#btn-group-object').click(function () {
    var title = $('#group-object-title').val();
    var message = $('#group-object-message').val();
    var group = $('#group-for-object').val();

    var object = { "messageId": messageId, "title": title, "message": message };

    connection.invoke("SendObjectToGroup", group, object).catch(err => console.error(err.toString()));
    messageId++;
});

$('#btn-user-message').click(function () {
    var message = $('#user-message').val();
    var user = $('#user-for-message').val();
    connection.invoke("SendToUser", user, message).catch(err => console.error(err.toString()));
});

$('#btn-user-object').click(function () {
    var title = $('#user-object-title').val();
    var message = $('#user-object-message').val();
    var group = $('#user-for-object').val();

    var object = { "messageId": messageId, "title": title, "message": message };

    connection.invoke("SendObjectToUser", group, object).catch(err => console.error(err.toString()));
    messageId++;
});

$('#btn-group-add').click(function () {
    var group = $('#group-to-add').val();
    connection.invoke("AddUserToGroup", group).catch(err => console.error(err.toString()));
});

$('#btn-group-remove').click(function () {
    var group = $('#group-to-remove').val();
    connection.invoke("RemoveUserFromGroup", group).catch(err => console.error(err.toString()));
});

async function start() {
    try {
        await connection.start();
        console.log('connected');
    } catch (err) {
        console.log(err);
        setTimeout(() => start(), 5000);
    }
};

connection.onclose(async () => {
    await start();
});

start();
