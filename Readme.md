### Documentations

For the interface and the class I will follow the same structure. but I will change the Date to DateValue also I may change the gender to enum instead of the string.

First I used sqlite hence its easier to develop (=> search about that) => I will add sql server later.

Assumed relations :
    * One-to-Many Relationship between `Entity` and `Address`
    * One-to-Many Relationship between `Entity` and `Date`
    * One-to-Many Relationship between `Entity` and `Name`

Implement Actions:
- `GetEntities`: Retrieves all `Entity` objects.
- `GetEntity`: Retrieves a specific `Entity` by ID.
- `PostEntity`: Creates a new `Entity`.
- `PutEntity`: Updates an existing `Entity`.
- `DeleteEntity`: Deletes an `Entity` by ID.

Lazy loading is enabled, for automatic loading when accessed, without the need for explicit eager loading(using `.Include`).

For the circular reference, the JSON serializer is instructed to ignore it by using the `JsonIgnore` attribute on the navigation properties that are causing it.

User can search by Id, Names, FirstName, Surname, Country, AddressLine

User can filter by Gender, Country, AddressLine, Date (start date & end date)

Sorting is dont through reflection(dynamically determine the property to sort by) to dynamically determine the property to sort by.

Pagination is applied using the Skip and Take methods based on the specified page and page size.

Pagination metadata is included in the response headers.

Sorting and pagination are applied before materializing the query using ToList() to optimize performance.


