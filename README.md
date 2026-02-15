# XEvent Platform

> ⚠️ **Important Notice**  
> This is a **sample / experimental project**.  
> Some of the features described below may be **partially implemented, simplified, or not implemented yet**.  
> The main goal of this project is to demonstrate **architecture, boundaries, and design decisions**, not production readiness.

---

## 1. Overview

**XEvent** is a modular, event-driven platform for managing events, tickets, reservations, payments, feedback, and reporting.

The system is designed around **Bounded Contexts (BCs)** with clear responsibilities, explicit contracts, and asynchronous communication using events.

### Key Characteristics

- Domain-Driven Design (DDD)
- Event-driven architecture
- Clear Bounded Context separation
- Kafka-based integration between BCs
- Redis for temporary state and capacity control
- Simple application services (no dispatcher pattern)
- Event publishing and Outbox simulation
- Acceptance tests with layered test drivers
- Infrastructure as Code (IaC) and Docker-based setup

---
## 2. Project Structure

```text
XEvent/
├── README.md
├── src/
│   ├── EventManagement/   # Event & content management BC
│   ├── Ticketing/         # Ticket types and capacity BC
│   ├── Reservation/      # Reservation & attendees BC
│   ├── Payment/          # Payment BC
│   ├── Feedback/         # Feedback BC
│   ├── Reporting/        # Read-model & analytics BC
│   ├── Common/           # Shared libraries (Quantum, HATEOAS)
│   └── Api/              # API layer & controllers
├── tests/
│   └── AcceptanceTests/
├── IaaC/
│   ├── Dockerfile
│   ├── docker-compose.yml
│   ├── ci/
│   │   ├── build.yml
│   │   └── deploy.yml
│   └── scripts/
└── docs/
    └── architecture.md
```

---

## 3. Bounded Contexts

| Bounded Context | Responsibility |
|----------------|----------------|
| Event Management | Event content, images, descriptions, metadata |
| Ticketing | Ticket types and capacity definitions |
| Reservation | Temporary & confirmed reservations, attendees, QR codes |
| Payment | Payment lifecycle and gateway integration |
| Feedback | Collecting feedback after events |
| Reporting | Read models, projections, analytics |

### Why BC Separation?

- Independent evolution
- Clear ownership and responsibility
- Reduced coupling
- Easier scaling
- Explicit integration via events

---

## 4. Architecture & Communication

### 4.1 Event-Driven Integration

- Each BC exposes:
  - Its own API
  - Its own domain events
- Domain events can be:
  - Consumed directly via API
  - Published to Kafka via a connector
- Outbox pattern is **simulated** for simplicity

---

## 5. Reservation Flow

1. User creates a reservation request.
2. Reservation BC checks ticket capacity.
3. Capacity is temporarily locked in Redis.
4. Reservation is created with a TTL (expiration).
5. Payment is initiated.
6. On successful payment:
   - Reservation is confirmed.
   - Capacity is finalized.
7. Each attendee receives:
   - A **unique ticket**
   - A **unique QR code**
8. Cancellation:
   - Releases capacity
   - Invalidates QR codes

---

## 6. Payment Flow

1. Payment BC handles gateway redirection.
2. Payment result is published as an event:
   - `PaymentSucceeded`
   - `PaymentFailed`
3. Reservation BC reacts:
   - Confirms reservation
   - Or cancels and releases capacity

---

## 7. Feedback Flow (Delayed Processing)

### Problem
Feedback should be requested **24 hours after an event ends**, without polling the database.

### Solution
Event-based delayed processing using Kafka topics.

### Flow

1. `EventCompleted` is published.
2. Feedback BC consumes the event.
3. If `now >= eventEnd + 24h`:
   - Feedback request is created.
4. Otherwise:
   - Message is published to a **delayed-feedback topic**
5. Delayed topic is periodically reprocessed until eligible.

This pattern is inspired by **delayed message processing** used in large-scale systems (e.g. log-based scheduling, retry topics).

---

## 8. Reporting

- Reporting BC is **read-only**
- No business logic
- Built from:
  - Event projections
  - Reservation events
  - Ticket and payment events
- Optimized for queries and analytics

---

## 9. APIs & Application Services

- Each BC exposes its own API
- Application services:
  - Are functional and explicit
  - Use a **simple service-based approach**
  - No command dispatcher pattern
- Domain logic stays inside aggregates

---

## 10. Testing Strategy

### 10.1 Unit Tests
- Domain rules
- Aggregates
- Application services

### 10.2 Acceptance Tests

- Very limited, focused scenarios
- Examples:
  - Reservation creation
  - Payment confirmation
  - Feedback scheduling
- Architecture:
  - **Test Driver** defines entry layer (API / App Service)
  - Tests can start from different layers
  - Infrastructure can be in-memory or simulated (Kafka, Redis)

---

## 11. Infrastructure as Code (IaC)
```text
IaaC/
├── Dockerfile
├── docker-compose.yml
├── ci/
│ ├── build.yml
│ └── deploy.yml
└── scripts/
```

- Docker-based local development
- CI/CD pipelines:
  - Build
  - Test
  - Package
  - Deploy
- Each BC can be deployed independently

---

## 12. Shared Libraries

To keep the project intentionally simple at this stage, the following shared libraries are **not used directly** in the current implementation.  
They are referenced mainly as architectural and conceptual foundations and may be gradually introduced as the system evolves.

Both libraries are based on **open-source projects originally authored by the same developer of this system**, which allows safe, controlled, and incremental adoption without introducing external dependencies prematurely.

### Quantum  
Based on: https://github.com/Quantum-Space-Org

- Aggregate root base classes  
- Domain events  
- Event sourcing utilities  

*(Open source. Originally authored by the system owner. Currently **not directly integrated**; only selected concepts are applied to avoid over-engineering.)*

### HATEOAS  
https://github.com/masoud-bahrami/HATEOAS.Net

- Hypermedia support for APIs  

*(Open source. Originally authored by the system owner. **Not enabled by default** and intended for future use where hypermedia provides clear benefits.)*


---

## 13. Important Notes

- Redis is used for:
  - Temporary reservations
  - Capacity control
- Kafka is used for:
  - Cross-BC communication
  - Delayed processing
- All BCs are stateless
- This project prioritizes:
  - Clarity
  - Architecture
  - Design decisions

---

## 14. Disclaimer

This project is **not production-ready**.

Its purpose is to:
- Explore architectural patterns
- Demonstrate BC separation
- Show event-driven workflows
- Serve as a learning and discussion base
