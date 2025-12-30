# Lecture: Implementing User Like Functionality

In this lecture, we implement the **user like functionality** and gain an understanding of the following concepts:

## Key Concepts Covered

1. **Many-to-Many Relationships** in Entity Framework  
2. **Configuring many-to-many relationships** using `DbContext`

---

## Many-to-Many Relationship Explained

An `AppUser` can be liked by **many** other `AppUsers`, and an `AppUser` can also **like many** other `AppUsers`.

With modern versions of **Entity Framework / .NET 5+**, EF Core has the ability to automatically create many-to-many join tables. However, in this implementation, we explicitly create and configure the join table for better control and querying.

---

## The Join Table: `UserLike`

We create a join table called **`UserLike`**, which connects users through a like relationship.

### Columns in `UserLike`
- `SourceUserId` → The user who liked someone
- `LikedUserId` → The user who was liked

This table joins the `AppUser` entity to itself and allows us to:

- Query users that a specific user has liked
- Query users who have liked a specific user

---

## Why Configuration Is Required

Up until now, we relied on **Entity Framework conventions** to generate the database automatically.

However, this is the point where **conventions are not enough**, and we must explicitly configure the relationship ourselves.

To do this, we use the **Fluent API**.

---

## Fluent API Configuration

We need to tell Entity Framework the following:

- An `AppUser` has **one SourceUser** with **many LikedUsers**
- An `AppUser` has **one LikedUser** with **many LikedByUsers**

This configuration enables Entity Framework to correctly understand and manage the self-referencing many-to-many relationship.

---
