# OrderProcessingSystem

High-throughput order processing API built with .NET 8, Clean Architecture, CQRS, and Transactional Outbox pattern. Guarantees zero overselling under concurrent load and at-least-once event delivery.

## **1. Setup Instructions**

### **Prerequisites**
- .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
- SQL Server LocalDB: comes with Visual Studio. Verify: `sqllocaldb i MSSQLLocalDB`
- Optional: k6 for load testing: https://k6.io/docs/get-started/installation/

### **1.1 Clone & Restore**
```bash
git clone <your-repo-url>
cd OrderProcessingSystem
dotnet restore
