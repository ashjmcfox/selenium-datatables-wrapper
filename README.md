C# Selenium Datatables Wrapper
==============================

A .NET C# wrapper for interacting with the [jQuery DataTables plugin](https://www.datatables.net/), while executing Selenium tests.

Two versions are provided:

- A generic implementation for working with the standard Selenium web driver (IWebDriver). This should be used for most cases.
- An AngularJS implementation which uses the [.NET port of Protractor](https://github.com/bbaia/protractor-net) (NgWebDriver). This can be used for AngularJS web applications.

Both versions are fully commented and explain what each method is doing.

Usage
-----

There are many helpful methods provided to interact with the DataTable.

You can interact with it directly or you can run some built-in tests. These tests can check things like changing the page size, pagination, filtering and more. Their method names are prefixed with "Test".

The advantage of using the built-in tests is that they will save you writing a lot of code to check common behaviour which applies to every table. If a test encounters a problem, it will throw an NUnit Assert Exception with an appropriate error message.

All built-in tests are designed to be flexible with the data that is available. For example, a pagination test will not fail if it tries to navigate to page 3 and you only have enough data present for 2 pages. It will skip certain tests if there is not enough data available.

```C#
// Create a new DataTable instance which will be used to interact with the table.
// Provide it with your web driver and the Selenium selector of the table.
var table = new DataTable(driver, By.Id("my-table-id"));

// ---------------------------------------------------------
// Now interact with it directly...
// ---------------------------------------------------------

// Change the number of results per page to 25.
table.ChangePageSize(DataTable.PageSizes.TwentyFive);

// Navigate to the next page.
table.ChangePagination(DataTable.Pagination.Next);

// Sort the "ID" column in descending order.
table.SortColumn("ID", DataTable.Sort.Descending);

// Sort the first column in ascending order. Index parameter is NOT zero based.
table.SortColumn(1, DataTable.Sort.Ascending);

// Search for some text in the table.
table.SearchForText("My search query");

// Get the summary text of the table.
var summary = table.GetSummaryText();

// Get the total number of entries in the table.
var total = table.GetTotalEntries();

// Get the element of the row which contains the supplied text.
table.GetRowElement("My item");

// Get the element of the first row. Index parameter is NOT zero based.
table.GetRowElement(1);

// Get the HTML value of the first cell in the second row of the table.
var secondRow = table.GetRowElement(2);
var html = table.GetCellValue(secondRow, 1, true);

// Click the hyperlink or button in the second cell of the third row in the table.
var thirdRow = table.GetRowElement(3);
table.ClickRowCell(thirdRow, 2);

// Reset the table to its initial load state.
table.ResetTable();

// ---------------------------------------------------------
// Or run some built-in tests...
// ---------------------------------------------------------

// Check the table has loaded using the built-in test method.
table.TestTableHasLoaded();

// Check the table supports changing the page size using the built-in test method.
table.TestPageSize();

// Check the table supports pagination using the built-in test method.
table.TestPagination();

// Check the table supports sorting using the built-in test method.
table.TestSorting();

// Check the table supports searching using the built-in test method.
table.TestSearching();

// Check that an item exists in the table.
table.TestTableHasItem("My item");
```

Pagination
----------

The following `pagingType` pagination styles are supported:

- `full`
- `full_numbers`
- `bootstrap` - using the third party [Bootstrap pagination plugin](https://github.com/Jowin/Datatables-Bootstrap3)

DataTables configured to use `simple` or `simple_numbers` may work depending on which methods are called and your specific use case, but it is not recommended.

Dependencies
------------
Any projects using this wrapper will require the following:

- [NUnit Framework](http://nunit.org/) | [NuGet Package](https://www.nuget.org/packages/NUnit)
- [Selenium](http://docs.seleniumhq.org/) | [NuGet Packages](https://www.nuget.org/profiles/selenium)

In addition, if using the AngularJS version:

- [.NET port of Protractor](https://github.com/bbaia/protractor-net) | [NuGet Package](https://www.nuget.org/packages/Protractor/)

Testing
-------
This wrapper has been tested using:

- Datatables v1.10.7
- Selenium v2.45.0
- Latest Chrome web driver.

Changelog
---------
See [CHANGELOG.md](https://github.com/ashjmcfox/selenium-datatables-wrapper/blob/master/CHANGELOG.md)

License
-------
See [LICENSE.md](https://github.com/ashjmcfox/selenium-datatables-wrapper/blob/master/LICENSE.md)
