---
status: Proposed
date: 2024-09-08
deciders: Dmitriy Melnik
---

# AD: Use gRPC for Microservices Synchronous Communication

## Context and Problem Statement

In developing a backend for a crowdfunding platform Drimstarter, the need for a reliable, scalable, and efficient communication protocol between microservices has emerged. With different components of the system demanding quick and robust interaction patterns for handling complex user interactions and transactional data, the choice of communication protocol becomes critical. The primary question is: "Which communication protocol best facilitates microservice interactions while ensuring scalability, performance, and maintainability?"

## Decision Drivers

* __Performance and Efficiency__: The protocol must handle high throughput and low latency to meet user expectations and system efficiency.
* __Scalability__: As the platform grows, the communication protocol should easily scale with increasing load and service complexity.
* __Interoperability__: Different services may be developed using different technologies; hence the protocol should support broad interoperability.
* __Developer Productivity__: The protocol should support clear and fast development cycles, including easy testing and debugging.

## Considered Options

1. __gRPC__
2. __RESTful HTTP/JSON__
3. __GraphQL__

## Decision Outcome

Chosen option: "gRPC", because it provides the best balance between performance, scalability, and developer productivity, especially suitable for internal service communications in microservices architecture.

### Consequences

Good, because:

* It supports bidirectional streaming and keeps persistent connections, which is efficient for real-time data processing.
* Schema-first approach ensures that services are strictly typed and adhere to agreed contracts, reducing integration bugs.

Bad, because:

* It is less flexible in terms of browser-based clients and public APIs where REST or GraphQL might be more suitable.
* Requires additional tools and setups for load balancing and polyglot environments which may increase the infrastructure complexity.

### Confirmation

To confirm that the implementation aligns with this ADR, the team will conduct periodic design reviews and use integration testing frameworks specific to gRPC. Performance metrics and service logs will also be monitored to ensure the protocol meets the system’s demand efficiently.

## Pros and Cons of the Options

### gRPC

* Uses HTTP/2 for transport, supporting efficient multiplexing and streams.
* Schema-based contract with Protocol Buffers enhances type-safety and performance.
* Native support for bidirectional streaming.

Good, because:

* Highly efficient in terms of latency and bandwidth.
* Strongly typed service contracts increase reliability.
* Supports streaming, ideal for real-time data.

Bad, because:

* Less human-readable, complicating debugging and manual testing.
* Limited browser support requires additional technologies for client-facing APIs.

### RESTful HTTP/JSON

* Ubiquitous and widely supported across different platforms and languages.
* Human-readable, making development and debugging simpler.

Good, because:

* Easy to use and understand, wide adoption.
* Flexible in handling different types of calls and responses.

Bad, because:

* Higher latency and overhead due to HTTP/1.1 (if not using HTTP/2).
* Requires more careful design to avoid under-fetching and over-fetching issues.

### GraphQL

* Allows clients to request exactly the data they need.
* Can aggregate data from multiple sources.

Good, because:

* Reduces the amount of data transferred over the network.
* Increases flexibility for frontend developers.

Bad, because:

* Complex to implement on the server side.
* Caching and rate limiting are more challenging than with REST or gRPC.

## More Information

This decision was made in alignment with the overall system architecture that prioritizes performance and efficiency for backend services. The team has agreed to re-evaluate this decision annually or in the event of significant changes in load or service requirements. For further insights and comparisons, refer to resources like Google’s Cloud Architecture Center and industry benchmarks on communication protocols in microservices.
