# Medicine Store Management System API

A comprehensive .NET 8 Web API for managing medicine store operations with authentication, user management, categories, and suppliers.

## 🏗️ Project Structure

```
MedicineAppApi/
├── 📁 Common/                          # Shared utilities and common code
│   ├── 📁 Attributes/                  # Custom attributes
│   │   └── ValidateModelAttribute.cs
│   ├── 📁 Constants/                   # Application constants
│   │   └── AppConstants.cs
│   ├── 📁 Exceptions/                  # Custom exception classes
│   │   └── AppException.cs
│   ├── 📁 Extensions/                  # Extension methods
│   │   ├── ApplicationBuilderExtensions.cs
│   │   └── ServiceCollectionExtensions.cs
│   ├── 📁 Helpers/                     # Helper classes
│   │   ├── JwtHelper.cs
│   │   └── PasswordHelper.cs
│   └── 📁 Models/                      # Common models
│       ├── ApiResponse.cs
│       └── PagedResult.cs
├── 📁 Controllers/                     # API Controllers
│   ├── AuthController.cs
│   ├── CategoryController.cs
│   └── SupplierController.cs
├── 📁 Data/                           # Data access layer
│   └── ApplicationDbContext.cs
├── 📁 DTOs/                           # Data Transfer Objects
│   ├── LoginDto.cs
│   ├── CategoryDto.cs
│   └── SupplierDto.cs
├── 📁 Mapping/                         # AutoMapper profiles
│   └── AutoMapperProfile.cs
├── 📁 Migrations/                      # Entity Framework migrations
├── 📁 Models/                         # Entity models
│   ├── User.cs
│   ├── Category.cs
│   └── Supplier.cs
├── 📁 Repositories/                   # Repository pattern implementation
│   ├── 📁 Interfaces/                 # Repository interfaces
│   │   ├── IRepository.cs
│   │   ├── IUserRepository.cs
│   │   ├── ICategoryRepository.cs
│   │   └── ISupplierRepository.cs
│   └── 📁 Implementations/            # Repository implementations
│       ├── Repository.cs
│       ├── UserRepository.cs
│       ├── CategoryRepository.cs
│       └── SupplierRepository.cs
├── 📁 Services/                       # Business logic services
│   └── AuthService.cs
├── 📁 Properties/                     # Project properties
├── appsettings.json                   # Application configuration
├── appsettings.Development.json       # Development configuration
├── MedicineAppApi.csproj              # Project file
├── Program.cs                         # Application entry point
└── README.md                          # This file
```

## 🚀 Features

### ✅ Authentication & Authorization
- JWT-based authentication
- User registration and login
- Password hashing with SHA256
- Role-based access control (ready for implementation)

### ✅ User Management
- User CRUD operations
- Email validation
- User status tracking

### ✅ Category Management
- Medicine categories (antibiotics, painkillers, etc.)
- Full CRUD operations
- Category descriptions

### ✅ Supplier Management
- Vendor/supplier information
- Contact details and addresses
- Full CRUD operations

### ✅ Architecture Patterns
- **Repository Pattern** - Clean data access layer
- **AutoMapper** - Automatic object mapping
- **Dependency Injection** - Loose coupling
- **DTO Pattern** - Data transfer objects
- **Service Layer** - Business logic separation

## 🛠️ Technology Stack

- **.NET 8** - Latest .NET framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database
- **AutoMapper** - Object mapping
- **JWT** - Authentication tokens
- **Swagger/OpenAPI** - API documentation

## 📋 API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration

### Categories
- `GET /api/category` - Get all categories
- `GET /api/category/{id}` - Get category by ID
- `POST /api/category` - Create new category
- `PUT /api/category/{id}` - Update category
- `DELETE /api/category/{id}` - Delete category

### Suppliers
- `GET /api/supplier` - Get all suppliers
- `GET /api/supplier/{id}` - Get supplier by ID
- `POST /api/supplier` - Create new supplier
- `PUT /api/supplier/{id}` - Update supplier
- `DELETE /api/supplier/{id}` - Delete supplier

## 🗄️ Database Schema

### Users Table
- `Id` (Primary Key)
- `Email` (Unique)
- `Password` (Hashed)
- `FirstName`
- `LastName`
- `CreatedAt`
- `IsActive`

### Categories Table
- `Id` (Primary Key)
- `Name` (Required)
- `Description`
- `CreatedAt`
- `UpdatedAt`

### Suppliers Table
- `Id` (Primary Key)
- `Name` (Required)
- `Contact`
- `Address`
- `CreatedAt`
- `UpdatedAt`

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd MedicineAppApi
   ```

2. **Update connection string**
   Edit `appsettings.json` and update the connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=medicineAppDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;Encrypt=false"
   }
   ```

3. **Run migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access Swagger UI**
   Navigate to `https://localhost:7000/swagger` or `http://localhost:5000/swagger`

## 🔧 Configuration

### JWT Settings
```json
"Jwt": {
  "Key": "YourSuperSecretKey123!@#$%^&*()_+",
  "Issuer": "MedicineAppApi",
  "Audience": "MedicineAppApi"
}
```

### Database Connection
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=DESKTOP-0VD29SN;Database=medicineAppDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;Encrypt=false"
}
```

## 🧪 Testing

### Using Swagger UI
1. Open `https://localhost:7000/swagger`
2. Test endpoints directly from the browser
3. Use the "Try it out" feature for each endpoint

### Using Postman
1. Import the API endpoints
2. Set the base URL: `https://localhost:7000/api`
3. Test authentication first, then other endpoints

## 🔒 Security Features

- **Password Hashing** - SHA256 encryption
- **JWT Tokens** - Secure authentication
- **Input Validation** - Model validation
- **SQL Injection Protection** - Entity Framework
- **CORS Configuration** - Cross-origin requests

## 📈 Future Enhancements

- [ ] Medicine inventory management
- [ ] Sales and transactions
- [ ] Stock alerts and notifications
- [ ] Reporting and analytics
- [ ] Role-based permissions
- [ ] Audit logging
- [ ] Email notifications
- [ ] Mobile app support

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

## 📞 Support

For support and questions, please contact the development team.
