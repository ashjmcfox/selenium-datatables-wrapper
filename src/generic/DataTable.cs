//-----------------------------------------------------------------------
// <copyright file="DataTable.cs" company="Ashley Fox">
//     Copyright Ashley Fox.
// </copyright>
// <author>Ashley Fox</author>
// <summary>
//      Description :   A class to store methods to interact with the
//                      jQuery DataTables plugin, while executing
//                      Selenium tests. Generic version.
//      Create Date :   09 June 2015
// </summary>
//-----------------------------------------------------------------------
namespace Selenium.Wrappers
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;

    using NUnit.Framework;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;

    using Keys = OpenQA.Selenium.Keys;

    /// <summary>
    /// DataTable class definition.
    /// </summary>
    public class DataTable
    {
        /// <summary>
        /// Field to store the Selenium web driver.
        /// </summary>
        private IWebDriver Driver { get; set; }

        /// <summary>
        /// Field to store the Selenium selector for this widget.
        /// </summary>
        private By Selector { get; set; }

        /// <summary>
        /// The page sizes that this widget supports.
        /// </summary>
        public enum PageSizes
        {
            /// <summary>
            /// 10 results per page.
            /// </summary>
            Ten,
            /// <summary>
            /// 25 results per page.
            /// </summary>
            TwentyFive,
            /// <summary>
            /// 50 results per page.
            /// </summary>
            Fifty,
            /// <summary>
            /// 100 results per page.
            /// </summary>
            Hundred
        }

        /// <summary>
        /// The pagination options that this widget supports.
        /// </summary>
        public enum Pagination
        {
            /// <summary>
            /// The first page of results.
            /// </summary>
            First,
            /// <summary>
            /// The previous page of results.
            /// </summary>
            Previous,
            /// <summary>
            /// The next page of results.
            /// </summary>
            Next,
            /// <summary>
            /// The last page of results.
            /// </summary>
            Last
        }

        /// <summary>
        /// The sorting options that this widget supports.
        /// </summary>
        public enum Sort
        {
            /// <summary>
            /// Sort in ascending order.
            /// </summary>
            Ascending,
            /// <summary>
            /// Sort in descending order.
            /// </summary>
            Descending
        }

        /// <summary>
        /// Method to change the number of results displayed per page.
        /// </summary>
        /// <param name="pageSize">
        /// The new page size option.
        /// </param>
        public void ChangePageSize(PageSizes pageSize)
        {
            // Get the ID of the original <table> element.
            // This is overridden and replaced by the third party DataTables widget.
            var id = this.Driver.FindElement(this.Selector).GetAttribute("id");

            // Navigate from the original <table> to the DataTables wrapper of the page size control.
            // The HTML for this control is dynamically injected.
            var wrapper = this.Driver.FindElement(By.Id(string.Format("{0}_length", id)));

            // Get the <select> element which sets the page size.
            var element = new SelectElement(wrapper.FindElement(By.TagName("select")));

            // Select the new page size value.
            switch (pageSize)
            {
                case PageSizes.Ten:
                    element.SelectByText("10");
                    break;

                case PageSizes.TwentyFive:
                    element.SelectByText("25");
                    break;

                case PageSizes.Fifty:
                    element.SelectByText("50");
                    break;

                case PageSizes.Hundred:
                    element.SelectByText("100");
                    break;
            }
        }

        /// <summary>
        /// Method to change the current page of results using pagination.
        /// </summary>
        /// <param name="page">
        /// The new page to navigate to.
        /// </param>
        public void ChangePagination(Pagination page)
        {
            // Get the ID of the original <table> element.
            // This is overridden and replaced by the third party DataTables widget.
            var id = this.Driver.FindElement(this.Selector).GetAttribute("id");

            // Navigate from the original <table> to the DataTables wrapper of the pagination control.
            // The HTML for this control is dynamically injected.
            var wrapper = this.Driver.FindElement(By.Id(string.Format("{0}_paginate", id)));
            
            // Get the paging style of the table.
            var pagingType = wrapper.GetAttribute("class").Replace("dataTables_paginate", string.Empty).Trim();

            switch (pagingType)
            {
                case "paging_full":
                case "paging_full_numbers":
                    // Attempt to navigate using the DataTables default full pagination style.
                    switch (page)
                    {
                        case Pagination.First:
                            this.Driver.FindElement(By.Id(string.Format("{0}_first", id))).Click();
                            break;

                        case Pagination.Previous:
                            this.Driver.FindElement(By.Id(string.Format("{0}_previous", id))).Click();
                            break;

                        case Pagination.Next:
                            this.Driver.FindElement(By.Id(string.Format("{0}_next", id))).Click();
                            break;

                        case Pagination.Last:
                            this.Driver.FindElement(By.Id(string.Format("{0}_last", id))).Click();
                            break;
                    }
                    break;

                case "paging_bs_four_button":
                case "paging_bs_full":
                case "paging_bootstrap":
                    // Attempt to navigate using the DataTables Bootstrap pagination plugin.
                    switch (page)
                    {
                        case Pagination.First:
                            wrapper.FindElement(By.CssSelector(".first a")).Click();
                            break;

                        case Pagination.Previous:
                            wrapper.FindElement(By.CssSelector(".prev a")).Click();
                            break;

                        case Pagination.Next:
                            wrapper.FindElement(By.CssSelector(".next a")).Click();
                            break;

                        case Pagination.Last:
                            wrapper.FindElement(By.CssSelector(".last a")).Click();
                            break;
                    }
                    break;

                default:
                    // Table is likely using an alternative pagination style which is unsupported. Throw an assert failure to report this.
                    Assert.IsTrue(false, "The table is using an alternative pagination style which is unsupported.");
                    break;
            }
        }

        /// <summary>
        /// Method to change the sorting of a column which contains the text supplied.
        /// </summary>
        /// <param name="column">
        /// The name of the column to sort.
        /// </param>
        /// <param name="direction">
        /// The direction to sort this column.
        /// </param>
        public void SortColumn(string column, Sort direction)
        {
            // Get the <table> element for this widget.
            var wrapper = this.Driver.FindElement(this.Selector);

            // Navigate from the original <table> to the column which contains the supplied column name.
            var col = wrapper.FindElement(By.XPath(string.Format("//thead/tr/th[text()='{0}']", column)));

            // Generate the css class for the direction in which we want to sort.
            var dir = direction.ToString() == "Ascending" ? "sorting_asc" : "sorting_desc";

            // If it is not already sorted in the right direction, click the column in order to trigger a sort.
            while (col.GetAttribute("class") != dir)
            {
                col.Click();
            }
        }

        /// <summary>
        /// Method to change the sorting of a column with the supplied index.
        /// </summary>
        /// <param name="index">
        /// The row index of the column to sort. Not zero based. Start at 1.
        /// </param>
        /// <param name="direction">
        /// The direction to sort this column.
        /// </param>
        public void SortColumn(int index, Sort direction)
        {
            // Get the <table> element for this widget.
            var wrapper = this.Driver.FindElement(this.Selector);

            // Navigate from the original <table> to the column with the supplied index.
            var col = wrapper.FindElement(By.XPath(string.Format("//thead/tr/th[{0}]", index)));

            // Generate the css class for the direction in which we want to sort.
            var dir = direction.ToString() == "Ascending" ? "sorting_asc" : "sorting_desc";

            // If it is not already sorted in the right direction, click the column in order to trigger a sort.
            while (col.GetAttribute("class") != dir)
            {
                col.Click();
            }
        }

        /// <summary>
        /// Method to search and filter results based on the string supplied.
        /// </summary>
        /// <param name="query">
        /// The query text to search for.
        /// </param>
        public void SearchForText(string query)
        {
            // Get the ID of the original <table> element.
            // This is overridden and replaced by the third party DataTables widget.
            var id = this.Driver.FindElement(this.Selector).GetAttribute("id");

            // Navigate from the original <table> to the DataTables wrapper of the search filter control.
            // The HTML for this control is dynamically injected.
            var wrapper = this.Driver.FindElement(By.Id(string.Format("{0}_filter", id)));

            // Get the <input> element where the search text is entered.
            var element = wrapper.FindElement(By.TagName("input"));

            // Clear the previous value using the backspace key instead of the built in Selenium Clear() method.
            // This is because the DataTables 'keyup' and 'keydown' events must be triggered in order to work correctly.
            while (wrapper.FindElement(By.TagName("input")).GetAttribute("value").Length > 0)
            {
                element.SendKeys(Keys.Backspace);
            }

            // Send the string which will be typed into the search box.
            element.SendKeys(query);

            // It takes a moment for DataTables to register that search text has been entered, so wait.
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Method to get the summary text for the table.
        /// </summary>
        /// <returns>
        /// Returns the summary text.
        /// </returns>
        public string GetSummaryText()
        {
            // Get the ID of the original <table> element.
            // This is overridden and replaced by the third party DataTables widget.
            var id = this.Driver.FindElement(this.Selector).GetAttribute("id");

            // Navigate from the original <table> to the DataTables wrapper of the summary text.
            // The HTML for this control is dynamically injected.
            var wrapper = this.Driver.FindElement(By.Id(string.Format("{0}_info", id)));

            // Return the summary text.
            return wrapper.Text;
        }

        /// <summary>
        /// Method to get the total number of entries for the table (before filtering).
        /// </summary>
        /// <returns>
        /// Returns the total number of entries (before filtering).
        /// </returns>
        public int GetTotalEntries()
        {
            // Get the total entries from the summary text using regular expressions.
            var match = Regex.Match(this.GetSummaryText(), @"Showing \d+ to \d+ of (\d+) entries.*");

            // Return the total entries.
            return match.Groups[1] != null ? Convert.ToInt32(match.Groups[1].Value) : 0;
        }

        /// <summary>
        /// Method to get a table row element which contains the text supplied.
        /// </summary>
        /// <param name="text">
        /// The text used to search each row.
        /// </param>
        /// <returns>
        /// Returns the table row element.
        /// </returns>
        public IWebElement GetRowElement(string text)
        {
            // Get the <table> element for this widget.
            var wrapper = this.Driver.FindElement(this.Selector);

            // Navigate from the original <table> to the row which contains the supplied text.
            return wrapper.FindElement(By.XPath(string.Format("//tbody/tr[td//text()[contains(., '{0}')]]", text)));
        }

        /// <summary>
        /// Method to get a table row element with the supplied index.
        /// </summary>
        /// <param name="index">
        /// The row index. Not zero based. Start at 1.
        /// </param>
        /// <returns>
        /// Returns the table row element.
        /// </returns>
        public IWebElement GetRowElement(int index)
        {
            // Get the <table> element for this widget.
            var wrapper = this.Driver.FindElement(this.Selector);

            // Navigate from the original <table> to the row with the supplied index.
            return wrapper.FindElement(By.XPath(string.Format("//tbody/tr[{0}]", index)));
        }

        /// <summary>
        /// Method to get the value of a particular row cell.
        /// </summary>
        /// <param name="rowElement">
        /// The element of the row being targeted.
        /// </param>
        /// <param name="cellIndex">
        /// The index of the cell being targeted. Not zero based. Start at 1.
        /// </param>
        /// <param name="asHtml">
        /// Determines if the value should be returned in its HTML source.
        /// </param>
        /// <returns>
        /// Returns the value of the cell.
        /// </returns>
        public string GetCellValue(IWebElement rowElement, int cellIndex, bool asHtml = false)
        {
            if (asHtml)
            {
                // Get the HTML source value of the row cell.
                return rowElement.FindElement(By.XPath(string.Format("//td[{0}]", cellIndex))).GetAttribute("innerHTML"); ;
            }

            // Get the text value of the row cell.
            return rowElement.FindElement(By.XPath(string.Format("//td[{0}]", cellIndex))).Text;
        }

        /// <summary>
        /// Method to click a hyperlink or button in a particular row cell.
        /// </summary>
        /// <param name="rowElement">
        /// The element of the row being targeted.
        /// </param>
        /// <param name="cellIndex">
        /// The index of the cell being targeted. Not zero based. Start at 1.
        /// </param>
        public void ClickRowCell(IWebElement rowElement, int cellIndex)
        {
            // Attempt to find a hyperlink or button to click within the supplied row and cell.
            try
            {
                // Search for a hyperlink to click.
                rowElement.FindElement(By.XPath(string.Format("//td[{0}]//a", cellIndex))).Click();
                return;
            }
            catch (NoSuchElementException)
            {
                // No hyperlinks found.
            }

            try
            {
                // Search for a button to click.
                rowElement.FindElement(By.XPath(string.Format("//td[{0}]//button", cellIndex))).Click();
            }
            catch (NoSuchElementException)
            {
                // No buttons found.
            }
        }

        /// <summary>
        /// Method to reset the table to its initial load state. This resets the page size, pagination, sorting and search fields.
        /// </summary>
        public void ResetTable()
        {
            // Reset the page size back to 10.
            this.ChangePageSize(PageSizes.Ten);

            // Reset the pagination by navigating back to page 1.
            this.ChangePagination(Pagination.First);

            // Reset the sorting so that the first column is in ascending order.
            this.SortColumn(1, Sort.Ascending);

            // Reset the search box by clearing all text from that field.
            this.SearchForText(string.Empty);
        }

        /// <summary>
        /// Method to automatically test that the table has loaded correctly.
        /// </summary>
        public void TestTableHasLoaded()
        {
            // Check the table has been initialised by making sure the summary text is rendered.
            Assert.IsTrue(this.GetSummaryText().Contains("Showing"), "The table has not loaded correctly");
        }

        /// <summary>
        /// Method to automatically test that the table supports changing the page size.
        /// </summary>
        public void TestPageSize()
        {
            // Reset the table to its initial load state, ready for this test.
            this.ResetTable();

            // Get the total number of entries for the table.
            var total = this.GetTotalEntries();

            if (total == 0)
            {
                // There are no entries in the table so look at the summary text and expect to see 0.
                Assert.IsTrue(
                    this.GetSummaryText() == "Showing 0 to 0 of 0 entries",
                    "The table does not display the correct summary text for 0 entries.");

                // Skip the remaining tests as there is no data.
                return;
            }

            // Check the table page length is initially 10 by looking at the summary text.
            Assert.IsTrue(
                total >= 10
                    ? this.GetSummaryText() == string.Format("Showing 1 to 10 of {0} entries", total)
                    : this.GetSummaryText() == string.Format("Showing 1 to {0} of {0} entries", total),
                "The table does not support changing the page size to 10.");

            // Change the number of results per page to 25.
            this.ChangePageSize(PageSizes.TwentyFive);

            // Check the table page length is now 25 by looking at the summary text.
            Assert.IsTrue(
                total >= 25
                    ? this.GetSummaryText() == string.Format("Showing 1 to 25 of {0} entries", total)
                    : this.GetSummaryText() == string.Format("Showing 1 to {0} of {0} entries", total),
                "The table does not support changing the page size to 25.");

            // Change the number of results per page to 50.
            this.ChangePageSize(PageSizes.Fifty);

            // Check the table page length is now 50 by looking at the summary text.
            Assert.IsTrue(
                total >= 50
                    ? this.GetSummaryText() == string.Format("Showing 1 to 50 of {0} entries", total)
                    : this.GetSummaryText() == string.Format("Showing 1 to {0} of {0} entries", total),
                "The table does not support changing the page size to 50.");

            // Change the number of results per page to 100.
            this.ChangePageSize(PageSizes.Hundred);

            // Check the table page length is now 100 by looking at the summary text.
            Assert.IsTrue(
                total >= 100
                    ? this.GetSummaryText() == string.Format("Showing 1 to 100 of {0} entries", total)
                    : this.GetSummaryText() == string.Format("Showing 1 to {0} of {0} entries", total),
                "The table does not support changing the page size to 100.");

            // Reset the number of results per page back to 10.
            this.ChangePageSize(PageSizes.Ten);

            // Check the table page length has been reset to 10 by looking at the summary text.
            Assert.IsTrue(
                total >= 10
                    ? this.GetSummaryText() == string.Format("Showing 1 to 10 of {0} entries", total)
                    : this.GetSummaryText() == string.Format("Showing 1 to {0} of {0} entries", total),
                "The table does not support changing the page size to 10.");
        }

        /// <summary>
        /// Method to automatically test that the table supports pagination.
        /// </summary>
        public void TestPagination()
        {
            // Reset the table to its initial load state, ready for this test.
            this.ResetTable();

            // Get the total number of entries for the table.
            var total = this.GetTotalEntries();

            if (total == 0)
            {
                // There are no entries in the table so look at the summary text and expect to see 0.
                Assert.IsTrue(
                    this.GetSummaryText() == "Showing 0 to 0 of 0 entries",
                    "The table does not display the correct summary text for 0 entries.");

                // Skip the remaining tests as there is no data.
                return;
            }

            // Check the table page is initially 1 by looking at the summary text.
            Assert.IsTrue(
                total >= 10
                    ? this.GetSummaryText() == string.Format("Showing 1 to 10 of {0} entries", total)
                    : this.GetSummaryText() == string.Format("Showing 1 to {0} of {0} entries", total),
                "The table does not support navigating to page 1.");

            if (total > 10)
            {
                // Change the current page to 2.
                this.ChangePagination(Pagination.Next);

                // Check the table page is now 2 by looking at the summary text.
                Assert.IsTrue(
                    total >= 20
                        ? this.GetSummaryText() == string.Format("Showing 11 to 20 of {0} entries", total)
                        : this.GetSummaryText() == string.Format("Showing 11 to {0} of {0} entries", total),
                    "The table does not support navigating to page 2.");
            }

            if (total > 20)
            {
                // Change the current page to 3.
                this.ChangePagination(Pagination.Next);

                // Check the table page is now 3 by looking at the summary text.
                Assert.IsTrue(
                    total >= 30
                        ? this.GetSummaryText() == string.Format("Showing 21 to 30 of {0} entries", total)
                        : this.GetSummaryText() == string.Format("Showing 21 to {0} of {0} entries", total),
                    "The table does not support navigating to page 3.");
            }

            // Reset the current page back to 1.
            this.ChangePagination(Pagination.First);

            // Check the table page has been reset to 1 by looking at the summary text.
            Assert.IsTrue(
                total >= 10
                    ? this.GetSummaryText() == string.Format("Showing 1 to 10 of {0} entries", total)
                    : this.GetSummaryText() == string.Format("Showing 1 to {0} of {0} entries", total),
                "The table does not support navigating to page 1.");
        }

        /// <summary>
        /// Method to automatically test that the table supports sorting.
        /// </summary>
        public void TestSorting()
        {
            // Reset the table to its initial load state, ready for this test.
            this.ResetTable();

            // Get the total number of entries for the table.
            var total = this.GetTotalEntries();

            if (total == 0)
            {
                // There are no entries in the table so look at the summary text and expect to see 0.
                Assert.IsTrue(
                    this.GetSummaryText() == "Showing 0 to 0 of 0 entries",
                    "The table does not display the correct summary text for 0 entries.");

                // Skip the remaining tests as there is no data.
                return;
            }

            if (total > 1)
            {
                // Get the value of the first column in the first row of the table.
                var currentValue = this.GetCellValue(this.GetRowElement(1), 1);

                // Sort the first column in descending order.
                this.SortColumn(1, Sort.Descending);

                // Get the value of the first column in the first row of the table, now that it has been sorted.
                var newValue = this.GetCellValue(this.GetRowElement(1), 1);

                // Check the table has sorted the results by making sure the first item is now different.
                Assert.IsTrue(
                    currentValue != newValue,
                    "The table does not support sorting.");

                // Reset the sorting so that the first column is in ascending order.
                this.SortColumn(1, Sort.Ascending);
            }
        }

        /// <summary>
        /// Method to automatically test that the table supports searching.
        /// </summary>
        public void TestSearching()
        {
            // Reset the table to its initial load state, ready for this test.
            this.ResetTable();

            // Get the total number of entries for the table.
            var total = this.GetTotalEntries();

            if (total == 0)
            {
                // There are no entries in the table so look at the summary text and expect to see 0.
                Assert.IsTrue(
                    this.GetSummaryText() == "Showing 0 to 0 of 0 entries",
                    "The table does not display the correct summary text for 0 entries.");

                // Skip the remaining tests as there is no data.
                return;
            }

            // Get the value of the first column in the first row of the table.
            var currentValue = this.GetCellValue(this.GetRowElement(1), 1);

            // Search for this value in the table.
            this.SearchForText(currentValue);

            // Get the value of the first column in the first row of the table, now that it has been searched.
            var newValue = this.GetCellValue(this.GetRowElement(1), 1);

            if (total > 1)
            {
                // Check the number of total entries is different. There should be less now that results are being filtered.
                Assert.IsTrue(
                    this.GetTotalEntries() < total,
                    "The table does not support searching. The number of results returned is incorrect.");
            }

            // Check the table is displaying the item that was searched for.
            Assert.IsTrue(
                currentValue == newValue,
                "The table does not support searching. The item cannot be found in the results.");

            // Check the summary text has been updated with the number of filtered results.
            Assert.IsTrue(
                this.GetSummaryText().Contains(string.Format("(filtered from {0} total entries)", total)),
                "The table does not support searching. The summary text is incorrect.");

            // Reset the search box by clearing all text from that field.
            this.SearchForText(string.Empty);
        }

        /// <summary>
        /// Method to automatically test that the table has a particular item which contains the supplied text.
        /// </summary>
        /// <param name="text">
        /// The text used to search the table.
        /// </param>
        public void TestTableHasItem(string text)
        {
            // Search the table using the text supplied. This filters out results which are not relevant.
            this.SearchForText(text);

            try
            {
                // Attempt to get the element for the row which contains the supplied text.
                this.GetRowElement(text);
            }
            catch (NoSuchElementException)
            {
                // Element not found so the item does not exist. Throw an assert failure to report this failed test.
                Assert.IsTrue(false, "The table does not contain the expected item.");
            }

            // Reset the search box by clearing all text from that field.
            this.SearchForText(string.Empty);
        }

        /// <summary>
        /// DataTable widget.
        /// </summary>
        /// <param name="driver">
        /// The Selenium web driver instance.
        /// </param>
        /// <param name="selector">
        /// The Selenium selector for this widget.
        /// </param>
        public DataTable(IWebDriver driver, By selector)
        {
            this.Driver = driver;
            this.Selector = selector;
        }
    }
}