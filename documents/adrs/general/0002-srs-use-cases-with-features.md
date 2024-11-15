---
status: Proposed
date: 2024-09-15
deciders: Dmitriy Melnik
---

# AD: Use Software Requirements Specification Document with Use Cases and Features List

## Context and Problem Statement

The team is developing a web product similar to Kickstarter.com. Each team member works remotely and autonomously. There is no business analyst on the team, which creates a risk of miscommunication regarding the system's functional requirements. To ensure a clear, structured, and thorough understanding of the product's needs, the team has decided to document all software requirements in a Software Requirements Specification (SRS) document. This document is crucial to the project's success, given the distributed and autonomous working setup of the team.

The problem the team faces is how to structure and document the functional requirements effectively, ensuring that all team members have a shared understanding of the system’s behavior without direct, ongoing communication or oversight from a business analyst.

## Decision Drivers

* __Remote and Autonomous Team__: Each member works remotely with minimal direct communication, making it essential to have a detailed, self-explanatory, and central requirements document.
* __Lack of a Business Analyst__: Without a dedicated business analyst, it is necessary for developers to have a clear, well-structured source of truth for software requirements.
* __Ensuring Project Success__: The SRS serves as a key artifact to avoid misunderstandings and misalignment in development, ensuring that everyone is working towards the same goals.
* __Shared Understanding Across the Team__: A structured SRS that includes use cases and functional requirements ensures clarity and consistency across the team.

## Considered Options

1. Use an SRS with detailed use cases and functional requirements
2. Use agile user stories and informal documentation
3. Use minimal documentation with verbal communication and task tracking tools

## Decision Outcome

Chosen option: "Use an SRS with detailed use cases and functional requirements", because it provides a structured and clear approach to documenting the system's behavior, which is especially important for a remote team without a business analyst.

### Consequences

Good, because:

* The SRS provides a well-organized format for documenting both high-level use cases and detailed functional requirements.
* Including use cases ensures that the system's functionality is described from the user's perspective, while the list of features translates these use cases into actionable functional requirements.
* The SRS acts as a single source of truth, minimizing the risk of misinterpretation or conflicting understandings among team members.
* New team members or stakeholders can easily reference the document to understand the system's requirements.
* The structured approach allows the document to evolve as the project grows, with new use cases and features added as necessary.

Bad, because:

* Writing and updating the SRS with detailed use cases and functional requirements may require significant time and effort, particularly for developers who are also focused on implementation.
* If the project evolves rapidly, there may be a risk of the SRS becoming outdated unless it is regularly maintained.
* Features documented under different use cases might overlap, potentially leading to redundancy.

### Confirmation

To confirm that the SRS aligns with this ADR, the team will conduct periodic SRS reviews.

## Pros and Cons of the Options

### Use an SRS with detailed use cases and functional requirements

Good, because:

* Provides a detailed and structured format for documenting requirements.
* Helps ensure that all team members have a clear understanding of the system’s behavior.
* Reduces the risk of miscommunication in a remote, autonomous team.
* Offers a solid foundation for future maintenance, development, and onboarding.

Bad, because:

* Time-consuming to create and maintain.
* Risk of redundancy if use cases overlap.
* Requires discipline to keep the document updated as the system evolves.

### Use agile user stories and informal documentation

Good, because:

* More flexible and adaptable to changes in the project.
* Easier to implement in an agile development environment.
* Allows the team to focus more on development than on documentation.

Bad, because:

* Lack of a formal structure can lead to misunderstandings or misinterpretation of requirements.
* Informal documentation may not provide sufficient detail for complex features.
* Challenging for remote and autonomous teams where clear communication is essential.

### Use minimal documentation with verbal communication and task tracking tools

Good, because:

* Minimal overhead in terms of documentation.
* Encourages frequent communication among team members.
* Task tracking tools can be useful for assigning and managing work.

Bad, because:

* High risk of miscommunication, especially in a remote team without a business analyst.
* Lack of formal documentation may result in incomplete understanding of requirements.
* Difficult to onboard new team members or stakeholders.

## More Information

* The Anatomy of a Use Case - https://medium.com/analysts-corner/the-anatomy-of-a-use-case-c82cd127c8b9
