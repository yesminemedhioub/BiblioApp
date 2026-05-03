# BiblioApp with n8n Integration

Complete library management system with automated email and SMS notifications via n8n workflow automation.

## 🎯 Features

### Core BiblioApp Features
- 📚 Book catalog management
- 👥 Author management
- 📅 Loan/Borrowing management
- 🔐 User authentication with roles (Admin, Librarian, Reader)
- 📊 Dashboard with statistics
- 🔍 Advanced search and filtering
- 📱 RESTful API with Swagger documentation

### n8n Automation Features
- 📧 **Email Notifications**:
  - Loan confirmation emails
  - Overdue book reminders
  - Return deadline reminders
  - Custom email notifications
  
- 💬 **SMS Notifications**:
  - Urgent overdue alerts
  - Loan confirmations
  - Custom SMS messages
  
- 🔄 **Workflow Automation**:
  - Trigger-based notifications
  - Scheduled reminders
  - Multi-step workflows
  - Error handling and retries

## 🚀 Quick Start

### Prerequisites
- Docker & Docker Compose
- 4GB+ RAM recommended
- 20GB+ free disk space

### Installation

1. **Clone and Navigate**
   ```bash
   cd /home/adam/Documents/BiblioApp
   ```

2. **Start All Services**
   ```bash
   docker-compose up -d
   ```

3. **Wait for Initialization** (first run takes 1-2 minutes)
   ```bash
   docker-compose ps
   ```

4. **Access Services**
   - BiblioApp: http://localhost:8080
   - n8n: http://localhost:5678
   - Swagger API Docs: http://localhost:8080/api-docs

### Default Credentials

**BiblioApp**:
- Email: admin@biblio.com
- Password: Admin@123456

**n8n**:
- Username: admin
- Password: admin123

## 📁 Project Structure

```
BiblioApp/
├── Dockerfile                      # Multi-stage .NET build
├── docker-compose.yml              # All services orchestration
├── .env                            # Environment configuration
├── .dockerignore                   # Docker build optimization
│
├── BiblioApp/                      # Main .NET application
│   ├── Program.cs                  # Startup & DI configuration
│   ├── BiblioApp.csproj            # Project file with dependencies
│   ├── Controllers/
│   │   ├── AccountController.cs    # Authentication
│   │   ├── LivresController.cs     # Book management
│   │   ├── AuteursController.cs    # Author management
│   │   ├── EmpruntsController.cs   # Loan management
│   │   └── Api/
│   │       ├── LivresController.cs # Book API
│   │       ├── AuteursController.cs# Author API
│   │       ├── EmpruntsController.cs# Loan API
│   │       └── NotificationsController.cs # 🆕 Notification API
│   │
│   ├── Services/
│   │   └── NotificationService.cs  # 🆕 n8n integration service
│   │
│   ├── Models/
│   │   ├── Livre.cs                # Book model
│   │   ├── Auteur.cs               # Author model
│   │   ├── Emprunt.cs              # Loan model
│   │   ├── DashboardViewModel.cs   # Dashboard data
│   │   └── ErrorViewModel.cs       # Error handling
│   │
│   ├── Data/
│   │   ├── BiblioContext.cs        # EF Core context
│   │   └── DbInitializer.cs        # Database seeding
│   │
│   ├── Views/                      # Razor templates
│   │   ├── Account/                # Auth views
│   │   ├── Livres/                 # Book views
│   │   ├── Auteurs/                # Author views
│   │   ├── Emprunts/               # Loan views
│   │   └── Shared/                 # Shared layouts
│   │
│   └── wwwroot/                    # Static files
│       ├── css/
│       ├── js/
│       └── lib/
│
├── n8n-workflows/                  # 🆕 n8n workflow templates
│   ├── email-notification.json     # Email workflow template
│   └── sms-notification.json       # SMS workflow template
│
├── Documentation/
│   ├── DOCKER_SETUP.md             # Docker configuration guide
│   ├── N8N_SETUP.md                # n8n integration guide
│   ├── API_DOCS.md                 # API documentation
│   └── README.md                   # This file

```

## 🐳 Docker Services

### Service Overview

| Service | Image | Port | Purpose |
|---------|-------|------|---------|
| **biblioapp** | .NET 8.0 ASP.NET Core | 8080 | Main application |
| **biblio_db** | SQL Server 2022 | 1433 | Application database |
| **biblio_n8n** | n8n latest | 5678 | Workflow automation |
| **biblio_postgres** | PostgreSQL 15 | 5432 | n8n database |

### Container Management

```bash
# View all containers
docker-compose ps

# View logs
docker-compose logs -f                    # All services
docker-compose logs biblioapp -f          # BiblioApp only
docker-compose logs biblio_n8n -f         # n8n only
docker-compose logs biblio_db -f          # SQL Server only
docker-compose logs biblio_postgres -f    # PostgreSQL only

# Stop services
docker-compose down

# Full clean restart
docker-compose down -v
docker-compose up -d

# Rebuild application
docker-compose build --no-cache biblioapp
docker-compose up -d
```

## 🔗 API Endpoints

### Authentication
```
POST   /api/account/login
POST   /api/account/register
POST   /api/account/logout
```

### Books
```
GET    /api/livres               # List all books
GET    /api/livres/:id           # Get book details
POST   /api/livres               # Create book
PUT    /api/livres/:id           # Update book
DELETE /api/livres/:id           # Delete book
```

### Authors
```
GET    /api/auteurs              # List all authors
GET    /api/auteurs/:id          # Get author details
POST   /api/auteurs              # Create author
PUT    /api/auteurs/:id          # Update author
DELETE /api/auteurs/:id          # Delete author
```

### Loans
```
GET    /api/emprunts             # List all loans
GET    /api/emprunts/:id         # Get loan details
POST   /api/emprunts             # Create loan
PUT    /api/emprunts/:id         # Update loan
DELETE /api/emprunts/:id         # Delete loan
GET    /api/emprunts/overdue     # Get overdue loans
```

### 🆕 Notifications
```
POST   /api/notifications/send-email              # Send custom email
POST   /api/notifications/send-sms                # Send custom SMS
POST   /api/notifications/borrow-notification     # Loan confirmation
POST   /api/notifications/overdue-notification    # Overdue reminder
POST   /api/notifications/return-reminder         # Return deadline reminder
GET    /api/notifications/health                  # Service health check
```

## 📧 Setting Up Email Notifications

### Using Gmail

1. **Enable 2FA on Gmail account**
2. **Create App Password**:
   - Go to https://myaccount.google.com/apppasswords
   - Select Mail → Windows Computer
   - Generate password
3. **Configure in n8n**:
   - Open n8n at http://localhost:5678
   - Go to Credentials → New
   - Search "Gmail"
   - Use app password
   - Save

### Using SendGrid

1. **Create SendGrid account**
2. **Generate API Key**
3. **Configure in n8n**:
   - Go to Credentials → New
   - Search "SendGrid"
   - Enter API key
   - Save

### Using Custom SMTP

1. **Get SMTP credentials** from your email provider
2. **Configure in n8n**:
   - Use "Email Send" node
   - Configure SMTP server
   - Enter credentials
   - Save

## 💬 Setting Up SMS Notifications

### Using Twilio

1. **Create Twilio account** at https://www.twilio.com
2. **Get credentials**:
   - Account SID
   - Auth Token
   - Phone Number (from Twilio)
3. **Configure in n8n**:
   - Go to Credentials → New
   - Search "Twilio"
   - Enter credentials
   - Save

### Using Plivo

1. **Create Plivo account** at https://www.plivo.com
2. **Get Auth ID and Auth Token**
3. **Configure in n8n**:
   - Go to Credentials → New
   - Search "Plivo"
   - Enter credentials
   - Save

## 🔄 Creating Workflows

### Example 1: Send Email on Loan

```bash
# 1. In BiblioApp, trigger notification:
curl -X POST http://localhost:8080/api/notifications/borrow-notification \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "userEmail": "user@example.com",
    "bookTitle": "The Great Gatsby",
    "dueDate": "2026-06-02T00:00:00Z"
  }'

# 2. n8n webhook receives request at:
# http://localhost:5678/webhook/biblio-send-email

# 3. Workflow sends email via configured provider
```

### Example 2: Scheduled Overdue Reminders

```
Trigger: Schedule (Every day at 9 AM)
  ↓
HTTP Request: Get overdue loans from BiblioApp API
  ↓
For Each Loan: 
  ↓
Send Email: Overdue reminder to user
  ↓
Log Result: Save to n8n database
```

## 🛠️ Development

### Making Code Changes

1. **Edit code locally** in your editor
2. **Rebuild application**:
   ```bash
   docker-compose build biblioapp
   ```
3. **Restart service**:
   ```bash
   docker-compose up -d biblioapp
   ```
4. **Check logs**:
   ```bash
   docker-compose logs biblioapp -f
   ```

### Debugging

```bash
# Shell into app container
docker exec -it biblio_app bash

# Run .NET CLI commands
docker exec -it biblio_app dotnet --version

# View application logs
docker logs biblio_app -f

# Check database connection
docker exec -it biblio_db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Password123 \
  -Q "SELECT 1"
```

## 📊 Monitoring & Logs

### Application Logs
```bash
docker-compose logs biblioapp -f | grep -i error
```

### n8n Workflow Logs
1. Open n8n: http://localhost:5678
2. Click on workflow
3. Click "Executions" tab
4. View execution details

### Database Logs
```bash
docker-compose logs biblio_db -f
```

### All Logs Combined
```bash
docker-compose logs -f --timestamps
```

## 🐛 Troubleshooting

### Application won't start
```bash
# Check logs
docker-compose logs biblioapp

# Full restart
docker-compose down -v
docker-compose up -d
```

### Port already in use
```bash
# Find process using port 8080
lsof -i :8080

# Kill process or change port in docker-compose.yml
```

### Database connection failed
```bash
# Check SQL Server logs
docker-compose logs biblio_db

# Verify connection string
docker exec biblio_app cat /app/appsettings.json
```

### n8n webhooks not working
```bash
# Verify n8n is running
docker-compose ps | grep n8n

# Check n8n logs
docker-compose logs biblio_n8n

# Test webhook manually
curl -X POST http://localhost:5678/webhook/biblio-send-email \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","subject":"Test","message":"Test"}'
```

## 📚 Additional Resources

- [BiblioApp GitHub](./README.md)
- [Docker Documentation](https://docs.docker.com/)
- [n8n Documentation](https://docs.n8n.io/)
- [ASP.NET Core Docs](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

## 📄 License

This project is provided as-is for educational and commercial use.

## 👥 Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues.

## 🤝 Support

For issues or questions:
1. Check the documentation files
2. Review Docker logs
3. Check n8n execution history
4. Review BiblioApp API documentation

---

**Built with ❤️ using .NET 8, Docker, n8n, and PostgreSQL**

**Happy coding! 🚀**
