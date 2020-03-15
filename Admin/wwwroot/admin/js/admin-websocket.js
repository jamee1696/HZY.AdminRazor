
//
//
//
//web socket
//
//
//
var ws;
var adminWebSocket = {
    config: {
        url: (location.protocol === "https:" ? "wss:" : "ws:") + "//" + window.location.host
    },
    start: function (register) {

        var support = "MozWebSocket" in window ? 'MozWebSocket' : ("WebSocket" in window ? 'WebSocket' : null);
        if (support == null) return;

        ws = new ReconnectingWebSocket(adminWebSocket.config.url);

        ws.onmessage = function (evt) {
            var data = JSON.parse(evt.data);
            console.log('data=', JSON.stringify(data), evt);
            if (data.action == "register") { app.load(); app.showMsg(data.msg); }
            if (data.action == "init") { app.load(); app.showMsg(data.msg); }
            if (data.action == "message") { app.showMsg(data.msg); }
        }

        // when the connection is established, this method is called
        ws.onopen = function (e) {
            adminWebSocket.send(register);
        }

        ws.onerror = function (e) {
            console.log('onerror=', e);
        }

        // when the connection is closed, this method is called
        ws.onclose = function (e) {
            console.log('onclose=', "连接关闭", e);
        }

    },
    send: function (data) {
        ws.send(JSON.stringify(data));
    }

};
