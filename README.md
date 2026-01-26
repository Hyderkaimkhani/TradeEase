# TradeEase

TradeEase is a multi-tenant supply chain management system with built-in accounting features. It helps companies manage customers, supplies, orders, accounts, and financial transactions in a structured and scalable way. The application supports multiple companies (tenants), each with its own accounts, users, and data separation.

# Features
## Core Supply Chain Modules

**Company Management** – Set up and manage multiple companies (multi-tenant support).

**Customer Management** – Add and manage customers with integrated accounting.

**Supply Management** – Create and track supplies linked to customers.

**Order Management** – Create orders against customers from existing supply (linked via TruckNumber or directly).

**Payments** – Send or receive payments against customers or through accounts.

## Accounting & Finance

**Accounts** – Each company has default virtual accounts (payable & receivable). Users can also create additional accounts.

**Double-Entry Transactions** – Automated debit/credit entries for every customer or account transaction.

**Expense Tracking** – Record and categorize expenses.

**Income Tracking** – Record and categorize income.

**Account Transfers** – Transfer amounts between accounts seamlessly.

**Statements** – Generate statements for customers or accounts for a clear financial view.

## Tech Stack

**Backend:** .NET Core (N-tier architecture → Controllers → Services → Repositories)

**Frontend:** Vue.js (TradeEase-Client)

**Database:** SQL (Database project)

**API Documentation:** Swagger

## Roadmap

### Planned improvements:

Enhanced reporting & analytics dashboards.

Notifications & alerts for key events (e.g., low balance, pending payments).

Role-based access control with fine-grained permissions.

Improved UX for order-to-supply linking workflows.

## Summary

TradeEase is not just a supply chain solution but also an integrated accounting system. From managing companies and customers to handling payments, expenses, and generating statements, it provides businesses with everything they need to manage operations and finances in one place.

<img width="200" height="350" alt="Dashboard" src="https://github.com/user-attachments/assets/1c223b9a-e419-4c6e-b258-939bbe237958" hspace="20" />
<img width="200" height="350" alt="Menu" src="https://github.com/user-attachments/assets/f925f4ae-78c6-44f7-a4ef-7d6b668ec073" hspace="20" />
 
<img width="200" height="350" alt="Customers" src="https://github.com/user-attachments/assets/46262c49-45fe-412e-aee4-2c6bdfd64e55"  hspace="20"/>
 
<img width="200" height="350" alt="Add Customer" src="https://github.com/user-attachments/assets/46262c49-45fe-412e-aee4-2c6bdfd64e55" hspace="20" />
<img width="200" height="350" alt="Supplies" src="https://github.com/user-attachments/assets/f348ff47-1dcc-4a78-a174-1e6e76bd9bc8"  hspace="20" />
<img width="200" height="350" alt="Add Supply" src="https://github.com/user-attachments/assets/e756116c-c00f-45cc-bb25-b81193be623a"  hspace="20"/>
<img width="200" height="350" alt="Add Order" src="https://github.com/user-attachments/assets/e64606de-84fc-4445-be87-f388f75e38bd" hspace="20" />
<img width="200" height="350" alt="Payments" src="https://github.com/user-attachments/assets/819a5003-a3fc-49de-9b82-754743e6c5f1"  hspace="20"/>
<img width="200" height="350" alt="Add Payment" src="https://github.com/user-attachments/assets/5a1c4202-426c-4fa8-b46a-2977747018c9" hspace="20" />
<img width="200" height="350" alt="Statement" src="https://github.com/user-attachments/assets/80dfcef4-5b44-4b32-abdb-9058819cb757"  hspace="20"/>
<img width="300" height="237" alt="Statement PDF" src="https://github.com/user-attachments/assets/ce708b99-51a7-4342-997d-2b5f188db51b" hspace="20" />
<img width="200" height="350" alt="Accounts" src="https://github.com/user-attachments/assets/69e6e5c0-515b-4aab-bbc9-41262627ed90"  hspace="20" />



