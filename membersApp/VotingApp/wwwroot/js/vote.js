"use strict";

var clientID = document.getElementById("seatNo").innerHTML;
var connection = new signalR.HubConnectionBuilder().withUrl("/voteHub").build();
start();


function start() {
    try {
        connection.start();
    } catch (err) {
        console.log(err);
        setTimeout(function () {
            return start();
        }, 5000);
    }
};
connection.onclose(function () {
    start();
});

connection.on("ReceiveMessage", function (message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("OpenPoll", function (pollid) {
    $("#seatNo").text("Seat: " + clientID);
    $("#voteNumber").text(pollid);
    $("#currentVote").hide();
    $("#voteButtons").show();
});

connection.on("ClosePoll", function (pollid) {
    $("#voteNumber").text("");
    $("#voteButtons").hide();
});

document.getElementById("sendYes").addEventListener("click", function (event) {
    //sendVote("Yes", clientID);
});
document.getElementById("sendNo").addEventListener("click", function (event) {
    //sendVote("No", clientID);
});
document.getElementById("sendAbstain").addEventListener("click", function (event) {
    //sendVote("Abstain", clientID);
});

function sendVote(message, seatno) {
    $('#voteButtons').hide();
    connection.invoke("Vote", message, seatno).catch(function (err) {
        return console.error(err.toString());
    });

}

function hideButtons() {
    $("#sendYes").hide();
    $("#sendNo").hide();
    $("#sendAbstain").hide();
}

