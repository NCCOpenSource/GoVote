"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/voteHub").build();
start();

async function start() {
    try {
        await connection.start();
    } catch (err) {
        console.log(err);
        setTimeout(() => start(), 5000);
    }
};

connection.on("DisplayVote", function (seatno, currentVote) {
    arr[seatno] = currentVote;
    updateVote(seatno, currentVote);
    updateTotals(seatno, currentVote);
});

connection.on("ClearDisplay", function (seatno, currentVote) {
    window.location.reload(false);
});


connection.on("BlankScreen", function () {
    $('#resultsSummary').hide();
    $('#chamberGraphic').hide();
});