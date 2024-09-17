---
status: Proposed
date: 2024-09-15
deciders: Asset Otinchiyev
---

# AD: Use Outbox for Microservices Asynchronous Communication

## Context and Problem Statement

Users often face issues where outgoing messages fail to send immediately due to connectivity problems, server delays, or intentional scheduling. Without a clear and intuitive way to manage these pending messages, users may lose track of what needs attention or be unaware that their messages are still in queue, resulting in miscommunication and workflow disruptions. An effective “Outbox” feature is necessary to address these issues by providing transparency and control over outgoing communications."

## Decision Drivers


1. Reliability and Resilience

	•	__Challenge__: Users rely on timely message delivery. The system must ensure that messages in the outbox are either sent as soon as possible or remain in the queue for a reasonable retry duration if an initial attempt fails.

	•	__Driver__: The outbox should have robust mechanisms for retrying message delivery, handling network or server outages, and notifying users of failures.

2. User Control and Flexibility

	•	__Challenge__: Users may want control over when and how messages are sent (e.g., scheduling emails, pausing sending, or retrying failed messages manually).

	•	__Driver__: The outbox must provide users with clear options to edit, delete, retry, or schedule outgoing messages. Users should be able to review and manage these actions easily.

3. Visibility and Transparency

	•	__Challenge__: Users may not always be aware of the status of their outgoing messages, leading to confusion about whether a message has been sent, failed, or is still pending.

	•  __Driver__: The outbox interface must display the current status of each message (e.g., queued, failed, retrying) and provide clear notifications when further action is required from the user.

4. Performance Impact

	•	__Challenge__: Handling a large number of messages in the outbox (especially if they are large files or high-priority) should not negatively impact the performance of the system.

	•	__Driver__: The outbox should be optimized for performance, ensuring that messages are queued efficiently without causing delays to other system operations.

5. Error Handling and Recovery

	•	__Challenge__: Sending failures (e.g., due to server issues or incorrect addresses) can cause messages to stay in the outbox indefinitely without user awareness.

	•	__Driver__: The outbox should handle errors gracefully, providing users with clear messages explaining the issue and offering suggestions for resolution (e.g., correct the email address, reconnect to the internet, etc.).

6. Integration with Other Features

	•	__Challenge__: The outbox may interact with various system components, such as the draft folder, sent items, and messaging queues. It should work seamlessly with these features.

	•	__Driver__: The outbox should integrate smoothly with related features, like draft saving, attachment management, and message logs, to provide a seamless experience for users.


## Considered Options

### Option 1: **Outbox Pattern (Chosen)**
- **Description**: Use an Outbox table in the same transaction as the business logic, with a background service to process unsent messages asynchronously.
- **Pros**:
  - Ensures transactional consistency: Messages are only sent if the business logic commits successfully.
  - Improves reliability by retrying message delivery until successful.
  - Decouples message creation from delivery, making the system more scalable and fault-tolerant.
- **Cons**:
  - Adds complexity due to background services and management of the outbox table.
  - Asynchronous delivery can introduce slight delays depending on the polling frequency.
  - Additional storage needed for managing unsent messages in the database.

### Option 2: **Immediate Synchronous Sending with Retry Logic**
- **Description**: Send messages immediately within the same process as the business logic and retry on failure.
- **Pros**:
  - Simple architecture, no need for background processes or additional storage tables.
  - Immediate feedback on success or failure of message delivery.
- **Cons**:
  - Coupling message delivery with the business logic increases risk of failure or delays in core transactions.
  - Retrying on failure may block business operations, leading to poor user experience.
  - Complex error-handling code must be embedded in business logic, increasing system complexity.

### Option 3: **Event-Driven Architecture with Message Queues**
- **Description**: Use a message queue system (e.g., RabbitMQ, Kafka) to decouple message sending from the business logic. Business operations will publish messages to the queue, and consumers will process them asynchronously.
- **Pros**:
  - Highly scalable: Message queues allow for distributed processing of messages across multiple consumers.
  - Reliable message delivery: Built-in message retry mechanisms and fault tolerance in modern message queue systems.
  - Decouples message production from consumption, improving system flexibility.
- **Cons**:
  - Requires additional infrastructure and operational overhead (managing message queues, consumers, etc.).
  - Over-engineered for simple use cases where an outbox might suffice.
  - Increased complexity in message handling and processing logic.

### Option 4: **Manual Retry Mechanism**
- **Description**: Messages that fail to send are marked as failed, and users are notified to manually retry sending the message at a later time.
- **Pros**:
  - No need for complex background processing or retry logic in the system.
  - Simple to implement, relying on user intervention to handle failures.
- **Cons**:
  - Risk of human error or oversight, leading to failed or unsent messages.
  - Inconsistent user experience: Relying on users to manually retry messages may lead to delays or missed messages.
  - Not scalable for systems with high message volumes or mission-critical communications.

## Outcome
The Outbox Pattern was selected for its ability to balance reliability, scalability, and decoupling while ensuring transactional integrity. It provides a robust mechanism for handling outgoing messages in a distributed and fault-tolerant manner.

## Next Steps
1. **Design the Outbox Table**:
    - Define the schema to store messages, including fields like `message_id`, `payload`, `status`, `created_at`, and `retry_count`.
2. **Implement Background Workers**:
    - Create a background service that polls the outbox periodically and attempts to send pending messages.
3. **Test Transactional Integrity**:
    - Ensure that messages are written to the outbox as part of the same transaction as the core business operation.
4. **Monitor Message Delivery**:
    - Implement logging and monitoring to track message status, retry counts, and failures in the outbox.

## Related Decisions
- Use of event-driven architecture for certain asynchronous tasks (yet to be explored).
- Evaluation of alternative message queuing systems for future needs.
