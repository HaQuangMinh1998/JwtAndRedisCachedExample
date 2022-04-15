
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
disable2();

var fnStart = () => {
  connection.start().then(enable2()).catch(function (err) {
    return console.error(err.toString());
  });
}
//open connect
document.getElementById("open2").addEventListener("click", function (event) {
  fnStart();
});
//close connect
document.getElementById("close2").addEventListener("click", function (event) {
  connection.stop().then(function () {
    disable2();
  }).catch(function (err) {
    return console.error(err.toString());
  });
});
//send message
document.getElementById("sendButton2").addEventListener("click", function (event) {
  var user = document.getElementById("userInput").value;
  var message = document.getElementById("messageInput").value;
    connection.invoke("SendToAll", user, message).catch(function (err) {
    return console.error(err.toString());
  });
  event.preventDefault();
});
//receive message
connection.on("ReceiveMessage2", function (user, message) {
  var li = document.createElement("li");
  document.getElementById("messagesList").appendChild(li);
  li.textContent = `${user} says ${message}`;
});




function disable2() {
  document.getElementById("sendButton2").disabled = true;
  document.getElementById("close2").disabled = true;
  document.getElementById("open2").disabled = false;
}
function enable2() {
  document.getElementById("sendButton2").disabled = false;
  document.getElementById("close2").disabled = false;
  document.getElementById("open2").disabled = true;
}
