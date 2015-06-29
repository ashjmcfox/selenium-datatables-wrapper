<a name="1.1.2"></a>
# 1.1.2 Bug fix release (2015-06-29)

## Bug Fixes

- Fixed issue with DataTable search test. Now accounts for searching a table with only one record.
- Fixed issues when interacting with multiple Datatables on the same page. Modified XPath selectors.

## Features

- Improved mechanism for clearing search strings from a DataTable. Provides better performance.

## Breaking Changes

- None

---

<a name="1.1.1"></a>
# 1.1.1 Pagination Mark II (2015-06-11)

## Bug Fixes

- Fixed detection for Bootstrap pagination plugin when using `bs_four_button` and `bs_full` paging options.

## Features

- None

## Breaking Changes

- None

---

<a name="1.1.0"></a>
# 1.1.0 Pagination (2015-06-09)

## Bug Fixes

- None

## Features

- Added support for the following `pagingType` pagination styles:

  - `full`
  - `full_numbers`
  - Using the third party [Bootstrap pagination plugin](https://github.com/Jowin/Datatables-Bootstrap3):
    - `bs_four_button`
    - `bs_full`

## Breaking Changes

- None

---

<a name="1.0.0"></a>
# 1.0.0 Initial release (2015-06-09)

## Bug Fixes

- N/A

## Features

- Interaction:
  - Change page size
  - Navigate pagination
  - Sort columns using the column name or index
  - Search for text
  - Click row cell (looks for a hyperlink or button)
- Retrieve information:
  - Get summary text
  - Get total entries
  - Get row element
  - Get cell value (as text or HTML source)
- Built-in tests:
  - Check table has loaded
  - Check changing the page size
  - Check navigating using pagination
  - Check sorting
  - Check searching
  - Check table has item
- Utility:
  - Reset table (clears settings for page size, pagination, filtering and sorting)

## Breaking Changes

- N/A