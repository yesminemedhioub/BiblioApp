# 🚀 BiblioApp + n8n - Quick Reference

## Start Everything
```bash
docker-compose up -d
```

## Access Services

| Service | URL | Credentials |
|---------|-----|-------------|
| BiblioApp | http://localhost:8080 | admin@biblio.com / Admin@123456 |
| n8n | http://localhost:5678 | admin / admin123 |
| Swagger API | http://localhost:8080/api-docs | (use BiblioApp token) |
| SQL Server | localhost:1433 | sa / YourStrong@Password123 |

## Common Commands

### Check Services
```bash
docker-compose ps
```

### View Logs
```bash
docker-compose logs biblioapp -f          # BiblioApp
docker-compose logs biblio_n8n -f         # n8n
docker-compose logs biblio_db -f          # SQL Server
docker-compose logs biblio_postgres -f    # PostgreSQL
```

### Stop Everything
```bash
docker-compose down
```

### Clean Restart
```bash
docker-compose down -v && docker-compose up -d
```

### Rebuild App
```bash
docker-compose build --no-cache biblioapp && docker-compose up -d
```

## Send Test Notification

### 1. Get auth token
```bash
curl -X POST http://localhost:8080/api/account/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@biblio.com","password":"Admin@123456"}'
```

### 2. Send email notification
```bash
curl -X POST http://localhost:8080/api/notifications/send-email \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "email":"your@email.com",
    "subject":"Test Subject",
    "message":"Test message body"
  }'
```

## n8n Setup Quick Steps

1. **Open n8n**: http://localhost:5678
2. **Login**: admin / admin123
3. **Create Workflow**:
   - Click "New Workflow"
   - Add "Webhook" trigger
   - Set path: `/biblio-send-email`
   - Add email node (Gmail, SendGrid, etc.)
   - Map fields from webhook
   - Add "Respond to Webhook" node
4. **Activate**: Toggle "Activate" switch
5. **Test**: Make API call from step 2

## Notification API Endpoints

```bash
# Send custom email
POST /api/notifications/send-email
Body: {"email","subject","message"}

# Send SMS
POST /api/notifications/send-sms
Body: {"phoneNumber","message"}

# Borrow confirmation
POST /api/notifications/borrow-notification
Body: {"userEmail","bookTitle","dueDate"}

# Overdue reminder
POST /api/notifications/overdue-notification
Body: {"userEmail","bookTitle","daysOverdue"}

# Return reminder
POST /api/notifications/return-reminder
Body: {"userEmail","bookTitle","dueDate"}

# Health check
GET /api/notifications/health
```

## Troubleshooting

### Services won't start
```bash
# Check logs
docker-compose logs

# Clean restart
docker-compose down -v
docker-compose up -d
```

### n8n webhook not receiving requests
```bash
# Verify n8n is running
docker-compose ps | grep n8n

# Check workflow is activated
# Open n8n and look for "Activate" toggle

# Test webhook directly
curl -X POST http://localhost:5678/webhook/biblio-send-email \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","subject":"Test","message":"Test"}'
```

### Email not sending
1. Configure credentials in n8n
2. Check n8n execution logs
3. Verify email settings (Gmail 2FA, SendGrid API key, etc.)

### BiblioApp can't reach n8n
- Verify both containers are on same network: `docker-compose ps`
- Both should show network `biblio_network`

## Database Connections

### SQL Server (BiblioApp)
- Host: localhost or sqlserver (internal)
- Port: 1433
- User: sa
- Password: YourStrong@Password123
- Database: BiblioDbS

### PostgreSQL (n8n)
- Host: localhost or postgres (internal)
- Port: 5432
- User: n8n
- Password: n8n_password_123
- Database: n8n

## Files & Locations

```
/home/adam/Documents/BiblioApp/
├── docker-compose.yml              # Services config
├── Dockerfile                       # App build config
├── N8N_SETUP.md                     # Detailed n8n guide
├── README_N8N.md                    # Full documentation
├── DOCKER_SETUP.md                  # Docker guide
├── BiblioApp/
│   ├── Program.cs                   # Startup & DI
│   ├── Services/
│   │   └── NotificationService.cs   # n8n integration
│   └── Controllers/Api/
│       └── NotificationsController.cs # Notification API
└── n8n-workflows/
    ├── email-notification.json
    └── sms-notification.json
```

## Next Steps

1. ✅ Start containers: `docker-compose up -d`
2. ✅ Login to BiblioApp: http://localhost:8080
3. ✅ Setup n8n: http://localhost:5678
4. ✅ Create email workflow in n8n
5. ✅ Test with API endpoint
6. ✅ Setup SMS (optional)
7. ✅ Create scheduled workflows

## Key Features

✨ **Automated Notifications**
- 📧 Email on book borrow
- ⏰ Overdue reminders
- 📚 Return deadline alerts
- 💬 SMS notifications

✨ **n8n Workflows**
- Custom webhook triggers
- Multiple providers (Gmail, SendGrid, Twilio, etc.)
- Scheduled jobs
- Error handling & retries

✨ **API Integration**
- RESTful endpoints
- Swagger documentation
- Bearer token auth
- Logging & monitoring

---

**Happy coding! 🎉**
