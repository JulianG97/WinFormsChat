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
| LI | Log In | Username | 3 - 10 (+ 7) | Login request sent from the client to the server |
| LO | Log Out | Username-SessionKey | 36 - 43 (+ 7) | Logout message sent from the client to the server |
| IA | Is Alive | - | 0 (+ 7) | Is alive message sent in a certain frequency from the client to the server and from the server to the client |
| AU | Add User | Username | 3 - 10 (+ 7) | Message sent from the server to the client to add a user to the online user box at the client |
| RU | Remove User | Username | 3 - 10 (+ 7) | Message sent from the server to the client to remove a user from the online user box at the client |
| SE | Session Key | SessionKey | 32 (+ 7) | Session key sent from the server to the client for identification purposes |
| ME | Message | Username-Message-SessionKey | 38 - 544 (+ 7) | Message sent from the client to the server |
| NM | New Message | Message | 1 - 500 (+ 7) | Message sent from the server to all clients |
