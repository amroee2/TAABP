# TAABP

Travel and Accommodation booking platform serves as the final project at my internship in **Foothill Technology Solutions**.

## Features

- User Management through ASP.Net Identity

Users can register and will recieve a confirmation link to their submitted email

### Provides:

- Email, password and other attributes validations
- Password Hashing
- Required Email Confirmation
- Email Changing
- Password Changing
- User personal information (address, phone number...) update.
- JWT Token generation and validation
- Api end points to see user last hotels visited, reviews, carts and payments
- Logging using Serilogger

### City, Hotel, Room, Amentiy, Featured Deals crud operations.

- Access control through authorization attributes
- City, hotel, room, amenity and featured deals creation, update and delete are done by admins.
- Top Visited Cities
- Hotel average reviews calculation based on user reviews
- Increment and decrement number of rooms for hotels after creation
- Increment and decrement number of hotels for cities after creation
- Hotel searching based on various fields

### User reservation and reviews

- User reviews on hotels with comments and rating that reflect on the overall rating of the hotel
- credit card and paypal payment options
- email confirmation after reservation

## Domain Models

ERD diagram

![TAABP ERD Diagram](https://github.com/user-attachments/assets/75a17830-9099-4af6-a2e9-4e82b001f7c4)

## Architecutre

![image](https://github.com/user-attachments/assets/9406e937-22cb-4b2a-ae20-61092fbb5f15)

**Clean Architecture** was used with the following layers

## Presentation layer:

- Controllers
- Logs

## Application layer:

- DTOs
- Exceptions
- Mappers
- Repositories Interfaces
- Servicies Interfaces
- Token Geneators
- Validators
- External dependencies Interfaces

## Core layer

Contains the domain models

- User
- Hotel
- Room
- Hotel Images
- Room Images
- City
- Reservation
- Review
- Amenity
- PaymentMethod
- CreditCard
- PayPal
- Cart
- CartItem
- FeaturedDeal

## Infrastructure layer

- Repositories
- Migrations
- Seedings
- External Dependencies

## Testing

- Integration testing for repository methods
- Unit Testing for service methods
- Api Testing through postman for api endpoints (Postman collection file is available)

## Swagger documentaiton 

[Api Documentation](https://app.swaggerhub.com/apis/fts-82b/api/1.0)

## Jira for task management

![image](https://github.com/user-attachments/assets/7eca6c94-63a4-4ea3-b91c-9f8666a545bd)
