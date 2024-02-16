# Joined Network Interactions
A .NET project that explores client(Baker, Eater)-server(Canteen) architecture using different types of network components. 
Canteen uses RabbitMQ while Baker uses gRPC and Eater uses SimpleRpc (https://github.com/DaniilSokolyuk/SimpleRpc). 
Using additional adapters, these components were joined and works together.

## The server-client logic
There are 24 hours in a day. 

Baker:
```
From 8 to 16 generates random positive amounts of new food portions baked. 
From 17 to 7 does nothing.

Baked amounts are accumulated in the server.
```

Eater:
```
From 11 to 18 generates random positive amounts of new food portions eaten. 
From 18H to 10H does nothing.

Eaten amounts are removed from the server.
```

Canteen:
```
The eaten amount increases profit and reputation proportionally.

If there is not enough food, reputation decreases proportionally.

After 19, portions left are thrown away and profit decreases proportionally.

```

Rules:
```
If profit and/or reputation is negative for 2 days, the canteen is closed for 1 day.
Afterwards, everything gets reset and the canteen restarts freshly.
```
