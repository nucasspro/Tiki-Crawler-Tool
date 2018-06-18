using Crawler_WF.Model;
using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Crawler_WF.UserControl
{
    public partial class TiKi_UC : System.Windows.Forms.UserControl
    {
        private List<Product> _productList = new List<Product>();

        private List<Link> _linkList = new List<Link>();

        public TiKi_UC()
        {
            InitializeComponent();
        }

        #region Event

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (!CheckFormat())
            {
                return;
            }

            ChromeDriver driver = new ChromeDriver();

            try
            {
                NavigateDriverToNewPage(driver);
                driver.Quit();
                txbNumberOfProduct.Text = _productList.Count + @"sản phẩm";

                foreach (var item in _productList)
                {
                    txbLink.Text += @"***********************************" + Environment.NewLine +
                                    item.Id + Environment.NewLine +
                                    item.Name + Environment.NewLine +
                                    item.Sku + Environment.NewLine +
                                    item.SalePrice + Environment.NewLine +
                                    item.RegularPrice + Environment.NewLine +
                                    item.Category + Environment.NewLine +
                                    item.Brand + Environment.NewLine + Environment.NewLine +
                                    "ShortInfo" + Environment.NewLine + Environment.NewLine + item.ShortInfo + Environment.NewLine + Environment.NewLine +
                                    "LongInfo" + Environment.NewLine + Environment.NewLine +
                                    item.LongInfo + Environment.NewLine + Environment.NewLine +
                                    "Description" + Environment.NewLine + Environment.NewLine +
                                    item.Description + Environment.NewLine + Environment.NewLine + Environment.NewLine +
                                    item.Image + Environment.NewLine +
                                    item.Link + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                txbLink.Text = @"Error click: " + ex;

                throw;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportTemplate();
        }

        #endregion Event

        #region Method

        private bool CheckFormat()
        {
            if (txbInputLink.Text != "" && txbPage.Text != "" && txbCategory.Text != "")
                return true;

            return false;
        }

        private void NavigateDriverToNewPage(ChromeDriver driver)
        {
            int number = Int32.Parse(txbPage.Text);
            try
            {
                for (int i = 1; i <= number; i++)
                {
                    var url = txbInputLink.Text.Trim() + @"&page=" + i;
                    driver.Url = url;
                    driver.Navigate();

                    _linkList = GetAllProductLinkOfPage(driver);
                    foreach (var link in _linkList)
                    {
                        txbHtml.Text += Environment.NewLine + link.ProductLink;
                        var product = GetInfoOfProduct(driver, link);
                        _productList.Add(product);
                    }
                }
            }
            catch (Exception e)
            {
                txbLink.Text = @"Error NavigateDriverToNewPage: " + e;

                throw;
            }
        }

        private List<Link> GetAllProductLinkOfPage(ChromeDriver driver)
        {
            var source = driver.PageSource;
            List<Link> links = new List<Link>();
            var productlist = Regex.Matches(source, @"data-title=(.*?)"" alt", RegexOptions.Singleline);

            foreach (var item in productlist)
            {
                string link = Regex.Match(item.ToString(), @"href=""(.*?)""", RegexOptions.Singleline).Value.Replace("href=", "").Replace("\"", "");
                links.Add(new Link(link));
            }
            return links;
        }

        private Product GetInfoOfProduct(ChromeDriver driver, Link link)
        {
            driver.Url = link.ProductLink;
            driver.Navigate();
            var html = driver.PageSource;

            try
            {
                string productId = Regex.Match(html, @"id=""product_id""(.*?)>", RegexOptions.Singleline).Value.Replace("id=\"product_id\" name=\"id\" type=\"hidden\" value=\"", "").Replace("\"", "").Replace("/>", "").Trim();
                string productName = Regex.Match(html, @"id=""product-name""(.*?)<", RegexOptions.Singleline).Value.Replace("id=\"product-name\">", "").Replace("<", "").Trim();
                string productSku = Regex.Match(html, @"id=""product_sku""(.*?)>", RegexOptions.Singleline).Value.Replace("id=\"product_sku\" name=\"sku\" type=\"hidden\" value=\"", "").Replace("\"", "").Replace("/>", "").Trim();
                txbHtml.Text = productId + Environment.NewLine + productSku;
                string productSalePrice;
                try
                {
                    productSalePrice = Regex.Match(html, @"class=""old-price-item""(.*?)>", RegexOptions.Singleline).Value.Replace("class=\"old-price-item\" data-value=\"", "").Replace("id=\"p-listpirce\">", "").Replace("\"", "").Trim();
                }
                catch (Exception)
                {
                    productSalePrice = "";
                }

                string productRegularPrice = Regex.Match(html, @"id=""product_price""(.*?)>", RegexOptions.Singleline).Value.Replace("id=\"product_price\" name=\"price\" type=\"hidden\" value=\"", "").Replace("\"", "").Replace("/>", "").Trim();
                string productCategory = Regex.Match(html, @"id=""productset_name""(.*?)>", RegexOptions.Singleline).Value.Replace("id=\"productset_name\" name=\"category\" type=\"hidden\" value=\"", "").Replace("\"", "").Replace("/>", "").Trim();

                IWebElement brandElement = driver.FindElementByCssSelector("div.item-brand:nth-child(2) > p:nth-child(2) > a:nth-child(1)");
                var productBrand = brandElement.Text;

                string productShortInfo;
                try
                {
                    IWebElement shortInfoElement = driver.FindElementByCssSelector(".top-feature-item");
                    productShortInfo = shortInfoElement.Text;
                }
                catch (Exception)
                {
                    productShortInfo = "";
                }

                string productLongInfo;
                try
                {
                    IWebElement longInfoElement = driver.FindElementByCssSelector("div.white-panel:nth-child(2)");
                    productLongInfo = longInfoElement.GetAttribute("innerHTML");
                }
                catch (Exception)
                {
                    productLongInfo = "";
                }

                string productDescription;
                try
                {
                    IWebElement descriptionElement = driver.FindElementByCssSelector("div.white-panel:nth-child(4)");
                    string path = @"src=""data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==""";
                    string readMode = @"<p class=""show-more""><a class=""js-show-more"" href=""#"" title=""Xem Thêm Nội Dung"">Xem Thêm Nội Dung</a></p>";
                    productDescription = descriptionElement.GetAttribute("innerHTML").Replace(path, "").Replace("data-src", "src").Replace(readMode, "");
                    txbHtml.Text = productDescription;
                }
                catch (Exception)
                {
                    productDescription = "";
                }

                string productImage = Regex.Match(html, @"data-zoom-image=""(.*?)""", RegexOptions.Singleline).Value.Replace("data-zoom-image=", "").Replace("\"", "").Trim();
                string productLink = link.ProductLink;
                Product product = new Product(productId, productName, productSku, productSalePrice, productRegularPrice, productCategory, productBrand, productShortInfo, productLongInfo, productDescription, productImage, productLink);
                return product;
            }
            catch (Exception e)
            {
                txbLink.Text = @"Error GetInfoOfProduct: " + e;
                throw;
            }
        }

        private void ExportTemplate()
        {
            int type = dropboxProductType.selectedIndex;
            try
            {
                using (ExcelPackage p = new ExcelPackage())
                {
                    p.Workbook.Worksheets.Add("Data");
                    ExcelWorksheet worksheet = p.Workbook.Worksheets[1];
                    worksheet.Name = "Data";

                    string[] arrheader = {
                        "ID", "Type", "SKU", "Name", "Published", "Is featured ?", "Visibility in catalog",
                        "Short description", "Description", "Date sale price starts", "Date sale price ends",
                        "Tax status", "Tax class", "In stock?", "Stock", "Backorders allowed?", "Sold individually?",
                        "Weight(kg)", "Length(cm)", "Width(cm)", "Height(cm)", "Allow customer reviews?",
                        "Purchase note", "Sale price", "Regular price", "Categories", "Tags", "Shipping class",
                        "Images", "Download limit", "Download expiry days", "Parent", "Grouped products", "Upsells",
                        "Cross-sells", "External URL", "Button text", "Position"
                    };

                    //Header
                    for (int i = 1; i <= 38; i++)
                    {
                        worksheet.Cells[1, i].Value = arrheader[i - 1];
                    }
                    int j = 2;
                    //Data
                    switch (type)
                    {
                        case 0:
                            {
                                foreach (var item in _productList)
                                {
                                    worksheet.Cells[j, 1].Value = item.Id;
                                    worksheet.Cells[j, 2].Value = "simple";
                                    worksheet.Cells[j, 3].Value = item.Sku;
                                    worksheet.Cells[j, 4].Value = item.Name;
                                    worksheet.Cells[j, 5].Value = 1;
                                    worksheet.Cells[j, 6].Value = 0;
                                    worksheet.Cells[j, 7].Value = "visible";
                                    worksheet.Cells[j, 8].Value = item.ShortInfo;
                                    worksheet.Cells[j, 9].Value = item.LongInfo + "\n\n\n\n" + item.Description;
                                    worksheet.Cells[j, 12].Value = "taxable";
                                    worksheet.Cells[j, 14].Value = 1;
                                    worksheet.Cells[j, 16].Value = 0;
                                    worksheet.Cells[j, 17].Value = 0;
                                    worksheet.Cells[j, 22].Value = 0;
                                    worksheet.Cells[j, 25].Value = item.RegularPrice;
                                    worksheet.Cells[j, 26].Value = item.Category;
                                    worksheet.Cells[j, 29].Value = item.Image;
                                    worksheet.Cells[j, 38].Value = 0;
                                    j++;
                                }
                                break;
                            }
                        case 1:
                            {
                                //foreach (var item in _productList)
                                //{
                                //    worksheet.Cells[j, 1].Value = item.Id;
                                //    worksheet.Cells[j, 2].Value = "simple";
                                //    worksheet.Cells[j, 3].Value = item.Sku;
                                //    worksheet.Cells[j, 4].Value = item.Name;
                                //    worksheet.Cells[j, 5].Value = 1;
                                //    worksheet.Cells[j, 6].Value = 0;
                                //    worksheet.Cells[j, 7].Value = "visible";
                                //    worksheet.Cells[j, 8].Value = item.ShortInfo;
                                //    worksheet.Cells[j, 9].Value = item.LongInfo + "\n\n\n\n" + item.Description;
                                //    worksheet.Cells[j, 12].Value = "taxable";
                                //    worksheet.Cells[j, 14].Value = 1;
                                //    worksheet.Cells[j, 16].Value = 0;
                                //    worksheet.Cells[j, 17].Value = 0;
                                //    worksheet.Cells[j, 22].Value = 0;
                                //    worksheet.Cells[j, 25].Value = item.Price;
                                //    worksheet.Cells[j, 26].Value = item.Category;
                                //    worksheet.Cells[j, 29].Value = item.Image;
                                //    worksheet.Cells[j, 38].Value = 0;
                                //    j++;
                                //}
                                break;
                            }
                        case 2:
                            {
                                foreach (var item in _productList)
                                {
                                    worksheet.Cells[j, 1].Value = item.Id;
                                    worksheet.Cells[j, 2].Value = "external";
                                    worksheet.Cells[j, 3].Value = item.Sku;
                                    worksheet.Cells[j, 4].Value = item.Name;
                                    worksheet.Cells[j, 5].Value = 1;
                                    worksheet.Cells[j, 6].Value = 0;
                                    worksheet.Cells[j, 7].Value = "visible";
                                    worksheet.Cells[j, 8].Value = item.ShortInfo;
                                    worksheet.Cells[j, 9].Value = item.LongInfo + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + item.Description;
                                    worksheet.Cells[j, 12].Value = "taxable";
                                    worksheet.Cells[j, 14].Value = 1;
                                    worksheet.Cells[j, 16].Value = 0;
                                    worksheet.Cells[j, 17].Value = 0;
                                    worksheet.Cells[j, 22].Value = 0;
                                    worksheet.Cells[j, 24].Value = item.RegularPrice;
                                    worksheet.Cells[j, 25].Value = item.SalePrice;
                                    worksheet.Cells[j, 26].Value = txbCategory.Text;
                                    worksheet.Cells[j, 27].Value = item.Brand;
                                    worksheet.Cells[j, 29].Value = item.Image;
                                    worksheet.Cells[j, 36].Value = item.Link;
                                    worksheet.Cells[j, 37].Value = "Nhận hàng trước, thanh toán sau!";
                                    worksheet.Cells[j, 38].Value = 0;
                                    j++;
                                }
                                break;
                            }
                        case 3:
                            {
                                //foreach (var item in _productList)
                                //{
                                //    worksheet.Cells[j, 1].Value = item.Id;
                                //    worksheet.Cells[j, 2].Value = "simple";
                                //    worksheet.Cells[j, 3].Value = item.Sku;
                                //    worksheet.Cells[j, 4].Value = item.Name;
                                //    worksheet.Cells[j, 5].Value = 1;
                                //    worksheet.Cells[j, 6].Value = 0;
                                //    worksheet.Cells[j, 7].Value = "visible";
                                //    worksheet.Cells[j, 8].Value = item.ShortInfo;
                                //    worksheet.Cells[j, 9].Value = item.LongInfo + "\n\n\n\n" + item.Description;
                                //    worksheet.Cells[j, 12].Value = "taxable";
                                //    worksheet.Cells[j, 14].Value = 1;
                                //    worksheet.Cells[j, 16].Value = 0;
                                //    worksheet.Cells[j, 17].Value = 0;
                                //    worksheet.Cells[j, 22].Value = 0;
                                //    worksheet.Cells[j, 25].Value = item.Price;
                                //    worksheet.Cells[j, 26].Value = item.Category;
                                //    worksheet.Cells[j, 29].Value = item.Image;
                                //    worksheet.Cells[j, 38].Value = 0;
                                //    j++;
                                //}
                                break;
                            }
                    }

                    var path = Directory.GetCurrentDirectory() + "\\data_" + DateTime.Now.ToFileTimeUtc() + "_" + dropboxProductType.selectedValue + ".xlsx";

                    Byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(path, bin);

                    txbLink.Text += @"Export thành công";
                }
            }
            catch (Exception e)
            {
                txbLink.Text = @"Error Export: " + e;

                throw;
            }
        }

        #endregion Method
    }
}