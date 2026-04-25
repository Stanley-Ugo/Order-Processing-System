# OrderProcessingSystem

High-throughput order processing API built with .NET 8, Clean Architecture, CQRS, and Transactional Outbox pattern. Guarantees zero overselling under concurrent load and at-least-once event delivery.

## **1. Setup Instructions**

### **Prerequisites**
- .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
- SQL Server LocalDB: comes with Visual Studio. Verify: `sqllocaldb i MSSQLLocalDB`

### **1.1 Clone & Restore**
```bash
git clone https://github.com/Stanley-Ugo/Order-Processing-System.git
cd OrderProcessingSystem.Api
dotnet restore

1.2 Database Migration + Seed
LocalDB connection string is in Api/appsettings.json and Worker/appsettings.json.

dotnet ef database update --project OrderProcessingSystem.Infrastructure --startup-project OrderProcessingSystem.Api

This creates OrderProcessingSystemDb with tables: Orders, Products, OrderItems, OutboxMessages and seeds 3 products.

1.3 Run API + Worker
Visual Studio 2022:

Right-click Solution → Properties → Startup Projects
Select "Multiple startup projects"
Set OrderProcessingSystem.Api = Start, OrderProcessingSystem.Worker = Start
Press F5
CLI:

    # Terminal 1
cd OrderProcessingSystem.Api
dotnet run

# Terminal 2
cd OrderProcessingSystem.Worker  
dotnet run

API: https://localhost:5001/swagger
Worker: Polls OutboxMessages every 5s, logs to console

1.4 Test Happy Path

curl -X POST https://localhost:5001/api/orders \
  -H "Idempotency-Key: 11111111-1111-1111-1111-111111111111" \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "items": [{"productId": "11111111-1111-1111-1111-111111111111", "quantity": 2}]
  }'

  Expected: 201 Created.

Verify:

SELECT Stock FROM Products WHERE Sku='TSHIRT-RED-L' -- 98
SELECT * FROM OutboxMessages -- 1 row, ProcessedOn updated by Worker

1.5 Load Test - Concurrency Proof
Reset stock to 10, then run 200 concurrent requests for qty=1:

UPDATE Products SET Stock = 10 WHERE Sku='TSHIRT-RED-L'

k6 run loadtest.js

Check result:

SELECT Stock FROM Products WHERE Sku='TSHIRT-RED-L' -- Must be exactly 0
SELECT COUNT(*) FROM Orders -- Must be exactly 10

This proves no overselling. 10 succeed with 201, 190 get 409 Conflict.


2. Architecture

OrderProcessingSystem/
├── Domain/                  # Core business logic. No dependencies.
│   ├── Entities/            # Order, Product, OrderItem
│   ├── Events/              # OrderPlacedEvent  
│   └── Common/              # OutboxMessage, Result<T>
├── Application/             # Use cases. Depends on Domain.
│   ├── Common/Interfaces/   # IApplicationDbContext, IDateTime
│   └── Orders/Commands/     # PlaceOrderCommand + Handler
├── Infrastructure/          # External concerns. Depends on Application.
│   ├── Persistence/         # AppDbContext, EF Configurations, Migrations
│   └── Outbox/              # OutboxMessage entity config
├── Api/                     # Entry point. Depends on Application, Infrastructure.
│   ├── Controllers/         # OrdersController, ProductsController
│   └── Program.cs           # DI, Middleware
└── Worker/                  # Background service. Depends on Infrastructure.
    └── OutboxProcessor.cs   # Polls outbox, marks processed


Request Flow

POST /api/orders with Idempotency-Key header
PlaceOrderCommandHandler starts Serializable transaction
Load products, check stock, decrement stock
Create Order aggregate
Create OutboxMessage with OrderPlacedEvent JSON
SaveChangesAsync() commits Order + Stock + Outbox atomically
Return 201 to client
Worker polls OutboxMessages WHERE ProcessedOn IS NULL
Worker logs event and sets ProcessedOn = UtcNow


3. Architecture Decisions

Clean Architecture
Domain isolated from EF/HTTP. Easy to unit test. Can swap SQL Server for Postgres without touching business logic.

CQRS + MediatR
Write path PlaceOrderCommand has complex logic: idempotency, concurrency, outbox. Read path can be optimized separately with Dapper later.

Transactional Outbox
Solves dual-write problem. DB tx guarantees Order + Event both persist or both rollback. Avoids 2PC/distributed transactions. Gives at-least-once delivery.

EF Core + RowVersion
Product.RowVersion byte[] mapped to SQL rowversion. EF throws DbUpdateConcurrencyException if stock changed between read/update. Enables optimistic concurrency.

IsolationLevel.Serializable
Prevents phantom reads. Scenario: T1 reads Stock=1, T2 reads Stock=1, both try to order 1. Serializable ensures one blocks until other commits.

Idempotency-Key
HTTP-safe retries. Client can retry on network timeout. Server returns cached result if key seen. Unique index on Orders.IdempotencyKey.

BackgroundService Worker
Decouples HTTP response from event publishing. API returns in <50ms. Worker handles slow/failed publishes with retry.

OutboxMessage in Domain
Part of aggregate transaction boundary. Keeping it in Domain avoids circular deps. Alternative IOutboxStore adds complexity without benefit for this scope.

4. Assumptions
Product catalog immutable for MVP - Products seeded once. No admin CRUD. Price locked at order time to prevent changes mid-checkout.
Single inventory location - Product.Stock is global. No warehouses/regions. Multi-location would require reservation tables.
Payment external - Order placed before payment. Prod would use Saga: OrderPlaced → PaymentRequested → PaymentSucceeded → OrderConfirmed.
CustomerId from auth - No Customers table. GUID passed from JWT claim. Validation out of scope.
No partial orders - All items must have stock or entire order fails. No backorders. Simplifies tx logic.
LocalDB for assessment - No Docker/Azure SQL to keep setup <2 mins. Prod: Azure SQL with read replicas for queries.
Polling not push - Worker polls DB every 5s. Acceptable for assessment. Prod: SQL change tracking + Service Bus trigger for <1s latency.
Event schema stable - OrderPlacedEvent JSON in outbox. Schema evolution via V2 event types, not handled here.
