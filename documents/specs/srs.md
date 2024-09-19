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
    * [Blog Posts Management](#blog-posts-management)
      * [Preconditions](#preconditions-2)
      * [Main Flow (Create a New Post)](#main-flow-create-a-new-post)
      * [Alternative Flow 1 (Update an Existing Post)](#alternative-flow-1-update-an-existing-post)
      * [Alternative Flow 2 (Delete an Existing Post)](#alternative-flow-2-delete-an-existing-post)
      * [Postconditions](#postconditions-2)
      * [Backend Features](#backend-features-2)
        * [Create Blog Post](#create-blog-post)
        * [Update Blog Post](#update-blog-post)
        * [Delete Blog Post](#delete-blog-post)
        * [Get Blog Post](#get-blog-post)
        * [List Blog Posts](#list-blog-posts)

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

### Blog Posts Management

__Id__: BlogPosts

__Actors__: Content Manager, Visitor

__Goal__: Allow the content manager to create, update, manage, and delete blog posts to share news about the platform with users.

#### Preconditions

* The content manager must be authenticated and have the necessary permissions to manage blog content.
* The platform must have an existing blog section where posts can be published, updated, or deleted.

#### Main Flow (Create a New Post)

1. The content manager logs in to the platform and navigates to the blog management section.
2. The content manager selects the "Create New Post" option.
3. The system presents a form where the content manager can:
    * Enter the post title.
    * Write or paste the content of the blog post.
    * Format the content (e.g., headings, bullet points, links).
    * Upload images or other media to be included in the post.
4. The content manager selects the "Preview" button to review the post before publishing.
5. The system displays a preview of the blog post as it will appear to users.
6. The content manager either:
    * Edits the post (if needed) and previews again, or
    * Selects the "Publish" button to make the post live on the platform.
7. The system confirms that the blog post has been successfully published and is now visible to users.

#### Alternative Flow 1 (Update an Existing Post)

1. The content manager navigates to the blog management section and selects an existing post to update.
2. The system displays the current content of the post in an editable format.
3. The content manager makes the necessary changes and previews the updated post.
4. The content manager selects the "Update" button to apply the changes.
5. The system confirms that the post has been successfully updated and the changes are now visible to users.

#### Alternative Flow 2 (Delete an Existing Post)

1. The content manager navigates to the blog management section and selects an existing post to delete.
2. The system displays the post details, including options to update or delete the post.
3. The content manager selects the "Delete Post" option.
4. The system prompts the content manager to confirm the deletion.
5. The content manager confirms the deletion.
6. The system deletes the blog post from the platform.
7. The system confirms that the post has been successfully deleted and is no longer visible to users.

#### Postconditions

* The blog post is either created, updated, or deleted based on the content manager's actions.
* Users can view the blog posts that are live on the platform, while deleted posts are no longer visible.

#### Backend Features

##### Create Blog Post

__Id__: BlogPosts.CreateBlogPost

__Authz__: Content Manager

__Description__:

Creates a blog post.

__Request__:

```http
POST /blog/posts
Content-Type: application/json

{
  "title": "News",
  "content": "Exciting news"
}
```

__Response__:

```http
201 Created
Content-Type: application/json
Location: https://drimstarter.com/blog/posts/abcdef

{
  "id": "abcdef"
  "title": "News",
  "content": "Exciting news"
  "createdAt": "2024-09-10T09:00:00Z",
  "updatedAt": "2024-09-10T09:00:00Z"
}
```

##### Update Blog Post

__Id__: BlogPosts.UpdateBlogPost

__Authz__: Content Manager

__Description__:

Updates a blog post.

__Request__:

```http
PUT /blog/posts/{postId}
Content-Type: application/json

{
  "title": "News",
  "content": "Exciting news"
}
```

__Response__:

```http
200 OK
Content-Type: application/json

{
  "id": "abcdef"
  "title": "News",
  "content": "Exciting news"
  "createdAt": "2024-09-10T09:00:00Z",
  "updatedAt": "2024-09-19T09:00:00Z"
}
```

##### Delete Blog Post

__Id__: BlogPosts.DeleteBlogPost

__Authz__: Content Manager

__Description__:

Deletes a blog post.

__Request__:

```http
DELETE /blog/posts/{postId}
```

__Response__:

```http
200 OK
```

##### Get Blog Post

__Id__: BlogPosts.GetBlogPost

__Authz__: Visitor

__Description__:

Returns a blog post.

```http
GET /blog/posts/{postId}
```

__Response__:

```http
200 OK
Content-Type: application/json

{
  "id": "abcdef"
  "title": "News",
  "content": "Exciting news"
  "createdAt": "2024-09-10T09:00:00Z",
  "updatedAt": "2024-09-19T09:00:00Z"
}
```

##### List Blog Posts

__Id__: BlogPosts.ListBlogPosts

__Authz__: Visitor

__Description__:

List blog posts sorted by creation date descending.

```http
GET /blog/posts
```

__Response__:

```http
200 OK
Content-Type: application/json

[
  {
    "id": "abcdef"
    "title": "News",
    "content": "Exciting news"
    "createdAt": "2024-09-10T09:00:00Z",
    "updatedAt": "2024-09-19T09:00:00Z"
  }
]
```
