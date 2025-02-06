# masstransit.rabbitmq.sample
RabbitMQ is an open-source message broker software that implements the Advanced Message Queuing Protocol (AMQP). It is written in the Erlang programming language and is built on the Open Telecom Platform framework for clustering and failover.

RabbitMQ can be used to decouple and distribute systems by sending messages between them. It supports a variety of messaging patterns, including point-to-point, publish/subscribe, and request/response.

RabbitMQ provides features such as routing, reliable delivery, and message persistence. It also has a built-in management interface that allows for monitoring and management of the broker, queues, and connections. Additionally, it supports various plugins, such as the RabbitMQ Management Plugin, that provide additional functionality.

## RabbitMQ Type
- direct：精確匹配路由鍵。
- fanout：將消息廣播給所有綁定的隊列，無論路由鍵為何。
- headers：根據消息的標頭進行路由。
- topic：根據路由鍵模式進行模糊匹配，支持通配符。

## MassTransit.RabbitMQ

- [RabbitMQ Transport](https://masstransit.io/documentation/transports/rabbitmq/)
- [Exceptions](https://masstransit.io/documentation/concepts/exceptions/)
