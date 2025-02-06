# masstransit.rabbitmq.sample
MassTransit RabbitMQ Sample

## RabbitMQ Type
- direct：精確匹配路由鍵。
- fanout：將消息廣播給所有綁定的隊列，無論路由鍵為何。
- headers：根據消息的標頭進行路由。
- topic：根據路由鍵模式進行模糊匹配，支持通配符。

## MassTransit.RabbitMQ

- [RabbitMQ Transport](https://masstransit.io/documentation/transports/rabbitmq/)
- [Exceptions](https://masstransit.io/documentation/concepts/exceptions/)
