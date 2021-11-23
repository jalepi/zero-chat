# zero-chat
Server-Client chat application implemented in .Net and ZeroMQ

## Running the Server Application
Executing ZeroChat.Server.Console.exe. The parameters are `RequestPort` and `MessagePort` respectively. Those are the default values.

```bash
ZeroChat.Server.Console.exe 5559 5560
```

## Running the Client Application
Executing ZeroChat.Client.Wpf.exe. The parameters are `RequestPort` and `MessagePort` respectively. Those are the default values.

```bash
ZeroChat.Client.Wpf.exe 5559 5560
```

## Solution and Projects
ZeroChat is a single .Net 6.0 Solution, developed with Clean Architecture:

### ZeroChat.Shared.Domain
Contains domain classes used by the common use cases

### ZeroChat.Shared.Application
Contains application classes that implement business use cases:
- Send Request calls using ZeroMQ RequestSocket
- Receive Request calls using ZeroMQ ResponseSocket
- Publish Messages using ZeroMQ PublisherSocket
- Subscribe to topics and receive Messages using ZeroMQ SubscriberSocket

All Sockets are enclosed by Runners.
Runners are abstractions for the Sockets and run inside a loop.

### ZeroChat.Client.Wpf
A WPF client application. Connects to default ports and opens `DefaultChannel` topic for sending and receiving messages.

### ZeroChat.Server.Console
A Console application. Binds to default ports - or receive them as parameters - receiving requests and forwarding them to be published as messages.

## Protocols
The Application implements a simple protocols.
There are 2 ports: `RequestPort` and `MessagePort`.
Server binds respectively to these ports using ZeroMQ's `ResponseSocket` and `PublisherSocket`, while Client connects to these ports using ZeroMQ's `RequestSocket` and `SubscriberSocket`.

The `RequestPort` has a default value of `5559`. 
The server binds to `@tcp://localhost:5559` with a `ResponseSocket`, while the client connects to `tcp://localhost:5559` with a `RequestSocket`.

The `MessagePort` has a default value of `5560`.
The server binds to `@tcp://localhost:5560` with a `PublisherSocket`, while the client connects to `tcp://localhost:5560` with a `SubscriberSocket`.

### Send chat messages:
From the client-side, using `RequestRunner`, sending chat messages are a Request call:
- First frame is the topic
- Second frame is the message payload

On the server-side, using `ResponseRunner`, received requests are handled and forwarded to the `PublisherRunner`, throughout `MessageRequestHandler`.
### Receive chat messages
Using `SubscriberRunner`, for the specific topic, chat messages are received:
  - The first and only frame is the message payload

### Runners
Runners are connected to channels - they operate as queues.
To send a request, for instance, a `RequestCall` is enqueued on the `RequestChannel`, and to publish a message, a `Message` is enqueued on the `MessageChannel`.

### RequestRunner (client-side)
`RequestRunner` waits for a `RequestCall` on the `RequestChannel`.
It makes the `Request` and receives a `Response`. The response is sent back to the caller by the `Callback<Response>`.
The `RequestCall` is essentially a `Request` and a `Callback<Response>` together.
This way, request-response is asynchronous on the client-side.

### ResponseRunner (server-side)
`ResponseRunner` receives requests from the socket. Every request is handled by `MessageRequestHandler`, which takes the received message and pushes to the `MessageChannel` to be published to all subscribers.

### PublisherRunner (server-side)
`PublisherRunner` waits a `Message` on the `MessageChannel`. When there is a message, it dequeues it and publishes on the `Topic`.
A `Message` has a `Topic` and `Payload`, both `string` type.

### SubscriberRunner (client-side)
`SubscriberRunner` receives `Message` from the socket, given a specified `Topic`. When there is a `Message`, it sends to the `MessageChannel`.

