# Azure Function - Excel Order Sync  
### How to survive Excel Hell (until it can be replaced)

## Background

In this scenario, a client was using **Microsoft Excel as the primary data store**
for order management.

Not as an export.  
Not as a report.  
But effectively as a database.

Replacing Excel with a proper database was the long-term goal, but such a change
could not happen immediately. Meanwhile, multiple systems still needed to
send updates and manual edits were becoming increasingly risky.

The question was not *“How do we get rid of Excel?”*  
The question was:

> **“How do we work with Excel in a controlled way until we can replace it?”**

---

## What this project demonstrates

This repository shows a pragmatic, transitional solution that introduces
structure and control around an Excel-based workflow.

An **Azure Function** acts as a single, well-defined entry point that:

- receives order updates via **Azure Service Bus**,
- validates incoming messages,
- processes batch operations (`add`, `update`, `delete`),
- applies them to a structured **Excel Table** stored in **Azure Blob Storage**,
- and routes invalid or failed messages to a **Dead Letter Queue (DLQ)**.

Excel remains the storage format, but **is no longer edited directly**.

---

## Why this approach

The goal was not to pretend Excel is a database.

The goal was to:
- reduce accidental data corruption,
- centralize all modifications,
- make changes auditable and repeatable,
- and create a clear migration path forward.

---

## Key characteristics

- Message-driven integration (Azure Service Bus)
- Explicit input validation
- Batch processing of mixed operations
- Centralized access to Excel via Azure Blob Storage
- Clear separation of concerns:
  - transport
  - validation
  - processing
- Predictable error handling using DLQ

---

## Who this is for

This project is meant as:
- a technical demonstration,
- a discussion starter,
- and a realistic example of working within legacy constraints.

Anyone who has ever inherited an “Excel-based system”
will recognize the problem immediately.

---

## Technical Overview

This section describes how the solution works from a technical perspective.

### High-level flow

1. An upstream system publishes a message with order updates to an **Azure Service Bus** queue.
2. An **Azure Function (isolated worker)** is triggered by the message.
3. The message payload is deserialized and validated.
4. Valid batch operations (`add`, `update`, `delete`) are applied to a structured
   **Excel Table** stored in **Azure Blob Storage**.
5. The updated Excel file is written back to Blob Storage.
6. If processing fails, the message is routed to a **Dead Letter Queue (DLQ)**.

---

## Message format

The Azure Function processes batch order updates defined by the
`OrderRowOperation` model.

Each message contains a list of row-level operations applied
to an Excel table.

---

## Operation examples

### Add - create a new order row

```json
{
  "Action": "add",
  "OrderLineId": "T-005",
  "OrderId": "ORD-TEST",
  "ProductName": "Test product",
  "Quantity": 1,
  "Price": 100,
  "Status": "New"
}
```

Used to append a new row to the Excel table.

Typically provided fields:
- `OrderLineId`
- `OrderId`
- `ProductName`
- `Quantity`
- `Price`
- `Status` (optional)

---

### Update - modify an existing order row

```json
{
  "Action": "update",
  "OrderLineId": "T-001",
  "Quantity": 2,
  "Price": 150,
  "Status": "Updated"
}
```

Used to update an existing row.

Notes:
- `OrderLineId` identifies the target row
- only provided fields are updated
- missing fields remain unchanged

---

### Delete - remove an order row

```json
{
  "Action": "delete",
  "OrderLineId": "T-002"
}
```

Used to remove a row from the Excel table.

---

## Full batch example

```json
{
  "OrderUpdates": [
    {
      "Action": "add",
      "OrderLineId": "T-005",
      "OrderId": "ORD-TEST",
      "ProductName": "Test product",
      "Quantity": 1,
      "Price": 100,
      "Status": "New"
    },
    {
      "Action": "update",
      "OrderLineId": "T-001",
      "Quantity": 2,
      "Price": 150
    },
    {
      "Action": "delete",
      "OrderLineId": "T-002"
    }
  ]
}
```
