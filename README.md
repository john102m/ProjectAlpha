# 🧪 Microservices + RabbitMQ Experiment

This repo showcases an end-to-end microservice architecture with **5 services** communicating asynchronously using **RabbitMQ** and exposed externally via a **YARP API Gateway**. The project is built using .NET 8 and demonstrates scalable design patterns for event-driven systems.

---

## 🧩 Services Overview

| Service        | Purpose                                  | Language | Status |
|----------------|-------------------------------------------|----------|--------|
| **BookingService**  | Accepts bookings and publishes messages to RabbitMQ | C# (.NET 8) | ✅ Working |
| **EmailService**    | Listens to RabbitMQ queue and logs booking confirmations | C# (.NET 8) | ✅ Consuming |
| **CatalogService**  | Responds to health checks and stub data | C# (.NET 8) | ✅ Routing |
| **YARP Gateway**    | Reverse proxies requests to internal services | C# (.NET 8) | ✅ Transform routing |
| **RabbitMQ**        | Message broker handling queues and exchanges | Docker container | ✅ Running |

---

## 🔁 Message Flow

1. **BookingService** publishes a booking message to `booking-confirmed` queue via RabbitMQ.
2. **EmailService** listens to this queue using a background service and logs the booking.
3. Messages are acknowledged once consumed.
4. Dead-letter queues (DLQ) can be configured to catch failed or rejected messages.
5. API Gateway routes `POST /booking/book` to BookingService, and `/catalog/home` to CatalogService, etc.

---

## 🐇 RabbitMQ

RabbitMQ runs in a Docker container exposed on:

- AMQP: `amqp://localhost:5672`
- Admin UI: [http://localhost:15672](http://localhost:15672)  
  (default: `guest` / `guest`)

Messages are sent using `BasicPublishAsync()` from BookingService and consumed via `AsyncEventingBasicConsumer` in EmailService.

---

## ⚙️ Tech Stack

- ASP.NET Core (.NET 8)
- RabbitMQ.Client v7+
- YARP (Yet Another Reverse Proxy)
- Docker
- Visual Studio 2022

---

## 🛠️ Running Locally

1. Start RabbitMQ via Docker:

```bash
docker run -d --hostname rabbitmq --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management
