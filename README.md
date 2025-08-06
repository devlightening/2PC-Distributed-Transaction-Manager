Alright, here is a more professional and detailed README.md in English, incorporating suggestions for adding images to showcase the project in action.

-----

# 2PC-Distributed-Transaction-Manager

This repository provides a practical demonstration of the **Two-Phase Commit (2PC) protocol** for ensuring atomicity (all or nothing) in distributed transactions within a microservices architecture. It meticulously illustrates how a central coordinator service orchestrates transactions across multiple participant services, effectively handling potential failures to maintain data consistency.

The primary goal of this project is to offer a hands-on understanding of the challenges inherent in distributed data consistency and how the 2PC protocol provides a robust solution through a practical, working example.

## Architecture and Communication

The project features a central **Coordinator** service responsible for managing the 2PC protocol and three participant services that constitute a distributed transaction:

  * **`Coordinator`**: The orchestrator of distributed transactions. It initiates the process, sends `prepare` messages to all participants, collects their responses, and makes the final `commit` or `rollback` decision. Crucially, it persists transaction states in a database, enhancing the system's resilience to failures.
  * **`OrderAPI`**: Manages order creation operations and acts as a participant by adhering to instructions from the `Coordinator`.
  * **`StockAPI`**: Responsible for managing the stock levels of products related to orders. It responds to the `Coordinator`'s messages to reserve or release stock.
  * **`PaymentAPI`**: Handles payment processing. As a participant, it follows the directives issued by the `Coordinator`.

Inter-service communication occurs synchronously over HTTP API calls.

### Two-Phase Commit (2PC) Protocol Flow

The 2PC protocol involves two distinct phases:

#### Phase 1: Prepare Phase

1.  **Initiation**: The `Coordinator` service initiates a new distributed transaction and generates a unique `transactionId`.
2.  **Inquiry**: The `Coordinator` sends a "ready to commit?" (`/ready`) request, along with the `transactionId`, to all participant services (`OrderAPI`, `StockAPI`, `PaymentAPI`).
3.  **Voting**: Each participant service checks if it can successfully complete its part of the transaction. If so, it responds with "Ready" (`true`); otherwise, it replies with "Not Ready" (`false`).

#### Phase 2: Commit or Rollback Phase

1.  **Decision**: The `Coordinator` aggregates the responses from all participants.
2.  **Commit**: If **all** participants voted "Ready," the `Coordinator` instructs them to "commit the transaction" (`/commit`).
3.  **Rollback**: If **any** participant voted "Not Ready," the `Coordinator` aborts the transaction and instructs **all** participants to "rollback the transaction" (`/rollback`). This ensures the fundamental principle of atomicity.

This protocol guarantees strong consistency across distributed systems by ensuring that a transaction either fully succeeds across all involved services or is entirely rolled back, leaving the system in a consistent state.

## Technologies Used

  * **.NET 8**: The primary development platform for the project.
  * **ASP.NET Core Minimal API**: Used for building lightweight and fast API endpoints for the services.
  * **Entity Framework Core**: Facilitates database interactions and Object-Relational Mapping (ORM).
  * **SQL Server**: The relational database used to persist transaction states, ensuring durability.
  * **HttpClientFactory**: Employed for managing HTTP client instances for inter-service communication, promoting efficiency and reliability.

## Setup and Running the Project

Follow these steps to run the project locally.

### Prerequisites

  * [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
  * SQL Server ([Download](https://www.microsoft.com/sql-server/sql-server-downloads)) or a SQL Server container running via Docker.
  * Visual Studio ([Download](https://visualstudio.microsoft.com/)) or your preferred code editor.

### Steps

1.  Clone the repository:

    \`\`\`bash
    git clone [https://github.com/devlightening/2PC-Distributed-Transaction-Manager.git](https://www.google.com/search?q=https://github.com/devlightening/2PC-Distributed-Transaction-Manager.git)
    cd 2PC-Distributed-Transaction-Manager
    \`\`\`

2.  Update the database connection string (`"SQLServer"`) in the `appsettings.json` file of the `Coordinator` project to match your local SQL Server setup.

3.  Run the `Coordinator` project first to ensure the database schema is created and populate the `Nodes` table with the necessary service information.

4.  Open the solution in Visual Studio and configure all services (`Coordinator`, `OrderAPI`, `StockAPI`, `PaymentAPI`) to run as multiple startup projects. Then, run the application.

5.  Use an HTTP client (like Postman, cURL, or a web browser) to initiate a distributed transaction by sending a GET request to the following endpoint of the `Coordinator` service:

    \`\`\`
    GET https://localhost:7194/created-order-transaction
    \`\`\`

    Observe the console outputs of each service and the changes in the database tables to track the flow of the 2PC protocol.

## Demonstrations (Images)
--Successful Transaction (Commit)

<img width="1900" height="1010" alt="Everything is fine" src="https://github.com/user-attachments/assets/5bddfa48-f86b-405b-8a48-337e7e252b4c" />

This image demonstrates a successful distributed transaction workflow. The console outputs for OrderAPI, StockAPI, and PaymentAPI show that they all responded with is Readyyyy, followed by is Committed. This confirms that all participant services successfully completed the prepare phase and subsequently committed the transaction as directed by the coordinator. The NodeStateTBL in SQL Server further validates this, showing the TransactionState for the relevant records has been updated to a value indicating a successful commit.


--Failed Transaction and Rollback

<img width="1908" height="1017" alt="Rollbacked" src="https://github.com/user-attachments/assets/7f627038-83ff-40a1-9b9a-4f1e3017a87c" />

This screenshot illustrates the fault-tolerant mechanism of the Two-Phase Commit protocol. In this scenario, the OrderAPI service returns a FALSE status for its READY check. Consequently, the coordinator aborts the transaction, and the console outputs for all services show a is Rollbacked message. This confirms that even a single failure in the prepare phase leads to a full rollback of the distributed transaction, maintaining data integrity across all services. The NodeStateTBL would reflect this by updating the TransactionState to a value indicating an aborted state.

--Database State
<img width="1215" height="518" alt="OrderAPI  READY FALSE" src="https://github.com/user-attachments/assets/d4985776-accf-4d72-887b-9054669adc00" />

This image provides a snapshot of the TwoPhaseCommit database, specifically the Nodes and NodeStates tables. The Nodes table lists all the services participating in the distributed transactions, such as StockAPI, OrderAPI, and PaymentAPI, along with their unique identifiers. The NodeStates table is crucial for the coordinator, as it persistently stores the live status of each service for a given transaction, including IsReady and TransactionState. This persistence ensures the coordinator can effectively track the transaction and make correct decisions, even in the event of a system crash.




### Successful Transaction (Commit)

Include an image here showing the console outputs of all services indicating "Ready," followed by "Committed." Additionally, include a database snapshot of the `NodeStateTBL` showing the `TransactionState` as `2` (Done) for all involved nodes of a specific `TransactionId`.

### Failed Transaction (Rollback)

Include an image here showcasing the console output of one of the participant services indicating it's "Not Ready." Subsequently, show the console outputs of all services indicating "Rollbacked." Also, provide a database snapshot of the `NodeStateTBL` where the `TransactionState` is `3` (Aborted) for all nodes of the corresponding `TransactionId`.

These images will provide visual confirmation of the 2PC protocol's behavior under both successful and failed transaction scenarios.

## Further Exploration

This project serves as a foundational example of the Two-Phase Commit protocol. Potential areas for further exploration include:

  * Implementing more sophisticated error handling and recovery mechanisms.
  * Investigating alternative distributed transaction patterns like Saga.
  * Exploring different communication protocols (e.g., gRPC) for inter-service communication.
  * Adding unit and integration tests to ensure the reliability of the implementation.


-----
