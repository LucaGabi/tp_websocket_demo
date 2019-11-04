'ues strict';

const loc = window.location;
const protocal = loc.protocol;
const w = protocal === `https:` ? `wss` : `ws`;

// important!! the web socket map path (/channel) must match to the definition on Startup.cs
const uri = `${w}://${loc.host}/channel`;

const list = document.getElementById(`messages`);
const btn = document.getElementById(`sendBtn`);

(function connect() {
    socket = new WebSocket(uri);

    socket.onopen = ($event) => {
        console.log(`opened connection to ` + uri);
    };

    socket.onclose = ($event) => {
        console.log(`closed connection from ` + uri);
    };

    socket.onmessage = ($event) => {
        appendItem(list, $event.data);
        console.log($event.data);
    };

    socket.onerror = ($event) => {
        console.log(`error: ` + $event.data);
    };
})();

btn.addEventListener(`click`, () => {
    const msg = document.getElementById(`msgInput`);
    const val = msg.value;
    if (val && val.length > 0) {
        sendMessage(msg.value);
    }
    msg.value = '';
});

function sendMessage(message) {
    // in some use case, we will send data by converting to base64 string to prevent from data broken
    const b64 = b64EncodeUnicode(message);
    console.log(`Sending: ` + message);
    socket.send(b64);
}

function appendItem(list, message) {
    const li = document.createElement(`li`);
    li.innerHTML = message.replace(/\n/g, `<br/>`);
    li.className = `list-group-item`;
    list.appendChild(li);
}

function b64EncodeUnicode(str) {
    // first we use encodeURIComponent to get percent-encoded UTF-8,
    // then we convert the percent encodings into raw bytes which
    // can be fed into btoa.
    return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g,
        // function toSolidBytes(match, p1) {
        (match, p1) => {
            // console.debug('match: ' + match);
            return String.fromCharCode(('0x' + p1));
        }));
}
        
function b64DecodeUnicode(str) {
    // Going backwards: from bytestream, to percent-encoding, to original string.
    if (str && str.length > 0) {
        try {
            return decodeURIComponent(atob(str).split('').map(function (c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));
        } catch (error) {
            console.log(error);
            return undefined;
        }
    } else {
        return undefined;
    }
}
