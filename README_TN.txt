FACTURATION TN - PROFESSIONAL TWO-PROJECT SETUP

1) Backend project: FacturationTnApp
   - Put AppData.cs and DataStore.cs in the root of the backend project.
   - Put Customer.cs, Product.cs, InvoiceItem.cs, Invoice.cs inside Models folder.

2) Frontend project: FacturationTnAppWinForms
   - Keep only Program.cs and MainForm.cs in WinForms.
   - Remove Models, AppData.cs, DataStore.cs from WinForms if they still exist.

3) Add project reference
   - Right click FacturationTnAppWinForms
   - Add > Project Reference
   - Check FacturationTnApp

4) Startup project
   - Right click FacturationTnAppWinForms
   - Set as Startup Project

5) Run
   - Press F5

The UI includes all requested functionalities:
- Add Customer
- Delete Customer
- Add Product
- Delete Product
- Create Invoice
- Show Invoices
- Search Invoices by Customer
- Tax Dashboard
- Sales Dashboard / Revenue by Month
- Save Data to File
- Load Data from File
