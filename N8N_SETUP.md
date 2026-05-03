# n8n Workflow Integration Guide

## Overview

n8n (pronounced "n8n") is an open-source workflow automation platform that integrates with BiblioApp to handle:
- 📧 Email notifications (book borrowed, overdue reminders, return notifications)
- 💬 SMS notifications for urgent alerts
- 🔄 Automated workflows triggered from BiblioApp API

## Architecture

```
┌──────────────────┐
│   BiblioApp      │
│   .NET 8.0       │
└────────┬─────────┘
         │ HTTP POST
         ▼
┌──────────────────────────┐
│   n8n Workflows          │
│   Port 5678              │
├──────────────────────────┤
│ - Send Email             │
│ - Send SMS               │
│ - Log to Database        │
└────────┬────────┬────────┘
         │        │
         ▼        ▼
    📧 Email  💬 SMS Provider
    Service
```

## Installation & Setup

### 1. Start All Services

```bash
cd /home/adam/Documents/BiblioApp

# Stop any existing containers
docker-compose down

# Start everything (includes n8n, PostgreSQL, SQL Server, BiblioApp)
docker-compose up -d
```

### 2. Access n8n

- **URL**: http://localhost:5678
- **Username**: admin
- **Password**: admin123

### 3. Verify Services

```bash
docker-compose ps
```

Expected output:
```
biblio_app       - Running on port 8080
biblio_n8n       - Running on port 5678
biblio_db        - SQL Server on port 1433
biblio_postgres  - PostgreSQL on port 5432
```

## Creating n8n Workflows

### Workflow 1: Send Email Notification

1. **Open n8n**: http://localhost:5678
2. **Create New Workflow**:
   - Click "New Workflow"
   - Name: "Send Email Notification"

3. **Add Webhook Trigger**:
   - Click "+" to add node
   - Search: "Webhook"
   - Select "Webhook"
   - Configure:
     - HTTP Method: POST
     - Path: `/biblio-send-email`
     - Authentication: None
   - Click "Activate Webhook"

4. **Add Email Node**:
   - Click "+" to add another node
   - Search: "Email Send" (or "Gmail" / "Sendgrid" if you prefer)
   - For Gmail:
     - Click "New Credentials"
     - Authenticate with your Gmail account
     - Configure:
       - **To Email**: `{{ $json.body.email }}`
       - **Subject**: `{{ $json.body.subject }}`
       - **Text**: `{{ $json.body.message }}`

5. **Add Response Node**:
   - Click "+" to add another node
   - Search: "Respond to Webhook"
   - Configure:
     - Set response: `{{ {"success": true, "message": "Email sent"} }}`

6. **Save & Activate**:
   - Click "Save"
   - Toggle "Activate" to enable

### Workflow 2: Send SMS Notification

Follow similar steps:
1. Create webhook at `/biblio-send-sms`
2. Add SMS provider (Twilio, Plivo, etc.)
3. Map fields: phone, message
4. Add response node
5. Save & activate

### Workflow 3: Send Overdue Reminders

Create a scheduled workflow:
1. **Add Trigger**: "Schedule Trigger"
   - Type: Every X minutes (e.g., every hour)
2. **Add HTTP Request Node**:
   - Method: GET
   - URL: `http://biblioapp:8080/api/emprunts/overdue`
3. **For Each Item**:
   - Add "Item Lists" → "For Each"
   - Add "HTTP Request" node to call send-email endpoint
4. **Save & Activate**

## BiblioApp API Endpoints

### Send Email
```bash
POST /api/notifications/send-email
Authorization: Bearer {token}

{
  "email": "user@example.com",
  "subject": "Test Subject",
  "message": "Test message body"
}
```

### Send SMS
```bash
POST /api/notifications/send-sms
Authorization: Bearer {token}

{
  "phoneNumber": "+1234567890",
  "message": "Test SMS message"
}
```

### Send Borrow Notification
```bash
POST /api/notifications/borrow-notification
Authorization: Bearer {token}

{
  "userEmail": "user@example.com",
  "bookTitle": "Book Name",
  "dueDate": "2026-06-02T00:00:00Z"
}
```

### Send Overdue Notification
```bash
POST /api/notifications/overdue-notification
Authorization: Bearer {token}

{
  "userEmail": "user@example.com",
  "bookTitle": "Book Name",
  "daysOverdue": 5
}
```

### Send Return Reminder
```bash
POST /api/notifications/return-reminder
Authorization: Bearer {token}

{
  "userEmail": "user@example.com",
  "bookTitle": "Book Name",
  "dueDate": "2026-06-02T00:00:00Z"
}
```

## Using Credentials in n8n

### Gmail Setup

1. In n8n, click your profile → "Credentials"
2. Click "New Credential"
3. Search for "Gmail"
4. Click "Connect My Account"
5. Follow OAuth flow
6. Save credential

### Twilio Setup (for SMS)

1. Get API credentials from [Twilio Console](https://console.twilio.com)
2. In n8n, create new "Twilio" credential
3. Enter:
   - Account SID
   - Auth Token
   - From Phone Number
4. Save

### SendGrid Setup (Email Alternative)

1. Get API key from [SendGrid](https://sendgrid.com)
2. In n8n, create new "SendGrid" credential
3. Enter API Key
4. Save

## Testing Workflows

### Test in n8n

1. Open workflow
2. Click "Execute Workflow"
3. Scroll down to see execution logs
4. Check "Output" tab for results

### Test from BiblioApp

```bash
# Get admin token first
curl -X POST http://localhost:8080/api/account/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@biblio.com","password":"Admin@123456"}'

# Send test email
curl -X POST http://localhost:8080/api/notifications/send-email \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email":"your@email.com",
    "subject":"Test",
    "message":"Test message"
  }'
```

## Monitoring & Logging

### View n8n Logs
```bash
docker-compose logs biblio_n8n -f
```

### View Workflow Execution History
1. Open workflow in n8n
2. Click "Executions" tab
3. View execution history and details

### View BiblioApp Logs
```bash
docker-compose logs biblioapp -f
```

## Environment Variables for n8n

Edit in `docker-compose.yml`:

```yaml
environment:
  - N8N_BASIC_AUTH_ACTIVE=true
  - N8N_BASIC_AUTH_USER=admin
  - N8N_BASIC_AUTH_PASSWORD=your_secure_password
  - N8N_HOST=localhost
  - N8N_PORT=5678
  - WEBHOOK_URL=http://localhost:5678/
  - NODE_ENV=production
```

## Production Considerations

1. **Use Environment Variables**:
   - Store credentials in secrets
   - Use `.env` file for sensitive data

2. **Configure Custom Domain**:
   - Set `N8N_HOST` to your domain
   - Set `WEBHOOK_URL` to your domain with HTTPS

3. **Enable SSL/TLS**:
   - Use reverse proxy (nginx, traefik)
   - Enable HTTPS for webhooks

4. **Backup Workflows**:
   ```bash
   docker exec biblio_n8n n8n export:workflow
   ```

5. **Monitor Execution**:
   - Set up alerts for failed workflows
   - Review execution history regularly

## Common Issues & Solutions

### Issue: Webhook returns 404
**Solution**: Ensure workflow is activated and webhook path matches

### Issue: Email not sending
**Solution**: Check Gmail credentials, verify SMTP settings

### Issue: n8n container won't start
**Solution**: Check PostgreSQL is running, verify environment variables

### Issue: BiblioApp can't reach n8n
**Solution**: Verify both are on same Docker network, check hostname

## Example: Complete Email Workflow

```json
{
  "nodes": [
    {
      "parameters": {
        "httpMethod": "POST",
        "path": "biblio-send-email",
        "authentication": "none"
      },
      "name": "Webhook",
      "type": "n8n-nodes-base.webhook",
      "typeVersion": 1,
      "position": [250, 300]
    },
    {
      "parameters": {
        "toEmail": "={{ $json.body.email }}",
        "subject": "={{ $json.body.subject }}",
        "textPlain": "={{ $json.body.message }}"
      },
      "name": "Send Email",
      "type": "n8n-nodes-base.emailSendGmail",
      "typeVersion": 1,
      "position": [500, 300]
    },
    {
      "parameters": {
        "respondWith": "text",
        "responseBody": "{\"success\": true}"
      },
      "name": "Respond",
      "type": "n8n-nodes-base.respondToWebhook",
      "typeVersion": 1,
      "position": [750, 300]
    }
  ]
}
```

## Backup & Recovery

### Export Workflows
```bash
docker exec biblio_n8n n8n export:workflow --all --output ./workflows
```

### Import Workflows
```bash
docker exec biblio_n8n n8n import:workflow --input ./workflows
```

## Support & Resources

- [n8n Documentation](https://docs.n8n.io/)
- [n8n Community Forum](https://community.n8n.io/)
- [n8n GitHub](https://github.com/n8n-io/n8n)
- [BiblioApp API Documentation](./API_DOCS.md)

---

**Happy automating! 🚀**
