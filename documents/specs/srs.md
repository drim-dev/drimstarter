# Software Requirements Specification

* [Software Requirements Specification](#software-requirements-specification)
  * [Use Cases](#use-cases)
  * [Overall Description](#overall-description)
    * [User Classes](#user-classes)
    * [User Registration and Authentication](#user-registration-and-authentication)
      * [Preconditions](#preconditions)
      * [Main Flow](#main-flow)
      * [Alternative Flow (Password Reset)](#alternative-flow-password-reset)
      * [Postconditions](#postconditions)
      * [Backend Features](#backend-features)
    * [User Profile Management](#user-profile-management)
      * [Preconditions](#preconditions-1)
      * [Main Flow](#main-flow-1)
      * [Postconditions](#postconditions-1)
      * [Backend Features](#backend-features-1)
        * [Get User Profile](#get-user-profile)
        * [Update User Profile](#update-user-profile)
        * [Update User Profile Avatar](#update-user-profile-avatar)
    * [Project Creation](#project-creation)
      * [Preconditions](#preconditions-2)
      * [Main Flow](#main-flow-2)
      * [Postconditions](#postconditions-2)
      * [Backend Features](#backend-features-2)
        * [Create project](#create-project)
        * [Upload Project Media](#upload-project-media)

## Use Cases

## Overall Description

### User Classes

* Visitors - Individuals who access the platform without registering or logging in.
* Registered Users - Individuals who have created an account on the platform.
* Backers - Registered users who pledge financial support to projects.
* Creators - Registered users who initiate and manage crowdfunding projects.
* Moderators - Staff or appointed individuals who monitor user-generated content.

### User Registration and Authentication

__Id__: Authentication

__Actors__: Visitor

__Goal__: Allow a visitor to create an account and securely log in to the platform.

#### Preconditions

* The visitor has access to the internet and the platform's website or mobile app.

#### Main Flow

1. The visitor navigates to the registration page.
2. The visitor selects the option to sign up via email or a social media account.
3. The system presents a registration form requesting necessary information (e.g., email, password).
4. The visitor completes and submits the form.
5. The system validates the input data.
6. The system creates a new user account.
7. The system sends a confirmation email to the visitor.
8. The visitor confirms their email address by clicking the link provided.
9. The visitor logs in using their credentials.

#### Alternative Flow (Password Reset)

* If the user forgets their password:
  1. The user selects "Forgot Password" on the login page.
  2. The system prompts the user to enter their registered email address.
  3. The user submits their email.
  4. The system sends a password reset link to the email.
  5. The user clicks the link and is prompted to create a new password.
  6. The user creates a new password and logs in.

#### Postconditions

* The user has an active account and can access platform features that require authentication.

#### Backend Features

TBD

### User Profile Management

__Id__: UserProfile

__Actors__: Registered User

__Goal__: Enable users to manage their personal profile information.

#### Preconditions

* The user is logged into their account.

#### Main Flow

1. The user navigates to their profile settings.
2. The system displays the current profile information.
3. The user edits their personal details: name, bio, date of registration, avatar.
4. The user saves the changes.
5. The system updates the profile information.

#### Postconditions

* The user's profile reflects the updated information.

#### Backend Features

##### Get User Profile

__Id__: UserProfile.GetUserProfile

__Authz__: Visitor

__Description__:

Returns a user profile.

__Request__:

```http
GET /users/{userId}/profile
```

__Response__:

```http
200 OK
Content-Type: application/json

{
  "name": "John Doe",
  "bio": "Software developer and tech enthusiast.",
  "registeredAt": "2024-09-10T09:00:00Z",
  "avatarUrl": "/images/abcdef"
}
```

##### Update User Profile

__Id__: UserProfile.UpdateUserProfile

__Authz__: Registered User

__Description__:

Updates a user profile.

__Request__:

```http
PATCH /users/{userId}/profile
Content-Type: application/json

{
  "name": "John Doe",
  "bio": "Software developer and tech enthusiast."
}
```

__Response__:

```http
200 OK
Content-Type: application/json

{
  "name": "John Doe",
  "bio": "Software developer and tech enthusiast.",
  "registeredAt": "2024-09-10T09:00:00Z",
  "avatarUrl": "/images/abcdef"
}
```

##### Update User Profile Avatar

__Id__: UserProfile.UpdateUserProfileAvatar

__Authz__: Registered User

__Description__:

Updates a user profile avatar.

__Request__:

```http
PUT /users/{userId}/avatar
Content-Type: multipart/form-data

(Form-data with the 'avatar' field containing the image file)
```

__Response__:

```http
200 OK
Content-Type: application/json

{
  "avatarUrl": "/images/abcdef"
}
```

### Project Creation

__Id__: ProjectCreation

__Actors__: Registered User

__Goal__: Enable users to create funding projects for their ideas or products.

#### Preconditions

* The user is logged into their account.

#### Main Flow

1. The user navigates to the "Create a Project" page.
2. The system displays a project creation form.
3. The user fills in the project details: title, description, story, funding goal, project duration (start and end dates), category, rewards for backers.
4. The user uploads supporting media.
5. The user submits the project for approval.

#### Postconditions

* The new project is created and pending approval.

#### Backend Features

##### Create project

__Id__: ProjectCreation.CreateProject

__Authz__: Registered User

__Description__:

Creates a new project.

__Request__:

```http
POST /projects
Content-Type: application/json

{
  "title": "Innovative Gadget",
  "description": "An innovative gadget that improves daily life.",
  "story": "This project was inspired by my passion for simplifying everyday tasks.",
  "fundingGoal": 50000,
  "startDate": "2024-10-01T00:00:00Z",
  "endDate": "2024-11-30T00:00:00Z",
  "categoryId": 1,
  "rewards": [
    {"amount": 10, "reward": "Thank you email"},
    {"amount": 50, "reward": "Early access to the product"}
  ]
}
```

__Response__:

```http
201 Created
Content-Type: application/json

{
  "id": "projectId456",
  "title": "Innovative Gadget",
  "description": "An innovative gadget that improves daily life.",
  "story": "This project was inspired by my passion for simplifying everyday tasks.",
  "fundingGoal": 50000,
  "sum": 0,
  "startDate": "2024-10-01T00:00:00Z",
  "endDate": "2024-11-30T00:00:00Z",
  "status": "Under Review",
  "createdAt": "2024-09-26T09:00:00Z"
}
```

##### Upload Project Media

__Id__: ProjectCreation.UploadProjectMedia

__Authz__: Registered User

__Description__:

Uploads media files for the project.

__Request__:

```http
POST /projects/{projectId}/media
Content-Type: multipart/form-data

(Form-data with fields for 'images' and 'video')
```

__Response__:

```http
200 OK
Content-Type: application/json

{
  "avatarUrl": "/images/abcdef"
}
```