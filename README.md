# WinFormsChat
A simple windows forms chat written in C#

![alt text](https://raw.githubusercontent.com/JulianG97/WinFormsChat/master/Client/icon.ico)

## Features
Coming soon...

## Protocol
```
|  CHAT  |   TP   |  Content   |   DEL  |
| Header |  Type  |  Content   |   End  |
| 4 byte | 2 byte | 0 - n byte | 1 byte |
```

### Protocol Types
| Type | Full Name | Value | Length | Description |
| ---- | --------- | ----- | ------ | ----------- |
| LI | Login | Username | 3 - 10 (+ 7) | Login request sent from the client to the server |
| LO | Logout | Username | 3 - 10 (+ 7) | Logout message sent from the client to the server |
| IA | IsAlive | - | 0 (+ 7) | Is alive message sent in a certain frequency from the client to the server and from the server to the client |
| ME | Message | Message | 1 - 500 (+ 7) | Message sent from the client to the server |
| NM | New Message | Message | 1 - 500 (+ 7) | Message sent from the server to all clients |
