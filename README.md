# Store Website

## Overview
The **Store Website** is a web-based e-commerce platform built using **ASP.NET Core MVC**. It allows users to browse products, add items to their shopping cart, and place orders. The website features user authentication, role-based authorization, and an admin panel for managing products, categories, and orders.

---

## Features

### User Functionality
- **Browse Products**: Users can view products by category and see bestseller recommendations.
- **Shopping Cart**: Users can add products to their shopping cart and proceed to checkout.
- **Order Management**: Users can view their order history and place new orders.
- **Authentication & Authorization**:
  - **User Roles**: Users and Admin roles with specific privileges.
  - **Secure Login/Registration**: Secure user management using ASP.NET Identity.

### Admin Functionality
- **Manage Products**: Add, edit, delete, and view products.
- **Manage Categories**: Add, edit, delete, and view categories.
- **Order Management**: View all orders and manage order statuses.
- **Analytics Dashboard**: Admins can view:
  - Total revenue from all orders.
  - The bestseller product and its total quantity sold.
  - Total number of users and categories.

---

## Technologies Used
- **Backend**: ASP.NET Core MVC
- **Frontend**: Razor Views, HTML, CSS, Bootstrap
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Identity

---

## Setup Instructions

### Prerequisites
- .NET SDK 6.0 or later
- Visual Studio 2022 or any preferred IDE
- SQL Server installed and running

### Steps to Run the Application
1. **Clone the Repository**
   ```bash
   git clone https://github.com/your-repo-name/store-website.git
   cd store-website
