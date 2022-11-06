# Mita Manga Reviews

## Roles
- Guest (Can view mangas and reviews)
- Reviewer (Can view, create, update and delete reviews)
- Admin (Can view, create, update and delete mangas and users)

## Models

### User
Available endpoints features:
- Register (For registering users with "Guest" role)
- Login
- Find all (Admin)
- Find by id (Admin)
- Create (In theory this endpoint should require "Admin" role, but it was removed in order to create users with any role to test the API)
- Update (Admin)
- Delete (Admin)

### Manga
Available endpoints features:
- Find all
- Find by id
- Create (Admin)
- Update (Admin)
- Delete (Admin)

### Reviews
Available endpoints features:
- Find all
- Find by id
- Create (Reviewer)
- Update (Reviewer, a review can only be edited by its owner)
- Delete (Reviewer, a review can only be deleted by its owner)

## Pending
- Models can be deleted because of foreign keys
