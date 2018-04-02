# WinFormsChat
A simple windows forms chat written in C#

## Features
Coming soon...

## Protocol
```
|  CHAT  |   TP   |  Content   |
| Header |  Type  |  Content   |
| 4 byte | 2 byte | 0 - n byte |
```

### Protocol Types
| Type | Full Name | Value | Length | Description |
| ---- | --------- | ----- | ------ | ----------- |
| LI | Login | Username | 0 - 10 (+ 6) | Login request sent from the client to the server |
| LO | Logout | Username | 3 - 10 (+ 6) | Logout message sent from the client to the server |
| IA | IsAlive | - | 0 (+ 6) | Is alive message sent in a certain frequency from the client to the server and from the server to the client |
| ME | Message | Message | 1 - 500 (+ 6) | Message sent from the client to the server |
| NM | New Message | Message | 1 - 500 (+ 6) | Message sent from the server to all clients |
