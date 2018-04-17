# WinFormsChat
A simple windows forms chat written in C#

![alt text](https://raw.githubusercontent.com/JulianG97/WinFormsChat/master/Client/icon.ico)

## Features
Coming soon...

## Protocol
```
|  CHAT  |   TP   |  Content   |   DEL  |
| Header |  Type  |  Content   |   End  |
```

### Protocol Types
| Type | Full Name | Value | Description |
| ---- | --------- | ----- | ----------- |
| LI | Log In | Username | Login request sent from the client to the server |
| LO | Log Out | Username-SessionKey | Logout message sent from the client to the server |
| IA | Is Alive | - | Is alive message sent in a certain frequency from the client to the server and from the server to the client |
| AU | Add User | Username | Message sent from the server to the client to add a user to the online user box at the client |
| RU | Remove User | Username | Message sent from the server to the client to remove a user from the online user box at the client |
| SE | Session Key | SessionKey | Session key sent from the server to the client for identification purposes |
| SR | Session Key Received | Username-SessionKey | Message sent from the client to the server indicating that the client has received the session key and the server can send him further messages |
| ME | Message | Username-Message-SessionKey | Message sent from the client to the server |
| NM | New Message | Message | Message sent from the server to all clients |
