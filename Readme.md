

# API Documentation

## Introduction

This API provides endpoints for managing entities in a system. Entities represent objects with various attributes such as addresses, dates, and names. The API allows users to perform CRUD (Create, Read, Update, Delete) operations on entities, as well as search, filter, sort, and paginate entities based on specific criteria.

## Endpoints

### 1. Get All Entities

- **URL:** `/api/Entity`
- **Method:** GET
- **Description:** Retrieves a list of all entities.
- **Request Parameters:**
    - None
- **Response:**
    - Status Code: 200 (OK)
    - Body: Array of entity objects

### 2. Get Entity by ID

- **URL:** `/api/Entity/{id}`
- **Method:** GET
- **Description:** Retrieves a specific entity by its ID.
- **Request Parameters:**
    - `id`: ID of the entity to retrieve (in the URL path)
- **Response:**
    - Status Code: 200 (OK) if the entity is found
    - Status Code: 404 (Not Found) if the entity is not found
    - Body: Entity object

### 3. Create Entity

- **URL:** `/api/Entity`
- **Method:** POST
- **Description:** Creates a new entity.
- **Request Body:**
    - JSON object representing the entity to create
- **Response:**
    - Status Code: 201 (Created) if the entity is created successfully
    - Status Code: 400 (Bad Request) if the request body is invalid
    - Body: Created entity object

### 4. Update Entity

- **URL:** `/api/Entity/{id}`
- **Method:** PUT
- **Description:** Updates an existing entity.
- **Request Parameters:**
    - `id`: ID of the entity to update (in the URL path)
- **Request Body:**
    - JSON object representing the updated entity
- **Response:**
    - Status Code: 204 (No Content) if the entity is updated successfully
    - Status Code: 400 (Bad Request) if the request body is invalid
    - Status Code: 404 (Not Found) if the entity is not found

### 5. Delete Entity

- **URL:** `/api/Entity/{id}`
- **Method:** DELETE
- **Description:** Deletes an existing entity.
- **Request Parameters:**
    - `id`: ID of the entity to delete (in the URL path)
- **Response:**
    - Status Code: 204 (No Content) if the entity is deleted successfully
    - Status Code: 404 (Not Found) if the entity is not found

### 6. Search and Filter Entities

- **URL:** `/?search={search}&gender={gender}&country={country}&addressLine={addressLine}&startDate={startDate}&endDate={endDate}&sortBy={sortBy}&sortDirection={sortDirection}&page={page}&pageSize={pageSize}`
- **Method:** GET
- **Description:** Retrieves a list of entities based on search and filter criteria.
- **Request Parameters:**
    - `search`: Search term for filtering entities by name
    - `gender`: Gender of the entities
    - `country`: Country of the entities' addresses
    - `addressLine`: Address line of the entities' addresses
    - `startDate`: Start date for filtering entities by date range
    - `endDate`: End date for filtering entities by date range
    - `sortBy`: Field to sort entities by (e.g., name, date)
    - `sortDirection`: Sorting direction (ASC or DESC)
    - `page`: Page number for pagination
    - `pageSize`: Number of entities per page
- **Response:**
    - Status Code: 200 (OK)
    - Header: Contains `X-Pagination` with value of `{"TotalCount":8,"PageSize":10,"CurrentPage":1,"TotalPages":1}`
    - Body: Array of filtered entity objects

Sure, here's the section formatted in Markdown:

## Authentication

### JSON Web Token (JWT) Authentication

The API utilizes JSON Web Tokens (JWT) for authentication. JWT is a compact, URL-safe means of representing claims to be transferred between two parties. It is commonly used for securing API endpoints by providing a stateless authentication mechanism.

#### Obtaining a JWT Token
You need to register a new user through `/Authenticate` endpoint once done you can obtain a JWT token by sending a POST request to the `/Authenticate/login` endpoint with valid credentials. Upon successful authentication, the server will respond with a JWT token in the response body.

##### Example Request:

```http
POST /auth/login HTTP/1.1
Content-Type: application/json

{
    "username": "example_user",
    "password": "example_password"
}
```

##### Example Response:

```json
HTTP/1.1 200 OK
Content-Type: application/json
{
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJleGFtcGxlX3VzZXIiLCJleHAiOjE2NDkzOTkxOTYsImlzcyI6Imh0dHBzOi8vYXV0aC5leGFtcGxlLmNvbSIsImlhdCI6MTY0OTM5NTU5Nn0.T2O2C1hIzZ9-2nY7Xe91gL5VIZr-LGj48sdPdqzO8P8"
}
```

#### Using the JWT Token

Once a JWT token is obtained, clients must include it in the `Authorization` header of subsequent requests to access protected endpoints. The token should be prefixed with the word "Bearer" followed by a space.

##### Example Request:

```http
GET /api/entity HTTP/1.1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJleGFtcGxlX3VzZXIiLCJleHAiOjE2NDkzOTkxOTYsImlzcyI6Imh0dHBzOi8vYXV0aC5leGFtcGxlLmNvbSIsImlhdCI6MTY0OTM5NTU5Nn0.T2O2C1hIzZ9-2nY7Xe91gL5VIZr-LGj48sdPdqzO8P8
```

#### Token Expiration

JWT tokens issued by the API have a limited lifespan and will expire after a certain period. Clients should be prepared to handle token expiration by refreshing tokens or re-authenticating when necessary.    

## Error Handling

The API returns appropriate HTTP status codes and error messages with the help of `try-catch` and `ILogger` to indicate the failure of each request. Common error scenarios include invalid request parameters, missing required fields, and entity not found and Transient error.

## Retry and Backoff Strategy 
To ensure the stability and availability of the API, also provide a seamless user experience and minizing the impact of transient filuers, the api has a retry mechanism, hence this can cause the system to be overlead through repeated retry which can affact the stability as well as the user experience, the api has back off mechanism to delay the periods between retries.

To balance between system stability, user experience, and effective handling of potential transient failures. The chosen retry and backoff strategy is exponential backoff (increasing delays based on exponential).

This done through `RetryHelper` which takes the action and try to applied on the database, and if it fails it apply the mechanism form a number of tries.
This helper has options configured through `program.cs` and is stored in `appsittings.json`

## Conclusion

This documentation provides an overview of the endpoints, request parameters, and response formats supported by the API. You can found the api source code on https://github.com/MohammedHamed12121/EntityAPI/tree/main

For more details and examples, please refer to the individual endpoint descriptions above.
