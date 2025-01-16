# TAABP

Travel and Accommodation booking platform serves as the final project at my internship in **Foothill Technology Solutions**.

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
