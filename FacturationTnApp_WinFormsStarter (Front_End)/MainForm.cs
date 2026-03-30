using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using FacturationTnApp;
using FacturationTnApp.Models;

namespace FacturationTnAppWinForms
{
    public class MainForm : Form
    {
        private AppData data = new AppData();
        private readonly List<InvoiceItem> draftItems = new List<InvoiceItem>();
        private readonly string dataFilePath = "appdata_winforms.json";

        private readonly Button btnSave = new Button();
        private readonly Button btnLoad = new Button();
        private readonly Button btnResetDemo = new Button();
        private readonly Label lblFileName = new Label();

        private readonly TabControl tabControl = new TabControl();

        private readonly TextBox txtCustomerName = new TextBox();
        private readonly TextBox txtCustomerPhone = new TextBox();
        private readonly Button btnAddCustomer = new Button();
        private readonly Button btnDeleteCustomer = new Button();
        private readonly DataGridView dgvCustomers = new DataGridView();

        private readonly TextBox txtProductName = new TextBox();
        private readonly TextBox txtProductPrice = new TextBox();
        private readonly TextBox txtProductVat = new TextBox();
        private readonly Button btnAddProduct = new Button();
        private readonly Button btnDeleteProduct = new Button();
        private readonly DataGridView dgvProducts = new DataGridView();

        private readonly ComboBox cmbInvoiceCustomer = new ComboBox();
        private readonly ComboBox cmbInvoiceProduct = new ComboBox();
        private readonly NumericUpDown nudQuantity = new NumericUpDown();
        private readonly NumericUpDown nudTaxStamp = new NumericUpDown();
        private readonly Button btnAddDraftItem = new Button();
        private readonly Button btnRemoveDraftItem = new Button();
        private readonly Button btnCreateInvoice = new Button();
        private readonly TextBox txtSearchInvoiceCustomer = new TextBox();
        private readonly Button btnSearchInvoices = new Button();
        private readonly Button btnShowAllInvoices = new Button();
        private readonly DataGridView dgvDraftItems = new DataGridView();
        private readonly DataGridView dgvInvoices = new DataGridView();
        private readonly DataGridView dgvInvoiceDetails = new DataGridView();

        private readonly Label lblTotalVat = new Label();
        private readonly Label lblTotalStamp = new Label();
        private readonly Button btnRefreshTax = new Button();
        private readonly DataGridView dgvVatByRate = new DataGridView();

        private readonly Label lblRevenueHT = new Label();
        private readonly Label lblRevenueTTC = new Label();
        private readonly Button btnRefreshSales = new Button();
        private readonly DataGridView dgvRevenueByMonth = new DataGridView();
        private readonly DataGridView dgvRevenueByCustomer = new DataGridView();
        private readonly DataGridView dgvRevenueByProduct = new DataGridView();

        public MainForm()
        {
            Text = "Facturation TN App - Professional UI";
            Width = 1380;
            Height = 860;
            MinimumSize = new Size(1200, 760);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BackColor = Color.WhiteSmoke;

            BuildLayout();
            WireEvents();
            LoadOrSeed();
            RefreshAll();
        }

        private void BuildLayout()
        {
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                Padding = new Padding(12, 10, 12, 10),
                BackColor = Color.White
            };

            btnSave.Text = "Save";
            btnSave.Size = new Size(100, 32);
            btnSave.Location = new Point(12, 12);

            btnLoad.Text = "Load";
            btnLoad.Size = new Size(100, 32);
            btnLoad.Location = new Point(120, 12);

            btnResetDemo.Text = "Reset Demo Data";
            btnResetDemo.Size = new Size(130, 32);
            btnResetDemo.Location = new Point(228, 12);

            lblFileName.Text = "Data file: appdata_winforms.json";
            lblFileName.AutoSize = true;
            lblFileName.Location = new Point(380, 18);
            lblFileName.ForeColor = Color.DimGray;

            headerPanel.Controls.Add(btnSave);
            headerPanel.Controls.Add(btnLoad);
            headerPanel.Controls.Add(btnResetDemo);
            headerPanel.Controls.Add(lblFileName);

            tabControl.Dock = DockStyle.Fill;

            TabPage tabCustomers = new TabPage("Customers");
            TabPage tabProducts = new TabPage("Products");
            TabPage tabInvoices = new TabPage("Invoices");
            TabPage tabTaxDashboard = new TabPage("Tax Dashboard");
            TabPage tabSalesDashboard = new TabPage("Sales Dashboard");

            BuildCustomersTab(tabCustomers);
            BuildProductsTab(tabProducts);
            BuildInvoicesTab(tabInvoices);
            BuildTaxDashboardTab(tabTaxDashboard);
            BuildSalesDashboardTab(tabSalesDashboard);

            tabControl.TabPages.Add(tabCustomers);
            tabControl.TabPages.Add(tabProducts);
            tabControl.TabPages.Add(tabInvoices);
            tabControl.TabPages.Add(tabTaxDashboard);
            tabControl.TabPages.Add(tabSalesDashboard);

            Controls.Add(tabControl);
            Controls.Add(headerPanel);
        }

        private void BuildCustomersTab(TabPage tab)
        {
            GroupBox grpForm = new GroupBox
            {
                Text = "Customer Management",
                Location = new Point(16, 16),
                Size = new Size(320, 260)
            };

            Label lblName = new Label { Text = "Customer Name", Location = new Point(18, 34), AutoSize = true };
            txtCustomerName.SetBounds(18, 58, 270, 28);

            Label lblPhone = new Label { Text = "Phone", Location = new Point(18, 102), AutoSize = true };
            txtCustomerPhone.SetBounds(18, 126, 270, 28);

            btnAddCustomer.Text = "Add Customer";
            btnAddCustomer.SetBounds(18, 180, 128, 34);

            btnDeleteCustomer.Text = "Delete Selected";
            btnDeleteCustomer.SetBounds(160, 180, 128, 34);

            grpForm.Controls.Add(lblName);
            grpForm.Controls.Add(txtCustomerName);
            grpForm.Controls.Add(lblPhone);
            grpForm.Controls.Add(txtCustomerPhone);
            grpForm.Controls.Add(btnAddCustomer);
            grpForm.Controls.Add(btnDeleteCustomer);

            GroupBox grpList = new GroupBox
            {
                Text = "Customers List",
                Location = new Point(356, 16),
                Size = new Size(980, 700)
            };

            ConfigureGrid(dgvCustomers);
            dgvCustomers.Dock = DockStyle.Fill;
            grpList.Controls.Add(dgvCustomers);

            tab.Controls.Add(grpForm);
            tab.Controls.Add(grpList);
        }

        private void BuildProductsTab(TabPage tab)
        {
            GroupBox grpForm = new GroupBox
            {
                Text = "Product Management",
                Location = new Point(16, 16),
                Size = new Size(320, 320)
            };

            Label lblName = new Label { Text = "Product Name", Location = new Point(18, 34), AutoSize = true };
            txtProductName.SetBounds(18, 58, 270, 28);

            Label lblPrice = new Label { Text = "Unit Price HT", Location = new Point(18, 102), AutoSize = true };
            txtProductPrice.SetBounds(18, 126, 270, 28);

            Label lblVat = new Label { Text = "VAT Rate (%)", Location = new Point(18, 170), AutoSize = true };
            txtProductVat.SetBounds(18, 194, 270, 28);

            btnAddProduct.Text = "Add Product";
            btnAddProduct.SetBounds(18, 248, 128, 34);

            btnDeleteProduct.Text = "Delete Selected";
            btnDeleteProduct.SetBounds(160, 248, 128, 34);

            grpForm.Controls.Add(lblName);
            grpForm.Controls.Add(txtProductName);
            grpForm.Controls.Add(lblPrice);
            grpForm.Controls.Add(txtProductPrice);
            grpForm.Controls.Add(lblVat);
            grpForm.Controls.Add(txtProductVat);
            grpForm.Controls.Add(btnAddProduct);
            grpForm.Controls.Add(btnDeleteProduct);

            GroupBox grpList = new GroupBox
            {
                Text = "Products List",
                Location = new Point(356, 16),
                Size = new Size(980, 700)
            };

            ConfigureGrid(dgvProducts);
            dgvProducts.Dock = DockStyle.Fill;
            grpList.Controls.Add(dgvProducts);

            tab.Controls.Add(grpForm);
            tab.Controls.Add(grpList);
        }

        private void BuildInvoicesTab(TabPage tab)
        {
            GroupBox grpCreate = new GroupBox
            {
                Text = "Create Invoice",
                Location = new Point(16, 16),
                Size = new Size(380, 300)
            };

            Label lblCustomer = new Label { Text = "Customer", Location = new Point(18, 34), AutoSize = true };
            cmbInvoiceCustomer.SetBounds(18, 58, 320, 28);
            cmbInvoiceCustomer.DropDownStyle = ComboBoxStyle.DropDownList;

            Label lblProduct = new Label { Text = "Product", Location = new Point(18, 102), AutoSize = true };
            cmbInvoiceProduct.SetBounds(18, 126, 320, 28);
            cmbInvoiceProduct.DropDownStyle = ComboBoxStyle.DropDownList;

            Label lblQuantity = new Label { Text = "Quantity", Location = new Point(18, 170), AutoSize = true };
            nudQuantity.SetBounds(18, 194, 100, 28);
            nudQuantity.Minimum = 1;
            nudQuantity.Maximum = 1000;
            nudQuantity.Value = 1;

            Label lblTaxStamp = new Label { Text = "Tax Stamp", Location = new Point(140, 170), AutoSize = true };
            nudTaxStamp.SetBounds(140, 194, 100, 28);
            nudTaxStamp.DecimalPlaces = 3;
            nudTaxStamp.Minimum = 0;
            nudTaxStamp.Maximum = 1000;

            btnAddDraftItem.Text = "Add Draft Item";
            btnAddDraftItem.SetBounds(18, 244, 140, 34);

            btnCreateInvoice.Text = "Create Invoice";
            btnCreateInvoice.SetBounds(172, 244, 140, 34);

            grpCreate.Controls.Add(lblCustomer);
            grpCreate.Controls.Add(cmbInvoiceCustomer);
            grpCreate.Controls.Add(lblProduct);
            grpCreate.Controls.Add(cmbInvoiceProduct);
            grpCreate.Controls.Add(lblQuantity);
            grpCreate.Controls.Add(nudQuantity);
            grpCreate.Controls.Add(lblTaxStamp);
            grpCreate.Controls.Add(nudTaxStamp);
            grpCreate.Controls.Add(btnAddDraftItem);
            grpCreate.Controls.Add(btnCreateInvoice);

            GroupBox grpDraft = new GroupBox
            {
                Text = "Draft Items",
                Location = new Point(16, 332),
                Size = new Size(560, 384)
            };

            ConfigureGrid(dgvDraftItems);
            dgvDraftItems.SetBounds(16, 30, 528, 290);

            btnRemoveDraftItem.Text = "Remove Selected Draft Item";
            btnRemoveDraftItem.SetBounds(16, 332, 220, 34);

            grpDraft.Controls.Add(dgvDraftItems);
            grpDraft.Controls.Add(btnRemoveDraftItem);

            GroupBox grpInvoices = new GroupBox
            {
                Text = "Invoices History",
                Location = new Point(598, 16),
                Size = new Size(738, 700)
            };

            Label lblSearch = new Label { Text = "Search by customer", Location = new Point(16, 34), AutoSize = true };
            txtSearchInvoiceCustomer.SetBounds(16, 58, 220, 28);

            btnSearchInvoices.Text = "Search";
            btnSearchInvoices.SetBounds(248, 56, 90, 30);

            btnShowAllInvoices.Text = "Show All";
            btnShowAllInvoices.SetBounds(348, 56, 90, 30);

            ConfigureGrid(dgvInvoices);
            dgvInvoices.SetBounds(16, 100, 706, 300);

            Label lblDetails = new Label { Text = "Selected Invoice Details", Location = new Point(16, 420), AutoSize = true };
            ConfigureGrid(dgvInvoiceDetails);
            dgvInvoiceDetails.SetBounds(16, 446, 706, 230);

            grpInvoices.Controls.Add(lblSearch);
            grpInvoices.Controls.Add(txtSearchInvoiceCustomer);
            grpInvoices.Controls.Add(btnSearchInvoices);
            grpInvoices.Controls.Add(btnShowAllInvoices);
            grpInvoices.Controls.Add(dgvInvoices);
            grpInvoices.Controls.Add(lblDetails);
            grpInvoices.Controls.Add(dgvInvoiceDetails);

            tab.Controls.Add(grpCreate);
            tab.Controls.Add(grpDraft);
            tab.Controls.Add(grpInvoices);
        }

        private void BuildTaxDashboardTab(TabPage tab)
        {
            GroupBox grpSummary = new GroupBox
            {
                Text = "Tax Summary",
                Location = new Point(16, 16),
                Size = new Size(1320, 120)
            };

            lblTotalVat.Text = "Total VAT Collected: 0.000";
            lblTotalVat.Location = new Point(24, 40);
            lblTotalVat.Size = new Size(360, 30);
            lblTotalVat.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            lblTotalStamp.Text = "Total Tax Stamp: 0.000";
            lblTotalStamp.Location = new Point(420, 40);
            lblTotalStamp.Size = new Size(300, 30);
            lblTotalStamp.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            btnRefreshTax.Text = "Refresh Tax Dashboard";
            btnRefreshTax.SetBounds(1060, 34, 220, 34);

            grpSummary.Controls.Add(lblTotalVat);
            grpSummary.Controls.Add(lblTotalStamp);
            grpSummary.Controls.Add(btnRefreshTax);

            GroupBox grpVatDetails = new GroupBox
            {
                Text = "VAT by Rate",
                Location = new Point(16, 152),
                Size = new Size(1320, 564)
            };

            ConfigureGrid(dgvVatByRate);
            dgvVatByRate.Dock = DockStyle.Fill;
            grpVatDetails.Controls.Add(dgvVatByRate);

            tab.Controls.Add(grpSummary);
            tab.Controls.Add(grpVatDetails);
        }

        private void BuildSalesDashboardTab(TabPage tab)
        {
            GroupBox grpSummary = new GroupBox
            {
                Text = "Sales Summary",
                Location = new Point(16, 16),
                Size = new Size(1320, 120)
            };

            lblRevenueHT.Text = "Total Revenue HT: 0.000";
            lblRevenueHT.Location = new Point(24, 40);
            lblRevenueHT.Size = new Size(320, 30);
            lblRevenueHT.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            lblRevenueTTC.Text = "Total Revenue TTC: 0.000";
            lblRevenueTTC.Location = new Point(380, 40);
            lblRevenueTTC.Size = new Size(320, 30);
            lblRevenueTTC.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            btnRefreshSales.Text = "Refresh Sales Dashboard";
            btnRefreshSales.SetBounds(1040, 34, 240, 34);

            grpSummary.Controls.Add(lblRevenueHT);
            grpSummary.Controls.Add(lblRevenueTTC);
            grpSummary.Controls.Add(btnRefreshSales);

            GroupBox grpMonth = new GroupBox
            {
                Text = "Revenue by Month",
                Location = new Point(16, 152),
                Size = new Size(430, 564)
            };
            ConfigureGrid(dgvRevenueByMonth);
            dgvRevenueByMonth.Dock = DockStyle.Fill;
            grpMonth.Controls.Add(dgvRevenueByMonth);

            GroupBox grpCustomer = new GroupBox
            {
                Text = "Revenue by Customer",
                Location = new Point(462, 152),
                Size = new Size(430, 564)
            };
            ConfigureGrid(dgvRevenueByCustomer);
            dgvRevenueByCustomer.Dock = DockStyle.Fill;
            grpCustomer.Controls.Add(dgvRevenueByCustomer);

            GroupBox grpProduct = new GroupBox
            {
                Text = "Revenue by Product",
                Location = new Point(908, 152),
                Size = new Size(428, 564)
            };
            ConfigureGrid(dgvRevenueByProduct);
            dgvRevenueByProduct.Dock = DockStyle.Fill;
            grpProduct.Controls.Add(dgvRevenueByProduct);

            tab.Controls.Add(grpSummary);
            tab.Controls.Add(grpMonth);
            tab.Controls.Add(grpCustomer);
            tab.Controls.Add(grpProduct);
        }

        private void WireEvents()
        {
            btnSave.Click += (s, e) => SaveData(true);
            btnLoad.Click += (s, e) => LoadData(true);
            btnResetDemo.Click += (s, e) => ResetDemoData();

            btnAddCustomer.Click += (s, e) => AddCustomer();
            btnDeleteCustomer.Click += (s, e) => DeleteCustomer();

            btnAddProduct.Click += (s, e) => AddProduct();
            btnDeleteProduct.Click += (s, e) => DeleteProduct();

            btnAddDraftItem.Click += (s, e) => AddDraftItem();
            btnRemoveDraftItem.Click += (s, e) => RemoveDraftItem();
            btnCreateInvoice.Click += (s, e) => CreateInvoice();
            btnSearchInvoices.Click += (s, e) => SearchInvoicesByCustomer();
            btnShowAllInvoices.Click += (s, e) => BindInvoicesGrid(data.Invoices);
            dgvInvoices.SelectionChanged += (s, e) => RefreshInvoiceDetails();

            btnRefreshTax.Click += (s, e) => RefreshTaxDashboard();
            btnRefreshSales.Click += (s, e) => RefreshSalesDashboard();
        }

        private void ConfigureGrid(DataGridView grid)
        {
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.ColumnHeadersHeight = 34;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 85, 155);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 230, 245);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        private void LoadOrSeed()
        {
            data = DataStore.Load(dataFilePath);

            if (data.Customers.Count == 0 && data.Products.Count == 0 && data.Invoices.Count == 0)
            {
                SeedData();
                SaveData(false);
            }
        }

        private void SeedData()
        {
            data.Customers.Add(new Customer { Id = data.NextCustomerId++, Name = "Yasin Mokni", Phone = "20123456" });
            data.Customers.Add(new Customer { Id = data.NextCustomerId++, Name = "Ali Ben Salah", Phone = "22111222" });

            data.Products.Add(new Product { Id = data.NextProductId++, Name = "Laptop", UnitPriceHT = 2500m, VatRate = 19m });
            data.Products.Add(new Product { Id = data.NextProductId++, Name = "Notebook Pack", UnitPriceHT = 12m, VatRate = 7m });
            data.Products.Add(new Product { Id = data.NextProductId++, Name = "Printer", UnitPriceHT = 480m, VatRate = 13m });
        }

        private void ResetDemoData()
        {
            DialogResult result = MessageBox.Show(
                "This will remove current data and recreate demo data. Continue?",
                "Reset Demo Data",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            data = new AppData();
            draftItems.Clear();
            SeedData();
            SaveData(false);
            RefreshAll();
            MessageBox.Show("Demo data reset successfully.");
        }

        private void RefreshAll()
        {
            RefreshCustomersGrid();
            RefreshProductsGrid();
            RefreshInvoiceCombos();
            RefreshDraftGrid();
            BindInvoicesGrid(data.Invoices);
            RefreshInvoiceDetails();
            RefreshTaxDashboard();
            RefreshSalesDashboard();
        }

        private void RefreshCustomersGrid()
        {
            dgvCustomers.DataSource = data.Customers
                .OrderBy(c => c.Id)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Phone
                })
                .ToList();
        }

        private void RefreshProductsGrid()
        {
            dgvProducts.DataSource = data.Products
                .OrderBy(p => p.Id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    UnitPriceHT = p.UnitPriceHT.ToString("F3"),
                    VatRate = p.VatRate.ToString("F0") + "%"
                })
                .ToList();
        }

        private void RefreshInvoiceCombos()
        {
            cmbInvoiceCustomer.DataSource = null;
            cmbInvoiceCustomer.DisplayMember = nameof(Customer.Name);
            cmbInvoiceCustomer.ValueMember = nameof(Customer.Id);
            cmbInvoiceCustomer.DataSource = data.Customers.OrderBy(c => c.Id).ToList();

            cmbInvoiceProduct.DataSource = null;
            cmbInvoiceProduct.DisplayMember = nameof(Product.Name);
            cmbInvoiceProduct.ValueMember = nameof(Product.Id);
            cmbInvoiceProduct.DataSource = data.Products.OrderBy(p => p.Id).ToList();
        }

        private void RefreshDraftGrid()
        {
            dgvDraftItems.DataSource = draftItems
                .Select((item, index) => new
                {
                    Index = index,
                    Product = item.Product.Name,
                    Quantity = item.Quantity,
                    HT = item.LineTotalHT.ToString("F3"),
                    VAT = item.LineVAT.ToString("F3"),
                    TTC = item.LineTotalTTC.ToString("F3")
                })
                .ToList();

            if (dgvDraftItems.Columns.Contains("Index"))
            {
                dgvDraftItems.Columns["Index"].Visible = false;
            }
        }

        private void BindInvoicesGrid(IEnumerable<Invoice> invoicesToBind)
        {
            dgvInvoices.DataSource = invoicesToBind
                .OrderByDescending(i => i.Date)
                .Select(i => new
                {
                    i.Id,
                    Date = i.Date.ToString("yyyy-MM-dd HH:mm"),
                    Customer = i.Customer.Name,
                    Items = i.Items.Count,
                    TotalHT = i.TotalHT.ToString("F3"),
                    TotalVAT = i.TotalVAT.ToString("F3"),
                    TaxStamp = i.TaxStamp.ToString("F3"),
                    TotalTTC = i.TotalTTC.ToString("F3")
                })
                .ToList();
        }

        private void RefreshInvoiceDetails()
        {
            int invoiceId;
            if (!TryGetSelectedId(dgvInvoices, out invoiceId))
            {
                dgvInvoiceDetails.DataSource = null;
                return;
            }

            Invoice? invoice = data.Invoices.FirstOrDefault(i => i.Id == invoiceId);
            if (invoice == null)
            {
                dgvInvoiceDetails.DataSource = null;
                return;
            }

            dgvInvoiceDetails.DataSource = invoice.Items
                .Select(item => new
                {
                    Product = item.Product.Name,
                    Quantity = item.Quantity,
                    UnitPriceHT = item.Product.UnitPriceHT.ToString("F3"),
                    VatRate = item.Product.VatRate.ToString("F0") + "%",
                    TotalHT = item.LineTotalHT.ToString("F3"),
                    VAT = item.LineVAT.ToString("F3"),
                    TTC = item.LineTotalTTC.ToString("F3")
                })
                .ToList();
        }

        private void RefreshTaxDashboard()
        {
            decimal totalVat = data.Invoices.Sum(i => i.TotalVAT);
            decimal totalStamp = data.Invoices.Sum(i => i.TaxStamp);

            lblTotalVat.Text = $"Total VAT Collected: {totalVat:F3}";
            lblTotalStamp.Text = $"Total Tax Stamp: {totalStamp:F3}";

            dgvVatByRate.DataSource = data.Invoices
                .SelectMany(i => i.Items)
                .GroupBy(item => item.Product.VatRate)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    VatRate = g.Key.ToString("F0") + "%",
                    ItemsCount = g.Count(),
                    TotalVat = g.Sum(x => x.LineVAT).ToString("F3"),
                    TotalHT = g.Sum(x => x.LineTotalHT).ToString("F3"),
                    TotalTTC = g.Sum(x => x.LineTotalTTC).ToString("F3")
                })
                .ToList();
        }

        private void RefreshSalesDashboard()
        {
            decimal totalRevenueHT = data.Invoices.Sum(i => i.TotalHT);
            decimal totalRevenueTTC = data.Invoices.Sum(i => i.TotalTTC);

            lblRevenueHT.Text = $"Total Revenue HT: {totalRevenueHT:F3}";
            lblRevenueTTC.Text = $"Total Revenue TTC: {totalRevenueTTC:F3}";

            dgvRevenueByMonth.DataSource = data.Invoices
                .GroupBy(i => i.Date.ToString("yyyy-MM"))
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Month = g.Key,
                    InvoiceCount = g.Count(),
                    RevenueHT = g.Sum(x => x.TotalHT).ToString("F3"),
                    RevenueTTC = g.Sum(x => x.TotalTTC).ToString("F3")
                })
                .ToList();

            dgvRevenueByCustomer.DataSource = data.Invoices
                .GroupBy(i => i.Customer.Name)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Customer = g.Key,
                    InvoiceCount = g.Count(),
                    RevenueHT = g.Sum(x => x.TotalHT).ToString("F3"),
                    RevenueTTC = g.Sum(x => x.TotalTTC).ToString("F3")
                })
                .ToList();

            dgvRevenueByProduct.DataSource = data.Invoices
                .SelectMany(i => i.Items)
                .GroupBy(item => item.Product.Name)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Product = g.Key,
                    Quantity = g.Sum(x => x.Quantity),
                    RevenueHT = g.Sum(x => x.LineTotalHT).ToString("F3"),
                    RevenueTTC = g.Sum(x => x.LineTotalTTC).ToString("F3")
                })
                .ToList();
        }

        private void AddCustomer()
        {
            string name = txtCustomerName.Text.Trim();
            string phone = txtCustomerPhone.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Enter customer name and phone.");
                return;
            }

            data.Customers.Add(new Customer
            {
                Id = data.NextCustomerId++,
                Name = name,
                Phone = phone
            });

            txtCustomerName.Clear();
            txtCustomerPhone.Clear();
            SaveData(false);
            RefreshAll();
            MessageBox.Show("Customer added successfully.");
        }

        private void DeleteCustomer()
        {
            int customerId;
            if (!TryGetSelectedId(dgvCustomers, out customerId))
            {
                MessageBox.Show("Select a customer to delete.");
                return;
            }

            Customer? customer = data.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
            {
                MessageBox.Show("Customer not found.");
                return;
            }

            data.Customers.Remove(customer);
            SaveData(false);
            RefreshAll();
            MessageBox.Show("Customer deleted. Historical invoices remain saved.");
        }

        private void AddProduct()
        {
            string name = txtProductName.Text.Trim();
            decimal price;
            decimal vat;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Enter product name.");
                return;
            }

            if (!TryParseDecimal(txtProductPrice.Text, out price))
            {
                MessageBox.Show("Invalid unit price HT.");
                return;
            }

            if (!TryParseDecimal(txtProductVat.Text, out vat))
            {
                MessageBox.Show("Invalid VAT rate.");
                return;
            }

            data.Products.Add(new Product
            {
                Id = data.NextProductId++,
                Name = name,
                UnitPriceHT = price,
                VatRate = vat
            });

            txtProductName.Clear();
            txtProductPrice.Clear();
            txtProductVat.Clear();
            SaveData(false);
            RefreshAll();
            MessageBox.Show("Product added successfully.");
        }

        private void DeleteProduct()
        {
            int productId;
            if (!TryGetSelectedId(dgvProducts, out productId))
            {
                MessageBox.Show("Select a product to delete.");
                return;
            }

            Product? product = data.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                MessageBox.Show("Product not found.");
                return;
            }

            data.Products.Remove(product);
            SaveData(false);
            RefreshAll();
            MessageBox.Show("Product deleted. Historical invoices remain saved.");
        }

        private void AddDraftItem()
        {
            Customer? customer = cmbInvoiceCustomer.SelectedItem as Customer;
            Product? product = cmbInvoiceProduct.SelectedItem as Product;

            if (customer == null)
            {
                MessageBox.Show("Select a customer first.");
                return;
            }

            if (product == null)
            {
                MessageBox.Show("Select a product first.");
                return;
            }

            draftItems.Add(new InvoiceItem
            {
                Product = new Product
                {
                    Id = product.Id,
                    Name = product.Name,
                    UnitPriceHT = product.UnitPriceHT,
                    VatRate = product.VatRate
                },
                Quantity = (int)nudQuantity.Value
            });

            RefreshDraftGrid();
        }

        private void RemoveDraftItem()
        {
            if (dgvDraftItems.CurrentRow == null || dgvDraftItems.CurrentRow.Cells["Index"].Value == null)
            {
                MessageBox.Show("Select a draft item to remove.");
                return;
            }

            int index = Convert.ToInt32(dgvDraftItems.CurrentRow.Cells["Index"].Value);
            if (index < 0 || index >= draftItems.Count)
            {
                MessageBox.Show("Invalid draft item selection.");
                return;
            }

            draftItems.RemoveAt(index);
            RefreshDraftGrid();
        }

        private void CreateInvoice()
        {
            Customer? customer = cmbInvoiceCustomer.SelectedItem as Customer;
            if (customer == null)
            {
                MessageBox.Show("Select a customer.");
                return;
            }

            if (draftItems.Count == 0)
            {
                MessageBox.Show("Add at least one draft item.");
                return;
            }

            Invoice invoice = new Invoice
            {
                Id = data.NextInvoiceId++,
                Customer = new Customer
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Phone = customer.Phone
                },
                Date = DateTime.Now,
                TaxStamp = nudTaxStamp.Value,
                Items = draftItems
                    .Select(item => new InvoiceItem
                    {
                        Product = new Product
                        {
                            Id = item.Product.Id,
                            Name = item.Product.Name,
                            UnitPriceHT = item.Product.UnitPriceHT,
                            VatRate = item.Product.VatRate
                        },
                        Quantity = item.Quantity
                    })
                    .ToList()
            };

            data.Invoices.Add(invoice);
            draftItems.Clear();
            nudTaxStamp.Value = 0;
            SaveData(false);
            RefreshAll();
            MessageBox.Show($"Invoice created successfully. Total TTC = {invoice.TotalTTC:F3}");
        }

        private void SearchInvoicesByCustomer()
        {
            string keyword = txtSearchInvoiceCustomer.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                BindInvoicesGrid(data.Invoices);
                return;
            }

            IEnumerable<Invoice> filtered = data.Invoices
                .Where(i => i.Customer.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            BindInvoicesGrid(filtered);
        }

        private void SaveData(bool showMessage)
        {
            DataStore.Save(dataFilePath, data);

            if (showMessage)
            {
                MessageBox.Show("Data saved successfully.");
            }
        }

        private void LoadData(bool showMessage)
        {
            data = DataStore.Load(dataFilePath);
            draftItems.Clear();
            RefreshAll();

            if (showMessage)
            {
                MessageBox.Show("Data loaded successfully.");
            }
        }

        private bool TryParseDecimal(string text, out decimal value)
        {
            string normalized = (text ?? string.Empty).Trim().Replace(',', '.');
            return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        private bool TryGetSelectedId(DataGridView grid, out int id)
        {
            id = 0;

            if (grid.CurrentRow == null || !grid.Columns.Contains("Id"))
            {
                return false;
            }

            object? value = grid.CurrentRow.Cells["Id"].Value;
            if (value == null)
            {
                return false;
            }

            return int.TryParse(value.ToString(), out id);
        }
    }
}
