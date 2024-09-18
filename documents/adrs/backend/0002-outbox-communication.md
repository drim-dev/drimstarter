---
status: Proposed
date: 2024-09-15
deciders: Asset Otinchiyev
---

# AD: Use Transactional Outbox with RabbitMQ and Kafka for Microservices Asynchronous Communication

## Context and Problem Statement

In developing a backend for a crowdfunding platform Drimstarter, the need for reliable, scalable, and fault-tolerant message delivery in asynchronous communication between microservices has emerged. Messages between services, such as notifications, payment updates, or email triggers, need to be delivered reliably even in the case of failures, downtime, or errors. The primary question is: "Which approach ensures reliable asynchronous message delivery while maintaining transactional integrity?"

## Decision Drivers

* **Reliability and Resilience**: The solution must guarantee that messages are delivered reliably even in case of failures, such as network issues or service crashes.
* **Scalability**: The solution should handle increasing loads of outgoing messages without degrading system performance.
* **Decoupling**: The message sending process must be decoupled from the core business logic to avoid blocking operations.
* **Transactional Integrity**: Messages should only be sent if the associated business logic is successfully committed.
* **Developer Productivity**: The approach should be straightforward to implement, test, and maintain in a distributed microservices environment.

## Considered Options

1. **Transactional Outbox with RabbitMQ**
2. **Transactional Outbox with Kafka**
3. **Outbox Pattern**
4. **Immediate Synchronous Sending with Retry Logic**
5. **Manual Retry Mechanism**

### Transactional Outbox with RabbitMQ

* Sends messages reliably using RabbitMQ as the message broker after being written to an outbox within the same transaction as the business logic.

**Good, because**:

* RabbitMQ provides a robust queuing mechanism with built-in retries, fault tolerance, and message durability.
* It is easier to set up and maintain compared to Kafka, and integrates well with existing microservices architecture.
* Guarantees that messages are sent after the successful completion of business transactions.

**Bad, because**:

* Adds additional infrastructure overhead by requiring RabbitMQ cluster management.
* While RabbitMQ is highly reliable, it may not handle extremely high throughput as efficiently as Kafka in large-scale systems.

### Transactional Outbox with Kafka

* Kafka is used as the message broker to handle asynchronous communication after messages are stored in the outbox within the same transaction as the business logic.

**Good, because**:

* Kafka is highly scalable and well-suited for handling large volumes of messages with strong durability and fault tolerance.
* Offers exactly-once message delivery semantics in combination with a transactional outbox pattern, providing robust reliability guarantees.
* Kafka’s distributed architecture allows for high throughput, making it suitable for scaling in larger environments.

**Bad, because**:

* Setting up and maintaining Kafka is complex and requires significant operational expertise.
* Kafka introduces additional latency due to its distributed nature, which may not be needed for smaller-scale systems.

### Outbox Pattern

* In the **Outbox Pattern**, messages are stored in a dedicated outbox table in the same transaction as the business logic. A background process asynchronously reads the outbox and delivers the messages to their destinations.

**Good, because**:

* Ensures transactional integrity by committing messages only if the business logic commits successfully.
* Allows decoupling message production from delivery, improving system scalability and fault tolerance.
* Simple and flexible enough for a variety of asynchronous communication scenarios.

**Bad, because**:

* Requires a background service to poll the outbox table, adding additional infrastructure complexity.
* Can introduce delays in message delivery based on how frequently the outbox is processed.
* Outbox table management increases operational overhead, especially for high-traffic services.

### Immediate Synchronous Sending with Retry Logic

* Sends messages immediately within the business logic process, with retry mechanisms in case of failure.

**Good, because**:

* Simple to implement with immediate feedback on message delivery success or failure.
* No need for background services or outbox management.

**Bad, because**:

* Coupling message sending with the business logic can cause delays and potential failures in core transactions.
* Retrying on failure can block business processes, degrading user experience.
* Increased complexity in error handling within the business logic.

### Manual Retry Mechanism

* Failed messages are marked as failed, and users are responsible for manually retrying message delivery.

**Good, because**:

* Simple implementation without background processing or retry logic.
* Relies on user intervention for failure recovery.

**Bad, because**:

* Risk of human error or delays in message delivery.
* Poor user experience due to manual intervention.
* Not scalable for high-volume or critical message processing.

## Decision Outcome

Chosen options: **Transactional Outbox with RabbitMQ** and **Transactional Outbox with Kafka**, because they provide a combination of strong message delivery guarantees, scalability, and fault tolerance, while ensuring transactional integrity and decoupling message production from delivery.

### Consequences

#### Transactional Outbox with RabbitMQ

**Good, because**:

* It ensures that messages are only sent if the related business transaction is successfully committed, avoiding inconsistencies.
* Decoupling message delivery from business logic makes the system more resilient and scalable.
* RabbitMQ provides fault-tolerant and highly scalable solutions with built-in retry and message durability mechanisms.

**Bad, because**:

* Adds complexity in managing infrastructure for RabbitMQ.
* Requires additional storage and monitoring for managing the outbox and messaging queues, increasing infrastructure overhead.

#### Transactional Outbox with Kafka

**Good, because**:

* Kafka is highly scalable and can handle a massive volume of messages with high throughput and durability.
* The exactly-once delivery semantics in Kafka ensure robust reliability, particularly for critical transactional systems.
* Kafka’s distributed nature makes it ideal for large-scale systems requiring fault tolerance and horizontal scaling.

**Bad, because**:

* Kafka setup and maintenance are more complex than RabbitMQ, requiring specialized operational expertise.
* Kafka’s distributed architecture introduces some latency, which may be overkill for smaller systems or low-volume use cases.
* Requires careful tuning of brokers, partitions, and consumers to achieve optimal performance and avoid bottlenecks.

### Confirmation

To confirm that the implementation aligns with this ADR, the team will conduct system-level tests to ensure that messages are only sent if the transaction commits successfully. Monitoring and logging will be used to track message statuses, retries, and failures. Performance benchmarks will be established to assess the impact of the outbox processing on system throughput.

## More Information

The combination of Transactional Outbox with RabbitMQ and Kafka was selected to ensure reliable, scalable, and fault-tolerant message delivery while maintaining transactional integrity. RabbitMQ will be used for simpler, lower-throughput use cases, while Kafka will be utilized in scenarios requiring high throughput and distributed processing. The team will implement background workers to process the outbox asynchronously, ensuring scalability and resilience. The decision will be re-evaluated as the system evolves and load increases.