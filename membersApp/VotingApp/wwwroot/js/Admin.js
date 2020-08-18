"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/voteHub").build();
start();

connection.on("ReceiveMessage", function (message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

 function start() {
    try {
        connection.start();
    } catch (err) {
        console.log(err);
        setTimeout(() => start(), 5000);
    }
};
connection.on("CastVote", function (message, clientID) {
    var li = document.createElement("li");
    li.textContent = "Client " + clientID + " voted " + message;
    document.getElementById("messagesList").appendChild(li);
});

